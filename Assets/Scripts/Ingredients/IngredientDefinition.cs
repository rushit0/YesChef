using UnityEngine;

namespace YesChef.Ingredients
{
    /// <summary>
    /// Authoring data for an ingredient.
    /// Visuals and gameplay values live in data assets so kitchen objects remain reusable and testable.
    /// </summary>
    [CreateAssetMenu(fileName = "IngredientDefinition", menuName = "YesChef/Ingredients/Ingredient Definition")]
    public sealed class IngredientDefinition : ScriptableObject
    {
        [SerializeField] private string ingredientId = "ingredient-id";
        [SerializeField] private string displayName = "Ingredient";
        [SerializeField] private Sprite icon;
        [SerializeField, Min(0f)] private float baseProcessDuration = 3f;

        public string IngredientId => ingredientId;
        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public float BaseProcessDuration => baseProcessDuration;
    }
}
