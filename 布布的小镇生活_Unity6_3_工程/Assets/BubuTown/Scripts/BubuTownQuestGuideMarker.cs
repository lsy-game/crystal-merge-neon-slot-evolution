using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownQuestGuideMarker : MonoBehaviour
    {
        public string QuestId;
        public string StepId;
        public string DisplayName;
        public int GuideOrder;
        public Vector3 WorldTarget;
        [TextArea(2, 4)] public string GuidanceText;
    }
}
