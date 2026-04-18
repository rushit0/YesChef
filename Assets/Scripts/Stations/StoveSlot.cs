using UnityEngine;
using YesChef.Ingredients;

namespace YesChef.Stations {
    /// <summary>
    /// Runtime slot state for the stove.
    /// Each slot cooks one ingredient independently.
    /// </summary>
    [System.Serializable]
    public sealed class StoveSlot {
        private float totalCookTime;

        public IngredientInstance Item { get; private set; }
        public float RemainingCookTime { get; private set; }
        public float TotalCookTime => totalCookTime;
        public bool IsCooking => Item != null && RemainingCookTime > 0f;
        public bool IsReady => Item != null && RemainingCookTime <= 0f;
        public float ProgressNormalized => Item == null || totalCookTime <= 0f
            ? 0f
            : Mathf.Clamp01(1f - (RemainingCookTime / totalCookTime));

        public bool IsEmpty() {
            return Item == null;
        }

        public void Assign(IngredientInstance item, float cookTimeSeconds) {
            Item = item;
            totalCookTime = cookTimeSeconds;
            RemainingCookTime = cookTimeSeconds;
        }

        public void Tick(float deltaTime) {
            if (!IsCooking) {
                return;
            }

            RemainingCookTime = Mathf.Max(0f, RemainingCookTime - deltaTime);
        }

        public IngredientInstance Remove() {
            IngredientInstance item = Item;
            Item = null;
            RemainingCookTime = 0f;
            totalCookTime = 0f;
            return item;
        }
    }
}
