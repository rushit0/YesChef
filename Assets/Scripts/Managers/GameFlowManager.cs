using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace YesChef.Managers
{
    /// <summary>
    /// Owns high-level game flow transitions and time scale changes.
    /// It coordinates the timer, score reset, and order reset in one predictable place.
    /// </summary>
    public sealed class GameFlowManager : MonoBehaviour
    {
        public enum GameState
        {
            StartMenu,
            Playing,
            Paused,
            GameOver
        }

        [SerializeField] private GameTimer gameTimer;
        [SerializeField] private OrderManager orderManager;
        [SerializeField] private ScoreManager scoreManager;

        public GameState CurrentState { get; private set; } = GameState.StartMenu;

        public event Action<GameState> StateChanged;
        public event Action GameStarted;
        public event Action GamePaused;
        public event Action GameResumed;
        public event Action<int, bool> GameEnded;

        private void Awake()
        {
            ResolveDependencies();
            Time.timeScale = 1f;
        }

        private void OnEnable()
        {
            if (gameTimer != null)
            {
                gameTimer.TimerCompleted += HandleTimerCompleted;
            }
        }

        private void OnDisable()
        {
            if (gameTimer != null)
            {
                gameTimer.TimerCompleted -= HandleTimerCompleted;
            }
        }

        public void StartGame()
        {
            ResolveDependencies();

            Time.timeScale = 1f;
            scoreManager?.ResetCurrentScore();
            orderManager?.Initialize(scoreManager);
            gameTimer?.StartTimer();

            SetState(GameState.Playing);
            GameStarted?.Invoke();
        }

        public void PauseGame()
        {
            if (CurrentState != GameState.Playing)
            {
                return;
            }

            Time.timeScale = 0f;
            gameTimer?.PauseTimer();
            SetState(GameState.Paused);
            GamePaused?.Invoke();
        }

        public void ResumeGame()
        {
            if (CurrentState != GameState.Paused)
            {
                return;
            }

            Time.timeScale = 1f;
            gameTimer?.ResumeTimer();
            SetState(GameState.Playing);
            GameResumed?.Invoke();
        }

        public void EndGame()
        {
            if (CurrentState == GameState.GameOver)
            {
                return;
            }

            Time.timeScale = 0f;
            gameTimer?.PauseTimer();
            SetState(GameState.GameOver);

            int finalScore = scoreManager != null ? scoreManager.CurrentScore : 0;
            bool newHighScore = scoreManager != null && scoreManager.HasBeatenHighScoreThisRun;
            GameEnded?.Invoke(finalScore, newHighScore);
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void HandleTimerCompleted()
        {
            EndGame();
        }

        private void ResolveDependencies()
        {
            if (gameTimer == null)
            {
                gameTimer = GetComponentInChildren<GameTimer>(true);
            }

            if (orderManager == null)
            {
                orderManager = GetComponentInChildren<OrderManager>(true);
            }

            if (scoreManager == null)
            {
                scoreManager = GetComponentInChildren<ScoreManager>(true);
            }
        }

        private void SetState(GameState newState)
        {
            if (CurrentState == newState)
            {
                return;
            }

            CurrentState = newState;
            StateChanged?.Invoke(CurrentState);
        }
    }
}
