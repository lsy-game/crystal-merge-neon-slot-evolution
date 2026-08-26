using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace DestinyRanger.EditorTools
{
    public static class FateWeaverPrefabBuilder
    {
        private const string ArtRoot = "Assets/DestinyRanger/Art/Generated/FateWeaverFull";
        private const string DataRoot = "Assets/DestinyRanger/Data";
        private const string PrefabRoot = "Assets/DestinyRanger/Prefabs/FateWeaver";

        [MenuItem("Destiny Ranger/Fate Weaver/Build Integration Prefabs")]
        public static void BuildIntegrationPrefabs()
        {
            Directory.CreateDirectory(PrefabRoot + "/SlotMachine");
            Directory.CreateDirectory(PrefabRoot + "/UI");
            Directory.CreateDirectory(PrefabRoot + "/Furniture");
            Directory.CreateDirectory(PrefabRoot + "/Audio");

            BuildSlotMachinePrefab("Chamber", "chamber");
            BuildSlotMachinePrefab("Forest", "forest");
            BuildUiCanvasPrefab();
            BuildFurniturePrefabs();
            BuildAudioHubPrefab();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Fate Weaver integration prefabs built under " + PrefabRoot);
        }

        [MenuItem("Destiny Ranger/Fate Weaver/Build Integration Prefabs And Scenes")]
        public static void BuildIntegrationPrefabsAndScenes()
        {
            BuildIntegrationPrefabs();
            BuildIntegratedScene("Chamber", "chamber", "fate-weaver-chamber-bg_1290x2796.png", "ChamberToneProfile");
            BuildIntegratedScene("Forest", "forest", "fate-weaver-battle-forest-bg_1290x1398.png", "ForestToneProfile");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Fate Weaver integration prefabs and scenes built.");
        }

        public static void BuildIntegrationPrefabsBatch()
        {
            BuildIntegrationPrefabs();
        }

        public static void BuildIntegrationPrefabsAndScenesBatch()
        {
            BuildIntegrationPrefabsAndScenes();
        }

        private static void BuildSlotMachinePrefab(string displayName, string sceneId)
        {
            var root = new GameObject(displayName + " SlotMachine");
            root.tag = "Tintable";
            root.AddComponent<SlotMachine>().symbols = LoadSymbolSet(sceneId).symbols;

            AddSpriteChild(root.transform, "Body", $"{ArtRoot}/SlotMachine/{sceneId}_slot_machine_body_800x900.png", 0);
            AddSpriteChild(root.transform, "Slot Base", $"{ArtRoot}/SlotMachine/{sceneId}_slot_machine_slot_base_800x900.png", 1);
            AddSpriteChild(root.transform, "Reels", $"{ArtRoot}/SlotMachine/{sceneId}_slot_machine_reels_800x900.png", 2);
            AddSpriteChild(root.transform, "Frame", $"{ArtRoot}/SlotMachine/{sceneId}_slot_machine_frame_800x900.png", 3);
            AddSpriteChild(root.transform, "Crystal Glow", $"{ArtRoot}/SlotMachine/{sceneId}_slot_machine_crystal_glow_800x900.png", 4);

            SavePrefab(root, $"{PrefabRoot}/SlotMachine/{displayName}SlotMachine.prefab");
        }

        private static void BuildUiCanvasPrefab()
        {
            var canvasGo = new GameObject("Fate Weaver UI Canvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGo.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1290, 2796);
            canvasGo.AddComponent<GraphicRaycaster>();

            var popup = AddImage(canvasGo.transform, "Popup Panel", $"{ArtRoot}/UI/Panels/panel_chamber_parchment_9slice_900x600_border96.png", new Vector2(900, 600));
            popup.type = Image.Type.Sliced;

            var button = AddButton(canvasGo.transform, "Primary Button", $"{ArtRoot}/UI/Buttons/primary_button_normal_280x100.png", new Vector2(280, 100));
            var spriteState = button.spriteState;
            spriteState.highlightedSprite = LoadSprite($"{ArtRoot}/UI/Buttons/primary_button_hover_280x100.png");
            spriteState.pressedSprite = LoadSprite($"{ArtRoot}/UI/Buttons/primary_button_pressed_280x100.png");
            spriteState.disabledSprite = LoadSprite($"{ArtRoot}/UI/Buttons/primary_button_disabled_280x100.png");
            button.spriteState = spriteState;

            var stop = AddButton(canvasGo.transform, "Stop Button", $"{ArtRoot}/UI/Buttons/stop_button_normal_200x200.png", new Vector2(200, 200));
            var stopState = stop.spriteState;
            stopState.highlightedSprite = LoadSprite($"{ArtRoot}/UI/Buttons/stop_button_highlight_200x200.png");
            stopState.pressedSprite = LoadSprite($"{ArtRoot}/UI/Buttons/stop_button_pressed_200x200.png");
            stop.spriteState = stopState;

            AddSlicedBar(canvasGo.transform, "HP Bar", $"{ArtRoot}/UI/Bars/hp_bar_background_400x20_9slice.png", $"{ArtRoot}/UI/Bars/hp_bar_fill_400x20_9slice.png", $"{ArtRoot}/UI/Bars/hp_bar_frame_400x20_9slice.png", new Vector2(400, 20));
            AddSlicedBar(canvasGo.transform, "Energy Spindle", $"{ArtRoot}/UI/Bars/energy_spindle_background_300x40_9slice.png", $"{ArtRoot}/UI/Bars/energy_spindle_fill_300x40_9slice.png", $"{ArtRoot}/UI/Bars/energy_spindle_frame_300x40_9slice.png", new Vector2(300, 40));

            SavePrefab(canvasGo, $"{PrefabRoot}/UI/FateWeaverUiCanvas.prefab");
        }

        private static void BuildFurniturePrefabs()
        {
            foreach (var spritePath in Directory.GetFiles($"{ArtRoot}/Furniture", "*.png", SearchOption.AllDirectories))
            {
                var normalized = spritePath.Replace('\\', '/');
                if (normalized.EndsWith("_shadow.png"))
                    continue;

                var name = Path.GetFileNameWithoutExtension(normalized);
                var root = new GameObject(name);
                root.tag = "Tintable";
                AddSpriteChild(root.transform, "Artwork", normalized, 1);

                var shadowPath = normalized.Replace(".png", "_shadow.png");
                if (File.Exists(shadowPath))
                {
                    var shadow = AddSpriteChild(root.transform, "Shadow", shadowPath, 0);
                    shadow.localPosition = new Vector3(.18f, -.65f, 0f);
                }

                SavePrefab(root, $"{PrefabRoot}/Furniture/{name}.prefab");
            }
        }

        private static void BuildAudioHubPrefab()
        {
            var root = new GameObject("Fate Weaver Audio Hub");
            var hub = root.AddComponent<FateWeaverAudioHub>();
            hub.catalog = AssetDatabase.LoadAssetAtPath<AudioEventCatalog>($"{DataRoot}/AudioEventCatalog.asset");
            hub.sfxSource = AddAudioSource(root, "SFX Source");
            hub.uiSource = AddAudioSource(root, "UI Source");
            hub.ambientSource = AddAudioSource(root, "Ambient Source");
            hub.ambientSource.loop = true;
            SavePrefab(root, $"{PrefabRoot}/Audio/FateWeaverAudioHub.prefab");
        }

        private static void BuildIntegratedScene(string displayName, string sceneId, string backgroundFile, string profileName)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var cameraGo = new GameObject("Main Camera");
            cameraGo.tag = "MainCamera";
            var camera = cameraGo.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.orthographic = true;
            camera.orthographicSize = sceneId == "chamber" ? 14f : 7f;
            cameraGo.transform.position = new Vector3(0f, 0f, -10f);

            var tint = cameraGo.AddComponent<SceneColorTint>();
            tint.ActiveProfile = AssetDatabase.LoadAssetAtPath<SceneToneProfile>($"{DataRoot}/SceneToneProfiles/{profileName}.asset");

            var background = AddSpriteChild(null, "Background", $"{ArtRoot}/Backgrounds/{backgroundFile}", -10);
            background.localScale = Vector3.one * .01f;

            var slotPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabRoot}/SlotMachine/{displayName}SlotMachine.prefab");
            if (slotPrefab)
            {
                var slot = (GameObject)PrefabUtility.InstantiatePrefab(slotPrefab);
                slot.name = displayName + " SlotMachine";
                slot.transform.position = new Vector3(0f, -1.6f, 0f);
                slot.transform.localScale = Vector3.one * .01f;
            }

            InstantiatePrefab($"{PrefabRoot}/UI/FateWeaverUiCanvas.prefab", "Fate Weaver UI Canvas");
            InstantiatePrefab($"{PrefabRoot}/Audio/FateWeaverAudioHub.prefab", "Fate Weaver Audio Hub");

            Directory.CreateDirectory("Assets/Scenes");
            EditorSceneManager.SaveScene(scene, $"Assets/Scenes/FateWeaver_{displayName}Integrated.unity");
        }

        private static SceneSymbolSet LoadSymbolSet(string sceneId)
        {
            return AssetDatabase.LoadAssetAtPath<SceneSymbolSet>($"{DataRoot}/Symbols/{UpperFirst(sceneId)}SymbolSet.asset");
        }

        private static Transform AddSpriteChild(Transform parent, string name, string path, int sortingOrder)
        {
            var child = new GameObject(name);
            if (parent)
                child.transform.SetParent(parent, false);
            var renderer = child.AddComponent<SpriteRenderer>();
            renderer.sprite = LoadSprite(path);
            renderer.sortingOrder = sortingOrder;
            return child.transform;
        }

        private static Image AddImage(Transform parent, string name, string path, Vector2 size)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = size;
            var image = go.AddComponent<Image>();
            image.sprite = LoadSprite(path);
            return image;
        }

        private static Button AddButton(Transform parent, string name, string path, Vector2 size)
        {
            var image = AddImage(parent, name, path, size);
            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            return button;
        }

        private static void AddSlicedBar(Transform parent, string name, string backgroundPath, string fillPath, string framePath, Vector2 size)
        {
            var root = new GameObject(name);
            root.transform.SetParent(parent, false);
            root.AddComponent<RectTransform>().sizeDelta = size;

            var background = AddImage(root.transform, "Background", backgroundPath, size);
            background.type = Image.Type.Sliced;
            var fill = AddImage(root.transform, "Fill", fillPath, size);
            fill.type = Image.Type.Sliced;
            var frame = AddImage(root.transform, "Frame", framePath, size);
            frame.type = Image.Type.Sliced;
        }

        private static AudioSource AddAudioSource(GameObject parent, string name)
        {
            var child = new GameObject(name);
            child.transform.SetParent(parent.transform, false);
            var source = child.AddComponent<AudioSource>();
            source.playOnAwake = false;
            return source;
        }

        private static Sprite LoadSprite(string path)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (!sprite)
                Debug.LogError("Missing sprite for Fate Weaver prefab build: " + path);
            return sprite;
        }

        private static void SavePrefab(GameObject root, string path)
        {
            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
        }

        private static void InstantiatePrefab(string path, string fallbackName)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (!prefab)
                return;

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.name = fallbackName;
        }

        private static string UpperFirst(string value)
        {
            return char.ToUpperInvariant(value[0]) + value.Substring(1);
        }
    }
}
