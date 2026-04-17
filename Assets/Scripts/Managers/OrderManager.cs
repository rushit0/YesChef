using System;
using System.Collections.Generic;
using UnityEngine;
using YesChef.Orders;

namespace YesChef.Managers
{
    /// <summary>
    /// Owns the runtime list of active kitchen orders.
    /// Gameplay systems can depend on this focused manager instead of reaching into UI or scene objects.
    /// </summary>
    public sealed class OrderManager : MonoBehaviour
    {
        [SerializeField] private List<OrderDefinition> availableOrders = new();

        private readonly List<OrderDefinition> activeOrders = new();

        public IReadOnlyList<OrderDefinition> ActiveOrders => activeOrders;

        public event Action<IReadOnlyList<OrderDefinition>> OrdersChanged;

        public void Initialize()
        {
            activeOrders.Clear();
            OrdersChanged?.Invoke(ActiveOrders);
        }

        public bool TryAddOrder(OrderDefinition orderDefinition)
        {
            if (orderDefinition == null || activeOrders.Contains(orderDefinition))
            {
                return false;
            }

            activeOrders.Add(orderDefinition);
            OrdersChanged?.Invoke(ActiveOrders);
            return true;
        }

        public bool TryAddFirstAvailableOrder()
        {
            foreach (OrderDefinition order in availableOrders)
            {
                if (TryAddOrder(order))
                {
                    return true;
                }
            }

            return false;
        }

        public bool CompleteOrder(OrderDefinition orderDefinition)
        {
            if (orderDefinition == null || !activeOrders.Remove(orderDefinition))
            {
                return false;
            }

            OrdersChanged?.Invoke(ActiveOrders);
            return true;
        }
    }
}
