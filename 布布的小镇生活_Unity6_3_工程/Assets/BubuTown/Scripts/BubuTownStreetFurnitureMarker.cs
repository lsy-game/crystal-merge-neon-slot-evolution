using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownStreetFurnitureMarker : MonoBehaviour
    {
        public string FurnitureId;
        public string FurnitureType;
        public string DisplayName;
        public string LinkedQuestHookId;
        public bool SupportsInteraction;
        public bool ReplaceableArtSlot;

        public string Summary()
        {
            return FurnitureId + " [" + FurnitureType + "] " + DisplayName;
        }
    }
}
