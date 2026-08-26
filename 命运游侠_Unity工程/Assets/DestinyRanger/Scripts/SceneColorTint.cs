using UnityEngine;
using UnityEngine.UI;

namespace DestinyRanger
{
    [DisallowMultipleComponent]
    public sealed class SceneColorTint : MonoBehaviour
    {
        [SerializeField] private SceneToneProfile activeProfile;
        [SerializeField] private bool applyOnStart = true;
        [SerializeField] private bool includeChildren = true;

        public SceneToneProfile ActiveProfile
        {
            get => activeProfile;
            set
            {
                activeProfile = value;
                ApplyProfile();
            }
        }

        private void Start()
        {
            if (applyOnStart)
                ApplyProfile();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
                ApplyProfile();
        }
#endif

        public void ApplyProfile()
        {
            if (!activeProfile)
                return;

            var mainColor = activeProfile.sceneMainColor;
            var shadowColor = activeProfile.sceneShadowColor;
            var overlayStrength = activeProfile.colorOverlayStrength;

            Shader.SetGlobalColor("_FateWeaverSceneMainColor", mainColor);
            Shader.SetGlobalColor("_FateWeaverSceneShadowColor", shadowColor);
            Shader.SetGlobalFloat("_FateWeaverColorOverlayStrength", overlayStrength);
            Shader.SetGlobalFloat("_FateWeaverShadowOpacity", activeProfile.shadowOpacity);
            Shader.SetGlobalColor("_FateWeaverAmbientTint", activeProfile.ambientTint);
            Shader.SetGlobalColor("_FateWeaverHighlightTint", activeProfile.highlightTint);
            Shader.SetGlobalColor("_FateWeaverShadowTint", activeProfile.shadowTint);
            Shader.SetGlobalFloat("_FateWeaverForegroundTintStrength", activeProfile.foregroundTintStrength);
            Shader.SetGlobalFloat("_FateWeaverUiTintStrength", activeProfile.uiTintStrength);
            Shader.SetGlobalFloat("_FateWeaverSaturation", activeProfile.saturation);
            Shader.SetGlobalFloat("_FateWeaverContrast", activeProfile.contrast);
            Shader.SetGlobalVector("_FateWeaverLightDirection", activeProfile.lightDirection);

            var camera = GetComponent<Camera>();
            if (camera)
                camera.backgroundColor = Color.Lerp(camera.backgroundColor, mainColor, .18f);

            ApplyTintableSprites(mainColor, overlayStrength);
            if (includeChildren)
                ApplyToChildren(mainColor);
        }

        private void ApplyTintableSprites(Color mainColor, float overlayStrength)
        {
            var taggedObjects = GameObject.FindGameObjectsWithTag("Tintable");
            foreach (var taggedObject in taggedObjects)
            {
                var spriteRenderer = taggedObject.GetComponent<SpriteRenderer>();
                if (spriteRenderer)
                    spriteRenderer.color = Color.Lerp(Color.white, mainColor, overlayStrength);
            }
        }

        private void ApplyToChildren(Color mainColor)
        {
            var renderers = GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var spriteRenderer in renderers)
                spriteRenderer.color = Color.Lerp(Color.white, mainColor, activeProfile.foregroundTintStrength);

            var graphics = GetComponentsInChildren<Graphic>(true);
            foreach (var graphic in graphics)
            {
                if (graphic is Text)
                    continue;
                graphic.color = Color.Lerp(graphic.color, activeProfile.ambientTint, activeProfile.uiTintStrength);
            }
        }
    }
}
