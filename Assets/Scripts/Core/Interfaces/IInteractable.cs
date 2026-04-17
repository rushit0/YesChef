using UnityEngine;

namespace YesChef.Core.Interfaces
{
    /// <summary>
    /// Contract for anything the player can trigger in the kitchen.
    /// The interactor is passed in so stations can inspect the chef that initiated the action.
    /// </summary>
    public interface IInteractable
    {
        void Interact(GameObject interactor);
    }
}
