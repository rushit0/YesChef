using System;
using System.Collections.Generic;
using UnityEngine;
using YesChef.Ingredients;

namespace YesChef.Orders
{
    /// <summary>
    /// Immutable recipe data used by the order system.
    /// ScriptableObjects are a clean fit here because orders are authored content, not scene state.
    /// </summary>
    [CreateAssetMenu(fileName = "OrderDefinition", menuName = "YesChef/Orders/Order Definition")]
    public sealed class OrderDefinition : ScriptableObject
    {
        [SerializeField] private string orderId = Guid.NewGuid().ToString("N");
        [SerializeField] private string displayName = "New Order";
        [SerializeField] private List<IngredientData> requiredIngredients = new();
        [SerializeField, Min(1)] private int scoreValue = 100;
        [SerializeField, Min(1f)] private float timeLimitSeconds = 60f;

        public string OrderId => orderId;
        public string DisplayName => displayName;
        public IReadOnlyList<IngredientData> RequiredIngredients => requiredIngredients;
        public int ScoreValue => scoreValue;
        public float TimeLimitSeconds => timeLimitSeconds;
    }
}
