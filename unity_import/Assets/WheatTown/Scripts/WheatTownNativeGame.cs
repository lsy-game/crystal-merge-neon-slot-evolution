using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WheatTown
{
    public sealed class WheatTownNativeGame : MonoBehaviour
    {
        private enum Page { Town, Bag, Task }
        private enum PlotState { Locked, Empty, GrowingOne, GrowingTwo, ReadyWheat }
        private enum BagTab { All, Crops, Processed, Materials, Collection }
        private enum TaskTab { Orders, Residents, DailyRoute }

        [Header("Backgrounds")]
        [SerializeField] private Sprite lobbyBackground;
        [SerializeField] private Sprite slotBackground;
        [SerializeField] private Sprite townBackground;

        [Header("Existing Symbols")]
        [SerializeField] private Sprite wheatIcon;
        [SerializeField] private Sprite breadIcon;
        [SerializeField] private Sprite milkIcon;
        [SerializeField] private Sprite appleIcon;
        [SerializeField] private Sprite gemIcon;
        [SerializeField] private Sprite wildIcon;
        [SerializeField] private Sprite giftIcon;

        [Header("Existing UI")]
        [SerializeField] private Sprite resourcePill;
        [SerializeField] private Sprite navTab;
        [SerializeField] private Sprite reelFrame;
        [SerializeField] private Sprite cardFrame;
        [SerializeField] private Sprite primaryButton;
        [SerializeField] private Sprite titlePlaque;
        [SerializeField] private Sprite utilityButton;
        [SerializeField] private Sprite settingsIcon;
        [SerializeField] private Sprite coinIcon;
        [SerializeField] private Sprite woodIcon;
        [SerializeField] private Sprite oreIcon;
        [SerializeField] private Sprite wheatCorner;
        [SerializeField] private Sprite vineCorner;
        [SerializeField] private Sprite gemRivets;
        [SerializeField] private Sprite creamGoldLabel;
        [SerializeField] private Sprite ribbonCap;
        [SerializeField] private Sprite woodDivider;
        [SerializeField] private Sprite infoCard;
        [SerializeField] private Sprite dialogueCard;
        [SerializeField] private Sprite statusStrip;
        [SerializeField] private Sprite symbolTile;

        [Header("V3 Polished UI Art")]
        [SerializeField] private Sprite v3PanelOrnate;
        [SerializeField] private Sprite v3DialogScroll;
        [SerializeField] private Sprite v3StatusBarGreen;
        [SerializeField] private Sprite v3NavPlaqueGreen;
        [SerializeField] private Sprite v3ButtonLargeGold;
        [SerializeField] private Sprite v3ButtonSmallGold;
        [SerializeField] private Sprite v3TabLeft;
        [SerializeField] private Sprite v3TabRight;
        [SerializeField] private Sprite v3CornerWheatSet;
        [SerializeField] private Sprite v3WoodDivider;
        [SerializeField] private Sprite v3SettingsMedallion;
        [SerializeField] private Sprite v3BadgeRed;

        [Header("Journey And Commission UI Art")]
        [SerializeField] private Sprite journeySign;
        [SerializeField] private Sprite orderBoardUi;
        [SerializeField] private Sprite eventScroll;
        [SerializeField] private Sprite milestoneMedal;
        [SerializeField] private Sprite summaryLedger;
        [SerializeField] private Sprite harvestChest;

        [Header("V4 Generated Native NPC Art")]
        [SerializeField] private Sprite npcMiaFull;
        [SerializeField] private Sprite npcTomFull;
        [SerializeField] private Sprite favorHeartIcon;
        [SerializeField] private Sprite commissionMarkIcon;

        [Header("V4 Generated Harvest UI Art")]
        [SerializeField] private Sprite harvestConsoleFrame;
        [SerializeField] private Sprite harvestCellTile;
        [SerializeField] private Sprite harvestEnergyBar;
        [SerializeField] private Sprite harvestButtonRound;
        [SerializeField] private Sprite harvestInfoPlaque;
        [SerializeField] private Sprite harvestBackPlaque;

        [Header("V5 Generated Town UI Art")]
        [SerializeField] private Sprite townTitlePlaque;
        [SerializeField] private Sprite townPlotFrame;
        [SerializeField] private Sprite townBuildingBase;
        [SerializeField] private Sprite townNameScroll;
        [SerializeField] private Sprite townBottomInfoFrame;
        [SerializeField] private Sprite townAttentionBadge;

        [Header("V5 Generated Inventory UI Art")]
        [SerializeField] private Sprite inventoryPanelFrame;
        [SerializeField] private Sprite inventoryItemSlot;
        [SerializeField] private Sprite inventoryTabActive;
        [SerializeField] private Sprite inventoryTabInactive;
        [SerializeField] private Sprite inventoryCountBadge;
        [SerializeField] private Sprite inventoryEmptyBasket;

        [Header("V5 Generated Task UI Art")]
        [SerializeField] private Sprite taskOrderBoardFrame;
        [SerializeField] private Sprite taskCommissionEnvelope;
        [SerializeField] private Sprite taskDailyRouteScroll;
        [SerializeField] private Sprite taskQuestRowFrame;
        [SerializeField] private Sprite taskMilestoneBadge;
        [SerializeField] private Sprite taskCollectionBook;

        [Header("V5 Generated Auth Settings UI Art")]
        [SerializeField] private Sprite authLoginCard;
        [SerializeField] private Sprite authInputFrame;
        [SerializeField] private Sprite settingsDialogFrame;
        [SerializeField] private Sprite settingsSliderArt;
        [SerializeField] private Sprite settingsToggleArt;
        [SerializeField] private Sprite agreementScrollFrame;

        [Header("V6 Generated Lightweight Polish UI Art")]
        [SerializeField] private Sprite v6MainPanelFrame;
        [SerializeField] private Sprite v6InfoCardFrame;
        [SerializeField] private Sprite v6TitlePlaque;
        [SerializeField] private Sprite v6PrimaryButton;
        [SerializeField] private Sprite v6SecondaryButton;
        [SerializeField] private Sprite v6ItemSlotFrame;
        [SerializeField] private Sprite v6NotificationBadge;
        [SerializeField] private Sprite v6ProgressBarFrame;

        [Header("V8 Clean Readability UI Art")]
        [SerializeField] private Sprite v8CleanPanel;

        [Header("V2 Native Farm Art")]
        [SerializeField] private Sprite plotEmpty;
        [SerializeField] private Sprite plotGrowingOne;
        [SerializeField] private Sprite plotGrowingTwo;
        [SerializeField] private Sprite plotReadyWheat;
        [SerializeField] private Sprite plotReadyApple;
        [Header("V10 Clean Field Overlay Art")]
        [SerializeField] private Sprite fieldEmptyClean;
        [SerializeField] private Sprite fieldSeedlingClean;
        [SerializeField] private Sprite fieldYoungWheatClean;
        [SerializeField] private Sprite fieldWheatClean;
        [SerializeField] private Sprite fieldAppleClean;
        [Header("V11 Baked Field Tiles")]
        [SerializeField] private Sprite[] bakedPlotEmpty;
        [SerializeField] private Sprite[] bakedPlotSeeded;
        [SerializeField] private Sprite[] bakedPlotSeedling;
        [SerializeField] private Sprite[] bakedPlotYoungWheat;
        [SerializeField] private Sprite[] bakedPlotWheat;
        [SerializeField] private Sprite[] bakedPlotApple;
        [SerializeField] private Sprite bakeryIcon;
        [SerializeField] private Sprite dairyIcon;
        [SerializeField] private Sprite houseMiaIcon;
        [SerializeField] private Sprite houseTomIcon;
        [SerializeField] private Sprite machineHarvestIcon;
        [SerializeField] private Sprite boardOrdersIcon;
        [SerializeField] private Sprite bagIcon;
        [SerializeField] private Sprite questIcon;
        [SerializeField] private Sprite miaIcon;
        [SerializeField] private Sprite tomIcon;
        [SerializeField] private Sprite completeBubbleIcon;
        [SerializeField] private Sprite lockIcon;
        [SerializeField] private Sprite sickleIcon;

        private readonly System.Random random = new System.Random();
        private bool harvestSpinning;
        private TMP_Text harvestResultText;
        private readonly List<Image> reelImages = new List<Image>();
        private readonly List<TMP_Text> reelLabels = new List<TMP_Text>();
        private readonly List<RectTransform> reelCells = new List<RectTransform>();
        private readonly Dictionary<string, Sprite> bakedResourceCache = new Dictionary<string, Sprite>();
        private readonly Dictionary<string, Sprite> generatedUiResourceCache = new Dictionary<string, Sprite>();
        private readonly PlotState[] plots = { PlotState.Empty, PlotState.Empty, PlotState.Empty, PlotState.Locked, PlotState.Locked, PlotState.Locked };
        private readonly float[] plotTimers = new float[6];

        private RectTransform root;
        private RectTransform authLayer;
        private RectTransform hudLayer;
        private RectTransform topResourceBar;
        private RectTransform bottomNavBar;
        private RectTransform townPage;
        private RectTransform bagPage;
        private RectTransform taskPage;
        private RectTransform harvestPage;
        private RectTransform modalLayer;
        private RectTransform toastBox;
        private Coroutine pageFadeRoutine;

        private readonly List<Button> navButtons = new List<Button>();
        private TMP_Text coinText;
        private TMP_Text woodText;
        private TMP_Text oreText;
        private TMP_Text toastText;
        private TMP_Text townHintText;
        private TMP_Text bagGridText;
        private TMP_Text taskBodyText;
        private TMP_Text harvestProgressText;
        private TMP_Text energyText;
        private TMP_InputField accountInput;
        private TMP_InputField passwordInput;
        private Toggle agreementToggle;

        private const string SavePrefix = "WheatTownGuest.";
        private Page activePage = Page.Town;
        private BagTab activeBagTab = BagTab.All;
        private TaskTab activeTaskTab = TaskTab.Orders;
        private int coins = 5000;
        private int wood;
        private int ore;
        private int wheat;
        private int apple;
        private int milk = 2;
        private int bread;
        private int cheese;
        private int dailyHarvest;
        private int energy;
        private int miaFavor;
        private int tomFavor;
        private bool bakeryBusy;
        private bool bakeryDone;
        private float bakeryTimer;
        private bool dairyBusy;
        private bool dairyDone;
        private float dairyTimer;
        private bool breadOrderDone;
        private bool cheeseOrderDone;
        private bool miaQuestDone;
        private bool tomQuestDone;
        private bool musicEnabled = true;
        private bool sfxEnabled = true;
        private float masterVolume = .55f;
        private readonly Dictionary<string, LayoutOverride> layoutOverrides = new Dictionary<string, LayoutOverride>();

        [Serializable]
        private sealed class LayoutOverrideFile
        {
            public LayoutOverride[] items;
        }

        [Serializable]
        private sealed class LayoutOverride
        {
            public string key;
            public float x;
            public float y;
            public float w;
            public float h;
        }

        private void Awake()
        {
            Application.targetFrameRate = 60;
            LoadLayoutOverrides();
            EnsureEventSystem();
            Build();
        }

        private void Update()
        {
            var changed = false;
            for (var i = 0; i < plots.Length; i++)
            {
                if (plots[i] != PlotState.GrowingOne && plots[i] != PlotState.GrowingTwo) continue;
                plotTimers[i] -= Time.deltaTime;
                if (plots[i] == PlotState.GrowingOne && plotTimers[i] <= 6f)
                {
                    plots[i] = PlotState.GrowingTwo;
                    changed = true;
                }
                if (plotTimers[i] <= 0f)
                {
                    plots[i] = PlotState.ReadyWheat;
                    changed = true;
                }
            }

            if (bakeryBusy && !bakeryDone)
            {
                bakeryTimer -= Time.deltaTime;
                if (bakeryTimer <= 0f)
                {
                    bakeryDone = true;
                    changed = true;
                    Toast("Bakery finished");
                }
            }

            if (dairyBusy && !dairyDone)
            {
                dairyTimer -= Time.deltaTime;
                if (dairyTimer <= 0f)
                {
                    dairyDone = true;
                    changed = true;
                    Toast("Dairy finished");
                }
            }

            if (changed)
            {
                RefreshCurrentPage();
            }
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused && HasGuestSave()) SaveGuestData();
        }

        private void OnApplicationQuit()
        {
            if (HasGuestSave()) SaveGuestData();
        }

        private void Build()
        {
            foreach (Transform child in transform) Destroy(child.gameObject);
            var canvas = NewObject<Canvas>("NativeCanvas", transform);
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            var scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(430, 932);
            scaler.matchWidthOrHeight = 1f;
            canvas.gameObject.AddComponent<GraphicRaycaster>();
            root = canvas.GetComponent<RectTransform>();
            Stretch(root);

            BuildPages();
            BuildHud();
            BuildAuth();
            BuildModalLayer();
            BuildToast();
            authLayer.gameObject.SetActive(true);
            hudLayer.gameObject.SetActive(false);
            ShowPage(Page.Town);
        }

        private void BuildPages()
        {
            townPage = CreatePage("Town", townBackground);
            bagPage = CreatePage("Bag", townBackground);
            taskPage = CreatePage("Tasks", townBackground);
            harvestPage = CreatePage("Harvest Mini Game", slotBackground);
            BuildTownPage();
            BuildBagPage();
            BuildTaskPage();
            BuildHarvestPage();
        }

        private RectTransform CreatePage(string name, Sprite bg)
        {
            var page = Panel(root, name + "Page", C(244, 226, 174, 255), Vector2.zero, new Vector2(430, 932), null);
            page.gameObject.AddComponent<CanvasGroup>();
            Stretch(page);
            ImageNode(page, "Background", bg, Vector2.zero, new Vector2(430, 932), Color.white, true);
            Panel(page, "WarmReadabilityWash", C(255, 246, 215, 24), Vector2.zero, new Vector2(430, 932), null);
            return page;
        }

        private void BuildHud()
        {
            hudLayer = Panel(root, "HUD", Color.clear, Vector2.zero, new Vector2(430, 932), null);
            Stretch(hudLayer);
            topResourceBar = Panel(hudLayer, "TopResourceBar", C(20, 59, 39, 242), new Vector2(0, 902), new Vector2(430, 62), null);
            Anchor(topResourceBar, 0, 1, 1, 1, 0, -62, 0, 0);
            ApplyLayoutOverride(topResourceBar);
            ImageNode(topResourceBar, "TopSoftHighlight", v3StatusBarGreen, new Vector2(0, -31), new Vector2(430, 62), C(255, 255, 255, 70), true);
            ImageNode(topResourceBar, "Logo", wheatIcon, new Vector2(-195, -31), new Vector2(32, 32), Color.white);
            TextNode(topResourceBar, "Title", "Wheat", new Vector2(-146, -31), new Vector2(76, 30), 16, C(255, 248, 222, 255), TextAnchor.MiddleLeft);
            coinText = ResourceText(topResourceBar, coinIcon, new Vector2(-58, -31));
            woodText = ResourceText(topResourceBar, woodIcon, new Vector2(34, -31));
            oreText = ResourceText(topResourceBar, oreIcon, new Vector2(126, -31));
            var settingsPlate = Panel(topResourceBar, "SettingsPlate", C(255, 245, 212, 245), new Vector2(198, -31), new Vector2(40, 40), null);
            AddReadableBorder(settingsPlate, new Vector2(40, 40), 2f, C(181, 137, 74, 190));
            ImageNode(settingsPlate, "SettingsArt", settingsIcon, Vector2.zero, new Vector2(24, 24), C(52, 74, 50, 255));
            var settings = IconButton(topResourceBar, "SettingsTop", settingsIcon, new Vector2(198, -31), new Vector2(48, 48), ShowSettings);
            settings.GetComponent<Image>().color = Color.clear;

            bottomNavBar = Panel(hudLayer, "BottomNavigation", C(18, 54, 37, 252), Vector2.zero, new Vector2(430, 78), null);
            Anchor(bottomNavBar, 0, 0, 1, 0, 0, 0, 0, 78);
            ImageNode(bottomNavBar, "BottomSoftArt", v3NavPlaqueGreen, new Vector2(0, 39), new Vector2(390, 74), C(255, 255, 255, 72));
            navButtons.Clear();
            navButtons.Add(NavButton(bottomNavBar, "Town", wheatIcon, -150, () => ShowPage(Page.Town)));
            navButtons.Add(NavButton(bottomNavBar, "Bag", bagIcon, -50, () => ShowPage(Page.Bag)));
            navButtons.Add(NavButton(bottomNavBar, "Tasks", questIcon, 50, () => ShowPage(Page.Task)));
            navButtons.Add(NavButton(bottomNavBar, "Settings", settingsIcon, 150, ShowSettings));
            RefreshHud();
        }

        private TMP_Text ResourceText(RectTransform parent, Sprite icon, Vector2 pos)
        {
            var group = Panel(parent, "Resource", C(255, 248, 220, 245), pos, new Vector2(92, 38), null);
            AddReadableBorder(group, new Vector2(92, 38), 2f, C(181, 137, 74, 170));
            ImageNode(group, "Icon", icon, new Vector2(-29, 0), new Vector2(24, 24), Color.white);
            return TextNode(group, "Value", "0", new Vector2(15, 0), new Vector2(56, 26), 16, C(35, 52, 38, 255), TextAnchor.MiddleLeft);
        }

        private Button NavButton(RectTransform parent, string label, Sprite icon, float x, UnityEngine.Events.UnityAction action)
        {
            var button = ButtonNode(parent, label, "", new Vector2(x, 39), new Vector2(88, 66), C(255, 249, 224, 126), action, null);
            ImageNode(button.transform, "Icon", icon, new Vector2(0, 13), new Vector2(30, 30), Color.white);
            TextNode(button.transform, "Label", label, new Vector2(0, -19), new Vector2(84, 24), 15, C(255, 248, 222, 255), TextAnchor.MiddleCenter);
            return button;
        }

        private void BuildAuth()
        {
            authLayer = Panel(root, "LoginPage", C(30, 65, 47, 255), Vector2.zero, new Vector2(430, 932), null);
            Stretch(authLayer);
            RebuildAuthContent();
        }

        private void RebuildAuthContent()
        {
            if (authLayer == null) return;
            Clear(authLayer);
            ImageNode(authLayer, "LoginBg", lobbyBackground, Vector2.zero, new Vector2(430, 932), Color.white, true);
            Panel(authLayer, "Shade", C(16, 34, 25, 70), Vector2.zero, new Vector2(430, 932), null);
            Panel(authLayer, "TopGradient", C(14, 37, 26, 72), new Vector2(0, 338), new Vector2(430, 222), null);
            ImageNode(authLayer, "BigLogo", wheatIcon, new Vector2(0, 350), new Vector2(56, 56), C(255, 247, 215, 255));
            TextNode(authLayer, "GameTitle", "Wheat Town", new Vector2(0, 306), new Vector2(330, 44), 31, C(255, 249, 224, 255), TextAnchor.MiddleCenter);
            TextNode(authLayer, "Subtitle", "Cozy Harvest · Town Life · Casual Growth", new Vector2(0, 270), new Vector2(360, 28), 15, C(255, 240, 202, 255), TextAnchor.MiddleCenter);

            var card = Panel(authLayer, "LoginCard", C(255, 246, 226, 255), new Vector2(0, -60), new Vector2(372, 612), AuthLoginCard());
            TextNode(card, "LoginTitle", "Guest Sign In", new Vector2(0, 240), new Vector2(290, 42), 28, C(255, 248, 222, 255), TextAnchor.MiddleCenter);
            TextNode(card, "LoginTip", "Letters and numbers only", new Vector2(0, 162), new Vector2(300, 28), 17, C(70, 64, 52, 255), TextAnchor.MiddleCenter);
            accountInput = Input(card, "ID", new Vector2(0, 102), "Account ID", false);
            passwordInput = Input(card, "Pass", new Vector2(0, 38), "Password", true);
            accountInput.onValueChanged.AddListener(_ => SanitizeInput(accountInput));
            passwordInput.onValueChanged.AddListener(_ => SanitizeInput(passwordInput));
            agreementToggle = ToggleNode(card, "I agree to Privacy and Terms", new Vector2(0, -26));
            ButtonNode(card, "Guest", "Guest Login", new Vector2(0, -94), new Vector2(276, 58), C(222, 103, 38, 255), GuestLogin, null);
            var login = ButtonNode(card, "Login", "Log in", new Vector2(0, -158), new Vector2(190, 46), C(255, 246, 219, 255), AccountLogin, null);
            AddReadableBorder(login.GetComponent<RectTransform>(), new Vector2(190, 46), 2f, C(181, 137, 74, 220));
            login.GetComponentInChildren<TMP_Text>().color = C(70, 85, 72, 255);
            if (HasGuestSave())
            {
                ButtonNode(card, "ContinueSave", "Continue Save", new Vector2(-82, -220), new Vector2(150, 44), C(46, 74, 43, 255), ContinueGuestSave, null);
                ButtonNode(card, "DeleteSave", "Log out", new Vector2(94, -220), new Vector2(126, 44), C(126, 70, 48, 255), DeleteGuestSave, null);
            }
            else
            {
                TextNode(card, "NoSaveHint", "No local guest save yet", new Vector2(0, -220), new Vector2(280, 26), 16, C(112, 100, 78, 255), TextAnchor.MiddleCenter);
            }
        }

        private void BuildTownPage()
        {
            ClearNonBackground(townPage);
            var titlePlate = Panel(townPage, "TownTitlePlate", C(24, 70, 46, 232), new Vector2(-132, 300), new Vector2(116, 40), null);
            AddReadableBorder(titlePlate, new Vector2(116, 40), 2f, C(181, 137, 74, 180));
            TextNode(titlePlate, "TownTitle", "Town", Vector2.zero, new Vector2(96, 24), 18, C(255, 248, 222, 255), TextAnchor.MiddleCenter);
            var hintPlate = Panel(townPage, "TownHintPlate", C(255, 250, 232, 238), new Vector2(76, 300), new Vector2(194, 30), null);
            AddReadableBorder(hintPlate, new Vector2(194, 30), 2f, C(181, 137, 74, 170));
            townHintText = TextNode(hintPlate, "TownHint", "Tap town cards", Vector2.zero, new Vector2(176, 22), 14, C(54, 65, 50, 255), TextAnchor.MiddleCenter);

            for (var i = 0; i < 6; i++)
            {
                var col = i % 2;
                var row = i / 2;
                var pos = new Vector2(-126 + col * 82, 218 - row * 74);
                PlotButton(i, pos);
            }

            SceneHotspot("Bakery", new Vector2(126, 194), new Vector2(126, 124), OpenBakery, bakeryDone);
            SceneHotspot("Dairy", new Vector2(126, 64), new Vector2(122, 116), OpenDairy, dairyDone);
            SceneHotspot("Mia", new Vector2(-112, -24), new Vector2(116, 108), () => OpenResident("Mia"), !miaQuestDone);
            SceneHotspot("Tom", new Vector2(4, -24), new Vector2(116, 108), () => OpenResident("Old Tom"), !tomQuestDone);
            SceneHotspot("Harvest", new Vector2(126, -122), new Vector2(126, 116), () => ShowHarvest(true), false);
            SceneHotspot("Board", new Vector2(-128, -170), new Vector2(112, 108), () => ShowPage(Page.Task), true);

            var objective = Panel(townPage, "TownObjective", C(26, 57, 40, 230), new Vector2(0, -292), new Vector2(326, 36), null);
            AddReadableBorder(objective, new Vector2(326, 36), 2f, C(181, 137, 74, 150));
            TextNode(objective, "Copy", "Goal: Plant -> Craft -> Order", Vector2.zero, new Vector2(300, 22), 13, C(255, 248, 222, 255), TextAnchor.MiddleCenter);
        }

        private void PlotButton(int index, Vector2 pos)
        {
            var state = plots[index];
            var button = ButtonNode(townPage, "Plot" + index, "", pos, new Vector2(96, 74), C(255, 255, 255, 1), () => ClickPlot(index), null);
            var buttonImage = button.GetComponent<Image>();
            if (buttonImage != null) buttonImage.color = C(255, 255, 255, 1);
            var tile = BakedFieldTile(index, state);
            if (tile != null)
            {
                var fieldTile = ImageNode(button.transform, "FieldTile", tile, Vector2.zero, new Vector2(96, 74), Color.white);
                var tileImage = fieldTile.GetComponent<Image>();
                if (tileImage != null) tileImage.preserveAspect = false;
            }
            if (state == PlotState.Locked)
            {
                ImageNode(button.transform, "Lock", lockIcon, new Vector2(0, 4), new Vector2(28, 28), C(255, 255, 255, 220));
                var lockBadge = Panel(button.transform, "LockLevelBadge", C(28, 60, 42, 245), new Vector2(0, -26), new Vector2(54, 22), null);
                TextNode(lockBadge, "LockText", "Lv." + (index + 1), Vector2.zero, new Vector2(50, 20), 13, C(255, 248, 222, 255), TextAnchor.MiddleCenter);
            }
            else if (state == PlotState.ReadyWheat)
            {
                ImageNode(button.transform, "Sickle", sickleIcon, new Vector2(30, 22), new Vector2(25, 25), Color.white);
            }
            else if (state == PlotState.GrowingOne || state == PlotState.GrowingTwo)
            {
                var timerBadge = Panel(button.transform, "TimerBadge", C(28, 60, 42, 245), new Vector2(0, 31), new Vector2(58, 22), null);
                TextNode(timerBadge, "Timer", Mathf.CeilToInt(plotTimers[index]) + "s", Vector2.zero, new Vector2(50, 20), 13, C(255, 248, 222, 255), TextAnchor.MiddleCenter);
            }
        }

        private Sprite BakedFieldTile(int index, PlotState state)
        {
            switch (state)
            {
                case PlotState.GrowingOne:
                    return PickBaked(bakedPlotSeeded, index) ?? ResourceBaked(index, "seeded") ?? PickBaked(bakedPlotSeedling, index) ?? ResourceBaked(index, "seedling") ?? FieldSeedling();
                case PlotState.GrowingTwo:
                    return PickBaked(bakedPlotYoungWheat, index) ?? ResourceBaked(index, "young_wheat") ?? PickBaked(bakedPlotSeedling, index) ?? ResourceBaked(index, "seedling") ?? FieldYoungWheat();
                case PlotState.ReadyWheat:
                    return PickBaked(bakedPlotWheat, index) ?? ResourceBaked(index, "wheat") ?? FieldWheat();
                case PlotState.Empty:
                case PlotState.Locked:
                default:
                    return PickBaked(bakedPlotEmpty, index) ?? ResourceBaked(index, "empty") ?? FieldEmpty();
            }
        }

        private static Sprite PickBaked(Sprite[] sprites, int index)
        {
            if (sprites == null || sprites.Length == 0) return null;
            if (index >= 0 && index < sprites.Length && sprites[index] != null) return sprites[index];
            return sprites[0];
        }

        private Sprite ResourceBaked(int index, string state)
        {
            var key = "WheatTown/field-baked-v11/plot_" + index + "_" + state;
            if (bakedResourceCache.TryGetValue(key, out var cached))
            {
                return cached;
            }

            var sprite = Resources.Load<Sprite>(key);
            if (sprite == null)
            {
                var texture = Resources.Load<Texture2D>(key);
                if (texture != null)
                {
                    sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f), 100f);
                    sprite.name = texture.name;
                }
            }

            bakedResourceCache[key] = sprite;
            return sprite;
        }

        private Sprite ResourceGeneratedUi(string fileName)
        {
            return ResourceGeneratedUi("generated-ui-v12", fileName);
        }

        private Sprite ResourceGeneratedUiV13(string fileName)
        {
            return ResourceGeneratedUi("generated-ui-v13", fileName);
        }

        private Sprite ResourceGeneratedUi(string folder, string fileName)
        {
            var key = "WheatTown/" + folder + "/" + fileName;
            if (generatedUiResourceCache.TryGetValue(key, out var cached))
            {
                return cached;
            }

            var sprite = Resources.Load<Sprite>(key);
            if (sprite == null)
            {
                var texture = Resources.Load<Texture2D>(key);
                if (texture != null)
                {
                    sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f), 100f);
                    sprite.name = texture.name;
                }
            }

            generatedUiResourceCache[key] = sprite;
            return sprite;
        }

        private Sprite FieldEmpty()
        {
            return fieldEmptyClean != null ? fieldEmptyClean : plotEmpty;
        }

        private Sprite FieldSeedling()
        {
            return fieldSeedlingClean != null ? fieldSeedlingClean : plotGrowingOne;
        }

        private Sprite FieldYoungWheat()
        {
            return fieldYoungWheatClean != null ? fieldYoungWheatClean : fieldWheatClean != null ? fieldWheatClean : plotGrowingTwo;
        }

        private Sprite FieldWheat()
        {
            return fieldWheatClean != null ? fieldWheatClean : plotReadyWheat;
        }

        private void SceneHotspot(string label, Vector2 pos, Vector2 size, UnityEngine.Events.UnityAction action, bool attention)
        {
            var button = ButtonNode(townPage, label + "Hotspot", "", pos, size, C(255, 255, 255, 1), action, null);
            var tapPlate = Panel(button.transform, "TapPlate", C(22, 57, 38, 248), new Vector2(0, -size.y * .36f), new Vector2(Mathf.Min(size.x - 10, 114), 36), null);
            TextNode(tapPlate, "TapLabel", label, Vector2.zero, new Vector2(tapPlate.sizeDelta.x - 10, 26), 16, C(255, 250, 224, 255), TextAnchor.MiddleCenter);
            if (attention)
            {
                ImageNode(button.transform, "Bubble", TownAttentionBadge(), new Vector2(size.x * .30f, size.y * .30f), new Vector2(24, 24), Color.white);
            }
        }

        private void BuildBagPage()
        {
            ClearNonBackground(bagPage);
            var header = Panel(bagPage, "BagHeader", C(24, 60, 40, 250), new Vector2(0, 300), new Vector2(386, 54), null);
            AddReadableBorder(header, new Vector2(386, 54), 2f, C(181, 137, 74, 190));
            TextNode(header, "Title", "Bag", new Vector2(-152, 0), new Vector2(50, 30), 21, C(255, 248, 222, 255), TextAnchor.MiddleLeft);
            BagTabButton(header, "All", BagTab.All, -94);
            BagTabButton(header, "Crops", BagTab.Crops, -36);
            BagTabButton(header, "Goods", BagTab.Processed, 26);
            BagTabButton(header, "Mats", BagTab.Materials, 88);
            BagTabButton(header, "Album", BagTab.Collection, 146);

            var panel = Panel(bagPage, "BagGrid", C(255, 250, 232, 255), new Vector2(0, 26), new Vector2(358, 504), null);
            AddReadableBorder(panel, new Vector2(358, 504), 3f, C(124, 86, 42, 220));
            var items = BagItems();
            for (var i = 0; i < 12; i++)
            {
                var col = i % 2;
                var row = i / 2;
                var cell = Panel(panel, "BagCell" + i, C(255, 246, 219, 255), new Vector2(-82 + col * 164, 174 - row * 72), new Vector2(148, 66), null);
                AddReadableBorder(cell, new Vector2(148, 66), 2f, C(181, 137, 74, 195));
                if (i < items.Count)
                {
                    ImageNode(cell, "Icon", items[i].Sprite, new Vector2(-44, 5), new Vector2(44, 44), Color.white);
                    TextNode(cell, "Name", ShortItemName(items[i].Name), new Vector2(34, 13), new Vector2(86, 24), 16, C(35, 52, 38, 255), TextAnchor.MiddleLeft);
                    var countBadge = Panel(cell, "CountBadge", C(39, 76, 47, 250), new Vector2(34, -17), new Vector2(88, 26), null);
                    TextNode(countBadge, "Count", "x" + items[i].Count, Vector2.zero, new Vector2(80, 22), 16, C(255, 248, 222, 255), TextAnchor.MiddleCenter);
                }
                else
                {
                    TextNode(cell, "Empty", "Empty", Vector2.zero, new Vector2(106, 24), 15, C(105, 98, 82, 175), TextAnchor.MiddleCenter);
                }
            }
            var hint = Panel(bagPage, "BagHintPlate", C(24, 58, 39, 245), new Vector2(0, -270), new Vector2(334, 36), null);
            bagGridText = TextNode(hint, "BagHint", BagTabLabel(activeBagTab) + " items for farming and orders", Vector2.zero, new Vector2(310, 24), 14, C(255, 248, 222, 255), TextAnchor.MiddleCenter);
        }

        private void BuildTaskPage()
        {
            ClearNonBackground(taskPage);
            var header = Panel(taskPage, "TaskHeader", C(24, 60, 40, 248), new Vector2(0, 292), new Vector2(386, 60), null);
            AddReadableBorder(header, new Vector2(386, 60), 2f, C(212, 166, 86, 230));
            TextNode(header, "Title", "Tasks", new Vector2(-148, 0), new Vector2(82, 34), 22, C(255, 248, 222, 255), TextAnchor.MiddleLeft);
            TaskTabButton(header, "Orders", TaskTab.Orders, -50);
            TaskTabButton(header, "Friends", TaskTab.Residents, 48);
            TaskTabButton(header, "Route", TaskTab.DailyRoute, 142);

            var panel = Panel(taskPage, "TaskPanel", C(250, 255, 240, 255), new Vector2(0, 22), new Vector2(366, 500), TaskOrderBoardFrame());
            if (activeTaskTab == TaskTab.Orders)
            {
                TextNode(panel, "PanelTitle", "Today Orders", new Vector2(0, 210), new Vector2(250, 38), 24, C(255, 248, 222, 255), TextAnchor.MiddleCenter);
                OrderCard(panel, "Bakery Order", breadIcon, breadOrderDone ? "Done today" : "Need Bread x1", bread >= 1 && !breadOrderDone, new Vector2(0, 112), SubmitBreadOrder);
                OrderCard(panel, "Dairy Order", milkIcon, cheeseOrderDone ? "Done today" : "Need Cheese x1", cheese >= 1 && !cheeseOrderDone, new Vector2(0, 20), SubmitCheeseOrder);
                TextNode(panel, "OrderHint", "Orders give coins and materials", new Vector2(0, -70), new Vector2(304, 30), 17, C(70, 64, 52, 255), TextAnchor.MiddleCenter);
                ButtonNode(panel, "Collection", "Open Album", new Vector2(0, -142), new Vector2(210, 52), C(232, 141, 47, 255), ShowCollection, null);
            }
            else if (activeTaskTab == TaskTab.Residents)
            {
                TextNode(panel, "PanelTitle", "Friends", new Vector2(0, 210), new Vector2(250, 38), 24, C(255, 248, 222, 255), TextAnchor.MiddleCenter);
                OrderCard(panel, "Mia Breakfast", wheatIcon, miaQuestDone ? "Favor " + miaFavor + "/5" : "Need Wheat x2", wheat >= 2 && !miaQuestDone, new Vector2(0, 112), SubmitMiaQuest);
                OrderCard(panel, "Tom Snack", breadIcon, tomQuestDone ? "Favor " + tomFavor + "/5" : "Need Bread x1", bread >= 1 && !tomQuestDone, new Vector2(0, 20), SubmitTomQuest);
                TextNode(panel, "ResidentHint", "Small requests raise favor", new Vector2(0, -70), new Vector2(304, 30), 17, C(70, 64, 52, 255), TextAnchor.MiddleCenter);
                ButtonNode(panel, "TalkMia", "Talk to Mia", new Vector2(-88, -142), new Vector2(144, 52), C(46, 74, 43, 255), () => OpenResident("Mia"), null);
                ButtonNode(panel, "TalkTom", "Talk to Tom", new Vector2(88, -142), new Vector2(144, 52), C(46, 74, 43, 255), () => OpenResident("Old Tom"), null);
            }
            else
            {
                TextNode(panel, "RouteTitle", "Daily Route", new Vector2(0, 210), new Vector2(260, 38), 24, C(255, 248, 222, 255), TextAnchor.MiddleCenter);
                RouteNode(panel, 10, "Coins 200", new Vector2(0, 142));
                RouteNode(panel, 30, "Gift x1", new Vector2(0, 98));
                RouteNode(panel, 60, "Ore x3", new Vector2(0, 54));
                RouteNode(panel, 100, "Bonus Harvest", new Vector2(0, 10));
                RouteNode(panel, 150, "Harvest Chest", new Vector2(0, -34));
                ProgressBar(panel, new Vector2(0, -92), new Vector2(260, 18), Mathf.Clamp01(dailyHarvest / 150f));
                TextNode(panel, "RouteProgress", "Harvest today: " + dailyHarvest + " / 150", new Vector2(0, -124), new Vector2(280, 26), 16, C(70, 64, 52, 255), TextAnchor.MiddleCenter);
                ButtonNode(panel, "Collection", "Open Album", new Vector2(0, -178), new Vector2(210, 50), C(232, 141, 47, 255), ShowCollection, null);
            }
            var hint = Panel(taskPage, "TaskHintPlate", C(24, 58, 39, 242), new Vector2(0, -246), new Vector2(334, 38), null);
            taskBodyText = TextNode(hint, "TaskHint", TaskTabLabel(activeTaskTab) + " tab", Vector2.zero, new Vector2(310, 24), 16, C(255, 248, 222, 255), TextAnchor.MiddleCenter);
            ButtonNode(taskPage, "BackTaskTown", "Back to Town", new Vector2(0, -300), new Vector2(230, 50), C(46, 74, 43, 255), () => ShowPage(Page.Town), null);
        }

        private void OrderCard(RectTransform parent, string title, Sprite icon, string desc, bool ready, Vector2 pos, UnityEngine.Events.UnityAction submit)
        {
            var row = Panel(parent, title, C(255, 248, 226, 252), pos, new Vector2(320, 80), null);
            AddReadableBorder(row, new Vector2(320, 80), 2f, C(181, 137, 74, 220));
            var iconBack = Panel(row, "IconBack", C(248, 233, 198, 245), new Vector2(-128, 0), new Vector2(56, 56), null);
            AddReadableBorder(iconBack, new Vector2(56, 56), 1.5f, C(181, 137, 74, 180));
            ImageNode(row, "Icon", icon, new Vector2(-128, 0), new Vector2(42, 42), Color.white);
            TextNode(row, "Title", title, new Vector2(-24, 16), new Vector2(176, 30), 19, C(35, 52, 38, 255), TextAnchor.MiddleLeft);
            TextNode(row, "Desc", desc, new Vector2(-24, -17), new Vector2(176, 28), 17, C(74, 66, 52, 255), TextAnchor.MiddleLeft);
            ButtonNode(row, "Submit", ready ? "Send" : "Wait", new Vector2(118, 0), new Vector2(82, 54), ready ? C(232, 141, 47, 255) : C(136, 132, 122, 255), submit, null);
            if (ready)
            {
                ImageNode(row, "ReadyBadge", TaskMilestoneBadge(), new Vector2(-128, 22), new Vector2(20, 20), Color.white);
            }
        }

        private void RouteNode(RectTransform parent, int target, string reward, Vector2 pos)
        {
            var complete = dailyHarvest >= target;
            var row = Panel(parent, "RouteNode" + target, C(255, 246, 219, complete ? (byte)255 : (byte)238), pos, new Vector2(268, 38), null);
            AddReadableBorder(row, new Vector2(268, 38), 2f, C(181, 137, 74, 190));
            ImageNode(row, "Medal", TaskMilestoneBadge(), new Vector2(-100, 0), new Vector2(26, 26), complete ? Color.white : C(255, 255, 255, 145));
            TextNode(row, "Target", target + "x", new Vector2(-44, 0), new Vector2(64, 24), 15, C(46, 74, 43, 255), TextAnchor.MiddleLeft);
            TextNode(row, "Reward", reward, new Vector2(52, 0), new Vector2(128, 24), 14, C(86, 75, 58, 255), TextAnchor.MiddleLeft);
            if (complete)
            {
                ImageNode(row, "Done", TaskMilestoneBadge(), new Vector2(108, 10), new Vector2(20, 20), Color.white);
            }
        }

        private void BagTabButton(RectTransform parent, string label, BagTab tab, float x)
        {
            var active = activeBagTab == tab;
            var button = ButtonNode(parent, "Tab" + label, label, new Vector2(x, 0), new Vector2(label.Length > 2 ? 62 : 52, 36), active ? C(255, 248, 222, 246) : C(255, 249, 238, 82), () =>
            {
                activeBagTab = tab;
                BuildBagPage();
            }, null);
            var text = button.GetComponentInChildren<TMP_Text>();
            if (text != null)
            {
                text.fontSize = 15;
                text.color = active ? C(44, 70, 46, 255) : C(255, 248, 222, 255);
            }
        }

        private void TaskTabButton(RectTransform parent, string label, TaskTab tab, float x)
        {
            var active = activeTaskTab == tab;
            var button = ButtonNode(parent, "Tab" + label, label, new Vector2(x, 0), new Vector2(80, 36), active ? C(255, 248, 222, 246) : C(255, 249, 238, 82), () =>
            {
                activeTaskTab = tab;
                BuildTaskPage();
            }, null);
            var text = button.GetComponentInChildren<TMP_Text>();
            if (text != null)
            {
                text.fontSize = 15;
                text.color = active ? C(44, 70, 46, 255) : C(255, 248, 222, 255);
            }
        }

        private List<(string Name, Sprite Sprite, int Count)> BagItems()
        {
            var all = new List<(string Name, Sprite Sprite, int Count)>
            {
                ("Wheat", wheatIcon, wheat), ("Apple", appleIcon, apple), ("Milk", milkIcon, milk), ("Bread", breadIcon, bread),
                ("Cheese", milkIcon, cheese), ("Wood", woodIcon, wood), ("Ore", oreIcon, ore), ("Coins", coinIcon, coins),
                ("Harvest Album", giftIcon, dailyHarvest), ("Mia Favor", NpcMia(), miaFavor), ("Tom Favor", NpcTom(), tomFavor)
            };
            if (activeBagTab == BagTab.All) return all;
            if (activeBagTab == BagTab.Crops) return all.FindAll(item => item.Name == "Wheat" || item.Name == "Apple" || item.Name == "Milk");
            if (activeBagTab == BagTab.Processed) return all.FindAll(item => item.Name == "Bread" || item.Name == "Cheese");
            if (activeBagTab == BagTab.Materials) return all.FindAll(item => item.Name == "Wood" || item.Name == "Ore" || item.Name == "Coins");
            return all.FindAll(item => item.Name.Contains("Album") || item.Name.Contains("Favor"));
        }

        private string BagTabLabel(BagTab tab)
        {
            if (tab == BagTab.Crops) return "Crops";
            if (tab == BagTab.Processed) return "Goods";
            if (tab == BagTab.Materials) return "Mats";
            if (tab == BagTab.Collection) return "Album";
            return "All";
        }

        private string ShortItemName(string name)
        {
            if (name == "Harvest Album") return "Album";
            if (name == "Mia Favor") return "Mia";
            if (name == "Tom Favor") return "Tom";
            return name;
        }

        private string TaskTabLabel(TaskTab tab)
        {
            if (tab == TaskTab.Residents) return "Friends";
            if (tab == TaskTab.DailyRoute) return "Route";
            return "Orders";
        }

        private void BuildHarvestPage()
        {
            ClearNonBackground(harvestPage);
            Panel(harvestPage, "HarvestReadableShade", C(14, 35, 25, 94), Vector2.zero, new Vector2(430, 932), null);

            var summary = Panel(harvestPage, "HarvestSummary", C(255, 252, 238, 255), new Vector2(0, 314), new Vector2(388, 78), null);
            AddReadableBorder(summary, new Vector2(388, 78), 3f, C(108, 78, 42, 245));
            ImageNode(summary, "WheatMark", wheatIcon, new Vector2(-170, 6), new Vector2(36, 36), C(255, 245, 205, 255));
            TextNode(summary, "Title", "Harvest", new Vector2(-100, 12), new Vector2(150, 34), 26, C(32, 57, 39, 255), TextAnchor.MiddleLeft);
            TextNode(summary, "Subtitle", "3 matching symbols win. Clear 5 x 3 table.", new Vector2(-10, -22), new Vector2(306, 26), 15, C(75, 67, 52, 255), TextAnchor.MiddleCenter);
            var progressPlate = Panel(summary, "ProgressPlate", C(235, 247, 224, 255), new Vector2(138, 12), new Vector2(116, 34), null);
            AddReadableBorder(progressPlate, new Vector2(116, 32), 2f, C(139, 107, 61, 210));
            harvestProgressText = TextNode(progressPlate, "Progress", "0 / 400", Vector2.zero, new Vector2(102, 24), 17, C(37, 84, 58, 255), TextAnchor.MiddleCenter);

            var machine = Panel(harvestPage, "HarvestMachine", C(255, 250, 232, 255), new Vector2(0, 18), new Vector2(376, 548), null);
            AddReadableBorder(machine, new Vector2(376, 548), 4f, C(108, 78, 42, 248));
            Panel(machine, "MachineInnerShade", C(255, 247, 222, 115), new Vector2(0, 12), new Vector2(354, 520), null);

            var title = Panel(machine, "MachineTitlePlate", C(30, 78, 50, 255), new Vector2(0, 228), new Vector2(322, 48), null);
            AddReadableBorder(title, new Vector2(322, 48), 2f, C(203, 158, 83, 210));
            TextNode(title, "MachineTitle", "Harvest Table", Vector2.zero, new Vector2(286, 30), 24, C(255, 250, 224, 255), TextAnchor.MiddleCenter);

            var reelPanel = Panel(machine, "ReelPanel", C(248, 238, 209, 255), new Vector2(0, 78), new Vector2(344, 282), null);
            AddReadableBorder(reelPanel, new Vector2(344, 282), 3f, C(120, 85, 44, 235));
            Panel(reelPanel, "ReelBackdrop", C(90, 60, 34, 42), Vector2.zero, new Vector2(324, 260), null);
            reelImages.Clear();
            reelLabels.Clear();
            reelCells.Clear();
            for (var i = 0; i < 15; i++)
            {
                var col = i % 5;
                var row = i / 5;
                var cell = Panel(reelPanel, "Cell" + i, C(255, 253, 242, 255), new Vector2(-136 + col * 68, 82 - row * 82), new Vector2(62, 72), null);
                AddReadableBorder(cell, new Vector2(62, 72), 2f, C(158, 111, 56, 220));
                Panel(cell, "IconGlow", C(255, 239, 164, 36), Vector2.zero, new Vector2(52, 58), null);
                reelCells.Add(cell);
                reelImages.Add(ImageNode(cell, "Icon", wheatIcon, Vector2.zero, new Vector2(48, 48), Color.white).GetComponent<Image>());
                reelLabels.Add(null);
            }

            var resultPanel = Panel(machine, "ResultPanel", C(255, 248, 224, 255), new Vector2(0, -92), new Vector2(322, 48), null);
            AddReadableBorder(resultPanel, new Vector2(322, 48), 2f, C(149, 105, 53, 215));
            harvestResultText = TextNode(resultPanel, "Result", "Tap Harvest to spin", Vector2.zero, new Vector2(296, 28), 17, C(55, 61, 45, 255), TextAnchor.MiddleCenter);

            var energyPanel = Panel(machine, "EnergyPanel", C(255, 248, 224, 255), new Vector2(0, -146), new Vector2(322, 42), null);
            AddReadableBorder(energyPanel, new Vector2(322, 42), 2f, C(149, 105, 53, 215));
            energyText = TextNode(energyPanel, "Energy", "Help Energy 0 / 6", new Vector2(-62, 0), new Vector2(170, 26), 16, C(55, 61, 45, 255), TextAnchor.MiddleLeft);
            ProgressBar(energyPanel, new Vector2(86, 0), new Vector2(130, 14), energy / 6f);

            var actionBar = Panel(machine, "ActionBar", C(30, 78, 50, 255), new Vector2(0, -212), new Vector2(338, 80), null);
            AddReadableBorder(actionBar, new Vector2(338, 80), 2f, C(203, 158, 83, 205));
            ButtonNode(actionBar, "Auto", "Auto", new Vector2(-114, 0), new Vector2(86, 54), C(52, 92, 58, 255), () => Toast("Auto harvest in next build"), null);
            ButtonNode(actionBar, "Harvest", "Harvest\n<size=16>10 Coins</size>", new Vector2(0, 0), new Vector2(118, 66), C(229, 119, 42, 255), HarvestSpin, null);
            ButtonNode(actionBar, "Info", "Info", new Vector2(114, 0), new Vector2(86, 54), C(52, 92, 58, 255), () => Toast("3+ same symbols win. Energy protects weak spins."), null);

            var back = ButtonNode(harvestPage, "BackTown", "Back to Town", new Vector2(0, -336), new Vector2(224, 52), C(255, 250, 232, 255), () => ShowHarvest(false), null);
            AddReadableBorder(back.GetComponent<RectTransform>(), new Vector2(224, 52), 2f, C(126, 86, 42, 235));
            back.GetComponentInChildren<TMP_Text>().color = C(35, 74, 47, 255);
            RefreshHud();
        }

        private void ClickPlot(int index)
        {
            switch (plots[index])
            {
                case PlotState.Locked:
                    Toast("Unlocks after town upgrade");
                    break;
                case PlotState.Empty:
                    ShowSeedPanel(index);
                    break;
                case PlotState.GrowingOne:
                case PlotState.GrowingTwo:
                    Toast("Growing: " + Mathf.CeilToInt(plotTimers[index]) + "s left");
                    break;
                case PlotState.ReadyWheat:
                    plots[index] = PlotState.Empty;
                    wheat += 1;
                    dailyHarvest += 1;
                    Toast("Harvested Wheat x1");
                    SaveGuestData();
                    RefreshCurrentPage();
                    break;
            }
            RefreshHud();
        }

        private void ShowSeedPanel(int index)
        {
            ShowSeedChoicePanel(index, () =>
            {
                plots[index] = PlotState.GrowingOne;
                plotTimers[index] = 12f;
                HideModal();
                Toast("Wheat planted");
                SaveGuestData();
                RefreshCurrentPage();
            });
        }

        private void OpenBakery()
        {
            if (bakeryDone)
            {
                bakeryDone = false;
                bakeryBusy = false;
                bread += 1;
                Toast("Collected Bread x1");
                SaveGuestData();
                RefreshHud();
                RefreshCurrentPage();
                return;
            }
            if (bakeryBusy)
            {
                Toast("Baking: " + Mathf.CeilToInt(bakeryTimer) + "s left");
                return;
            }
            ShowProcessPanel("Bakery", bakeryIcon, breadIcon, wheatIcon, "Wheat", wheat, 3, "Bread", 14f, () =>
            {
                if (wheat < 3)
                {
                    Toast("Need Wheat. Harvest farms first.");
                    return;
                }
                wheat -= 3;
                bakeryBusy = true;
                bakeryDone = false;
                bakeryTimer = 14f;
                HideModal();
                Toast("Bakery started");
                SaveGuestData();
                RefreshHud();
                RefreshCurrentPage();
            });
        }

        private void OpenDairy()
        {
            if (dairyDone)
            {
                dairyDone = false;
                dairyBusy = false;
                cheese += 1;
                Toast("Collected Cheese x1");
                SaveGuestData();
                RefreshHud();
                RefreshCurrentPage();
                return;
            }
            if (dairyBusy)
            {
                Toast("Dairy work: " + Mathf.CeilToInt(dairyTimer) + "s left");
                return;
            }
            ShowProcessPanel("Dairy", dairyIcon, milkIcon, milkIcon, "Milk", milk, 2, "Cheese", 16f, () =>
            {
                if (milk < 2)
                {
                    Toast("Need Milk. Use harvest or route rewards.");
                    return;
                }
                milk -= 2;
                dairyBusy = true;
                dairyDone = false;
                dairyTimer = 16f;
                HideModal();
                Toast("Dairy started");
                SaveGuestData();
                RefreshHud();
                RefreshCurrentPage();
            });
        }

        private void OpenResident(string resident)
        {
            var isMia = resident == "Mia";
            var done = isMia ? miaQuestDone : tomQuestDone;
            var need = isMia ? "Wheat x2" : "Bread x1";
            var favor = isMia ? miaFavor : tomFavor;
            ShowResidentPanel(resident, isMia ? NpcMia() : NpcTom(), need, favor, done, () =>
            {
                if (done)
                {
                    HideModal();
                    return;
                }
                if (isMia) SubmitMiaQuest(); else SubmitTomQuest();
                HideModal();
            });
        }

        private void SubmitBreadOrder()
        {
            if (breadOrderDone)
            {
                Toast("Order already done");
                return;
            }
            if (bread < 1)
            {
                Toast("Need Bread. Use the Bakery first.");
                return;
            }
            bread -= 1;
            coins += 320;
            wood += 6;
            breadOrderDone = true;
            Toast("Order done: Coins +320, Wood +6");
            SaveGuestData();
            RefreshAll();
        }

        private void SubmitCheeseOrder()
        {
            if (cheeseOrderDone)
            {
                Toast("Dairy order already done");
                return;
            }
            if (cheese < 1)
            {
                Toast("Need Cheese. Use the Dairy first.");
                return;
            }
            cheese -= 1;
            coins += 360;
            ore += 3;
            cheeseOrderDone = true;
            Toast("Order done: Coins +360, Ore +3");
            SaveGuestData();
            RefreshAll();
        }

        private void SubmitMiaQuest()
        {
            if (miaQuestDone)
            {
                Toast("Mia request already done");
                return;
            }
            if (wheat < 2)
            {
                Toast("Need Wheat x2");
                return;
            }
            wheat -= 2;
            miaFavor++;
            coins += 180;
            miaQuestDone = true;
            Toast("Mia favor up. Coins +180");
            SaveGuestData();
            RefreshAll();
        }

        private void SubmitTomQuest()
        {
            if (tomQuestDone)
            {
                Toast("Tom request already done");
                return;
            }
            if (bread < 1)
            {
                Toast("Need Bread x1");
                return;
            }
            bread -= 1;
            tomFavor++;
            ore += 2;
            tomQuestDone = true;
            Toast("Tom favor up. Ore +2");
            SaveGuestData();
            RefreshAll();
        }

        private void HarvestSpin()
        {
            if (harvestSpinning)
            {
                Toast("Harvest is spinning");
                return;
            }
            if (coins < 10)
            {
                Toast("Need Coins. Finish orders or requests.");
                return;
            }
            StartCoroutine(HarvestSpinSequence());
        }

        private IEnumerator HarvestSpinSequence()
        {
            harvestSpinning = true;
            coins -= 10;
            dailyHarvest++;
            wood += 1;
            if (dailyHarvest % 3 == 0) ore++;

            var pool = Symbols();
            if (harvestResultText != null) harvestResultText.text = "Spinning...";

            for (var step = 0; step < 9; step++)
            {
                for (var i = 0; i < reelImages.Count; i++)
                {
                    var flash = Pick(pool);
                    reelImages[i].sprite = flash.Sprite;
                    if (reelCells.Count > i && reelCells[i] != null)
                    {
                        reelCells[i].localScale = Vector3.one * (step % 2 == 0 ? 0.96f : 1.02f);
                    }
                }
                yield return new WaitForSeconds(0.025f + step * 0.008f);
            }

            var reward = 0;
            var results = new List<(string Name, Sprite Sprite, int Weight, int Pay)>();
            var counts = new Dictionary<string, int>();
            for (var i = 0; i < reelImages.Count; i++)
            {
                var symbol = Pick(pool);
                results.Add(symbol);
                reelImages[i].sprite = symbol.Sprite;
                if (reelLabels.Count > i && reelLabels[i] != null) reelLabels[i].text = symbol.Name;
                if (!counts.ContainsKey(symbol.Name)) counts[symbol.Name] = 0;
                counts[symbol.Name]++;
                if (reelCells.Count > i && reelCells[i] != null) reelCells[i].localScale = Vector3.one;
                if (i % 5 == 4) yield return new WaitForSeconds(0.055f);
            }

            var winners = new HashSet<string>();
            foreach (var item in counts)
            {
                if (item.Value < 3) continue;
                winners.Add(item.Key);
                var pay = 0;
                foreach (var symbol in pool)
                {
                    if (symbol.Name == item.Key)
                    {
                        pay = symbol.Pay;
                        break;
                    }
                }
                reward += Mathf.Max(1, pay) * item.Value;
            }

            yield return StartCoroutine(HighlightHarvestWinners(results, winners));

            if (reward < 10)
            {
                energy++;
                if (energy >= 6)
                {
                    energy = 0;
                    reward += 25;
                    Toast("Help Energy bonus: Coins +25");
                }
            }
            else
            {
                energy = 0;
            }
            coins += reward;
            if (harvestResultText != null)
            {
                harvestResultText.text = reward > 0 ? "Win +" + reward + " coins · Wood +1" : "No match · Wood +1";
            }
            Toast("Gained Coins +" + reward + ", Wood +1" + (dailyHarvest % 3 == 0 ? ", Ore +1" : ""));
            SaveGuestData();
            RefreshHud();
            harvestSpinning = false;
        }

        private IEnumerator HighlightHarvestWinners(List<(string Name, Sprite Sprite, int Weight, int Pay)> results, HashSet<string> winners)
        {
            if (winners == null || winners.Count == 0)
            {
                yield return new WaitForSeconds(0.08f);
                yield break;
            }

            for (var pulse = 0; pulse < 2; pulse++)
            {
                for (var i = 0; i < results.Count && i < reelCells.Count; i++)
                {
                    if (reelCells[i] == null) continue;
                    reelCells[i].localScale = winners.Contains(results[i].Name) ? Vector3.one * 1.13f : Vector3.one * 0.92f;
                }
                yield return new WaitForSeconds(0.11f);
                for (var i = 0; i < reelCells.Count; i++)
                {
                    if (reelCells[i] != null) reelCells[i].localScale = Vector3.one;
                }
                yield return new WaitForSeconds(0.08f);
            }
        }

        private List<(string Name, Sprite Sprite, int Weight, int Pay)> Symbols()
        {
            return new List<(string, Sprite, int, int)>
            {
                ("Wheat", wheatIcon, 24, 1),
                ("Bread", breadIcon, 16, 2),
                ("Milk", milkIcon, 16, 0),
                ("Apple", appleIcon, 14, 1),
                ("Gem", gemIcon, 8, 3),
                ("Mill", wildIcon, 10, 2),
                ("Basket", giftIcon, 12, 2),
            };
        }

        private (string Name, Sprite Sprite, int Weight, int Pay) Pick(List<(string Name, Sprite Sprite, int Weight, int Pay)> pool)
        {
            var total = 0;
            foreach (var item in pool) total += item.Weight;
            var roll = random.Next(total);
            foreach (var item in pool)
            {
                roll -= item.Weight;
                if (roll < 0) return item;
            }
            return pool[0];
        }

        private void ShowBottomPanel(string title, string body, string actionLabel, UnityEngine.Events.UnityAction action, Sprite portrait = null)
        {
            modalLayer.gameObject.SetActive(true);
            Clear(modalLayer);
            Panel(modalLayer, "Mask", C(0, 0, 0, 120), Vector2.zero, new Vector2(430, 932), null);
            var panel = Panel(modalLayer, "BottomPanel", C(255, 255, 255, 250), new Vector2(0, -284), new Vector2(430, 300), ScrollFrame());
            ImageNode(panel, "Corner", wheatCorner, new Vector2(-184, 110), new Vector2(44, 56), Color.white);
            if (portrait != null) ImageNode(panel, "Portrait", portrait, new Vector2(-134, 42), new Vector2(92, 120), Color.white);
            var hasPortrait = portrait != null;
            TextNode(panel, "Title", title, new Vector2(hasPortrait ? 40 : 0, 106), new Vector2(hasPortrait ? 230 : 330, 34), 22, C(46, 74, 43, 255), hasPortrait ? TextAnchor.MiddleLeft : TextAnchor.MiddleCenter);
            TextNode(panel, "Body", body, new Vector2(hasPortrait ? 52 : 0, 22), new Vector2(hasPortrait ? 238 : 330, 128), 16, C(73, 63, 48, 255), TextAnchor.MiddleCenter);
            ButtonNode(panel, "Action", actionLabel, new Vector2(0, -102), new Vector2(180, 44), C(232, 141, 47, 255), action, null);
            ButtonNode(panel, "Close", "X", new Vector2(184, 118), new Vector2(32, 32), C(46, 74, 43, 255), HideModal, SoftFrame());
        }

        private void ShowSeedChoicePanel(int plotIndex, UnityEngine.Events.UnityAction plantAction)
        {
            modalLayer.gameObject.SetActive(true);
            Clear(modalLayer);
            Panel(modalLayer, "Mask", C(0, 0, 0, 148), Vector2.zero, new Vector2(430, 932), null);

            // Keep the generated bottom sheet close to its native wide aspect ratio.
            // This prevents the decorative wheat frame from stretching vertically over the controls.
            var panel = Panel(modalLayer, "SeedChoicePanel", C(255, 250, 232, 255), new Vector2(0, -262), new Vector2(402, 238), SeedChoiceFrame());
            TextNode(panel, "Title", "Choose Seed", new Vector2(0, 86), new Vector2(230, 34), 23, C(255, 248, 222, 255), TextAnchor.MiddleCenter);
            TextNode(panel, "PlotHint", "Plot " + (plotIndex + 1), new Vector2(0, 55), new Vector2(200, 24), 15, C(111, 82, 42, 255), TextAnchor.MiddleCenter);

            var seedCard = Panel(panel, "WheatSeedCard", C(255, 251, 235, 252), new Vector2(-88, -3), new Vector2(154, 92), null);
            AddReadableBorder(seedCard, new Vector2(154, 92), 2f, C(181, 137, 74, 210));
            ImageNode(seedCard, "Icon", wheatIcon, new Vector2(-48, 12), new Vector2(40, 40), Color.white);
            TextNode(seedCard, "Name", "Wheat", new Vector2(26, 16), new Vector2(82, 26), 18, C(46, 74, 43, 255), TextAnchor.MiddleLeft);
            TextNode(seedCard, "Meta", "12s · Free", new Vector2(26, -13), new Vector2(88, 24), 15, C(111, 82, 42, 255), TextAnchor.MiddleLeft);
            var selected = Panel(seedCard, "Selected", C(70, 122, 73, 255), new Vector2(0, -36), new Vector2(154, 20), null);
            TextNode(selected, "Text", "SELECTED", Vector2.zero, new Vector2(140, 18), 13, C(255, 248, 222, 255), TextAnchor.MiddleCenter);

            var lockedCard = Panel(panel, "AppleSeedLocked", C(242, 236, 218, 248), new Vector2(88, -3), new Vector2(154, 92), null);
            AddReadableBorder(lockedCard, new Vector2(154, 92), 2f, C(181, 137, 74, 150));
            ImageNode(lockedCard, "Icon", appleIcon, new Vector2(-48, 12), new Vector2(40, 40), C(255, 255, 255, 125));
            ImageNode(lockedCard, "Lock", lockIcon, new Vector2(55, 27), new Vector2(22, 22), Color.white);
            TextNode(lockedCard, "Name", "Apple", new Vector2(26, 16), new Vector2(82, 26), 18, C(86, 86, 78, 230), TextAnchor.MiddleLeft);
            TextNode(lockedCard, "Meta", "Unlock later", new Vector2(26, -13), new Vector2(92, 24), 15, C(120, 112, 96, 230), TextAnchor.MiddleLeft);

            ButtonNode(panel, "Plant", "Plant Wheat", new Vector2(0, -87), new Vector2(220, 46), C(232, 141, 47, 255), plantAction, null);
            ButtonNode(panel, "Close", "X", new Vector2(174, 87), new Vector2(38, 38), C(126, 70, 48, 255), HideModal, null);
        }

        private void ShowProcessPanel(string title, Sprite building, Sprite product, Sprite material, string materialName, int materialCount, int materialNeed, string productName, float seconds, UnityEngine.Events.UnityAction processAction)
        {
            modalLayer.gameObject.SetActive(true);
            Clear(modalLayer);
            Panel(modalLayer, "Mask", C(0, 0, 0, 145), Vector2.zero, new Vector2(430, 932), null);

            var panel = Panel(modalLayer, title + "ProcessPanel", C(255, 250, 232, 252), new Vector2(0, -246), new Vector2(386, 356), null);
            AddReadableBorder(panel, new Vector2(386, 356), 3f, C(124, 86, 42, 230));
            var header = Panel(panel, "Header", C(35, 82, 54, 245), new Vector2(0, 142), new Vector2(386, 54), null);
            TextNode(header, "Title", title, new Vector2(-88, 0), new Vector2(190, 30), 24, C(255, 248, 222, 255), TextAnchor.MiddleLeft);
            TextNode(header, "SlotLabel", "Craft Slot 1 / 1", new Vector2(100, 0), new Vector2(150, 24), 14, C(255, 235, 190, 255), TextAnchor.MiddleRight);
            ButtonNode(panel, "Close", "X", new Vector2(164, 142), new Vector2(38, 38), C(126, 70, 48, 255), HideModal, null);

            ImageNode(panel, "Building", building, new Vector2(-126, 66), new Vector2(82, 86), Color.white);
            TextNode(panel, "BuildingHint", "Production building", new Vector2(56, 84), new Vector2(210, 26), 16, C(70, 64, 52, 255), TextAnchor.MiddleLeft);
            ProgressBar(panel, new Vector2(56, 54), new Vector2(210, 16), 0f);

            var recipe = Panel(panel, "RecipeRow", C(255, 246, 219, 255), new Vector2(0, -28), new Vector2(326, 96), null);
            AddReadableBorder(recipe, new Vector2(326, 96), 2f, C(181, 137, 74, 200));
            ImageNode(recipe, "Product", product, new Vector2(-132, 14), new Vector2(50, 50), Color.white);
            TextNode(recipe, "Name", productName, new Vector2(-58, 22), new Vector2(126, 26), 18, C(46, 74, 43, 255), TextAnchor.MiddleLeft);
            TextNode(recipe, "Time", "Time: " + Mathf.RoundToInt(seconds) + "s", new Vector2(-58, -10), new Vector2(126, 22), 14, C(111, 82, 42, 255), TextAnchor.MiddleLeft);
            ImageNode(recipe, "Material", material, new Vector2(74, 10), new Vector2(32, 32), Color.white);
            TextNode(recipe, "Need", materialName + " " + materialCount + " / " + materialNeed, new Vector2(126, 10), new Vector2(88, 24), 14, materialCount >= materialNeed ? C(46, 110, 72, 255) : C(160, 80, 55, 255), TextAnchor.MiddleLeft);

            ButtonNode(panel, "Process", materialCount >= materialNeed ? "Craft" : "Need Materials", new Vector2(0, -126), new Vector2(210, 46), materialCount >= materialNeed ? C(232, 141, 47, 255) : C(168, 150, 128, 255), processAction, null);
        }

        private void ShowResidentPanel(string resident, Sprite portrait, string need, int favor, bool done, UnityEngine.Events.UnityAction submit)
        {
            modalLayer.gameObject.SetActive(true);
            Clear(modalLayer);
            Panel(modalLayer, "Mask", C(0, 0, 0, 165), Vector2.zero, new Vector2(430, 932), null);

            var panel = Panel(modalLayer, "ResidentCommissionPanel", C(255, 251, 235, 255), new Vector2(0, -26), new Vector2(386, 432), null);
            AddReadableBorder(panel, new Vector2(386, 432), 3f, C(124, 86, 42, 235));
            var header = Panel(panel, "Header", C(35, 82, 54, 255), new Vector2(0, 187), new Vector2(386, 58), null);
            TextNode(header, "Title", resident + " Request", new Vector2(-42, 0), new Vector2(252, 34), 23, C(255, 248, 222, 255), TextAnchor.MiddleLeft);
            ButtonNode(panel, "Close", "X", new Vector2(164, 187), new Vector2(42, 42), C(126, 70, 48, 255), HideModal, null);

            Panel(panel, "PortraitBack", C(255, 247, 224, 255), new Vector2(-126, 62), new Vector2(118, 148), null);
            AddReadableBorder(panel.Find("PortraitBack").GetComponent<RectTransform>(), new Vector2(118, 148), 2f, C(181, 137, 74, 190));
            ImageNode(panel, "Portrait", portrait, new Vector2(-126, 70), new Vector2(92, 120), Color.white);
            TextNode(panel, "ResidentName", resident, new Vector2(-126, -18), new Vector2(116, 26), 18, C(46, 74, 43, 255), TextAnchor.MiddleCenter);
            TextNode(panel, "Dialogue", "Let's care for town today.", new Vector2(68, 118), new Vector2(226, 34), 16, C(73, 63, 48, 255), TextAnchor.MiddleLeft);
            InfoRow(panel, "Need", need, new Vector2(68, 68));
            InfoRow(panel, "Status", done ? "Done today" : "Waiting", new Vector2(68, 18));
            InfoRow(panel, "Reward", "Coins + favor", new Vector2(68, -32));
            ImageNode(panel, "HeartBadge", FavorHeart(), new Vector2(-170, -116), new Vector2(28, 28), Color.white);
            ProgressBar(panel, new Vector2(-106, -116), new Vector2(112, 16), Mathf.Clamp01(favor / 5f));
            TextNode(panel, "Favor", favor + " / 5", new Vector2(-32, -116), new Vector2(64, 22), 15, C(91, 82, 67, 255), TextAnchor.MiddleLeft);
            ButtonNode(panel, "Submit", done ? "Close" : "Submit", new Vector2(82, -144), new Vector2(196, 52), done ? C(46, 74, 43, 255) : C(232, 141, 47, 255), submit, null);
        }

        private void ShowSettings()
        {
            modalLayer.gameObject.SetActive(true);
            Clear(modalLayer);
            Panel(modalLayer, "Mask", C(0, 0, 0, 145), Vector2.zero, new Vector2(430, 932), null);

            var dialog = Panel(modalLayer, "SettingsDialog", Color.white, Vector2.zero, new Vector2(398, 630), SettingsDialogFrame());
            TextNode(dialog, "Title", "Settings", new Vector2(0, 250), new Vector2(220, 38), 28, C(255, 248, 222, 255), TextAnchor.MiddleCenter);
            TextNode(dialog, "Hint", "Audio / Save", new Vector2(0, 202), new Vector2(250, 28), 16, C(94, 75, 48, 255), TextAnchor.MiddleCenter);
            ButtonNode(dialog, "CloseTop", "X", new Vector2(168, 248), new Vector2(42, 42), C(126, 70, 48, 255), HideModal, null);

            SettingToggleRow(dialog, "Music", musicEnabled, new Vector2(0, 140), () =>
            {
                musicEnabled = !musicEnabled;
                ShowSettings();
            });
            SettingToggleRow(dialog, "SFX", sfxEnabled, new Vector2(0, 76), () =>
            {
                sfxEnabled = !sfxEnabled;
                ShowSettings();
            });

            VolumeStepper(dialog, new Vector2(0, 10));

            ButtonNode(dialog, "Privacy", "Privacy", new Vector2(-90, -86), new Vector2(150, 46), C(46, 74, 43, 255), () => ShowAgreement("Privacy"), null);
            ButtonNode(dialog, "Terms", "Terms", new Vector2(90, -86), new Vector2(150, 46), C(46, 74, 43, 255), () => ShowAgreement("Terms"), null);
            ButtonNode(dialog, "Logout", "Save & Log out", new Vector2(0, -160), new Vector2(272, 52), C(46, 74, 43, 255), LogoutToAuth, null);
            ButtonNode(dialog, "DeleteSave", "Delete Save", new Vector2(0, -230), new Vector2(246, 50), C(126, 70, 48, 255), DeleteGuestSaveFromSettings, null);
            TextNode(dialog, "NativeTip", "Local guest save only. No web page is opened.", new Vector2(0, -278), new Vector2(318, 30), 15, C(96, 84, 64, 255), TextAnchor.MiddleCenter);
        }

        private void SettingToggleRow(RectTransform parent, string label, bool value, Vector2 pos, UnityEngine.Events.UnityAction action)
        {
            var row = Panel(parent, label + "Row", C(255, 246, 219, 18), pos, new Vector2(326, 50), null);
            TextNode(row, "Label", label, new Vector2(-104, 0), new Vector2(120, 30), 20, C(46, 74, 43, 255), TextAnchor.MiddleLeft);
            ButtonNode(row, "Toggle", value ? "On" : "Off", new Vector2(110, 0), new Vector2(94, 42), value ? C(70, 122, 73, 255) : C(156, 148, 132, 255), action, null);
        }

        private void VolumeStepper(RectTransform parent, Vector2 pos)
        {
            var row = Panel(parent, "VolumeRow", C(255, 246, 219, 18), pos, new Vector2(326, 52), null);
            TextNode(row, "VolumeLabel", "Volume", new Vector2(-106, 0), new Vector2(94, 30), 20, C(46, 74, 43, 255), TextAnchor.MiddleLeft);
            ButtonNode(row, "VolumeDown", "-", new Vector2(18, 0), new Vector2(44, 40), C(46, 74, 43, 255), () =>
            {
                masterVolume = Mathf.Clamp01(masterVolume - .1f);
                AudioListener.volume = masterVolume;
                ShowSettings();
            }, null);
            TextNode(row, "VolumeValue", Mathf.RoundToInt(masterVolume * 100f) + "%", new Vector2(78, 0), new Vector2(64, 30), 19, C(73, 63, 48, 255), TextAnchor.MiddleCenter);
            ButtonNode(row, "VolumeUp", "+", new Vector2(136, 0), new Vector2(44, 40), C(46, 74, 43, 255), () =>
            {
                masterVolume = Mathf.Clamp01(masterVolume + .1f);
                AudioListener.volume = masterVolume;
                ShowSettings();
            }, null);
        }

        private void SliderNode(RectTransform parent, Vector2 pos, Vector2 size)
        {
            var slider = NewObject<Slider>("VolumeSlider", parent);
            var rect = slider.GetComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;

            var background = Panel(rect, "Background", C(245, 226, 183, 255), Vector2.zero, new Vector2(size.x + 20, size.y + 14), null);
            AddReadableBorder(background, new Vector2(size.x + 20, size.y + 14), 2f, C(181, 137, 74, 180));
            var fillArea = Panel(rect, "Fill Area", Color.clear, Vector2.zero, size, null);
            Stretch(fillArea);
            fillArea.offsetMin = new Vector2(10, 0);
            fillArea.offsetMax = new Vector2(-10, 0);
            var fill = Panel(fillArea, "Fill", C(70, 122, 73, 190), Vector2.zero, new Vector2(size.x * masterVolume, 6), null);
            fill.anchorMin = new Vector2(0, .5f);
            fill.anchorMax = new Vector2(0, .5f);
            fill.pivot = new Vector2(0, .5f);
            fill.anchoredPosition = new Vector2(-size.x * .5f + 10, 0);
            var handleArea = Panel(rect, "Handle Slide Area", Color.clear, Vector2.zero, size, null);
            Stretch(handleArea);
            var handle = Panel(handleArea, "Handle", C(232, 141, 47, 255), Vector2.zero, new Vector2(24, 24), null);
            AddReadableBorder(handle, new Vector2(24, 24), 2f, C(105, 68, 35, 230));
            handle.GetComponent<Image>().raycastTarget = true;

            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = masterVolume;
            slider.targetGraphic = handle.GetComponent<Image>();
            slider.fillRect = null;
            slider.handleRect = handle;
            slider.onValueChanged.AddListener(value =>
            {
                masterVolume = value;
                AudioListener.volume = value;
                fill.sizeDelta = new Vector2(size.x * value, 6);
            });
            TextNode(rect, "Value", Mathf.RoundToInt(masterVolume * 100f) + "%", new Vector2(102, 0), new Vector2(50, 20), 13, C(73, 63, 48, 255), TextAnchor.MiddleLeft);
        }

        private void ShowAgreement(string title)
        {
            modalLayer.gameObject.SetActive(true);
            Clear(modalLayer);
            Panel(modalLayer, "Mask", C(0, 0, 0, 150), Vector2.zero, new Vector2(430, 932), null);
            var dialog = Panel(modalLayer, title, C(255, 255, 255, 252), Vector2.zero, new Vector2(380, 430), AgreementScrollFrame());
            TextNode(dialog, "Title", title, new Vector2(0, 160), new Vector2(300, 38), 24, C(46, 74, 43, 255), TextAnchor.MiddleCenter);
            TextNode(dialog, "Body", "Prototype local notice.\nA final build needs full terms, privacy notice, age rating, and platform compliance setup.\n\nNo external value exchange. No web page is opened.", new Vector2(0, 24), new Vector2(304, 220), 14, C(73, 63, 48, 255), TextAnchor.MiddleCenter);
            ButtonNode(dialog, "BackSettings", "Back", new Vector2(-82, -158), new Vector2(130, 40), C(46, 74, 43, 255), ShowSettings, SoftFrame());
            ButtonNode(dialog, "Close", "Close", new Vector2(82, -158), new Vector2(130, 42), C(232, 141, 47, 255), HideModal, null);
        }

        private void ShowCollection()
        {
            modalLayer.gameObject.SetActive(true);
            Clear(modalLayer);
            Panel(modalLayer, "Mask", C(0, 0, 0, 158), Vector2.zero, new Vector2(430, 932), null);
            var dialog = Panel(modalLayer, "CollectionDialog", C(250, 255, 240, 255), Vector2.zero, new Vector2(386, 500), TaskOrderBoardFrame());
            ImageNode(dialog, "BookIcon", TaskCollectionBook(), new Vector2(-118, 210), new Vector2(34, 32), Color.white);
            TextNode(dialog, "Title", "Town Album", new Vector2(10, 210), new Vector2(230, 36), 24, C(255, 248, 222, 255), TextAnchor.MiddleCenter);
            ButtonNode(dialog, "CloseTop", "X", new Vector2(166, 210), new Vector2(40, 40), C(126, 70, 48, 255), HideModal, null);

            CollectionRow(dialog, "Wheat Album", wheatIcon, dailyHarvest, 18, new Vector2(0, 116));
            CollectionRow(dialog, "Bread Baking", breadIcon, bread, 3, new Vector2(0, 50));
            CollectionRow(dialog, "Friend Favor", giftIcon, miaFavor + tomFavor, 10, new Vector2(0, -16));

            var tip = Panel(dialog, "TipBox", C(255, 246, 219, 248), new Vector2(0, -92), new Vector2(314, 58), null);
            AddReadableBorder(tip, new Vector2(314, 58), 2f, C(181, 137, 74, 180));
            TextNode(tip, "Tip", "Album rewards are one-time town-growth resources only.", Vector2.zero, new Vector2(286, 42), 16, C(73, 63, 48, 255), TextAnchor.MiddleCenter);
            ButtonNode(dialog, "Close", "OK", new Vector2(0, -170), new Vector2(190, 50), C(232, 141, 47, 255), HideModal, null);
        }

        private void CollectionRow(RectTransform parent, string label, Sprite icon, int value, int target, Vector2 pos)
        {
            var row = Panel(parent, label + "Row", C(255, 246, 219, 250), pos, new Vector2(314, 58), null);
            AddReadableBorder(row, new Vector2(314, 58), 2f, C(181, 137, 74, 180));
            ImageNode(row, "Icon", icon, new Vector2(-128, 0), new Vector2(36, 36), Color.white);
            TextNode(row, "Label", label, new Vector2(-50, 12), new Vector2(142, 26), 17, C(46, 74, 43, 255), TextAnchor.MiddleLeft);
            ProgressBar(row, new Vector2(42, -12), new Vector2(144, 10), Mathf.Clamp01((float)value / target));
            TextNode(row, "Value", value + " / " + target, new Vector2(116, 12), new Vector2(70, 22), 15, C(111, 82, 42, 255), TextAnchor.MiddleRight);
        }

        private void InfoRow(RectTransform parent, string label, string value, Vector2 pos)
        {
            var row = Panel(parent, label + "InfoRow", C(255, 246, 219, 255), pos, new Vector2(232, 40), null);
            AddReadableBorder(row, new Vector2(232, 40), 2f, C(181, 137, 74, 180));
            TextNode(row, "Label", label, new Vector2(-80, 0), new Vector2(68, 24), 15, C(46, 74, 43, 255), TextAnchor.MiddleLeft);
            TextNode(row, "Value", value, new Vector2(34, 0), new Vector2(136, 24), 15, C(73, 63, 48, 255), TextAnchor.MiddleLeft);
        }

        private void GuestLogin()
        {
            if (!AuthFormReady())
            {
                return;
            }
            if (HasGuestSave()) LoadGuestSave();
            else SaveGuestData();
            ShowAuth(false);
            ShowPage(Page.Town);
            Toast("Guest login success. Local save enabled.");
        }

        private void AccountLogin()
        {
            if (!AuthFormReady())
            {
                return;
            }
            Toast("Account not found. Use Guest Login.");
        }

        private void ContinueGuestSave()
        {
            if (!AuthFormReady()) return;
            LoadGuestSave();
            ShowAuth(false);
            ShowPage(Page.Town);
            Toast("Save loaded");
        }

        private void DeleteGuestSave()
        {
            PlayerPrefs.DeleteKey(SavePrefix + "exists");
            PlayerPrefs.DeleteKey(SavePrefix + "coins");
            PlayerPrefs.DeleteKey(SavePrefix + "wood");
            PlayerPrefs.DeleteKey(SavePrefix + "ore");
            PlayerPrefs.DeleteKey(SavePrefix + "wheat");
            PlayerPrefs.DeleteKey(SavePrefix + "apple");
            PlayerPrefs.DeleteKey(SavePrefix + "milk");
            PlayerPrefs.DeleteKey(SavePrefix + "bread");
            PlayerPrefs.DeleteKey(SavePrefix + "cheese");
            PlayerPrefs.DeleteKey(SavePrefix + "dailyHarvest");
            PlayerPrefs.DeleteKey(SavePrefix + "energy");
            PlayerPrefs.DeleteKey(SavePrefix + "miaFavor");
            PlayerPrefs.DeleteKey(SavePrefix + "tomFavor");
            PlayerPrefs.DeleteKey(SavePrefix + "breadOrderDone");
            PlayerPrefs.DeleteKey(SavePrefix + "cheeseOrderDone");
            PlayerPrefs.DeleteKey(SavePrefix + "miaQuestDone");
            PlayerPrefs.DeleteKey(SavePrefix + "tomQuestDone");
            for (var i = 0; i < plots.Length; i++)
            {
                PlayerPrefs.DeleteKey(SavePrefix + "plotState" + i);
                PlayerPrefs.DeleteKey(SavePrefix + "plotTimer" + i);
            }
            PlayerPrefs.Save();
            ResetGuestState();
            RebuildAuthContent();
            Toast("Local guest save cleared");
        }

        private bool AuthFormReady()
        {
            if (string.IsNullOrWhiteSpace(accountInput.text) || string.IsNullOrWhiteSpace(passwordInput.text))
            {
                Toast("Enter ID and password");
                return false;
            }
            if (!agreementToggle.isOn)
            {
                Toast("Please agree to Privacy and Terms first.");
                return false;
            }
            return true;
        }

        private void LogoutToAuth()
        {
            SaveGuestData();
            HideModal();
            ShowAuth(true);
            Toast("Saved. Please sign in again.");
        }

        private void DeleteGuestSaveFromSettings()
        {
            HideModal();
            DeleteGuestSave();
            ShowAuth(true);
        }

        private bool HasGuestSave()
        {
            return PlayerPrefs.GetInt(SavePrefix + "exists", 0) == 1;
        }

        private void SaveGuestData()
        {
            PlayerPrefs.SetInt(SavePrefix + "exists", 1);
            PlayerPrefs.SetInt(SavePrefix + "coins", coins);
            PlayerPrefs.SetInt(SavePrefix + "wood", wood);
            PlayerPrefs.SetInt(SavePrefix + "ore", ore);
            PlayerPrefs.SetInt(SavePrefix + "wheat", wheat);
            PlayerPrefs.SetInt(SavePrefix + "apple", apple);
            PlayerPrefs.SetInt(SavePrefix + "milk", milk);
            PlayerPrefs.SetInt(SavePrefix + "bread", bread);
            PlayerPrefs.SetInt(SavePrefix + "cheese", cheese);
            PlayerPrefs.SetInt(SavePrefix + "dailyHarvest", dailyHarvest);
            PlayerPrefs.SetInt(SavePrefix + "energy", energy);
            PlayerPrefs.SetInt(SavePrefix + "miaFavor", miaFavor);
            PlayerPrefs.SetInt(SavePrefix + "tomFavor", tomFavor);
            PlayerPrefs.SetInt(SavePrefix + "breadOrderDone", breadOrderDone ? 1 : 0);
            PlayerPrefs.SetInt(SavePrefix + "cheeseOrderDone", cheeseOrderDone ? 1 : 0);
            PlayerPrefs.SetInt(SavePrefix + "miaQuestDone", miaQuestDone ? 1 : 0);
            PlayerPrefs.SetInt(SavePrefix + "tomQuestDone", tomQuestDone ? 1 : 0);
            for (var i = 0; i < plots.Length; i++)
            {
                PlayerPrefs.SetInt(SavePrefix + "plotState" + i, (int)plots[i]);
                PlayerPrefs.SetFloat(SavePrefix + "plotTimer" + i, plotTimers[i]);
            }
            PlayerPrefs.Save();
        }

        private void LoadGuestSave()
        {
            coins = PlayerPrefs.GetInt(SavePrefix + "coins", 5000);
            wood = PlayerPrefs.GetInt(SavePrefix + "wood", 0);
            ore = PlayerPrefs.GetInt(SavePrefix + "ore", 0);
            wheat = PlayerPrefs.GetInt(SavePrefix + "wheat", 0);
            apple = PlayerPrefs.GetInt(SavePrefix + "apple", 0);
            milk = PlayerPrefs.GetInt(SavePrefix + "milk", 2);
            bread = PlayerPrefs.GetInt(SavePrefix + "bread", 0);
            cheese = PlayerPrefs.GetInt(SavePrefix + "cheese", 0);
            dailyHarvest = PlayerPrefs.GetInt(SavePrefix + "dailyHarvest", 0);
            energy = PlayerPrefs.GetInt(SavePrefix + "energy", 0);
            miaFavor = PlayerPrefs.GetInt(SavePrefix + "miaFavor", 0);
            tomFavor = PlayerPrefs.GetInt(SavePrefix + "tomFavor", 0);
            breadOrderDone = PlayerPrefs.GetInt(SavePrefix + "breadOrderDone", 0) == 1;
            cheeseOrderDone = PlayerPrefs.GetInt(SavePrefix + "cheeseOrderDone", 0) == 1;
            miaQuestDone = PlayerPrefs.GetInt(SavePrefix + "miaQuestDone", 0) == 1;
            tomQuestDone = PlayerPrefs.GetInt(SavePrefix + "tomQuestDone", 0) == 1;
            for (var i = 0; i < plots.Length; i++)
            {
                plots[i] = (PlotState)PlayerPrefs.GetInt(SavePrefix + "plotState" + i, i < 3 ? (int)PlotState.Empty : (int)PlotState.Locked);
                plotTimers[i] = PlayerPrefs.GetFloat(SavePrefix + "plotTimer" + i, 0f);
            }
            bakeryBusy = false;
            bakeryDone = false;
            dairyBusy = false;
            dairyDone = false;
            RefreshAll();
        }

        private void ResetGuestState()
        {
            coins = 5000;
            wood = 0;
            ore = 0;
            wheat = 0;
            apple = 0;
            milk = 2;
            bread = 0;
            cheese = 0;
            dailyHarvest = 0;
            energy = 0;
            miaFavor = 0;
            tomFavor = 0;
            bakeryBusy = false;
            bakeryDone = false;
            dairyBusy = false;
            dairyDone = false;
            breadOrderDone = false;
            cheeseOrderDone = false;
            miaQuestDone = false;
            tomQuestDone = false;
            for (var i = 0; i < plots.Length; i++)
            {
                plots[i] = i < 3 ? PlotState.Empty : PlotState.Locked;
                plotTimers[i] = 0f;
            }
        }

        private void SanitizeInput(TMP_InputField input)
        {
            var clean = Regex.Replace(input.text ?? string.Empty, "[^a-zA-Z0-9]", "");
            if (clean == input.text) return;
            input.SetTextWithoutNotify(clean);
            Toast("Only letters and numbers are allowed");
        }

        private void ShowPage(Page page)
        {
            activePage = page;
            if (bottomNavBar != null) bottomNavBar.gameObject.SetActive(true);
            SetPageVisible(townPage, page == Page.Town, false);
            SetPageVisible(bagPage, page == Page.Bag, false);
            SetPageVisible(taskPage, page == Page.Task, false);
            SetPageVisible(harvestPage, false, false);
            RefreshAll();
            FadeIn(ActivePageTransform());
        }

        private void ShowHarvest(bool show)
        {
            if (bottomNavBar != null) bottomNavBar.gameObject.SetActive(!show);
            SetPageVisible(harvestPage, show, false);
            SetPageVisible(townPage, !show && activePage == Page.Town, false);
            SetPageVisible(bagPage, !show && activePage == Page.Bag, false);
            SetPageVisible(taskPage, !show && activePage == Page.Task, false);
            RefreshHud();
            FadeIn(show ? harvestPage : ActivePageTransform());
        }

        private void ShowAuth(bool show)
        {
            if (show) RebuildAuthContent();
            authLayer.gameObject.SetActive(show);
            hudLayer.gameObject.SetActive(!show);
        }

        private void HideModal()
        {
            modalLayer.gameObject.SetActive(false);
        }

        private void RefreshAll()
        {
            RefreshHud();
            RefreshCurrentPage();
        }

        private void RefreshCurrentPage()
        {
            if (activePage == Page.Town) BuildTownPage();
            if (activePage == Page.Bag) BuildBagPage();
            if (activePage == Page.Task) BuildTaskPage();
            if (harvestPage.gameObject.activeSelf) RefreshHud();
        }

        private void RefreshHud()
        {
            if (coinText != null) coinText.text = coins.ToString("N0");
            if (woodText != null) woodText.text = wood.ToString("N0");
            if (oreText != null) oreText.text = ore.ToString("N0");
            if (harvestProgressText != null) harvestProgressText.text = dailyHarvest + " / 400";
            if (energyText != null) energyText.text = "Help Energy " + energy + " / 6";
            RefreshNavState();
        }

        private void RefreshNavState()
        {
            for (var i = 0; i < navButtons.Count; i++)
            {
                var active = (i == 0 && activePage == Page.Town) || (i == 1 && activePage == Page.Bag) || (i == 2 && activePage == Page.Task);
                var image = navButtons[i].GetComponent<Image>();
                if (image != null) image.color = active ? C(255, 255, 255, 255) : C(255, 255, 255, 145);
                foreach (var text in navButtons[i].GetComponentsInChildren<TMP_Text>())
                {
                    text.color = active ? C(255, 248, 222, 255) : C(205, 214, 190, 255);
                }
                foreach (var icon in navButtons[i].GetComponentsInChildren<Image>())
                {
                    if (icon.gameObject == navButtons[i].gameObject) continue;
                    icon.color = active ? Color.white : C(220, 230, 210, 190);
                }
            }
        }

        private RectTransform ActivePageTransform()
        {
            if (activePage == Page.Bag) return bagPage;
            if (activePage == Page.Task) return taskPage;
            return townPage;
        }

        private void SetPageVisible(RectTransform page, bool visible, bool instant)
        {
            if (page == null) return;
            page.gameObject.SetActive(visible);
            var group = EnsureCanvasGroup(page);
            group.alpha = visible || instant ? 1f : 0f;
            group.interactable = visible;
            group.blocksRaycasts = visible;
        }

        private CanvasGroup EnsureCanvasGroup(RectTransform page)
        {
            var group = page.GetComponent<CanvasGroup>();
            if (group == null) group = page.gameObject.AddComponent<CanvasGroup>();
            return group;
        }

        private void FadeIn(RectTransform page)
        {
            if (page == null || !page.gameObject.activeSelf) return;
            if (pageFadeRoutine != null) StopCoroutine(pageFadeRoutine);
            pageFadeRoutine = StartCoroutine(FadeInRoutine(page));
        }

        private IEnumerator FadeInRoutine(RectTransform page)
        {
            var group = EnsureCanvasGroup(page);
            group.alpha = 0f;
            var elapsed = 0f;
            const float duration = .18f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                group.alpha = Mathf.Clamp01(elapsed / duration);
                yield return null;
            }
            group.alpha = 1f;
            pageFadeRoutine = null;
        }

        private void Toast(string message)
        {
            if (toastText == null) return;
            toastText.text = message;
            toastBox.gameObject.SetActive(true);
            CancelInvoke(nameof(HideToast));
            Invoke(nameof(HideToast), 1.8f);
        }

        private void HideToast()
        {
            if (toastBox != null) toastBox.gameObject.SetActive(false);
        }

        private void BuildModalLayer()
        {
            modalLayer = Panel(root, "ModalLayer", Color.clear, Vector2.zero, new Vector2(430, 932), null);
            Stretch(modalLayer);
            modalLayer.gameObject.SetActive(false);
        }

        private void BuildToast()
        {
            toastBox = Panel(root, "Toast", C(27, 52, 39, 242), new Vector2(0, -350), new Vector2(330, 38), v3StatusBarGreen);
            toastText = TextNode(toastBox, "Text", "", Vector2.zero, new Vector2(310, 30), 14, Color.white, TextAnchor.MiddleCenter);
            toastBox.gameObject.SetActive(false);
        }

        private TMP_InputField Input(Transform parent, string label, Vector2 pos, string placeholder, bool password)
        {
            var box = Panel(parent, label + "Box", C(255, 253, 246, 255), pos, new Vector2(294, 52), null);
            AddReadableBorder(box, new Vector2(294, 52), 2f, C(176, 133, 75, 230));
            TextNode(box, "Label", label, new Vector2(-122, 0), new Vector2(48, 26), 16, C(46, 74, 43, 255), TextAnchor.MiddleLeft);
            var input = NewObject<TMP_InputField>(label + "Input", box);
            var hit = input.gameObject.AddComponent<Image>();
            hit.color = Color.clear;
            hit.raycastTarget = true;
            var rect = input.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(210, 38);
            rect.anchoredPosition = new Vector2(42, 0);
            input.targetGraphic = hit;
            input.textComponent = TextNode(rect, "Text", "", Vector2.zero, new Vector2(210, 32), 18, C(20, 32, 24, 255), TextAnchor.MiddleLeft);
            input.placeholder = TextNode(rect, "Placeholder", placeholder, Vector2.zero, new Vector2(210, 32), 16, C(116, 108, 92, 255), TextAnchor.MiddleLeft);
            input.contentType = password ? TMP_InputField.ContentType.Password : TMP_InputField.ContentType.Standard;
            return input;
        }

        private Toggle ToggleNode(Transform parent, string label, Vector2 pos)
        {
            var toggle = NewObject<Toggle>("Agreement", parent);
            var rect = toggle.GetComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(316, 42);
            var bg = Panel(rect, "Box", C(255, 255, 255, 255), new Vector2(-144, 0), new Vector2(26, 26), null);
            AddReadableBorder(bg, new Vector2(26, 26), 2f, C(176, 133, 75, 230));
            var check = Panel(bg, "Checkmark", C(232, 141, 47, 255), Vector2.zero, new Vector2(16, 16), null);
            bg.GetComponent<Image>().raycastTarget = true;
            check.GetComponent<Image>().raycastTarget = false;
            TextNode(rect, "Label", label, new Vector2(20, 0), new Vector2(274, 32), 16, C(55, 68, 55, 255), TextAnchor.MiddleLeft);
            toggle.targetGraphic = bg.GetComponent<Image>();
            toggle.graphic = check.GetComponent<Image>();
            toggle.isOn = false;
            return toggle;
        }

        private void ProgressBar(Transform parent, Vector2 pos, Vector2 size, float progress)
        {
            var track = Panel(parent, "ProgressTrack", C(255, 255, 255, 230), pos, size, v6ProgressBarFrame != null ? v6ProgressBarFrame : LabelFrame());
            var fill = Panel(track, "ProgressFill", C(70, 122, 73, 210), Vector2.zero, new Vector2(size.x * Mathf.Clamp01(progress), Mathf.Max(4, size.y * .45f)), null);
            fill.anchorMin = new Vector2(0, .5f);
            fill.anchorMax = new Vector2(0, .5f);
            fill.pivot = new Vector2(0, .5f);
            fill.anchoredPosition = new Vector2(-size.x * .5f, 0);
            ImageNode(track, "EndCap", wheatCorner, new Vector2(size.x * .5f - 12, 0), new Vector2(18, 22), Color.white);
        }

        private void LabelPlate(Transform parent, string label, Vector2 pos, Vector2 size)
        {
            var plate = Panel(parent, label + "LabelPlate", C(24, 58, 39, 238), pos, size, null);
            AddReadableBorder(plate, size, 2f, C(181, 137, 74, 150));
            TextNode(plate, "Text", label, Vector2.zero, new Vector2(size.x - 12, size.y - 4), 15, C(255, 248, 222, 255), TextAnchor.MiddleCenter);
        }

        private Sprite OrnateFrame()
        {
            return v3PanelOrnate != null ? v3PanelOrnate : (infoCard != null ? infoCard : cardFrame);
        }

        private Sprite CleanMainPanel()
        {
            return v8CleanPanel != null ? v8CleanPanel :
                (v6MainPanelFrame != null ? v6MainPanelFrame :
                (v6InfoCardFrame != null ? v6InfoCardFrame : OrnateFrame()));
        }

        private Sprite CompactPanel()
        {
            return v6InfoCardFrame != null ? v6InfoCardFrame :
                (v6MainPanelFrame != null ? v6MainPanelFrame : SoftFrame());
        }

        private Sprite ScrollFrame()
        {
            return v3DialogScroll != null ? v3DialogScroll : (dialogueCard != null ? dialogueCard : cardFrame);
        }

        private Sprite SoftFrame()
        {
            return v6SecondaryButton != null ? v6SecondaryButton : (v3ButtonSmallGold != null ? v3ButtonSmallGold : (cardFrame != null ? cardFrame : creamGoldLabel));
        }

        private Sprite LabelFrame()
        {
            return v3TabRight != null ? v3TabRight : (creamGoldLabel != null ? creamGoldLabel : cardFrame);
        }

        private Sprite ItemFrame()
        {
            return v6ItemSlotFrame != null ? v6ItemSlotFrame : (symbolTile != null ? symbolTile : LabelFrame());
        }

        private Sprite NpcMia()
        {
            return npcMiaFull != null ? npcMiaFull : miaIcon;
        }

        private Sprite NpcTom()
        {
            return npcTomFull != null ? npcTomFull : tomIcon;
        }

        private Sprite FavorHeart()
        {
            return favorHeartIcon != null ? favorHeartIcon : (v3BadgeRed != null ? v3BadgeRed : completeBubbleIcon);
        }

        private Sprite CommissionMark()
        {
            return commissionMarkIcon != null ? commissionMarkIcon : (v3BadgeRed != null ? v3BadgeRed : completeBubbleIcon);
        }

        private Sprite HarvestConsoleFrame()
        {
            return harvestConsoleFrame != null ? harvestConsoleFrame : OrnateFrame();
        }

        private Sprite HarvestCellTile()
        {
            return harvestCellTile != null ? harvestCellTile : ItemFrame();
        }

        private Sprite HarvestEnergyBar()
        {
            return harvestEnergyBar != null ? harvestEnergyBar : LabelFrame();
        }

        private Sprite HarvestButtonRound()
        {
            return harvestButtonRound != null ? harvestButtonRound : (v3ButtonLargeGold != null ? v3ButtonLargeGold : primaryButton);
        }

        private Sprite HarvestInfoPlaque()
        {
            return harvestInfoPlaque != null ? harvestInfoPlaque : ScrollFrame();
        }

        private Sprite HarvestBackPlaque()
        {
            return harvestBackPlaque != null ? harvestBackPlaque : SoftFrame();
        }

        private Sprite TownTitlePlaque()
        {
            return townTitlePlaque != null ? townTitlePlaque : v3StatusBarGreen;
        }

        private Sprite TownPlotFrame()
        {
            return townPlotFrame != null ? townPlotFrame : LabelFrame();
        }

        private Sprite TownBuildingBase()
        {
            return townBuildingBase != null ? townBuildingBase : null;
        }

        private Sprite TownNameScroll()
        {
            return townNameScroll != null ? townNameScroll : LabelFrame();
        }

        private Sprite TownBottomInfoFrame()
        {
            return townBottomInfoFrame != null ? townBottomInfoFrame : OrnateFrame();
        }

        private Sprite TownAttentionBadge()
        {
            return v6NotificationBadge != null ? v6NotificationBadge : (townAttentionBadge != null ? townAttentionBadge : (v3BadgeRed != null ? v3BadgeRed : completeBubbleIcon));
        }

        private Sprite InventoryPanelFrame()
        {
            return inventoryPanelFrame != null ? inventoryPanelFrame : OrnateFrame();
        }

        private Sprite InventoryItemSlot()
        {
            return v6ItemSlotFrame != null ? v6ItemSlotFrame : (inventoryItemSlot != null ? inventoryItemSlot : ItemFrame());
        }

        private Sprite InventoryTabActive()
        {
            return inventoryTabActive != null ? inventoryTabActive : v3TabRight;
        }

        private Sprite InventoryTabInactive()
        {
            return inventoryTabInactive != null ? inventoryTabInactive : v3TabLeft;
        }

        private Sprite InventoryCountBadge()
        {
            return inventoryCountBadge != null ? inventoryCountBadge : LabelFrame();
        }

        private Sprite InventoryEmptyBasket()
        {
            return inventoryEmptyBasket != null ? inventoryEmptyBasket : bagIcon;
        }

        private Sprite TaskOrderBoardFrame()
        {
            return ResourceGeneratedUiV13("task_panel") ?? taskOrderBoardFrame ?? OrnateFrame();
        }

        private Sprite TaskCommissionEnvelope()
        {
            return taskCommissionEnvelope != null ? taskCommissionEnvelope : ScrollFrame();
        }

        private Sprite TaskDailyRouteScroll()
        {
            return taskDailyRouteScroll != null ? taskDailyRouteScroll : eventScroll;
        }

        private Sprite TaskQuestRowFrame()
        {
            return taskQuestRowFrame != null ? taskQuestRowFrame : ScrollFrame();
        }

        private Sprite TaskMilestoneBadge()
        {
            return taskMilestoneBadge != null ? taskMilestoneBadge : (milestoneMedal != null ? milestoneMedal : v3BadgeRed);
        }

        private Sprite TaskCollectionBook()
        {
            return taskCollectionBook != null ? taskCollectionBook : summaryLedger;
        }

        private Sprite AuthLoginCard()
        {
            return ResourceGeneratedUiV13("login_frame") ?? authLoginCard ?? ScrollFrame();
        }

        private Sprite AuthInputFrame()
        {
            return null;
        }

        private Sprite SettingsDialogFrame()
        {
            return ResourceGeneratedUiV13("task_panel") ?? settingsDialogFrame ?? OrnateFrame();
        }

        private Sprite SeedChoiceFrame()
        {
            return ResourceGeneratedUiV13("seed_sheet") ?? ScrollFrame();
        }

        private Sprite SettingsSliderArt()
        {
            return settingsSliderArt != null ? settingsSliderArt : LabelFrame();
        }

        private Sprite SettingsToggleArt()
        {
            return settingsToggleArt != null ? settingsToggleArt : primaryButton;
        }

        private Sprite AgreementScrollFrame()
        {
            return agreementScrollFrame != null ? agreementScrollFrame : OrnateFrame();
        }

        private Button IconButton(Transform parent, string name, Sprite icon, Vector2 pos, Vector2 size, UnityEngine.Events.UnityAction action)
        {
            var button = ButtonNode(parent, name, "", pos, size, Color.clear, action, null);
            ImageNode(button.transform, "Icon", icon, Vector2.zero, size, Color.white);
            return button;
        }

        private Button ButtonNode(Transform parent, string name, string label, Vector2 pos, Vector2 size, Color color, UnityEngine.Events.UnityAction action, Sprite sprite)
        {
            var button = NewObject<Button>(name, parent);
            var image = button.gameObject.AddComponent<Image>();
            if (sprite == primaryButton && v6PrimaryButton != null) sprite = v6PrimaryButton;
            else if (sprite == primaryButton && v3ButtonLargeGold != null) sprite = v3ButtonLargeGold;
            image.color = color;
            image.sprite = sprite;
            image.raycastTarget = true;
            image.type = sprite != null ? UnityEngine.UI.Image.Type.Sliced : UnityEngine.UI.Image.Type.Simple;
            button.targetGraphic = image;
            button.onClick.AddListener(action);
            AttachPressScale(button);
            var rect = button.GetComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;
            ApplyLayoutOverride(rect);
            if (!string.IsNullOrEmpty(label))
            {
                var fontSize = ButtonFontSize(size);
                var textColor = color.a >= .9f ? AccessibleButtonTextColor(color) : C(255, 248, 222, 255);
                var text = TextNode(rect, "Text", label, Vector2.zero, new Vector2(rect.sizeDelta.x - 10, rect.sizeDelta.y - 4), fontSize, textColor, TextAnchor.MiddleCenter);
                text.fontStyle = FontStyles.Bold;
            }
            return button;
        }

        private static int ButtonFontSize(Vector2 size)
        {
            if (size.y <= 30f) return 14;
            if (size.y <= 36f) return 15;
            if (size.y <= 44f) return 16;
            if (size.y <= 52f) return 17;
            return 18;
        }

        private static Color AccessibleButtonTextColor(Color background)
        {
            var light = C(255, 248, 222, 255);
            var dark = C(24, 38, 27, 255);
            return ContrastRatio(light, background) >= 4.5f ? light : dark;
        }

        private static float ContrastRatio(Color a, Color b)
        {
            var aLum = RelativeLuminance(a);
            var bLum = RelativeLuminance(b);
            var lighter = Mathf.Max(aLum, bLum);
            var darker = Mathf.Min(aLum, bLum);
            return (lighter + .05f) / (darker + .05f);
        }

        private static float RelativeLuminance(Color color)
        {
            return .2126f * LinearChannel(color.r) + .7152f * LinearChannel(color.g) + .0722f * LinearChannel(color.b);
        }

        private static float LinearChannel(float value)
        {
            return value <= .04045f ? value / 12.92f : Mathf.Pow((value + .055f) / 1.055f, 2.4f);
        }

        private void AttachPressScale(Button button)
        {
            var trigger = button.gameObject.AddComponent<EventTrigger>();
            AddTrigger(trigger, EventTriggerType.PointerDown, _ => button.transform.localScale = Vector3.one * .95f);
            AddTrigger(trigger, EventTriggerType.PointerUp, _ => button.transform.localScale = Vector3.one);
            AddTrigger(trigger, EventTriggerType.PointerExit, _ => button.transform.localScale = Vector3.one);
        }

        private void AddTrigger(EventTrigger trigger, EventTriggerType eventType, UnityEngine.Events.UnityAction<BaseEventData> action)
        {
            var entry = new EventTrigger.Entry { eventID = eventType };
            entry.callback.AddListener(action);
            trigger.triggers.Add(entry);
        }

        private RectTransform Panel(Transform parent, string name, Color color, Vector2 pos, Vector2 size, Sprite sprite)
        {
            var image = NewObject<Image>(name, parent);
            image.color = color;
            image.sprite = sprite;
            image.raycastTarget = false;
            if (name == "Mask") image.raycastTarget = true;
            image.type = sprite != null && !UsesFullPanelArt(name) ? UnityEngine.UI.Image.Type.Sliced : UnityEngine.UI.Image.Type.Simple;
            var rect = image.GetComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;
            ApplyLayoutOverride(rect);
            return rect;
        }

        private static bool UsesFullPanelArt(string name)
        {
            return name == "LoginCard" || name == "SettingsDialog" || name == "TaskPanel" || name == "SeedChoicePanel" || name == "CollectionDialog";
        }

        private void AddReadableBorder(RectTransform parent, Vector2 size, float thickness = 3f, Color? color = null)
        {
            var lineColor = color ?? C(136, 93, 43, 210);
            Panel(parent, "BorderTop", lineColor, new Vector2(0, size.y * .5f - thickness * .5f), new Vector2(size.x, thickness), null);
            Panel(parent, "BorderBottom", lineColor, new Vector2(0, -size.y * .5f + thickness * .5f), new Vector2(size.x, thickness), null);
            Panel(parent, "BorderLeft", lineColor, new Vector2(-size.x * .5f + thickness * .5f, 0), new Vector2(thickness, size.y), null);
            Panel(parent, "BorderRight", lineColor, new Vector2(size.x * .5f - thickness * .5f, 0), new Vector2(thickness, size.y), null);
        }

        private RectTransform ImageNode(Transform parent, string name, Sprite sprite, Vector2 pos, Vector2 size, Color color, bool cover = false)
        {
            var image = NewObject<Image>(name, parent);
            image.sprite = sprite;
            image.color = color;
            image.raycastTarget = false;
            image.preserveAspect = !cover;
            var rect = image.GetComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;
            ApplyLayoutOverride(rect);
            return rect;
        }

        private TextMeshProUGUI TextNode(Transform parent, string name, string text, Vector2 pos, Vector2 size, int fontSize, Color color, TextAnchor anchor)
        {
            var label = NewObject<TextMeshProUGUI>(name, parent);
            label.text = text;
            label.richText = true;
            label.alignment = ToTmpAlignment(anchor);
            var effectiveFontSize = fontSize <= 14 ? fontSize + 2 : fontSize;
            label.fontSize = effectiveFontSize;
            label.color = color;
            label.raycastTarget = false;
            label.enableWordWrapping = true;
            label.overflowMode = TextOverflowModes.Ellipsis;
            label.enableAutoSizing = false;
            label.fontSizeMax = effectiveFontSize;
            label.fontSizeMin = effectiveFontSize;
            label.outlineColor = IsLightText(color) ? C(18, 28, 20, 190) : Color.clear;
            label.outlineWidth = IsLightText(color) ? .025f : 0f;
            label.extraPadding = true;
            label.margin = new Vector4(4, 2, 4, 2);
            if (effectiveFontSize >= 14)
            {
                label.fontStyle = FontStyles.Bold;
            }
            var rect = label.GetComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;
            ApplyLayoutOverride(rect);
            return label;
        }

        private void LoadLayoutOverrides()
        {
            layoutOverrides.Clear();
            var asset = Resources.Load<TextAsset>("WheatTown/layout-overrides");
            if (asset == null || string.IsNullOrEmpty(asset.text))
            {
                return;
            }

            try
            {
                var file = JsonUtility.FromJson<LayoutOverrideFile>(asset.text);
                if (file == null || file.items == null) return;
                foreach (var item in file.items)
                {
                    if (item == null || string.IsNullOrEmpty(item.key)) continue;
                    layoutOverrides[item.key] = item;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[WheatTown] Failed to parse layout-overrides.json: " + ex.Message);
            }
        }

        private void ApplyLayoutOverride(RectTransform rect)
        {
            if (rect == null || layoutOverrides.Count == 0) return;
            var key = RuntimePath(rect);
            if (!layoutOverrides.TryGetValue(key, out var item)) return;
            rect.anchoredPosition = new Vector2(item.x, item.y);
            rect.sizeDelta = new Vector2(item.w, item.h);
        }

        private static string RuntimePath(Transform transform)
        {
            var names = new List<string>();
            var current = transform;
            while (current != null)
            {
                names.Add(current.name);
                if (current.name == "NativeCanvas") break;
                current = current.parent;
            }
            names.Reverse();
            return string.Join("/", names);
        }

        private static bool IsLightText(Color color)
        {
            return color.r + color.g + color.b > 2.1f;
        }

        private static TextAlignmentOptions ToTmpAlignment(TextAnchor anchor)
        {
            switch (anchor)
            {
                case TextAnchor.UpperLeft: return TextAlignmentOptions.TopLeft;
                case TextAnchor.UpperCenter: return TextAlignmentOptions.Top;
                case TextAnchor.UpperRight: return TextAlignmentOptions.TopRight;
                case TextAnchor.MiddleLeft: return TextAlignmentOptions.MidlineLeft;
                case TextAnchor.MiddleRight: return TextAlignmentOptions.MidlineRight;
                case TextAnchor.LowerLeft: return TextAlignmentOptions.BottomLeft;
                case TextAnchor.LowerCenter: return TextAlignmentOptions.Bottom;
                case TextAnchor.LowerRight: return TextAlignmentOptions.BottomRight;
                default: return TextAlignmentOptions.Center;
            }
        }

        private static T NewObject<T>(string name, Transform parent) where T : Component
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(T));
            go.transform.SetParent(parent, false);
            return go.GetComponent<T>();
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;
        }

        private static void Anchor(RectTransform rect, float minX, float minY, float maxX, float maxY, float left, float bottom, float right, float top)
        {
            rect.anchorMin = new Vector2(minX, minY);
            rect.anchorMax = new Vector2(maxX, maxY);
            rect.offsetMin = new Vector2(left, bottom);
            rect.offsetMax = new Vector2(-right, top);
        }

        private static void Clear(Transform parent)
        {
            var toDelete = new List<GameObject>();
            foreach (Transform child in parent)
            {
                child.gameObject.SetActive(false);
                toDelete.Add(child.gameObject);
            }
            foreach (var item in toDelete)
            {
                if (Application.isPlaying) Destroy(item);
                else DestroyImmediate(item);
            }
        }

        private static void ClearNonBackground(Transform parent)
        {
            var keep = new HashSet<string> { "Background", "ReadabilityWash", "WarmReadabilityWash" };
            var toDelete = new List<GameObject>();
            foreach (Transform child in parent)
            {
                if (!keep.Contains(child.name))
                {
                    child.gameObject.SetActive(false);
                    toDelete.Add(child.gameObject);
                }
            }
            foreach (var item in toDelete)
            {
                if (Application.isPlaying) Destroy(item);
                else DestroyImmediate(item);
            }
        }

        private static Color C(byte r, byte g, byte b, byte a)
        {
            return new Color32(r, g, b, a);
        }

        private static void EnsureEventSystem()
        {
            var eventObject = EventSystem.current != null
                ? EventSystem.current.gameObject
                : new GameObject("EventSystem", typeof(EventSystem));

            var inputSystemModuleType = Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (inputSystemModuleType != null)
            {
                if (eventObject.GetComponent(inputSystemModuleType) == null)
                    eventObject.AddComponent(inputSystemModuleType);

                var oldModule = eventObject.GetComponent<StandaloneInputModule>();
                if (oldModule != null)
                {
                    if (Application.isPlaying) Destroy(oldModule);
                    else DestroyImmediate(oldModule);
                }
                return;
            }

            if (eventObject.GetComponent<StandaloneInputModule>() == null)
                eventObject.AddComponent<StandaloneInputModule>();
        }
    }
}
