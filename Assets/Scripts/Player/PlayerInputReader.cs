using UnityEngine;
using UnityEngine.InputSystem;

namespace YesChef.Player {
    public sealed class PlayerInputReader : MonoBehaviour {
        public Vector2 MoveInput { get; private set; }
        public bool InteractPressedThisFrame { get; private set; }
        private InputSystem_Actions controls;

        private void Awake() {
            controls = new InputSystem_Actions();
        }

        private void OnEnable() {
            controls.Player.Enable();
            controls.Player.Interact.performed += OnInteractPerformed;
        }

        private void OnDisable() {
            controls.Player.Interact.performed -= OnInteractPerformed;
            controls.Player.Disable();
        }

        private void Update() {
            MoveInput = controls.Player.Move.ReadValue<Vector2>();
        }

        private void LateUpdate() {
            InteractPressedThisFrame = false;
        }

        private void OnInteractPerformed(InputAction.CallbackContext context) {
            InteractPressedThisFrame = true;
        }

        private void OnDestroy() {
            controls.Dispose();
        }
    }
}