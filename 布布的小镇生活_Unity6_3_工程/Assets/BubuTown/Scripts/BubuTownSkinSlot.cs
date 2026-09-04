using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownSkinSlot : MonoBehaviour
    {
        public string CharacterId;
        public string PublicOriginalSlotName = "PublicOriginal";
        public string LocalPrivateSlotName = "LocalPrivateSkinMount";
        public Transform PublicOriginalRoot;
        public Transform LocalPrivateSkinMount;
        public bool AllowLocalPrivateSkin;
        [TextArea(2, 4)] public string PolicyNote = "Public builds use original assets. Local private skins stay in ignored folders and are not committed.";
    }
}
