using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownAssetQualityGateMarker : MonoBehaviour
    {
        public string GateId;
        public string Category;
        public string MinimumTextureRule;
        public string PreferredSource;
        public int ReplacementPriority;
        public bool RejectPixelatedTextures;
        public bool RequiresAttributionCheck;

        public string Summary()
        {
            return Category + " gate " + GateId + " priority " + ReplacementPriority;
        }
    }
}
