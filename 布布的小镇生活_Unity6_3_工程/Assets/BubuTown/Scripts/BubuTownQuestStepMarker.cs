using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownQuestStepMarker : MonoBehaviour
    {
        public string QuestId;
        public string StepId;
        public string StepName;
        [TextArea(2, 4)] public string StepHint;
        public string RequiredCompletedStepId;
        [TextArea(2, 3)] public string PrerequisiteMissingMessage;
        public int RequiredStepsForQuest = 1;
        public bool CompletesQuestOnInteract = true;
    }
}
