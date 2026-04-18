using UnityEngine;
using YesChef.Core.Interfaces;

namespace YesChef.Player
{
    /// <summary>
    /// Finds the closest interactable in range and invokes it when the interact input is pressed.
    /// It only handles interaction selection, leaving carry state to PlayerCarryController.
    /// </summary>
    [RequireComponent(typeof(PlayerInputReader))]
    public sealed class PlayerInteractionController : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float interactionRadius = 1.5f;
        [SerializeField] private LayerMask interactionLayers = ~0;

        private readonly Collider[] overlapResults = new Collider[16];

        private PlayerInputReader inputReader;

        private void Awake()
        {
            inputReader = GetComponent<PlayerInputReader>();
        }

        private void Update()
        {
            if (!inputReader.InteractPressedThisFrame)
            {
                return;
            }

            IInteractable nearestInteractable = FindNearestInteractable();
            nearestInteractable?.Interact(gameObject);
        }

        private IInteractable FindNearestInteractable()
        {
            int hitCount = Physics.OverlapSphereNonAlloc(
                transform.position,
                interactionRadius,
                overlapResults,
                interactionLayers,
                QueryTriggerInteraction.Collide);

            IInteractable nearestInteractable = null;
            float nearestDistanceSqr = float.MaxValue;

            for (int index = 0; index < hitCount; index++)
            {
                Collider hitCollider = overlapResults[index];
                if (hitCollider == null)
                {
                    continue;
                }

                IInteractable interactable = FindInteractable(hitCollider);
                if (interactable == null)
                {
                    continue;
                }

                Vector3 offset = hitCollider.ClosestPoint(transform.position) - transform.position;
                float distanceSqr = offset.sqrMagnitude;
                if (distanceSqr >= nearestDistanceSqr)
                {
                    continue;
                }

                nearestDistanceSqr = distanceSqr;
                nearestInteractable = interactable;
            }

            return nearestInteractable;
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

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.65f, 0f, 0.8f);
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }
#endif
    }
}
