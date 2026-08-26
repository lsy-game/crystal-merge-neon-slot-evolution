using UnityEngine;

namespace DestinyRanger
{
    [CreateAssetMenu(menuName = "Destiny Ranger/Scene Symbol Set")]
    public sealed class SceneSymbolSet : ScriptableObject
    {
        public string sceneId;
        public Sprite sword;
        public Sprite staff;
        public Sprite heart;
        public Sprite shield;
        public Sprite skull;
        public Sprite star;
        public Sprite[] symbols;
        public Sprite[] disabledSymbols;
        public Sprite[] highlightSymbols;
    }
}
