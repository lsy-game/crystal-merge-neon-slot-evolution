using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WheatTown.EditorTools
{
    public static class WheatTownUiPreviewBuilder
    {
        private const string PreviewScenePath = "Assets/Scenes/麦穗小镇_UI布局预览.unity";
        private const string LayoutOverridePath = "Assets/Resources/WheatTown/layout-overrides.json";
        private static readonly Vector2 PageSize = new Vector2(430, 932);

        [Serializable]
        private sealed class LayoutOverrideFile
        {
            public List<LayoutOverrideItem> items = new List<LayoutOverrideItem>();
        }

        [Serializable]
        private sealed class LayoutOverrideItem
        {
            public string key;
            public float x;
            public float y;
            public float w;
            public float h;
        }

        [MenuItem("WheatTown/创建中文UI布局预览场景")]
        public static void CreateChineseUiPreviewScene()
        {
            EnsureTmpResources();

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var cameraObject = new GameObject("预览相机_MainCamera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            cameraObject.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
            cameraObject.GetComponent<Camera>().backgroundColor = new Color(0.08f, 0.22f, 0.17f, 1f);

            var eventSystem = new GameObject("事件系统_EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            eventSystem.transform.SetAsLastSibling();

            var canvasObject = new GameObject("【UI布局预览_可手动拖拽】", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = PageSize;
            scaler.matchWidthOrHeight = 1f;

            var guide = Text(canvasObject.transform, "说明_先选中页面根节点再手动拖拽", "中文 UI 布局预览：每个页面都是静态可编辑对象；农田/按钮/弹窗均可直接拖拽。运行场景仍使用 WheatTownBootstrap。", new Vector2(0, 500), new Vector2(980, 36), 18, new Color(1f, .96f, .82f, 1f), TextAlignmentOptions.Center);
            guide.raycastTarget = false;

            CreateLoginPreview(canvasObject.transform, new Vector2(-1350, 0));
            CreateTownPreview(canvasObject.transform, new Vector2(-810, 0));
            CreateHarvestPreview(canvasObject.transform, new Vector2(-270, 0));
            CreateBagPreview(canvasObject.transform, new Vector2(270, 0));
            CreateTaskPreview(canvasObject.transform, new Vector2(810, 0));
            CreateSettingsPreview(canvasObject.transform, new Vector2(1350, 0));
            CreateSeedPreview(canvasObject.transform, new Vector2(1890, 0));

            Directory.CreateDirectory(Path.GetDirectoryName(PreviewScenePath));
            EditorSceneManager.SaveScene(scene, PreviewScenePath);
            EditorSceneManager.OpenScene(PreviewScenePath);
            Selection.activeObject = canvasObject;
            Debug.Log("[WheatTown] 中文 UI 布局预览场景已生成: " + PreviewScenePath);
        }

        [MenuItem("WheatTown/从中文预览同步布局到游戏")]
        public static void SyncChinesePreviewLayoutToGame()
        {
            if (!File.Exists(PreviewScenePath))
            {
                Debug.LogError("[WheatTown] 找不到中文 UI 预览场景，请先执行 WheatTown/创建中文UI布局预览场景");
                return;
            }

            var active = EditorSceneManager.GetActiveScene();
            if (active.path != PreviewScenePath)
            {
                EditorSceneManager.OpenScene(PreviewScenePath, OpenSceneMode.Single);
            }

            var rootObject = GameObject.Find("【UI布局预览_可手动拖拽】");
            if (rootObject == null)
            {
                Debug.LogError("[WheatTown] 预览场景里找不到根节点：【UI布局预览_可手动拖拽】");
                return;
            }

            var root = rootObject.transform;
            NormalizePreviewVisuals(root);
            var file = new LayoutOverrideFile();

            AddTownMappings(file, root);
            AddLoginMappings(file, root);
            AddHarvestMappings(file, root);
            AddBagMappings(file, root);
            AddTaskMappings(file, root);
            AddSettingsMappings(file, root);
            AddSeedMappings(file, root);

            Directory.CreateDirectory(Path.GetDirectoryName(LayoutOverridePath));
            File.WriteAllText(LayoutOverridePath, JsonUtility.ToJson(file, true));
            AssetDatabase.ImportAsset(LayoutOverridePath, ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();
            Debug.Log("[WheatTown] 已同步中文预览布局到游戏: " + LayoutOverridePath + "，共 " + file.items.Count + " 项。运行游戏会自动读取。");
        }

        private static void NormalizePreviewVisuals(Transform root)
        {
            foreach (var image in root.GetComponentsInChildren<Image>(true))
            {
                if (image.name == "背景图_可替换")
                {
                    image.color = Color.white;
                    EditorUtility.SetDirty(image);
                }
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        private static void AddTownMappings(LayoutOverrideFile file, Transform root)
        {
            const string page = "02_小镇主界面_可编辑";
            Add(file, root, page + "/小镇标题条", "NativeCanvas/TownPage/TownTitlePlate");
            Add(file, root, page + "/小镇提示条", "NativeCanvas/TownPage/TownHintPlate");
            for (var i = 0; i < 6; i++)
            {
                Add(file, root, page + "/农田格_" + (i + 1) + "_可替换图片", "NativeCanvas/TownPage/Plot" + i);
            }
            Add(file, root, page + "/可点击区_Bakery", "NativeCanvas/TownPage/BakeryHotspot");
            Add(file, root, page + "/可点击区_Dairy", "NativeCanvas/TownPage/DairyHotspot");
            Add(file, root, page + "/可点击区_Mia", "NativeCanvas/TownPage/MiaHotspot");
            Add(file, root, page + "/可点击区_Tom", "NativeCanvas/TownPage/TomHotspot");
            Add(file, root, page + "/可点击区_Harvest", "NativeCanvas/TownPage/HarvestHotspot");
            Add(file, root, page + "/可点击区_Board", "NativeCanvas/TownPage/BoardHotspot");
            Add(file, root, page + "/目标条_Goal", "NativeCanvas/TownPage/TownObjective");
        }

        private static void AddLoginMappings(LayoutOverrideFile file, Transform root)
        {
            const string page = "01_登录界面_可编辑";
            Add(file, root, page + "/登录卡片_LoginCard", "NativeCanvas/LoginPage/LoginCard");
            Add(file, root, page + "/登录标题_LoginTitle", "NativeCanvas/LoginPage/LoginCard/LoginTitle", page + "/登录卡片_LoginCard");
            Add(file, root, page + "/账号输入框_ID", "NativeCanvas/LoginPage/LoginCard/IDBox", page + "/登录卡片_LoginCard");
            Add(file, root, page + "/密码输入框_Pass", "NativeCanvas/LoginPage/LoginCard/PassBox", page + "/登录卡片_LoginCard");
            Add(file, root, page + "/协议勾选_Agreement", "NativeCanvas/LoginPage/LoginCard/Agreement", page + "/登录卡片_LoginCard");
            Add(file, root, page + "/游客登录按钮_GuestLogin", "NativeCanvas/LoginPage/LoginCard/Guest", page + "/登录卡片_LoginCard");
            Add(file, root, page + "/普通登录按钮_LogIn", "NativeCanvas/LoginPage/LoginCard/Login", page + "/登录卡片_LoginCard");
            Add(file, root, page + "/继续存档按钮_ContinueSave", "NativeCanvas/LoginPage/LoginCard/ContinueSave", page + "/登录卡片_LoginCard");
            Add(file, root, page + "/注销按钮_LogOut", "NativeCanvas/LoginPage/LoginCard/DeleteSave", page + "/登录卡片_LoginCard");
        }

        private static void AddHarvestMappings(LayoutOverrideFile file, Transform root)
        {
            const string page = "03_转盘界面_可编辑";
            Add(file, root, page + "/顶部信息卡_HarvestSummary", "NativeCanvas/Harvest Mini GamePage/HarvestSummary");
            Add(file, root, page + "/转盘主体卡_HarvestMachine", "NativeCanvas/Harvest Mini GamePage/HarvestMachine");
            Add(file, root, page + "/转盘标题条_MachineTitle", "NativeCanvas/Harvest Mini GamePage/HarvestMachine/MachineTitlePlate", page + "/转盘主体卡_HarvestMachine");
            Add(file, root, page + "/三行五列转盘区域_ReelPanel", "NativeCanvas/Harvest Mini GamePage/HarvestMachine/ReelPanel", page + "/转盘主体卡_HarvestMachine");
            Add(file, root, page + "/结果栏_ResultPanel", "NativeCanvas/Harvest Mini GamePage/HarvestMachine/ResultPanel", page + "/转盘主体卡_HarvestMachine");
            Add(file, root, page + "/能量条_EnergyPanel", "NativeCanvas/Harvest Mini GamePage/HarvestMachine/EnergyPanel", page + "/转盘主体卡_HarvestMachine");
            Add(file, root, page + "/操作按钮背景_ActionBar", "NativeCanvas/Harvest Mini GamePage/HarvestMachine/ActionBar", page + "/转盘主体卡_HarvestMachine");
            Add(file, root, page + "/自动按钮_Auto", "NativeCanvas/Harvest Mini GamePage/HarvestMachine/ActionBar/Auto", page + "/操作按钮背景_ActionBar");
            Add(file, root, page + "/旋转按钮_Harvest", "NativeCanvas/Harvest Mini GamePage/HarvestMachine/ActionBar/Harvest", page + "/操作按钮背景_ActionBar");
            Add(file, root, page + "/说明按钮_Info", "NativeCanvas/Harvest Mini GamePage/HarvestMachine/ActionBar/Info", page + "/操作按钮背景_ActionBar");
            Add(file, root, page + "/返回小镇_BackToTown", "NativeCanvas/Harvest Mini GamePage/BackTown");
            for (var i = 0; i < 15; i++)
            {
                Add(file, root, page + "/转盘格_Cell_" + (i + 1), "NativeCanvas/Harvest Mini GamePage/HarvestMachine/ReelPanel/Cell" + i, page + "/三行五列转盘区域_ReelPanel");
            }
        }

        private static void AddBagMappings(LayoutOverrideFile file, Transform root)
        {
            const string page = "04_背包界面_可编辑";
            Add(file, root, page + "/背包顶部栏_BagHeader", "NativeCanvas/BagPage/BagHeader");
            Add(file, root, page + "/背包网格_BagGrid", "NativeCanvas/BagPage/BagGrid");
            Add(file, root, page + "/背包说明_BagHint", "NativeCanvas/BagPage/BagHintPlate");
            for (var i = 0; i < 12; i++)
            {
                Add(file, root, page + "/背包物品格_" + (i + 1), "NativeCanvas/BagPage/BagGrid/BagCell" + i, page + "/背包网格_BagGrid");
            }
        }

        private static void AddTaskMappings(LayoutOverrideFile file, Transform root)
        {
            const string page = "05_任务界面_可编辑";
            Add(file, root, page + "/任务顶部栏_TaskHeader", "NativeCanvas/TasksPage/TaskHeader");
            Add(file, root, page + "/任务主卡_TaskPanel", "NativeCanvas/TasksPage/TaskPanel");
            Add(file, root, page + "/订单行_1", "NativeCanvas/TasksPage/TaskPanel/Bakery Order", page + "/任务主卡_TaskPanel");
            Add(file, root, page + "/订单行_2", "NativeCanvas/TasksPage/TaskPanel/Dairy Order", page + "/任务主卡_TaskPanel");
            Add(file, root, page + "/图鉴按钮_Album", "NativeCanvas/TasksPage/TaskPanel/Collection", page + "/任务主卡_TaskPanel");
            Add(file, root, page + "/返回小镇按钮_BackToTown", "NativeCanvas/TasksPage/BackTaskTown");
        }

        private static void AddSeedMappings(LayoutOverrideFile file, Transform root)
        {
            const string page = "07_种子选择弹层_可编辑";
            Add(file, root, page + "/种子弹层_SeedChoicePanel", "NativeCanvas/ModalLayer/SeedChoicePanel");
            Add(file, root, page + "/标题_Title", "NativeCanvas/ModalLayer/SeedChoicePanel/Title", page + "/种子弹层_SeedChoicePanel");
            Add(file, root, page + "/田地提示_PlotHint", "NativeCanvas/ModalLayer/SeedChoicePanel/PlotHint", page + "/种子弹层_SeedChoicePanel");
            Add(file, root, page + "/小麦种子卡_WheatSeedCard", "NativeCanvas/ModalLayer/SeedChoicePanel/WheatSeedCard", page + "/种子弹层_SeedChoicePanel");
            Add(file, root, page + "/苹果种子卡_AppleSeedLocked", "NativeCanvas/ModalLayer/SeedChoicePanel/AppleSeedLocked", page + "/种子弹层_SeedChoicePanel");
            Add(file, root, page + "/种植按钮_Plant", "NativeCanvas/ModalLayer/SeedChoicePanel/Plant", page + "/种子弹层_SeedChoicePanel");
            Add(file, root, page + "/关闭按钮_Close", "NativeCanvas/ModalLayer/SeedChoicePanel/Close", page + "/种子弹层_SeedChoicePanel");
        }

        private static void AddSettingsMappings(LayoutOverrideFile file, Transform root)
        {
            const string page = "06_设置弹窗_可编辑";
            Add(file, root, page + "/设置弹窗_SettingsDialog", "NativeCanvas/ModalLayer/SettingsDialog");
            Add(file, root, page + "/设置标题_SettingsTitle", "NativeCanvas/ModalLayer/SettingsDialog/Title", page + "/设置弹窗_SettingsDialog");
            Add(file, root, page + "/音乐开关_Music", "NativeCanvas/ModalLayer/SettingsDialog/MusicRow", page + "/设置弹窗_SettingsDialog");
            Add(file, root, page + "/音效开关_SFX", "NativeCanvas/ModalLayer/SettingsDialog/SFXRow", page + "/设置弹窗_SettingsDialog");
            Add(file, root, page + "/音量调整_Volume", "NativeCanvas/ModalLayer/SettingsDialog/VolumeRow", page + "/设置弹窗_SettingsDialog");
            Add(file, root, page + "/隐私按钮_Privacy", "NativeCanvas/ModalLayer/SettingsDialog/Privacy", page + "/设置弹窗_SettingsDialog");
            Add(file, root, page + "/条款按钮_Terms", "NativeCanvas/ModalLayer/SettingsDialog/Terms", page + "/设置弹窗_SettingsDialog");
            Add(file, root, page + "/保存注销_SaveLogout", "NativeCanvas/ModalLayer/SettingsDialog/Logout", page + "/设置弹窗_SettingsDialog");
            Add(file, root, page + "/删除存档_DeleteSave", "NativeCanvas/ModalLayer/SettingsDialog/DeleteSave", page + "/设置弹窗_SettingsDialog");
            Add(file, root, page + "/关闭_Close", "NativeCanvas/ModalLayer/SettingsDialog/CloseTop", page + "/设置弹窗_SettingsDialog");
        }

        private static void Add(LayoutOverrideFile file, Transform root, string previewPath, string runtimePath, string previewRuntimeParentPath = null)
        {
            var source = root.Find(previewPath) as RectTransform;
            if (source == null)
            {
                Debug.LogWarning("[WheatTown] 预览对象不存在，跳过: " + previewPath);
                return;
            }

            var pos = source.anchoredPosition;
            if (!string.IsNullOrEmpty(previewRuntimeParentPath))
            {
                var parent = root.Find(previewRuntimeParentPath) as RectTransform;
                if (parent != null)
                {
                    pos -= parent.anchoredPosition;
                }
            }

            file.items.Add(new LayoutOverrideItem
            {
                key = runtimePath,
                x = Mathf.Round(pos.x * 100f) / 100f,
                y = Mathf.Round(pos.y * 100f) / 100f,
                w = Mathf.Round(source.sizeDelta.x * 100f) / 100f,
                h = Mathf.Round(source.sizeDelta.y * 100f) / 100f
            });
        }

        private static void CreateLoginPreview(Transform root, Vector2 offset)
        {
            var page = Page(root, "01_登录界面_可编辑", offset, "Wheat Town");
            Card(page, "登录卡片_LoginCard", new Vector2(0, -60), new Vector2(372, 612));
            Text(page, "登录标题_LoginTitle", "Guest Sign In", new Vector2(0, 180), new Vector2(290, 42), 28, new Color(1f, .97f, .87f, 1f), TextAlignmentOptions.Center);
            Field(page, "账号输入框_ID", new Vector2(0, 42), "ID");
            Field(page, "密码输入框_Pass", new Vector2(0, -22), "Pass");
            Toggle(page, "协议勾选_Agreement", new Vector2(0, -86), "Agree to Privacy and Terms");
            Button(page, "游客登录按钮_GuestLogin", new Vector2(0, -154), new Vector2(276, 58), "Guest Login", true);
            Button(page, "普通登录按钮_LogIn", new Vector2(0, -218), new Vector2(190, 46), "Log in", false);
            Button(page, "继续存档按钮_ContinueSave", new Vector2(-82, -280), new Vector2(150, 44), "Continue Save", false);
            Button(page, "注销按钮_LogOut", new Vector2(94, -280), new Vector2(126, 44), "Log out", false);
        }

        private static void CreateTownPreview(Transform root, Vector2 offset)
        {
            var page = Page(root, "02_小镇主界面_可编辑", offset, "Town");
            Header(page, "小镇标题条", new Vector2(-132, 300), new Vector2(116, 40), "Town");
            Header(page, "小镇提示条", new Vector2(76, 300), new Vector2(194, 30), "Tap town cards");
            var fieldSprites = new[]
            {
                LoadSprite("Assets/WheatTown/Art/Images/field-baked-v11/plot_0_wheat.png"),
                LoadSprite("Assets/WheatTown/Art/Images/field-baked-v11/plot_1_seeded.png"),
                LoadSprite("Assets/WheatTown/Art/Images/field-baked-v11/plot_2_young_wheat.png"),
                LoadSprite("Assets/WheatTown/Art/Images/field-baked-v11/plot_3_empty.png"),
                LoadSprite("Assets/WheatTown/Art/Images/field-baked-v11/plot_4_empty.png"),
                LoadSprite("Assets/WheatTown/Art/Images/field-baked-v11/plot_5_empty.png")
            };
            for (var i = 0; i < 6; i++)
            {
                var col = i % 2;
                var row = i / 2;
                FieldPlot(page, "农田格_" + (i + 1) + "_可替换图片", new Vector2(-126 + col * 82, 218 - row * 74), fieldSprites[i], i >= 3 ? "Lv." + (i + 1) : "");
            }
            Hotspot(page, "可点击区_Bakery", new Vector2(126, 194), new Vector2(126, 124), "Bakery");
            Hotspot(page, "可点击区_Dairy", new Vector2(126, 64), new Vector2(122, 116), "Dairy");
            Hotspot(page, "可点击区_Mia", new Vector2(-112, -24), new Vector2(116, 108), "Mia");
            Hotspot(page, "可点击区_Tom", new Vector2(4, -24), new Vector2(116, 108), "Tom");
            Hotspot(page, "可点击区_Harvest", new Vector2(126, -122), new Vector2(126, 116), "Harvest");
            Hotspot(page, "可点击区_Board", new Vector2(-128, -170), new Vector2(112, 108), "Board");
            Header(page, "目标条_Goal", new Vector2(0, -292), new Vector2(326, 36), "Goal: Plant -> Craft -> Order");
            BottomNav(page);
        }

        private static void CreateHarvestPreview(Transform root, Vector2 offset)
        {
            var page = Page(root, "03_转盘界面_可编辑", offset, "Harvest");
            Card(page, "顶部信息卡_HarvestSummary", new Vector2(0, 304), new Vector2(382, 82));
            Text(page, "标题_Harvest", "Harvest", new Vector2(-98, 316), new Vector2(150, 34), 26, new Color(.13f, .23f, .16f, 1f), TextAlignmentOptions.MidlineLeft);
            Text(page, "说明_玩法一句话", "Spin the 3 x 5 table. Clear symbols first, then rewards.", new Vector2(-2, 282), new Vector2(344, 26), 15, new Color(.29f, .26f, .20f, 1f), TextAlignmentOptions.Center);
            Card(page, "进度小卡_0_400", new Vector2(134, 318), new Vector2(116, 32));
            Text(page, "进度数字_0_400", "0 / 400", new Vector2(134, 318), new Vector2(102, 24), 17, new Color(.15f, .33f, .23f, 1f), TextAlignmentOptions.Center);

            Card(page, "转盘主体卡_HarvestMachine", new Vector2(0, 18), new Vector2(376, 548));
            Header(page, "转盘标题条_MachineTitle", new Vector2(0, 228), new Vector2(322, 48), "Harvest Table");
            Card(page, "三行五列转盘区域_ReelPanel", new Vector2(0, 78), new Vector2(344, 282));
            for (var i = 0; i < 15; i++)
            {
                var col = i % 5;
                var row = i / 5;
                Slot(page, "转盘格_Cell_" + (i + 1), new Vector2(-136 + col * 68, 160 - row * 82), new Vector2(62, 72), "Icon");
            }
            Card(page, "结果栏_ResultPanel", new Vector2(0, -92), new Vector2(322, 48));
            Text(page, "结果文字_Result", "Tap Harvest to spin", new Vector2(0, -92), new Vector2(296, 28), 17, new Color(.22f, .24f, .18f, 1f), TextAlignmentOptions.Center);
            Card(page, "能量条_EnergyPanel", new Vector2(0, -146), new Vector2(322, 42));
            Text(page, "能量文字_HelpEnergy", "Help Energy 0 / 6", new Vector2(-62, -146), new Vector2(170, 26), 16, new Color(.22f, .24f, .18f, 1f), TextAlignmentOptions.MidlineLeft);
            Header(page, "操作按钮背景_ActionBar", new Vector2(0, -212), new Vector2(338, 80), "");
            Button(page, "自动按钮_Auto", new Vector2(-114, -212), new Vector2(86, 54), "Auto", false);
            Button(page, "旋转按钮_Harvest", new Vector2(0, -212), new Vector2(118, 66), "Harvest", true);
            Button(page, "说明按钮_Info", new Vector2(114, -212), new Vector2(86, 54), "Info", false);
            Button(page, "返回小镇_BackToTown", new Vector2(0, -336), new Vector2(224, 52), "Back to Town", false);
            Header(page, "预览备注_转盘页隐藏底部导航", new Vector2(0, -396), new Vector2(300, 28), "Harvest page hides bottom nav");
        }

        private static void CreateBagPreview(Transform root, Vector2 offset)
        {
            var page = Page(root, "04_背包界面_可编辑", offset, "Bag");
            Header(page, "背包顶部栏_BagHeader", new Vector2(0, 300), new Vector2(370, 48), "Bag   All  Crops  Goods  Mats  Album");
            Card(page, "背包网格_BagGrid", new Vector2(0, 24), new Vector2(344, 492));
            for (var i = 0; i < 12; i++)
            {
                var col = i % 2;
                var row = i / 2;
                Slot(page, "背包物品格_" + (i + 1), new Vector2(-78 + col * 156, 192 - row * 70), new Vector2(138, 62), i == 11 ? "Empty" : "Item x0");
            }
            Header(page, "背包说明_BagHint", new Vector2(0, -270), new Vector2(320, 30), "All items for farming and orders");
            BottomNav(page);
        }

        private static void CreateTaskPreview(Transform root, Vector2 offset)
        {
            var page = Page(root, "05_任务界面_可编辑", offset, "Tasks");
            Header(page, "任务顶部栏_TaskHeader", new Vector2(0, 292), new Vector2(386, 60), "Tasks     Orders     Friends     Route");
            Card(page, "任务主卡_TaskPanel", new Vector2(0, 22), new Vector2(366, 500));
            Text(page, "任务标题_TodayOrders", "Today Orders", new Vector2(0, 232), new Vector2(250, 38), 24, new Color(1f, .97f, .87f, 1f), TextAlignmentOptions.Center);
            Slot(page, "订单行_1", new Vector2(0, 134), new Vector2(320, 80), "Bakery Order      Wait");
            Slot(page, "订单行_2", new Vector2(0, 42), new Vector2(320, 80), "Dairy Order        Wait");
            Text(page, "任务说明_OrderHint", "Orders give coins and materials.", new Vector2(0, -48), new Vector2(304, 30), 17, new Color(.27f, .25f, .20f, 1f), TextAlignmentOptions.Center);
            Button(page, "图鉴按钮_Album", new Vector2(0, -120), new Vector2(210, 52), "Open Album", true);
            Button(page, "返回小镇按钮_BackToTown", new Vector2(0, -300), new Vector2(230, 50), "Back to Town", false);
            BottomNav(page);
        }

        private static void CreateSettingsPreview(Transform root, Vector2 offset)
        {
            var page = Page(root, "06_设置弹窗_可编辑", offset, "Settings");
            Card(page, "设置弹窗_SettingsDialog", new Vector2(0, 0), new Vector2(398, 630));
            Text(page, "设置标题_SettingsTitle", "Settings", new Vector2(0, 250), new Vector2(220, 38), 28, new Color(1f, .97f, .87f, 1f), TextAlignmentOptions.Center);
            Slot(page, "音乐开关_Music", new Vector2(0, 140), new Vector2(326, 50), "Music                         On");
            Slot(page, "音效开关_SFX", new Vector2(0, 76), new Vector2(326, 50), "SFX                           On");
            Slot(page, "音量调整_Volume", new Vector2(0, 10), new Vector2(326, 52), "Volume        -    55%    +");
            Button(page, "隐私按钮_Privacy", new Vector2(-90, -86), new Vector2(150, 46), "Privacy", false);
            Button(page, "条款按钮_Terms", new Vector2(90, -86), new Vector2(150, 46), "Terms", false);
            Button(page, "保存注销_SaveLogout", new Vector2(0, -160), new Vector2(272, 52), "Save & Log out", false);
            Button(page, "删除存档_DeleteSave", new Vector2(0, -230), new Vector2(246, 50), "Delete Save", false);
            Button(page, "关闭_Close", new Vector2(168, 248), new Vector2(42, 42), "X", true);
        }

        private static void CreateSeedPreview(Transform root, Vector2 offset)
        {
            var page = Page(root, "07_种子选择弹层_可编辑", offset, "Seed Choice");
            Card(page, "种子弹层_SeedChoicePanel", new Vector2(0, -262), new Vector2(402, 238));
            Text(page, "标题_Title", "Choose Seed", new Vector2(0, -176), new Vector2(230, 34), 23, new Color(1f, .97f, .87f, 1f), TextAlignmentOptions.Center);
            Text(page, "田地提示_PlotHint", "Plot 1", new Vector2(0, -207), new Vector2(200, 24), 15, new Color(.44f, .32f, .16f, 1f), TextAlignmentOptions.Center);
            Slot(page, "小麦种子卡_WheatSeedCard", new Vector2(-88, -265), new Vector2(154, 92), "Wheat\n12s · Free");
            Slot(page, "苹果种子卡_AppleSeedLocked", new Vector2(88, -265), new Vector2(154, 92), "Apple\nUnlock later");
            Button(page, "种植按钮_Plant", new Vector2(0, -349), new Vector2(220, 46), "Plant Wheat", true);
            Button(page, "关闭按钮_Close", new Vector2(174, -175), new Vector2(38, 38), "X", false);
        }

        private static RectTransform Page(Transform root, string name, Vector2 pos, string title)
        {
            var page = CreatePanel(root, name, pos, PageSize, new Color(.07f, .25f, .17f, 1f));
            var bg = Image(page, "背景图_可替换", LoadSprite("Assets/WheatTown/Art/Images/town-v7/town-main-clean-v7.png"), Vector2.zero, PageSize);
            bg.color = Color.white;
            Header(page, "页面名称_" + title, new Vector2(0, 430), new Vector2(380, 36), name);
            return page;
        }

        private static void BottomNav(RectTransform page)
        {
            Header(page, "底部导航_BottomNavigation", new Vector2(0, -390), new Vector2(360, 70), "Town        Bag        Tasks        Settings");
        }

        private static void Field(RectTransform page, string name, Vector2 pos, string label)
        {
            Slot(page, name, pos, new Vector2(294, 52), label + "       Letters or numbers only");
        }

        private static void Toggle(RectTransform page, string name, Vector2 pos, string label)
        {
            Slot(page, name, pos, new Vector2(304, 38), "□  " + label);
        }

        private static void Button(RectTransform page, string name, Vector2 pos, Vector2 size, string label, bool primary)
        {
            var color = primary ? new Color(.87f, .40f, .15f, 1f) : new Color(.18f, .29f, .17f, 1f);
            var button = CreatePanel(page, name, pos, size, color);
            AddBorder(button, size, new Color(.95f, .74f, .36f, .85f));
            Text(button, "文字_Text", label, Vector2.zero, new Vector2(size.x - 10, size.y - 6), size.y >= 50 ? 18 : 15, Color.white, TextAlignmentOptions.Center);
        }

        private static void Header(RectTransform parent, string name, Vector2 pos, Vector2 size, string label)
        {
            var panel = CreatePanel(parent, name, pos, size, new Color(.10f, .28f, .18f, .96f));
            AddBorder(panel, size, new Color(.71f, .54f, .29f, .7f));
            if (!string.IsNullOrEmpty(label))
            {
                Text(panel, "文字_Text", label, Vector2.zero, new Vector2(size.x - 12, size.y - 4), size.y >= 40 ? 16 : 13, new Color(1f, .97f, .87f, 1f), TextAlignmentOptions.Center);
            }
        }

        private static void Card(RectTransform parent, string name, Vector2 pos, Vector2 size)
        {
            string artPath = null;
            if (name.Contains("登录卡片")) artPath = "Assets/Resources/WheatTown/generated-ui-v13/login_frame.png";
            else if (name.Contains("任务主卡") || name.Contains("设置弹窗")) artPath = "Assets/Resources/WheatTown/generated-ui-v13/task_panel.png";
            else if (name.Contains("种子弹层")) artPath = "Assets/Resources/WheatTown/generated-ui-v13/seed_sheet.png";

            var panel = CreatePanel(parent, name, pos, size, artPath == null ? new Color(1f, .98f, .91f, .97f) : new Color(1f, .98f, .90f, 1f));
            if (artPath != null)
            {
                panel.GetComponent<Image>().sprite = LoadSprite(artPath);
                panel.GetComponent<Image>().type = UnityEngine.UI.Image.Type.Simple;
                return;
            }
            AddBorder(panel, size, new Color(.49f, .34f, .16f, .92f));
        }

        private static void Slot(RectTransform parent, string name, Vector2 pos, Vector2 size, string label)
        {
            var panel = CreatePanel(parent, name, pos, size, new Color(1f, .96f, .86f, .98f));
            AddBorder(panel, size, new Color(.71f, .54f, .29f, .82f));
            Text(panel, "文字_Text", label, Vector2.zero, new Vector2(size.x - 10, size.y - 6), size.x > 90 ? 14 : 12, new Color(.18f, .22f, .16f, 1f), TextAlignmentOptions.Center);
        }

        private static void Label(RectTransform parent, string name, Vector2 pos, string label)
        {
            Header(parent, name, pos, new Vector2(108, 30), label);
        }

        private static void Hotspot(RectTransform parent, string name, Vector2 pos, Vector2 size, string label)
        {
            var panel = CreatePanel(parent, name, pos, size, new Color(1f, 1f, 1f, .015f));
            Header(panel, "标签_" + label, new Vector2(0, -size.y * .36f), new Vector2(Mathf.Min(size.x - 10, 114), 36), label);
        }

        private static void FieldPlot(RectTransform parent, string name, Vector2 pos, Sprite sprite, string badge)
        {
            var holder = CreatePanel(parent, name, pos, new Vector2(96, 74), new Color(1f, 1f, 1f, .015f));
            if (sprite != null)
            {
                var image = Image(holder, "整块农田状态图_FieldTile", sprite, Vector2.zero, new Vector2(96, 74));
                image.preserveAspect = false;
            }
            if (string.IsNullOrEmpty(badge)) return;

            var label = CreatePanel(holder, "等级小牌_" + badge, new Vector2(0, -27), new Vector2(54, 22), new Color(.11f, .26f, .17f, .92f));
            Text(label, "文字_Text", badge, Vector2.zero, new Vector2(48, 18), 13, new Color(1f, .97f, .87f, 1f), TextAlignmentOptions.Center);
        }

        private static RectTransform CreatePanel(Transform parent, string name, Vector2 pos, Vector2 size, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;
            var image = go.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return rect;
        }

        private static Image Image(Transform parent, string name, Sprite sprite, Vector2 pos, Vector2 size)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;
            var image = go.GetComponent<Image>();
            image.sprite = sprite;
            image.color = Color.white;
            image.raycastTarget = false;
            image.type = sprite != null ? UnityEngine.UI.Image.Type.Simple : UnityEngine.UI.Image.Type.Simple;
            return image;
        }

        private static TextMeshProUGUI Text(Transform parent, string name, string value, Vector2 pos, Vector2 size, int fontSize, Color color, TextAlignmentOptions alignment)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;
            var text = go.GetComponent<TextMeshProUGUI>();
            text.text = value;
            text.fontSize = fontSize;
            text.fontStyle = FontStyles.Bold;
            text.color = color;
            text.alignment = alignment;
            text.enableAutoSizing = false;
            text.raycastTarget = false;
            text.overflowMode = TextOverflowModes.Ellipsis;
            var light = color.r + color.g + color.b > 2.2f;
            text.outlineColor = light ? new Color(.07f, .11f, .08f, .78f) : Color.clear;
            text.outlineWidth = light ? .025f : 0f;
            return text;
        }

        private static void AddBorder(RectTransform parent, Vector2 size, Color color)
        {
            CreateLine(parent, "边框_上", new Vector2(0, size.y * .5f - 1), new Vector2(size.x, 2), color);
            CreateLine(parent, "边框_下", new Vector2(0, -size.y * .5f + 1), new Vector2(size.x, 2), color);
            CreateLine(parent, "边框_左", new Vector2(-size.x * .5f + 1, 0), new Vector2(2, size.y), color);
            CreateLine(parent, "边框_右", new Vector2(size.x * .5f - 1, 0), new Vector2(2, size.y), color);
        }

        private static void CreateLine(Transform parent, string name, Vector2 pos, Vector2 size, Color color)
        {
            CreatePanel(parent, name, pos, size, color);
        }

        private static Sprite LoadSprite(string path)
        {
            if (File.Exists(path))
            {
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer != null)
                {
                    var changed = false;
                    if (importer.textureType != TextureImporterType.Sprite)
                    {
                        importer.textureType = TextureImporterType.Sprite;
                        importer.spriteImportMode = SpriteImportMode.Single;
                        changed = true;
                    }
                    if (!importer.alphaIsTransparency)
                    {
                        importer.alphaIsTransparency = true;
                        changed = true;
                    }
                    if (importer.mipmapEnabled)
                    {
                        importer.mipmapEnabled = false;
                        changed = true;
                    }
                    if (importer.textureCompression != TextureImporterCompression.Uncompressed)
                    {
                        importer.textureCompression = TextureImporterCompression.Uncompressed;
                        changed = true;
                    }
                    if (changed)
                    {
                        importer.SaveAndReimport();
                    }
                    else
                    {
                        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    }
                }
            }
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static void EnsureTmpResources()
        {
            if (!File.Exists("Assets/TextMesh Pro/Resources/TMP Settings.asset"))
            {
                TMP_PackageUtilities.ImportProjectResourcesMenu();
            }
        }
    }
}
