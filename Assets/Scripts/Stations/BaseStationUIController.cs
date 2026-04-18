using System;
using System.Collections.Generic;
using UnityEngine;
using YesChef.Core.Interfaces;
using YesChef.Player;

namespace YesChef.Stations
{
    /// <summary>
    /// Base station class for stations that publish contextual popup actions.
    /// </summary>
    public abstract class BaseStationUIController : StationBase, IContextActionSource
    {
        private readonly List<ContextActionData> cachedActions = new();

        public abstract string PopupTitle { get; }
        public virtual Transform PopupAnchor => transform;

        public event Action ContextActionsChanged;

        public sealed override void Interact(GameObject interactor)
        {
            if (!TryGetCarryController(interactor, out PlayerCarryController carryController))
            {
                return;
            }

            cachedActions.Clear();
            GetContextActions(carryController, cachedActions);

            foreach (ContextActionData action in cachedActions)
            {
                if (!action.IsEnabled)
                {
                    continue;
                }

                action.Execute?.Invoke();
                return;
            }
        }

        public abstract void GetContextActions(PlayerCarryController playerCarryController, List<ContextActionData> actions);

        protected void NotifyContextChanged()
        {
            ContextActionsChanged?.Invoke();
        }
    }
}
