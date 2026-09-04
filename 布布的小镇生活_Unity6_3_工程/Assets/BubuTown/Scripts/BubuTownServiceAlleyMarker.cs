using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownServiceAlleyMarker : MonoBehaviour
    {
        public string AlleyId;
        public string AlleyType;
        public string DisplayName;
        public string LinkedBuildingId;
        public string LinkedQuestHookId;
        public bool SupportsDeliveryQuest;
        public bool SupportsMaintenanceQuest;
        public bool KeepPlayerSightlineClear;

        public string Summary()
        {
            return AlleyId + " [" + AlleyType + "] " + DisplayName;
        }
    }
}
