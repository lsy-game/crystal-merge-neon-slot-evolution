using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownNightReadabilityMarker : MonoBehaviour
    {
        public string ZoneId;
        public string ZoneType;
        public string DisplayName;
        public string LinkedSceneAreaId;
        public float TargetIntensity;
        public bool RequiresReflectionProbe;
        public bool RuntimeTunable;

        public string Summary()
        {
            return ZoneId + " [" + ZoneType + "] intensity " + TargetIntensity.ToString("0.00");
        }
    }
}
