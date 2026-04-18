using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using YesChef.Player;

namespace YesChef.Managers {
    /// <summary>
    /// Owns high-level game flow transitions and time scale changes.
    /// It coordinates the timer, score reset, and order reset in one predictable place.
    /// </summary>
    public sealed class GameFlowManager : MonoBehaviour {
        public enum GameState {
            StartMenu,
            Playing,
            Paused,
            GameOver
        }

        [SerializeField] private GameTimer gameTimer;
        [SerializeField] private OrderManager orderManager;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private PlayerInputReader playerInputReader;

        public GameState CurrentState { get; private set; } = GameState.StartMenu;

        public event Action<GameState> StateChanged;
        public event Action GameStarted;
        public event Action GamePaused;
        public event Action GameResumed;
        public event Action<int, bool> GameEnded;

        private void Awake() {
            ResolveDependencies();
            Time.timeScale = 1f;
        }

        private void OnEnable() {
            if (gameTimer != null) {
                gameTimer.TimerCompleted += HandleTimerCompleted;
            }
        }

        private void OnDisable() {
            if (gameTimer != null) {
                gameTimer.TimerCompleted -= HandleTimerCompleted;
            }
        }

        private void Update() {
            if (playerInputReader == null || !playerInputReader.PausePressedThisFrame) {
                return;
            }

            if (CurrentState == GameState.Playing) {
                PauseGame();
            }
            else if (CurrentState == GameState.Paused) {
                ResumeGame();
            }
        }

        public void StartGame() {
            ResolveDependencies();

            Time.timeScale = 1f;
            scoreManager?.ResetCurrentScore();
            orderManager?.Initialize(scoreManager);
            gameTimer?.StartTimer();

            SetState(GameState.Playing);
            GameStarted?.Invoke();
        }

        public void PauseGame() {
            if (CurrentState != GameState.Playing) {
                return;
            }

            gameTimer?.PauseTimer();
            SetState(GameState.Paused);
            GamePaused?.Invoke();
            Time.timeScale = 0f;
        }

        public void ResumeGame() {
            if (CurrentState != GameState.Paused) {
                return;
            }

            Time.timeScale = 1f;
            gameTimer?.ResumeTimer();
            SetState(GameState.Playing);
            GameResumed?.Invoke();
        }

        public void EndGame() {
            if (CurrentState == GameState.GameOver) {
                return;
            }

            gameTimer?.PauseTimer();
            SetState(GameState.GameOver);
            if (scoreManager != null) scoreManager.SetHighScore();
            int finalScore = scoreManager != null ? scoreManager.CurrentScore : 0;
            bool newHighScore = scoreManager != null && scoreManager.HasBeatenHighScoreThisRun;
            GameEnded?.Invoke(finalScore, newHighScore);
            Time.timeScale = 0f;
        }

        public void RestartGame() {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void HandleTimerCompleted() {
            EndGame();
        }

        private void ResolveDependencies() {
            if (gameTimer == null) {
                gameTimer = GetComponentInChildren<GameTimer>(true);
            }

            if (orderManager == null) {
                orderManager = GetComponentInChildren<OrderManager>(true);
            }

            if (scoreManager == null) {
                scoreManager = GetComponentInChildren<ScoreManager>(true);
            }

            if (playerInputReader == null) {
                playerInputReader = FindAnyObjectByType<PlayerInputReader>();
            }
        }

        private void SetState(GameState newState) {
            if (CurrentState == newState) {
                return;
            }

            CurrentState = newState;
            StateChanged?.Invoke(CurrentState);
        }
    }
}
