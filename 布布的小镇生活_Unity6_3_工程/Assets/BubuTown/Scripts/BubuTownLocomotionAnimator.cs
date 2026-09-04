using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownLocomotionAnimator : MonoBehaviour
    {
        public Animator Animator;
        public Transform VisualRoot;
        public Transform Head;
        public Transform HairTail;
        public Transform SkirtOrCoat;
        public Transform LeftArm;
        public Transform RightArm;
        public Transform LeftLeg;
        public Transform RightLeg;
        public float WalkCycleSpeed = 5.8f;
        public float RunCycleSpeed = 8.6f;
        public float ArmSwingDegrees = 20f;
        public float LegSwingDegrees = 24f;
        public float BobHeight = 0.035f;

        private float smoothedSpeed01;
        private float targetSpeed01;
        private bool sprinting;
        private bool grounded = true;
        private float phase;
        private Vector3 visualStartLocalPosition;

        public void SetLocomotion(float speed01, bool isSprinting, bool isGrounded, Vector3 localMove)
        {
            targetSpeed01 = Mathf.Clamp01(speed01);
            sprinting = isSprinting;
            grounded = isGrounded;

            if (Animator == null)
            {
                return;
            }

            Animator.SetFloat("Speed", targetSpeed01, 0.12f, Time.deltaTime);
            Animator.SetFloat("MoveX", Mathf.Clamp(localMove.x, -1f, 1f), 0.12f, Time.deltaTime);
            Animator.SetFloat("MoveY", Mathf.Clamp(localMove.z, -1f, 1f), 0.12f, Time.deltaTime);
            Animator.SetBool("Grounded", grounded);
            Animator.SetBool("Sprinting", sprinting);
        }

        private void Awake()
        {
            if (Animator == null)
            {
                Animator = GetComponentInChildren<Animator>();
            }

            if (VisualRoot != null)
            {
                visualStartLocalPosition = VisualRoot.localPosition;
            }
        }

        private void LateUpdate()
        {
            smoothedSpeed01 = Mathf.MoveTowards(smoothedSpeed01, targetSpeed01, Time.deltaTime * 5.5f);
            var cycleSpeed = sprinting ? RunCycleSpeed : WalkCycleSpeed;
            phase += Time.deltaTime * cycleSpeed * Mathf.Lerp(0.35f, 1f, smoothedSpeed01);

            if (Animator != null && Animator.runtimeAnimatorController != null)
            {
                return;
            }

            ApplyProceduralPreview();
        }

        private void ApplyProceduralPreview()
        {
            var weight = grounded ? smoothedSpeed01 : 0f;
            var swing = Mathf.Sin(phase) * weight;
            var oppositeSwing = -swing;
            var bob = Mathf.Abs(Mathf.Sin(phase * 2f)) * BobHeight * weight;

            if (VisualRoot != null)
            {
                VisualRoot.localPosition = visualStartLocalPosition + Vector3.up * bob;
            }

            SetLocalXRotation(LeftArm, swing * ArmSwingDegrees);
            SetLocalXRotation(RightArm, oppositeSwing * ArmSwingDegrees);
            SetLocalXRotation(LeftLeg, oppositeSwing * LegSwingDegrees);
            SetLocalXRotation(RightLeg, swing * LegSwingDegrees);

            if (Head != null)
            {
                Head.localRotation = Quaternion.Euler(Mathf.Sin(phase * 2f) * 1.4f * weight, 0f, 0f);
            }

            if (HairTail != null)
            {
                HairTail.localRotation = Quaternion.Euler(-8f + oppositeSwing * 8f, 0f, 0f);
            }

            if (SkirtOrCoat != null)
            {
                SkirtOrCoat.localRotation = Quaternion.Euler(swing * 2.5f, 0f, 0f);
            }
        }

        private static void SetLocalXRotation(Transform target, float degrees)
        {
            if (target != null)
            {
                target.localRotation = Quaternion.Euler(degrees, 0f, 0f);
            }
        }
    }
}
