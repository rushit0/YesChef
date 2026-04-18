using UnityEngine;
using YesChef.Core.Interfaces;
using YesChef.Player;

namespace YesChef.Stations {
    /// <summary>
    /// Permanently removes the currently held ingredient from the player's hands.
    /// </summary>
    public sealed class TrashStation : BaseStationUIController {
        public override string PopupTitle => "Trash";

        public override void GetContextActions(PlayerCarryController playerCarryController, System.Collections.Generic.List<ContextActionData> actions) {
            if (playerCarryController == null || !playerCarryController.HasItem()) {
                return;
            }

            actions.Add(new ContextActionData("Trash Item", true, () => TrashHeldItem(playerCarryController)));
        }

        private void TrashHeldItem(PlayerCarryController carryController) {
            if (carryController == null || !carryController.HasItem()) {
                return;
            }

            carryController.DropItem();
            NotifyContextChanged();
        }
    }
}
