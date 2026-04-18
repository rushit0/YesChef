using UnityEngine;
using YesChef.Player;

namespace YesChef.Stations
{
    /// <summary>
    /// Permanently removes the currently held ingredient from the player's hands.
    /// </summary>
    public sealed class TrashStation : StationBase
    {
        public override void Interact(GameObject interactor)
        {
            if (!TryGetCarryController(interactor, out PlayerCarryController carryController))
            {
                return;
            }

            if (!carryController.HasItem())
            {
                return;
            }

            carryController.DropItem();
        }
    }
}
