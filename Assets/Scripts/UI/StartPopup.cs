using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YesChef.Core;
using YesChef.Managers;

namespace YesChef.UI
{
    /// <summary>
    /// Start menu popup that explains controls and begins the round.
    /// </summary>
    public sealed class StartPopup : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TMP_Text controlsLabel;
        [SerializeField] private Button startButton;
        [SerializeField] private TMP_Text startButtonLabel;

        private void Awake()
        {
            canvasGroup ??= GetComponent<CanvasGroup>();
            if (startButtonLabel == null && startButton != null)
            {
                startButtonLabel = startButton.GetComponentInChildren<TMP_Text>(true);
            }

            if (controlsLabel != null)
            {
                controlsLabel.text = "Move: WASD / Arrow Keys\nInteract: Context Buttons\nPause: Esc";
            }
        }

        private void OnEnable()
        {
            if (startButton != null)
            {
                startButton.onClick.AddListener(HandleStartClicked);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameFlowManager.StateChanged += HandleStateChanged;
                HandleStateChanged(GameManager.Instance.GameFlowManager.CurrentState);
            }
        }

        private void OnDisable()
        {
            if (startButton != null)
            {
                startButton.onClick.RemoveListener(HandleStartClicked);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameFlowManager.StateChanged -= HandleStateChanged;
            }
        }

        private void HandleStartClicked()
        {
            if (GameManager.Instance == null)
            {
                return;
            }

            GameFlowManager gameFlowManager = GameManager.Instance.GameFlowManager;
            if (gameFlowManager.CurrentState == GameFlowManager.GameState.StartMenu)
            {
                gameFlowManager.StartGame();
            }
            else if (gameFlowManager.CurrentState == GameFlowManager.GameState.Paused)
            {
                gameFlowManager.ResumeGame();
            }
        }

        private void HandleStateChanged(GameFlowManager.GameState state)
        {
            bool shouldShow = state == GameFlowManager.GameState.StartMenu || state == GameFlowManager.GameState.Paused;
            SetVisible(shouldShow);

            if (startButtonLabel != null)
            {
                startButtonLabel.text = state == GameFlowManager.GameState.Paused ? "Resume" : "Start";
            }
        }

        private void SetVisible(bool isVisible)
        {
            if (canvasGroup == null)
            {
                return;
            }

            canvasGroup.alpha = isVisible ? 1f : 0f;
            canvasGroup.interactable = isVisible;
            canvasGroup.blocksRaycasts = isVisible;
        }
    }
}
