using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownArtReplacementAuditMarker : MonoBehaviour
    {
        public string AuditId;
        public string TargetArea;
        public string ReplacementCategory;
        public string AcceptanceRule;
        public string SuggestedSource;
        public int Priority;
        public bool RequiresUrpMaterial;
        public bool RejectPixelatedTextures;
        public bool PublicRepoSafe;

        public string Summary()
        {
            return TargetArea + " -> " + ReplacementCategory + " priority " + Priority;
        }
    }
}
