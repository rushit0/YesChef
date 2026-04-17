using UnityEngine;

namespace YesChef.Core.Interfaces
{
    /// <summary>
    /// Contract for anything the player can trigger in the kitchen.
    /// Interactions are actor-driven so objects can validate who is using them.
    /// </summary>
    public interface IInteractable
    {
        bool CanInteract(GameObject interactor);
        void Interact(GameObject interactor);
    }
}
