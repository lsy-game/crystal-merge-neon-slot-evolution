using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownDayEndPoint : MonoBehaviour
    {
        public string QuestId = "Q010";
        [TextArea(2, 4)] public string EndDayMessage = "今天也要回家。小镇进度已保存，明天继续慢慢布置。";
    }
}
