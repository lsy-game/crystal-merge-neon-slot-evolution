using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownEntranceReadabilityMarker : MonoBehaviour
    {
        public string EntranceId;
        public string DisplayName;
        public string DestinationType;
        public string LinkedInteractableId;
        public bool ShowOnMinimap;
        public bool NightReadable;
        public bool QuestRelevant;

        public string Summary()
        {
            return DisplayName + " -> " + DestinationType + " / " + LinkedInteractableId;
        }
    }
}
