using UnityEngine;
using YesChef.Core.Interfaces;

namespace YesChef.Player
{
    /// <summary>
    /// Handles player-facing interaction and held-item ownership.
    /// Keeping the player logic thin makes it easier to swap input systems without touching kitchen domain code.
    /// </summary>
    public sealed class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private Transform holdPoint;
        [SerializeField, Min(0.1f)] private float interactionDistance = 2f;
        [SerializeField] private LayerMask interactionLayers = ~0;

        private IHoldable heldItem;

        public bool TryGetHeldItem(out IHoldable holdable)
        {
            holdable = heldItem;
            return holdable != null;
        }

        public bool TryPickup(IHoldable holdable)
        {
            if (holdable == null || heldItem != null || holdPoint == null)
            {
                return false;
            }

            heldItem = holdable;
            heldItem.OnPickedUp(holdPoint);
            return true;
        }

        public void ReleaseHeldItem(Vector3 worldPosition)
        {
            if (heldItem == null)
            {
                return;
            }

            heldItem.OnDropped(worldPosition);
            heldItem = null;
        }

        public bool TryInteractForward()
        {
            if (!Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, interactionDistance, interactionLayers))
            {
                return false;
            }

            IInteractable interactable = FindInteractable(hit.collider);
            if (interactable == null || !interactable.CanInteract(gameObject))
            {
                return false;
            }

            interactable.Interact(gameObject);
            return true;
        }

        private static IInteractable FindInteractable(Collider sourceCollider)
        {
            MonoBehaviour[] behaviours = sourceCollider.GetComponentsInParent<MonoBehaviour>();
            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (behaviour is IInteractable interactable)
                {
                    return interactable;
                }
            }

            return null;
        }
    }
}
