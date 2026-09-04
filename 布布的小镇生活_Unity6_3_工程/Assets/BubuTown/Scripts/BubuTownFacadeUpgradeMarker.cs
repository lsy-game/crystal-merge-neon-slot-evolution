using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownFacadeUpgradeMarker : MonoBehaviour
    {
        public string SlotId;
        public string TargetStyle;
        public string PreferredAssetType;
        public int ReplacementPriority;
        public bool NeedsInteriorBehindGlass;
        public bool StreetLevelCritical;

        public string Summary()
        {
            return SlotId + " " + TargetStyle + " priority " + ReplacementPriority;
        }
    }
}
