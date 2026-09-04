using System.IO;
using System.Reflection;
using BubuTown;
using UniGLTF;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRM;

namespace BubuTown.EditorTools
{
    public static class BubuTownVrmDaMoguCandidateImporter
    {
        private const string ScenePath = "Assets/Scenes/BubuTownPrototype.unity";
        private const string PlayerPath = "BubuTown_Prototype_All_Visible_Before_Play/08_Player_And_Runtime/Player_Start_Bubu";
        private const string ProceduralVisualName = "DaMogu_C_Anime_ThirdPerson_Prototype";
        private const string RuntimeObjectName = "DaMogu_VRM_Candidate_Runtime";
        private const string CandidateMarkerName = "DaMogu_VRM_Candidate_A_AvatarSample_A";
        private const string CandidateVrmPath = "Assets/BubuTown/Characters/Prototype/VRoidSamples/DaMogu_VRM_Candidate_A_AvatarSample_A.vrm";
        private const string CandidatePrefabPath = "Assets/BubuTown/Characters/Prototype/VRoidSamples/DaMogu_VRM_Candidate_A_AvatarSample_A.prefab";
        private const string CandidateTextureFolder = "Assets/BubuTown/Characters/Prototype/VRoidSamples/DaMogu_VRM_Candidate_A_AvatarSample_A.Textures";
        private const string CandidateUrpMaterialFolder = "Assets/BubuTown/Characters/Prototype/VRoidSamples/DaMogu_VRM_Candidate_A_URP_Materials";
        private const string HoodiePinkOverridePath = "Assets/BubuTown/Characters/Prototype/VRoidSamples/DaMogu_VRM_Candidate_A.Textures_Overrides_hoodie_pink.png";
        private const string ControllerPath = "Assets/BubuTown/Characters/Prototype/DaMogu_Locomotion/Bubu_DaMogu_Locomotion.controller";
        private const string PrototypeMaterialFolder = "Assets/BubuTown/Characters/Prototype/DaMogu_Locomotion/Materials";
        private const string ReportPath = "Assets/BubuTown/Docs/DaMoguVrmCandidateIntegrationReport.md";
        private const string PreviewPath = "Assets/BubuTown/Docs/DaMoguVrmCandidatePreview.png";
        private const string DesktopPreviewPath = "/Users/zhendian/Desktop/星湾镇_大蘑菇_VRM候选A_游戏内预览.png";
        private const string DesktopWalkFramesFolder = "/Users/zhendian/Desktop/星湾镇_大蘑菇_VRM候选A_Walk动画帧";
        private const string DesktopRunFramesFolder = "/Users/zhendian/Desktop/星湾镇_大蘑菇_VRM候选A_Run动画帧";
        private const string TargetStyleAddonsName = "DaMogu_TargetStyle_Addons_v1";

        [MenuItem("BubuTown/Install DaMogu VRM Candidate A")]
        public static void InstallDaMoguVrmCandidateA()
        {
            EnsureAsset(CandidateVrmPath);
            EnsureAsset(ControllerPath);
            AssetDatabase.ImportAsset(ControllerPath, ImportAssetOptions.ForceUpdate);

            EnsureCandidatePrefab();
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(CandidatePrefabPath);
            if (prefab == null)
            {
                throw new System.Exception("[BubuTown] VRM candidate prefab was not created: " + CandidatePrefabPath);
            }

            var controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(ControllerPath);
            if (controller == null)
            {
                throw new System.Exception("[BubuTown] Missing locomotion Animator Controller: " + ControllerPath);
            }

            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var player = GameObject.Find(PlayerPath);
            if (player == null)
            {
                throw new System.Exception("[BubuTown] Missing player root: " + PlayerPath);
            }

            RemoveChild(player.transform, RuntimeObjectName);
            RemoveChild(player.transform, CandidateMarkerName);
            RemoveChild(player.transform, "Humanoid_Locomotion_Runtime_Prototype");
            RemoveLegacyTargetV2Artifacts(player.transform);
            var procedural = player.transform.Find(ProceduralVisualName);
            if (procedural != null)
            {
                procedural.gameObject.SetActive(false);
            }

            var runtime = PrefabUtility.InstantiatePrefab(prefab, player.transform) as GameObject;
            if (runtime == null)
            {
                throw new System.Exception("[BubuTown] Could not instantiate VRM candidate prefab: " + CandidateVrmPath);
            }

            runtime.name = RuntimeObjectName;
            runtime.transform.localPosition = new Vector3(0f, -0.92f, 0f);
            runtime.transform.localRotation = Quaternion.identity;
            runtime.transform.localScale = Vector3.one * 0.92f;
            EnableAllRenderers(runtime);
            ApplyReadableUrpMaterials(runtime);
            EnsureMarker(runtime.transform);

            var animator = runtime.GetComponent<Animator>();
            if (animator == null)
            {
                animator = runtime.AddComponent<Animator>();
            }

            animator.runtimeAnimatorController = controller;
            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            CreateTargetStyleAddons(runtime, animator);

            var locomotion = player.GetComponent<BubuTownLocomotionAnimator>();
            if (locomotion == null)
            {
                locomotion = player.AddComponent<BubuTownLocomotionAnimator>();
            }

            locomotion.Animator = animator;
            locomotion.VisualRoot = runtime.transform;

            var playerController = player.GetComponent<BubuTownPlayerController>();
            if (playerController != null)
            {
                playerController.LocomotionAnimator = locomotion;
            }

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
            CapturePreview(prefab, controller);
            WriteReport();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[BubuTown] DaMogu VRM candidate A installed.");
        }

        [MenuItem("BubuTown/Capture DaMogu VRM Candidate Locomotion Frames")]
        public static void CaptureDaMoguVrmCandidateLocomotionFrames()
        {
            EnsureAsset(CandidateVrmPath);
            EnsureAsset(ControllerPath);
            EnsureCandidatePrefab();

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(CandidatePrefabPath);
            var controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(ControllerPath);
            if (prefab == null || controller == null)
            {
                throw new System.Exception("[BubuTown] Missing VRM candidate prefab or locomotion controller.");
            }

            CaptureClipFrames(prefab, controller as AnimatorController, "WalkForwards", DesktopWalkFramesFolder, 36);
            CaptureClipFrames(prefab, controller as AnimatorController, "RunForwards", DesktopRunFramesFolder, 36);
            Debug.Log("[BubuTown] DaMogu VRM candidate locomotion frames captured.");
        }

        [MenuItem("BubuTown/Validate DaMogu VRM Candidate Runtime")]
        public static void ValidateDaMoguVrmCandidateRuntime()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var player = GameObject.Find(PlayerPath);
            if (player == null)
            {
                throw new System.Exception("[BubuTown] Missing player root: " + PlayerPath);
            }

            var runtime = player.transform.Find(RuntimeObjectName);
            if (runtime == null)
            {
                throw new System.Exception("[BubuTown] Missing VRM runtime child: " + RuntimeObjectName);
            }

            var animator = runtime.GetComponent<Animator>();
            if (animator == null || animator.runtimeAnimatorController == null)
            {
                throw new System.Exception("[BubuTown] VRM runtime is missing an Animator controller.");
            }

            if (animator.applyRootMotion)
            {
                throw new System.Exception("[BubuTown] VRM runtime must keep root motion disabled.");
            }

            var locomotion = player.GetComponent<BubuTownLocomotionAnimator>();
            var playerController = player.GetComponent<BubuTownPlayerController>();
            if (locomotion == null || locomotion.Animator != animator || playerController == null || playerController.LocomotionAnimator != locomotion)
            {
                throw new System.Exception("[BubuTown] Player controller, locomotion driver, and VRM Animator are not linked.");
            }

            var rendererCount = runtime.GetComponentsInChildren<Renderer>(true).Length;
            if (rendererCount == 0)
            {
                throw new System.Exception("[BubuTown] VRM runtime has no renderers.");
            }

            if (runtime.Find(TargetStyleAddonsName) == null)
            {
                throw new System.Exception("[BubuTown] Missing DaMogu target style add-ons.");
            }

            Debug.Log("[BubuTown] DaMogu VRM candidate runtime validated. Renderers=" + rendererCount + ", Controller=" + animator.runtimeAnimatorController.name);
        }

        private static void EnsureCandidatePrefab()
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(CandidatePrefabPath) != null)
            {
                AssetDatabase.DeleteAsset(CandidatePrefabPath);
            }

            using (var data = new GlbFileParser(Path.GetFullPath(CandidateVrmPath)).Parse())
            using (var context = new VRMImporterContext(new VRMData(data)))
            {
                var prefabPath = CreateUnityPath(CandidatePrefabPath);
                var editor = CreateVrmEditorImporterContext(context, prefabPath);
                var loaded = context.Load();
                if (loaded == null || loaded.gameObject == null)
                {
                    throw new System.Exception("[BubuTown] Could not load VRM candidate runtime object.");
                }

                loaded.EnableUpdateWhenOffscreen();
                loaded.ShowMeshes();
                EnableAllRenderers(loaded.gameObject);
                SaveVrmAsAsset(editor, loaded);
            }

            AssetDatabase.ImportAsset(CandidatePrefabPath, ImportAssetOptions.ForceUpdate);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(CandidatePrefabPath);
            if (prefab != null)
            {
                ApplyReadableUrpMaterials(prefab);
                EditorUtility.SetDirty(prefab);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void CapturePreview(GameObject prefab, RuntimeAnimatorController controller)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            RenderSettings.ambientLight = new Color(0.82f, 0.84f, 0.88f);

            var light = new GameObject("Preview_Key_Light", typeof(Light));
            light.transform.rotation = Quaternion.Euler(45f, -35f, 0f);
            light.GetComponent<Light>().type = LightType.Directional;
            light.GetComponent<Light>().intensity = 1.65f;

            var model = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            if (model == null)
            {
                throw new System.Exception("[BubuTown] Could not instantiate VRM candidate for preview.");
            }

            model.transform.position = Vector3.zero;
            model.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            model.transform.localScale = Vector3.one;
            EnableAllRenderers(model);
            ApplyReadableUrpMaterials(model);
            var animator = model.GetComponent<Animator>();
            if (animator != null)
            {
                animator.runtimeAnimatorController = controller;
                SampleClip(controller as AnimatorController, model, "WalkForwards", 0.36f);
                CreateTargetStyleAddons(model, animator);
            }

            var cameraObject = new GameObject("Preview_Camera", typeof(Camera));
            var camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.88f, 0.91f, 0.94f);
            camera.orthographic = true;
            var bounds = CalculateRendererBounds(model);
            Debug.Log("[BubuTown] VRM preview bounds center=" + bounds.center + " size=" + bounds.size);
            if (!IsFinite(bounds.center) || !IsFinite(bounds.size) || bounds.size.y < 0.1f || bounds.size.y > 10f)
            {
                bounds = new Bounds(new Vector3(0f, 0.85f, 0f), new Vector3(1.1f, 1.7f, 0.7f));
                Debug.LogWarning("[BubuTown] VRM preview bounds looked invalid, using fallback bounds.");
            }
            camera.orthographicSize = Mathf.Clamp(bounds.size.y * 0.62f, 0.8f, 2.2f);
            camera.nearClipPlane = 0.05f;
            camera.farClipPlane = 20f;
            var target = bounds.center + Vector3.up * bounds.size.y * 0.04f;
            camera.transform.position = target + new Vector3(0f, 0f, -4.2f);
            camera.transform.rotation = Quaternion.LookRotation(target - camera.transform.position, Vector3.up);

            var renderTexture = new RenderTexture(900, 1100, 24);
            var previousTarget = camera.targetTexture;
            var previousActive = RenderTexture.active;
            camera.targetTexture = renderTexture;
            camera.Render();
            RenderTexture.active = renderTexture;
            var image = new Texture2D(900, 1100, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0f, 0f, 900, 1100), 0, 0);
            image.Apply();

            Directory.CreateDirectory(Path.GetDirectoryName(PreviewPath));
            File.WriteAllBytes(PreviewPath, image.EncodeToPNG());
            Directory.CreateDirectory(Path.GetDirectoryName(DesktopPreviewPath));
            File.WriteAllBytes(DesktopPreviewPath, image.EncodeToPNG());
            Object.DestroyImmediate(image);
            camera.targetTexture = previousTarget;
            RenderTexture.active = previousActive;
            Object.DestroyImmediate(renderTexture);
            EditorSceneManager.CloseScene(scene, true);
            AssetDatabase.ImportAsset(PreviewPath, ImportAssetOptions.ForceUpdate);
        }

        private static void CaptureClipFrames(GameObject prefab, AnimatorController controller, string clipName, string outputFolder, int frameCount)
        {
            if (controller == null)
            {
                throw new System.Exception("[BubuTown] Missing AnimatorController for VRM frame capture.");
            }

            var clip = FindClip(controller, clipName);
            Directory.CreateDirectory(outputFolder);

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            RenderSettings.ambientLight = new Color(0.9f, 0.88f, 0.86f);

            var keyLight = new GameObject("Preview_Key_Light", typeof(Light));
            keyLight.transform.rotation = Quaternion.Euler(35f, -25f, 0f);
            keyLight.GetComponent<Light>().type = LightType.Directional;
            keyLight.GetComponent<Light>().intensity = 1.15f;

            var fillLight = new GameObject("Preview_Fill_Light", typeof(Light));
            fillLight.transform.rotation = Quaternion.Euler(15f, 35f, 0f);
            fillLight.GetComponent<Light>().type = LightType.Directional;
            fillLight.GetComponent<Light>().intensity = 0.55f;

            var model = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            if (model == null)
            {
                throw new System.Exception("[BubuTown] Could not instantiate VRM candidate for frame capture.");
            }

            model.transform.position = Vector3.zero;
            model.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            model.transform.localScale = Vector3.one;
            EnableAllRenderers(model);
            ApplyReadableUrpMaterials(model);
            var animator = model.GetComponent<Animator>();
            if (animator != null)
            {
                CreateTargetStyleAddons(model, animator);
            }

            var bounds = CalculateRendererBounds(model);
            var target = bounds.center + Vector3.up * bounds.size.y * 0.04f;
            var cameraObject = new GameObject("Preview_Camera", typeof(Camera));
            var camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.88f, 0.91f, 0.94f);
            camera.orthographic = true;
            camera.orthographicSize = Mathf.Clamp(bounds.size.y * 0.62f, 0.8f, 2.2f);
            camera.nearClipPlane = 0.05f;
            camera.farClipPlane = 20f;
            camera.transform.position = target + new Vector3(0f, 0f, -4.2f);
            camera.transform.rotation = Quaternion.LookRotation(target - camera.transform.position, Vector3.up);

            var renderTexture = new RenderTexture(720, 900, 24);
            var previousTarget = camera.targetTexture;
            var previousActive = RenderTexture.active;
            camera.targetTexture = renderTexture;

            for (var frame = 0; frame < frameCount; frame++)
            {
                var time = clip.length * frame / frameCount;
                clip.SampleAnimation(model, time);
                camera.Render();
                RenderTexture.active = renderTexture;
                var image = new Texture2D(720, 900, TextureFormat.RGB24, false);
                image.ReadPixels(new Rect(0f, 0f, 720, 900), 0, 0);
                image.Apply();
                File.WriteAllBytes(Path.Combine(outputFolder, frame.ToString("000") + ".png"), image.EncodeToPNG());
                Object.DestroyImmediate(image);
            }

            camera.targetTexture = previousTarget;
            RenderTexture.active = previousActive;
            Object.DestroyImmediate(renderTexture);
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }

        private static void SampleClip(AnimatorController controller, GameObject root, string containsName, float normalizedTime)
        {
            if (controller == null)
            {
                return;
            }

            foreach (var clip in controller.animationClips)
            {
                if (clip != null && clip.name.Contains(containsName))
                {
                    clip.SampleAnimation(root, Mathf.Clamp01(normalizedTime) * Mathf.Max(0.01f, clip.length));
                    return;
                }
            }
        }

        private static AnimationClip FindClip(AnimatorController controller, string containsName)
        {
            foreach (var clip in controller.animationClips)
            {
                if (clip != null && clip.name.Contains(containsName))
                {
                    return clip;
                }
            }

            throw new System.Exception("[BubuTown] Missing animation clip containing: " + containsName);
        }

        private static void EnsureMarker(Transform runtime)
        {
            var marker = runtime.Find(CandidateMarkerName);
            if (marker == null)
            {
                var child = new GameObject(CandidateMarkerName);
                child.transform.SetParent(runtime, false);
            }
        }

        private static void WriteReport()
        {
            var report =
                "# DaMogu VRM Candidate Integration Report\n\n" +
                "Status: AvatarSample_A is installed as the current third-person DaMogu visual candidate for movement and camera evaluation.\n\n" +
                "Purpose: replace the rejected primitive/recolored prototype with a real anime-style Humanoid model so walking, running, camera height, and body readability can be judged in the Star Bay Town scene.\n\n" +
                "Source: `madjin/vrm-samples`, `vroid/stable/AvatarSample_A.vrm`.\n\n" +
                "License note: VRoid sample models can be used under the VRoid sample conditions. Copyright is not waived, and this candidate is a prototype/reference asset rather than the final original public DaMogu character.\n\n" +
                "Unity import route: UniVRM v0.131.2 through UPM packages `com.vrmc.gltf` and `com.vrmc.univrm`, matching the source model's VRM 0.x format.\n\n" +
                "Runtime scene hookup:\n" +
                "- Player root: `08_Player_And_Runtime/Player_Start_Bubu`\n" +
                "- Runtime visual child: `DaMogu_VRM_Candidate_Runtime`\n" +
                "- Candidate marker: `DaMogu_VRM_Candidate_A_AvatarSample_A`\n" +
                "- Animator controller: `Bubu_DaMogu_Locomotion.controller`\n\n" +
                "Motion preview outputs:\n" +
                "- `/Users/zhendian/Desktop/星湾镇_大蘑菇_VRM候选A_Walk动画预览.gif`\n" +
                "- `/Users/zhendian/Desktop/星湾镇_大蘑菇_VRM候选A_Run动画预览.gif`\n\n" +
                "Current art judgment:\n" +
                "- Better than the rejected primitive/recolored prototype because it is a real skinned anime Humanoid model.\n" +
                "- Still not final DaMogu: pants silhouette is too heavy for a cozy modern-life protagonist, the face/material response needs a softer toon setup, and the hairstyle should be redesigned toward a cleaner mature-cute look.\n" +
                "- The animation pipeline is useful, but final polish should add dedicated idle, walk, jog, tired walk, interact, sleep, bike mount, and work-minigame poses.\n\n" +
                "Next art pass recommendation: use this candidate to confirm locomotion feel, then create an original DaMogu VRoid/Blender model with a more mature face, cleaner hair silhouette, lighter lower-body silhouette, and pink-white casual seaside outfit.\n";
            Directory.CreateDirectory(Path.GetDirectoryName(ReportPath));
            File.WriteAllText(ReportPath, report);
        }

        private static void EnsureAsset(string assetPath)
        {
            if (!File.Exists(assetPath))
            {
                throw new System.Exception("[BubuTown] Missing required asset: " + assetPath);
            }
        }

        private static void EnableAllRenderers(GameObject root)
        {
            foreach (var renderer in root.GetComponentsInChildren<Renderer>(true))
            {
                renderer.enabled = true;
                renderer.gameObject.SetActive(true);
                if (renderer is SkinnedMeshRenderer skinned)
                {
                    skinned.updateWhenOffscreen = true;
                    skinned.forceMatrixRecalculationPerRender = true;
                }
            }
        }

        private static void ApplyReadableUrpMaterials(GameObject root)
        {
            Directory.CreateDirectory(CandidateUrpMaterialFolder);
            foreach (var renderer in root.GetComponentsInChildren<Renderer>(true))
            {
                var materials = renderer.sharedMaterials;
                for (var i = 0; i < materials.Length; i++)
                {
                    var source = materials[i];
                    if (source == null)
                    {
                        continue;
                    }

                    var texture = LoadTextureForMaterial(source.name);
                    if (texture == null)
                    {
                        continue;
                    }

                    materials[i] = CreateReadableUrpMaterial(source.name, texture);
                }

                renderer.sharedMaterials = materials;
                EditorUtility.SetDirty(renderer);
            }
        }

        private static void CreateTargetStyleAddons(GameObject runtime, Animator animator)
        {
            RemoveChild(runtime.transform, TargetStyleAddonsName);
            var root = new GameObject(TargetStyleAddonsName);
            root.transform.SetParent(runtime.transform, false);
            root.transform.localPosition = Vector3.zero;
            root.transform.localRotation = Quaternion.identity;
            root.transform.localScale = Vector3.one;

            var navy = LoadPrototypeMaterial("DaMogu_ModelV1_Navy.mat");
            var softPink = LoadPrototypeMaterial("DaMogu_ModelV1_SoftPink.mat");
            var white = LoadPrototypeMaterial("DaMogu_ModelV1_White.mat");
            var bagCream = LoadPrototypeMaterial("DaMogu_ModelV1_BagCream.mat");
            var hair = LoadPrototypeMaterial("DaMogu_ModelV1_Hair.mat");
            var shoePink = LoadPrototypeMaterial("DaMogu_ModelV1_ShoePink.mat");

            CreateBox(root.transform, "Hoodie_White_Upper_Read", white, new Vector3(0f, 0.88f, -0.02f), new Vector3(0.48f, 0.28f, 0.16f), Quaternion.identity);
            CreateBox(root.transform, "Hoodie_Pink_Lower_Read", softPink, new Vector3(0f, 0.65f, -0.03f), new Vector3(0.54f, 0.3f, 0.16f), Quaternion.identity);
            CreateBox(root.transform, "Skort_Navy_ThirdPerson_Read", navy, new Vector3(0f, 0.36f, -0.02f), new Vector3(0.58f, 0.18f, 0.42f), Quaternion.Euler(0f, 0f, 0f));

            CreateBox(root.transform, "Bag_Strap_Diagonal_Read", bagCream, new Vector3(0.06f, 0.72f, -0.22f), new Vector3(0.045f, 0.68f, 0.035f), Quaternion.Euler(0f, 0f, -28f));
            CreateBox(root.transform, "Crossbody_Bag_Read", bagCream, new Vector3(0.34f, 0.52f, -0.16f), new Vector3(0.22f, 0.16f, 0.09f), Quaternion.Euler(0f, 0f, -8f));

            var head = animator != null ? animator.GetBoneTransform(HumanBodyBones.Head) : null;
            var leftFoot = animator != null ? animator.GetBoneTransform(HumanBodyBones.LeftFoot) : null;
            var rightFoot = animator != null ? animator.GetBoneTransform(HumanBodyBones.RightFoot) : null;
            var leftLowerArm = animator != null ? animator.GetBoneTransform(HumanBodyBones.LeftLowerArm) : null;
            var rightLowerArm = animator != null ? animator.GetBoneTransform(HumanBodyBones.RightLowerArm) : null;

            if (head != null)
            {
                CreateSphere(head, "Half_Up_Bun_Read", hair, new Vector3(0f, 0.08f, -0.13f), new Vector3(0.15f, 0.15f, 0.13f));
                CreateSphere(head, "Back_Bob_Hair_Read", hair, new Vector3(0f, -0.08f, -0.11f), new Vector3(0.28f, 0.26f, 0.12f));
            }

            if (leftFoot != null)
            {
                CreateBox(leftFoot, "Left_Sneaker_Pink_White_Read", shoePink, new Vector3(0f, 0.02f, 0.08f), new Vector3(0.16f, 0.08f, 0.24f), Quaternion.identity);
            }

            if (rightFoot != null)
            {
                CreateBox(rightFoot, "Right_Sneaker_Pink_White_Read", shoePink, new Vector3(0f, 0.02f, 0.08f), new Vector3(0.16f, 0.08f, 0.24f), Quaternion.identity);
            }

            if (leftLowerArm != null)
            {
                CreateBox(leftLowerArm, "Left_Pink_Sleeve_Read", softPink, new Vector3(0f, 0.02f, 0f), new Vector3(0.12f, 0.28f, 0.12f), Quaternion.identity);
            }

            if (rightLowerArm != null)
            {
                CreateBox(rightLowerArm, "Right_Pink_Sleeve_Read", softPink, new Vector3(0f, 0.02f, 0f), new Vector3(0.12f, 0.28f, 0.12f), Quaternion.identity);
            }
        }

        private static Material LoadPrototypeMaterial(string fileName)
        {
            return AssetDatabase.LoadAssetAtPath<Material>(PrototypeMaterialFolder + "/" + fileName);
        }

        private static GameObject CreateBox(Transform parent, string name, Material material, Vector3 localPosition, Vector3 localScale, Quaternion localRotation)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = name;
            obj.transform.SetParent(parent, false);
            obj.transform.localPosition = localPosition;
            obj.transform.localRotation = localRotation;
            obj.transform.localScale = localScale;
            ApplySimpleRenderSetup(obj, material);
            return obj;
        }

        private static GameObject CreateSphere(Transform parent, string name, Material material, Vector3 localPosition, Vector3 localScale)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.name = name;
            obj.transform.SetParent(parent, false);
            obj.transform.localPosition = localPosition;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = localScale;
            ApplySimpleRenderSetup(obj, material);
            return obj;
        }

        private static void ApplySimpleRenderSetup(GameObject obj, Material material)
        {
            var collider = obj.GetComponent<Collider>();
            if (collider != null)
            {
                Object.DestroyImmediate(collider);
            }

            var renderer = obj.GetComponent<Renderer>();
            if (renderer != null && material != null)
            {
                renderer.sharedMaterial = material;
            }
        }

        private static Material CreateReadableUrpMaterial(string sourceName, Texture2D texture)
        {
            var safeName = sourceName.Replace("/", "_").Replace("\\", "_");
            var path = CandidateUrpMaterialFolder + "/" + safeName + "_Readable.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
                AssetDatabase.CreateAsset(material, path);
            }

            var tint = ColorForMaterial(sourceName);
            material.mainTexture = texture;
            material.color = tint;
            if (material.HasProperty("_BaseMap"))
            {
                material.SetTexture("_BaseMap", texture);
            }
            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", tint);
            }
            if (material.HasProperty("_Smoothness"))
            {
                material.SetFloat("_Smoothness", 0.2f);
            }
            ConfigureSurfaceMode(material, sourceName.Contains("_FACE") || sourceName.Contains("_EYE") || sourceName.Contains("Bottoms"));
            if (sourceName.Contains("Bottoms"))
            {
                material.color = new Color(1f, 1f, 1f, 0.02f);
                if (material.HasProperty("_BaseColor"))
                {
                    material.SetColor("_BaseColor", new Color(1f, 1f, 1f, 0.02f));
                }
            }

            EditorUtility.SetDirty(material);
            return material;
        }

        private static void ConfigureSurfaceMode(Material material, bool transparent)
        {
            if (transparent)
            {
                material.SetOverrideTag("RenderType", "Transparent");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                SetIfPresent(material, "_Surface", 1f);
                SetIfPresent(material, "_Blend", 0f);
                SetIfPresent(material, "_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
                SetIfPresent(material, "_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                SetIfPresent(material, "_ZWrite", 0f);
                material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            }
            else
            {
                material.SetOverrideTag("RenderType", "Opaque");
                material.renderQueue = -1;
                SetIfPresent(material, "_Surface", 0f);
                SetIfPresent(material, "_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                SetIfPresent(material, "_DstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
                SetIfPresent(material, "_ZWrite", 1f);
                material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
            }
        }

        private static void SetIfPresent(Material material, string propertyName, float value)
        {
            if (material.HasProperty(propertyName))
            {
                material.SetFloat(propertyName, value);
            }
        }

        private static Texture2D LoadTextureForMaterial(string materialName)
        {
            if (materialName == "F00_006_01_Tops_01_CLOTH")
            {
                return AssetDatabase.LoadAssetAtPath<Texture2D>(HoodiePinkOverridePath);
            }

            var textureName = TextureNameForMaterial(materialName);
            if (string.IsNullOrEmpty(textureName))
            {
                return null;
            }

            return AssetDatabase.LoadAssetAtPath<Texture2D>(CandidateTextureFolder + "/" + textureName + ".png");
        }

        private static string TextureNameForMaterial(string materialName)
        {
            if (materialName.Contains("_HAIR_"))
            {
                return materialName.Replace("_HAIR_", "_");
            }

            var suffixes = new[] { "_SKIN", "_CLOTH", "_FACE", "_EYE" };
            foreach (var suffix in suffixes)
            {
                if (materialName.EndsWith(suffix, System.StringComparison.Ordinal))
                {
                    return materialName.Substring(0, materialName.Length - suffix.Length);
                }
            }

            return materialName;
        }

        private static Color ColorForMaterial(string materialName)
        {
            if (materialName.Contains("_SKIN"))
            {
                return new Color(1f, 0.86f, 0.78f, 1f);
            }

            if (materialName.Contains("_HAIR_"))
            {
                return new Color(0.96f, 0.82f, 0.9f, 1f);
            }

            if (materialName.Contains("_CLOTH"))
            {
                return new Color(1f, 0.96f, 0.98f, 1f);
            }

            return Color.white;
        }

        private static Bounds CalculateRendererBounds(GameObject root)
        {
            var renderers = root.GetComponentsInChildren<Renderer>(true);
            var hasBounds = false;
            var bounds = new Bounds(root.transform.position + Vector3.up, Vector3.one);
            foreach (var renderer in renderers)
            {
                if (!renderer.enabled)
                {
                    continue;
                }

                if (!hasBounds)
                {
                    bounds = renderer.bounds;
                    hasBounds = true;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            if (!hasBounds)
            {
                Debug.LogWarning("[BubuTown] VRM candidate preview found no renderer bounds; using fallback framing.");
            }
            else
            {
                Debug.Log("[BubuTown] VRM candidate preview renderer count=" + renderers.Length + " bounds=" + bounds);
            }

            return bounds;
        }

        private static bool IsFinite(Vector3 value)
        {
            return float.IsFinite(value.x) && float.IsFinite(value.y) && float.IsFinite(value.z);
        }

        private static object CreateUnityPath(string assetPath)
        {
            var unityPathType = FindType("UniGLTF.UnityPath");
            var fromUnityPath = unityPathType.GetMethod("FromUnityPath", BindingFlags.Public | BindingFlags.Static);
            if (fromUnityPath == null)
            {
                throw new System.Exception("[BubuTown] UniGLTF.UnityPath.FromUnityPath was not found.");
            }

            return fromUnityPath.Invoke(null, new object[] { assetPath });
        }

        private static object CreateVrmEditorImporterContext(VRMImporterContext context, object prefabPath)
        {
            var importerType = FindType("VRM.VRMEditorImporterContext");
            return System.Activator.CreateInstance(importerType, context, prefabPath);
        }

        private static void SaveVrmAsAsset(object editorContext, object loaded)
        {
            var saveMethod = editorContext.GetType().GetMethod("SaveAsAsset", BindingFlags.Public | BindingFlags.Instance);
            if (saveMethod == null)
            {
                throw new System.Exception("[BubuTown] VRMEditorImporterContext.SaveAsAsset was not found.");
            }

            saveMethod.Invoke(editorContext, new[] { loaded });
        }

        private static System.Type FindType(string fullName)
        {
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(fullName);
                if (type != null)
                {
                    return type;
                }
            }

            throw new System.Exception("[BubuTown] Required UniVRM type was not found: " + fullName);
        }

        private static void RemoveChild(Transform parent, string childName)
        {
            var child = parent.Find(childName);
            if (child != null)
            {
                Object.DestroyImmediate(child.gameObject);
            }
        }

        private static void RemoveLegacyTargetV2Artifacts(Transform root)
        {
            var staleObjects = new System.Collections.Generic.List<GameObject>();
            CollectLegacyTargetV2Artifacts(root, staleObjects);
            foreach (var staleObject in staleObjects)
            {
                Object.DestroyImmediate(staleObject);
            }
        }

        private static void CollectLegacyTargetV2Artifacts(Transform root, System.Collections.Generic.List<GameObject> staleObjects)
        {
            foreach (Transform child in root)
            {
                if (child.name == "DaMogu_TargetLook_Overlay_v2" ||
                    child.name.StartsWith("DaMoguTargetV2_", System.StringComparison.Ordinal))
                {
                    staleObjects.Add(child.gameObject);
                    continue;
                }

                CollectLegacyTargetV2Artifacts(child, staleObjects);
            }
        }
    }
}
