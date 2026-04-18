using System;
using UnityEngine;

namespace YesChef.Managers
{
    /// <summary>
    /// Central countdown timer for the match.
    /// It exposes events so UI and flow systems can react without polling hidden state.
    /// </summary>
    public sealed class GameTimer : MonoBehaviour
    {
        private const float DefaultDurationSeconds = 180f;

        [SerializeField, Min(1f)] private float durationSeconds = DefaultDurationSeconds;

        private bool isRunning;

        public float DurationSeconds => durationSeconds;
        public float RemainingTimeSeconds { get; private set; }

        public event Action<float> TimeChanged;
        public event Action TimerCompleted;

        private void Awake()
        {
            RemainingTimeSeconds = durationSeconds;
        }

        private void Update()
        {
            if (!isRunning)
            {
                return;
            }

            RemainingTimeSeconds = Mathf.Max(0f, RemainingTimeSeconds - Time.unscaledDeltaTime);
            TimeChanged?.Invoke(RemainingTimeSeconds);

            if (RemainingTimeSeconds > 0f)
            {
                return;
            }

            isRunning = false;
            TimerCompleted?.Invoke();
        }

        public void ResetTimer()
        {
            isRunning = false;
            RemainingTimeSeconds = durationSeconds;
            TimeChanged?.Invoke(RemainingTimeSeconds);
        }

        public void StartTimer()
        {
            RemainingTimeSeconds = durationSeconds;
            isRunning = true;
            TimeChanged?.Invoke(RemainingTimeSeconds);
        }

        public void PauseTimer()
        {
            isRunning = false;
        }

        public void ResumeTimer()
        {
            if (RemainingTimeSeconds <= 0f)
            {
                return;
            }

            isRunning = true;
        }
    }
}
