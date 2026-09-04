using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownPublicRealmMarker : MonoBehaviour
    {
        public string RealmId;
        public string RealmType;
        public string DisplayName;
        public string LinkedRouteOrQuestId;
        public bool SupportsAccessibility;
        public bool ReplaceableArtSlot;

        public string Summary()
        {
            return RealmId + " [" + RealmType + "] " + DisplayName;
        }
    }
}
