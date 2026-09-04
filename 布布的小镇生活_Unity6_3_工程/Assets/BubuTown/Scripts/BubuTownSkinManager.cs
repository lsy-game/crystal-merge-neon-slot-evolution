using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownSkinManager : MonoBehaviour
    {
        public string PublicOriginalPath = "Assets/BubuTown/Characters/PublicOriginal/";
        public string IgnoredPrivateSkinsPath = "Assets/PrivateSkins/";
        public string IgnoredLocalOnlyPath = "Assets/Characters/LocalOnly/";
        public BubuTownSkinSlot[] SkinSlots;

        public void RefreshSlotsFromScene()
        {
            SkinSlots = FindObjectsOfType<BubuTownSkinSlot>();
        }
    }
}
