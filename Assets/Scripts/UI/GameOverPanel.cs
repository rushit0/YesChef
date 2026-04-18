using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YesChef.Core;
using YesChef.Managers;

namespace YesChef.UI {
    /// <summary>
    /// Displays end-of-match results and restart controls.
    /// </summary>
    public sealed class GameOverPanel : MonoBehaviour {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TMP_Text finalScoreLabel;
        [SerializeField] private TMP_Text newHighScoreLabel;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button quitButton;

        private void Awake() {
            canvasGroup ??= GetComponent<CanvasGroup>();
        }

        private void OnEnable() {
            if (restartButton != null) {
                restartButton.onClick.AddListener(HandleRestartClicked);
            }

            if (quitButton != null) {
                quitButton.onClick.AddListener(HandleQuitClicked);
            }

            if (GameManager.Instance != null) {
                GameManager.Instance.GameFlowManager.GameEnded += HandleGameEnded;
                GameManager.Instance.GameFlowManager.StateChanged += HandleStateChanged;
                HandleStateChanged(GameManager.Instance.GameFlowManager.CurrentState);
            }
        }

        private void OnDisable() {
            if (restartButton != null) {
                restartButton.onClick.RemoveListener(HandleRestartClicked);
            }

            if (quitButton != null) {
                quitButton.onClick.RemoveListener(HandleQuitClicked);
            }

            if (GameManager.Instance != null) {
                GameManager.Instance.GameFlowManager.GameEnded -= HandleGameEnded;
                GameManager.Instance.GameFlowManager.StateChanged -= HandleStateChanged;
            }
        }

        private void HandleGameEnded(int finalScore, bool newHighScore) {
            if (finalScoreLabel != null) {
                finalScoreLabel.text = $"Final Score: {finalScore}";
            }

            if (newHighScoreLabel != null) {
                newHighScoreLabel.gameObject.SetActive(newHighScore);
                newHighScoreLabel.text = "New High Score!";
            }
        }

        private void HandleRestartClicked() {
            GameManager.Instance?.GameFlowManager.RestartGame();
        }

        private void HandleQuitClicked() {
            Application.Quit();
        }

        private void HandleStateChanged(GameFlowManager.GameState state) {
            SetVisible(state == GameFlowManager.GameState.GameOver);
        }

        private void SetVisible(bool isVisible) {
            if (canvasGroup == null) {
                return;
            }

            canvasGroup.alpha = isVisible ? 1f : 0f;
            canvasGroup.interactable = isVisible;
            canvasGroup.blocksRaycasts = isVisible;
        }
    }
}
