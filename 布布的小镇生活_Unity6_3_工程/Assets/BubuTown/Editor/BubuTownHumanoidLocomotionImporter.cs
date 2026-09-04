using System.IO;
using BubuTown;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BubuTown.EditorTools
{
    public static class BubuTownHumanoidLocomotionImporter
    {
        private const string ScenePath = "Assets/Scenes/BubuTownPrototype.unity";
        private const string PlayerPath = "BubuTown_Prototype_All_Visible_Before_Play/08_Player_And_Runtime/Player_Start_Bubu";
        private const string PrototypeFolder = "Assets/BubuTown/Characters/Prototype";
        private const string SourceFolder = PrototypeFolder + "/UnityStandardFemale";
        private const string ControllerFolder = PrototypeFolder + "/DaMogu_Locomotion";
        private const string RigPath = SourceFolder + "/Rig/defaultfemale_rig.fbx";
        private const string IdlePath = SourceFolder + "/Animations/Exploration/f@Idle.fbx";
        private const string WalkPath = SourceFolder + "/Animations/Exploration/f@WalkForwards.fbx";
        private const string RunPath = SourceFolder + "/Animations/Exploration/f@RunForwards.fbx";
        private const string SprintPath = SourceFolder + "/Animations/Exploration/f@SprintForwards.fbx";
        private const string ControllerPath = ControllerFolder + "/Bubu_DaMogu_Locomotion.controller";
        private const string RecoloredTexturePath = ControllerFolder + "/DaMogu_ModelV1_RecoloredAlbedo.png";
        private const string ReportPath = "Assets/BubuTown/Docs/DaMoguHumanoidLocomotionPrototypeReport.md";
        private const string PreviewSheetPath = "Assets/BubuTown/Docs/DaMoguHumanoidLocomotionPreviewSheet.png";
        private const string DesktopPreviewSheetPath = "/Users/zhendian/Desktop/星湾镇_大蘑菇_Humanoid真实动作采样预览.png";
        private const string DesktopWalkFramesFolder = "/Users/zhendian/Desktop/星湾镇_大蘑菇_真实Walk动画帧";
        private const string DesktopRunFramesFolder = "/Users/zhendian/Desktop/星湾镇_大蘑菇_真实Run动画帧";
        private const string DesktopReadableClothingWalkFramesFolder = "/Users/zhendian/Desktop/星湾镇_大蘑菇_衣服清晰Walk动画帧_v0_3";
        private const string DesktopReadableClothingRunFramesFolder = "/Users/zhendian/Desktop/星湾镇_大蘑菇_衣服清晰Run动画帧_v0_3";
        private const string DesktopLayeredClothingWalkFramesFolder = "/Users/zhendian/Desktop/星湾镇_大蘑菇_分层衣服Walk动画帧_v0_4";
        private const string DesktopLayeredClothingRunFramesFolder = "/Users/zhendian/Desktop/星湾镇_大蘑菇_分层衣服Run动画帧_v0_4";
        private const string RuntimeObjectName = "Humanoid_Locomotion_Runtime_Prototype";
        private const string PlayableModelName = "DaMogu_Playable_Model_v1";

        [MenuItem("BubuTown/Import DaMogu Humanoid Locomotion Prototype")]
        public static void ImportDaMoguHumanoidLocomotionPrototype()
        {
            EnsureRequiredFiles();
            Directory.CreateDirectory(ControllerFolder);
            Directory.CreateDirectory(Path.GetDirectoryName(ReportPath));

            ConfigureRigImporter(RigPath, ModelImporterAvatarSetup.CreateFromThisModel, null);
            AssetDatabase.ImportAsset(RigPath, ImportAssetOptions.ForceUpdate);

            var avatar = LoadAvatar(RigPath);
            ConfigureRigImporter(IdlePath, ModelImporterAvatarSetup.CopyFromOther, avatar);
            ConfigureRigImporter(WalkPath, ModelImporterAvatarSetup.CopyFromOther, avatar);
            ConfigureRigImporter(RunPath, ModelImporterAvatarSetup.CopyFromOther, avatar);
            ConfigureRigImporter(SprintPath, ModelImporterAvatarSetup.CopyFromOther, avatar);
            AssetDatabase.ImportAsset(IdlePath, ImportAssetOptions.ForceUpdate);
            AssetDatabase.ImportAsset(WalkPath, ImportAssetOptions.ForceUpdate);
            AssetDatabase.ImportAsset(RunPath, ImportAssetOptions.ForceUpdate);
            AssetDatabase.ImportAsset(SprintPath, ImportAssetOptions.ForceUpdate);

            var controller = CreateLocomotionController();
            AttachToPrototypeScene(controller);
            WriteReport();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[BubuTown] DaMogu Humanoid locomotion prototype imported.");
        }

        [MenuItem("BubuTown/Capture DaMogu Humanoid Locomotion Preview Sheet")]
        public static void CaptureDaMoguHumanoidLocomotionPreviewSheet()
        {
            EnsureRequiredFiles();
            ConfigureRigImporter(RigPath, ModelImporterAvatarSetup.CreateFromThisModel, null);
            AssetDatabase.ImportAsset(RigPath, ImportAssetOptions.ForceUpdate);

            var avatar = LoadAvatar(RigPath);
            ConfigureRigImporter(IdlePath, ModelImporterAvatarSetup.CopyFromOther, avatar);
            ConfigureRigImporter(WalkPath, ModelImporterAvatarSetup.CopyFromOther, avatar);
            ConfigureRigImporter(RunPath, ModelImporterAvatarSetup.CopyFromOther, avatar);
            ConfigureRigImporter(SprintPath, ModelImporterAvatarSetup.CopyFromOther, avatar);

            var sheet = CapturePreviewSheet();
            Directory.CreateDirectory(Path.GetDirectoryName(PreviewSheetPath));
            File.WriteAllBytes(PreviewSheetPath, sheet.EncodeToPNG());
            Directory.CreateDirectory(Path.GetDirectoryName(DesktopPreviewSheetPath));
            File.WriteAllBytes(DesktopPreviewSheetPath, sheet.EncodeToPNG());
            Object.DestroyImmediate(sheet);

            AssetDatabase.ImportAsset(PreviewSheetPath, ImportAssetOptions.ForceUpdate);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[BubuTown] DaMogu Humanoid locomotion preview sheet captured: " + PreviewSheetPath);
        }

        [MenuItem("BubuTown/Capture DaMogu Humanoid Locomotion Gif Frames")]
        public static void CaptureDaMoguHumanoidLocomotionGifFrames()
        {
            EnsureRequiredFiles();
            ConfigureRigImporter(RigPath, ModelImporterAvatarSetup.CreateFromThisModel, null);
            AssetDatabase.ImportAsset(RigPath, ImportAssetOptions.ForceUpdate);

            var avatar = LoadAvatar(RigPath);
            ConfigureRigImporter(WalkPath, ModelImporterAvatarSetup.CopyFromOther, avatar);
            ConfigureRigImporter(RunPath, ModelImporterAvatarSetup.CopyFromOther, avatar);

            CaptureClipFrameSequence(WalkPath, DesktopWalkFramesFolder, 36);
            CaptureClipFrameSequence(RunPath, DesktopRunFramesFolder, 36);
            Debug.Log("[BubuTown] DaMogu Humanoid locomotion GIF frames captured.");
        }

        [MenuItem("BubuTown/Capture DaMogu Readable Clothing Motion Frames v0.3")]
        public static void CaptureDaMoguReadableClothingMotionFrames()
        {
            EnsureRequiredFiles();
            ConfigureRigImporter(RigPath, ModelImporterAvatarSetup.CreateFromThisModel, null);
            AssetDatabase.ImportAsset(RigPath, ImportAssetOptions.ForceUpdate);

            var avatar = LoadAvatar(RigPath);
            ConfigureRigImporter(WalkPath, ModelImporterAvatarSetup.CopyFromOther, avatar);
            ConfigureRigImporter(RunPath, ModelImporterAvatarSetup.CopyFromOther, avatar);

            CaptureClipFrameSequence(WalkPath, DesktopReadableClothingWalkFramesFolder, 36);
            CaptureClipFrameSequence(RunPath, DesktopReadableClothingRunFramesFolder, 36);
            Debug.Log("[BubuTown] DaMogu readable clothing motion frames v0.3 captured.");
        }

        [MenuItem("BubuTown/Capture DaMogu Layered Clothing Motion Frames v0.4")]
        public static void CaptureDaMoguLayeredClothingMotionFrames()
        {
            EnsureRequiredFiles();
            ConfigureRigImporter(RigPath, ModelImporterAvatarSetup.CreateFromThisModel, null);
            AssetDatabase.ImportAsset(RigPath, ImportAssetOptions.ForceUpdate);

            var avatar = LoadAvatar(RigPath);
            ConfigureRigImporter(WalkPath, ModelImporterAvatarSetup.CopyFromOther, avatar);
            ConfigureRigImporter(RunPath, ModelImporterAvatarSetup.CopyFromOther, avatar);

            CaptureClipFrameSequence(WalkPath, DesktopLayeredClothingWalkFramesFolder, 36);
            CaptureClipFrameSequence(RunPath, DesktopLayeredClothingRunFramesFolder, 36);
            Debug.Log("[BubuTown] DaMogu layered clothing motion frames v0.4 captured.");
        }

        private static void ConfigureRigImporter(string assetPath, ModelImporterAvatarSetup avatarSetup, Avatar sourceAvatar)
        {
            var importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            if (importer == null)
            {
                throw new System.Exception("[BubuTown] Missing ModelImporter for: " + assetPath);
            }

            importer.animationType = ModelImporterAnimationType.Human;
            importer.avatarSetup = avatarSetup;
            importer.sourceAvatar = sourceAvatar;
            importer.importAnimation = true;
            importer.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
            importer.SaveAndReimport();
        }

        private static AnimatorController CreateLocomotionController()
        {
            if (File.Exists(ControllerPath))
            {
                AssetDatabase.DeleteAsset(ControllerPath);
            }

            var controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
            controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
            controller.AddParameter("MoveX", AnimatorControllerParameterType.Float);
            controller.AddParameter("MoveY", AnimatorControllerParameterType.Float);
            controller.AddParameter("Grounded", AnimatorControllerParameterType.Bool);
            controller.AddParameter("Sprinting", AnimatorControllerParameterType.Bool);

            var layer = controller.layers[0];
            layer.stateMachine.states = new ChildAnimatorState[0];

            var state = layer.stateMachine.AddState("DaMogu_Locomotion_Blend");
            state.motion = CreateBlendTree(controller);
            state.writeDefaultValues = true;
            layer.stateMachine.defaultState = state;
            controller.layers[0] = layer;
            return controller;
        }

        private static BlendTree CreateBlendTree(AnimatorController controller)
        {
            var blendTree = new BlendTree
            {
                name = "Speed_BlendTree",
                blendParameter = "Speed",
                useAutomaticThresholds = false,
                blendType = BlendTreeType.Simple1D
            };
            AssetDatabase.AddObjectToAsset(blendTree, controller);

            blendTree.AddChild(LoadClip(IdlePath), 0f);
            blendTree.AddChild(LoadClip(WalkPath), 0.38f);
            blendTree.AddChild(LoadClip(RunPath), 0.78f);
            blendTree.AddChild(LoadClip(SprintPath), 1f);
            return blendTree;
        }

        private static void AttachToPrototypeScene(RuntimeAnimatorController controller)
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var player = GameObject.Find(PlayerPath);
            if (player == null)
            {
                throw new System.Exception("[BubuTown] Missing player root: " + PlayerPath);
            }

            RemoveChild(player.transform, RuntimeObjectName);
            var procedural = player.transform.Find("DaMogu_C_Anime_ThirdPerson_Prototype");
            if (procedural != null)
            {
                procedural.gameObject.SetActive(false);
            }

            var rigPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(RigPath);
            var runtime = PrefabUtility.InstantiatePrefab(rigPrefab, player.transform) as GameObject;
            if (runtime == null)
            {
                throw new System.Exception("[BubuTown] Could not instantiate rig prefab: " + RigPath);
            }

            runtime.name = RuntimeObjectName;
            runtime.transform.localPosition = new Vector3(0f, -0.92f, 0f);
            runtime.transform.localRotation = Quaternion.identity;
            runtime.transform.localScale = Vector3.one * 0.96f;

            var animator = runtime.GetComponent<Animator>();
            if (animator == null)
            {
                animator = runtime.AddComponent<Animator>();
            }
            animator.avatar = LoadAvatar(RigPath);
            animator.runtimeAnimatorController = controller;
            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            ApplyDaMoguSkinnedModelV1(runtime);

            var locomotion = player.GetComponent<BubuTownLocomotionAnimator>();
            if (locomotion == null)
            {
                locomotion = player.AddComponent<BubuTownLocomotionAnimator>();
            }
            locomotion.Animator = animator;
            locomotion.VisualRoot = runtime.transform;

            var controllerComponent = player.GetComponent<BubuTownPlayerController>();
            if (controllerComponent != null)
            {
                controllerComponent.LocomotionAnimator = locomotion;
            }

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }

        private static AnimationClip LoadClip(string assetPath)
        {
            foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(assetPath))
            {
                if (asset is AnimationClip clip && !clip.name.StartsWith("__preview__", System.StringComparison.Ordinal))
                {
                    return clip;
                }
            }

            throw new System.Exception("[BubuTown] Missing animation clip in: " + assetPath);
        }

        private static Avatar LoadAvatar(string assetPath)
        {
            foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(assetPath))
            {
                if (asset is Avatar avatar && avatar.isHuman && avatar.isValid)
                {
                    return avatar;
                }
            }

            throw new System.Exception("[BubuTown] Missing valid Humanoid avatar in: " + assetPath);
        }

        private static Texture2D CapturePreviewSheet()
        {
            var clips = new[]
            {
                ("Idle", LoadClip(IdlePath)),
                ("Walk", LoadClip(WalkPath)),
                ("Run", LoadClip(RunPath)),
                ("Sprint", LoadClip(SprintPath))
            };
            var sampleTimes01 = new[] { 0f, 0.25f, 0.5f, 0.75f, 0.95f };
            const int cellWidth = 360;
            const int cellHeight = 480;
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var sheet = new Texture2D(cellWidth * sampleTimes01.Length, cellHeight * clips.Length, TextureFormat.RGB24, false);
            FillTexture(sheet, new Color32(238, 240, 244, 255));
            RenderSettings.ambientLight = new Color(0.78f, 0.82f, 0.88f);

            var keyLight = new GameObject("Preview_Key_Light", typeof(Light));
            keyLight.transform.rotation = Quaternion.Euler(42f, -35f, 0f);
            keyLight.GetComponent<Light>().type = LightType.Directional;
            keyLight.GetComponent<Light>().intensity = 1.6f;

            var cameraObject = new GameObject("Preview_Camera", typeof(Camera));
            var camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.88f, 0.89f, 0.91f);
            camera.orthographic = true;
            camera.orthographicSize = 1.28f;
            camera.nearClipPlane = 0.05f;
            camera.farClipPlane = 20f;
            camera.transform.position = new Vector3(0f, 1.05f, -4.2f);
            camera.transform.rotation = Quaternion.LookRotation(new Vector3(0f, 0.82f, 0f) - camera.transform.position, Vector3.up);

            var renderTexture = new RenderTexture(cellWidth, cellHeight, 24);
            var previousTarget = camera.targetTexture;
            var previousActive = RenderTexture.active;
            camera.targetTexture = renderTexture;

            var rigPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(RigPath);
            for (var row = 0; row < clips.Length; row++)
            {
                for (var col = 0; col < sampleTimes01.Length; col++)
                {
                    var rig = PrefabUtility.InstantiatePrefab(rigPrefab) as GameObject;
                    if (rig == null)
                    {
                        throw new System.Exception("[BubuTown] Could not instantiate preview rig.");
                    }

                    rig.transform.position = Vector3.zero;
                    rig.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                    rig.transform.localScale = Vector3.one;
                    var clip = clips[row].Item2;
                    ApplyDaMoguSkinnedModelV1(rig);
                    clip.SampleAnimation(rig, Mathf.Clamp01(sampleTimes01[col]) * Mathf.Max(0.01f, clip.length));

                    camera.Render();
                    RenderTexture.active = renderTexture;
                    var cell = new Texture2D(cellWidth, cellHeight, TextureFormat.RGB24, false);
                    cell.ReadPixels(new Rect(0f, 0f, cellWidth, cellHeight), 0, 0);
                    cell.Apply();
                    sheet.SetPixels(col * cellWidth, (clips.Length - 1 - row) * cellHeight, cellWidth, cellHeight, cell.GetPixels());
                    Object.DestroyImmediate(cell);
                    Object.DestroyImmediate(rig);
                }
            }

            sheet.Apply();
            camera.targetTexture = previousTarget;
            RenderTexture.active = previousActive;
            Object.DestroyImmediate(renderTexture);
            EditorSceneManager.CloseScene(scene, true);
            return sheet;
        }

        private static void CaptureClipFrameSequence(string clipPath, string outputFolder, int frameCount)
        {
            Directory.CreateDirectory(outputFolder);
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            RenderSettings.ambientLight = new Color(0.78f, 0.82f, 0.88f);

            var keyLight = new GameObject("Preview_Key_Light", typeof(Light));
            keyLight.transform.rotation = Quaternion.Euler(42f, -35f, 0f);
            keyLight.GetComponent<Light>().type = LightType.Directional;
            keyLight.GetComponent<Light>().intensity = 1.6f;

            var cameraObject = new GameObject("Preview_Camera", typeof(Camera));
            var camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.88f, 0.89f, 0.91f);
            camera.orthographic = true;
            camera.orthographicSize = 1.28f;
            camera.nearClipPlane = 0.05f;
            camera.farClipPlane = 20f;
            camera.transform.position = new Vector3(0f, 1.05f, -4.2f);
            camera.transform.rotation = Quaternion.LookRotation(new Vector3(0f, 0.82f, 0f) - camera.transform.position, Vector3.up);

            var renderTexture = new RenderTexture(640, 800, 24);
            var previousTarget = camera.targetTexture;
            var previousActive = RenderTexture.active;
            camera.targetTexture = renderTexture;

            var clip = LoadClip(clipPath);
            var rigPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(RigPath);
            for (var frame = 0; frame < frameCount; frame++)
            {
                var rig = PrefabUtility.InstantiatePrefab(rigPrefab) as GameObject;
                if (rig == null)
                {
                    throw new System.Exception("[BubuTown] Could not instantiate preview rig.");
                }

                rig.transform.position = Vector3.zero;
                rig.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                rig.transform.localScale = Vector3.one;
                ApplyDaMoguSkinnedModelV1(rig);
                var time = clip.length * frame / frameCount;
                clip.SampleAnimation(rig, time);

                camera.Render();
                RenderTexture.active = renderTexture;
                var image = new Texture2D(640, 800, TextureFormat.RGB24, false);
                image.ReadPixels(new Rect(0f, 0f, 640, 800), 0, 0);
                image.Apply();
                File.WriteAllBytes(Path.Combine(outputFolder, frame.ToString("000") + ".png"), image.EncodeToPNG());
                Object.DestroyImmediate(image);
                Object.DestroyImmediate(rig);
            }

            camera.targetTexture = previousTarget;
            RenderTexture.active = previousActive;
            Object.DestroyImmediate(renderTexture);
            EditorSceneManager.CloseScene(scene, true);
        }

        private static void ApplyDaMoguSkinnedModelV1(GameObject rigRoot)
        {
            BuildDaMoguPlayableModel(rigRoot);
        }

        private static void BuildDaMoguPlayableModel(GameObject rigRoot)
        {
            ShowImportedBodyAsAnimationUnderlay(rigRoot);
            RemoveChild(rigRoot.transform, PlayableModelName);
            RemoveExistingDaMoguParts(rigRoot.transform);

            var model = new GameObject(PlayableModelName);
            model.transform.SetParent(rigRoot.transform, false);
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = Vector3.one;

            var hips = FindDeepChild(rigRoot.transform, "Hips");
            var spine = FindDeepChild(rigRoot.transform, "Spine");
            var chest = FindDeepChild(rigRoot.transform, "Chest");
            var head = FindDeepChild(rigRoot.transform, "Head");
            var leftUpperArm = FindDeepChild(rigRoot.transform, "LeftUpperArm");
            var rightUpperArm = FindDeepChild(rigRoot.transform, "RightUpperArm");
            var leftLowerArm = FindDeepChild(rigRoot.transform, "LeftLowerArm");
            var rightLowerArm = FindDeepChild(rigRoot.transform, "RightLowerArm");
            var leftHand = FindDeepChild(rigRoot.transform, "LeftHand");
            var rightHand = FindDeepChild(rigRoot.transform, "RightHand");
            var leftUpperLeg = FindDeepChild(rigRoot.transform, "LeftUpperLeg");
            var rightUpperLeg = FindDeepChild(rigRoot.transform, "RightUpperLeg");
            var leftLowerLeg = FindDeepChild(rigRoot.transform, "LeftLowerLeg");
            var rightLowerLeg = FindDeepChild(rigRoot.transform, "RightLowerLeg");
            var leftFoot = FindDeepChild(rigRoot.transform, "LeftFoot");
            var rightFoot = FindDeepChild(rigRoot.transform, "RightFoot");

            AddRigPart(rigRoot.transform, hips, PrimitiveType.Cube, "Navy_Skort_BackPanel", new Vector3(0f, -0.01f, 0.02f), Vector3.zero, new Vector3(0.48f, 0.22f, 0.32f), ModelMaterial("Navy"));
            AddRigPart(rigRoot.transform, hips, PrimitiveType.Cube, "Navy_Skort_FrontPanel", new Vector3(0f, 0.01f, -0.12f), new Vector3(4f, 0f, 0f), new Vector3(0.42f, 0.16f, 0.08f), ModelMaterial("Navy"));
            AddRigPart(rigRoot.transform, hips, PrimitiveType.Cube, "Navy_InnerShorts_Read", new Vector3(0f, -0.09f, 0f), Vector3.zero, new Vector3(0.34f, 0.08f, 0.22f), ModelMaterial("NavyDark"));
            AddRigPart(rigRoot.transform, spine, PrimitiveType.Cube, "White_Inner_Top_Read", new Vector3(0f, 0.07f, -0.02f), Vector3.zero, new Vector3(0.30f, 0.30f, 0.16f), ModelMaterial("White"));
            AddRigPart(rigRoot.transform, chest, PrimitiveType.Cube, "White_Windbreaker_UpperPanel", new Vector3(0f, 0.11f, -0.01f), Vector3.zero, new Vector3(0.45f, 0.18f, 0.23f), ModelMaterial("White"));
            AddRigPart(rigRoot.transform, chest, PrimitiveType.Cube, "Pink_Windbreaker_LowerPanel", new Vector3(0f, -0.06f, -0.01f), Vector3.zero, new Vector3(0.46f, 0.30f, 0.24f), ModelMaterial("SoftPink"));
            AddRigPart(rigRoot.transform, chest, PrimitiveType.Cube, "Pink_Windbreaker_Hem", new Vector3(0f, -0.22f, -0.01f), Vector3.zero, new Vector3(0.48f, 0.06f, 0.25f), ModelMaterial("PinkHem"));
            AddRigPart(rigRoot.transform, chest, PrimitiveType.Cube, "White_Hood_BackRead", new Vector3(0f, 0.20f, 0.10f), new Vector3(18f, 0f, 0f), new Vector3(0.36f, 0.18f, 0.14f), ModelMaterial("White"));
            // The crossbody bag is intentionally deferred until hands and sleeves read cleanly in motion.

            AddRigPart(rigRoot.transform, head, PrimitiveType.Sphere, "Soft_Mature_Anime_Head", new Vector3(0f, 0.05f, 0f), Vector3.zero, new Vector3(0.2f, 0.24f, 0.18f), ModelMaterial("Skin"));
            AddRigPart(rigRoot.transform, head, PrimitiveType.Sphere, "DarkBrown_Hair_Cap", new Vector3(0f, 0.11f, 0.02f), Vector3.zero, new Vector3(0.22f, 0.18f, 0.2f), ModelMaterial("Hair"));
            AddRigPart(rigRoot.transform, head, PrimitiveType.Cube, "Shoulder_Length_Hair_Back", new Vector3(0f, -0.06f, 0.12f), Vector3.zero, new Vector3(0.30f, 0.24f, 0.08f), ModelMaterial("Hair"));
            AddRigPart(rigRoot.transform, head, PrimitiveType.Sphere, "Small_HalfUp_Bun", new Vector3(0.1f, 0.18f, 0.1f), Vector3.zero, new Vector3(0.1f, 0.08f, 0.1f), ModelMaterial("Hair"));
            AddRigPart(rigRoot.transform, head, PrimitiveType.Sphere, "Left_Eye_Dark", new Vector3(-0.055f, 0.06f, -0.155f), Vector3.zero, new Vector3(0.025f, 0.018f, 0.01f), ModelMaterial("Eye"));
            AddRigPart(rigRoot.transform, head, PrimitiveType.Sphere, "Right_Eye_Dark", new Vector3(0.055f, 0.06f, -0.155f), Vector3.zero, new Vector3(0.025f, 0.018f, 0.01f), ModelMaterial("Eye"));
            AddRigPart(rigRoot.transform, head, PrimitiveType.Cube, "Small_Natural_Mouth", new Vector3(0f, 0.005f, -0.165f), Vector3.zero, new Vector3(0.055f, 0.008f, 0.008f), ModelMaterial("Mouth"));

            AddRigPart(rigRoot.transform, leftUpperLeg, PrimitiveType.Capsule, "Left_Upper_Leg_Skin", Vector3.zero, Vector3.zero, new Vector3(0.09f, 0.24f, 0.09f), ModelMaterial("Skin"));
            AddRigPart(rigRoot.transform, rightUpperLeg, PrimitiveType.Capsule, "Right_Upper_Leg_Skin", Vector3.zero, Vector3.zero, new Vector3(0.09f, 0.24f, 0.09f), ModelMaterial("Skin"));
            AddRigPart(rigRoot.transform, leftLowerLeg, PrimitiveType.Capsule, "Left_Lower_Leg_Skin", Vector3.zero, Vector3.zero, new Vector3(0.075f, 0.23f, 0.075f), ModelMaterial("Skin"));
            AddRigPart(rigRoot.transform, rightLowerLeg, PrimitiveType.Capsule, "Right_Lower_Leg_Skin", Vector3.zero, Vector3.zero, new Vector3(0.075f, 0.23f, 0.075f), ModelMaterial("Skin"));
            AddRigPart(rigRoot.transform, leftFoot, PrimitiveType.Cube, "Left_PinkWhite_Sneaker", new Vector3(0f, 0f, -0.045f), Vector3.zero, new Vector3(0.11f, 0.07f, 0.22f), ModelMaterial("ShoePink"));
            AddRigPart(rigRoot.transform, rightFoot, PrimitiveType.Cube, "Right_PinkWhite_Sneaker", new Vector3(0f, 0f, -0.045f), Vector3.zero, new Vector3(0.11f, 0.07f, 0.22f), ModelMaterial("ShoePink"));
        }

        private static void ShowImportedBodyAsAnimationUnderlay(GameObject root)
        {
            var underlayMaterial = ModelMaterial("UnderlaySkin");
            foreach (var renderer in root.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer.transform.parent == null || renderer.transform.name == PlayableModelName)
                {
                    continue;
                }

                renderer.enabled = true;
                renderer.sharedMaterial = underlayMaterial;
            }
        }

        private static GameObject AddPart(Transform modelRoot, Transform bone, PrimitiveType type, string name, Vector3 localPosition, Vector3 localEuler, Vector3 localScale, Material material)
        {
            var parent = bone != null ? bone : modelRoot;
            var part = GameObject.CreatePrimitive(type);
            part.name = "DaMoguV1_" + name;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPosition;
            part.transform.localRotation = Quaternion.Euler(localEuler);
            part.transform.localScale = localScale;
            var collider = part.GetComponent<Collider>();
            if (collider != null)
            {
                Object.DestroyImmediate(collider);
            }

            var renderer = part.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = material;
            }
            return part;
        }

        private static GameObject AddRigPart(Transform rigRoot, Transform bone, PrimitiveType type, string name, Vector3 rigSpaceOffset, Vector3 worldEuler, Vector3 localScale, Material material)
        {
            var parent = bone != null ? bone : rigRoot;
            var part = GameObject.CreatePrimitive(type);
            part.name = "DaMoguV1_" + name;
            part.transform.position = parent.position + rigRoot.TransformDirection(rigSpaceOffset);
            part.transform.rotation = rigRoot.rotation * Quaternion.Euler(worldEuler);
            part.transform.localScale = localScale;
            part.transform.SetParent(parent, true);
            var collider = part.GetComponent<Collider>();
            if (collider != null)
            {
                Object.DestroyImmediate(collider);
            }

            var renderer = part.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = material;
            }
            return part;
        }

        private static void RemoveExistingDaMoguParts(Transform root)
        {
            var childrenToDestroy = new System.Collections.Generic.List<GameObject>();
            CollectExistingDaMoguParts(root, childrenToDestroy);
            foreach (var child in childrenToDestroy)
            {
                Object.DestroyImmediate(child);
            }
        }

        private static void CollectExistingDaMoguParts(Transform root, System.Collections.Generic.List<GameObject> parts)
        {
            foreach (Transform child in root)
            {
                if (child.name.StartsWith("DaMoguV1_", System.StringComparison.Ordinal))
                {
                    parts.Add(child.gameObject);
                    continue;
                }

                CollectExistingDaMoguParts(child, parts);
            }
        }

        private static Transform FindDeepChild(Transform root, string name)
        {
            if (root.name == name)
            {
                return root;
            }

            foreach (Transform child in root)
            {
                var match = FindDeepChild(child, name);
                if (match != null)
                {
                    return match;
                }
            }

            return null;
        }

        private static Material ModelMaterial(string name)
        {
            var folder = ControllerFolder + "/Materials";
            Directory.CreateDirectory(folder);
            var path = folder + "/DaMogu_ModelV1_" + name + ".mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
                AssetDatabase.CreateAsset(material, path);
            }

            material.color = ModelColor(name);
            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", ModelColor(name));
            }
            if (material.HasProperty("_Smoothness"))
            {
                material.SetFloat("_Smoothness", 0.28f);
            }
            EditorUtility.SetDirty(material);
            return material;
        }

        private static Color ModelColor(string name)
        {
            switch (name)
            {
                case "Skin":
                    return new Color(0.96f, 0.76f, 0.66f);
                case "UnderlaySkin":
                    return new Color(0.96f, 0.80f, 0.72f);
                case "Hair":
                    return new Color(0.16f, 0.10f, 0.08f);
                case "SoftPink":
                    return new Color(0.96f, 0.58f, 0.68f);
                case "PinkHem":
                    return new Color(0.95f, 0.48f, 0.62f);
                case "White":
                    return new Color(0.95f, 0.93f, 0.88f);
                case "Navy":
                    return new Color(0.05f, 0.07f, 0.16f);
                case "NavyDark":
                    return new Color(0.03f, 0.04f, 0.10f);
                case "BagCream":
                    return new Color(0.92f, 0.74f, 0.62f);
                case "BagStrap":
                    return new Color(0.96f, 0.72f, 0.78f);
                case "ShoePink":
                    return new Color(0.98f, 0.78f, 0.82f);
                case "Eye":
                    return new Color(0.04f, 0.03f, 0.03f);
                case "Mouth":
                    return new Color(0.72f, 0.25f, 0.28f);
                default:
                    return Color.white;
            }
        }

        private static void FillTexture(Texture2D texture, Color32 color)
        {
            var pixels = new Color32[texture.width * texture.height];
            for (var i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }
            texture.SetPixels32(pixels);
            texture.Apply();
        }

        private static void EnsureRequiredFiles()
        {
            RequireFile(RigPath);
            RequireFile(IdlePath);
            RequireFile(WalkPath);
            RequireFile(RunPath);
            RequireFile(SprintPath);
        }

        private static void RequireFile(string assetPath)
        {
            if (!File.Exists(assetPath))
            {
                throw new System.Exception("[BubuTown] Missing prototype locomotion asset: " + assetPath);
            }
        }

        private static void RemoveChild(Transform parent, string childName)
        {
            var child = parent.Find(childName);
            if (child != null)
            {
                Object.DestroyImmediate(child.gameObject);
            }
        }

        private static void WriteReport()
        {
            var report =
                "# DaMogu Humanoid Locomotion Prototype Report\n\n" +
                "Status: imported for temporary animation-pipeline validation.\n\n" +
                "Source: Unity-Technologies/Standard-Assets-Characters, female third-person rig and locomotion clips.\n\n" +
                "License: Unity Companion License for Unity-dependent projects. This is suitable as a Unity-only prototype dependency, not the final public character identity.\n\n" +
                "Imported assets:\n" +
                "- `Rig/defaultfemale_rig.fbx`\n" +
                "- `Animations/Exploration/f@Idle.fbx`\n" +
                "- `Animations/Exploration/f@WalkForwards.fbx`\n" +
                "- `Animations/Exploration/f@RunForwards.fbx`\n" +
                "- `Animations/Exploration/f@SprintForwards.fbx`\n\n" +
                "Runtime scene hookup:\n" +
                "- Player root: `08_Player_And_Runtime/Player_Start_Bubu`\n" +
                "- Runtime animated child: `Humanoid_Locomotion_Runtime_Prototype`\n" +
                "- Playable visual marker: `DaMogu_Playable_Model_v1`\n" +
                "- Animator controller: `Bubu_DaMogu_Locomotion.controller`\n" +
                "- Parameters driven by `BubuTownLocomotionAnimator`: `Speed`, `MoveX`, `MoveY`, `Grounded`, `Sprinting`\n\n" +
                "Playable model v0.3 note: the current motion-preview DaMogu visual uses the Humanoid rig with simple bone-attached readable clothing parts: pink-white windbreaker, navy skort/short skirt, bare-leg read, pink-white sneakers, brown bob/half-up bun, and a crossbody bag. It is suitable for checking movement feel and clothing readability together, but it is not the final polished anime model.\n\n" +
                "Art direction note: the final DaMogu model should still be an original anime-style seaside urban character, then retargeted onto this same Animator pipeline.\n";
            File.WriteAllText(ReportPath, report);
        }
    }
}
