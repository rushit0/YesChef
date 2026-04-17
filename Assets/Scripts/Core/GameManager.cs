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
        [SerializeField] private GameStateManager gameStateManager;
        [SerializeField] private OrderManager orderManager;

        public GameStateManager GameStateManager => gameStateManager;
        public OrderManager OrderManager => orderManager;

        public event Action Bootstrapped;

        protected override void Awake()
        {
            base.Awake();

            if (Instance != this)
            {
                return;
            }

            DontDestroyOnLoad(gameObject);

            BootstrapManagers();
            Bootstrapped?.Invoke();
        }

        private void Start()
        {
            gameStateManager.SetState(GameStateManager.GameState.Playing);
        }

        private void BootstrapManagers()
        {
            gameStateManager = ResolveManager(gameStateManager, "Game State Manager");
            orderManager = ResolveManager(orderManager, "Order Manager");

            orderManager.Initialize();
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
