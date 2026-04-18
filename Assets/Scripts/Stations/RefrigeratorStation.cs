using System.Collections.Generic;
using UnityEngine;
using YesChef.Ingredients;
using YesChef.Player;

namespace YesChef.Stations
{
    /// <summary>
    /// Supplies a random raw ingredient when the player has empty hands.
    /// </summary>
    public sealed class RefrigeratorStation : StationBase
    {
        [Header("Available Ingredients")]
        [SerializeField] private IngredientData vegetableIngredient;
        [SerializeField] private IngredientData cheeseIngredient;
        [SerializeField] private IngredientData meatIngredient;

        public override void Interact(GameObject interactor)
        {
            if (!TryGetCarryController(interactor, out PlayerCarryController carryController) || carryController.HasItem())
            {
                return;
            }

            IngredientData ingredientData = GetRandomIngredientData();
            if (ingredientData == null)
            {
                return;
            }

            carryController.TryPickup(new IngredientInstance(ingredientData, IngredientProcessState.Raw));
        }
        public int testIndex;
        private IngredientData GetRandomIngredientData()
        {
            List<IngredientData> candidates = new(3);

            AddIfValid(candidates, vegetableIngredient, IngredientType.Vegetable);
            AddIfValid(candidates, cheeseIngredient, IngredientType.Cheese);
            AddIfValid(candidates, meatIngredient, IngredientType.Meat);

            if (candidates.Count == 0)
            {
                return null;
            }

            //int randomIndex = Random.Range(0, candidates.Count);
            //return candidates[randomIndex];
            return candidates[testIndex];
        }

        private static void AddIfValid(List<IngredientData> candidates, IngredientData ingredientData, IngredientType expectedType)
        {
            if (ingredientData != null && ingredientData.Type == expectedType)
            {
                candidates.Add(ingredientData);
            }
        }
    }
}
