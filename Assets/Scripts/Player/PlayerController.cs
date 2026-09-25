using UnityEngine;

namespace AIEmotionWorld.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 12f;
        [SerializeField] private float gravity = -20f;

        [Header("References")]
        [SerializeField] private Transform movementReference;

        private CharacterController characterController;
        private float verticalVelocity;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();

            if (movementReference == null && Camera.main != null)
            {
                movementReference = Camera.main.transform;
            }
        }

        private void Update()
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            Vector3 movementDirection = GetMovementDirection(horizontalInput, verticalInput);
            Move(movementDirection);
            Rotate(movementDirection);
        }

        private Vector3 GetMovementDirection(float horizontalInput, float verticalInput)
        {
            Transform reference = movementReference != null ? movementReference : transform;
            Vector3 forward = reference.forward;
            Vector3 right = reference.right;

            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 direction = forward * verticalInput + right * horizontalInput;
            return direction.sqrMagnitude > 1f ? direction.normalized : direction;
        }

        private void Move(Vector3 direction)
        {
            if (characterController.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            verticalVelocity += gravity * Time.deltaTime;

            Vector3 velocity = direction * moveSpeed;
            velocity.y = verticalVelocity;
            characterController.Move(velocity * Time.deltaTime);
        }

        private void Rotate(Vector3 direction)
        {
            if (direction.sqrMagnitude < 0.001f)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime);
        }
    }
}
