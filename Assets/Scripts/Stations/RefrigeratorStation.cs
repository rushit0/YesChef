using UnityEngine;
using YesChef.Core.Interfaces;
using YesChef.Ingredients;
using YesChef.Player;

namespace YesChef.Stations
{
    /// <summary>
    /// Offers ingredient selection buttons when the player is near the refrigerator.
    /// </summary>
    public sealed class RefrigeratorStation : BaseStationUIController
    {
        [Header("Available Ingredients")]
        [SerializeField] private IngredientData vegetableIngredient;
        [SerializeField] private IngredientData cheeseIngredient;
        [SerializeField] private IngredientData meatIngredient;

        public override string PopupTitle => "Select Ingredient";

        public override void GetContextActions(PlayerCarryController playerCarryController, System.Collections.Generic.List<ContextActionData> actions)
        {
            bool canTakeItem = playerCarryController != null && !playerCarryController.HasItem();
            AddIngredientAction(actions, playerCarryController, vegetableIngredient, "Vegetable", canTakeItem, IngredientType.Vegetable);
            AddIngredientAction(actions, playerCarryController, cheeseIngredient, "Cheese", canTakeItem, IngredientType.Cheese);
            AddIngredientAction(actions, playerCarryController, meatIngredient, "Meat", canTakeItem, IngredientType.Meat);
        }

        private void AddIngredientAction(System.Collections.Generic.List<ContextActionData> actions, PlayerCarryController carryController, IngredientData ingredientData, string label, bool canTakeItem, IngredientType expectedType)
        {
            if (ingredientData == null || ingredientData.Type != expectedType)
            {
                return;
            }

            string buttonLabel = canTakeItem ? label : $"{label} (Hands Full)";
            actions.Add(new ContextActionData(buttonLabel, canTakeItem, () => GiveIngredient(carryController, ingredientData)));
        }

        private void GiveIngredient(PlayerCarryController carryController, IngredientData ingredientData)
        {
            if (carryController == null || ingredientData == null || carryController.HasItem())
            {
                return;
            }

            carryController.TryPickup(new IngredientInstance(ingredientData, IngredientProcessState.Raw));
            NotifyContextChanged();
        }
    }
}
