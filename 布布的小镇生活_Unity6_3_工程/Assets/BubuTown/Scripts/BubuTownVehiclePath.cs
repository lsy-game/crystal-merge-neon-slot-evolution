using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownVehiclePath : MonoBehaviour
    {
        public string PathId;
        public bool Loop = true;
        public Transform[] Waypoints = new Transform[0];

        public Transform FirstWaypoint()
        {
            return Waypoints != null && Waypoints.Length > 0 ? Waypoints[0] : null;
        }
    }
}
