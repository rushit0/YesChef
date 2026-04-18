using System.Collections.Generic;
using TMPro;
using UnityEngine;
using YesChef.Core.Interfaces;

namespace YesChef.UI {
    /// <summary>
    /// Renders a contextual popup near the active station or window.
    /// </summary>
    public sealed class ContextPopupUI : MonoBehaviour {
        [SerializeField] private RectTransform panelRoot;
        [SerializeField] private TMP_Text titleLabel;
        [SerializeField] private RectTransform buttonContainer;
        [SerializeField] private ContextActionButton buttonPrefab;
        [SerializeField] private Vector3 panelOffset;
        [SerializeField] private Camera worldCamera;

        private readonly List<ContextActionButton> buttonPool = new();

        private Transform targetAnchor;
        private bool isVisible;

        private void Awake() {
            Hide();
        }

        private void LateUpdate() {
            if (!isVisible || targetAnchor == null || panelRoot == null) {
                return;
            }

            Camera cameraToUse = worldCamera != null ? worldCamera : Camera.main;
            if (cameraToUse == null) {
                return;
            }

            transform.position = targetAnchor.position + panelOffset;
        }

        public void Show(string title, IReadOnlyList<ContextActionData> actions, Transform popupAnchor) {
            if (panelRoot == null || buttonContainer == null || buttonPrefab == null) {
                return;
            }

            targetAnchor = popupAnchor;
            isVisible = true;
            panelRoot.gameObject.SetActive(true);

            if (titleLabel != null) {
                titleLabel.text = title;
            }

            EnsureButtonPool(actions.Count);
            for (int index = 0; index < buttonPool.Count; index++) {
                bool shouldShow = index < actions.Count;
                buttonPool[index].gameObject.SetActive(shouldShow);

                if (!shouldShow) {
                    continue;
                }

                ContextActionData action = actions[index];
                buttonPool[index].Bind(action.Label, action.IsEnabled, () => action.Execute?.Invoke());
            }
        }

        public void Hide() {
            isVisible = false;
            targetAnchor = null;

            if (panelRoot != null) {
                panelRoot.gameObject.SetActive(false);
            }
        }

        private void EnsureButtonPool(int requiredCount) {
            while (buttonPool.Count < requiredCount) {
                ContextActionButton buttonInstance = Instantiate(buttonPrefab, buttonContainer);
                buttonPool.Add(buttonInstance);
            }
        }
    }
}
