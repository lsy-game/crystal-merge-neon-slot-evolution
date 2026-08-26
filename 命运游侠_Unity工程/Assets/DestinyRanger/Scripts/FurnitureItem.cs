using UnityEngine;

namespace DestinyRanger
{
    [CreateAssetMenu(menuName = "Destiny Ranger/Furniture Item")]
    public sealed class FurnitureItem : ScriptableObject
    {
        public string itemId;
        public string displayName;
        public string category;
        public Vector2 sizePixels;
        public string unlockCondition;
        public Sprite sprite;
        public Sprite shadowSprite;
    }
}
