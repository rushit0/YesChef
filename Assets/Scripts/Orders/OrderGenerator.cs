using System.Collections.Generic;
using UnityEngine;
using YesChef.Ingredients;

namespace YesChef.Orders
{
    /// <summary>
    /// Generates random runtime orders using the available ingredient catalog.
    /// </summary>
    public sealed class OrderGenerator : MonoBehaviour
    {
        [SerializeField] private IngredientData vegetableIngredient;
        [SerializeField] private IngredientData cheeseIngredient;
        [SerializeField] private IngredientData meatIngredient;

        public OrderData GenerateRandomOrder()
        {
            List<IngredientData> catalog = BuildCatalog();
            if (catalog.Count == 0)
            {
                return null;
            }

            int ingredientCount = Random.value < 0.5f ? 2 : 3;
            List<OrderData.RequiredIngredient> requirements = new(ingredientCount);

            for (int index = 0; index < ingredientCount; index++)
            {
                IngredientData ingredientData = catalog[Random.Range(0, catalog.Count)];
                requirements.Add(new OrderData.RequiredIngredient(ingredientData));
            }

            return new OrderData(requirements);
        }

        private List<IngredientData> BuildCatalog()
        {
            List<IngredientData> catalog = new(3);
            AddIfValid(catalog, vegetableIngredient, IngredientType.Vegetable);
            AddIfValid(catalog, cheeseIngredient, IngredientType.Cheese);
            AddIfValid(catalog, meatIngredient, IngredientType.Meat);
            return catalog;
        }

        private static void AddIfValid(List<IngredientData> catalog, IngredientData ingredientData, IngredientType expectedType)
        {
            if (ingredientData != null && ingredientData.Type == expectedType)
            {
                catalog.Add(ingredientData);
            }
        }
    }
}
