using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownFriendVisitSystem : MonoBehaviour
    {
        public BubuTownGameState State;
        public int RequiredWarmth = 20;
        public BubuTownHomeVisitorSpot[] VisitorSpots;

        private bool lastUnlocked;

        private void Start()
        {
            Refresh();
        }

        private void Update()
        {
            Refresh();
        }

        public string VisitSummary()
        {
            if (State == null)
            {
                return "朋友来访\n等待小屋进度系统。";
            }

            var unlocked = State.HomeWarmthScore() >= RequiredWarmth;
            return unlocked ? "朋友来访\n已开放：NPC 可以来小屋坐坐。" : "朋友来访\n温馨度 " + State.HomeWarmthScore() + "/" + RequiredWarmth + " 后开放。";
        }

        private void Refresh()
        {
            if (State == null || VisitorSpots == null)
            {
                return;
            }

            var unlocked = State.HomeWarmthScore() >= RequiredWarmth;
            if (unlocked == lastUnlocked && Application.isPlaying)
            {
                return;
            }

            lastUnlocked = unlocked;
            foreach (var spot in VisitorSpots)
            {
                if (spot != null)
                {
                    spot.Refresh(unlocked);
                }
            }
        }
    }
}
