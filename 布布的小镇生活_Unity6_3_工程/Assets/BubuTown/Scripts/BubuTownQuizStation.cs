using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownQuizStation : MonoBehaviour
    {
        public string QuestId = "Q008";
        public string Question = "布布的小镇第一版地图中心是什么地方？";
        public string Answer = "中央广场";
        public string[] Choices = { "中央广场", "家具店仓库", "公园树下" };
        public int CorrectChoiceIndex = 0;
        [TextArea(2, 4)] public string PassMessage = "答对了：中央广场是小镇的中心。";
        [TextArea(2, 4)] public string FailMessage = "再想想：小镇的中心是出生时看到的大广场。";

        public string PromptText()
        {
            var text = "课后小测验\n" + Question;
            if (Choices != null)
            {
                for (var i = 0; i < Choices.Length; i++)
                {
                    text += "\n" + (i + 1) + ". " + Choices[i];
                }
            }

            return text + "\n按 1/2/3 作答";
        }

        public bool IsCorrectChoice(int choiceIndex)
        {
            return choiceIndex == CorrectChoiceIndex;
        }
    }
}
