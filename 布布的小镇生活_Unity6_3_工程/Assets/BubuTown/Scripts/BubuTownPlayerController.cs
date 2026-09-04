using UnityEngine;

namespace BubuTown
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class BubuTownPlayerController : MonoBehaviour
    {
        public float WalkSpeed = 4f;
        public float RunSpeed = 7f;
        public float Acceleration = 18f;
        public float Deceleration = 22f;
        public float RotationSharpness = 14f;
        public float Gravity = -18f;
        public Transform CameraTransform;
        public BubuTownLocomotionAnimator LocomotionAnimator;

        private CharacterController controller;
        private Vector3 planarVelocity;
        private float verticalSpeed;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            if (CameraTransform == null && Camera.main != null)
            {
                CameraTransform = Camera.main.transform;
            }

            if (LocomotionAnimator == null)
            {
                LocomotionAnimator = GetComponentInChildren<BubuTownLocomotionAnimator>();
            }
        }

        private void Update()
        {
            var input = Vector2.ClampMagnitude(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")), 1f);
            var forward = CameraTransform != null ? CameraTransform.forward : Vector3.forward;
            var right = CameraTransform != null ? CameraTransform.right : Vector3.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            var move = Vector3.ClampMagnitude(forward * input.y + right * input.x, 1f);
            var sprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            var speed = sprinting ? RunSpeed : WalkSpeed;
            var desiredVelocity = move * speed;
            var rate = desiredVelocity.sqrMagnitude > planarVelocity.sqrMagnitude ? Acceleration : Deceleration;
            planarVelocity = Vector3.MoveTowards(planarVelocity, desiredVelocity, rate * Time.deltaTime);
            if (controller.isGrounded && verticalSpeed < 0f)
            {
                verticalSpeed = -1f;
            }

            verticalSpeed += Gravity * Time.deltaTime;
            controller.Move((planarVelocity + Vector3.up * verticalSpeed) * Time.deltaTime);
            if (planarVelocity.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(planarVelocity.normalized), Time.deltaTime * RotationSharpness);
            }

            if (LocomotionAnimator != null)
            {
                var localMove = transform.InverseTransformDirection(planarVelocity);
                var normalizedSpeed = Mathf.Clamp01(planarVelocity.magnitude / Mathf.Max(RunSpeed, 0.01f));
                LocomotionAnimator.SetLocomotion(normalizedSpeed, sprinting && input.sqrMagnitude > 0.01f, controller.isGrounded, localMove);
            }
        }
    }
}
