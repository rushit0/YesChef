using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YesChef.Ingredients;

namespace YesChef.UI {
    /// <summary>
    /// Visualizes one ingredient requirement inside a customer order.
    /// </summary>
    public sealed class OrderIngredientIconView : MonoBehaviour {
        [SerializeField] private Image iconImage;

        public void Bind(Sprite icon, IngredientProcessState requiredState) {
            if (iconImage != null) {
                iconImage.sprite = icon;
                iconImage.enabled = icon != null;
            }

        }
    }
}
