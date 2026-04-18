using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using YesChef.Core.Interfaces;
using YesChef.Ingredients;
using YesChef.Managers;
using YesChef.Player;

namespace YesChef.Orders
{
    /// <summary>
    /// Customer-facing delivery window that owns one active order at a time.
    /// </summary>
    public sealed class CustomerWindow : MonoBehaviour, IInteractable
    {
        private const float RespawnDelaySeconds = 5f;

        [Header("Optional UI")]
        [SerializeField] private TMP_Text orderLabel;
        [SerializeField] private TMP_Text timerLabel;
        [SerializeField] private TMP_Text statusLabel;

        private readonly StringBuilder orderDescriptionBuilder = new();

        private ScoreManager scoreManager;
        private Coroutine respawnRoutine;

        public OrderData CurrentOrder { get; private set; }

        public event Action<CustomerWindow> RespawnRequested;

        public void Initialize(ScoreManager manager)
        {
            scoreManager = manager;
        }

        public void SetOrder(OrderData order)
        {
            if (respawnRoutine != null)
            {
                StopCoroutine(respawnRoutine);
                respawnRoutine = null;
            }

            CurrentOrder = order;
            RefreshDisplay("Waiting");
        }

        public void Interact(GameObject interactor)
        {
            if (CurrentOrder == null || CurrentOrder.IsComplete)
            {
                return;
            }

            PlayerCarryController carryController = interactor != null ? interactor.GetComponent<PlayerCarryController>() : null;
            IngredientInstance heldItem = carryController?.PeekItem();
            if (heldItem == null || !CurrentOrder.TryMatchIngredient(heldItem))
            {
                return;
            }

            carryController.DropItem();

            if (CurrentOrder.IsComplete)
            {
                CompleteOrder();
                return;
            }

            RefreshDisplay("Delivered");
        }

        private void Update()
        {
            if (CurrentOrder == null || CurrentOrder.IsComplete)
            {
                return;
            }

            CurrentOrder.AdvanceTime(Time.deltaTime);
            UpdateTimerLabel();
        }

        private void CompleteOrder()
        {
            int score = CurrentOrder.CalculateScore();
            scoreManager?.AddScore(score);
            RefreshDisplay($"Complete ({score:+#;-#;0})");
            respawnRoutine = StartCoroutine(RequestRespawnRoutine());
        }

        private IEnumerator RequestRespawnRoutine()
        {
            yield return new WaitForSeconds(RespawnDelaySeconds);

            CurrentOrder = null;
            respawnRoutine = null;
            RefreshDisplay("Next customer");
            RespawnRequested?.Invoke(this);
        }

        private void RefreshDisplay(string status)
        {
            if (orderLabel != null)
            {
                orderLabel.text = BuildOrderDescription();
            }

            if (statusLabel != null)
            {
                statusLabel.text = status;
            }

            UpdateTimerLabel();
        }

        private void UpdateTimerLabel()
        {
            if (timerLabel == null)
            {
                return;
            }

            timerLabel.text = CurrentOrder == null
                ? "Time: --"
                : $"Time: {CurrentOrder.OpenTime:0.0}s";
        }

        private string BuildOrderDescription()
        {
            if (CurrentOrder == null)
            {
                return "No Order";
            }

            orderDescriptionBuilder.Clear();

            for (int index = 0; index < CurrentOrder.RequiredIngredients.Count; index++)
            {
                if (index > 0)
                {
                    orderDescriptionBuilder.AppendLine();
                }

                OrderData.RequiredIngredient requirement = CurrentOrder.RequiredIngredients[index];
                orderDescriptionBuilder.Append(requirement.Type);
                if (requirement.RequiredState == IngredientProcessState.Prepared)
                {
                    orderDescriptionBuilder.Append(" (Prepared)");
                }
            }

            return orderDescriptionBuilder.ToString();
        }
    }
}
