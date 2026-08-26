using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DestinyRanger.EditorTools
{
    public sealed class FateWeaverArtFusionProcessor : AssetPostprocessor
    {
        private const string FusionFolder = "Assets/DestinyRanger/Art/Generated/FateWeaverFusion";
        private const string DeliverablesFolder = "Assets/DestinyRanger/Art/Generated/FateWeaverDeliverables";
        private const string PreviewFolder = "Assets/DestinyRanger/Art/Generated/FateWeaverFusionPreviews";
        private const string ValidationScenePath = "Assets/Scenes/FateWeaverArtFusionValidation.unity";
        private const string ChamberBg = "Assets/DestinyRanger/Art/Generated/fate-weaver-chamber-bg.png";
        private const string BattleBg = "Assets/DestinyRanger/Art/Generated/fate-weaver-battle-forest-bg.png";

        private void OnPreprocessTexture()
        {
            if (!IsManagedPng(assetPath))
                return;

            var importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.isReadable = true;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.maxTextureSize = 4096;
            if (assetPath.Contains("9slice"))
                importer.spriteBorder = new Vector4(96, 96, 96, 96);
        }

        private void OnPostprocessTexture(Texture2D texture)
        {
            if (!IsManagedPng(assetPath))
                return;

            FeatherAlpha(texture, 2, .42f);
        }

        [MenuItem("Destiny Ranger/Art Fusion/Reimport Fusion Sprites")]
        public static void ReimportFusionSprites()
        {
            foreach (var guid in AssetDatabase.FindAssets("t:Texture2D", new[] { FusionFolder }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
            AssetDatabase.Refresh();
            Debug.Log("Fate Weaver fusion sprites reimported with edge feathering.");
        }

        [MenuItem("Destiny Ranger/Art Fusion/Reimport Deliverable Sprites")]
        public static void ReimportDeliverableSprites()
        {
            foreach (var guid in AssetDatabase.FindAssets("t:Texture2D", new[] { DeliverablesFolder }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
            AssetDatabase.Refresh();
            Debug.Log("Fate Weaver deliverable sprites reimported with edge feathering and 9-slice borders.");
        }

        [MenuItem("Destiny Ranger/Art Fusion/Create Validation Scene")]
        public static void CreateValidationScene()
        {
            EnsureSpriteImport(ChamberBg);
            EnsureSpriteImport(BattleBg);
            ReimportFusionSprites();

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var cameraGo = new GameObject("FusionValidationCamera");
            var camera = cameraGo.AddComponent<Camera>();
            camera.backgroundColor = new Color32(10, 15, 30, 255);
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.orthographic = true;
            camera.orthographicSize = 13.98f;
            cameraGo.transform.position = new Vector3(0, 0, -10);

            var chamber = new GameObject("Validation_Chamber");
            chamber.transform.position = new Vector3(-7.2f, 0, 0);
            AddSprite(chamber.transform, "ChamberBackground", ChamberBg, Vector3.zero, new Vector3(12.9f, 27.96f, 1));
            AddSprite(chamber.transform, "ChamberRug", FusionFolder + "/furniture_chamber_rug.png", new Vector3(0, -8.9f, 0), new Vector3(5.2f, 2.7f, 1));
            AddSprite(chamber.transform, "AileenShadow", FusionFolder + "/aileen_chamber_shadow.png", new Vector3(-3.4f, -7.7f, 0), new Vector3(3.8f, 1.2f, 1));
            AddSprite(chamber.transform, "Aileen", FusionFolder + "/aileen_chamber.png", new Vector3(-3.4f, -4.1f, 0), new Vector3(4.0f, 6.3f, 1));
            AddSprite(chamber.transform, "Bookcase", FusionFolder + "/furniture_chamber_bookcase.png", new Vector3(4.3f, 2.2f, 0), new Vector3(2.7f, 5.8f, 1));
            AddSprite(chamber.transform, "ChamberPanel", FusionFolder + "/ui_panel_chamber_parchment.png", new Vector3(0, 10.1f, 0), new Vector3(5.8f, 2.5f, 1));

            var forest = new GameObject("Validation_Forest");
            forest.transform.position = new Vector3(7.2f, 0, 0);
            AddSprite(forest.transform, "ForestBackground", BattleBg, Vector3.zero, new Vector3(12.9f, 27.96f, 1));
            AddSprite(forest.transform, "SlimeShadow", FusionFolder + "/forest_shadow_slime_shadow.png", new Vector3(2.5f, 3.2f, 0), new Vector3(3.9f, 1.1f, 1));
            AddSprite(forest.transform, "Slime", FusionFolder + "/forest_shadow_slime.png", new Vector3(2.5f, 5.6f, 0), new Vector3(3.9f, 3.6f, 1));
            AddSprite(forest.transform, "ForestPanel", FusionFolder + "/ui_panel_forest_frosted_metal.png", new Vector3(0, .4f, 0), new Vector3(5.8f, 2.5f, 1));
            AddSprite(forest.transform, "ForestSymbols", FusionFolder + "/symbols_forest_sheet.png", new Vector3(0, -5.8f, 0), new Vector3(5.1f, 3.4f, 1));

            Directory.CreateDirectory("Assets/Scenes");
            EditorSceneManager.SaveScene(scene, ValidationScenePath);
            AssetDatabase.Refresh();
            Debug.Log("Fate Weaver art fusion validation scene created: " + ValidationScenePath);
        }

        [MenuItem("Destiny Ranger/Art Fusion/Render Validation Screenshots")]
        public static void RenderValidationScreenshots()
        {
            CreateValidationScene();
            var camera = Object.FindObjectOfType<Camera>();
            if (!camera)
                throw new System.InvalidOperationException("Validation camera missing.");

            Directory.CreateDirectory(PreviewFolder);
            RenderCamera(camera, new Vector3(-7.2f, 0, -10), PreviewFolder + "/unity_validation_chamber.png");
            RenderCamera(camera, new Vector3(7.2f, 0, -10), PreviewFolder + "/unity_validation_forest.png");
            AssetDatabase.Refresh();
            Debug.Log("Fate Weaver art fusion screenshots rendered under " + PreviewFolder);
        }

        private static void FeatherAlpha(Texture2D texture, int pixels, float strength)
        {
            var width = texture.width;
            var height = texture.height;
            var source = texture.GetPixels32();
            var result = new Color32[source.Length];
            System.Array.Copy(source, result, source.Length);

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var index = y * width + x;
                if (source[index].a == 0)
                    continue;

                var nearEdge = false;
                for (var oy = -pixels; oy <= pixels && !nearEdge; oy++)
                for (var ox = -pixels; ox <= pixels; ox++)
                {
                    var nx = x + ox;
                    var ny = y + oy;
                    if (nx < 0 || ny < 0 || nx >= width || ny >= height || source[ny * width + nx].a < 12)
                    {
                        nearEdge = true;
                        break;
                    }
                }

                if (nearEdge)
                    result[index].a = (byte)Mathf.RoundToInt(source[index].a * (1f - strength));
            }

            texture.SetPixels32(result);
            texture.Apply();
        }

        private static bool IsManagedPng(string path)
        {
            return path.EndsWith(".png") && (path.StartsWith(FusionFolder) || path.StartsWith(DeliverablesFolder));
        }

        private static void EnsureSpriteImport(string path)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (!importer)
                return;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.maxTextureSize = 4096;
            importer.SaveAndReimport();
        }

        private static void AddSprite(Transform parent, string name, string path, Vector3 localPosition, Vector3 localScale)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (!sprite)
                return;
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            go.transform.localScale = localScale;
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
        }

        private static void RenderCamera(Camera camera, Vector3 cameraPosition, string path)
        {
            camera.transform.position = cameraPosition;
            var rt = new RenderTexture(1290, 2796, 24, RenderTextureFormat.ARGB32);
            var tex = new Texture2D(1290, 2796, TextureFormat.RGBA32, false);
            camera.targetTexture = rt;
            camera.Render();
            RenderTexture.active = rt;
            tex.ReadPixels(new Rect(0, 0, 1290, 2796), 0, 0);
            tex.Apply();
            File.WriteAllBytes(path, tex.EncodeToPNG());
            camera.targetTexture = null;
            RenderTexture.active = null;
            Object.DestroyImmediate(rt);
            Object.DestroyImmediate(tex);
        }
    }
}
