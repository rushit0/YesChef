using TMPro;
using UnityEngine;
using YesChef.Core;

namespace YesChef.UI
{
    /// <summary>
    /// Displays live match information during gameplay.
    /// It only reacts to manager events and formats text for the player.
    /// </summary>
    public sealed class HUDController : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreLabel;
        [SerializeField] private TMP_Text highScoreLabel;
        [SerializeField] private TMP_Text remainingTimeLabel;

        private void OnEnable()
        {
            if (GameManager.Instance == null)
            {
                return;
            }

            GameManager.Instance.ScoreManager.ScoreChanged += HandleScoreChanged;
            GameManager.Instance.GameTimer.TimeChanged += HandleTimeChanged;

            HandleScoreChanged(GameManager.Instance.ScoreManager.CurrentScore, GameManager.Instance.ScoreManager.HighScore);
            HandleTimeChanged(GameManager.Instance.GameTimer.RemainingTimeSeconds);
        }

        private void OnDisable()
        {
            if (GameManager.Instance == null)
            {
                return;
            }

            GameManager.Instance.ScoreManager.ScoreChanged -= HandleScoreChanged;
            GameManager.Instance.GameTimer.TimeChanged -= HandleTimeChanged;
        }

        private void HandleScoreChanged(int currentScore, int highScore)
        {
            if (scoreLabel != null)
            {
                scoreLabel.text = $"Score: {currentScore}";
            }

            if (highScoreLabel != null)
            {
                highScoreLabel.text = $"High: {highScore}";
            }
        }

        private void HandleTimeChanged(float remainingSeconds)
        {
            if (remainingTimeLabel == null)
            {
                return;
            }

            int totalSeconds = Mathf.CeilToInt(Mathf.Max(0f, remainingSeconds));
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            remainingTimeLabel.text = $"{minutes:00}:{seconds:00}";
        }
    }
}
