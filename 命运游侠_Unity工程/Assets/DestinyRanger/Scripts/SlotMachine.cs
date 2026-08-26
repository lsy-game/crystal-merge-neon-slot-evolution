using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DestinyRanger
{
    public sealed class SlotMachine : MonoBehaviour
    {
        public int energyCost = 30;
        public Sprite[] symbols;

        private readonly int[,] grid = new int[3, 3];
        private readonly int[] columnResults = new int[3];
        private readonly bool[] columnStopped = new bool[3];
        private readonly float[] columnSpeeds = new float[3];
        private Image[,] cells;
        private Button[] stopButtons;
        private Image[] buttonImages;
        private RectTransform[] columnRoots;
        private EnergySystem energySystem;
        private SkillManager skillManager;
        private RelicSystem relicSystem;
        private Text stateText;
        private bool active;

        public void Bind(
            EnergySystem energy,
            SkillManager skills,
            RelicSystem relics,
            Image[,] slotCells,
            RectTransform[] columns,
            Button[] buttons,
            Text status)
        {
            energySystem = energy;
            skillManager = skills;
            relicSystem = relics;
            cells = slotCells;
            columnRoots = columns;
            stopButtons = buttons;
            buttonImages = new Image[buttons.Length];
            stateText = status;

            for (var i = 0; i < buttons.Length; i++)
            {
                var index = i;
                buttonImages[i] = buttons[i].GetComponent<Image>();
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() => StopColumn(index));
            }

            RandomizeGrid();
            SetButtons(false);
        }

        public void Activate()
        {
            if (active || energySystem == null || !energySystem.ConsumeEnergy(energyCost))
                return;

            active = true;
            Time.timeScale = .3f;
            for (var x = 0; x < 3; x++)
            {
                columnStopped[x] = false;
                columnResults[x] = 0;
                columnSpeeds[x] = Random.Range(3000f, 5000f);
            }
            SetButtons(true);
            SetState("命运纺机启动：停止三列");
        }

        public void StopColumn(int columnIndex)
        {
            if (!active || columnStopped[columnIndex])
                return;

            columnResults[columnIndex] = GetCurrentSymbolIndex(columnIndex);
            for (var y = 0; y < 3; y++)
            {
                grid[columnIndex, y] = (columnResults[columnIndex] + y) % symbols.Length;
                cells[columnIndex, y].sprite = symbols[grid[columnIndex, y]];
            }

            columnStopped[columnIndex] = true;
            stopButtons[columnIndex].interactable = false;
            if (buttonImages[columnIndex])
                buttonImages[columnIndex].color = new Color(.35f, .38f, .45f, .8f);
            StartCoroutine(ColumnHalo(columnIndex));
            SetState("第 " + (columnIndex + 1) + " 列已锁定");

            if (AllColumnsStopped())
            {
                Time.timeScale = 1f;
                active = false;
                SetButtons(false);
                EvaluateResults();
            }
        }

        private void Update()
        {
            if (!active || cells == null)
                return;

            for (var x = 0; x < 3; x++)
            {
                if (columnStopped[x])
                    continue;

                var shift = Mathf.FloorToInt(Time.unscaledTime * columnSpeeds[x] / 180f);
                for (var y = 0; y < 3; y++)
                {
                    var index = Mathf.Abs(shift + x * 2 + y) % symbols.Length;
                    grid[x, y] = index;
                    cells[x, y].sprite = symbols[index];
                    cells[x, y].color = Color.Lerp(Color.white, new Color(1f, .85f, .45f), Mathf.PingPong(Time.unscaledTime * 3f + y, 1f) * .35f);
                }
            }
        }

        private int GetCurrentSymbolIndex(int columnIndex)
        {
            return grid[columnIndex, 1];
        }

        private void EvaluateResults()
        {
            relicSystem?.ApplyPreEvaluationRules(grid, 4, 2);
            RefreshGridSprites();

            var perfects = 0;
            var partials = 0;
            EvaluateLine(0, 0, 1, 0, 2, 0, ref perfects, ref partials);
            EvaluateLine(0, 1, 1, 1, 2, 1, ref perfects, ref partials);
            EvaluateLine(0, 2, 1, 2, 2, 2, ref perfects, ref partials);
            EvaluateLine(0, 0, 0, 1, 0, 2, ref perfects, ref partials);
            EvaluateLine(1, 0, 1, 1, 1, 2, ref perfects, ref partials);
            EvaluateLine(2, 0, 2, 1, 2, 2, ref perfects, ref partials);

            if (relicSystem == null || relicSystem.HasEffect(RelicEffect.EnableDiagonals))
            {
                EvaluateLine(0, 0, 1, 1, 2, 2, ref perfects, ref partials);
                EvaluateLine(2, 0, 1, 1, 0, 2, ref perfects, ref partials);
            }

            if (perfects == 0 && partials == 0)
                SetState("无连线：继续积攒能量");
            else
                SetState("结算完成：完美 " + perfects + " / 半完美 " + partials);
        }

        private void EvaluateLine(
            int ax, int ay,
            int bx, int by,
            int cx, int cy,
            ref int perfects,
            ref int partials)
        {
            var a = grid[ax, ay];
            var b = grid[bx, by];
            var c = grid[cx, cy];
            if (a == b && b == c)
            {
                perfects++;
                StartCoroutine(FlashLine(ax, ay, bx, by, cx, cy, true));
                skillManager?.TriggerPerfect(a, cells[bx, by].rectTransform);
                return;
            }

            if (a == 4 || b == 4 || c == 4)
                return;

            if (a == b || a == c || b == c)
            {
                partials++;
                StartCoroutine(FlashLine(ax, ay, bx, by, cx, cy, false));
                skillManager?.TriggerPartial(a == b || a == c ? a : b);
            }
        }

        private IEnumerator FlashLine(int ax, int ay, int bx, int by, int cx, int cy, bool perfect)
        {
            var flashes = perfect ? 3 : 1;
            var colorA = perfect ? new Color32(212, 175, 55, 255) : new Color32(100, 200, 255, 220);
            for (var i = 0; i < flashes; i++)
            {
                cells[ax, ay].color = colorA;
                cells[bx, by].color = colorA;
                cells[cx, cy].color = colorA;
                yield return new WaitForSecondsRealtime(.15f);
                cells[ax, ay].color = Color.white;
                cells[bx, by].color = Color.white;
                cells[cx, cy].color = Color.white;
                yield return new WaitForSecondsRealtime(.15f);
            }
        }

        private IEnumerator ColumnHalo(int columnIndex)
        {
            var root = columnRoots[columnIndex];
            var start = root.localScale;
            for (var t = 0f; t < 1f; t += Time.unscaledDeltaTime / .2f)
            {
                var s = 1f + Mathf.Sin(t * Mathf.PI) * .18f;
                root.localScale = start * s;
                yield return null;
            }
            root.localScale = start;
        }

        private bool AllColumnsStopped()
        {
            return columnStopped[0] && columnStopped[1] && columnStopped[2];
        }

        private void RandomizeGrid()
        {
            for (var x = 0; x < 3; x++)
            for (var y = 0; y < 3; y++)
                grid[x, y] = Random.Range(0, Mathf.Max(1, symbols.Length));
            RefreshGridSprites();
        }

        private void RefreshGridSprites()
        {
            if (cells == null || symbols == null || symbols.Length == 0)
                return;

            for (var x = 0; x < 3; x++)
            for (var y = 0; y < 3; y++)
            {
                cells[x, y].sprite = symbols[grid[x, y]];
                cells[x, y].color = Color.white;
            }
        }

        private void SetButtons(bool enabled)
        {
            if (stopButtons == null)
                return;

            for (var i = 0; i < stopButtons.Length; i++)
            {
                stopButtons[i].interactable = enabled;
                if (buttonImages[i])
                    buttonImages[i].color = enabled ? new Color32(100, 200, 255, 230) : new Color32(90, 94, 105, 180);
            }
        }

        private void SetState(string text)
        {
            if (stateText)
                stateText.text = text;
        }
    }
}
