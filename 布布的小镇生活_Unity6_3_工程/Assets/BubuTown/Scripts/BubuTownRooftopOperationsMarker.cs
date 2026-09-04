using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownRooftopOperationsMarker : MonoBehaviour
    {
        public string OperationId;
        public string OperationType;
        public string DisplayName;
        public string LinkedBuildingId;
        public bool SupportsMaintenanceQuest;
        public bool ReplaceableArtSlot;

        public string Summary()
        {
            return OperationId + " [" + OperationType + "] " + DisplayName;
        }
    }
}
