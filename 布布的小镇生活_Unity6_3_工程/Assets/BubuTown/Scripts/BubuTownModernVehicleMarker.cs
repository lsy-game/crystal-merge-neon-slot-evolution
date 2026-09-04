using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownModernVehicleMarker : MonoBehaviour
    {
        public string VehicleId;
        public string VehicleType;
        public string DisplayName;
        public string LinkedRouteOrQuestId;
        public bool SupportsVehiclePath;
        public bool SupportsGarageGameplay;
        public bool ReplaceableModelSlot;

        public string Summary()
        {
            return VehicleId + " [" + VehicleType + "] " + DisplayName;
        }
    }
}
