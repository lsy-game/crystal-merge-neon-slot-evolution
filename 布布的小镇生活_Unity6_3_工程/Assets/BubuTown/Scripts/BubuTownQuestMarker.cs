using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownQuestMarker : MonoBehaviour
    {
        public string QuestId;
        public string QuestName;
        public string StartsAt;
        public string Target;
        public int CoinReward;
        public int FavorReward;
        public bool PriorityForFirstPlayableLoop;
    }
}
