using System;
using UnityEngine;
using YesChef.Ingredients;

namespace YesChef.Player {
    /// <summary>
    /// Owns the single ingredient the chef is currently carrying.
    /// This controller is deliberately small so inventory rules stay isolated from visuals and input.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PlayerCarryController : MonoBehaviour {
        private IngredientInstance carriedItem;

        public event Action<IngredientInstance> CarriedItemChanged;

        public bool HasItem() {
            return carriedItem != null;
        }

        public bool TryPickup(IngredientInstance item) {
            if (item == null || carriedItem != null) {
                return false;
            }

            carriedItem = item;
            CarriedItemChanged?.Invoke(carriedItem);
            return true;
        }

        public IngredientInstance DropItem() {
            if (carriedItem == null) {
                return null;
            }

            IngredientInstance droppedItem = carriedItem;
            carriedItem = null;
            CarriedItemChanged?.Invoke(carriedItem);
            return droppedItem;
        }

        public IngredientInstance PeekItem() {
            return carriedItem;
        }
    }
}
