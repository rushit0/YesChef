using UnityEngine;

namespace YesChef.Player
{
    /// <summary>
    /// Moves the chef using a CharacterController and rotates toward travel direction.
    /// The controller depends only on input state, which keeps movement logic easy to test and extend.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInputReader))]
    public sealed class PlayerMovementController : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float moveSpeed = 5f;
        [SerializeField, Min(0f)] private float movementSmoothing = 12f;
        [SerializeField, Min(0f)] private float rotationSmoothing = 15f;
        [SerializeField, Min(0f)] private float gravity = 20f;

        private CharacterController characterController;
        private PlayerInputReader inputReader;
        private Vector3 currentVelocity;
        private float verticalVelocity;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            inputReader = GetComponent<PlayerInputReader>();
        }

        private void Update()
        {
            Vector3 desiredMovement = new Vector3(inputReader.MoveInput.x, 0f, inputReader.MoveInput.y) * moveSpeed;
            currentVelocity = Vector3.Lerp(currentVelocity, desiredMovement, 1f - Mathf.Exp(-movementSmoothing * Time.deltaTime));

            if (characterController.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -1f;
            }
            else
            {
                verticalVelocity -= gravity * Time.deltaTime;
            }

            Vector3 frameMovement = new(currentVelocity.x, verticalVelocity, currentVelocity.z);
            characterController.Move(frameMovement * Time.deltaTime);

            RotateTowardsMovementDirection(currentVelocity);
        }

        private void RotateTowardsMovementDirection(Vector3 planarVelocity)
        {
            Vector3 planarDirection = new(planarVelocity.x, 0f, planarVelocity.z);
            if (planarDirection.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(planarDirection.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                1f - Mathf.Exp(-rotationSmoothing * Time.deltaTime));
        }
    }
}
