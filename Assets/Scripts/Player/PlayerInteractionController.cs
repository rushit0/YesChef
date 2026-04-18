using System.Collections.Generic;
using UnityEngine;
using YesChef.Core.Interfaces;
using YesChef.UI;

namespace YesChef.Player
{
    /// <summary>
    /// Detects nearby contextual-action sources and drives the shared popup UI.
    /// This replaces keyboard-driven interaction with proximity-based clickable actions.
    /// </summary>
    [RequireComponent(typeof(PlayerCarryController))]
    public sealed class PlayerInteractionController : MonoBehaviour
    {
        [SerializeField] private ContextPopupUI contextPopupUI;

        private readonly Dictionary<IContextActionSource, int> nearbySources = new();
        private readonly List<ContextActionData> actionsBuffer = new();

        private PlayerCarryController playerCarryController;
        private IContextActionSource currentSource;

        private void Awake()
        {
            playerCarryController = GetComponent<PlayerCarryController>();

            if (contextPopupUI == null)
            {
                contextPopupUI = FindAnyObjectByType<ContextPopupUI>();
            }
        }

        private void Update()
        {
            IContextActionSource nearestSource = FindNearestSource();
            if (!ReferenceEquals(currentSource, nearestSource))
            {
                SetCurrentSource(nearestSource);
            }

            RefreshPopup();
        }

        private void OnEnable()
        {
            if (playerCarryController != null)
            {
                playerCarryController.CarriedItemChanged += HandleCarriedItemChanged;
            }
        }

        private void OnDisable()
        {
            if (playerCarryController != null)
            {
                playerCarryController.CarriedItemChanged -= HandleCarriedItemChanged;
            }

            SetCurrentSource(null);
            nearbySources.Clear();
            contextPopupUI?.Hide();
        }

        private void OnTriggerEnter(Collider other)
        {
            IContextActionSource source = FindContextActionSource(other);
            if (source == null)
            {
                return;
            }

            nearbySources.TryGetValue(source, out int count);
            nearbySources[source] = count + 1;
            RefreshPopup();
        }

        private void OnTriggerExit(Collider other)
        {
            IContextActionSource source = FindContextActionSource(other);
            if (source == null || !nearbySources.TryGetValue(source, out int count))
            {
                return;
            }

            if (count <= 1)
            {
                nearbySources.Remove(source);
            }
            else
            {
                nearbySources[source] = count - 1;
            }

            if (ReferenceEquals(currentSource, source) && !nearbySources.ContainsKey(source))
            {
                SetCurrentSource(FindNearestSource());
            }

            RefreshPopup();
        }

        private void HandleCarriedItemChanged(Ingredients.IngredientInstance _)
        {
            RefreshPopup();
        }

        private IContextActionSource FindNearestSource()
        {
            IContextActionSource nearestSource = null;
            float nearestDistanceSqr = float.MaxValue;

            foreach (IContextActionSource source in nearbySources.Keys)
            {
                if (source == null)
                {
                    continue;
                }

                Vector3 anchorPosition = source.PopupAnchor != null ? source.PopupAnchor.position : transform.position;
                float distanceSqr = (anchorPosition - transform.position).sqrMagnitude;
                if (distanceSqr >= nearestDistanceSqr)
                {
                    continue;
                }

                nearestDistanceSqr = distanceSqr;
                nearestSource = source;
            }

            return nearestSource;
        }

        private void SetCurrentSource(IContextActionSource newSource)
        {
            if (currentSource != null)
            {
                currentSource.ContextActionsChanged -= HandleSourceActionsChanged;
            }

            currentSource = newSource;

            if (currentSource != null)
            {
                currentSource.ContextActionsChanged += HandleSourceActionsChanged;
            }
        }

        private void HandleSourceActionsChanged()
        {
            RefreshPopup();
        }

        private void RefreshPopup()
        {
            if (contextPopupUI == null || currentSource == null || playerCarryController == null)
            {
                contextPopupUI?.Hide();
                return;
            }

            actionsBuffer.Clear();
            currentSource.GetContextActions(playerCarryController, actionsBuffer);

            if (actionsBuffer.Count == 0)
            {
                contextPopupUI.Hide();
                return;
            }

            contextPopupUI.Show(currentSource.PopupTitle, actionsBuffer, currentSource.PopupAnchor);
        }

        private static IContextActionSource FindContextActionSource(Collider sourceCollider)
        {
            MonoBehaviour[] behaviours = sourceCollider.GetComponentsInParent<MonoBehaviour>();
            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (behaviour is IContextActionSource contextActionSource)
                {
                    return contextActionSource;
                }
            }

            return null;
        }
    }
}
