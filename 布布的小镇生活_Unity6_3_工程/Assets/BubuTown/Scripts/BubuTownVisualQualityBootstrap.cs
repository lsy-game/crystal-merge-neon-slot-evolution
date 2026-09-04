using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownVisualQualityBootstrap : MonoBehaviour
    {
        public int TargetAntiAliasing = 4;
        public AnisotropicFiltering TargetAnisotropicFiltering = AnisotropicFiltering.ForceEnable;
        public int TargetFrameRate = 60;
        public bool EnableCameraHdr = true;
        public bool EnableCameraMsaa = true;
        public bool EnableSoftVegetation = true;
        public ShadowQuality TargetShadowQuality = ShadowQuality.All;
        public ShadowResolution TargetShadowResolution = ShadowResolution.High;
        public float TargetShadowDistance = 85f;
        public float TargetLodBias = 1.75f;
        public int TargetMaximumLodLevel = 0;
        public float TargetRenderScale = 1f;
        public int TargetGlobalTextureMipmapLimit = 0;

        private void Awake()
        {
            Apply();
        }

        private void OnValidate()
        {
            TargetAntiAliasing = Mathf.Clamp(TargetAntiAliasing, 0, 8);
            TargetFrameRate = Mathf.Clamp(TargetFrameRate, 30, 240);
            TargetShadowDistance = Mathf.Clamp(TargetShadowDistance, 20f, 180f);
            TargetLodBias = Mathf.Clamp(TargetLodBias, 0.5f, 4f);
            TargetMaximumLodLevel = Mathf.Clamp(TargetMaximumLodLevel, 0, 3);
            TargetRenderScale = Mathf.Clamp(TargetRenderScale, 0.75f, 1.5f);
            TargetGlobalTextureMipmapLimit = Mathf.Clamp(TargetGlobalTextureMipmapLimit, 0, 3);
        }

        public void Apply()
        {
            QualitySettings.antiAliasing = TargetAntiAliasing;
            QualitySettings.anisotropicFiltering = TargetAnisotropicFiltering;
            QualitySettings.softVegetation = EnableSoftVegetation;
            QualitySettings.shadows = TargetShadowQuality;
            QualitySettings.shadowResolution = TargetShadowResolution;
            QualitySettings.shadowDistance = TargetShadowDistance;
            QualitySettings.lodBias = TargetLodBias;
            QualitySettings.maximumLODLevel = TargetMaximumLodLevel;
            QualitySettings.globalTextureMipmapLimit = TargetGlobalTextureMipmapLimit;
            ScalableBufferManager.ResizeBuffers(TargetRenderScale, TargetRenderScale);
            Application.targetFrameRate = TargetFrameRate;

            var cameras = FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var camera in cameras)
            {
                camera.allowHDR = EnableCameraHdr;
                camera.allowMSAA = EnableCameraMsaa;
            }
        }
    }
}
