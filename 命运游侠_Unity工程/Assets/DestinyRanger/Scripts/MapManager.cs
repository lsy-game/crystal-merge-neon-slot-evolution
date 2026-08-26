using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DestinyRanger
{
    public sealed class MapManager : MonoBehaviour
    {
        private readonly string[] nodeTypes = { "战斗", "事件", "祭坛", "商店", "Boss" };
        private RectTransform root;
        private Text eventText;
        private UnityAction<int> onNodeSelected;
        private int unlockedIndex;

        public void Build(RectTransform parent, Text log, UnityAction<int> selected)
        {
            root = parent;
            eventText = log;
            onNodeSelected = selected;
            unlockedIndex = 0;

            for (var i = 0; i < 8; i++)
            {
                var go = new GameObject("MapNode_" + i, typeof(Image), typeof(Button));
                go.transform.SetParent(root, false);
                var rect = go.GetComponent<RectTransform>();
                rect.sizeDelta = i == 7 ? new Vector2(150, 150) : new Vector2(120, 120);
                rect.anchoredPosition = new Vector2(-420 + i * 120, 250 + Mathf.Sin(i * 1.2f) * 140f);
                var image = go.GetComponent<Image>();
                image.sprite = FateWeaverGame.WhiteSprite;
                image.color = NodeColor(i);

                var label = FateWeaverGame.CreateText(go.transform, nodeTypes[Mathf.Min(i / 2, nodeTypes.Length - 1)], 26, Color.white, TextAnchor.MiddleCenter);
                label.rectTransform.sizeDelta = rect.sizeDelta;

                var index = i;
                go.GetComponent<Button>().onClick.AddListener(() => SelectNode(index));
            }
            SetEvent("选择发光节点进入房间");
        }

        private void SelectNode(int index)
        {
            if (index > unlockedIndex + 1)
            {
                SetEvent("路径未连通");
                return;
            }

            unlockedIndex = Mathf.Max(unlockedIndex, index);
            SetEvent("进入：" + nodeTypes[Mathf.Min(index / 2, nodeTypes.Length - 1)] + " / 事件：" + RandomEvent());
            onNodeSelected?.Invoke(index);
        }

        private Color NodeColor(int index)
        {
            switch (index)
            {
                case 1:
                case 2:
                    return new Color32(100, 200, 255, 220);
                case 3:
                    return new Color32(80, 200, 100, 220);
                case 5:
                    return new Color32(200, 80, 180, 220);
                case 7:
                    return new Color32(180, 50, 50, 240);
                default:
                    return new Color32(212, 175, 55, 230);
            }
        }

        private string RandomEvent()
        {
            var events = new[] { "星尘补给", "遗物抉择", "裂隙伏击", "命运丝线回收", "祭坛祝福" };
            return events[Random.Range(0, events.Length)];
        }

        private void SetEvent(string text)
        {
            if (eventText)
                eventText.text = text;
        }
    }
}
