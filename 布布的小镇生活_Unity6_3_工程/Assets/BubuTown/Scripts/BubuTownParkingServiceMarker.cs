using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownParkingServiceMarker : MonoBehaviour
    {
        public string ServiceId;
        public string DisplayName;
        public string ServiceType;
        public string LinkedRouteNodeId;
        public bool IsPlayerFacing;
        public bool IsQuestReady;

        public string Summary()
        {
            return DisplayName + " [" + ServiceType + "] -> " + LinkedRouteNodeId;
        }
    }
}
