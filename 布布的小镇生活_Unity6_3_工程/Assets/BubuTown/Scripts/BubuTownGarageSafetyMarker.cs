using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownGarageSafetyMarker : MonoBehaviour
    {
        public string SafetyId;
        public string SafetyType;
        public string DisplayName;
        public string LinkedRouteNodeId;
        public bool IsEmergencyCritical;
        public bool QuestReady;

        public string Summary()
        {
            return SafetyId + " [" + SafetyType + "] " + DisplayName;
        }
    }
}
