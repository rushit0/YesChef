using UnityEngine;
using YesChef.Core.Interfaces;

namespace YesChef.Player
{
    /// <summary>
    /// Backward-compatible shim for older references.
    /// New gameplay code should depend on PlayerInteractionController directly.
    /// </summary>
    [RequireComponent(typeof(PlayerInteractionController))]
    public sealed class PlayerInteractor : MonoBehaviour
    {
        private PlayerInteractionController interactionController;

        private void Awake()
        {
            interactionController = GetComponent<PlayerInteractionController>();
        }

        public bool TryGetHeldItem(out IHoldable holdable)
        {
            return interactionController.TryGetHeldItem(out holdable);
        }

        public bool TryPickup(IHoldable holdable)
        {
            return interactionController.TryPickup(holdable);
        }

        public void ReleaseHeldItem(Vector3 worldPosition)
        {
            interactionController.ReleaseHeldItem(worldPosition);
        }
    }
}
