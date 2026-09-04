using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class BuildModlyMovementFeasibilityPreview
{
    private const string BaseDir = "Assets/StarBayTown/NPCPrototype";
    private const string PrefabDir = BaseDir + "/Prefabs/TexturedProjection";
    private const string ScenePath = BaseDir + "/Scenes/StarBayNPC_ModlyMovementFeasibility.unity";
    private const string ScreenshotPath = BaseDir + "/Docs/StarBayNPC_ModlyMovementFeasibilityPreview.png";

    public static void Run()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
        Directory.CreateDirectory(Path.GetDirectoryName(ScreenshotPath));
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        RenderSettings.ambientLight = new Color(0.62f, 0.64f, 0.66f);
        RenderSettings.fog = false;

        CreateCamera();
        CreateLight("Key Light", new Vector3(-3.6f, 5.2f, -4.6f), new Vector3(48f, -32f, 0f), 1.05f);
        CreateLight("Soft Fill", new Vector3(3.8f, 2.8f, -3.2f), new Vector3(18f, 38f, 0f), 0.34f);
        CreateFloor();

        BuildStaticMeshLane();
        BuildRigTargetLane();
        BuildUnityEntryLane();

        EditorSceneManager.SaveScene(scene, ScenePath);
        CaptureScreenshot();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void BuildStaticMeshLane()
    {
        var npc = SpawnTexturedNpc("PF_NPC_TownAdmin_TexturedProjection.prefab", new Vector3(-2.35f, 0f, 0.2f), Quaternion.identity);
        if (npc != null)
        {
            npc.name = "ModlyStaticMesh_CanMoveAsRoot";
            AddCharacterController(npc);
            StripNameplates(npc);
        }

        CreateTitle("Modly 静态网格", new Vector3(-2.35f, 2.05f, 0.05f));
        CreateSmallLabel("外形更像角色\nskins=0 / animations=0", new Vector3(-2.35f, 1.82f, 0.05f));
        CreateGroundLabel("能整体位移、碰撞、转向\n不能自然摆臂迈腿", new Vector3(-2.35f, 0.04f, -0.92f), 0.032f);
        CreateRootMotionTrail(new Vector3(-2.92f, 0.025f, 0.86f), new Vector3(-1.78f, 0.025f, 0.86f));
    }

    private static void BuildRigTargetLane()
    {
        var ghost = SpawnTexturedNpc("PF_NPC_TownAdmin_TexturedProjection.prefab", new Vector3(0f, 0f, 0.24f), Quaternion.identity);
        if (ghost != null)
        {
            ghost.name = "ModlyMesh_AsRiggingSource";
            ghost.transform.localScale *= 0.92f;
            StripNameplates(ghost);
            TintRenderers(ghost, new Color(1.0f, 0.88f, 0.92f, 0.58f));
        }

        CreateHumanoidRigProxy(new Vector3(0f, 0f, 0.11f));
        CreateTitle("下一步：绑骨蒙皮", new Vector3(0f, 2.05f, 0.05f));
        CreateSmallLabel("Blender/AccuRIG/Mixamo 做骨架\n重点修手、裙摆、袖口、头发", new Vector3(0f, 1.82f, 0.05f));
        CreateGroundLabel("要接近参考图\n衣服要真厚度/分件网格", new Vector3(0f, 0.04f, -0.92f), 0.031f);
    }

    private static void BuildUnityEntryLane()
    {
        var npc = SpawnTexturedNpc("PF_NPC_BakeryOwner_TexturedProjection.prefab", new Vector3(2.35f, 0f, 0.2f), Quaternion.identity);
        if (npc != null)
        {
            npc.name = "UnityReadyChecklist_SourceMesh";
            StripNameplates(npc);
            AddCharacterController(npc);
            npc.AddComponent<Animator>();
        }

        CreateTitle("Unity 入场标准", new Vector3(2.35f, 2.05f, 0.05f));
        CreateSmallLabel("SkinnedMeshRenderer + Avatar\nIdle/Walk/Run 动作片段", new Vector3(2.35f, 1.82f, 0.05f));
        CreateChecklistBadge("骨骼", new Vector3(1.75f, 0.18f, -0.88f), new Color(0.94f, 0.72f, 0.78f));
        CreateChecklistBadge("蒙皮", new Vector3(2.15f, 0.18f, -0.88f), new Color(0.94f, 0.83f, 0.62f));
        CreateChecklistBadge("Avatar", new Vector3(2.58f, 0.18f, -0.88f), new Color(0.70f, 0.86f, 0.78f));
        CreateChecklistBadge("动画", new Vector3(3.02f, 0.18f, -0.88f), new Color(0.70f, 0.80f, 0.96f));
    }

    private static GameObject SpawnTexturedNpc(string prefabName, Vector3 position, Quaternion rotation)
    {
        var prefabPath = PrefabDir + "/" + prefabName;
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogWarning("Missing textured NPC prefab: " + prefabPath);
            return null;
        }

        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.transform.position = position;
        instance.transform.rotation = rotation;
        return instance;
    }

    private static void AddCharacterController(GameObject root)
    {
        var bounds = CalculateBounds(root);
        var controller = root.GetComponent<CharacterController>();
        if (controller == null)
        {
            controller = root.AddComponent<CharacterController>();
        }

        controller.center = root.transform.InverseTransformPoint(bounds.center);
        controller.height = Mathf.Max(1.0f, bounds.size.y);
        controller.radius = Mathf.Clamp(Mathf.Max(bounds.size.x, bounds.size.z) * 0.26f, 0.16f, 0.36f);
        controller.stepOffset = 0.28f;
        controller.slopeLimit = 45f;
    }

    private static Bounds CalculateBounds(GameObject root)
    {
        var renderers = root.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            return new Bounds(root.transform.position + Vector3.up * 0.9f, new Vector3(0.5f, 1.8f, 0.5f));
        }

        var bounds = renderers[0].bounds;
        for (var i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }
        return bounds;
    }

    private static void StripNameplates(GameObject root)
    {
        var children = root.GetComponentsInChildren<Transform>(true);
        foreach (var child in children)
        {
            if (child != root.transform && child.name.Contains("Nameplate"))
            {
                Object.DestroyImmediate(child.gameObject);
            }
        }
    }

    private static void TintRenderers(GameObject root, Color tint)
    {
        foreach (var renderer in root.GetComponentsInChildren<Renderer>())
        {
            var source = renderer.sharedMaterial;
            var material = source != null ? new Material(source) : new Material(Shader.Find("Standard"));
            material.name = renderer.name + "_RigSourceTint";
            material.color = tint;
            material.SetFloat("_Glossiness", 0.14f);
            renderer.sharedMaterial = material;
        }
    }

    private static void CreateHumanoidRigProxy(Vector3 offset)
    {
        var matBone = CreateMaterial("MAT_RigProxy_Bone", new Color(0.22f, 0.28f, 0.34f));
        var matJoint = CreateMaterial("MAT_RigProxy_Joint", new Color(0.98f, 0.66f, 0.72f));

        var hips = offset + new Vector3(0f, 0.92f, -0.18f);
        var spine = offset + new Vector3(0f, 1.23f, -0.18f);
        var chest = offset + new Vector3(0f, 1.43f, -0.18f);
        var neck = offset + new Vector3(0f, 1.55f, -0.18f);
        var head = offset + new Vector3(0f, 1.72f, -0.18f);

        CreateBone(hips, spine, 0.035f, matBone);
        CreateBone(spine, chest, 0.035f, matBone);
        CreateBone(chest, neck, 0.03f, matBone);
        CreateBone(neck, head, 0.028f, matBone);

        CreateLimb(offset + new Vector3(-0.20f, 1.38f, -0.18f), offset + new Vector3(-0.46f, 1.08f, -0.20f), offset + new Vector3(-0.40f, 0.78f, -0.16f), matBone, matJoint);
        CreateLimb(offset + new Vector3(0.20f, 1.38f, -0.18f), offset + new Vector3(0.45f, 1.08f, -0.20f), offset + new Vector3(0.39f, 0.78f, -0.16f), matBone, matJoint);
        CreateLimb(offset + new Vector3(-0.13f, 0.90f, -0.18f), offset + new Vector3(-0.23f, 0.52f, -0.05f), offset + new Vector3(-0.16f, 0.09f, -0.36f), matBone, matJoint);
        CreateLimb(offset + new Vector3(0.13f, 0.90f, -0.18f), offset + new Vector3(0.22f, 0.50f, -0.28f), offset + new Vector3(0.18f, 0.09f, 0.05f), matBone, matJoint);

        CreateJoint(hips, 0.055f, matJoint);
        CreateJoint(chest, 0.05f, matJoint);
        CreateJoint(head, 0.075f, matJoint);
    }

    private static void CreateLimb(Vector3 root, Vector3 mid, Vector3 end, Material boneMaterial, Material jointMaterial)
    {
        CreateBone(root, mid, 0.026f, boneMaterial);
        CreateBone(mid, end, 0.024f, boneMaterial);
        CreateJoint(root, 0.044f, jointMaterial);
        CreateJoint(mid, 0.040f, jointMaterial);
        CreateJoint(end, 0.036f, jointMaterial);
    }

    private static void CreateBone(Vector3 start, Vector3 end, float radius, Material material)
    {
        var delta = end - start;
        var bone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        bone.name = "Rig Bone";
        bone.transform.position = start + delta * 0.5f;
        bone.transform.rotation = Quaternion.FromToRotation(Vector3.up, delta.normalized);
        bone.transform.localScale = new Vector3(radius, delta.magnitude * 0.5f, radius);
        bone.GetComponent<Renderer>().sharedMaterial = material;
        Object.DestroyImmediate(bone.GetComponent<Collider>());
    }

    private static void CreateJoint(Vector3 position, float radius, Material material)
    {
        var joint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        joint.name = "Rig Joint";
        joint.transform.position = position;
        joint.transform.localScale = Vector3.one * radius;
        joint.GetComponent<Renderer>().sharedMaterial = material;
        Object.DestroyImmediate(joint.GetComponent<Collider>());
    }

    private static void CreateRootMotionTrail(Vector3 start, Vector3 end)
    {
        var material = CreateMaterial("MAT_RootMotionTrail", new Color(0.25f, 0.54f, 0.82f));
        CreateBone(start, end, 0.018f, material);

        for (var i = 0; i < 4; i++)
        {
            var t = i / 3f;
            var marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.name = "Root Motion Marker";
            marker.transform.position = Vector3.Lerp(start, end, t);
            marker.transform.localScale = new Vector3(0.11f, 0.025f, 0.11f);
            marker.GetComponent<Renderer>().sharedMaterial = material;
            Object.DestroyImmediate(marker.GetComponent<Collider>());
        }
    }

    private static void CreateChecklistBadge(string text, Vector3 position, Color color)
    {
        var badge = GameObject.CreatePrimitive(PrimitiveType.Cube);
        badge.name = text + " Badge";
        badge.transform.position = position;
        badge.transform.localScale = new Vector3(0.36f, 0.10f, 0.18f);
        badge.GetComponent<Renderer>().sharedMaterial = CreateMaterial("MAT_Badge_" + text, color);
        Object.DestroyImmediate(badge.GetComponent<Collider>());

        var label = new GameObject(text + " Label");
        label.transform.position = position + new Vector3(0f, 0.075f, -0.095f);
        label.transform.rotation = Quaternion.Euler(68f, 0f, 0f);
        var mesh = label.AddComponent<TextMesh>();
        mesh.text = text;
        mesh.fontSize = 34;
        mesh.characterSize = 0.021f;
        mesh.anchor = TextAnchor.MiddleCenter;
        mesh.alignment = TextAlignment.Center;
        mesh.color = new Color(0.10f, 0.12f, 0.14f);
    }

    private static void CreateTitle(string text, Vector3 position)
    {
        var label = new GameObject(text);
        label.transform.position = position;
        var mesh = label.AddComponent<TextMesh>();
        mesh.text = text;
        mesh.fontSize = 36;
        mesh.characterSize = 0.034f;
        mesh.anchor = TextAnchor.MiddleCenter;
        mesh.alignment = TextAlignment.Center;
        mesh.color = new Color(0.12f, 0.13f, 0.15f);
    }

    private static void CreateSmallLabel(string text, Vector3 position)
    {
        var label = new GameObject(text);
        label.transform.position = position;
        var mesh = label.AddComponent<TextMesh>();
        mesh.text = text;
        mesh.fontSize = 30;
        mesh.characterSize = 0.025f;
        mesh.anchor = TextAnchor.MiddleCenter;
        mesh.alignment = TextAlignment.Center;
        mesh.color = new Color(0.22f, 0.23f, 0.25f);
    }

    private static void CreateGroundLabel(string text, Vector3 position, float characterSize)
    {
        var label = new GameObject(text);
        label.transform.position = position;
        label.transform.rotation = Quaternion.Euler(68f, 0f, 0f);
        var mesh = label.AddComponent<TextMesh>();
        mesh.text = text;
        mesh.fontSize = 30;
        mesh.characterSize = characterSize;
        mesh.anchor = TextAnchor.MiddleCenter;
        mesh.alignment = TextAlignment.Center;
        mesh.color = new Color(0.17f, 0.18f, 0.20f);
    }

    private static void CreateCamera()
    {
        var cameraObject = new GameObject("Preview Camera");
        var camera = cameraObject.AddComponent<Camera>();
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0f, 1.28f, -7.55f);
        cameraObject.transform.rotation = Quaternion.Euler(6.0f, 0f, 0f);
        camera.fieldOfView = 38f;
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
        floor.transform.localScale = new Vector3(7.0f, 0.06f, 2.55f);
        floor.GetComponent<Renderer>().sharedMaterial = CreateMaterial("MAT_ModlyMovementPreview_Floor", new Color(0.80f, 0.82f, 0.80f));
    }

    private static Material CreateMaterial(string name, Color color)
    {
        var material = new Material(Shader.Find("Standard"))
        {
            name = name,
            color = color
        };
        material.SetFloat("_Glossiness", 0.18f);
        return material;
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
