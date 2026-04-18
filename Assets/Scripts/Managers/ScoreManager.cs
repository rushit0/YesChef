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

        private int startingHighScore;

        public int CurrentScore { get; private set; }
        public int HighScore { get; private set; }
        public bool HasBeatenHighScoreThisRun { get; private set; }

        public event Action<int, int> ScoreChanged;

        private void Awake()
        {
            HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
            startingHighScore = HighScore;
            NotifyScoreChanged();
        }

        public void ResetCurrentScore()
        {
            CurrentScore = 0;
            startingHighScore = HighScore;
            HasBeatenHighScoreThisRun = false;
            NotifyScoreChanged();
        }

        public void AddScore(int amount)
        {
            CurrentScore += amount;

            if (CurrentScore > HighScore)
            {
                HighScore = CurrentScore;
                HasBeatenHighScoreThisRun = HighScore > startingHighScore;
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
