using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownSecondLevelConnectionMarker : MonoBehaviour
    {
        public string ConnectionId;
        public string ConnectionType;
        public string DisplayName;
        public string LinkedRouteOrQuestId;
        public bool SupportsPhotoSpot;
        public bool SupportsTraversal;
        public bool ReplaceableArtSlot;

        public string Summary()
        {
            return ConnectionId + " [" + ConnectionType + "] " + DisplayName;
        }
    }
}
