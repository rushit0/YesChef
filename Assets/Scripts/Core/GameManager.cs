using System;
using UnityEngine;
using YesChef.Managers;
using YesChef.Utilities;

namespace YesChef.Core
{
    /// <summary>
    /// Root composition entry point for the game.
    /// Scene objects talk to focused managers, while this bootstrapper wires them together and survives scene changes.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public sealed class GameManager : MonoSingleton<GameManager>
    {
        [Header("Bootstrap References")]
        [SerializeField] private GameFlowManager gameFlowManager;
        [SerializeField] private GameTimer gameTimer;
        [SerializeField] private OrderManager orderManager;
        [SerializeField] private ScoreManager scoreManager;

        public GameFlowManager GameFlowManager => gameFlowManager;
        public GameTimer GameTimer => gameTimer;
        public OrderManager OrderManager => orderManager;
        public ScoreManager ScoreManager => scoreManager;

        public event Action Bootstrapped;

        protected override void Awake()
        {
            base.Awake();

            if (Instance != this)
            {
                return;
            }

            BootstrapManagers();
            Bootstrapped?.Invoke();
        }

        private void BootstrapManagers()
        {
            gameFlowManager = ResolveManager(gameFlowManager, "Game Flow Manager");
            gameTimer = ResolveManager(gameTimer, "Game Timer");
            orderManager = ResolveManager(orderManager, "Order Manager");
            scoreManager = ResolveManager(scoreManager, "Score Manager");

            gameTimer.ResetTimer();
        }

        private T ResolveManager<T>(T currentReference, string objectName) where T : Component
        {
            if (currentReference != null)
            {
                return currentReference;
            }

            T existingManager = GetComponentInChildren<T>(true);
            if (existingManager != null)
            {
                return existingManager;
            }

            GameObject managerObject = new(objectName);
            managerObject.transform.SetParent(transform);
            return managerObject.AddComponent<T>();
        }
    }
}
