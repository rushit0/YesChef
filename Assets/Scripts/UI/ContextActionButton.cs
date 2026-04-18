using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace YesChef.UI {
    /// <summary>
    /// Reusable popup button view for one contextual action.
    /// </summary>
    public sealed class ContextActionButton : MonoBehaviour {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text label;

        public void Bind(string actionLabel, bool isEnabled, UnityAction callback) {
            if (label != null) {
                label.text = actionLabel;
            }

            if (button == null) {
                return;
            }

            button.onClick.RemoveAllListeners();
            button.interactable = isEnabled;

            if (isEnabled && callback != null) {
                button.onClick.AddListener(callback);
            }
        }
    }
}
