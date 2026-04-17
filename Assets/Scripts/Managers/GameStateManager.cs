using System;
using UnityEngine;

namespace YesChef.Managers
{
    /// <summary>
    /// Owns high-level game flow.
    /// Separating state transitions from GameManager keeps bootstrap responsibilities small and focused.
    /// </summary>
    public sealed class GameStateManager : MonoBehaviour
    {
        public enum GameState
        {
            Booting,
            Playing,
            Paused,
            GameOver
        }

        public GameState CurrentState { get; private set; } = GameState.Booting;

        public event Action<GameState> StateChanged;

        public void SetState(GameState newState)
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
