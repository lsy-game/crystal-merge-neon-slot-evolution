using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownThirdPersonCamera : MonoBehaviour
    {
        public Transform Target;
        public Vector3 Offset = new Vector3(0f, 2.55f, -4.65f);
        public float MouseSensitivity = 2.5f;
        public float FollowSharpness = 12f;
        public float Pitch = 14f;
        public float Yaw;

        private void LateUpdate()
        {
            if (Target == null)
            {
                return;
            }

            Yaw += Input.GetAxis("Mouse X") * MouseSensitivity;
            Pitch = Mathf.Clamp(Pitch - Input.GetAxis("Mouse Y") * MouseSensitivity, -10f, 55f);
            var rotation = Quaternion.Euler(Pitch, Yaw, 0f);
            transform.position = Vector3.Lerp(transform.position, Target.position + rotation * Offset, Time.deltaTime * FollowSharpness);
            transform.rotation = rotation;
        }
    }
}
