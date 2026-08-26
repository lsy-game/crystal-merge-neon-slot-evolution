using UnityEngine;

namespace DestinyRanger
{
    [CreateAssetMenu(menuName = "Destiny Ranger/Scene Tone Profile")]
    public sealed class SceneToneProfile : ScriptableObject
    {
        public Color sceneMainColor = Color.white;
        public Color sceneShadowColor = Color.black;
        [Range(0f, 1f)] public float colorOverlayStrength = .12f;
        [Range(0f, 1f)] public float shadowOpacity = .45f;
        public Color ambientTint = Color.white;
        public Color highlightTint = Color.white;
        public Color shadowTint = Color.black;
        [Range(0f, 1f)] public float foregroundTintStrength = .12f;
        [Range(0f, 1f)] public float uiTintStrength = .08f;
        [Range(.2f, 2f)] public float saturation = 1f;
        [Range(.2f, 2f)] public float contrast = 1f;
        public Vector2 lightDirection = new Vector2(-.6f, .8f);
        public string notes;
    }
}
