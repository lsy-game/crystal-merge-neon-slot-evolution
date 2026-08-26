using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DestinyRanger
{
    public sealed class FateWeaverGame : MonoBehaviour
    {
        public Sprite chamberBackground;
        public Sprite battleBackground;
        public Texture2D symbolSheet;

        public static Sprite WhiteSprite { get; private set; }

        private static readonly Color32 DeepBlue = new Color32(10, 15, 30, 255);
        private static readonly Color32 Gold = new Color32(212, 175, 55, 255);
        private static readonly Color32 EnergyBlue = new Color32(100, 200, 255, 255);
        private static readonly Color32 LifeGreen = new Color32(80, 200, 100, 255);
        private static readonly Color32 MagicPink = new Color32(200, 80, 180, 255);
        private static readonly Color32 WarningRed = new Color32(180, 50, 50, 255);
        private static readonly Color32 TextWhite = new Color32(240, 235, 220, 255);
        private static readonly Color32 MaskBlack = new Color32(0, 0, 0, 180);

        private Canvas canvas;
        private RectTransform root;
        private RectTransform loginPanel;
        private RectTransform mainPanel;
        private RectTransform battlePanel;
        private RectTransform mapPanel;
        private RectTransform weavePanel;
        private Image fade;
        private Sprite[] symbolSprites;
        private EnergySystem energySystem;
        private SkillManager skillManager;
        private RelicSystem relicSystem;
        private SlotMachine slotMachine;
        private MapManager mapManager;
        private Text currencyText;
        private Text battleLog;
        private float largeOrbTimer;

        private void Awake()
        {
            EnsureWhiteSprite();
            symbolSprites = BuildSymbolSprites();
            BuildCanvas();
            energySystem = gameObject.AddComponent<EnergySystem>();
            skillManager = gameObject.AddComponent<SkillManager>();
            relicSystem = gameObject.AddComponent<RelicSystem>();
            slotMachine = gameObject.AddComponent<SlotMachine>();
            slotMachine.symbols = symbolSprites;
            mapManager = gameObject.AddComponent<MapManager>();

            BuildLogin();
            BuildMain();
            BuildBattle();
            BuildMap();
            BuildWeave();
            ShowOnly(loginPanel);
        }

        private void Update()
        {
            if (!battlePanel || !battlePanel.gameObject.activeSelf)
                return;

            largeOrbTimer -= Time.unscaledDeltaTime;
            if (largeOrbTimer <= 0f)
            {
                largeOrbTimer = Random.Range(8f, 12f);
                SpawnLargeOrb();
            }
        }

        private void BuildCanvas()
        {
            var canvasGo = new GameObject("FateWeaverCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGo.transform.SetParent(transform, false);
            canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1290, 2796);
            scaler.matchWidthOrHeight = 1f;
            root = canvasGo.GetComponent<RectTransform>();

            fade = CreateImage(root, "Fade", Vector2.zero, new Vector2(1290, 2796), Color.clear);
            fade.raycastTarget = false;
            fade.transform.SetAsLastSibling();
        }

        private void BuildLogin()
        {
            loginPanel = CreatePanel("LoginScreen");
            CreateImage(loginPanel, "LoginDeepBlue", Vector2.zero, new Vector2(1290, 2796), DeepBlue);

            var spindle = CreateImage(loginPanel, "SpindleSilhouette", new Vector2(0, 360), new Vector2(600, 600), new Color32(212, 175, 55, 70));
            spindle.type = Image.Type.Simple;
            StartCoroutine(RotateLoop(spindle.rectTransform, 10f));

            for (var i = 0; i < 50; i++)
            {
                var p = CreateImage(loginPanel, "LoginParticle_" + i, Vector2.zero, Vector2.one * Random.Range(4, 9), i % 3 == 0 ? Gold : TextWhite);
                StartCoroutine(Orbit(p.rectTransform, Random.Range(250f, 430f), i * 7.2f, 10f));
            }

            CreateText(loginPanel, "命运纺机", 80, Gold, TextAnchor.MiddleCenter, new Vector2(0, -140), new Vector2(760, 110));
            var login = CreateButton(loginPanel, "登录游戏", new Vector2(0, -440), new Vector2(280, 100), Gold);
            login.onClick.AddListener(() => StartCoroutine(LoginTransition(spindle.rectTransform)));
            var switchAccount = CreateButton(loginPanel, "切换账号", new Vector2(0, -560), new Vector2(240, 80), new Color32(80, 84, 100, 160));
            switchAccount.onClick.AddListener(() => Toast(loginPanel, "账号系统预留"));
        }

        private void BuildMain()
        {
            mainPanel = CreatePanel("Main_命运织室");
            AddBackground(mainPanel, chamberBackground, "GeneratedChamberBackground");
            CreateRoomProps(mainPanel);

            var machine = CreateMachine(mainPanel, new Vector2(0, -430), new Vector2(800, 900), false);
            StartCoroutine(Pulse(machine, .02f, 2f));

            var hero = CreateImage(mainPanel, "Hero_Aileen_Idle", new Vector2(-345, -250), new Vector2(220, 520), MagicPink);
            CreateText(hero.transform, "艾琳", 28, TextWhite, TextAnchor.MiddleCenter, new Vector2(0, -285), new Vector2(180, 40));
            StartCoroutine(HeroIdle(hero.rectTransform));

            BuildTopHud(mainPanel);
            BuildBottomMenu(mainPanel);
            BuildFloatingButtons(mainPanel);
        }

        private void BuildBattle()
        {
            battlePanel = CreatePanel("BattleScreen");
            AddBackground(battlePanel, battleBackground, "GeneratedBattleBackground");
            CreateImage(battlePanel, "BattleTopDim", new Vector2(0, 700), new Vector2(1290, 1398), new Color32(0, 0, 0, 45));

            var hero = CreateImage(battlePanel, "BattleHero", new Vector2(-345, 700), new Vector2(210, 430), MagicPink);
            StartCoroutine(HeroIdle(hero.rectTransform));
            for (var i = 0; i < 3; i++)
            {
                var enemy = CreateImage(battlePanel, "Enemy_" + i, new Vector2(260 + i * 135, 710 + i * 30), new Vector2(155, 220), new Color32(60, 180, 120, 220));
                StartCoroutine(EnemyLoop(enemy.rectTransform, i));
            }

            var hpBg = CreateImage(battlePanel, "HpBarBg", new Vector2(-345, 60), new Vector2(400, 20), new Color32(70, 25, 25, 255));
            var hpFill = CreateImage(hpBg.rectTransform, "HpBarFill", Vector2.zero, new Vector2(400, 20), WarningRed);
            hpFill.type = Image.Type.Filled;
            hpFill.fillMethod = Image.FillMethod.Horizontal;
            hpFill.fillAmount = 1f;

            var shieldBg = CreateImage(battlePanel, "ShieldBg", new Vector2(-345, 28), new Vector2(400, 10), new Color32(30, 55, 70, 220));
            var shieldFill = CreateImage(shieldBg.rectTransform, "ShieldFill", Vector2.zero, new Vector2(400, 10), EnergyBlue);
            shieldFill.type = Image.Type.Filled;
            shieldFill.fillMethod = Image.FillMethod.Horizontal;
            shieldFill.fillAmount = 0f;

            var energyShell = CreateImage(battlePanel, "EnergySpindle", new Vector2(0, 60), new Vector2(300, 40), new Color32(22, 40, 70, 230));
            var energyFill = CreateImage(energyShell.rectTransform, "EnergyFill", Vector2.zero, new Vector2(300, 40), EnergyBlue);
            energyFill.type = Image.Type.Filled;
            energyFill.fillMethod = Image.FillMethod.Horizontal;
            var energyValue = CreateText(battlePanel, "0/100", 24, TextWhite, TextAnchor.MiddleCenter, new Vector2(0, 18), new Vector2(220, 32));
            energySystem.Bind(energyFill, energyValue, energyShell.rectTransform);
            energySystem.OnEnergyFull += () => slotMachine.Activate();

            for (var i = 0; i < 4; i++)
                CreateImage(battlePanel, "BuffIcon_" + i, new Vector2(450 + i * 55, 55), new Vector2(40, 40), i % 2 == 0 ? Gold : LifeGreen);

            var slotStatus = CreateText(battlePanel, "能量 >= 30 可启动纺机", 28, TextWhite, TextAnchor.MiddleCenter, new Vector2(0, -205), new Vector2(720, 44));
            var slotCells = new Image[3, 3];
            var columns = new RectTransform[3];
            var slotRoot = CreateMachine(battlePanel, new Vector2(0, -555), new Vector2(580, 580), true);
            for (var x = 0; x < 3; x++)
            {
                columns[x] = new GameObject("SlotColumn_" + x, typeof(RectTransform)).GetComponent<RectTransform>();
                columns[x].SetParent(slotRoot, false);
                columns[x].sizeDelta = new Vector2(180, 580);
                columns[x].anchoredPosition = new Vector2((x - 1) * 200, 0);
                for (var y = 0; y < 3; y++)
                    slotCells[x, y] = CreateImage(columns[x], "SlotCell_" + x + "_" + y, new Vector2(0, (1 - y) * 200), new Vector2(180, 180), new Color32(20, 28, 46, 255));
            }

            var buttons = new Button[3];
            for (var i = 0; i < 3; i++)
                buttons[i] = CreateButton(battlePanel, "停", new Vector2((i - 1) * 200, -1190), new Vector2(160, 160), EnergyBlue, true);
            slotMachine.Bind(energySystem, skillManager, relicSystem, slotCells, columns, buttons, slotStatus);

            battleLog = CreateText(battlePanel, "战斗开始", 28, Gold, TextAnchor.MiddleCenter, new Vector2(0, 225), new Vector2(760, 44));
            var flash = CreateImage(battlePanel, "CombatFlash", Vector2.zero, new Vector2(1290, 2796), Color.clear);
            flash.gameObject.SetActive(false);
            skillManager.Bind(battlePanel, battleLog, hpFill, shieldFill, flash);

            var charge = CreateButton(battlePanel, "能量+25", new Vector2(430, -1220), new Vector2(190, 70), Gold);
            charge.onClick.AddListener(() => AddEnergyOrb(25, charge.GetComponent<RectTransform>().anchoredPosition));
        }

        private void BuildMap()
        {
            mapPanel = CreatePanel("AdventureMap");
            CreateImage(mapPanel, "MapParchment", Vector2.zero, new Vector2(1180, 2400), new Color32(54, 43, 36, 235));
            CreateText(mapPanel, "裂隙地图", 72, Gold, TextAnchor.MiddleCenter, new Vector2(0, 1050), new Vector2(500, 100));
            var log = CreateText(mapPanel, "选择节点", 34, TextWhite, TextAnchor.MiddleCenter, new Vector2(0, -900), new Vector2(900, 80));
            mapManager.Build(mapPanel, log, i => { if (i == 0 || i == 7) StartCoroutine(TransitionTo(battlePanel)); });
            CreateButton(mapPanel, "返回", new Vector2(0, -1110), new Vector2(240, 86), Gold).onClick.AddListener(() => StartCoroutine(TransitionTo(mainPanel)));
        }

        private void BuildWeave()
        {
            weavePanel = CreatePanel("WeaveScreen");
            AddBackground(weavePanel, chamberBackground, "WeaveChamberZoom");
            CreateText(weavePanel, "编织界面", 72, Gold, TextAnchor.MiddleCenter, new Vector2(0, 980), new Vector2(600, 100));
            CreateMachine(weavePanel, Vector2.zero, new Vector2(900, 1000), false);
            CreateText(weavePanel, "外壳展开 / 遗物与符号调整预留", 36, TextWhite, TextAnchor.MiddleCenter, new Vector2(0, -760), new Vector2(900, 90));
            CreateButton(weavePanel, "返回", new Vector2(0, -1110), new Vector2(240, 86), Gold).onClick.AddListener(() => StartCoroutine(TransitionTo(mainPanel)));
        }

        private void BuildTopHud(RectTransform parent)
        {
            CreateImage(parent, "AvatarFrame", new Vector2(-545, 1278), new Vector2(100, 100), Gold);
            CreateText(parent, "裂隙行者", 32, TextWhite, TextAnchor.MiddleLeft, new Vector2(-420, 1300), new Vector2(260, 40));
            CreateText(parent, "Lv.15", 28, Gold, TextAnchor.MiddleLeft, new Vector2(-420, 1260), new Vector2(160, 36));
            var staminaBg = CreateImage(parent, "StaminaBg", new Vector2(-435, 1196), new Vector2(200, 16), new Color32(72, 54, 18, 255));
            CreateImage(staminaBg.rectTransform, "StaminaFill", Vector2.zero, new Vector2(130, 16), Gold);
            CreateText(parent, "32/100", 24, TextWhite, TextAnchor.MiddleCenter, new Vector2(-435, 1196), new Vector2(180, 28));

            currencyText = CreateText(parent, "⚙ 9999     ◇ 9999     ✦ 99", 28, TextWhite, TextAnchor.MiddleRight, new Vector2(350, 1280), new Vector2(760, 46));
        }

        private void BuildBottomMenu(RectTransform parent)
        {
            CreateImage(parent, "BottomParchmentBar", new Vector2(0, -1298), new Vector2(1290, 200), new Color32(54, 43, 36, 235));
            var labels = new[] { "冒险", "英雄", "编织", "工坊", "任务" };
            for (var i = 0; i < labels.Length; i++)
            {
                var button = CreateButton(parent, labels[i], new Vector2(-480 + i * 240, -1295), new Vector2(120, 120), new Color32(72, 62, 52, 210));
                var index = i;
                button.onClick.AddListener(() =>
                {
                    StartCoroutine(Bounce(button.transform));
                    if (index == 0) StartCoroutine(TransitionTo(mapPanel));
                    else if (index == 2) StartCoroutine(TransitionTo(weavePanel));
                    else Toast(mainPanel, labels[index] + " 功能预留");
                });
            }
        }

        private void BuildFloatingButtons(RectTransform parent)
        {
            var labels = new[] { "✉", "日", "友" };
            for (var i = 0; i < labels.Length; i++)
            {
                var button = CreateButton(parent, labels[i], new Vector2(560, 590 - i * 100), new Vector2(60, 60), new Color32(30, 36, 54, 178), true);
                if (i < 2)
                    CreateImage(button.transform, "UnreadDot", new Vector2(24, 24), new Vector2(20, 20), WarningRed);
            }
        }

        private RectTransform CreateMachine(RectTransform parent, Vector2 pos, Vector2 size, bool emptySlots)
        {
            var rootMachine = new GameObject("FateSpinningMachine", typeof(RectTransform)).GetComponent<RectTransform>();
            rootMachine.SetParent(parent, false);
            rootMachine.sizeDelta = size;
            rootMachine.anchoredPosition = pos;
            CreateImage(rootMachine, "BrassFrame", Vector2.zero, size, new Color32(90, 70, 25, 230));
            CreateImage(rootMachine, "InnerCrystal", Vector2.zero, size - new Vector2(70, 100), new Color32(20, 30, 55, 230));
            for (var x = 0; x < 3; x++)
            for (var y = 0; y < 3; y++)
            {
                var cell = CreateImage(rootMachine, "VisibleSymbolSlot_" + x + "_" + y, new Vector2((x - 1) * 200, (1 - y) * 200), new Vector2(180, 180), new Color32(12, 20, 38, 245));
                if (!emptySlots && symbolSprites.Length > 0)
                    cell.sprite = symbolSprites[(x + y * 2) % symbolSprites.Length];
            }
            return rootMachine;
        }

        private void CreateRoomProps(RectTransform parent)
        {
            CreateImage(parent, "WindowStarNight", new Vector2(-420, 740), new Vector2(400, 600), new Color32(15, 28, 60, 180));
            for (var i = 0; i < 8; i++)
            {
                var star = CreateImage(parent, "WindowStar_" + i, new Vector2(-550 + Random.Range(0, 260), 550 + Random.Range(0, 340)), Vector2.one * Random.Range(8, 16), TextWhite);
                StartCoroutine(Pulse(star.rectTransform, .3f, Random.Range(2f, 3f)));
            }
            CreateImage(parent, "BookcaseSilhouette", new Vector2(435, 640), new Vector2(260, 760), new Color32(4, 7, 12, 150));
            CreateImage(parent, "CandleLeft", new Vector2(-360, -650), new Vector2(48, 180), Gold);
            CreateImage(parent, "CandleRight", new Vector2(360, -650), new Vector2(48, 180), Gold);
        }

        private void AddBackground(RectTransform parent, Sprite sprite, string name)
        {
            var bg = CreateImage(parent, name, Vector2.zero, new Vector2(1290, 2796), DeepBlue);
            if (sprite)
            {
                bg.sprite = sprite;
                bg.preserveAspect = false;
                bg.color = Color.white;
            }
        }

        private void SpawnLargeOrb()
        {
            var button = CreateButton(battlePanel, "", new Vector2(0, 680), new Vector2(110, 110), Gold, true);
            button.name = "LargeEnergyOrb";
            var rect = button.GetComponent<RectTransform>();
            button.onClick.AddListener(() =>
            {
                AddEnergyOrb(25, rect.anchoredPosition);
                Destroy(button.gameObject);
            });
            StartCoroutine(OrbDrift(rect));
        }

        private void AddEnergyOrb(int amount, Vector2 pos)
        {
            energySystem.AddEnergy(amount);
            var text = CreateText(battlePanel, "能量+" + amount + "！", 34, Gold, TextAnchor.MiddleCenter, pos + Vector2.up * 80, new Vector2(220, 50));
            StartCoroutine(FloatAndDestroy(text.rectTransform, text, .8f));
            if (energySystem.currentEnergy >= 30)
                slotMachine.Activate();
        }

        private IEnumerator LoginTransition(RectTransform spindle)
        {
            for (var t = 0f; t < .5f; t += Time.unscaledDeltaTime)
            {
                spindle.Rotate(Vector3.forward, 720f * Time.unscaledDeltaTime);
                yield return null;
            }
            yield return Flash(Color.white, .3f);
            yield return TransitionTo(mainPanel);
        }

        private IEnumerator TransitionTo(RectTransform target)
        {
            yield return Fade(0f, 1f, .22f);
            ShowOnly(target);
            if (target == mainPanel)
                StartCoroutine(MainEntrance());
            yield return Fade(1f, 0f, .38f);
        }

        private IEnumerator MainEntrance()
        {
            mainPanel.localScale = Vector3.one * .98f;
            for (var t = 0f; t < 1f; t += Time.unscaledDeltaTime / .8f)
            {
                mainPanel.localScale = Vector3.Lerp(Vector3.one * .98f, Vector3.one, Mathf.SmoothStep(0, 1, t));
                yield return null;
            }
        }

        private void ShowOnly(RectTransform panel)
        {
            loginPanel.gameObject.SetActive(panel == loginPanel);
            mainPanel.gameObject.SetActive(panel == mainPanel);
            battlePanel.gameObject.SetActive(panel == battlePanel);
            mapPanel.gameObject.SetActive(panel == mapPanel);
            weavePanel.gameObject.SetActive(panel == weavePanel);
            fade.transform.SetAsLastSibling();
        }

        private IEnumerator Fade(float from, float to, float duration)
        {
            fade.gameObject.SetActive(true);
            for (var t = 0f; t < 1f; t += Time.unscaledDeltaTime / duration)
            {
                fade.color = new Color(0, 0, 0, Mathf.Lerp(from, to, t));
                yield return null;
            }
            fade.color = new Color(0, 0, 0, to);
            if (Mathf.Approximately(to, 0f))
                fade.gameObject.SetActive(false);
        }

        private IEnumerator Flash(Color color, float duration)
        {
            fade.gameObject.SetActive(true);
            for (var t = 0f; t < 1f; t += Time.unscaledDeltaTime / duration)
            {
                fade.color = new Color(color.r, color.g, color.b, Mathf.Sin(t * Mathf.PI));
                yield return null;
            }
            fade.color = Color.clear;
            fade.gameObject.SetActive(false);
        }

        private IEnumerator Orbit(RectTransform rect, float radius, float startAngle, float speed)
        {
            while (rect)
            {
                var angle = (startAngle + Time.unscaledTime * speed) * Mathf.Deg2Rad;
                rect.anchoredPosition = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius + Vector2.up * 360;
                yield return null;
            }
        }

        private IEnumerator RotateLoop(RectTransform rect, float degreesPerSecond)
        {
            while (rect)
            {
                rect.Rotate(Vector3.forward, degreesPerSecond * Time.unscaledDeltaTime);
                yield return null;
            }
        }

        private IEnumerator Pulse(RectTransform rect, float amount, float period)
        {
            var baseScale = rect.localScale;
            while (rect)
            {
                var s = 1f + Mathf.Sin(Time.unscaledTime / period * Mathf.PI * 2f) * amount;
                rect.localScale = baseScale * s;
                yield return null;
            }
        }

        private IEnumerator HeroIdle(RectTransform rect)
        {
            while (rect)
            {
                rect.anchoredPosition += Vector2.up * (Mathf.Sin(Time.unscaledTime * 2f) * .15f);
                rect.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.unscaledTime * .7f) * 1.5f);
                yield return null;
            }
        }

        private IEnumerator EnemyLoop(RectTransform rect, int offset)
        {
            while (rect)
            {
                rect.anchoredPosition += Vector2.left * (Mathf.Sin(Time.time * 1.5f + offset) * .2f);
                yield return null;
            }
        }

        private IEnumerator Bounce(Transform target)
        {
            target.localScale = Vector3.one * .9f;
            yield return new WaitForSecondsRealtime(.08f);
            target.localScale = Vector3.one * 1.05f;
            yield return new WaitForSecondsRealtime(.12f);
            target.localScale = Vector3.one;
        }

        private IEnumerator OrbDrift(RectTransform rect)
        {
            var start = rect.anchoredPosition;
            var end = start + Vector2.left * 480f;
            for (var t = 0f; t < 1f && rect; t += Time.unscaledDeltaTime / 4.8f)
            {
                rect.anchoredPosition = Vector2.Lerp(start, end, t);
                rect.localScale = Vector3.one * (1f + Mathf.Sin(Time.unscaledTime * 7f) * .08f);
                yield return null;
            }
            if (rect) Destroy(rect.gameObject);
        }

        private IEnumerator FloatAndDestroy(RectTransform rect, Graphic graphic, float duration)
        {
            var start = rect.anchoredPosition;
            for (var t = 0f; t < 1f; t += Time.unscaledDeltaTime / duration)
            {
                rect.anchoredPosition = start + Vector2.up * (80f * t);
                graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 1f - t);
                yield return null;
            }
            Destroy(rect.gameObject);
        }

        private void Toast(RectTransform parent, string message)
        {
            var panel = CreateImage(parent, "Toast", new Vector2(0, 0), new Vector2(900, 180), MaskBlack);
            var text = CreateText(panel.transform, message, 36, TextWhite, TextAnchor.MiddleCenter, Vector2.zero, new Vector2(860, 120));
            StartCoroutine(FloatAndDestroy(panel.rectTransform, panel, 1.2f));
            StartCoroutine(FloatAndDestroy(text.rectTransform, text, 1.2f));
        }

        private RectTransform CreatePanel(string name)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(root, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return rect;
        }

        private static Image CreateImage(Transform parent, string name, Vector2 pos, Vector2 size, Color color)
        {
            var go = new GameObject(name, typeof(Image));
            go.transform.SetParent(parent, false);
            var image = go.GetComponent<Image>();
            image.sprite = WhiteSprite;
            image.color = color;
            image.raycastTarget = false;
            image.rectTransform.sizeDelta = size;
            image.rectTransform.anchoredPosition = pos;
            return image;
        }

        public static Text CreateText(Transform parent, string text, int size, Color color, TextAnchor anchor)
        {
            return CreateText(parent, text, size, color, anchor, Vector2.zero, new Vector2(300, 60));
        }

        private static Text CreateText(Transform parent, string text, int size, Color color, TextAnchor anchor, Vector2 pos, Vector2 box)
        {
            var go = new GameObject("Text_" + text, typeof(Text));
            go.transform.SetParent(parent, false);
            var label = go.GetComponent<Text>();
            label.text = text;
            label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            label.fontSize = size;
            label.color = color;
            label.alignment = anchor;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Truncate;
            label.rectTransform.anchoredPosition = pos;
            label.rectTransform.sizeDelta = box;
            return label;
        }

        private static Button CreateButton(Transform parent, string text, Vector2 pos, Vector2 size, Color color, bool round = false)
        {
            var go = new GameObject("Button_" + text, typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var image = go.GetComponent<Image>();
            image.sprite = WhiteSprite;
            image.color = color;
            image.type = Image.Type.Sliced;
            image.rectTransform.anchoredPosition = pos;
            image.rectTransform.sizeDelta = size;
            var button = go.GetComponent<Button>();
            var colors = button.colors;
            colors.pressedColor = color * .8f;
            colors.highlightedColor = Color.Lerp(color, Color.white, .15f);
            button.colors = colors;
            if (!string.IsNullOrEmpty(text))
                CreateText(go.transform, text, size.y >= 100 ? 32 : 26, TextWhite, TextAnchor.MiddleCenter, Vector2.zero, size);
            return button;
        }

        private Sprite[] BuildSymbolSprites()
        {
            if (!symbolSheet)
                return BuildFallbackSymbols();

            var result = new Sprite[6];
            var cellW = symbolSheet.width / 3;
            var cellH = symbolSheet.height / 2;
            for (var i = 0; i < 6; i++)
            {
                var x = i % 3;
                var y = 1 - i / 3;
                result[i] = Sprite.Create(symbolSheet, new Rect(x * cellW, y * cellH, cellW, cellH), new Vector2(.5f, .5f), 100f);
            }
            return result;
        }

        private static Sprite[] BuildFallbackSymbols()
        {
            var colors = new Color[] { Color.gray, MagicPink, Color.red, EnergyBlue, Color.black, Gold };
            var sprites = new Sprite[6];
            for (var i = 0; i < sprites.Length; i++)
            {
                var tex = new Texture2D(128, 128, TextureFormat.RGBA32, false);
                for (var x = 0; x < 128; x++)
                for (var y = 0; y < 128; y++)
                {
                    var d = Vector2.Distance(new Vector2(x, y), new Vector2(64, 64));
                    tex.SetPixel(x, y, d < 52 ? colors[i] : new Color(0, 0, 0, 0));
                }
                tex.Apply();
                sprites[i] = Sprite.Create(tex, new Rect(0, 0, 128, 128), new Vector2(.5f, .5f), 100f);
            }
            return sprites;
        }

        private static void EnsureWhiteSprite()
        {
            if (WhiteSprite)
                return;
            var tex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
            for (var x = 0; x < 4; x++)
            for (var y = 0; y < 4; y++)
                tex.SetPixel(x, y, Color.white);
            tex.Apply();
            WhiteSprite = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(.5f, .5f), 100f);
        }
    }
}
