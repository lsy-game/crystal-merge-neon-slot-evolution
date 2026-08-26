using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DestinyRanger.EditorTools
{
    public static class FateWeaverSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/FateWeaverPrototype.unity";
        private const string ChamberPath = "Assets/DestinyRanger/Art/Generated/fate-weaver-chamber-bg.png";
        private const string BattlePath = "Assets/DestinyRanger/Art/Generated/fate-weaver-battle-forest-bg.png";
        private const string SymbolsPath = "Assets/DestinyRanger/Art/Generated/fate-weaver-symbols-sheet.png";

        [MenuItem("Destiny Ranger/Create Fate Weaver Prototype Scene")]
        public static void CreateFateWeaverPrototypeScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var cameraGo = new GameObject("Main Camera");
            cameraGo.tag = "MainCamera";
            var camera = cameraGo.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color32(10, 15, 30, 255);
            camera.orthographic = true;
            camera.orthographicSize = 7f;
            cameraGo.transform.position = new Vector3(0, 0, -10);

            EnsureSpriteImport(ChamberPath);
            EnsureSpriteImport(BattlePath);
            EnsureReadableTexture(SymbolsPath);

            var controller = new GameObject("FateWeaverGame");
            var game = controller.AddComponent<FateWeaverGame>();
            game.chamberBackground = AssetDatabase.LoadAssetAtPath<Sprite>(ChamberPath);
            game.battleBackground = AssetDatabase.LoadAssetAtPath<Sprite>(BattlePath);
            game.symbolSheet = AssetDatabase.LoadAssetAtPath<Texture2D>(SymbolsPath);

            Directory.CreateDirectory("Assets/Scenes");
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Fate Weaver prototype scene created: " + ScenePath);
        }

        public static void CreateFateWeaverPrototypeSceneBatch()
        {
            CreateFateWeaverPrototypeScene();
        }

        public static void ValidateFateWeaverPrototypeBatch()
        {
            CreateFateWeaverPrototypeScene();
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            if (!Object.FindObjectOfType<FateWeaverGame>())
                throw new System.InvalidOperationException("FateWeaverGame missing from " + scene.path);
            RequireAsset(ChamberPath);
            RequireAsset(BattlePath);
            RequireAsset(SymbolsPath);
            RequireFileContains("Assets/DestinyRanger/Scripts/SlotMachine.cs", "EvaluateLine(0, 0, 1, 0, 2, 0", "Horizontal line evaluation is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/SlotMachine.cs", "RelicEffect.EnableDiagonals", "Diagonal relic gate is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/EnergySystem.cs", "OnEnergyFull", "Energy full event is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/SkillManager.cs", "三骷髅", "Skull penalty skill mapping is missing.");
            Debug.Log("Fate Weaver prototype validation passed.");
        }

        private static void EnsureSpriteImport(string path)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
                return;

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.maxTextureSize = 4096;
            importer.SaveAndReimport();
        }

        private static void EnsureReadableTexture(string path)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
                return;

            importer.textureType = TextureImporterType.Default;
            importer.isReadable = true;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.maxTextureSize = 4096;
            importer.SaveAndReimport();
        }

        private static void RequireAsset(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Required generated asset missing", path);
        }

        private static void RequireFileContains(string path, string needle, string message)
        {
            if (!File.ReadAllText(path).Contains(needle))
                throw new System.InvalidOperationException(message);
        }
    }
}
