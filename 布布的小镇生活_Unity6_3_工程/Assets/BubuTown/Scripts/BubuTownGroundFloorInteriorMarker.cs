using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownGroundFloorInteriorMarker : MonoBehaviour
    {
        public string InteriorId;
        public string InteriorType;
        public string DisplayName;
        public string LinkedEntranceId;
        public string LinkedQuestHookId;
        public bool VisibleThroughGlass;
        public bool SupportsInteriorLoad;
        public bool ReplaceablePrefabSlot;

        public string Summary()
        {
            return InteriorId + " [" + InteriorType + "] " + DisplayName;
        }
    }
}
