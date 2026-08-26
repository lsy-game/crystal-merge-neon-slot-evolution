using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DestinyRanger.EditorTools
{
    public static class DestinyRangerSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/DestinyRangerPrototype.unity";
        private const string ControllerName = "DestinyRangerPrototype";
        private const string ConceptPath = "Assets/DestinyRanger/Art/Generated/destiny-ranger-concept.png";
        private const string RuneSheetPath = "Assets/DestinyRanger/Art/Generated/destiny-ranger-rune-ui.png";
        private const string CharacterSheetPath = "Assets/DestinyRanger/Art/Generated/destiny-ranger-sprite-sheet.png";
        private const string RuneIconSheetPath = "Assets/DestinyRanger/Art/Generated/destiny-ranger-rune-icons-sheet.png";

        [MenuItem("Destiny Ranger/Create Prototype Scene")]
        public static void CreatePrototypeScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var cameraGo = new GameObject("Main Camera");
            cameraGo.tag = "MainCamera";
            var camera = cameraGo.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 7.4f;
            camera.backgroundColor = Parse("#2B3A42");
            cameraGo.transform.position = new Vector3(0, 0, -10);

            var controller = new GameObject(ControllerName);
            var prototype = controller.AddComponent<DestinyRangerPrototype>();
            EnsureSpriteImport(ConceptPath);
            EnsureSpriteImport(RuneSheetPath);
            EnsureSpriteImport(CharacterSheetPath);
            EnsureSpriteImport(RuneIconSheetPath);
            AssignOptionalSprite(prototype, "conceptBackground", ConceptPath);
            AssignOptionalSprite(prototype, "runeConceptSheet", RuneSheetPath);
            AssignOptionalSprite(prototype, "characterSpriteSheet", CharacterSheetPath);
            AssignOptionalSprite(prototype, "runeIconSheet", RuneIconSheetPath);

            Directory.CreateDirectory("Assets/Scenes");
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.Refresh();
            Debug.Log("Destiny Ranger prototype scene created: " + ScenePath);
        }

        public static void CreatePrototypeSceneBatch()
        {
            CreatePrototypeScene();
        }

        private static void AssignOptionalSprite(Object target, string fieldName, string path)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (!sprite) return;
            var so = new SerializedObject(target);
            var prop = so.FindProperty(fieldName);
            if (prop == null) return;
            prop.objectReferenceValue = sprite;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void EnsureSpriteImport(string path)
        {
            if (!File.Exists(path)) return;
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) return;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
        }

        private static Color Parse(string html)
        {
            ColorUtility.TryParseHtmlString(html, out var color);
            return color;
        }
    }
}
