using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownProximityLabel : MonoBehaviour
    {
        public Transform Player;
        public float VisibleDistance = 1.35f;
        public bool FaceCamera = true;

        private Renderer[] renderers;
        private Camera mainCamera;

        private void Awake()
        {
            renderers = GetComponentsInChildren<Renderer>(true);
            mainCamera = Camera.main;
            SetVisible(false);
        }

        private void Update()
        {
            if (Player == null)
            {
                var playerObject = GameObject.Find("Player_Start_Bubu");
                if (playerObject != null)
                {
                    Player = playerObject.transform;
                }
            }

            if (Player == null)
            {
                SetVisible(false);
                return;
            }

            var distance = Vector3.Distance(Player.position, transform.position);
            SetVisible(distance <= VisibleDistance);

            if (FaceCamera && mainCamera != null && distance <= VisibleDistance)
            {
                var direction = transform.position - mainCamera.transform.position;
                if (direction.sqrMagnitude > 0.01f)
                {
                    transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
                }
            }
        }

        private void SetVisible(bool visible)
        {
            if (renderers == null)
            {
                return;
            }

            foreach (var item in renderers)
            {
                if (item != null)
                {
                    item.enabled = visible;
                }
            }
        }
    }
}
