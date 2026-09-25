using UnityEngine;

namespace AIEmotionWorld.Player
{
    public sealed class ThirdPersonCamera : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;
        [SerializeField] private float distance = 6f;
        [SerializeField] private float targetHeight = 1.6f;

        [Header("Camera")]
        [SerializeField] private float mouseSensitivity = 2f;
        [SerializeField] private float followSmoothTime = 0.08f;
        [SerializeField] private float minPitch = -10f;
        [SerializeField] private float maxPitch = 60f;

        private float yaw;
        private float pitch = 20f;
        private Vector3 followVelocity;

        private void Awake()
        {
            Vector3 initialRotation = transform.eulerAngles;
            yaw = initialRotation.y;
            pitch = initialRotation.x;
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 focusPoint = target.position + Vector3.up * targetHeight;
            Vector3 desiredPosition = focusPoint - rotation * Vector3.forward * distance;

            transform.position = Vector3.SmoothDamp(
                transform.position,
                desiredPosition,
                ref followVelocity,
                followSmoothTime);
            transform.rotation = rotation;
        }
    }
}
