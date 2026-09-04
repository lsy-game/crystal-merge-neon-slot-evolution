using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class GenerateStarBayNPCPreview
{
    private const string Root = "Assets/StarBayTown/NPCPrototype";
    private const string PrefabDir = Root + "/Prefabs";
    private const string MaterialDir = Root + "/Materials";
    private const string SceneDir = Root + "/Scenes";
    private const string DocsDir = Root + "/Docs";

    [MenuItem("StarBayTown/Generate NPC Preview")]
    public static void Generate()
    {
        Directory.CreateDirectory(PrefabDir);
        Directory.CreateDirectory(MaterialDir);
        Directory.CreateDirectory(SceneDir);
        Directory.CreateDirectory(DocsDir);

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        RenderSettings.ambientLight = new Color(0.78f, 0.82f, 0.86f);
        RenderSettings.skybox = null;

        var root = new GameObject("StarBay_NPC_FirstBatch_Preview");
        CreateFloor();
        CreateBackdrop();

        CreateNpc(new NpcSpec
        {
            id = "TownAdministrator",
            displayName = "小镇管理员",
            position = new Vector3(-2.5f, 0, 0),
            hair = new Color(0.22f, 0.16f, 0.12f),
            skin = new Color(1.0f, 0.80f, 0.68f),
            top = new Color(0.43f, 0.74f, 0.82f),
            jacket = new Color(0.95f, 0.97f, 0.92f),
            skirt = new Color(0.22f, 0.36f, 0.46f),
            accent = new Color(1.0f, 0.77f, 0.25f),
            shoes = new Color(0.95f, 0.95f, 0.9f),
            roleTag = "Welcome / Quest",
            accessory = Accessory.BadgeAndClipboard
        }, root.transform);

        CreateNpc(new NpcSpec
        {
            id = "StarBayHotelReception",
            displayName = "星湾旅店前台",
            position = new Vector3(0, 0, 0),
            hair = new Color(0.32f, 0.20f, 0.12f),
            skin = new Color(1.0f, 0.79f, 0.66f),
            top = new Color(0.89f, 0.73f, 0.55f),
            jacket = new Color(0.51f, 0.34f, 0.20f),
            skirt = new Color(0.24f, 0.22f, 0.20f),
            accent = new Color(0.93f, 0.47f, 0.42f),
            shoes = new Color(0.22f, 0.16f, 0.12f),
            roleTag = "Hotel / Sleep",
            accessory = Accessory.ScarfAndKey
        }, root.transform);

        CreateNpc(new NpcSpec
        {
            id = "CreamPlanetBakeryOwner",
            displayName = "奶油星球老板",
            position = new Vector3(2.5f, 0, 0),
            hair = new Color(0.42f, 0.25f, 0.16f),
            skin = new Color(1.0f, 0.81f, 0.68f),
            top = new Color(1.0f, 0.67f, 0.72f),
            jacket = new Color(1.0f, 0.93f, 0.86f),
            skirt = new Color(0.65f, 0.38f, 0.36f),
            accent = new Color(0.99f, 0.86f, 0.28f),
            shoes = new Color(0.58f, 0.34f, 0.28f),
            roleTag = "Bakery / Part-time",
            accessory = Accessory.ApronAndCake
        }, root.transform);

        CreateLightingAndCamera();

        EditorSceneManager.SaveScene(scene, SceneDir + "/StarBayNPC_FirstBatchPreview.unity");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        CapturePreview();
    }

    private static void CreateNpc(NpcSpec spec, Transform parent)
    {
        var npc = new GameObject("NPC_" + spec.id + "_" + spec.displayName);
        npc.transform.SetParent(parent);
        npc.transform.position = spec.position;

        var skin = Mat(spec.id + "_Skin", spec.skin);
        var hair = Mat(spec.id + "_Hair", spec.hair);
        var top = Mat(spec.id + "_Top", spec.top);
        var jacket = Mat(spec.id + "_Jacket", spec.jacket);
        var skirt = Mat(spec.id + "_Bottom", spec.skirt);
        var accent = Mat(spec.id + "_Accent", spec.accent);
        var shoes = Mat(spec.id + "_Shoes", spec.shoes);
        var glass = Mat(spec.id + "_SoftGlass", new Color(0.72f, 0.9f, 1.0f, 0.38f));
        glass.SetFloat("_Mode", 3);
        glass.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        glass.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        glass.SetInt("_ZWrite", 0);
        glass.DisableKeyword("_ALPHATEST_ON");
        glass.EnableKeyword("_ALPHABLEND_ON");
        glass.renderQueue = 3000;

        AddCube(npc.transform, "Torso", new Vector3(0, 0.95f, 0), new Vector3(0.50f, 0.72f, 0.30f), top);
        AddSphere(npc.transform, "Soft_Shoulder", new Vector3(0, 1.25f, 0), new Vector3(0.54f, 0.18f, 0.32f), top);
        AddCube(npc.transform, "Jacket_Block", new Vector3(0, 0.93f, -0.17f), new Vector3(0.60f, 0.58f, 0.08f), jacket);
        AddCube(npc.transform, "Collar", new Vector3(0, 1.30f, -0.19f), new Vector3(0.42f, 0.07f, 0.08f), accent);
        AddCapsule(npc.transform, "Neck", new Vector3(0, 1.35f, 0), new Vector3(0.12f, 0.18f, 0.12f), skin);
        AddSphere(npc.transform, "Head", new Vector3(0, 1.55f, 0), new Vector3(0.34f, 0.38f, 0.32f), skin);
        AddSphere(npc.transform, "Hair_Cap", new Vector3(0, 1.67f, -0.02f), new Vector3(0.37f, 0.25f, 0.34f), hair);
        AddSphere(npc.transform, "Back_Hair", new Vector3(0, 1.48f, 0.12f), new Vector3(0.40f, 0.38f, 0.18f), hair);

        AddCapsule(npc.transform, "Left_Arm", new Vector3(-0.38f, 1.0f, 0), new Vector3(0.13f, 0.58f, 0.13f), skin, new Vector3(0, 0, -10));
        AddCapsule(npc.transform, "Right_Arm", new Vector3(0.38f, 1.0f, 0), new Vector3(0.13f, 0.58f, 0.13f), skin, new Vector3(0, 0, 10));
        AddCube(npc.transform, "Bottom", new Vector3(0, 0.55f, 0), new Vector3(0.56f, 0.30f, 0.32f), skirt);
        AddCapsule(npc.transform, "Left_Leg", new Vector3(-0.16f, 0.23f, 0), new Vector3(0.13f, 0.45f, 0.13f), skin);
        AddCapsule(npc.transform, "Right_Leg", new Vector3(0.16f, 0.23f, 0), new Vector3(0.13f, 0.45f, 0.13f), skin);
        AddCube(npc.transform, "Left_Shoe", new Vector3(-0.16f, 0.02f, -0.05f), new Vector3(0.18f, 0.08f, 0.28f), shoes);
        AddCube(npc.transform, "Right_Shoe", new Vector3(0.16f, 0.02f, -0.05f), new Vector3(0.18f, 0.08f, 0.28f), shoes);

        AddFace(npc.transform, hair);
        AddAccessory(npc.transform, spec, accent, jacket, glass);
        AddLabel(npc.transform, spec.displayName, spec.roleTag);

        var collider = npc.AddComponent<CapsuleCollider>();
        collider.center = new Vector3(0, 0.82f, 0);
        collider.height = 1.72f;
        collider.radius = 0.34f;

        PrefabUtility.SaveAsPrefabAsset(npc, PrefabDir + "/PF_NPC_" + spec.id + ".prefab");
    }

    private static void AddAccessory(Transform parent, NpcSpec spec, Material accent, Material jacket, Material glass)
    {
        switch (spec.accessory)
        {
            case Accessory.BadgeAndClipboard:
                AddCube(parent, "Town_Badge", new Vector3(-0.22f, 1.14f, -0.22f), new Vector3(0.13f, 0.13f, 0.03f), accent);
                AddCube(parent, "Clipboard", new Vector3(0.48f, 0.88f, -0.13f), new Vector3(0.22f, 0.32f, 0.04f), jacket);
                break;
            case Accessory.ScarfAndKey:
                AddCube(parent, "Warm_Scarf", new Vector3(0, 1.27f, -0.24f), new Vector3(0.48f, 0.12f, 0.07f), accent);
                AddCube(parent, "Room_Key_Tag", new Vector3(0.34f, 0.76f, -0.22f), new Vector3(0.10f, 0.16f, 0.03f), accent);
                break;
            case Accessory.ApronAndCake:
                AddCube(parent, "Cream_Apron", new Vector3(0, 0.86f, -0.23f), new Vector3(0.44f, 0.62f, 0.05f), jacket);
                AddSphere(parent, "Cake_Tray", new Vector3(0.48f, 0.82f, -0.12f), new Vector3(0.22f, 0.05f, 0.22f), accent);
                AddSphere(parent, "Cake_Dome", new Vector3(0.48f, 0.90f, -0.12f), new Vector3(0.18f, 0.08f, 0.18f), glass);
                break;
        }
    }

    private static void AddFace(Transform parent, Material dark)
    {
        AddSphere(parent, "Left_Eye", new Vector3(-0.095f, 1.58f, -0.29f), new Vector3(0.035f, 0.045f, 0.018f), dark);
        AddSphere(parent, "Right_Eye", new Vector3(0.095f, 1.58f, -0.29f), new Vector3(0.035f, 0.045f, 0.018f), dark);
        AddCube(parent, "Soft_Smile", new Vector3(0, 1.48f, -0.305f), new Vector3(0.12f, 0.018f, 0.018f), dark);
    }

    private static void AddLabel(Transform parent, string title, string tag)
    {
        var titleObject = new GameObject("Label_" + title);
        titleObject.transform.SetParent(parent);
        titleObject.transform.localPosition = new Vector3(0, 2.03f, -0.18f);
        titleObject.transform.localRotation = Quaternion.identity;
        var text = titleObject.AddComponent<TextMesh>();
        text.text = title;
        text.anchor = TextAnchor.MiddleCenter;
        text.alignment = TextAlignment.Center;
        text.characterSize = 0.026f;
        text.fontSize = 24;
        text.color = new Color(0.16f, 0.20f, 0.22f);
    }

    private static void CreateFloor()
    {
        var floorMat = Mat("MAT_StarBay_NPCPreview_Floor", new Color(0.82f, 0.82f, 0.76f));
        AddCube(null, "Preview_Floor", new Vector3(0, -0.045f, 0), new Vector3(7.5f, 0.08f, 3.5f), floorMat);
        for (var x = -3; x <= 3; x++)
            AddCube(null, "Tile_Line_X_" + x, new Vector3(x, 0.002f, 0), new Vector3(0.015f, 0.01f, 3.5f), Mat("MAT_TileLine", new Color(0.68f, 0.68f, 0.62f)));
        for (var z = -1; z <= 1; z++)
            AddCube(null, "Tile_Line_Z_" + z, new Vector3(0, 0.004f, z), new Vector3(7.5f, 0.01f, 0.015f), Mat("MAT_TileLine", new Color(0.68f, 0.68f, 0.62f)));
    }

    private static void CreateBackdrop()
    {
        AddCube(null, "Warm_White_Backdrop", new Vector3(0, 1.25f, 1.1f), new Vector3(7.5f, 2.5f, 0.12f), Mat("MAT_StarBay_NPCPreview_Backdrop", new Color(0.93f, 0.92f, 0.86f)));
        AddCube(null, "Sea_Blue_Header", new Vector3(0, 2.35f, 1.02f), new Vector3(7.5f, 0.36f, 0.14f), Mat("MAT_StarBay_NPCPreview_SeaBlue", new Color(0.20f, 0.68f, 0.82f)));
    }

    private static void CreateLightingAndCamera()
    {
        var sun = new GameObject("Sun_Key_Light").AddComponent<Light>();
        sun.type = LightType.Directional;
        sun.intensity = 1.15f;
        sun.transform.rotation = Quaternion.Euler(45, -30, 0);

        var fill = new GameObject("Warm_Fill_Light").AddComponent<Light>();
        fill.type = LightType.Point;
        fill.intensity = 1.4f;
        fill.range = 8f;
        fill.transform.position = new Vector3(0, 2.8f, -2.2f);

        var cameraObject = new GameObject("Preview_Camera");
        var camera = cameraObject.AddComponent<Camera>();
        camera.transform.position = new Vector3(0, 1.18f, -7.0f);
        camera.transform.rotation = Quaternion.Euler(6, 0, 0);
        camera.fieldOfView = 32;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.82f, 0.92f, 0.98f);
        cameraObject.tag = "MainCamera";
    }

    private static void CapturePreview()
    {
        var camera = Object.FindObjectOfType<Camera>();
        var texture = new RenderTexture(1800, 1100, 24);
        camera.targetTexture = texture;
        camera.Render();
        RenderTexture.active = texture;
        var image = new Texture2D(texture.width, texture.height, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        image.Apply();
        File.WriteAllBytes(DocsDir + "/StarBayNPC_FirstBatchPreview.png", image.EncodeToPNG());
        camera.targetTexture = null;
        RenderTexture.active = null;
        Object.DestroyImmediate(texture);
        Object.DestroyImmediate(image);
    }

    private static GameObject AddCube(Transform parent, string name, Vector3 position, Vector3 scale, Material material)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        if (parent != null) go.transform.SetParent(parent);
        go.transform.localPosition = position;
        go.transform.localScale = scale;
        go.GetComponent<Renderer>().sharedMaterial = material;
        return go;
    }

    private static GameObject AddSphere(Transform parent, string name, Vector3 position, Vector3 scale, Material material)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = name;
        if (parent != null) go.transform.SetParent(parent);
        go.transform.localPosition = position;
        go.transform.localScale = scale;
        go.GetComponent<Renderer>().sharedMaterial = material;
        return go;
    }

    private static GameObject AddCapsule(Transform parent, string name, Vector3 position, Vector3 scale, Material material, Vector3? rotation = null)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        go.name = name;
        if (parent != null) go.transform.SetParent(parent);
        go.transform.localPosition = position;
        go.transform.localScale = scale;
        go.transform.localRotation = Quaternion.Euler(rotation ?? Vector3.zero);
        go.GetComponent<Renderer>().sharedMaterial = material;
        return go;
    }

    private static Material Mat(string name, Color color)
    {
        var path = MaterialDir + "/" + name + ".mat";
        var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            mat = new Material(Shader.Find("Standard"));
            AssetDatabase.CreateAsset(mat, path);
        }
        mat.color = color;
        return mat;
    }

    private struct NpcSpec
    {
        public string id;
        public string displayName;
        public string roleTag;
        public Vector3 position;
        public Color hair;
        public Color skin;
        public Color top;
        public Color jacket;
        public Color skirt;
        public Color accent;
        public Color shoes;
        public Accessory accessory;
    }

    private enum Accessory
    {
        BadgeAndClipboard,
        ScarfAndKey,
        ApronAndCake
    }
}
