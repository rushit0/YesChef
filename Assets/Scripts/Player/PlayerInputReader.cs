using UnityEngine;
using UnityEngine.InputSystem;

namespace YesChef.Player {
    /// <summary>
    /// Reads movement input for the chef.
    /// Interaction is now proximity-popup driven, so no gameplay action key is exposed here.
    /// </summary>
    public sealed class PlayerInputReader : MonoBehaviour {
        public Vector2 MoveInput { get; private set; }
        public bool PausePressedThisFrame { get; private set; }

        private InputSystem_Actions controls;

        private void Awake() {
            controls = new InputSystem_Actions();
        }

        private void OnEnable() {
            controls.Player.Enable();
            controls.UI.Enable();
        }

        private void OnDisable() {
            controls.Player.Disable();
            controls.UI.Disable();
        }

        private void Update() {
            MoveInput = controls.Player.Move.ReadValue<Vector2>();
            PausePressedThisFrame = controls.UI.Cancel.WasPressedThisFrame();
        }

        private void OnDestroy() {
            controls.Dispose();
        }
    }
}
