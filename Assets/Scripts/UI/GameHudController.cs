using TMPro;
using UnityEngine;
using YesChef.Core;
using YesChef.Managers;
using YesChef.Orders;

namespace YesChef.UI
{
    /// <summary>
    /// Thin presentation layer for HUD data.
    /// UI listens to manager events and renders state, which avoids pushing UI concerns into core gameplay systems.
    /// </summary>
    public sealed class GameHudController : MonoBehaviour
    {
        [SerializeField] private TMP_Text stateLabel;
        [SerializeField] private TMP_Text activeOrderCountLabel;

        private void OnEnable()
        {
            if (GameManager.Instance == null)
            {
                return;
            }

            GameManager.Instance.GameStateManager.StateChanged += HandleStateChanged;
            GameManager.Instance.OrderManager.OrdersChanged += HandleOrdersChanged;

            HandleStateChanged(GameManager.Instance.GameStateManager.CurrentState);
            HandleOrdersChanged(GameManager.Instance.OrderManager.ActiveOrders);
        }

        private void OnDisable()
        {
            if (GameManager.Instance == null)
            {
                return;
            }

            GameManager.Instance.GameStateManager.StateChanged -= HandleStateChanged;
            GameManager.Instance.OrderManager.OrdersChanged -= HandleOrdersChanged;
        }

        private void HandleStateChanged(GameStateManager.GameState gameState)
        {
            if (stateLabel != null)
            {
                stateLabel.text = $"State: {gameState}";
            }
        }

        private void HandleOrdersChanged(System.Collections.Generic.IReadOnlyList<OrderDefinition> activeOrders)
        {
            if (activeOrderCountLabel != null)
            {
                activeOrderCountLabel.text = $"Orders: {activeOrders.Count}";
            }
        }
    }
}
