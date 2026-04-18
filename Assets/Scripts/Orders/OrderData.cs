using System.Collections.Generic;
using UnityEngine;
using YesChef.Ingredients;

namespace YesChef.Orders
{
    /// <summary>
    /// Runtime order state owned by a customer window.
    /// </summary>
    public sealed class OrderData
    {
        public sealed class RequiredIngredient
        {
            public RequiredIngredient(IngredientData ingredientData)
            {
                IngredientData = ingredientData;
                RequiredState = ingredientData != null && ingredientData.RequiresProcessing
                    ? IngredientProcessState.Prepared
                    : IngredientProcessState.Raw;
            }

            public IngredientData IngredientData { get; }
            public IngredientType Type => IngredientData.Type;
            public IngredientProcessState RequiredState { get; }
            public int ScoreValue => Type switch
            {
                IngredientType.Vegetable => 20,
                IngredientType.Cheese => 10,
                IngredientType.Meat => 30,
                _ => 0
            };

            public bool Matches(IngredientInstance ingredient)
            {
                return ingredient != null &&
                    ingredient.Data != null &&
                    ingredient.Data.Type == Type &&
                    ingredient.State == RequiredState;
            }
        }

        private readonly List<RequiredIngredient> remainingIngredients;
        private readonly int totalIngredientValue;

        public OrderData(List<RequiredIngredient> requiredIngredients)
        {
            remainingIngredients = requiredIngredients ?? new List<RequiredIngredient>();

            foreach (RequiredIngredient ingredient in remainingIngredients)
            {
                totalIngredientValue += ingredient?.ScoreValue ?? 0;
            }
        }

        public IReadOnlyList<RequiredIngredient> RequiredIngredients => remainingIngredients;
        public float OpenTime { get; private set; }
        public bool IsComplete => remainingIngredients.Count == 0;

        public void AdvanceTime(float deltaTime)
        {
            if (!IsComplete)
            {
                OpenTime += Mathf.Max(0f, deltaTime);
            }
        }

        public bool TryMatchIngredient(IngredientInstance ingredient)
        {
            for (int index = 0; index < remainingIngredients.Count; index++)
            {
                RequiredIngredient requirement = remainingIngredients[index];
                if (!requirement.Matches(ingredient))
                {
                    continue;
                }

                remainingIngredients.RemoveAt(index);
                return true;
            }

            return false;
        }

        public int CalculateScore()
        {
            return totalIngredientValue - Mathf.FloorToInt(OpenTime);
        }
    }
}
