using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownRoadFlowMarker : MonoBehaviour
    {
        public string FlowId;
        public string FlowType;
        public string DisplayName;
        public string LinkedRouteNodeId;
        public bool SupportsVehiclePath;
        public bool SupportsPedestrianSafety;
        public bool VisibleOnMinimap;

        public string Summary()
        {
            return FlowId + " [" + FlowType + "] " + DisplayName;
        }
    }
}
