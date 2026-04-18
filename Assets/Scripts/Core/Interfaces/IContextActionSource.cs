using System;
using System.Collections.Generic;
using UnityEngine;
using YesChef.Player;

namespace YesChef.Core.Interfaces {
    /// <summary>
    /// Describes a single contextual action that can be rendered as a popup button.
    /// </summary>
    public readonly struct ContextActionData {
        public ContextActionData(string label, bool isEnabled, Action execute) {
            Label = label;
            IsEnabled = isEnabled;
            Execute = execute;
        }

        public string Label { get; }
        public bool IsEnabled { get; }
        public Action Execute { get; }
    }

    /// <summary>
    /// Implemented by gameplay objects that can expose contextual actions to the popup UI.
    /// </summary>
    public interface IContextActionSource {
        string PopupTitle { get; }
        Transform PopupAnchor { get; }
        event Action ContextActionsChanged;
        void GetContextActions(PlayerCarryController playerCarryController, List<ContextActionData> actions);
    }
}
