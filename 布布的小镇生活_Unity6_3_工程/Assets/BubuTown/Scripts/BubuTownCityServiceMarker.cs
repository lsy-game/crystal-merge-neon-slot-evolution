using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownCityServiceMarker : MonoBehaviour
    {
        public string ServiceId;
        public string ServiceType;
        public string DisplayName;
        public string LinkedQuestHookId;
        public bool SupportsInteraction;
        public bool VisibleOnCityMap;
        public bool ReplaceableArtSlot;

        public string Summary()
        {
            return ServiceId + " [" + ServiceType + "] " + DisplayName;
        }
    }
}
