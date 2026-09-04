using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownHomeVisitorSpot : MonoBehaviour
    {
        public string VisitorName;
        public int RequiredWarmth = 20;
        public TextMesh StatusLabel;
        [TextArea(2, 3)] public string LockedMessage = "温馨度 20 后开放朋友来访。";
        [TextArea(2, 3)] public string UnlockedMessage = "可以邀请这位朋友来小屋坐坐。";
        [TextArea(2, 3)] public string VisitMessage = "朋友在小屋里坐了一会儿，屋子更像家了。";

        public void Refresh(bool unlocked)
        {
            if (StatusLabel == null)
            {
                return;
            }

            StatusLabel.text = VisitorName + "\n" + (unlocked ? UnlockedMessage : LockedMessage);
        }

        public string Interact(BubuTownGameState state)
        {
            if (state == null)
            {
                return "朋友来访系统还没有连接小屋进度。";
            }

            var warmth = state.HomeWarmthScore();
            if (warmth < RequiredWarmth)
            {
                return VisitorName + " 还没准备来访：小屋温馨度 " + warmth + "/" + RequiredWarmth + "。";
            }

            return VisitorName + " 来小屋坐坐了。" + VisitMessage;
        }
    }
}
