using UnityEngine;
using YesChef.Core.Interfaces;
using YesChef.Ingredients;
using YesChef.Player;

namespace YesChef.Stations
{
    /// <summary>
    /// Common low-level base for kitchen stations.
    /// It centralizes shared inventory helpers while leaving UI-specific behavior to higher layers.
    /// </summary>
    public abstract class StationBase : MonoBehaviour, IInteractable
    {
        public abstract void Interact(GameObject interactor);

        protected static bool TryGetCarryController(GameObject interactor, out PlayerCarryController carryController)
        {
            carryController = interactor != null ? interactor.GetComponent<PlayerCarryController>() : null;
            return carryController != null;
        }

        protected static IngredientInstance PeekHeldItem(PlayerCarryController carryController)
        {
            return carryController?.PeekItem();
        }

        protected static IngredientInstance TakeHeldItem(PlayerCarryController carryController)
        {
            return carryController?.DropItem();
        }

        protected static bool TryGiveItem(PlayerCarryController carryController, IngredientInstance item)
        {
            return carryController != null && item != null && carryController.TryPickup(item);
        }

        protected static bool IsIngredient(IngredientInstance ingredient, IngredientType type, IngredientProcessState state)
        {
            return ingredient != null && ingredient.Data.Type == type && ingredient.State == state;
        }
    }
}
