using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownCityDeviceMarker : MonoBehaviour
    {
        public string DeviceId;
        public string DeviceType;
        public string DisplayName;
        public string LinkedQuestHookId;
        public bool RequiresMaintenance;
        public bool VisibleOnCityMap;

        public string Summary()
        {
            return DeviceType + " " + DisplayName + " / " + DeviceId;
        }
    }
}
