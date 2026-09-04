using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownCityLandmarkMarker : MonoBehaviour
    {
        public string LandmarkId;
        public string LandmarkType;
        public string DisplayName;
        public string LinkedQuestHookId;
        public bool SupportsPhotoMode;
        public bool VisibleOnCityMap;
        public bool ReplaceableArtSlot;

        public string Summary()
        {
            return LandmarkId + " [" + LandmarkType + "] " + DisplayName;
        }
    }
}
