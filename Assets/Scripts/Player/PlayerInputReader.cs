using UnityEngine;

namespace YesChef.Player
{
    /// <summary>
    /// Reads raw top-down chef input and exposes it as simple state for other components.
    /// This keeps input collection separate from movement and interaction behaviour.
    /// </summary>
    public sealed class PlayerInputReader : MonoBehaviour
    {
        public Vector2 MoveInput { get; private set; }
        public bool InteractPressedThisFrame { get; private set; }

        private void Update()
        {
            MoveInput = ReadMovementInput();
            InteractPressedThisFrame = Input.GetKeyDown(KeyCode.E);
        }

        private static Vector2 ReadMovementInput()
        {
            float horizontal = 0f;
            float vertical = 0f;

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                horizontal -= 1f;
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                horizontal += 1f;
            }

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                vertical -= 1f;
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                vertical += 1f;
            }

            Vector2 moveInput = new(horizontal, vertical);
            return moveInput.sqrMagnitude > 1f ? moveInput.normalized : moveInput;
        }
    }
}
