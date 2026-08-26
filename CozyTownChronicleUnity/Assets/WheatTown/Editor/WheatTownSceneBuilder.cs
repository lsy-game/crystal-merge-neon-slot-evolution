using System.IO;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace WheatTown.EditorTools
{
    public static class WheatTownSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/WheatTownBootstrap.unity";

        [MenuItem("WheatTown/Create Native Game Scene")]
        public static void CreateBootstrapScene()
        {
            EnsureTmpEssentialResources();
            EnsureAllSprites("Assets/WheatTown/Art/Images");

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            cameraObject.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
            cameraObject.GetComponent<Camera>().backgroundColor = new Color(0.08f, 0.22f, 0.17f, 1f);

            CreateCompatibleEventSystem();

            var gameObject = new GameObject("WheatTownNativeGame", typeof(WheatTown.WheatTownNativeGame));
            AssignSprites(gameObject.GetComponent<WheatTown.WheatTownNativeGame>());

            Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
            EditorSceneManager.SaveScene(scene, ScenePath);
            Debug.Log("[WheatTown] Native Unity scene created: " + ScenePath);

            if (!Application.isBatchMode)
            {
                EditorUtility.DisplayDialog("Wheat Town", "Native Unity scene created:\n" + ScenePath + "\n\nRunning now stays inside Unity and opens no web page.", "OK");
            }
        }

        [MenuItem("WheatTown/Validate Native MVP")]
        public static void ValidateNativeMvp()
        {
            var errors = new List<string>();
            EnsureTmpEssentialResources();
            EnsureAllSprites("Assets/WheatTown/Art/Images");

            RequireFile(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs");
            RequireMissing(errors, "Assets/WheatTown/Scripts/WheatTownWebLauncher.cs");
            RequireFile(errors, "Assets/TextMesh Pro/Resources/TMP Settings.asset");
            RequireFile(errors, "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
            RequireFile(errors, "Assets/WheatTown/Art/Images/town-v7/town-main-clean-v7.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/v3-ui/v3_panel_ornate.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/v3-ui/v3_dialog_scroll.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/v3-ui/v3_status_bar_green.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/v3-ui/v3_nav_plaque_green.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/journey-ui/journey-sign.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/journey-ui/order-board.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/journey-ui/event-scroll.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/journey-ui/milestone-medal.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/journey-ui/summary-ledger.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/journey-ui/harvest-chest.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/native-v4/npc_mia_full.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/native-v4/npc_tom_full.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/native-v4/icon_favor_heart.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/native-v4/icon_commission_mark.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/harvest-v4/harvest_console_frame.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/harvest-v4/harvest_cell_tile.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/harvest-v4/harvest_energy_bar.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/harvest-v4/harvest_button_round.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/harvest-v4/harvest_info_plaque.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/harvest-v4/harvest_back_plaque.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/town-v5/town_title_plaque.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/town-v5/town_plot_frame.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/town-v5/town_building_base.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/town-v5/town_name_scroll.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/town-v5/town_bottom_info_frame.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/town-v5/town_attention_badge.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/inventory-v5/inventory_panel_frame.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/inventory-v5/inventory_item_slot.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/inventory-v5/inventory_tab_active.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/inventory-v5/inventory_tab_inactive.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/inventory-v5/inventory_count_badge.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/inventory-v5/inventory_empty_basket.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/task-v5/task_order_board_frame.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/task-v5/task_commission_envelope.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/task-v5/task_daily_route_scroll.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/task-v5/task_quest_row_frame.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/task-v5/task_milestone_badge.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/task-v5/task_collection_book.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/auth-settings-v5/auth_login_card.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/ui-v9/login_card_clean.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/auth-settings-v5/auth_input_frame.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/auth-settings-v5/settings_dialog_frame.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/auth-settings-v5/settings_slider_art.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/auth-settings-v5/settings_toggle_art.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/auth-settings-v5/agreement_scroll_frame.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/ui-polish-v6/v6_primary_button.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/ui-polish-v6/v6_secondary_button.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/ui-polish-v6/v6_item_slot_frame.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/ui-polish-v6/v6_notification_badge.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/ui-polish-v6/v6_progress_bar_frame.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/ui-v8/clean_parchment_panel.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/native-v2/plot_empty.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/field-states/field_empty_clean.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/field-states/field_seedling_clean.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/field-states/field_young_wheat_clean.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/field-states/field_wheat_clean.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/field-states/field_apple_clean.png");
            for (var i = 0; i < 6; i++)
            {
                RequireFile(errors, "Assets/WheatTown/Art/Images/field-baked-v11/plot_" + i + "_empty.png");
                RequireFile(errors, "Assets/WheatTown/Art/Images/field-baked-v11/plot_" + i + "_seeded.png");
                RequireFile(errors, "Assets/WheatTown/Art/Images/field-baked-v11/plot_" + i + "_seedling.png");
                RequireFile(errors, "Assets/WheatTown/Art/Images/field-baked-v11/plot_" + i + "_young_wheat.png");
                RequireFile(errors, "Assets/WheatTown/Art/Images/field-baked-v11/plot_" + i + "_wheat.png");
                RequireFile(errors, "Assets/WheatTown/Art/Images/field-baked-v14/plot_" + i + "_empty.png");
                RequireFile(errors, "Assets/WheatTown/Art/Images/field-baked-v14/plot_" + i + "_seeded.png");
                RequireFile(errors, "Assets/WheatTown/Art/Images/field-baked-v14/plot_" + i + "_seedling.png");
                RequireFile(errors, "Assets/WheatTown/Art/Images/field-baked-v14/plot_" + i + "_young_wheat.png");
                RequireFile(errors, "Assets/WheatTown/Art/Images/field-baked-v14/plot_" + i + "_wheat.png");
                RequireFile(errors, "Assets/Resources/WheatTown/field-baked-v14/plot_" + i + "_empty.png");
                RequireFile(errors, "Assets/Resources/WheatTown/field-baked-v14/plot_" + i + "_seeded.png");
                RequireFile(errors, "Assets/Resources/WheatTown/field-baked-v14/plot_" + i + "_seedling.png");
                RequireFile(errors, "Assets/Resources/WheatTown/field-baked-v14/plot_" + i + "_young_wheat.png");
                RequireFile(errors, "Assets/Resources/WheatTown/field-baked-v14/plot_" + i + "_wheat.png");
            }
            RequireFile(errors, "Assets/WheatTown/Art/Images/native-v2/building_dairy.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/native-v2/house_mia.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/native-v2/house_tom.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/native-v2/machine_harvest.png");
            RequireFile(errors, "Assets/WheatTown/Art/Images/native-v2/board_orders.png");
            RequireFile(errors, "Assets/Resources/WheatTown/generated-ui-v13/login_frame.png");
            RequireFile(errors, "Assets/Resources/WheatTown/generated-ui-v13/task_panel.png");
            RequireFile(errors, "Assets/Resources/WheatTown/generated-ui-v13/seed_sheet.png");
            RequireFile(errors, ScenePath);

            RequireText(errors, "Assets/WheatTown/Editor/WheatTownSceneBuilder.cs", "town-v7/town-main-clean-v7.png");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "Page { Town, Bag, Task }");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "OpenDairy");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "SubmitCheeseOrder");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "ShowAgreement");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "FadeInRoutine");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "LabelPlate");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "RefreshNavState");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "ShowResidentPanel");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "RouteNode");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "CollectionRow");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "ShowSeedChoicePanel");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "ShowProcessPanel");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "NpcMia");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "CommissionMark");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "HarvestConsoleFrame");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "HarvestButtonRound");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "TownTitlePlaque");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "TownBuildingBase");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "InventoryPanelFrame");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "InventoryItemSlot");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "TaskOrderBoardFrame");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "TaskQuestRowFrame");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "AuthLoginCard");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "SettingsDialogFrame");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "V6 Generated Lightweight Polish UI Art");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "V8 Clean Readability UI Art");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "CleanMainPanel");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "TextMeshProUGUI");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "TMP_InputField");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "generated-ui-v13");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "AccessibleButtonTextColor");
            RequireText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "SeedChoiceFrame");
            ForbidText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "LegacyRuntime.ttf");
            ForbidText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "UnityEngine.UI.Text");

            ForbidText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "Application.OpenURL");
            ForbidText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "WebLauncher");
            ForbidText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "SLOT");
            ForbidText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "RTP");
            ForbidText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "下注");
            ForbidText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "中奖");
            ForbidText(errors, "Assets/WheatTown/Scripts/WheatTownNativeGame.cs", "赔付");
            ForbidText(errors, "Assets/Scenes/WheatTownBootstrap.unity", "WheatTownWebLauncher");
            ForbidText(errors, "Assets/Scenes/WheatTownBootstrap.unity", "Application.OpenURL");

            if (errors.Count > 0)
            {
                throw new System.Exception("[WheatTown] Native MVP validation failed:\n- " + string.Join("\n- ", errors));
            }

            Debug.Log("[WheatTown] Native MVP validation passed. Scene, art bindings, native-only entry, labels, settings, production loop, and forbidden terms checked.");
        }

        [MenuItem("WheatTown/Validate Runtime UI Structure")]
        public static void ValidateRuntimeUiStructure()
        {
            EnsureTmpEssentialResources();
            EnsureAllSprites("Assets/WheatTown/Art/Images");

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            CreateCompatibleEventSystem();
            var go = new GameObject("WheatTownNativeGame", typeof(WheatTown.WheatTownNativeGame));
            AssignSprites(go.GetComponent<WheatTown.WheatTownNativeGame>());

            var method = typeof(WheatTown.WheatTownNativeGame).GetMethod("Awake", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            method.Invoke(go.GetComponent<WheatTown.WheatTownNativeGame>(), null);

            var errors = new List<string>();
            RequireObject(errors, go.transform, "NativeCanvas/LoginPage/LoginCard/IDBox/IDInput");
            RequireObject(errors, go.transform, "NativeCanvas/LoginPage/LoginCard/PassBox/PassInput");
            RequireObject(errors, go.transform, "NativeCanvas/LoginPage/LoginCard/Agreement");
            RequireObject(errors, go.transform, "NativeCanvas/LoginPage/LoginCard/Guest");
            RequireObject(errors, go.transform, "NativeCanvas/LoginPage/LoginCard/Login");
            RequireObject(errors, go.transform, "NativeCanvas/TownPage/BakeryHotspot");
            RequireObject(errors, go.transform, "NativeCanvas/TownPage/DairyHotspot");
            RequireObject(errors, go.transform, "NativeCanvas/TownPage/MiaHotspot");
            RequireObject(errors, go.transform, "NativeCanvas/TownPage/TomHotspot");
            RequireObject(errors, go.transform, "NativeCanvas/TownPage/HarvestHotspot");
            RequireObject(errors, go.transform, "NativeCanvas/TownPage/BoardHotspot");
            RequireMissingObject(errors, go.transform, "NativeCanvas/TownPage/MiaNpc");
            RequireMissingObject(errors, go.transform, "NativeCanvas/TownPage/TomNpc");
            for (var i = 0; i < 6; i++)
            {
                RequireObject(errors, go.transform, "NativeCanvas/TownPage/Plot" + i);
                RequireObject(errors, go.transform, "NativeCanvas/TownPage/Plot" + i + "/FieldTile");
                RequireMissingObject(errors, go.transform, "NativeCanvas/TownPage/Plot" + i + "/FieldArt");
            }
            RequireObject(errors, go.transform, "NativeCanvas/HUD/BottomNavigation/Town");
            RequireObject(errors, go.transform, "NativeCanvas/HUD/BottomNavigation/Bag");
            RequireObject(errors, go.transform, "NativeCanvas/HUD/BottomNavigation/Tasks");
            RequireObject(errors, go.transform, "NativeCanvas/HUD/BottomNavigation/Settings");
            RequireButtonRaycast(errors, go.transform, "NativeCanvas/TownPage/BakeryHotspot");
            RequireButtonRaycast(errors, go.transform, "NativeCanvas/TownPage/DairyHotspot");
            RequireButtonRaycast(errors, go.transform, "NativeCanvas/TownPage/MiaHotspot");
            RequireButtonRaycast(errors, go.transform, "NativeCanvas/TownPage/TomHotspot");
            RequireButtonRaycast(errors, go.transform, "NativeCanvas/TownPage/HarvestHotspot");
            RequireButtonRaycast(errors, go.transform, "NativeCanvas/TownPage/BoardHotspot");
            for (var i = 0; i < 6; i++)
            {
                RequireButtonRaycast(errors, go.transform, "NativeCanvas/TownPage/Plot" + i);
                RequireButtonMinSize(errors, go.transform, "NativeCanvas/TownPage/Plot" + i, 48f);
            }
            RequireButtonMinSize(errors, go.transform, "NativeCanvas/TownPage/BakeryHotspot", 48f);
            RequireButtonMinSize(errors, go.transform, "NativeCanvas/TownPage/DairyHotspot", 48f);
            RequireButtonMinSize(errors, go.transform, "NativeCanvas/TownPage/MiaHotspot", 48f);
            RequireButtonMinSize(errors, go.transform, "NativeCanvas/TownPage/TomHotspot", 48f);
            RequireButtonMinSize(errors, go.transform, "NativeCanvas/TownPage/HarvestHotspot", 48f);
            RequireButtonMinSize(errors, go.transform, "NativeCanvas/TownPage/BoardHotspot", 48f);
            RequireButtonRaycast(errors, go.transform, "NativeCanvas/LoginPage/LoginCard/Guest");
            RequireButtonRaycast(errors, go.transform, "NativeCanvas/LoginPage/LoginCard/Login");
            RequireButtonRaycast(errors, go.transform, "NativeCanvas/HUD/BottomNavigation/Town");
            RequireButtonRaycast(errors, go.transform, "NativeCanvas/HUD/BottomNavigation/Bag");
            RequireButtonRaycast(errors, go.transform, "NativeCanvas/HUD/BottomNavigation/Tasks");
            RequireButtonRaycast(errors, go.transform, "NativeCanvas/HUD/BottomNavigation/Settings");
            RequireButtonMinSize(errors, go.transform, "NativeCanvas/HUD/BottomNavigation/Town", 48f);
            RequireButtonMinSize(errors, go.transform, "NativeCanvas/HUD/BottomNavigation/Bag", 48f);
            RequireButtonMinSize(errors, go.transform, "NativeCanvas/HUD/BottomNavigation/Tasks", 48f);
            RequireButtonMinSize(errors, go.transform, "NativeCanvas/HUD/BottomNavigation/Settings", 48f);
            RequireTmpQuality(errors, go);

            if (errors.Count > 0)
            {
                throw new System.Exception("[WheatTown] Runtime UI structure validation failed:\n- " + string.Join("\n- ", errors));
            }

            Debug.Log("[WheatTown] Runtime UI structure validation passed. Login fields, Town hotspots, nav buttons, TMP readability and raycast targets checked.");
        }

        private static void AssignSprites(WheatTown.WheatTownNativeGame game)
        {
            var so = new SerializedObject(game);
            Set(so, "lobbyBackground", "Assets/WheatTown/Art/Images/lobby-background.png");
            Set(so, "slotBackground", "Assets/WheatTown/Art/Images/premium-slot-background.png");
            Set(so, "townBackground", "Assets/WheatTown/Art/Images/town-v7/town-main-clean-v7.png");

            Set(so, "wheatIcon", "Assets/WheatTown/Art/Images/symbols/wheat.png");
            Set(so, "breadIcon", "Assets/WheatTown/Art/Images/symbols/bread.png");
            Set(so, "milkIcon", "Assets/WheatTown/Art/Images/symbols/milk.png");
            Set(so, "appleIcon", "Assets/WheatTown/Art/Images/symbols/apple.png");
            Set(so, "gemIcon", "Assets/WheatTown/Art/Images/symbols/gem.png");
            Set(so, "wildIcon", "Assets/WheatTown/Art/Images/symbols/wild.png");
            Set(so, "giftIcon", "Assets/WheatTown/Art/Images/symbols/gift.png");

            Set(so, "resourcePill", "Assets/WheatTown/Art/Images/premium-ui/resource-pill.png");
            Set(so, "navTab", "Assets/WheatTown/Art/Images/premium-ui/nav-tab.png");
            Set(so, "reelFrame", "Assets/WheatTown/Art/Images/premium-ui/reel-frame.png");
            Set(so, "cardFrame", "Assets/WheatTown/Art/Images/premium-ui/card-frame.png");
            Set(so, "primaryButton", "Assets/WheatTown/Art/Images/premium-polish/primary-button.png");
            Set(so, "titlePlaque", "Assets/WheatTown/Art/Images/premium-ui/title-plaque.png");
            Set(so, "utilityButton", "Assets/WheatTown/Art/Images/premium-ui/utility-button.png");
            Set(so, "settingsIcon", "Assets/WheatTown/Art/Images/premium-icons/settings.png");
            Set(so, "coinIcon", "Assets/WheatTown/Art/Images/premium-icons/coin.png");
            Set(so, "woodIcon", "Assets/WheatTown/Art/Images/premium-icons/wood.png");
            Set(so, "oreIcon", "Assets/WheatTown/Art/Images/premium-icons/ore.png");
            Set(so, "wheatCorner", "Assets/WheatTown/Art/Images/premium-ornaments/wheat-corner.png");
            Set(so, "vineCorner", "Assets/WheatTown/Art/Images/premium-ornaments/vine-corner.png");
            Set(so, "gemRivets", "Assets/WheatTown/Art/Images/premium-ornaments/gem-rivets.png");
            Set(so, "creamGoldLabel", "Assets/WheatTown/Art/Images/premium-ornaments/cream-gold-label.png");
            Set(so, "ribbonCap", "Assets/WheatTown/Art/Images/premium-ornaments/ribbon-cap.png");
            Set(so, "woodDivider", "Assets/WheatTown/Art/Images/premium-ornaments/wood-divider.png");
            Set(so, "infoCard", "Assets/WheatTown/Art/Images/premium-polish/info-card.png");
            Set(so, "dialogueCard", "Assets/WheatTown/Art/Images/premium-polish/dialogue-card.png");
            Set(so, "statusStrip", "Assets/WheatTown/Art/Images/premium-polish/status-strip.png");
            Set(so, "symbolTile", "Assets/WheatTown/Art/Images/premium-polish/symbol-tile.png");

            Set(so, "v3PanelOrnate", "Assets/WheatTown/Art/Images/v3-ui/v3_panel_ornate.png");
            Set(so, "v3DialogScroll", "Assets/WheatTown/Art/Images/v3-ui/v3_dialog_scroll.png");
            Set(so, "v3StatusBarGreen", "Assets/WheatTown/Art/Images/v3-ui/v3_status_bar_green.png");
            Set(so, "v3NavPlaqueGreen", "Assets/WheatTown/Art/Images/v3-ui/v3_nav_plaque_green.png");
            Set(so, "v3ButtonLargeGold", "Assets/WheatTown/Art/Images/v3-ui/v3_button_large_gold.png");
            Set(so, "v3ButtonSmallGold", "Assets/WheatTown/Art/Images/v3-ui/v3_button_small_gold.png");
            Set(so, "v3TabLeft", "Assets/WheatTown/Art/Images/v3-ui/v3_tab_left.png");
            Set(so, "v3TabRight", "Assets/WheatTown/Art/Images/v3-ui/v3_tab_right.png");
            Set(so, "v3CornerWheatSet", "Assets/WheatTown/Art/Images/v3-ui/v3_corner_wheat_set.png");
            Set(so, "v3WoodDivider", "Assets/WheatTown/Art/Images/v3-ui/v3_wood_divider.png");
            Set(so, "v3SettingsMedallion", "Assets/WheatTown/Art/Images/v3-ui/v3_settings_medallion.png");
            Set(so, "v3BadgeRed", "Assets/WheatTown/Art/Images/v3-ui/v3_badge_red.png");

            Set(so, "journeySign", "Assets/WheatTown/Art/Images/journey-ui/journey-sign.png");
            Set(so, "orderBoardUi", "Assets/WheatTown/Art/Images/journey-ui/order-board.png");
            Set(so, "eventScroll", "Assets/WheatTown/Art/Images/journey-ui/event-scroll.png");
            Set(so, "milestoneMedal", "Assets/WheatTown/Art/Images/journey-ui/milestone-medal.png");
            Set(so, "summaryLedger", "Assets/WheatTown/Art/Images/journey-ui/summary-ledger.png");
            Set(so, "harvestChest", "Assets/WheatTown/Art/Images/journey-ui/harvest-chest.png");

            Set(so, "npcMiaFull", "Assets/WheatTown/Art/Images/native-v4/npc_mia_full.png");
            Set(so, "npcTomFull", "Assets/WheatTown/Art/Images/native-v4/npc_tom_full.png");
            Set(so, "favorHeartIcon", "Assets/WheatTown/Art/Images/native-v4/icon_favor_heart.png");
            Set(so, "commissionMarkIcon", "Assets/WheatTown/Art/Images/native-v4/icon_commission_mark.png");

            Set(so, "harvestConsoleFrame", "Assets/WheatTown/Art/Images/harvest-v4/harvest_console_frame.png");
            Set(so, "harvestCellTile", "Assets/WheatTown/Art/Images/harvest-v4/harvest_cell_tile.png");
            Set(so, "harvestEnergyBar", "Assets/WheatTown/Art/Images/harvest-v4/harvest_energy_bar.png");
            Set(so, "harvestButtonRound", "Assets/WheatTown/Art/Images/harvest-v4/harvest_button_round.png");
            Set(so, "harvestInfoPlaque", "Assets/WheatTown/Art/Images/harvest-v4/harvest_info_plaque.png");
            Set(so, "harvestBackPlaque", "Assets/WheatTown/Art/Images/harvest-v4/harvest_back_plaque.png");

            Set(so, "townTitlePlaque", "Assets/WheatTown/Art/Images/town-v5/town_title_plaque.png");
            Set(so, "townPlotFrame", "Assets/WheatTown/Art/Images/town-v5/town_plot_frame.png");
            Set(so, "townBuildingBase", "Assets/WheatTown/Art/Images/town-v5/town_building_base.png");
            Set(so, "townNameScroll", "Assets/WheatTown/Art/Images/town-v5/town_name_scroll.png");
            Set(so, "townBottomInfoFrame", "Assets/WheatTown/Art/Images/town-v5/town_bottom_info_frame.png");
            Set(so, "townAttentionBadge", "Assets/WheatTown/Art/Images/town-v5/town_attention_badge.png");

            Set(so, "inventoryPanelFrame", "Assets/WheatTown/Art/Images/inventory-v5/inventory_panel_frame.png");
            Set(so, "inventoryItemSlot", "Assets/WheatTown/Art/Images/inventory-v5/inventory_item_slot.png");
            Set(so, "inventoryTabActive", "Assets/WheatTown/Art/Images/inventory-v5/inventory_tab_active.png");
            Set(so, "inventoryTabInactive", "Assets/WheatTown/Art/Images/inventory-v5/inventory_tab_inactive.png");
            Set(so, "inventoryCountBadge", "Assets/WheatTown/Art/Images/inventory-v5/inventory_count_badge.png");
            Set(so, "inventoryEmptyBasket", "Assets/WheatTown/Art/Images/inventory-v5/inventory_empty_basket.png");

            Set(so, "taskOrderBoardFrame", "Assets/WheatTown/Art/Images/task-v5/task_order_board_frame.png");
            Set(so, "taskCommissionEnvelope", "Assets/WheatTown/Art/Images/task-v5/task_commission_envelope.png");
            Set(so, "taskDailyRouteScroll", "Assets/WheatTown/Art/Images/task-v5/task_daily_route_scroll.png");
            Set(so, "taskQuestRowFrame", "Assets/WheatTown/Art/Images/task-v5/task_quest_row_frame.png");
            Set(so, "taskMilestoneBadge", "Assets/WheatTown/Art/Images/task-v5/task_milestone_badge.png");
            Set(so, "taskCollectionBook", "Assets/WheatTown/Art/Images/task-v5/task_collection_book.png");

            Set(so, "authLoginCard", "Assets/WheatTown/Art/Images/ui-v9/login_card_clean.png");
            Set(so, "authInputFrame", "Assets/WheatTown/Art/Images/auth-settings-v5/auth_input_frame.png");
            Set(so, "settingsDialogFrame", "Assets/WheatTown/Art/Images/auth-settings-v5/settings_dialog_frame.png");
            Set(so, "settingsSliderArt", "Assets/WheatTown/Art/Images/auth-settings-v5/settings_slider_art.png");
            Set(so, "settingsToggleArt", "Assets/WheatTown/Art/Images/auth-settings-v5/settings_toggle_art.png");
            Set(so, "agreementScrollFrame", "Assets/WheatTown/Art/Images/auth-settings-v5/agreement_scroll_frame.png");

            Set(so, "v6MainPanelFrame", "Assets/WheatTown/Art/Images/ui-polish-v6/v6_main_panel_frame.png");
            Set(so, "v6InfoCardFrame", "Assets/WheatTown/Art/Images/ui-polish-v6/v6_info_card_frame.png");
            Set(so, "v6TitlePlaque", "Assets/WheatTown/Art/Images/ui-polish-v6/v6_title_plaque.png");
            Set(so, "v6PrimaryButton", "Assets/WheatTown/Art/Images/ui-polish-v6/v6_primary_button.png");
            Set(so, "v6SecondaryButton", "Assets/WheatTown/Art/Images/ui-polish-v6/v6_secondary_button.png");
            Set(so, "v6ItemSlotFrame", "Assets/WheatTown/Art/Images/ui-polish-v6/v6_item_slot_frame.png");
            Set(so, "v6NotificationBadge", "Assets/WheatTown/Art/Images/ui-polish-v6/v6_notification_badge.png");
            Set(so, "v6ProgressBarFrame", "Assets/WheatTown/Art/Images/ui-polish-v6/v6_progress_bar_frame.png");
            Set(so, "v8CleanPanel", "Assets/WheatTown/Art/Images/ui-v8/clean_parchment_panel.png");

            Set(so, "plotEmpty", "Assets/WheatTown/Art/Images/native-v2/plot_empty.png");
            Set(so, "plotGrowingOne", "Assets/WheatTown/Art/Images/native-v2/plot_growing_1.png");
            Set(so, "plotGrowingTwo", "Assets/WheatTown/Art/Images/native-v2/plot_growing_2.png");
            Set(so, "plotReadyWheat", "Assets/WheatTown/Art/Images/native-v2/plot_ready_wheat.png");
            Set(so, "plotReadyApple", "Assets/WheatTown/Art/Images/native-v2/plot_ready_apple.png");
            Set(so, "fieldEmptyClean", "Assets/WheatTown/Art/Images/field-states/field_empty_clean.png");
            Set(so, "fieldSeedlingClean", "Assets/WheatTown/Art/Images/field-states/field_seedling_clean.png");
            Set(so, "fieldYoungWheatClean", "Assets/WheatTown/Art/Images/field-states/field_young_wheat_clean.png");
            Set(so, "fieldWheatClean", "Assets/WheatTown/Art/Images/field-states/field_wheat_clean.png");
            Set(so, "fieldAppleClean", "Assets/WheatTown/Art/Images/field-states/field_apple_clean.png");
            SetArray(so, "bakedPlotEmpty", "Assets/WheatTown/Art/Images/field-baked-v14/plot_{0}_empty.png");
            SetArray(so, "bakedPlotSeeded", "Assets/WheatTown/Art/Images/field-baked-v14/plot_{0}_seeded.png");
            SetArray(so, "bakedPlotSeedling", "Assets/WheatTown/Art/Images/field-baked-v14/plot_{0}_seedling.png");
            SetArray(so, "bakedPlotYoungWheat", "Assets/WheatTown/Art/Images/field-baked-v14/plot_{0}_young_wheat.png");
            SetArray(so, "bakedPlotWheat", "Assets/WheatTown/Art/Images/field-baked-v14/plot_{0}_wheat.png");
            SetArray(so, "bakedPlotApple", "Assets/WheatTown/Art/Images/field-baked-v11/plot_{0}_apple.png");
            Set(so, "bakeryIcon", "Assets/WheatTown/Art/Images/buildings/bakery.png");
            Set(so, "dairyIcon", "Assets/WheatTown/Art/Images/native-v2/building_dairy.png");
            Set(so, "houseMiaIcon", "Assets/WheatTown/Art/Images/native-v2/house_mia.png");
            Set(so, "houseTomIcon", "Assets/WheatTown/Art/Images/native-v2/house_tom.png");
            Set(so, "machineHarvestIcon", "Assets/WheatTown/Art/Images/native-v2/machine_harvest.png");
            Set(so, "boardOrdersIcon", "Assets/WheatTown/Art/Images/native-v2/board_orders.png");
            Set(so, "bagIcon", "Assets/WheatTown/Art/Images/native-v2/icon_bag.png");
            Set(so, "questIcon", "Assets/WheatTown/Art/Images/native-v2/icon_quest.png");
            Set(so, "miaIcon", "Assets/WheatTown/Art/Images/buildings/mia.png");
            Set(so, "tomIcon", "Assets/WheatTown/Art/Images/native-v2/npc_tom_idle.png");
            Set(so, "completeBubbleIcon", "Assets/WheatTown/Art/Images/native-v2/icon_complete_bubble.png");
            Set(so, "lockIcon", "Assets/WheatTown/Art/Images/native-v2/icon_lock.png");
            Set(so, "sickleIcon", "Assets/WheatTown/Art/Images/native-v2/icon_sickle.png");
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void EnsureTmpEssentialResources()
        {
            if (File.Exists("Assets/TextMesh Pro/Resources/TMP Settings.asset") &&
                File.Exists("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset"))
            {
                return;
            }

            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogWarning("[WheatTown] TMP Essential Resources need import, but Unity is in Play Mode. Stop Play Mode and run WheatTown/Create Native Game Scene again.");
                return;
            }

            var packages = Directory.GetFiles(
                "Library/PackageCache",
                "TMP Essential Resources.unitypackage",
                SearchOption.AllDirectories);

            if (packages.Length == 0)
            {
                TMP_PackageResourceImporter.ImportResources(true, false, false);
            }
            else
            {
                AssetDatabase.ImportPackage(packages[0], false);
            }

            AssetDatabase.Refresh();
            Debug.Log("[WheatTown] TMP Essential Resources imported for crisp TextMeshPro UI.");
        }

        private static void CreateCompatibleEventSystem()
        {
            var eventObject = new GameObject("EventSystem", typeof(EventSystem));
            var inputSystemModuleType = Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (inputSystemModuleType != null)
            {
                eventObject.AddComponent(inputSystemModuleType);
                return;
            }

            eventObject.AddComponent<StandaloneInputModule>();
        }

        private static void Set(SerializedObject so, string property, string assetPath)
        {
            var prop = so.FindProperty(property);
            if (prop == null)
            {
                Debug.LogWarning("[WheatTown] Missing serialized property: " + property);
                return;
            }
            if (File.Exists(assetPath))
            {
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            }
            prop.objectReferenceValue = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (prop.objectReferenceValue == null)
            {
                Debug.LogWarning("[WheatTown] Sprite not found or not imported: " + assetPath);
            }
        }

        private static void SetArray(SerializedObject so, string property, string assetPathFormat)
        {
            var prop = so.FindProperty(property);
            if (prop == null)
            {
                Debug.LogWarning("[WheatTown] Missing serialized array property: " + property);
                return;
            }

            prop.arraySize = 6;
            for (var i = 0; i < 6; i++)
            {
                var assetPath = string.Format(assetPathFormat, i);
                if (File.Exists(assetPath))
                {
                    AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                }

                var element = prop.GetArrayElementAtIndex(i);
                element.objectReferenceValue = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                if (element.objectReferenceValue == null)
                {
                    Debug.LogWarning("[WheatTown] Baked field sprite not found or not imported: " + assetPath);
                }
            }
        }

        private static void EnsureAllSprites(string root)
        {
            var guids = AssetDatabase.FindAssets("t:Texture2D", new[] { root });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null)
                {
                    continue;
                }

                var changed = false;
                if (importer.textureType != TextureImporterType.Sprite)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    changed = true;
                }
                var desiredBorder = DesiredBorder(path);
                if (desiredBorder != Vector4.zero && importer.spriteBorder != desiredBorder)
                {
                    importer.spriteBorder = desiredBorder;
                    changed = true;
                }
                if (importer.mipmapEnabled)
                {
                    importer.mipmapEnabled = false;
                    changed = true;
                }
                if (!importer.alphaIsTransparency)
                {
                    importer.alphaIsTransparency = true;
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
            }
        }

        private static void RequireObject(List<string> errors, Transform root, string path)
        {
            if (root.Find(path) == null)
            {
                errors.Add("Missing UI object: " + path);
            }
        }

        private static void RequireMissingObject(List<string> errors, Transform root, string path)
        {
            if (root.Find(path) != null)
            {
                errors.Add("Unexpected UI object: " + path);
            }
        }

        private static void RequireButtonRaycast(List<string> errors, Transform root, string path)
        {
            var target = root.Find(path);
            if (target == null)
            {
                return;
            }
            var button = target.GetComponent<UnityEngine.UI.Button>();
            var image = target.GetComponent<UnityEngine.UI.Image>();
            if (button == null)
            {
                errors.Add("Missing Button component: " + path);
            }
            if (image == null || !image.raycastTarget)
            {
                errors.Add("Button image does not receive raycast: " + path);
            }
        }

        private static void RequireButtonMinSize(List<string> errors, Transform root, string path, float minSize)
        {
            var target = root.Find(path);
            if (target == null)
            {
                return;
            }
            var rect = target.GetComponent<RectTransform>();
            if (rect == null)
            {
                errors.Add("Missing RectTransform on button: " + path);
                return;
            }
            if (rect.sizeDelta.x < minSize || rect.sizeDelta.y < minSize)
            {
                errors.Add("Button hit target too small: " + path + " = " + rect.sizeDelta);
            }
        }

        private static void RequireTmpQuality(List<string> errors, GameObject root)
        {
            var labels = root.GetComponentsInChildren<TextMeshProUGUI>(true);
            if (labels.Length < 40)
            {
                errors.Add("Too few TMP labels built: " + labels.Length);
            }
            foreach (var label in labels)
            {
                if (label.fontSize < 13f)
                {
                    errors.Add("TMP font too small: " + label.name + " = " + label.fontSize);
                    return;
                }
                if (label.raycastTarget)
                {
                    errors.Add("TMP label blocks raycast: " + label.name);
                    return;
                }
            }
        }

        private static Vector4 DesiredBorder(string path)
        {
            if (path.Contains("/v3-ui/"))
            {
                if (path.Contains("button") || path.Contains("tab") || path.Contains("status_bar") || path.Contains("nav_plaque"))
                {
                    return new Vector4(44, 44, 44, 44);
                }
                if (path.Contains("badge") || path.Contains("settings_medallion") || path.Contains("corner") || path.Contains("divider"))
                {
                    return Vector4.zero;
                }
                return new Vector4(74, 74, 74, 74);
            }
            if (path.Contains("/harvest-v4/"))
            {
                if (path.Contains("button_round"))
                {
                    return Vector4.zero;
                }
                if (path.Contains("cell_tile"))
                {
                    return new Vector4(58, 58, 58, 58);
                }
                if (path.Contains("energy_bar") || path.Contains("info_plaque") || path.Contains("back_plaque"))
                {
                    return new Vector4(64, 64, 48, 48);
                }
                return new Vector4(86, 86, 86, 86);
            }
            if (path.Contains("/town-v5/"))
            {
                if (path.Contains("badge") || path.Contains("building_base") || path.Contains("plot_frame") || path.Contains("title_plaque"))
                {
                    return Vector4.zero;
                }
                if (path.Contains("name_scroll"))
                {
                    return new Vector4(74, 74, 44, 44);
                }
                if (path.Contains("bottom_info_frame"))
                {
                    return new Vector4(86, 86, 72, 72);
                }
                return Vector4.zero;
            }
            if (path.Contains("/inventory-v5/"))
            {
                if (path.Contains("empty_basket"))
                {
                    return Vector4.zero;
                }
                if (path.Contains("panel_frame"))
                {
                    return new Vector4(88, 88, 96, 96);
                }
                if (path.Contains("item_slot"))
                {
                    return new Vector4(74, 74, 74, 74);
                }
                if (path.Contains("tab") || path.Contains("count_badge"))
                {
                    return new Vector4(58, 58, 42, 42);
                }
                return Vector4.zero;
            }
            if (path.Contains("/task-v5/"))
            {
                if (path.Contains("milestone_badge") || path.Contains("collection_book"))
                {
                    return Vector4.zero;
                }
                if (path.Contains("quest_row_frame"))
                {
                    return new Vector4(78, 78, 54, 54);
                }
                if (path.Contains("order_board_frame"))
                {
                    return new Vector4(92, 92, 96, 96);
                }
                if (path.Contains("commission_envelope") || path.Contains("daily_route_scroll"))
                {
                    return new Vector4(86, 86, 76, 76);
                }
                return Vector4.zero;
            }
            if (path.Contains("/auth-settings-v5/"))
            {
                if (path.Contains("slider_art") || path.Contains("toggle_art"))
                {
                    return Vector4.zero;
                }
                if (path.Contains("input_frame"))
                {
                    return new Vector4(74, 74, 44, 44);
                }
                return new Vector4(88, 88, 88, 88);
            }
            if (path.Contains("/ui-polish-v6/"))
            {
                if (path.Contains("notification_badge"))
                {
                    return Vector4.zero;
                }
                if (path.Contains("progress_bar"))
                {
                    return new Vector4(74, 74, 38, 38);
                }
                if (path.Contains("button"))
                {
                    return new Vector4(76, 76, 58, 58);
                }
                if (path.Contains("item_slot"))
                {
                    return new Vector4(82, 82, 82, 82);
                }
                if (path.Contains("info_card") || path.Contains("main_panel"))
                {
                    return new Vector4(92, 92, 92, 92);
                }
                return new Vector4(72, 72, 52, 52);
            }
            if (path.Contains("/ui-v8/"))
            {
                return new Vector4(160, 160, 160, 160);
            }
            if (path.Contains("/ui-v9/"))
            {
                return new Vector4(180, 180, 180, 180);
            }
            if (path.Contains("/premium-ui/") || path.Contains("/premium-polish/") || path.Contains("/ui/"))
            {
                if (path.Contains("button") || path.Contains("nav-tab") || path.Contains("resource-pill") || path.Contains("status-strip") || path.Contains("title-plaque"))
                {
                    return new Vector4(42, 42, 42, 42);
                }
                if (path.Contains("symbol-tile"))
                {
                    return new Vector4(34, 34, 34, 34);
                }
                return new Vector4(64, 64, 64, 64);
            }
            if (path.Contains("cream-gold-label"))
            {
                return new Vector4(40, 40, 30, 30);
            }
            return Vector4.zero;
        }

        private static void RequireFile(List<string> errors, string path)
        {
            if (!File.Exists(path))
            {
                errors.Add("Missing required file: " + path);
            }
        }

        private static void RequireMissing(List<string> errors, string path)
        {
            if (File.Exists(path))
            {
                errors.Add("Forbidden old file still exists: " + path);
            }
        }

        private static void RequireText(List<string> errors, string path, string text)
        {
            if (!File.Exists(path))
            {
                errors.Add("Cannot inspect missing file: " + path);
                return;
            }
            if (!File.ReadAllText(path).Contains(text))
            {
                errors.Add("Expected text not found in " + path + ": " + text);
            }
        }

        private static void ForbidText(List<string> errors, string path, string text)
        {
            if (!File.Exists(path))
            {
                return;
            }
            if (File.ReadAllText(path).Contains(text))
            {
                errors.Add("Forbidden text found in " + path + ": " + text);
            }
        }
    }
}
