using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownWeatherAmenityMarker : MonoBehaviour
    {
        public string AmenityId;
        public string AmenityType;
        public string DisplayName;
        public string LinkedQuestHookId;
        public bool SupportsRainGameplay;
        public bool VisibleOnCityMap;
        public bool ReplaceableArtSlot;

        public string Summary()
        {
            return AmenityId + " [" + AmenityType + "] " + DisplayName;
        }
    }
}
