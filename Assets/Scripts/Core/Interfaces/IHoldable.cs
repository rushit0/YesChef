using UnityEngine;

namespace YesChef.Core.Interfaces
{
    /// <summary>
    /// Represents an object that can move between world stations and the player's hands.
    /// Keeping this as an interface lets ingredients, tools, and plated meals share the same pipeline.
    /// </summary>
    public interface IHoldable
    {
        Transform Transform { get; }
        bool IsHeld { get; }
        void OnPickedUp(Transform holdPoint);
        void OnDropped(Vector3 worldPosition);
    }
}
