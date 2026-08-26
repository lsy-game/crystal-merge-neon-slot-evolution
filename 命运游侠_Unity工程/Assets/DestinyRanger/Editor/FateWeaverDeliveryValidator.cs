using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace DestinyRanger.EditorTools
{
    public static class FateWeaverDeliveryValidator
    {
        private const string ArtRoot = "Assets/DestinyRanger/Art/Generated/FateWeaverFull";
        private const string DataRoot = "Assets/DestinyRanger/Data";
        private const string PrefabRoot = "Assets/DestinyRanger/Prefabs/FateWeaver";
        private const string SceneRoot = "Assets/Scenes";

        [MenuItem("Destiny Ranger/Fate Weaver/Validate Full Delivery")]
        public static void ValidateFullDelivery()
        {
            var errors = new List<string>();

            ValidateSceneFoundation(errors);
            ValidateCharacters(errors);
            ValidateMonsters(errors);
            ValidateSlotMachineAndSymbols(errors);
            ValidateUi(errors);
            ValidateFurniture(errors);
            ValidateAudio(errors);
            ValidateIntegration(errors);

            if (errors.Count > 0)
                throw new InvalidOperationException("Fate Weaver delivery validation failed:\n- " + string.Join("\n- ", errors));

            Debug.Log("Fate Weaver full delivery validation passed.");
        }

        public static void ValidateFullDeliveryBatch()
        {
            ValidateFullDelivery();
        }

        [MenuItem("Destiny Ranger/Fate Weaver/Validate Built Integration")]
        public static void ValidateBuiltIntegration()
        {
            var errors = new List<string>();

            ValidateBuiltPrefabs(errors);
            ValidateBuiltScenes(errors);

            if (errors.Count > 0)
                throw new InvalidOperationException("Fate Weaver built integration validation failed:\n- " + string.Join("\n- ", errors));

            Debug.Log("Fate Weaver built integration validation passed.");
        }

        public static void ValidateBuiltIntegrationBatch()
        {
            ValidateBuiltIntegration();
        }

        private static void ValidateSceneFoundation(List<string> errors)
        {
            RequireTexture(errors, $"{ArtRoot}/Backgrounds/fate-weaver-chamber-bg_1290x2796.png", 1290, 2796);
            RequireTexture(errors, $"{ArtRoot}/Backgrounds/fate-weaver-battle-forest-bg_1290x1398.png", 1290, 1398);
            RequireTexture(errors, $"{ArtRoot}/Backgrounds/fate-weaver-battle-volcano-bg_1290x1398.png", 1290, 1398);
            RequireTexture(errors, $"{ArtRoot}/Backgrounds/fate-weaver-battle-void-boss-bg_1290x1398.png", 1290, 1398);
            RequireTexture(errors, "Assets/DestinyRanger/Art/Common/shadow_default.png", 256, 256);

            RequireProfile(errors, "ChamberToneProfile", new Color32(20, 25, 50, 255), new Color32(5, 8, 20, 255), .10f, .40f);
            RequireProfile(errors, "ForestToneProfile", new Color32(40, 60, 30, 255), new Color32(10, 20, 5, 255), .15f, .50f);
            RequireProfile(errors, "VolcanoToneProfile", new Color32(60, 30, 20, 255), new Color32(20, 5, 5, 255), .12f, .45f);
            RequireProfile(errors, "VoidToneProfile", new Color32(40, 20, 40, 255), new Color32(15, 5, 15, 255), .18f, .55f);

            RequireFile(errors, "Assets/DestinyRanger/Scripts/SceneColorTint.cs");
            RequireFile(errors, "Assets/DestinyRanger/Editor/FateWeaverAssetPostprocessor.cs");
        }

        private static void ValidateCharacters(List<string> errors)
        {
            foreach (var character in new[] { "aileen", "grick", "luna" })
            {
                RequireSequence(errors, $"{ArtRoot}/Characters/{character}", $"{character}_idle_*_512x768.png", 4, 512, 768);
                RequireSequence(errors, $"{ArtRoot}/Characters/{character}", $"{character}_attack_*_512x768.png", 4, 512, 768);
                RequireSequence(errors, $"{ArtRoot}/Characters/{character}", $"{character}_hit_*_512x768.png", 2, 512, 768);
                RequireSequence(errors, $"{ArtRoot}/Characters/{character}", $"{character}_skill_*_512x768.png", 3, 512, 768);
                RequireSequence(errors, $"{ArtRoot}/Characters/{character}", $"{character}_death_*_512x768.png", 3, 512, 768);
                RequireSequence(errors, $"{ArtRoot}/Characters/{character}/Shadows", $"{character}_*_shadow.png", 16);
            }
        }

        private static void ValidateMonsters(List<string> errors)
        {
            RequireMonster(errors, "shadow_small", 256, 2, 2, 0, 2);
            RequireMonster(errors, "treant", 384, 2, 3, 0, 3);
            RequireMonster(errors, "toxic_moth", 384, 2, 2, 0, 2);
            RequireMonster(errors, "gargoyle", 512, 2, 4, 0, 3);
            RequireMonster(errors, "void_weaver_boss", 768, 4, 6, 2, 5);
        }

        private static void ValidateSlotMachineAndSymbols(List<string> errors)
        {
            foreach (var scene in new[] { "chamber", "forest" })
            {
                foreach (var layer in new[] { "body", "frame", "reels", "slot_base", "crystal_glow" })
                    RequireTexture(errors, $"{ArtRoot}/SlotMachine/{scene}_slot_machine_{layer}_800x900.png", 800, 900);
            }

            foreach (var scene in new[] { "chamber", "forest", "volcano", "void" })
            {
                foreach (var symbol in new[] { "sword", "staff", "heart", "shield", "skull", "star" })
                    RequireTexture(errors, $"{ArtRoot}/Symbols/{scene}/symbol_{symbol}_{scene}.png", 180, 180);

                RequireFile(errors, $"{DataRoot}/Symbols/{UpperFirst(scene)}SymbolSet.asset");
            }

            foreach (var symbol in new[] { "sword", "staff", "heart", "shield", "skull", "star" })
            {
                RequireTexture(errors, $"{ArtRoot}/Symbols/disabled/symbol_{symbol}_disabled.png", 180, 180);
                RequireTexture(errors, $"{ArtRoot}/Symbols/highlight/symbol_{symbol}_highlight.png", 180, 180);
            }
        }

        private static void ValidateUi(List<string> errors)
        {
            foreach (var icon in new[] { "adventure", "hero", "home", "quest", "weave", "workshop" })
                RequireTexture(errors, $"{ArtRoot}/UI/BottomMenu/icon_bottom_{icon}_120x120.png", 120, 120);

            foreach (var icon in new[] { "coin_gear", "diamond_prism", "fate_thread" })
                RequireTexture(errors, $"{ArtRoot}/UI/Currency/icon_currency_{icon}_64x64.png", 64, 64);

            foreach (var icon in new[] { "altar", "battle", "boss", "event", "shop" })
                RequireTexture(errors, $"{ArtRoot}/UI/MapNodes/icon_map_{icon}_120x120.png", 120, 120);

            RequireNineSlice(errors, $"{ArtRoot}/UI/Panels/panel_chamber_parchment_9slice_900x600_border96.png", 900, 600, 96);
            RequireNineSlice(errors, $"{ArtRoot}/UI/Panels/panel_forest_frosted_metal_9slice_900x600_border96.png", 900, 600, 96);
            RequireNineSlice(errors, $"{ArtRoot}/UI/Panels/panel_common_dark_translucent_9slice_900x600_border96.png", 900, 600, 96);

            foreach (var state in new[] { "normal", "hover", "pressed", "disabled" })
                RequireTexture(errors, $"{ArtRoot}/UI/Buttons/primary_button_{state}_280x100.png", 280, 100);

            foreach (var state in new[] { "normal", "highlight", "pressed" })
                RequireTexture(errors, $"{ArtRoot}/UI/Buttons/stop_button_{state}_200x200.png", 200, 200);

            RequireTexture(errors, $"{ArtRoot}/UI/Buttons/close_button_normal_40x40.png", 40, 40);
            RequireTexture(errors, $"{ArtRoot}/UI/Buttons/close_button_pressed_40x40.png", 40, 40);

            RequireNineSlice(errors, $"{ArtRoot}/UI/Bars/hp_bar_background_400x20_9slice.png", 400, 20, 8);
            RequireNineSlice(errors, $"{ArtRoot}/UI/Bars/hp_bar_frame_400x20_9slice.png", 400, 20, 8);
            RequireNineSlice(errors, $"{ArtRoot}/UI/Bars/hp_bar_fill_400x20_9slice.png", 400, 20, 8);
            RequireNineSlice(errors, $"{ArtRoot}/UI/Bars/energy_spindle_background_300x40_9slice.png", 300, 40, 8);
            RequireNineSlice(errors, $"{ArtRoot}/UI/Bars/energy_spindle_frame_300x40_9slice.png", 300, 40, 8);
            RequireNineSlice(errors, $"{ArtRoot}/UI/Bars/energy_spindle_fill_300x40_9slice.png", 300, 40, 8);
            RequireTexture(errors, $"{ArtRoot}/UI/title_fate_weaver_命运纺机_4x_supersampled_400x100.png", 400, 100);
            RequireFile(errors, "Assets/Fonts/STHeiti Medium.ttc");
            RequireFile(errors, "Assets/Fonts/Songti.ttc");
        }

        private static void ValidateFurniture(List<string> errors)
        {
            RequireFurnitureSet(errors, "window", new[] { "window_star_night", "window_aurora", "window_abyss_rift", "window_forest_morning", "window_japanese_paper" }, 350, 550);
            RequireFurnitureSet(errors, "bookcase", new[] { "bookcase_oak", "bookcase_arcane", "bookcase_crystal" }, 200, 400);
            RequireFurnitureSet(errors, "tapestry", new[] { "tapestry_fate", "tapestry_hero", "tapestry_star" }, 400, 300);
            RequireFurnitureSet(errors, "rug", new[] { "rug_warm", "rug_magic_circle", "rug_gold_thread" }, 700, 500);
            RequireFurnitureSet(errors, "decor", new[] { "decor_candle", "decor_crystal_ball", "decor_hourglass", "decor_pet_cat", "decor_weaver_bird" }, 120, 120);
            RequireFurnitureSet(errors, "display", new[] { "display_trophy_case" }, 300, 400);
            RequireFurnitureSet(errors, "boss_badge", new[] { "boss_badge_1", "boss_badge_2", "boss_badge_3", "boss_badge_4", "boss_badge_5" }, 64, 64);

            foreach (var asset in Directory.GetFiles($"{DataRoot}/Furniture", "*.asset"))
                RequireFile(errors, asset.Replace('\\', '/'));
        }

        private static void ValidateAudio(List<string> errors)
        {
            RequireSequence(errors, "Assets/Audio/SFX/SlotMachine", "*.wav", 6);
            RequireSequence(errors, "Assets/Audio/SFX/Combat", "*.wav", 11);
            RequireSequence(errors, "Assets/Audio/SFX/UI", "*.wav", 6);
            RequireSequence(errors, "Assets/Audio/Ambient", "*.wav", 2);
            RequireFile(errors, $"{DataRoot}/AudioEventCatalog.asset");
        }

        private static void ValidateIntegration(List<string> errors)
        {
            RequireRecursiveSequence(errors, "Assets/DestinyRanger/Animations/FateWeaver", "*.anim", 31);
            foreach (var scene in new[] { "Chamber", "Forest", "Volcano", "Void" })
                RequireFileContains(errors, $"{SceneRoot}/FateWeaver_{scene}Tone.unity", "m_Script: {fileID: 11500000, guid: 0fa1f63170a24e449bf0f2f5196d9412, type: 3}", $"SceneColorTint missing in {scene} tone scene.");

            RequireFileContains(errors, "Packages/manifest.json", "com.unity.textmeshpro", "TextMeshPro package missing from manifest.");
            RequireFile(errors, "Assets/DestinyRanger/Docs/SELF_CHECK_REPORT.md");
        }

        private static void ValidateBuiltPrefabs(List<string> errors)
        {
            ValidateSlotMachinePrefab(errors, $"{PrefabRoot}/SlotMachine/ChamberSlotMachine.prefab");
            ValidateSlotMachinePrefab(errors, $"{PrefabRoot}/SlotMachine/ForestSlotMachine.prefab");

            var uiCanvas = RequirePrefabComponent<Canvas>(errors, $"{PrefabRoot}/UI/FateWeaverUiCanvas.prefab");
            if (uiCanvas)
            {
                var buttons = uiCanvas.GetComponentsInChildren<Button>(true);
                var images = uiCanvas.GetComponentsInChildren<Image>(true);
                if (buttons.Length < 2)
                    errors.Add("FateWeaverUiCanvas.prefab expected at least 2 Button components.");
                if (images.Length < 8)
                    errors.Add("FateWeaverUiCanvas.prefab expected panel/button/bar Image components.");
            }

            var audioHub = RequirePrefabComponent<FateWeaverAudioHub>(errors, $"{PrefabRoot}/Audio/FateWeaverAudioHub.prefab");
            if (audioHub)
            {
                if (!audioHub.catalog)
                    errors.Add("FateWeaverAudioHub.prefab has no AudioEventCatalog assigned.");
                if (!audioHub.sfxSource || !audioHub.uiSource || !audioHub.ambientSource)
                    errors.Add("FateWeaverAudioHub.prefab expected SFX, UI, and Ambient AudioSource references.");
            }

            if (!Directory.Exists($"{PrefabRoot}/Furniture"))
            {
                errors.Add("Missing folder: " + $"{PrefabRoot}/Furniture");
                return;
            }

            var furniturePrefabs = Directory.GetFiles($"{PrefabRoot}/Furniture", "*.prefab", SearchOption.TopDirectoryOnly);
            if (furniturePrefabs.Length != 25)
                errors.Add($"{PrefabRoot}/Furniture expected 25 furniture prefabs, got {furniturePrefabs.Length}.");

            foreach (var prefab in furniturePrefabs)
            {
                var root = AssetDatabase.LoadAssetAtPath<GameObject>(prefab.Replace('\\', '/'));
                if (!root)
                    continue;
                if (root.GetComponentsInChildren<SpriteRenderer>(true).Length < 1)
                    errors.Add(prefab + " expected at least one SpriteRenderer.");
            }
        }

        private static void ValidateBuiltScenes(List<string> errors)
        {
            RequireFile(errors, $"{SceneRoot}/FateWeaver_ChamberIntegrated.unity");
            RequireFile(errors, $"{SceneRoot}/FateWeaver_ForestIntegrated.unity");
        }

        private static void ValidateSlotMachinePrefab(List<string> errors, string path)
        {
            var slot = RequirePrefabComponent<SlotMachine>(errors, path);
            if (!slot)
                return;

            if (slot.symbols == null || slot.symbols.Length != 6)
                errors.Add(path + " expected SlotMachine.symbols to contain 6 sprites.");
        }

        private static T RequirePrefabComponent<T>(List<string> errors, string path) where T : Component
        {
            RequireFile(errors, path);
            var root = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (!root)
            {
                errors.Add(path + " could not be loaded as prefab GameObject.");
                return null;
            }

            var component = root.GetComponentInChildren<T>(true);
            if (!component)
                errors.Add(path + " missing component " + typeof(T).Name + ".");

            return component;
        }

        private static void RequireMonster(List<string> errors, string monster, int size, int idle, int attack, int hit, int death)
        {
            var path = $"{ArtRoot}/Monsters/Forest/{monster}";
            RequireSequence(errors, path, $"{monster}_idle_*_{size}x{size}.png", idle, size, size);
            RequireSequence(errors, path, $"{monster}_attack_*_{size}x{size}.png", attack, size, size);
            if (hit > 0)
                RequireSequence(errors, path, $"{monster}_hit_*_{size}x{size}.png", hit, size, size);
            RequireSequence(errors, path, $"{monster}_death_*_{size}x{size}.png", death, size, size);
            RequireSequence(errors, $"{path}/Shadows", $"{monster}_*_shadow.png", idle + attack + hit + death);
        }

        private static void RequireFurnitureSet(List<string> errors, string folder, IEnumerable<string> names, int width, int height)
        {
            foreach (var name in names)
            {
                RequireTexture(errors, $"{ArtRoot}/Furniture/{folder}/{name}.png", width, height);
                RequireTexture(errors, $"{ArtRoot}/Furniture/{folder}/{name}_shadow.png");
                RequireFile(errors, $"{DataRoot}/Furniture/{name}.asset");
            }
        }

        private static void RequireProfile(List<string> errors, string name, Color expectedMain, Color expectedShadow, float expectedOverlay, float expectedShadowOpacity)
        {
            var path = $"{DataRoot}/SceneToneProfiles/{name}.asset";
            var profile = AssetDatabase.LoadAssetAtPath<SceneToneProfile>(path);
            if (!profile)
            {
                errors.Add(path + " could not be loaded as SceneToneProfile.");
                return;
            }

            RequireColor(errors, path + " sceneMainColor", profile.sceneMainColor, expectedMain);
            RequireColor(errors, path + " sceneShadowColor", profile.sceneShadowColor, expectedShadow);
            RequireFloat(errors, path + " colorOverlayStrength", profile.colorOverlayStrength, expectedOverlay);
            RequireFloat(errors, path + " shadowOpacity", profile.shadowOpacity, expectedShadowOpacity);
        }

        private static void RequireTexture(List<string> errors, string path, int expectedWidth = -1, int expectedHeight = -1)
        {
            if (!File.Exists(path))
            {
                errors.Add("Missing texture: " + path);
                return;
            }

            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (!texture)
            {
                errors.Add(path + " could not be loaded as Texture2D.");
                return;
            }

            if (expectedWidth > 0 && expectedHeight > 0 && (texture.width != expectedWidth || texture.height != expectedHeight))
                errors.Add($"{path} expected {expectedWidth}x{expectedHeight}, got {texture.width}x{texture.height}.");

            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                errors.Add(path + " has no TextureImporter.");
                return;
            }

            if (importer.textureType != TextureImporterType.Sprite)
                errors.Add(path + " is not imported as Sprite.");
            if (importer.mipmapEnabled)
                errors.Add(path + " has mipmaps enabled.");
            if (!importer.alphaIsTransparency)
                errors.Add(path + " alphaIsTransparency is disabled.");
        }

        private static void RequireNineSlice(List<string> errors, string path, int expectedWidth, int expectedHeight, int expectedBorder)
        {
            RequireTexture(errors, path, expectedWidth, expectedHeight);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
                return;

            var border = importer.spriteBorder;
            if (!Nearly(border.x, expectedBorder) || !Nearly(border.y, expectedBorder) || !Nearly(border.z, expectedBorder) || !Nearly(border.w, expectedBorder))
                errors.Add($"{path} expected sprite border {expectedBorder}, got {border}.");
        }

        private static void RequireSequence(List<string> errors, string folder, string pattern, int expectedCount, int expectedWidth = -1, int expectedHeight = -1)
        {
            if (!Directory.Exists(folder))
            {
                errors.Add("Missing folder: " + folder);
                return;
            }

            var files = Directory.GetFiles(folder, pattern, SearchOption.TopDirectoryOnly);
            if (files.Length != expectedCount)
                errors.Add($"{folder}/{pattern} expected {expectedCount} files, got {files.Length}.");

            if (expectedWidth <= 0 || expectedHeight <= 0)
                return;

            foreach (var file in files)
                RequireTexture(errors, file.Replace('\\', '/'), expectedWidth, expectedHeight);
        }

        private static void RequireRecursiveSequence(List<string> errors, string folder, string pattern, int expectedCount)
        {
            if (!Directory.Exists(folder))
            {
                errors.Add("Missing folder: " + folder);
                return;
            }

            var files = Directory.GetFiles(folder, pattern, SearchOption.AllDirectories);
            if (files.Length != expectedCount)
                errors.Add($"{folder}/**/{pattern} expected {expectedCount} files, got {files.Length}.");
        }

        private static void RequireFile(List<string> errors, string path)
        {
            if (!File.Exists(path))
                errors.Add("Missing file: " + path);
        }

        private static void RequireFileContains(List<string> errors, string path, string needle, string message)
        {
            if (!File.Exists(path))
            {
                errors.Add("Missing file: " + path);
                return;
            }

            if (!File.ReadAllText(path).Contains(needle))
                errors.Add(message);
        }

        private static void RequireColor(List<string> errors, string label, Color actual, Color expected)
        {
            if (Mathf.Abs(actual.r - expected.r) > .005f || Mathf.Abs(actual.g - expected.g) > .005f || Mathf.Abs(actual.b - expected.b) > .005f)
                errors.Add($"{label} expected {expected}, got {actual}.");
        }

        private static void RequireFloat(List<string> errors, string label, float actual, float expected)
        {
            if (Mathf.Abs(actual - expected) > .001f)
                errors.Add($"{label} expected {expected}, got {actual}.");
        }

        private static bool Nearly(float actual, float expected)
        {
            return Mathf.Abs(actual - expected) < .001f;
        }

        private static string UpperFirst(string value)
        {
            return char.ToUpperInvariant(value[0]) + value.Substring(1);
        }
    }
}
