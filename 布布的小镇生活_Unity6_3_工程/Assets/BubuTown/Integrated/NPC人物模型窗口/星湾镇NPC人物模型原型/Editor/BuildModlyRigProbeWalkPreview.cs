using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class BuildModlyRigProbeWalkPreview
{
    private const string BaseDir = "Assets/StarBayTown/NPCPrototype";
    private const string ModelSuffix = "GameProbe";
    private const string SharedClipKey = "TownAdmin";
    private const string RiggedDir = BaseDir + "/Models/RiggedGameProbe";
    private const string ControllerDir = BaseDir + "/Animation/RiggedGameProbe";
    private const string ScenePath = BaseDir + "/Scenes/StarBayNPC_ModlyGameProbeSharedWalk.unity";
    private const string ScreenshotPath = BaseDir + "/Docs/StarBayNPC_ModlyGameProbeSharedWalkPreview.png";
    private const string DesktopDir = "/Users/zhendian/Desktop/星月湾小镇NPC可动性测试输出";

    private static readonly ProbeNpc[] Npcs =
    {
        new ProbeNpc("TownAdmin", "小镇管理员", new Color(0.63f, 0.77f, 0.92f), -1.72f, 0.00f),
        new ProbeNpc("HotelOwner", "旅店老板/前台", new Color(0.88f, 0.76f, 0.61f), 0.00f, Mathf.PI),
        new ProbeNpc("BakeryOwner", "蛋糕店老板", new Color(0.96f, 0.72f, 0.78f), 1.72f, Mathf.PI * 0.55f)
    };

    [MenuItem("StarBay/NPC/Build Modly Rig Probe Walk Preview")]
    public static void Run()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
        Directory.CreateDirectory(Path.GetDirectoryName(ScreenshotPath));
        Directory.CreateDirectory(ControllerDir);
        Directory.CreateDirectory(DesktopDir);

        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        ConfigureModelImporters();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.74f, 0.76f, 0.77f);
        RenderSettings.fog = false;

        CreateCamera();
        CreateLight("Key Light", new Vector3(-3.5f, 5.5f, -4.0f), new Vector3(50f, -32f, 0f), 1.12f);
        CreateLight("Soft Fill", new Vector3(3.8f, 3.0f, -3.4f), new Vector3(28f, 40f, 0f), 0.35f);
        CreateFloor();
        CreateTitle("Modly 静态单网格 -> 轻量 GameProbe + 共享 Humanoid Walk", new Vector3(0f, 2.32f, 0.02f), 0.026f);
        CreateSmallLabel("三个人共用同一个走路动画；这一步验证它能否成为后续方案", new Vector3(0f, 2.16f, 0.02f), 0.020f);

        foreach (var npc in Npcs)
        {
            BuildNpcColumn(npc);
        }

        EditorSceneManager.SaveScene(scene, ScenePath);
        CaptureScreenshot();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void ConfigureModelImporters()
    {
        foreach (var npc in Npcs)
        {
            var modelPath = GetModelPath(npc.Key);
            var importer = AssetImporter.GetAtPath(modelPath) as ModelImporter;
            if (importer == null)
            {
                Debug.LogWarning("Missing rig probe FBX: " + modelPath);
                continue;
            }

            importer.importAnimation = true;
            importer.animationType = ModelImporterAnimationType.Generic;
            importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
            importer.materialImportMode = ModelImporterMaterialImportMode.None;
            importer.importCameras = false;
            importer.importLights = false;
            importer.SaveAndReimport();

            AssetDatabase.ImportAsset(modelPath, ImportAssetOptions.ForceSynchronousImport);
            var genericPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
            var skeleton = BuildSkeleton(genericPrefab);
            if (skeleton.Length == 0)
            {
                Debug.LogWarning("Could not read skeleton from generic import: " + modelPath);
                continue;
            }

            importer.animationType = ModelImporterAnimationType.Human;
            importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
            importer.humanDescription = BuildHumanDescription(skeleton);
            ConfigureLoopingWalkClip(importer);
            importer.SaveAndReimport();
        }
    }

    private static void ConfigureLoopingWalkClip(ModelImporter importer)
    {
        var clips = importer.defaultClipAnimations;
        if (clips == null || clips.Length == 0)
        {
            return;
        }

        for (var i = 0; i < clips.Length; i++)
        {
            clips[i].name = "RigProbe_WalkInPlace";
            clips[i].loopTime = true;
            clips[i].wrapMode = WrapMode.Loop;
        }

        importer.clipAnimations = clips;
    }

    private static SkeletonBone[] BuildSkeleton(GameObject prefab)
    {
        if (prefab == null)
        {
            return Array.Empty<SkeletonBone>();
        }

        return prefab
            .GetComponentsInChildren<Transform>(true)
            .Select(transform => new SkeletonBone
            {
                name = transform.name,
                position = transform.localPosition,
                rotation = transform.localRotation,
                scale = transform.localScale
            })
            .ToArray();
    }

    private static HumanDescription BuildHumanDescription(SkeletonBone[] skeleton)
    {
        var skeletonNames = skeleton.Select(bone => bone.name).ToHashSet();
        var human = new[]
        {
            Map(HumanBodyBones.Hips, "Hips", skeletonNames),
            Map(HumanBodyBones.Spine, "Spine", skeletonNames),
            Map(HumanBodyBones.Chest, "Chest", skeletonNames),
            Map(HumanBodyBones.Neck, "Neck", skeletonNames),
            Map(HumanBodyBones.Head, "Head", skeletonNames),
            Map(HumanBodyBones.LeftUpperArm, "LeftUpperArm", skeletonNames),
            Map(HumanBodyBones.LeftLowerArm, "LeftLowerArm", skeletonNames),
            Map(HumanBodyBones.LeftHand, "LeftHand", skeletonNames),
            Map(HumanBodyBones.RightUpperArm, "RightUpperArm", skeletonNames),
            Map(HumanBodyBones.RightLowerArm, "RightLowerArm", skeletonNames),
            Map(HumanBodyBones.RightHand, "RightHand", skeletonNames),
            Map(HumanBodyBones.LeftUpperLeg, "LeftUpperLeg", skeletonNames),
            Map(HumanBodyBones.LeftLowerLeg, "LeftLowerLeg", skeletonNames),
            Map(HumanBodyBones.LeftFoot, "LeftFoot", skeletonNames),
            Map(HumanBodyBones.RightUpperLeg, "RightUpperLeg", skeletonNames),
            Map(HumanBodyBones.RightLowerLeg, "RightLowerLeg", skeletonNames),
            Map(HumanBodyBones.RightFoot, "RightFoot", skeletonNames),
        }.Where(bone => !string.IsNullOrEmpty(bone.boneName)).ToArray();

        return new HumanDescription
        {
            human = human,
            skeleton = skeleton,
            upperArmTwist = 0.5f,
            lowerArmTwist = 0.5f,
            upperLegTwist = 0.5f,
            lowerLegTwist = 0.5f,
            armStretch = 0.05f,
            legStretch = 0.05f,
            feetSpacing = 0.0f,
            hasTranslationDoF = false
        };
    }

    private static HumanBone Map(HumanBodyBones humanBone, string boneName, System.Collections.Generic.HashSet<string> skeletonNames)
    {
        if (!skeletonNames.Contains(boneName))
        {
            Debug.LogWarning("RigProbe human mapping missing bone: " + boneName);
            return default;
        }

        return new HumanBone
        {
            humanName = HumanTrait.BoneName[(int)humanBone],
            boneName = boneName,
            limit = new HumanLimit { useDefaultValues = true }
        };
    }

    private static void BuildNpcColumn(ProbeNpc npc)
    {
        var modelPath = GetModelPath(npc.Key);
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
        if (prefab == null)
        {
            Debug.LogWarning("Missing rig probe prefab: " + modelPath);
            return;
        }

        var root = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        root.name = npc.Key + "_RigProbe_WalkPose";
        root.transform.position = new Vector3(npc.X, 0f, 0.12f);
        root.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        AttachWalkController(root, npc.Key);

        var bounds = CalculateBounds(root);
        root.transform.position -= new Vector3(0f, bounds.min.y, 0f);
        ApplyWalkPose(root, npc.Phase);
        ApplyProbeMaterial(root, npc.Tint);
        AddCharacterController(root);

        var stats = GetStats(root);
        CreateTitle(npc.DisplayName, new Vector3(npc.X, 2.00f, 0.02f), 0.024f);
        CreateSmallLabel(stats, new Vector3(npc.X, 1.84f, 0.02f), 0.016f);
        CreateGroundLabel("共享动画可播放\n衣摆/手/头发仍需手工权重", new Vector3(npc.X, 0.13f, -0.80f));
        CreatePoseTrail(npc.X, npc.Tint);

        var animator = root.GetComponent<Animator>();
        if (animator != null && animator.avatar != null)
        {
            Debug.Log($"{npc.Key} Avatar isHuman={animator.avatar.isHuman} isValid={animator.avatar.isValid}");
        }
        else
        {
            Debug.Log($"{npc.Key} has no valid Animator avatar on imported prefab");
        }
    }

    private static void AttachWalkController(GameObject root, string key)
    {
        var sharedModelPath = GetModelPath(SharedClipKey);
        var clip = LoadWalkClip(sharedModelPath);
        if (clip == null)
        {
            Debug.LogWarning("No imported shared walk clip found for: " + sharedModelPath);
            return;
        }

        var controllerPath = $"{ControllerDir}/{key}_GameProbeSharedWalk.controller";
        AssetDatabase.DeleteAsset(controllerPath);
        var controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
        var stateMachine = controller.layers[0].stateMachine;
        var state = stateMachine.AddState("RigProbe_WalkInPlace");
        state.motion = clip;
        state.speed = 1.0f;
        stateMachine.defaultState = state;

        var animator = root.GetComponent<Animator>();
        if (animator == null)
        {
            animator = root.AddComponent<Animator>();
        }

        animator.runtimeAnimatorController = controller;
        animator.applyRootMotion = false;
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        Debug.Log($"{key} shared walk controller attached with clip {clip.name} from {SharedClipKey}");
    }

    private static AnimationClip LoadWalkClip(string modelPath)
    {
        return AssetDatabase
            .LoadAllAssetsAtPath(modelPath)
            .OfType<AnimationClip>()
            .FirstOrDefault(clip => !clip.name.StartsWith("__preview__", StringComparison.OrdinalIgnoreCase));
    }

    private static string GetModelPath(string key)
    {
        return $"{RiggedDir}/{key}_{ModelSuffix}.fbx";
    }

    private static void ApplyWalkPose(GameObject root, float phase)
    {
        var swing = Mathf.Sin(phase);
        var counter = Mathf.Sin(phase + Mathf.PI);

        Rotate(root, "Hips", new Vector3(0f, 0f, -2.0f * swing));
        Rotate(root, "Chest", new Vector3(0f, 0f, 2.5f * swing));
        Rotate(root, "LeftUpperLeg", new Vector3(18f * swing, 0f, 2.0f));
        Rotate(root, "LeftLowerLeg", new Vector3(-18f * Mathf.Max(0f, -swing), 0f, 0f));
        Rotate(root, "LeftFoot", new Vector3(8f * Mathf.Max(0f, swing), 0f, 0f));
        Rotate(root, "RightUpperLeg", new Vector3(18f * counter, 0f, -2.0f));
        Rotate(root, "RightLowerLeg", new Vector3(-18f * Mathf.Max(0f, -counter), 0f, 0f));
        Rotate(root, "RightFoot", new Vector3(8f * Mathf.Max(0f, counter), 0f, 0f));
        Rotate(root, "LeftUpperArm", new Vector3(15f * counter, 0f, -3f));
        Rotate(root, "LeftLowerArm", new Vector3(6f, 0f, 0f));
        Rotate(root, "RightUpperArm", new Vector3(15f * swing, 0f, 3f));
        Rotate(root, "RightLowerArm", new Vector3(6f, 0f, 0f));

        var hips = FindDeep(root.transform, "Hips");
        if (hips != null)
        {
            hips.localPosition += Vector3.up * (0.012f * Mathf.Abs(swing));
        }
    }

    private static void Rotate(GameObject root, string boneName, Vector3 euler)
    {
        var bone = FindDeep(root.transform, boneName);
        if (bone == null)
        {
            return;
        }

        bone.localRotation *= Quaternion.Euler(euler);
    }

    private static Transform FindDeep(Transform root, string name)
    {
        if (root.name == name)
        {
            return root;
        }

        foreach (Transform child in root)
        {
            var result = FindDeep(child, name);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    private static string GetStats(GameObject root)
    {
        var renderers = root.GetComponentsInChildren<SkinnedMeshRenderer>();
        var meshRenderers = root.GetComponentsInChildren<MeshRenderer>();
        var bones = renderers.SelectMany(renderer => renderer.bones).Where(bone => bone != null).Select(bone => bone.name).Distinct().Count();
        var verts = renderers.Sum(renderer => renderer.sharedMesh != null ? renderer.sharedMesh.vertexCount : 0);
        var avatar = root.GetComponent<Animator>()?.avatar;
        var avatarText = avatar != null ? $"Avatar {avatar.isHuman}/{avatar.isValid}" : "Avatar --";
        return $"SkinnedMeshRenderer={renderers.Length}  MeshRenderer={meshRenderers.Length}\nBones={bones}  Verts={verts:N0}  {avatarText}";
    }

    private static void ApplyProbeMaterial(GameObject root, Color tint)
    {
        var material = new Material(Shader.Find("Standard"))
        {
            name = root.name + "_RigProbeTint",
            color = tint
        };
        material.SetFloat("_Glossiness", 0.24f);
        material.SetFloat("_Metallic", 0.0f);

        foreach (var renderer in root.GetComponentsInChildren<Renderer>())
        {
            renderer.sharedMaterial = material;
        }
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
        controller.radius = Mathf.Clamp(Mathf.Max(bounds.size.x, bounds.size.z) * 0.24f, 0.16f, 0.34f);
        controller.stepOffset = 0.24f;
        controller.slopeLimit = 45f;
    }

    private static Bounds CalculateBounds(GameObject root)
    {
        var renderers = root.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            return new Bounds(root.transform.position + Vector3.up * 0.85f, new Vector3(0.5f, 1.7f, 0.5f));
        }

        var bounds = renderers[0].bounds;
        for (var i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        return bounds;
    }

    private static void CreatePoseTrail(float x, Color color)
    {
        var material = CreateMaterial("MAT_RigProbeTrail_" + x.ToString("0.0"), new Color(color.r, color.g, color.b, 0.82f));
        for (var i = 0; i < 4; i++)
        {
            var marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.name = "Walk Pose Marker";
            marker.transform.position = new Vector3(x - 0.28f + i * 0.19f, 0.025f, 0.78f);
            marker.transform.localScale = new Vector3(0.10f, 0.024f, 0.10f);
            marker.GetComponent<Renderer>().sharedMaterial = material;
            UnityEngine.Object.DestroyImmediate(marker.GetComponent<Collider>());
        }
    }

    private static void CreateCamera()
    {
        var cameraObject = new GameObject("Preview Camera");
        var camera = cameraObject.AddComponent<Camera>();
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0f, 1.14f, -6.9f);
        cameraObject.transform.rotation = Quaternion.Euler(4.5f, 0f, 0f);
        camera.orthographic = true;
        camera.orthographicSize = 1.64f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.90f, 0.93f, 0.94f);
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
        floor.name = "Rig Probe Floor";
        floor.transform.position = new Vector3(0f, -0.035f, 0.08f);
        floor.transform.localScale = new Vector3(6.2f, 0.07f, 2.65f);
        floor.GetComponent<Renderer>().sharedMaterial = CreateMaterial("MAT_RigProbeFloor", new Color(0.79f, 0.82f, 0.80f));
    }

    private static void CreateTitle(string text, Vector3 position, float size)
    {
        var label = new GameObject(text);
        label.transform.position = position;
        var mesh = label.AddComponent<TextMesh>();
        mesh.text = text;
        mesh.fontSize = 36;
        mesh.characterSize = size;
        mesh.anchor = TextAnchor.MiddleCenter;
        mesh.alignment = TextAlignment.Center;
        mesh.color = new Color(0.11f, 0.12f, 0.14f);
    }

    private static void CreateSmallLabel(string text, Vector3 position, float size)
    {
        var label = new GameObject(text);
        label.transform.position = position;
        var mesh = label.AddComponent<TextMesh>();
        mesh.text = text;
        mesh.fontSize = 30;
        mesh.characterSize = size;
        mesh.anchor = TextAnchor.MiddleCenter;
        mesh.alignment = TextAlignment.Center;
        mesh.color = new Color(0.21f, 0.22f, 0.24f);
    }

    private static void CreateGroundLabel(string text, Vector3 position)
    {
        var label = new GameObject(text);
        label.transform.position = position;
        label.transform.rotation = Quaternion.Euler(68f, 0f, 0f);
        var mesh = label.AddComponent<TextMesh>();
        mesh.text = text;
        mesh.fontSize = 30;
        mesh.characterSize = 0.021f;
        mesh.anchor = TextAnchor.MiddleCenter;
        mesh.alignment = TextAlignment.Center;
        mesh.color = new Color(0.16f, 0.17f, 0.19f);
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

        var bytes = texture.EncodeToPNG();
        File.WriteAllBytes(ScreenshotPath, bytes);
        File.WriteAllBytes(Path.Combine(DesktopDir, "StarBayNPC_ModlyGameProbeSharedWalkPreview.png"), bytes);

        camera.targetTexture = null;
        RenderTexture.active = null;
        UnityEngine.Object.DestroyImmediate(renderTexture);
        UnityEngine.Object.DestroyImmediate(texture);
    }

    private readonly struct ProbeNpc
    {
        public readonly string Key;
        public readonly string DisplayName;
        public readonly Color Tint;
        public readonly float X;
        public readonly float Phase;

        public ProbeNpc(string key, string displayName, Color tint, float x, float phase)
        {
            Key = key;
            DisplayName = displayName;
            Tint = tint;
            X = x;
            Phase = phase;
        }
    }
}
