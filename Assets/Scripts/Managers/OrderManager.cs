using System;
using System.Collections.Generic;
using UnityEngine;
using YesChef.Orders;

namespace YesChef.Managers
{
    /// <summary>
    /// Owns the active customer windows and keeps the kitchen supplied with runtime orders.
    /// </summary>
    public sealed class OrderManager : MonoBehaviour
    {
        [SerializeField] private OrderGenerator orderGenerator;
        [SerializeField] private CustomerWindow[] customerWindows = new CustomerWindow[4];

        private readonly List<OrderData> activeOrders = new();
        private bool initialized;

        public IReadOnlyList<OrderData> ActiveOrders => activeOrders;

        public event Action<IReadOnlyList<OrderData>> OrdersChanged;

        public void Initialize(ScoreManager scoreManager)
        {
            ResolveDependencies();
            UnsubscribeFromWindows();
            SubscribeToWindows(scoreManager);
            ResetOrders();
            RefreshActiveOrders();
        }

        private void ResolveDependencies()
        {
            if (orderGenerator == null)
            {
                orderGenerator = GetComponentInChildren<OrderGenerator>(true);
            }

            bool hasAssignedWindows = false;
            foreach (CustomerWindow customerWindow in customerWindows)
            {
                if (customerWindow != null)
                {
                    hasAssignedWindows = true;
                    break;
                }
            }

            if (!hasAssignedWindows)
            {
                CustomerWindow[] discoveredWindows = FindObjectsByType<CustomerWindow>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                int windowCount = Mathf.Min(customerWindows.Length, discoveredWindows.Length);
                for (int index = 0; index < windowCount; index++)
                {
                    customerWindows[index] = discoveredWindows[index];
                }
            }
        }

        private void SubscribeToWindows(ScoreManager scoreManager)
        {
            foreach (CustomerWindow customerWindow in customerWindows)
            {
                if (customerWindow == null)
                {
                    continue;
                }

                customerWindow.Initialize(scoreManager);
                customerWindow.RespawnRequested += HandleRespawnRequested;
            }
        }

        private void UnsubscribeFromWindows()
        {
            foreach (CustomerWindow customerWindow in customerWindows)
            {
                if (customerWindow != null)
                {
                    customerWindow.RespawnRequested -= HandleRespawnRequested;
                }
            }
        }

        private void ResetOrders()
        {
            foreach (CustomerWindow customerWindow in customerWindows)
            {
                AssignNewOrder(customerWindow);
            }

            initialized = true;
        }

        private void HandleRespawnRequested(CustomerWindow customerWindow)
        {
            AssignNewOrder(customerWindow);
            RefreshActiveOrders();
        }

        private void AssignNewOrder(CustomerWindow customerWindow)
        {
            if (customerWindow == null || orderGenerator == null)
            {
                return;
            }

            OrderData nextOrder = orderGenerator.GenerateRandomOrder();
            if (nextOrder != null)
            {
                customerWindow.SetOrder(nextOrder);
            }
        }

        private void RefreshActiveOrders()
        {
            activeOrders.Clear();

            foreach (CustomerWindow customerWindow in customerWindows)
            {
                if (customerWindow?.CurrentOrder != null)
                {
                    activeOrders.Add(customerWindow.CurrentOrder);
                }
            }

            OrdersChanged?.Invoke(ActiveOrders);
        }

        private void OnDisable()
        {
            UnsubscribeFromWindows();
        }
    }
}
