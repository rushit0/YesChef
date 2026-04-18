using System;
using UnityEngine;

namespace YesChef.Managers
{
    /// <summary>
    /// Tracks the current run score and persists the best score with PlayerPrefs.
    /// </summary>
    public sealed class ScoreManager : MonoBehaviour
    {
        private const string HighScoreKey = "YesChef.HighScore";

        public int CurrentScore { get; private set; }
        public int HighScore { get; private set; }

        public event Action<int, int> ScoreChanged;

        private void Awake()
        {
            HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
            NotifyScoreChanged();
        }

        public void ResetCurrentScore()
        {
            CurrentScore = 0;
            NotifyScoreChanged();
        }

        public void AddScore(int amount)
        {
            CurrentScore += amount;

            if (CurrentScore > HighScore)
            {
                HighScore = CurrentScore;
                PlayerPrefs.SetInt(HighScoreKey, HighScore);
                PlayerPrefs.Save();
            }

            NotifyScoreChanged();
        }

        private void NotifyScoreChanged()
        {
            ScoreChanged?.Invoke(CurrentScore, HighScore);
        }
    }
}
