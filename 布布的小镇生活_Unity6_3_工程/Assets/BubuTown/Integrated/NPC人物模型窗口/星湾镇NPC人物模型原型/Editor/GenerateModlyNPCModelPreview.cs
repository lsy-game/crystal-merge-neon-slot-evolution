using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class GenerateModlyNPCModelPreview
{
    private const string BaseDir = "Assets/StarBayTown/NPCPrototype";
    private const string ModelDir = BaseDir + "/Models/Generated";
    private const string ScenePath = BaseDir + "/Scenes/StarBayNPC_ModlyGeneratedPreview.unity";
    private const string ScreenshotPath = BaseDir + "/Docs/StarBayNPC_ModlyGeneratedPreview.png";

    public static void Run()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        RenderSettings.ambientLight = new Color(0.72f, 0.76f, 0.80f);
        RenderSettings.fog = false;

        CreateCamera();
        CreateLight("Key Light", new Vector3(-3.5f, 5.5f, -4.5f), new Vector3(55f, -35f, 0f), 2.2f);
        CreateLight("Fill Light", new Vector3(4.5f, 3.0f, -2.0f), new Vector3(45f, 35f, 0f), 0.7f);
        CreateFloor();

        SpawnModel("TownAdmin_Modly_FirstPass.obj", "Town Administrator", new Vector3(-2.2f, 0f, 0f));
        SpawnModel("HotelOwner_Modly_FirstPass.obj", "Hotel Owner / Reception", Vector3.zero);
        SpawnModel("BakeryOwner_Modly_FirstPass.obj", "Cream Planet Bakery Owner", new Vector3(2.2f, 0f, 0f));

        Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
        EditorSceneManager.SaveScene(scene, ScenePath);
        CaptureScreenshot();
        AssetDatabase.Refresh();
    }

    private static void SpawnModel(string fileName, string label, Vector3 position)
    {
        var assetPath = ModelDir + "/" + fileName;
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
        if (prefab == null)
        {
            Debug.LogWarning("Missing generated model: " + assetPath);
            return;
        }

        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.name = label;
        instance.transform.position = position;
        instance.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        NormalizeToHeight(instance, 1.85f);
        ApplyPreviewMaterial(instance);
        CreateLabel(label, position + new Vector3(0f, 0.04f, -0.72f));
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

        if (bounds.size.y > 0.001f)
        {
            var scale = targetHeight / bounds.size.y;
            root.transform.localScale *= scale;
        }

        renderers = root.GetComponentsInChildren<Renderer>();
        bounds = renderers[0].bounds;
        for (var i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        root.transform.position += new Vector3(0f, -bounds.min.y, 0f);
    }

    private static void ApplyPreviewMaterial(GameObject root)
    {
        var material = new Material(Shader.Find("Standard"))
        {
            name = "MAT_Modly_FirstPass_Clay",
            color = new Color(0.82f, 0.76f, 0.68f)
        };
        material.SetFloat("_Glossiness", 0.28f);

        foreach (var renderer in root.GetComponentsInChildren<Renderer>())
        {
            renderer.sharedMaterial = material;
        }
    }

    private static void CreateLabel(string text, Vector3 position)
    {
        var label = new GameObject(text + " Label");
        label.transform.position = position;
        label.transform.rotation = Quaternion.Euler(70f, 0f, 0f);
        var mesh = label.AddComponent<TextMesh>();
        mesh.text = text;
        mesh.fontSize = 40;
        mesh.characterSize = 0.045f;
        mesh.anchor = TextAnchor.MiddleCenter;
        mesh.alignment = TextAlignment.Center;
        mesh.color = new Color(0.18f, 0.19f, 0.20f);
    }

    private static void CreateCamera()
    {
        var cameraObject = new GameObject("Preview Camera");
        var camera = cameraObject.AddComponent<Camera>();
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0f, 1.35f, -5.4f);
        cameraObject.transform.rotation = Quaternion.Euler(10f, 0f, 0f);
        camera.fieldOfView = 34f;
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
        floor.transform.localScale = new Vector3(7.2f, 0.06f, 2.4f);
        var renderer = floor.GetComponent<Renderer>();
        renderer.sharedMaterial = new Material(Shader.Find("Standard"))
        {
            name = "MAT_Modly_Preview_Floor",
            color = new Color(0.78f, 0.81f, 0.80f)
        };
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

        Directory.CreateDirectory(Path.GetDirectoryName(ScreenshotPath));
        File.WriteAllBytes(ScreenshotPath, texture.EncodeToPNG());

        camera.targetTexture = null;
        RenderTexture.active = null;
        Object.DestroyImmediate(renderTexture);
        Object.DestroyImmediate(texture);
    }
}
