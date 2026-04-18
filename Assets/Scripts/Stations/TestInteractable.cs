using UnityEngine;
using YesChef.Core.Interfaces;


public class TestInteractable : MonoBehaviour, IInteractable {

    public void Interact(GameObject interactor) {
        Debug.Log("Interacted!");
    }
}
