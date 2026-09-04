using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class BuildOptimizedModlyNPCPrefabs
{
    private const string BaseDir = "Assets/StarBayTown/NPCPrototype";
    private const string ModelDir = BaseDir + "/Models/Optimized";
    private const string MaterialDir = BaseDir + "/Materials/GeneratedOptimized";
    private const string PrefabDir = BaseDir + "/Prefabs/GeneratedOptimized";
    private const string ScenePath = BaseDir + "/Scenes/StarBayNPC_OptimizedPrefabPreview.unity";
    private const string ScreenshotPath = BaseDir + "/Docs/StarBayNPC_OptimizedPrefabPreview.png";

    private struct NpcSpec
    {
        public string Model;
        public string Prefab;
        public string Label;
        public Color Color;
        public Vector3 PreviewPosition;

        public NpcSpec(string model, string prefab, string label, Color color, Vector3 previewPosition)
        {
            Model = model;
            Prefab = prefab;
            Label = label;
            Color = color;
            PreviewPosition = previewPosition;
        }
    }

    private static readonly NpcSpec[] Specs =
    {
        new NpcSpec("TownAdmin_Optimized.obj", "PF_NPC_TownAdmin_ModlyOptimized.prefab", "小镇管理员", new Color(0.62f, 0.78f, 0.92f), new Vector3(-2.2f, 0f, 0f)),
        new NpcSpec("HotelOwner_Optimized.obj", "PF_NPC_HotelOwner_ModlyOptimized.prefab", "星湾旅店老板/前台", new Color(0.76f, 0.58f, 0.40f), Vector3.zero),
        new NpcSpec("BakeryOwner_Optimized.obj", "PF_NPC_BakeryOwner_ModlyOptimized.prefab", "奶油星球蛋糕店老板", new Color(0.96f, 0.70f, 0.78f), new Vector3(2.2f, 0f, 0f)),
    };

    public static void Run()
    {
        Directory.CreateDirectory(MaterialDir);
        Directory.CreateDirectory(PrefabDir);
        Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
        Directory.CreateDirectory(Path.GetDirectoryName(ScreenshotPath));

        foreach (var spec in Specs)
        {
            BuildPrefab(spec);
        }

        BuildPreviewScene();
        CaptureScreenshot();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void BuildPrefab(NpcSpec spec)
    {
        var model = AssetDatabase.LoadAssetAtPath<GameObject>(ModelDir + "/" + spec.Model);
        if (model == null)
        {
            Debug.LogWarning("Missing optimized model: " + spec.Model);
            return;
        }

        var root = new GameObject(spec.Label);
        var visual = (GameObject)PrefabUtility.InstantiatePrefab(model);
        visual.name = "Visual";
        visual.transform.SetParent(root.transform, false);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localRotation = Quaternion.identity;

        NormalizeToHeight(root, 1.82f);
        AssignMaterial(root, spec);
        AddCollider(root);
        AddNameplate(root, spec.Label);

        PrefabUtility.SaveAsPrefabAsset(root, PrefabDir + "/" + spec.Prefab);
        Object.DestroyImmediate(root);
    }

    private static void BuildPreviewScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        RenderSettings.ambientLight = new Color(0.42f, 0.45f, 0.48f);
        RenderSettings.fog = false;

        CreateCamera();
        CreateLight("Key Light", new Vector3(-3.5f, 5.5f, -4.5f), new Vector3(55f, -35f, 0f), 1.15f);
        CreateLight("Fill Light", new Vector3(4.5f, 3.2f, -2.5f), new Vector3(45f, 35f, 0f), 0.35f);
        CreateFloor();

        foreach (var spec in Specs)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabDir + "/" + spec.Prefab);
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.transform.position = spec.PreviewPosition;
            instance.transform.rotation = Quaternion.identity;
        }

        EditorSceneManager.SaveScene(scene, ScenePath);
    }

    private static void NormalizeToHeight(GameObject root, float targetHeight)
    {
        var renderers = root.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            return;
        }

        var bounds = renderers[0].bounds;
        for (var i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        var height = Mathf.Max(0.001f, bounds.size.y);
        root.transform.localScale *= targetHeight / height;
        AlignFeetToGround(root);
    }

    private static void AlignFeetToGround(GameObject root)
    {
        var renderers = root.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            return;
        }

        var bounds = renderers[0].bounds;
        for (var i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }
        root.transform.position += new Vector3(0f, -bounds.min.y, 0f);
    }

    private static void AssignMaterial(GameObject root, NpcSpec spec)
    {
        var materialPath = MaterialDir + "/" + Path.GetFileNameWithoutExtension(spec.Prefab) + "_Body.mat";
        var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (material == null)
        {
            material = new Material(Shader.Find("Standard"));
            AssetDatabase.CreateAsset(material, materialPath);
        }

        material.name = Path.GetFileNameWithoutExtension(materialPath);
        material.color = spec.Color;
        material.SetFloat("_Glossiness", 0.34f);
        material.SetFloat("_Metallic", 0.02f);

        foreach (var renderer in root.GetComponentsInChildren<Renderer>())
        {
            if (renderer is MeshRenderer)
            {
                renderer.sharedMaterial = material;
            }
        }
    }

    private static void AddCollider(GameObject root)
    {
        var renderers = root.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            return;
        }

        var bounds = renderers[0].bounds;
        for (var i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        var collider = root.AddComponent<CapsuleCollider>();
        collider.center = root.transform.InverseTransformPoint(bounds.center);
        collider.height = Mathf.Max(1.0f, bounds.size.y);
        collider.radius = Mathf.Clamp(Mathf.Max(bounds.size.x, bounds.size.z) * 0.28f, 0.18f, 0.42f);
    }

    private static void AddNameplate(GameObject root, string text)
    {
        var label = new GameObject("Nameplate");
        label.transform.SetParent(root.transform, false);
        label.transform.localPosition = new Vector3(0f, 1.98f, 0f);
        label.transform.localRotation = Quaternion.identity;

        var mesh = label.AddComponent<TextMesh>();
        mesh.text = text;
        mesh.fontSize = 30;
        mesh.characterSize = 0.032f;
        mesh.anchor = TextAnchor.MiddleCenter;
        mesh.alignment = TextAlignment.Center;
        mesh.color = new Color(0.16f, 0.17f, 0.18f);
    }

    private static void CreateCamera()
    {
        var cameraObject = new GameObject("Preview Camera");
        var camera = cameraObject.AddComponent<Camera>();
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0f, 1.20f, -6.4f);
        cameraObject.transform.rotation = Quaternion.Euler(7f, 0f, 0f);
        camera.fieldOfView = 31f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.92f, 0.94f, 0.95f);
    }

    private static void CreateLight(string name, Vector3 position, Vector3 rotation, float intensity)
    {
        var lightObject = new GameObject(name);
        lightObject.transform.position = position;
        lightObject.transform.rotation = Quaternion.Euler(rotation);
        var light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = intensity;
        light.color = Color.white;
    }

    private static void CreateFloor()
    {
        var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Preview Floor";
        floor.transform.position = new Vector3(0f, -0.03f, 0.08f);
        floor.transform.localScale = new Vector3(7.3f, 0.06f, 2.4f);

        var material = new Material(Shader.Find("Standard"))
        {
            name = "MAT_OptimizedPreview_Floor",
            color = new Color(0.78f, 0.81f, 0.80f)
        };
        floor.GetComponent<Renderer>().sharedMaterial = material;
    }

    private static void CaptureScreenshot()
    {
        var camera = Camera.main;
        var renderTexture = new RenderTexture(1600, 1000, 24);
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
