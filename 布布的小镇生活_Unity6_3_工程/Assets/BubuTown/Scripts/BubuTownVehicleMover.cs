using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownVehicleMover : MonoBehaviour
    {
        public BubuTownVehiclePath Path;
        public float SpeedMetersPerSecond = 2.4f;
        public float TurnSpeedDegrees = 180f;
        public bool MoveOnPlay = true;

        private int _nextWaypointIndex;

        private void Start()
        {
            var first = Path != null ? Path.FirstWaypoint() : null;
            if (first != null)
            {
                transform.position = first.position;
                _nextWaypointIndex = Mathf.Min(1, Path.Waypoints.Length - 1);
            }
        }

        private void Update()
        {
            if (!MoveOnPlay || Path == null || Path.Waypoints == null || Path.Waypoints.Length < 2)
            {
                return;
            }

            var target = Path.Waypoints[_nextWaypointIndex];
            if (target == null)
            {
                AdvanceWaypoint();
                return;
            }

            var toTarget = target.position - transform.position;
            toTarget.y = 0f;
            if (toTarget.sqrMagnitude < 0.08f)
            {
                AdvanceWaypoint();
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, target.position, SpeedMetersPerSecond * Time.deltaTime);
            var targetRotation = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, TurnSpeedDegrees * Time.deltaTime);
        }

        private void AdvanceWaypoint()
        {
            _nextWaypointIndex++;
            if (_nextWaypointIndex >= Path.Waypoints.Length)
            {
                _nextWaypointIndex = Path.Loop ? 0 : Path.Waypoints.Length - 1;
                if (!Path.Loop)
                {
                    MoveOnPlay = false;
                }
            }
        }
    }
}
