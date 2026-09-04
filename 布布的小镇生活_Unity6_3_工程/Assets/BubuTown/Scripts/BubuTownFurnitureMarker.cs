using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownFurnitureMarker : MonoBehaviour
    {
        public string FurnitureId;
        public string FurnitureName;
        public int Price;
        public int WarmthValue;
        public int RequiredWarmth;
        public bool StartsInShopCatalog = true;
        public bool PlacedInStarterHome;
    }
}
