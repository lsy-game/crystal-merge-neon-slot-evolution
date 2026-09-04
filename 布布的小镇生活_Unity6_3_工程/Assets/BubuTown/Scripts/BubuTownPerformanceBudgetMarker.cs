using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownPerformanceBudgetMarker : MonoBehaviour
    {
        public string SectorId;
        public string DisplayName;
        public int TargetMaxRenderers;
        public int TargetMaxLights;
        public bool IsStreamingBoundary;
        public bool RequiresLodSwap;
        public Transform[] RuntimeHooks = new Transform[0];

        public string Summary()
        {
            return DisplayName + " renderers<=" + TargetMaxRenderers + " lights<=" + TargetMaxLights;
        }
    }
}
