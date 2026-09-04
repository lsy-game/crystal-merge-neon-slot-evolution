using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownInteractable : MonoBehaviour
    {
        public BubuTownInteractableType Type;
        public string Id;
        public string DisplayName;
        [TextArea(2, 4)] public string InteractionPrompt;
        public bool VisibleInHierarchyBeforePlay = true;
    }
}
