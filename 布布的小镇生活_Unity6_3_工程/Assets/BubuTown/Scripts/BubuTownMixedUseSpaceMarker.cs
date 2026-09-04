using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownMixedUseSpaceMarker : MonoBehaviour
    {
        public string SpaceId;
        public string SpaceType;
        public string DisplayName;
        public string LinkedEntranceId;
        public bool HasInteriorSilhouette;
        public bool IsQuestReady;

        public string Summary()
        {
            return DisplayName + " [" + SpaceType + "] -> " + LinkedEntranceId;
        }
    }
}
