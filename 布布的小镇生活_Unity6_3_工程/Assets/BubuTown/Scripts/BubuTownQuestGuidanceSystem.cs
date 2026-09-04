using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownQuestGuidanceSystem : MonoBehaviour
    {
        public BubuTownGameState State;

        public string CurrentGuideSummary()
        {
            if (State == null || State.ActiveQuestIds.Count == 0)
            {
                return "任务引导\n去找 NPC 或公告牌接任务。";
            }

            foreach (var questId in State.ActiveQuestIds)
            {
                var marker = NextMarkerForQuest(questId);
                if (marker != null)
                {
                    return "任务引导\n" + State.QuestNameForId(questId) + "\n" + marker.GuidanceText;
                }
            }

            return "任务引导\n查看任务面板确认下一步。";
        }

        private BubuTownQuestGuideMarker NextMarkerForQuest(string questId)
        {
            BubuTownQuestGuideMarker best = null;
            foreach (var marker in FindObjectsOfType<BubuTownQuestGuideMarker>())
            {
                if (marker.QuestId != questId)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(marker.StepId) && State.IsQuestStepComplete(marker.StepId))
                {
                    continue;
                }

                if (best == null || marker.GuideOrder < best.GuideOrder)
                {
                    best = marker;
                }
            }

            return best;
        }
    }
}
