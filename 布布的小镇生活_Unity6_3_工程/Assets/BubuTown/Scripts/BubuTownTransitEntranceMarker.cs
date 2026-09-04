using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownTransitEntranceMarker : MonoBehaviour
    {
        public string EntranceId;
        public string EntranceType;
        public string DisplayName;
        public string LinkedRouteNodeId;
        public bool SupportsAccessibility;
        public bool SupportsQuestHooks;
        public bool ReplaceableArtSlot;

        public string Summary()
        {
            return EntranceId + " [" + EntranceType + "] " + DisplayName;
        }
    }
}
