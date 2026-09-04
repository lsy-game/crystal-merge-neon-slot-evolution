using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownGreenAmenityMarker : MonoBehaviour
    {
        public string AmenityId;
        public string AmenityType;
        public string DisplayName;
        public string LinkedQuestHookId;
        public bool SupportsPhotoSpot;
        public bool SupportsRestSpot;
        public bool ReplaceableArtSlot;

        public string Summary()
        {
            return AmenityId + " [" + AmenityType + "] " + DisplayName;
        }
    }
}
