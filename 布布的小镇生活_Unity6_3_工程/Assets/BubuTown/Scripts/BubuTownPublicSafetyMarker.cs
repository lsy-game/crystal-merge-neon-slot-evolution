using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownPublicSafetyMarker : MonoBehaviour
    {
        public string SafetyId;
        public string SafetyType;
        public string DisplayName;
        public string LinkedQuestHookId;
        public bool EmergencyCritical;
        public bool VisibleOnCityMap;
        public bool ReplaceableArtSlot;

        public string Summary()
        {
            return SafetyId + " [" + SafetyType + "] " + DisplayName;
        }
    }
}
