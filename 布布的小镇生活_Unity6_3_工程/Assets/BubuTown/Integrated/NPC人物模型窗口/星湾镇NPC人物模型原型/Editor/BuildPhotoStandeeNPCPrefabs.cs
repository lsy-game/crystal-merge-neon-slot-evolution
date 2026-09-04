using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class BuildPhotoStandeeNPCPrefabs
{
    private const string BaseDir = "Assets/StarBayTown/NPCPrototype";
    private const string TextureDir = BaseDir + "/Textures/PhotoStandee";
    private const string MeshDir = BaseDir + "/Models/PhotoStandee";
    private const string MaterialDir = BaseDir + "/Materials/PhotoStandee";
    private const string PrefabDir = BaseDir + "/Prefabs/PhotoStandee";
    private const string ScenePath = BaseDir + "/Scenes/StarBayNPC_PhotoStandeePreview.unity";
    private const string ScreenshotPath = BaseDir + "/Docs/StarBayNPC_PhotoStandeePreview.png";

    private struct NpcSpec
    {
        public string Texture;
        public string BackTexture;
        public string Prefab;
        public string Mesh;
        public string Label;
        public Vector3 PreviewPosition;

        public NpcSpec(string texture, string backTexture, string prefab, string mesh, string label, Vector3 previewPosition)
        {
            Texture = texture;
            BackTexture = backTexture;
            Prefab = prefab;
            Mesh = mesh;
            Label = label;
            PreviewPosition = previewPosition;
        }
    }

    private static readonly NpcSpec[] Specs =
    {
        new NpcSpec("town_admin_photo_standee.png", "town_admin_photo_standee_back.png", "PF_NPC_TownAdmin_PhotoStandee.prefab", "TownAdmin_PhotoStandee.asset", "小镇管理员", new Vector3(-1.85f, 0f, 0f)),
        new NpcSpec("hotel_owner_photo_standee.png", "hotel_owner_photo_standee_back.png", "PF_NPC_HotelOwner_PhotoStandee.prefab", "HotelOwner_PhotoStandee.asset", "星湾旅店老板/前台", Vector3.zero),
        new NpcSpec("bakery_owner_photo_standee.png", "bakery_owner_photo_standee_back.png", "PF_NPC_BakeryOwner_PhotoStandee.prefab", "BakeryOwner_PhotoStandee.asset", "奶油星球蛋糕店老板", new Vector3(1.85f, 0f, 0f)),
    };

    public static void Run()
    {
        Directory.CreateDirectory(MeshDir);
        Directory.CreateDirectory(MaterialDir);
        Directory.CreateDirectory(PrefabDir);
        Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
        Directory.CreateDirectory(Path.GetDirectoryName(ScreenshotPath));

        ImportTextures();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        foreach (var spec in Specs)
        {
            BuildPrefab(spec);
        }

        BuildPreviewScene();
        CaptureScreenshot();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void ImportTextures()
    {
        foreach (var spec in Specs)
        {
            var texturePath = TextureDir + "/" + spec.Texture;
            ImportTransparentTexture(texturePath);
            ImportTransparentTexture(TextureDir + "/" + spec.BackTexture);
        }
    }

    private static void ImportTransparentTexture(string texturePath)
    {
        AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceSynchronousImport);
        var importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
        if (importer == null)
        {
            return;
        }

        importer.textureType = TextureImporterType.Default;
        importer.alphaSource = TextureImporterAlphaSource.FromInput;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.maxTextureSize = 2048;
        importer.SaveAndReimport();
    }

    private static void BuildPrefab(NpcSpec spec)
    {
        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(TextureDir + "/" + spec.Texture);
        var backTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(TextureDir + "/" + spec.BackTexture);
        if (texture == null || backTexture == null)
        {
            Debug.LogWarning("Missing photo standee texture: " + spec.Texture);
            return;
        }

        var targetHeight = 1.82f;
        var width = targetHeight * texture.width / Mathf.Max(1f, texture.height);
        var mesh = CreateTwoSidedQuad(width, targetHeight);
        var meshPath = MeshDir + "/" + spec.Mesh;
        AssetDatabase.DeleteAsset(meshPath);
        AssetDatabase.CreateAsset(mesh, meshPath);

        var frontMaterial = CreateMaterial(spec, texture, "_Front");
        var backMaterial = CreateMaterial(spec, backTexture, "_Back");

        var root = new GameObject(spec.Label);
        var visual = new GameObject("Visual");
        visual.transform.SetParent(root.transform, false);
        visual.transform.localPosition = new Vector3(0f, targetHeight * 0.5f, 0f);
        visual.AddComponent<MeshFilter>().sharedMesh = mesh;
        visual.AddComponent<MeshRenderer>().sharedMaterials = new[] { frontMaterial, backMaterial };

        var collider = root.AddComponent<CapsuleCollider>();
        collider.center = new Vector3(0f, targetHeight * 0.5f, 0f);
        collider.height = targetHeight;
        collider.radius = Mathf.Clamp(width * 0.28f, 0.18f, 0.36f);

        PrefabUtility.SaveAsPrefabAsset(root, PrefabDir + "/" + spec.Prefab);
        Object.DestroyImmediate(root);
    }

    private static Mesh CreateTwoSidedQuad(float width, float height)
    {
        var halfWidth = width * 0.5f;
        var mesh = new Mesh
        {
            name = "PhotoStandeeMesh"
        };

        mesh.vertices = new[]
        {
            new Vector3(-halfWidth, -height * 0.5f, -0.006f),
            new Vector3(halfWidth, -height * 0.5f, -0.006f),
            new Vector3(halfWidth, height * 0.5f, -0.006f),
            new Vector3(-halfWidth, height * 0.5f, -0.006f),
            new Vector3(-halfWidth, -height * 0.5f, 0.006f),
            new Vector3(halfWidth, -height * 0.5f, 0.006f),
            new Vector3(halfWidth, height * 0.5f, 0.006f),
            new Vector3(-halfWidth, height * 0.5f, 0.006f),
        };
        mesh.uv = new[]
        {
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(1f, 1f),
            new Vector2(0f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
        };
        mesh.subMeshCount = 2;
        mesh.SetTriangles(new[] { 0, 2, 1, 0, 3, 2 }, 0);
        mesh.SetTriangles(new[] { 4, 5, 6, 4, 6, 7 }, 1);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    private static Material CreateMaterial(NpcSpec spec, Texture2D texture, string suffix)
    {
        var materialPath = MaterialDir + "/" + Path.GetFileNameWithoutExtension(spec.Prefab) + suffix + "_Photo.mat";
        var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (material == null)
        {
            material = new Material(Shader.Find("Unlit/Transparent"));
            AssetDatabase.CreateAsset(material, materialPath);
        }

        material.shader = Shader.Find("Unlit/Transparent");
        material.color = Color.white;
        material.mainTexture = texture;
        material.renderQueue = 3000;
        return material;
    }

    private static void BuildPreviewScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        RenderSettings.ambientLight = Color.white;
        RenderSettings.fog = false;

        CreateCamera();
        CreateLight();
        CreateFloor();

        foreach (var spec in Specs)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabDir + "/" + spec.Prefab);
            if (prefab == null)
            {
                continue;
            }

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.transform.position = spec.PreviewPosition;
            instance.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            AddSceneLabel(spec.Label, spec.PreviewPosition + new Vector3(0f, 2.02f, 0f));
        }

        EditorSceneManager.SaveScene(scene, ScenePath);
    }

    private static void AddSceneLabel(string text, Vector3 position)
    {
        var label = new GameObject("Nameplate");
        label.transform.position = position;
        label.transform.rotation = Quaternion.identity;

        var mesh = label.AddComponent<TextMesh>();
        mesh.text = text;
        mesh.fontSize = 28;
        mesh.characterSize = 0.03f;
        mesh.anchor = TextAnchor.MiddleCenter;
        mesh.alignment = TextAlignment.Center;
        mesh.color = new Color(0.14f, 0.15f, 0.16f);
    }

    private static void CreateCamera()
    {
        var cameraObject = new GameObject("Preview Camera");
        var camera = cameraObject.AddComponent<Camera>();
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0f, 1.08f, -6.4f);
        cameraObject.transform.rotation = Quaternion.Euler(2f, 0f, 0f);
        camera.fieldOfView = 33f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.91f, 0.93f, 0.94f);
    }

    private static void CreateLight()
    {
        var lightObject = new GameObject("Preview Light");
        lightObject.transform.rotation = Quaternion.Euler(45f, -20f, 0f);
        var light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 0.6f;
        light.color = Color.white;
    }

    private static void CreateFloor()
    {
        var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Preview Floor";
        floor.transform.position = new Vector3(0f, -0.04f, 0.45f);
        floor.transform.localScale = new Vector3(6.9f, 0.035f, 1.35f);

        var material = new Material(Shader.Find("Unlit/Color"))
        {
            name = "MAT_PhotoStandeePreview_Floor",
            color = new Color(0.78f, 0.80f, 0.79f)
        };
        floor.GetComponent<Renderer>().sharedMaterial = material;
    }

    private static void CaptureScreenshot()
    {
        var camera = Camera.main;
        var renderTexture = new RenderTexture(1800, 1125, 24);
        camera.targetTexture = renderTexture;
        camera.Render();

        RenderTexture.active = renderTexture;
        var texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        File.WriteAllBytes(ScreenshotPath, texture.EncodeToPNG());

        camera.targetTexture = null;
        RenderTexture.active = null;
        Object.DestroyImmediate(renderTexture);
        Object.DestroyImmediate(texture);
    }
}
