using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownHomeProgress : MonoBehaviour
    {
        public BubuTownGameState State;
        public int FirstBedTarget = 4;
        public int FirstWarmthTarget = 12;
        public int FriendUnlockTarget = 20;
        public string UnlockNote = "温馨度提升后，解锁新家具、新任务和更亲近的 NPC 对话。";
        public string FirstBedUnlock = "温馨度达到 4：小屋有了第一处舒服角落，家具店推荐小书柜和台灯。";
        public string FirstRoomUnlock = "温馨度达到 12：第一阶段小屋成型，新的小镇对话和家具目标已解锁。";
        public string FriendVisitUnlock = "温馨度达到 20：朋友来访阶段解锁，NPC 会更期待参观布布的小屋。";

        private void Update()
        {
            if (State == null)
            {
                return;
            }

            State.HomeWarmthTarget = FirstWarmthTarget;
            var warmth = State.HomeWarmthScore();
            TryUnlock(warmth, FirstBedTarget, FirstBedUnlock);
            TryUnlock(warmth, FirstWarmthTarget, FirstRoomUnlock);
            TryUnlock(warmth, FriendUnlockTarget, FriendVisitUnlock);
        }

        private void TryUnlock(int warmth, int target, string message)
        {
            if (warmth >= target && !State.IsWarmthMilestoneUnlocked(target))
            {
                State.UnlockWarmthMilestone(target, message);
            }
        }
    }
}
