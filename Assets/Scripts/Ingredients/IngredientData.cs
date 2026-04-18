using UnityEngine;

namespace YesChef.Ingredients {
    /// <summary>
    /// Authoring asset for a kitchen ingredient.
    /// Data is kept in a ScriptableObject so runtime instances stay lightweight and easy to duplicate.
    /// </summary>
    [CreateAssetMenu(fileName = "IngredientData", menuName = "YesChef/Ingredients/Ingredient Data")]
    public sealed class IngredientData : ScriptableObject {
        [SerializeField] private string ingredientName = "Ingredient";
        [SerializeField] private IngredientType type = IngredientType.Vegetable;
        [SerializeField] private Sprite icon;
        [SerializeField, Min(0)] private int scoreValue = 10;
        [SerializeField] private bool requiresProcessing;
        [SerializeField] private GameObject rawPrefab;
        [SerializeField] private GameObject preparedPrefab;

        public string IngredientName => ingredientName;
        public IngredientType Type => type;
        public Sprite Icon => icon;
        public int ScoreValue => scoreValue;
        public bool RequiresProcessing => requiresProcessing;
        public GameObject RawPrefab => rawPrefab;
        public GameObject PreparedPrefab => preparedPrefab;

        /// <summary>
        /// Resolves the correct presentation prefab for the current ingredient state.
        /// If a prepared prefab is missing, the raw visual is used as a safe fallback.
        /// </summary>
        public GameObject GetPrefabForState(IngredientProcessState state) {
            return state == IngredientProcessState.Prepared && preparedPrefab != null
                ? preparedPrefab
                : rawPrefab;
        }
    }
}
