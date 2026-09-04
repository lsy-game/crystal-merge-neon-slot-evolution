using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class BuildTexturedProjectionNPCPrefabs
{
    private const string BaseDir = "Assets/StarBayTown/NPCPrototype";
    private const string ModelDir = BaseDir + "/Models/TexturedProjection";
    private const string MaterialDir = BaseDir + "/Materials/TexturedProjection";
    private const string PrefabDir = BaseDir + "/Prefabs/TexturedProjection";
    private const string ScenePath = BaseDir + "/Scenes/StarBayNPC_TexturedProjectionPreview.unity";
    private const string ScreenshotPath = BaseDir + "/Docs/StarBayNPC_TexturedProjectionPreview.png";

    private struct NpcSpec
    {
        public string Model;
        public string Texture;
        public string Prefab;
        public string Label;
        public Vector3 PreviewPosition;

        public NpcSpec(string model, string texture, string prefab, string label, Vector3 previewPosition)
        {
            Model = model;
            Texture = texture;
            Prefab = prefab;
            Label = label;
            PreviewPosition = previewPosition;
        }
    }

    private static readonly NpcSpec[] Specs =
    {
        new NpcSpec("TownAdmin_TexturedProjection.obj", "town_admin_front.png", "PF_NPC_TownAdmin_TexturedProjection.prefab", "小镇管理员", new Vector3(-2.25f, 0f, 0f)),
        new NpcSpec("HotelOwner_TexturedProjection.obj", "hotel_owner_front.png", "PF_NPC_HotelOwner_TexturedProjection.prefab", "星湾旅店老板/前台", Vector3.zero),
        new NpcSpec("BakeryOwner_TexturedProjection.obj", "bakery_owner_front.png", "PF_NPC_BakeryOwner_TexturedProjection.prefab", "奶油星球蛋糕店老板", new Vector3(2.25f, 0f, 0f)),
    };

    public static void Run()
    {
        Directory.CreateDirectory(MaterialDir);
        Directory.CreateDirectory(PrefabDir);
        Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
        Directory.CreateDirectory(Path.GetDirectoryName(ScreenshotPath));

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

    private static void BuildPrefab(NpcSpec spec)
    {
        var modelPath = ModelDir + "/" + spec.Model;
        var model = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
        if (model == null)
        {
            Debug.LogWarning("Missing textured projection model: " + modelPath);
            return;
        }

        var root = new GameObject(spec.Label);
        var visual = (GameObject)PrefabUtility.InstantiatePrefab(model);
        visual.name = "Visual";
        visual.transform.SetParent(root.transform, false);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localRotation = Quaternion.identity;

        NormalizeToHeight(root, 1.82f);
        AssignTexturedMaterial(root, spec);
        AddCollider(root);
        AddNameplate(root, spec.Label);

        PrefabUtility.SaveAsPrefabAsset(root, PrefabDir + "/" + spec.Prefab);
        Object.DestroyImmediate(root);
    }

    private static void AssignTexturedMaterial(GameObject root, NpcSpec spec)
    {
        var materialPath = MaterialDir + "/" + Path.GetFileNameWithoutExtension(spec.Prefab) + "_Projected.mat";
        var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (material == null)
        {
            material = new Material(Shader.Find("Standard"));
            AssetDatabase.CreateAsset(material, materialPath);
        }

        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(ModelDir + "/" + spec.Texture);
        material.name = Path.GetFileNameWithoutExtension(materialPath);
        material.color = Color.white;
        material.mainTexture = texture;
        material.SetFloat("_Glossiness", 0.18f);
        material.SetFloat("_Metallic", 0.0f);

        foreach (var renderer in root.GetComponentsInChildren<Renderer>())
        {
            if (renderer is MeshRenderer)
            {
                renderer.sharedMaterial = material;
            }
        }
    }

    private static void BuildPreviewScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        RenderSettings.ambientLight = new Color(0.58f, 0.60f, 0.62f);
        RenderSettings.fog = false;

        CreateCamera();
        CreateLight("Key Light", new Vector3(-3.2f, 5.3f, -4.2f), new Vector3(48f, -34f, 0f), 0.92f);
        CreateLight("Soft Front Light", new Vector3(0f, 2.1f, -3.5f), new Vector3(8f, 0f, 0f), 0.22f);
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
        label.transform.localPosition = new Vector3(0f, 2.0f, 0f);
        label.transform.localRotation = Quaternion.identity;

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
        cameraObject.transform.position = new Vector3(0f, 1.18f, -6.65f);
        cameraObject.transform.rotation = Quaternion.Euler(6.5f, 0f, 0f);
        camera.fieldOfView = 29.5f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.91f, 0.93f, 0.94f);
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
        floor.transform.localScale = new Vector3(7.4f, 0.06f, 2.4f);

        var material = new Material(Shader.Find("Standard"))
        {
            name = "MAT_TexturedProjectionPreview_Floor",
            color = new Color(0.80f, 0.82f, 0.80f)
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
