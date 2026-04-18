using UnityEngine;
using YesChef.Player;

namespace YesChef.Ingredients {
    /// <summary>
    /// Handles the carried ingredient's spawned visual in the player's hand socket.
    /// Presentation stays here so the carry controller can remain a pure gameplay state holder.
    /// </summary>
    [RequireComponent(typeof(PlayerCarryController))]
    public sealed class IngredientVisual : MonoBehaviour {
        [SerializeField] private Transform handSocket;

        private PlayerCarryController carryController;
        private IngredientInstance observedItem;
        private GameObject spawnedVisual;

        private void Awake() {
            carryController = GetComponent<PlayerCarryController>();
        }

        private void OnEnable() {
            carryController.CarriedItemChanged += HandleCarriedItemChanged;
            HandleCarriedItemChanged(carryController.PeekItem());
        }

        private void OnDisable() {
            if (carryController != null) {
                carryController.CarriedItemChanged -= HandleCarriedItemChanged;
            }

            ObserveItem(null);
            ClearVisual();
        }

        private void HandleCarriedItemChanged(IngredientInstance item) {
            ObserveItem(item);
            RefreshVisual();
        }

        private void HandleObservedItemStateChanged(IngredientProcessState _) {
            RefreshVisual();
        }

        private void ObserveItem(IngredientInstance item) {
            if (observedItem != null) {
                observedItem.StateChanged -= HandleObservedItemStateChanged;
            }

            observedItem = item;

            if (observedItem != null) {
                observedItem.StateChanged += HandleObservedItemStateChanged;
            }
        }

        private void RefreshVisual() {
            ClearVisual();

            if (observedItem?.Data == null) {
                return;
            }

            GameObject visualPrefab = observedItem.Data.GetPrefabForState(observedItem.State);
            if (visualPrefab == null) {
                return;
            }

            Transform socket = handSocket != null ? handSocket : transform;
            spawnedVisual = Instantiate(visualPrefab, socket);
            spawnedVisual.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            spawnedVisual.transform.localScale = Vector3.one;
        }

        private void ClearVisual() {
            if (spawnedVisual == null) {
                return;
            }

            Destroy(spawnedVisual);
            spawnedVisual = null;
        }
    }
}
