using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DestinyRanger.EditorTools
{
    public static class DestinyRangerSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/DestinyRangerPrototype.unity";
        private const string ControllerName = "DestinyRangerSideScroller";
        private const string ConceptPath = "Assets/DestinyRanger/Art/Generated/destiny-ranger-concept.png";
        private const string RuneSheetPath = "Assets/DestinyRanger/Art/Generated/destiny-ranger-rune-ui.png";
        private const string CharacterSheetPath = "Assets/DestinyRanger/Art/Generated/destiny-ranger-sprite-sheet.png";
        private const string RuneIconSheetPath = "Assets/DestinyRanger/Art/Generated/destiny-ranger-rune-icons-sheet.png";
        private const string AdventureStagePath = "Assets/DestinyRanger/Art/Generated/adventure-stage-forest-long-v1.png";
        private const string AdventureHeroPath = "Assets/DestinyRanger/Art/Generated/adventure-hero-anim-32-v1.png";
        private const string AdventureEnemyPath = "Assets/DestinyRanger/Art/Generated/adventure-enemy-anim-24-v21-forest-qstyle.png";
        private const string AdventureUiPath = "Assets/DestinyRanger/Art/Generated/adventure-ui-sheet.png";
        private const string AdventurePlatformPath = "Assets/DestinyRanger/Art/Generated/adventure-platform-sheet-v8-painted-solid.png";
        private const string AdventureGroundWallPath = "Assets/DestinyRanger/Art/Generated/adventure-ground-wall-v2-painted-solid.png";
        private const string AdventureForegroundPropsPath = "Assets/DestinyRanger/Art/Generated/adventure-foreground-props-v1.png";
        private const string AdventureForegroundGrassPath = "Assets/DestinyRanger/Art/Generated/adventure-foreground-grass-strip-v1.png";
        private const string AdventureAirDecorPath = "Assets/DestinyRanger/Art/Generated/adventure-air-decor-sheet-v2-clean.png";
        private const string AdventureControlUiPath = "Assets/DestinyRanger/Art/Generated/adventure-hud-controls-v2.png";
        private const string AdventureRuneUiPath = "Assets/DestinyRanger/Art/Generated/adventure-rune-ui-sheet.png";
        private const string AdventureCombatVfxPath = "Assets/DestinyRanger/Art/Generated/adventure-combat-vfx-sheet.png";
        private const string AdventureCompanionPetPath = "Assets/DestinyRanger/Art/Generated/adventure-companion-pet-v1.png";
        private const string ExternalVfxRoot = "Assets/DestinyRanger/Art/ExternalVfx";

        [MenuItem("Destiny Ranger/Create Prototype Scene")]
        public static void CreatePrototypeScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var cameraGo = new GameObject("Main Camera");
            cameraGo.tag = "MainCamera";
            var camera = cameraGo.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 5.5f;
            camera.backgroundColor = Parse("#101A2D");
            cameraGo.transform.position = new Vector3(0, 0, -10);

            var controller = new GameObject(ControllerName);
            var prototype = controller.AddComponent<DestinyRangerSideScroller>();
            EnsureSpriteImport(AdventureStagePath);
            EnsureTextureImport(AdventureHeroPath);
            EnsureTextureImport(AdventureEnemyPath);
            EnsureTextureImport(AdventureUiPath);
            EnsureTextureImport(AdventurePlatformPath);
            EnsureTextureImport(AdventureGroundWallPath);
            EnsureTextureImport(AdventureForegroundPropsPath);
            EnsureTextureImport(AdventureForegroundGrassPath);
            EnsureTextureImport(AdventureAirDecorPath);
            EnsureTextureImport(AdventureControlUiPath);
            EnsureTextureImport(AdventureRuneUiPath);
            EnsureTextureImport(AdventureCombatVfxPath);
            EnsureTextureImport(AdventureCompanionPetPath);
            EnsureExternalVfxImport();
            AssignOptionalSprite(prototype, "stageBackground", AdventureStagePath);
            AssignOptionalTexture(prototype, "heroSheet", AdventureHeroPath);
            AssignOptionalTexture(prototype, "enemySheet", AdventureEnemyPath);
            AssignOptionalTexture(prototype, "uiSheet", AdventureUiPath);
            AssignOptionalTexture(prototype, "platformSheet", AdventurePlatformPath);
            AssignOptionalTexture(prototype, "groundWallTexture", AdventureGroundWallPath);
            AssignOptionalTexture(prototype, "foregroundPropsSheet", AdventureForegroundPropsPath);
            AssignOptionalTexture(prototype, "foregroundGrassTexture", AdventureForegroundGrassPath);
            AssignOptionalTexture(prototype, "airDecorSheet", AdventureAirDecorPath);
            AssignOptionalTexture(prototype, "controlUiSheet", AdventureControlUiPath);
            AssignOptionalTexture(prototype, "runeUiSheet", AdventureRuneUiPath);
            AssignOptionalTexture(prototype, "combatVfxSheet", AdventureCombatVfxPath);
            AssignOptionalTexture(prototype, "companionPetSheet", AdventureCompanionPetPath);
            AssignTextureArrayFromFolder(prototype, "externalQuickSlashFrames", ExternalVfxRoot + "/E53138_QuickSlash");
            AssignTextureArrayFromFolder(prototype, "externalSwordArcFrames", ExternalVfxRoot + "/E53137_SwordArc", "jian");
            AssignTextureArrayFromFolder(prototype, "externalSwordWaveFrames", ExternalVfxRoot + "/E53073_LightningSword", "jian");
            AssignTextureArrayFromFolder(prototype, "externalLightningFrames", ExternalVfxRoot + "/E53073_LightningSword", "shandian");
            AssignTextureArrayFromFolder(prototype, "externalHeavyBurstFrames", ExternalVfxRoot + "/E53110_HeavyBurst", "4_");
            AssignTextureArrayFromFolder(prototype, "externalRuneEnergyFrames", ExternalVfxRoot + "/E53069_RuneEnergy", "a_");
            AssignTextureArrayFromFolder(prototype, "externalBossImpactFrames", ExternalVfxRoot + "/E53130_BossImpact", "7_");

            Directory.CreateDirectory("Assets/Scenes");
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.Refresh();
            Debug.Log("Destiny Ranger prototype scene created: " + ScenePath);
        }

        public static void CreatePrototypeSceneBatch()
        {
            CreatePrototypeScene();
        }

        public static void BatchHeartbeat()
        {
            File.WriteAllText("/private/tmp/destiny-ranger-batch-heartbeat.txt", System.DateTime.UtcNow.ToString("O"));
            Debug.Log("Destiny Ranger batch heartbeat wrote /private/tmp/destiny-ranger-batch-heartbeat.txt");
        }

        public static void RenderPrototypePreviewBatch()
        {
            CreatePrototypeScene();
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var controller = Object.FindObjectOfType<DestinyRangerSideScroller>();
            if (!controller)
                throw new System.InvalidOperationException("DestinyRangerSideScroller is missing from " + scene.path);

            typeof(DestinyRangerSideScroller)
                .GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.Invoke(controller, null);

            RequireObject("GeneratedForestStage");
            RequireObject("AdventureGeneratedForegroundGrassStrip_0");
            RequireObject("AdventureGeneratedForegroundGrassContactShade");
            RequireObject("Hero_Lightblade");
            RequireObject("HeroContrastSilhouette");
            RequireObject("HeroReadableTorso");
            RequireObject("HeroReadableHead");
            RequireObject("HeroReadableCape");
            RequireObject("HeroReadableBlade");
            RequireObject("HeroReadabilityAura_DisabledNoGroundBlob");
            RequireObject("HeroLandingPredictor");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "ComputeCameraLookAhead", "Camera lookahead framing is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "CameraForwardLookAhead", "Camera forward lookahead constant is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "CameraBacktrackLookAhead", "Camera backtrack lookahead constant is missing.");
            RequireObject("CompanionSpirit");
            RequireObject("WorldSpaceControlHints");
            RequireObject("WorldJoystick");
            RequireObject("WorldButton_攻击");
            RequireObject("RuneOpen");
            RequireObject("RuneSealPanel");
            RequireObject("RuneSealTitle");
            RequireObject("RuneResonanceGrade");
            RequireObject("RuneSlot_0");
            RequireObject("RuneSlot_2");
            RequireObject("RuneSlotText_0");
            RequireObject("RuneSlotText_2");
            RequireObject("RuneRewardText");
            RequireObject("RuneBuildDeltaText");
            RequireObject("RuneComplianceText");
            RequireObject("NextSlotHintText");
            RequireObject("RoomRewardPanel");
            RequireObject("RoomRewardTitle");
            RequireObject("RoomRewardHint");
            RequireObject("RoomRewardOption_0");
            RequireObject("RoomRewardIcon_0");
            RequireObject("RoomRewardRarityStrip_0");
            RequireObject("RoomRewardTypeBadge_0");
            RequireObject("RoomRewardValue_0");
            RequireObject("RoomRewardSynergyHint_0");
            RequireObject("RoomRewardRecommend_0");
            RequireObject("RoomRewardOption_2");
            RequireObject("RoomRewardIcon_2");
            RequireObject("RoomRewardTypeBadge_2");
            RequireObject("RoomRewardSynergyHint_2");
            RequireObject("RoomRewardRecommend_2");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "RoomRewardSynergyHint", "Room reward build synergy hint is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "RoomRewardTypeLabel", "Room reward type badge label is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "RoomRewardTypeColor", "Room reward type badge color is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "SLOT", "Room reward SLOT type label is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "MarkRecommendedRoomReward", "Room reward recommendation marker is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "RoomRewardRecommendationScore", "Room reward recommendation scoring is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "推荐 ·", "Room reward recommended short label is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "CaptureRuneBuildSnapshot", "Rune build snapshot before SLOT roll is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "RuneBuildDeltaText", "Rune build delta result text is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "AppendRuneDelta", "Rune build before/after comparison is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "BuildNextSlotHintText", "Next SLOT anticipation HUD is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "再 {remain}% 出奖励 SLOT", "Next SLOT anticipation microcopy is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "RuneAnticipationThreshold", "Rune near-ready anticipation threshold is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "将启封", "Rune near-ready button label is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "临界 {Mathf.FloorToInt(fateEnergy)}%", "Rune near-ready Next SLOT microcopy is missing.");
            RequireObject("StepPlatform_A");
            RequireObject("AdventureSolidGround_DirtCore");
            RequireObject("AdventureSolidGround_MossyWalkableCap");
            RequireObject("AdventureGroundWallArt_FullBottomCoverage_0");
            RequireObject("AdventureGeneratedForegroundGrassStrip_0");
            RequireObject("AdventureGeneratedForegroundGrassContactShade");
            RequireTextureGrid(AdventurePlatformPath, 4, 1, "Clean platform 4-frame sheet");
            RequireObject("GeneratedMapLayout");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "BuildStageMapLayout", "Stage map layout generator is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "BuildMapLayout_CanopyClimb", "Canopy climb map variant is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "BuildMapLayout_BrokenBridge", "Broken bridge map variant is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "BuildMapLayout_RuneRidge", "Rune ridge map variant is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "StageMapLayoutIndex", "Seeded stage map layout selection is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/MAP_VARIATION_SPEC.md", "RenderMapPreviewBatch", "Map variation screenshot spec is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/MAP_VARIATION_SPEC.md", "PreviewMapRuneJar_Left/Mid/Right", "Map preview rune jar acceptance check is missing.");
            RequireObject("RoomGate_0");
            RequireObject("RoomGateDoorL_0");
            RequireObject("RoomGateState_0");
            RequireObject("RoomGate_7");
            RequireObject("RoomGateDoorR_7");
            RequireObject("RoomGateState_7");
            RequireObject("RoomGateSpecialBand_3");
            RequireObject("RoomGateSpecialBadge_3");
            RequireObject("RoomGateSpecialBand_5");
            RequireObject("RoomGateSpecialBadge_5");
            RequireObject("RoomGateSpecialBand_7");
            RequireObject("RoomGateSpecialBadge_7");
            RequireObject("RuneProgressGate_A");
            RequireObject("RuneProgressGate_G");
            RequireObject("BrokenRuneJar_A");
            RequireObject("PlatformRewardGuideText_BrokenRuneJar_A");
            RequireObject("EnemyIntentReadRing");
            RequireObject("EnemyContactAo_CleanEllipse");
            RequireObject("EnemyIntentReadText");
            RequireObject("RuneSupplyShop");
            RequireObject("AdventureHudCanvas");
            RequireObject("SafeAreaRoot");
            RequireObject("ObjectiveBackplate");
            RequireObject("NoticeBackplate");
            RequireObject("RunProgressText");
            RequireObject("RoomTitleText");
            RequireObject("TutorialText");
            RequireObject("FirstRunNudgeText");
            RequireObject("FirstRunNudgeBackplate");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "RoomModifier", "Room modifier system is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "RoomObjective", "Room objective system is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "ObjectiveRuneJar", "Objective rune jar room goal is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "PlatformRewardGuide", "Platform reward guide marker is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "ActivePlatformRuneVesselInSegment", "Platform rune objective HUD hint is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "UpdatePlatformRouteHint", "Platform rune route hint update is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "上推跳跃，攻击开箱", "Platform rune route action microcopy is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "可跳上平台开箱拿 SLOT 能量，也可继续推进", "Optional platform rune objective microcopy is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "TimedClear", "Timed room challenge is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "RuneBadge_\" + labels[i]", "Rune badge creation is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "runeBadgeImages[i].gameObject.SetActive(active)", "Rune badge inactive-state logic is missing.");
            RequireObject("BuildStyleBackplate");
            RequireObject("BuildStyleText");
            RequireObject("RouteMinimap");
            RequireObject("PlatformRouteHintText");
            RequireObject("RouteNode_7");
            RequireObject("BossHud");
            RequireObject("BossHudHpFill");
            RequireObject("BossPressureFill");
            RequireObject("BossPressureText");
            RequireObject("BossHudAttackText");
            RequireObject("BossResponseHintText");
            RequireObject("BossHudPhaseText");
            RequireObject("BossHudPhaseMarker");
            RequireObject("BossArenaLockdown");
            RequireObject("BossArenaLeftLockEdgeHint_ThinNoPlate");
            RequireObject("BossArenaRightLockEdgeHint_ThinNoPlate");
            RequireObject("BossArenaGroundSeal_HairlineNoPlate");
            RequireObject("BossArenaLeftLockCoreTick_NoYellowPlate");
            RequireObject("BossArenaRightLockCoreTick_NoYellowPlate");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "SpawnBossPhaseTransition", "Boss phase transition readability is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "UpdateBossPressureHud", "Boss pressure HUD is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "BuildBossArenaLockdown", "Boss arena lockdown visuals are missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "UpdateBossArenaLockdown", "Boss arena lockdown update is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "锁场中", "Boss lockdown objective microcopy is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "UpdateRoomGateSpecialBadge", "Special room gate badge update is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "RoomGateSpecialBadgeLabel", "Special room gate badge labels are missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "RoomGateSpecialBadgeColor", "Special room gate badge colors are missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "补给", "Shop room gate badge microcopy is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "BuildBossResponseHint", "Boss response hint builder is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "保留闪避处理第二段", "Boss tactical response hint microcopy is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "SpawnBossDefeatCeremony", "Boss defeat ceremony is missing.");
            RequireObject("EnemyStatusText");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "UpdateEnemyStatusReadability", "Enemy status readability labels are missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "ApplyEnemyFreeze", "Enemy freeze status behavior is missing.");
            RequireObject("DangerVignette");
            RequireObject("DamageDirectionCue");
            RequireObject("TargetCompassBackplate");
            RequireObject("TargetCompassText");
            RequireObject("ThreatAlertBackplate");
            RequireObject("ThreatAlertText");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "ShowDamageDirection", "Damage direction cue is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "ShowThreatAlert", "High-threat HUD attack announce is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "UpdateThreatAlertHud", "High-threat HUD attack announce update is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "UpdateTargetCompass", "Target/threat direction compass update is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "TryFindCombatCompassTarget", "Combat target compass selection is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "TryFindObjectiveCompassTarget", "Objective target compass selection is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "IsWorldXOutsideCamera", "Offscreen target compass detection is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "Boss 巨拳砸地", "Boss ground pound HUD announce is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "弓手瞄准", "Archer HUD threat announce is missing.");
            RequireObject("LowHealthHintText");
            RequireObject("HurtRecoveryText");
            RequireObject("HurtRecoveryBackplate");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "LowHealthHintText", "Low health tactical hint is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "BuildHurtRecoveryHint", "Player hurt recovery hint is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "受击恢复：短暂无敌", "Player hurt recovery microcopy is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "SpawnDodgeNowHint", "Enemy high-threat dodge hint is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "ClearNearbyEnemyProjectiles", "Perfect dodge projectile clear reward is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "AddDangerPattern", "Shape-coded danger warning patterns are missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "DangerPatternKind.Stripes", "Striped danger warning pattern is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "DangerPatternKind.Crosshair", "Crosshair danger warning pattern is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "DangerPatternKind.ToxicWaves", "Toxic wave danger warning pattern is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "AddDangerPatternGlyph", "Danger warning glyph layer is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "DangerPatternDamageGlyph", "Damage warning glyph is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "DangerPatternToxicGlyph", "Toxic warning glyph is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "DangerPatternCrosshairGlyph", "Crosshair warning glyph is missing.");
            RequireObject("CombatFlash");
            RequireObject("ComboBackplate");
            RequireObject("ComboText");
            RequireObject("ComboHeatBar");
            RequireObject("ComboHeatFill");
            RequireObject("HitConfirmBackplate");
            RequireObject("HitConfirmText");
            RequireObject("HitConfirmTierText");
            RequireObject("HitConfirmImpactBar");
            RequireObject("HitConfirmImpactFill");
            RequireObject("HeroActionBeatBackplate");
            RequireObject("HeroActionBeatText");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "SpawnKillRewardConfirmation", "Kill reward confirmation feedback is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "ShowMergedResourcePickupText", "Pickup collection confirmation feedback is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "ShowHitConfirm", "Mobile hit confirmation HUD is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "UpdateHitConfirmHud", "Mobile hit confirmation HUD update is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "hitConfirmImpactPower", "Mobile hit strength HUD is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "PulseHitConfirmButton", "Mobile hit-confirm button rebound is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "SourceSkill", "Skill hit-confirm button source is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "hitConfirmTierLabel = \"\";", "Light hit source should stay compact.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "破势 / 强命中", "Heavy hit source label is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "术式 / 范围命中", "Skill hit source label is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "SpawnHeroCleanActionStreak", "Hero clean action streak cue is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "HeroDodgeCleanStreak_NoEchoBody_NoTexturePlate", "Dodge clean streak cue is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "HeroSkillSharpCastCore_NoEchoBody_NoYellowPlate", "Skill clean cast cue is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "TriggerHitFrameBeat", "Unified hit-frame beat is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "HitFrameBeatText", "Hit-frame beat label is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "HeavyHitStopDuration", "Tiered heavy hit stop duration is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "ShowHeroActionBeat", "Hero action beat HUD trigger is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "UpdateHeroActionBeatHud", "Hero action beat HUD update is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "ToggleHeroActionBeatHud", "Hero action beat settings toggle is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "DestinyRanger.HeroActionBeatHud", "Hero action beat local preference is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "正式默认关闭", "Hero action beat formal-default-off copy is missing.");
            RequireObject("Attack");
            RequireObject("AttackComboPip_0");
            RequireObject("AttackComboPip_2");
            RequireObject("AttackStepText");
            RequireObject("AttackStateText");
            RequireObject("AttackBufferText");
            RequireObject("JumpBufferText");
            RequireObject("DodgeBufferText");
            RequireObject("DodgeStateText");
            RequireObject("RuneStateText");
            RequireObject("Skill4");
            RequireObject("Skill1CostText");
            RequireObject("Skill4CostText");
            RequireObject("Skill1StateText");
            RequireObject("Skill2StateText");
            RequireObject("Skill3StateText");
            RequireObject("Skill4StateText");
            RequireObject("ThumbReachSafeZone");
            RequireObject("ThumbReachSafeZoneText");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "ApplyHitConfirmSafeLayout", "Hit-confirm HUD safe layout is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "命中提示会避开右手热区", "Control edit thumb-zone guidance is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "jumpBufferTimer", "Jump input buffer is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "dodgeBufferTimer", "Dodge input buffer is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "AttackInputBufferDuration", "Named attack input buffer duration is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "UpdateInputBufferIndicators", "Mobile input buffer visual indicators are missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "UpdateControlStateTexts", "Mobile button state microcopy is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "魔力不足", "Skill insufficient-resource microcopy is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "ReadyGlow", "Combat button ready glow factory is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "ApplyReadyGlow", "Combat button ready glow update is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "SkillReadyGlowFor", "Skill ready glow lookup is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "CombatControlsLocked", "Combat ready glow modal lock guard is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "Skill1ReadyGlow", "Skill ready glow object is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "RuneReadyGlow", "Rune ready glow object is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "破势可接", "Attack heavy-ready microcopy is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "缓存·破势", "Attack queued-input microcopy is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "预闪", "Dodge queued-input microcopy is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "jumpAxisHeldLastFrame", "Jump axis edge trigger is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "TriggerHeroPosePunch", "Hero pose punch feedback is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "RunFootstepInterval", "Hero run footstep rhythm feedback is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "ApplyAttackHitFrameImpulse", "Hero hit-frame body impulse is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "TriggerEnemyHitReaction", "Enemy hit-reaction pose punch is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "SpawnEnemyHitDirectionCue", "Enemy hit-direction visual cue is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "SpawnEnemyRecoilHitLines", "Enemy recoil hit-line readability cue is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "EnemyHitDirectionTrail", "Enemy hit-direction trail is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "EnemyHitStunGroundLine_NoYellowPlate", "Enemy hit-stun ground line is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "UpdateEnemyHitReaction", "Enemy hit-reaction recovery update is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "破势硬直", "Enemy heavy hit-stun readability text is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "UpdateAttackComboIndicator", "Attack combo mobile button indicator is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "AttackRhythmWindowFill", "Attack combo rhythm window fill is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "UpdateAttackRhythmWindow", "Attack combo rhythm window update is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "attackRhythmWindowText.text", "Attack rhythm heavy-window microcopy is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "Image.Type.Filled", "UI fill bars must use filled Image type for rhythm, HP and impact bars.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "JumpCutVelocityMultiplier", "Variable jump height cutoff tuning is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "AttachJumpButtonEvents", "Touch jump PointerDown/PointerUp handling is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "CutJumpHeight", "Short-tap jump cut behavior is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "SpawnJumpCutFeedback", "Short-tap airborne jump feedback is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "touchJumpHeld", "Touch jump held state is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "JumpCutDust", "Variable jump release feedback is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "AttachSkillPreviewEvents", "Mobile skill aim-preview pointer events are missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "ShowSkillAimPreview", "Mobile skill aim-preview display is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "UpdateSkillAimPreview", "Mobile skill aim-preview follow update is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "HideSkillAimPreview", "Mobile skill aim-preview cleanup is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "SkillAimPreviewGuideLine_NoPlate", "Directional skill aim-preview clean guide line is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "SkillAimPreviewThinAreaLine", "Area skill aim-preview clean area line is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "SpawnSkillReleaseBeat", "Skill release-frame confirmation feedback is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "SkillPosePunch", "Skill cast hero pose feedback is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "UseEmergencyHeroSprite", "Emergency hero visibility fallback is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "HapticTier", "Tiered haptic feedback is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "hapticCooldownTimer", "Haptic cooldown throttling is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "RewardHapticEcho", "Reward haptic echo is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "sfxHeavyHit", "Heavy hit procedural audio is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "sfxSkillCast", "Skill cast procedural audio is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "sfxBossWarn", "Boss warning procedural audio is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "sfxGateOpen", "Gate opening procedural audio is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "sfxPickup", "Pickup procedural audio is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "ApplyButtonReadiness", "Mobile button readiness feedback is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "AttachTouchDownFeedback", "Mobile touch-down visual feedback hook is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "PulseTouchDownFeedback", "Mobile touch-down pulse trigger is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "ApplyTouchDownEcho", "Mobile touch-down outer-rim echo is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "ResetTouchDownEcho", "Mobile touch-down echo reset is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "CombatControlMinTouchSize", "Combat control minimum touch size guard is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "ClampCombatControlPosition", "Combat control safe-area clamp is missing.");
            RequireObject("JoystickTouchZone");
            RequireObject("JoystickIntentText");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "MoveDynamicJoystickBase", "Dynamic mobile joystick base is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "UpdateJoystickVisualState", "Joystick active visual feedback is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "UpdateJoystickIntentText", "Joystick intent text feedback is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "UpdateJoystickIntentLock", "Joystick intent lock is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "JoystickAllowsSwipeDodge", "Joystick swipe dodge guard is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "ShowAutoAimTargetCue", "Auto aim target readability cue is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "AutoAimTargetFootReticle_NoText_NoTexturePlate", "Auto aim target reticle feedback is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "UpdateSkillCostText", "Skill MP cost text feedback is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "BuildNextRunChallenge", "Post-run next challenge prompt is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "BuildRunDiagnosis", "Post-run diagnosis line is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "BuildRecommendedNextBuild", "Post-run recommended build line is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "问题诊断", "Post-run diagnosis microcopy is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "推荐构筑", "Post-run recommended build microcopy is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "DailyTrialSeed", "Offline daily trial seed is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "UpdateDailyTrialRecords", "Local daily trial record tracking is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "BuildDailyTrialRunRecordLine", "Daily trial result record line is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "BuildDamageSourceSummary", "Run damage source summary is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "RecordDamageSource", "Damage source tracking is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "CycleTrialHeatLevel", "Trial heat title toggle is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "ApplyTrialHeatToEnemy", "Trial heat enemy scaling is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "DestinyRanger.TrialHeatLevel", "Trial heat local setting is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "HeatClear", "Trial heat achievement is missing.");
            RequireObject("PauseReasonText");
            RequireObject("ShopPanel");
            RequireObject("ShopHealIcon");
            RequireObject("ShopHealPrice");
            RequireObject("ShopHealState");
            RequireObject("ShopHealAdvice");
            RequireObject("ShopRuneIcon");
            RequireObject("ShopRunePrice");
            RequireObject("ShopRuneState");
            RequireObject("ShopRuneAdvice");
            RequireObject("ShopDecisionText");
            RequireObject("ShopComplianceText");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "ShopHealAdvice", "Shop heal decision advice is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "ShopRuneAdvice", "Shop rune decision advice is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "BuildShopDecisionText", "Shop total decision guide is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "低血先回血", "Shop low-health heal recommendation is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "尽快成型", "Shop rune build recommendation is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "成型：买 SLOT", "Shop build-first decision guide is missing.");
            RequireObject("TitlePanel");
            RequireObject("TitleStats");
            RequireObject("StartDailyTrial");
            RequireObject("TrialHeatToggle");
            RequireObject("TitleModeGuideText");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "BuildTitleModeGuideText", "Title mode guide text is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "今日试炼只保存本机最佳，不联网排行", "Title daily trial local-only guide is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "不改变 SLOT 概率", "Title heat SLOT odds disclosure is missing.");
            RequireObject("TitleAchievements");
            RequireObject("TitleRuneCodex");
            RequireObject("AchievementPanel");
            RequireObject("AchievementBody");
            RequireObject("AchievementPrivacyNote");
            RequireObject("AchievementClose");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "BuildTrackedAchievementHint", "Tracked achievement replay hint is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "AchievementProgressText", "Achievement progress text is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "BuildRouteArchiveSummary", "Route archive achievement summary is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "AchievementShortDescription", "Compact achievement description is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "AchievementStatusLabel", "Achievement status label is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "achievementBodyText.lineSpacing = .84f", "Achievement panel compact line spacing is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "进度：", "Achievement list progress microcopy is missing.");
            RequireObject("RuneCodexPanel");
            RequireObject("RuneCodexBody");
            RequireObject("RuneCodexCompliance");
            RequireObject("RuneCodexClose");
            RequireObject("RuneOddsText");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "UpdateRuneOddsText", "In-run rune odds HUD update is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "RuneOddsHudLine", "Reusable rune odds HUD disclosure line is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "约25%三星、57%二星、18%逆运祝福", "Rune codex odds disclosure is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "广告激励或联网排行", "Rune codex no ad/ranking disclosure is missing.");
            RequireObject("TitleAbout");
            RequireObject("OnboardingPanel");
            RequireObject("OnboardingCoreLoop");
            RequireObject("OnboardingStepIcon_0");
            RequireObject("OnboardingAssistLine");
            RequireObject("OnboardingStart");
            RequireObject("OnboardingSkip");
            RequireObject("SettingsOnboardingReplay");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "ShowOnboardingReplayFromSettings", "Settings onboarding replay entry is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "onboardingReplayMode", "Onboarding replay mode guard is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "返回设置", "Onboarding replay return label is missing.");
            RequireObject("Restart");
            RequireObject("VictoryStats");
            RequireObject("VictoryCauseText");
            RequireObject("VictoryAssistNoteText");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "victoryStatsText.lineSpacing = .92f", "Victory stats compact line spacing is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "CompactRunLedger", "Victory compact run ledger is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "CompactLedgerEntry", "Victory compact ledger entry is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "SpawnPlayerDefeatCeremony", "Player defeat ceremony feedback is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "BuildVictoryCauseLine", "Victory/failure cause line is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "倒在 {source}", "Failure cause microcopy is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "BuildFirstRunAssistAdvice", "First-run failure assist advice is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "BuildVictoryAssistNote", "Victory assist/retry note is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "BuildRunBuildReview", "Run build review line is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "BuildGradeLabel", "Run build grade label is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "BuildFocusRuneLabel", "Run build focus rune label is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "BuildWeaknessLabel", "Run build weakness label is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "构筑复盘", "Victory build-review microcopy is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "辅助模式只改动作容错", "Assist mode achievement note is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "FirstRunNudgeMessage", "First-run contextual nudge logic is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "平台小宝箱可选", "First-run platform rune nudge is missing.");
            RequireObject("VictoryTitleReturn");
            RequireObject("PauseRuneCodex");
            RequireObject("PauseSettings");
            RequireObject("SettingsPanel");
            RequireObject("SettingsReset");
            RequireObject("SettingsAbout");
            RequireObject("SettingsComfortBadges");
            RequireObject("AutoAttackToggle");
            RequireObject("ControlEditToggle");
            RequireObject("ControlEditHint");
            RequireObject("ControlOpacityToggle");
            RequireObject("HeroActionBeatToggle");
            RequireObject("VolumeSlider");
            RequireObject("VolumeSliderValue");
            RequireObject("SfxVolumeSlider");
            RequireObject("SfxVolumeSliderValue");
            RequireObject("MusicVolumeSlider");
            RequireObject("MusicVolumeSliderValue");
            RequireObject("EffectsToggle");
            RequireObject("FrameRateToggle");
            RequireObject("AboutPanel");
            RequireObject("AboutBody");
            RequireObject("AboutCompliance");
            RequireObject("AboutReleaseBlockerText");
            RequireObject("AboutClearState");
            RequireObject("AboutClearLocalData");
            RequireObject("AboutClose");

            var camera = Camera.main;
            if (!camera)
                throw new System.InvalidOperationException("Preview camera is missing.");

            const int width = 2732;
            const int height = 2048;
            var renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            var previousTarget = camera.targetTexture;
            var previousActive = RenderTexture.active;
            camera.targetTexture = renderTexture;
            RenderTexture.active = renderTexture;
            camera.Render();
            var output = new Texture2D(width, height, TextureFormat.RGBA32, false);
            output.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            output.Apply();
            File.WriteAllBytes("/private/tmp/destiny-ranger-preview.png", output.EncodeToPNG());
            camera.targetTexture = previousTarget;
            RenderTexture.active = previousActive;
            Object.DestroyImmediate(renderTexture);
            Object.DestroyImmediate(output);
            Debug.Log("Destiny Ranger preview rendered: /private/tmp/destiny-ranger-preview.png");
        }

        public static void RenderCombatPreviewBatch()
        {
            CreatePrototypeScene();
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var controller = Object.FindObjectOfType<DestinyRangerSideScroller>();
            if (!controller)
                throw new System.InvalidOperationException("DestinyRangerSideScroller is missing from " + scene.path);

            typeof(DestinyRangerSideScroller)
                .GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.Invoke(controller, null);
            controller.DebugBuildCombatPreviewPose();
            DumpLargeVisibleSpriteRenderers("/private/tmp/destiny-ranger-combat-renderers.txt");

            RequireObject("CompanionSpirit");
            RequireObject("PreviewSlashCoreLine_NoYellowPlate");
            RequireObject("PreviewSlashEdgeLine_NoYellowPlate");
            RequireObject("PreviewSlashFootAnchor_NoYellowPlate");
            RequireObject("PreviewPetShot_NoOcclude");
            RequireObject("PreviewThunderSpine_NoYellowPlate");
            RequireObject("PreviewThunderContact_NoYellowPlate");
            RequireObject("PreviewPerfectDodgeFx_NoYellowPlate");
            RequireObject("PreviewPerfectDodgeConfirmTick_NoYellowPlate");
            RequireObject("PreviewBossWarn");
            RequireObject("PreviewDamageConfirmTick_NoYellowPlate");
            RequireObject("HitConfirmText");
            RequireObject("HeroReadabilityAura_DisabledNoGroundBlob");
            RequireObject("DangerVignette");

            var camera = Camera.main;
            if (!camera)
                throw new System.InvalidOperationException("Preview camera is missing.");

            const int width = 2732;
            const int height = 2048;
            var renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            var previousTarget = camera.targetTexture;
            var previousActive = RenderTexture.active;
            camera.targetTexture = renderTexture;
            RenderTexture.active = renderTexture;
            camera.Render();
            var output = new Texture2D(width, height, TextureFormat.RGBA32, false);
            output.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            output.Apply();
            File.WriteAllBytes("/private/tmp/destiny-ranger-combat-preview.png", output.EncodeToPNG());
            camera.targetTexture = previousTarget;
            RenderTexture.active = previousActive;
            Object.DestroyImmediate(renderTexture);
            Object.DestroyImmediate(output);
            Debug.Log("Destiny Ranger combat preview rendered: /private/tmp/destiny-ranger-combat-preview.png");
        }

        private static void DumpLargeVisibleSpriteRenderers(string path)
        {
            var lines = Object.FindObjectsOfType<SpriteRenderer>()
                .Where(sr => sr && sr.enabled && sr.gameObject.activeInHierarchy && sr.color.a > .001f)
                .Select(sr =>
                {
                    var b = sr.bounds;
                    var c = sr.color;
                    return new
                    {
                        Area = b.size.x * b.size.y,
                        Line = $"{sr.sortingOrder,4} a={c.a:0.000} area={b.size.x * b.size.y:0.00} size={b.size.x:0.00}x{b.size.y:0.00} pos={b.center.x:0.00},{b.center.y:0.00} name={sr.gameObject.name} sprite={(sr.sprite ? sr.sprite.name : "null")}"
                    };
                })
                .Where(x => x.Area > .18f)
                .OrderByDescending(x => x.Area)
                .Select(x => x.Line)
                .ToArray();
            File.WriteAllLines(path, lines);
        }

        public static void RenderMotionCleanPreviewBatch()
        {
            CreatePrototypeScene();
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var controller = Object.FindObjectOfType<DestinyRangerSideScroller>();
            if (!controller)
                throw new System.InvalidOperationException("DestinyRangerSideScroller is missing from " + scene.path);

            typeof(DestinyRangerSideScroller)
                .GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.Invoke(controller, null);
            controller.DebugBuildMotionCleanPreviewPose();

            RequireObject("MotionCleanPreviewHeroSlash_NoEchoBody");
            RequireObject("MotionCleanPreviewHeroFootContact_CleanEllipse");
            RequireObject("MotionCleanPreviewEnemyHitPin_NoEchoBody");
            RequireObject("MotionCleanPreviewEnemyFootContact_CleanEllipse");
            RequireObject("HeroReadabilityAura_DisabledNoGroundBlob");

            var camera = Camera.main;
            if (!camera)
                throw new System.InvalidOperationException("Preview camera is missing.");

            const int width = 2732;
            const int height = 2048;
            var renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            var previousTarget = camera.targetTexture;
            var previousActive = RenderTexture.active;
            camera.targetTexture = renderTexture;
            RenderTexture.active = renderTexture;
            camera.Render();
            var output = new Texture2D(width, height, TextureFormat.RGBA32, false);
            output.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            output.Apply();
            File.WriteAllBytes("/private/tmp/destiny-ranger-motion-clean-preview.png", output.EncodeToPNG());
            camera.targetTexture = previousTarget;
            RenderTexture.active = previousActive;
            Object.DestroyImmediate(renderTexture);
            Object.DestroyImmediate(output);
            Debug.Log("Destiny Ranger motion clean preview rendered: /private/tmp/destiny-ranger-motion-clean-preview.png");
        }

        public static void RenderMapPreviewBatch()
        {
            CreatePrototypeScene();
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var controller = Object.FindObjectOfType<DestinyRangerSideScroller>();
            if (!controller)
                throw new System.InvalidOperationException("DestinyRangerSideScroller is missing from " + scene.path);

            typeof(DestinyRangerSideScroller)
                .GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.Invoke(controller, null);
            controller.DebugBuildMapPreviewPose();

            RequireObject("GeneratedMapLayout");
            RequireObject("MapVariantLabel");
            RequireObject("PreviewMapRuneJar_Left");
            RequireObject("PreviewMapRuneJar_Mid");
            RequireObject("PreviewMapRuneJar_Right");
            RequireObject("StepPlatform_A");
            RequireObject("AdventureSolidGround_DirtCore");
            RequireObject("AdventureGroundWallArt_FullBottomCoverage_0");
            RequireObject("WhiteFenceSection_Left");
            RequireObject("MushroomCluster_Left");
            RequireObject("StepPlatform_A_AdventureArtUnderContactAo");
            RequireObject("StepPlatform_A_AdventureArtFullSolidBacking");
            RequireObject("AirDecor_PaintedCanopy_Left_NoCollision");
            RequireObject("AirDecor_PaintedCloudMist_Mid_NoCollision");
            RequireObject("AirDecor_PaintedVineCluster_Right_NoCollision");
            RequireObject("AirDecor_PaintedRuinArch_Back_NoCollision");
            RenderMainCameraToPng("/private/tmp/destiny-ranger-map-preview.png");
            Debug.Log("Destiny Ranger map preview rendered: /private/tmp/destiny-ranger-map-preview.png");
        }

        public static void RenderAllMapLayoutsPreviewBatch()
        {
            CreatePrototypeScene();
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var controller = Object.FindObjectOfType<DestinyRangerSideScroller>();
            if (!controller)
                throw new System.InvalidOperationException("DestinyRangerSideScroller is missing from " + scene.path);

            typeof(DestinyRangerSideScroller)
                .GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.Invoke(controller, null);

            for (int layout = 0; layout < 4; layout++)
            {
                controller.DebugBuildMapPreviewPose(layout);
                RequireObject("GeneratedMapLayout");
                RequireObject("AdventureGroundWallArt_FullBottomCoverage_0");
                RequireObject("AdventureGeneratedForegroundGrassStrip_0");
                RequireObject("AdventureGeneratedForegroundGrassContactShade");
                RequireObject("StepPlatform_A");
                RequireObject("StepPlatform_A_AdventureArtFullSolidBacking");
                RequireObject("AirDecor_PaintedCanopy_Left_NoCollision");
                RequireObject("AirDecor_PaintedCloudMist_Mid_NoCollision");
                RequireObject("AirDecor_PaintedVineCluster_Right_NoCollision");
                RequireObject("AirDecor_PaintedRuinArch_Back_NoCollision");
                RequireTextureGrid(AdventurePlatformPath, 4, 1, "Clean platform 4-frame sheet");
                RenderMainCameraToPng($"/private/tmp/destiny-ranger-map-layout-{layout}.png");
            }
            Debug.Log("Destiny Ranger all map layout previews rendered: /private/tmp/destiny-ranger-map-layout-0..3.png");
        }

        public static void AuditAllMapLayoutsBatch()
        {
            CreatePrototypeScene();
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var controller = Object.FindObjectOfType<DestinyRangerSideScroller>();
            if (!controller)
                throw new System.InvalidOperationException("DestinyRangerSideScroller is missing from " + scene.path);

            typeof(DestinyRangerSideScroller)
                .GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.Invoke(controller, null);

            string report = "";
            for (int layout = 0; layout < 4; layout++)
            {
                if (layout > 0) report += "\n\n";
                report += controller.DebugAuditMapLayoutReachability(layout);
            }
            File.WriteAllText("/private/tmp/destiny-ranger-map-audit.txt", report);
            Debug.Log("Destiny Ranger map reachability audit wrote /private/tmp/destiny-ranger-map-audit.txt");
        }

        public static void AuditMovementUnstuckBatch()
        {
            CreatePrototypeScene();
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var controller = Object.FindObjectOfType<DestinyRangerSideScroller>();
            if (!controller)
                throw new System.InvalidOperationException("DestinyRangerSideScroller is missing from " + scene.path);

            typeof(DestinyRangerSideScroller)
                .GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.Invoke(controller, null);

            string report = controller.DebugAuditMovementUnstuck();
            if (report.Contains("blocked") || report.Contains("failed"))
                throw new System.InvalidOperationException("Movement unstuck audit failed:\n" + report);
            File.WriteAllText("/private/tmp/destiny-ranger-movement-audit.txt", report);
            Debug.Log("Destiny Ranger movement unstuck audit wrote /private/tmp/destiny-ranger-movement-audit.txt");
        }

        public static void AuditEnemyAttackInterruptionBatch()
        {
            CreatePrototypeScene();
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var controller = Object.FindObjectOfType<DestinyRangerSideScroller>();
            if (!controller)
                throw new System.InvalidOperationException("DestinyRangerSideScroller is missing from " + scene.path);

            typeof(DestinyRangerSideScroller)
                .GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.Invoke(controller, null);

            string report = controller.DebugAuditEnemyAttackInterruption();
            if (report.Contains("failed"))
                throw new System.InvalidOperationException("Enemy attack interruption audit failed:\n" + report);
            File.WriteAllText("/private/tmp/destiny-ranger-enemy-interrupt-audit.txt", report);
            Debug.Log("Destiny Ranger enemy attack interruption audit wrote /private/tmp/destiny-ranger-enemy-interrupt-audit.txt");
        }

        public static void AuditCombatRoomPacingBatch()
        {
            CreatePrototypeScene();
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var controller = Object.FindObjectOfType<DestinyRangerSideScroller>();
            if (!controller)
                throw new System.InvalidOperationException("DestinyRangerSideScroller is missing from " + scene.path);

            typeof(DestinyRangerSideScroller)
                .GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.Invoke(controller, null);

            string report = controller.DebugAuditCombatRoomPacing();
            if (report.Contains("failed"))
                throw new System.InvalidOperationException("Combat room pacing audit failed:\n" + report);
            File.WriteAllText("/private/tmp/destiny-ranger-combat-room-pacing-audit.txt", report);
            Debug.Log("Destiny Ranger combat room pacing audit wrote /private/tmp/destiny-ranger-combat-room-pacing-audit.txt");
        }

        public static void AuditCombatHitFeelBatch()
        {
            CreatePrototypeScene();
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var controller = Object.FindObjectOfType<DestinyRangerSideScroller>();
            if (!controller)
                throw new System.InvalidOperationException("DestinyRangerSideScroller is missing from " + scene.path);

            typeof(DestinyRangerSideScroller)
                .GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.Invoke(controller, null);

            string report = controller.DebugAuditCombatHitFeel();
            if (report.Contains("failed"))
                throw new System.InvalidOperationException("Combat hit feel audit failed:\n" + report);
            File.WriteAllText("/private/tmp/destiny-ranger-combat-hit-feel-audit.txt", report);
            Debug.Log("Destiny Ranger combat hit feel audit wrote /private/tmp/destiny-ranger-combat-hit-feel-audit.txt");
        }

        public static void AuditHudReadabilityBatch()
        {
            CreatePrototypeScene();
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var controller = Object.FindObjectOfType<DestinyRangerSideScroller>();
            if (!controller)
                throw new System.InvalidOperationException("DestinyRangerSideScroller is missing from " + scene.path);

            typeof(DestinyRangerSideScroller)
                .GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.Invoke(controller, null);

            string report = controller.DebugAuditHudReadability();
            if (report.Contains("failed"))
                throw new System.InvalidOperationException("HUD readability audit failed:\n" + report);
            File.WriteAllText("/private/tmp/destiny-ranger-hud-readability-audit.txt", report);
            Debug.Log("Destiny Ranger HUD readability audit wrote /private/tmp/destiny-ranger-hud-readability-audit.txt");
        }

        public static void RenderRunePreviewBatch()
        {
            CreatePrototypeScene();
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var controller = Object.FindObjectOfType<DestinyRangerSideScroller>();
            if (!controller)
                throw new System.InvalidOperationException("DestinyRangerSideScroller is missing from " + scene.path);

            typeof(DestinyRangerSideScroller)
                .GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.Invoke(controller, null);
            controller.DebugBuildRunePreviewPose();

            RequireObject("RuneSealPanel");
            RequireObject("RuneSealTitle");
            RequireObject("RuneSealText");
            RequireObject("RuneResonanceGrade");
            RequireObject("RuneSlot_0");
            RequireObject("RuneSlot_1");
            RequireObject("RuneSlot_2");
            RequireObject("RuneRewardText");
            RequireObject("RuneBuildDeltaText");
            RenderMainCameraToPng("/private/tmp/destiny-ranger-rune-preview.png");
            Debug.Log("Destiny Ranger rune preview rendered: /private/tmp/destiny-ranger-rune-preview.png");
        }

        public static void RenderSettingsPreviewBatch()
        {
            CreatePrototypeScene();
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var controller = Object.FindObjectOfType<DestinyRangerSideScroller>();
            if (!controller)
                throw new System.InvalidOperationException("DestinyRangerSideScroller is missing from " + scene.path);

            typeof(DestinyRangerSideScroller)
                .GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.Invoke(controller, null);
            controller.DebugBuildSettingsPreviewPose();

            RequireObject("SettingsPanel");
            RequireObject("HeroActionBeatToggle");
            RequireObject("ThumbReachSafeZone");
            RenderMainCameraToPng("/private/tmp/destiny-ranger-settings-preview.png");
            Debug.Log("Destiny Ranger settings preview rendered: /private/tmp/destiny-ranger-settings-preview.png");
        }

        private static void RenderMainCameraToPng(string path)
        {
            var camera = Camera.main;
            if (!camera)
                throw new System.InvalidOperationException("Preview camera is missing.");

            const int width = 2732;
            const int height = 2048;
            var renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            var previousTarget = camera.targetTexture;
            var previousActive = RenderTexture.active;
            camera.targetTexture = renderTexture;
            RenderTexture.active = renderTexture;
            camera.Render();
            var output = new Texture2D(width, height, TextureFormat.RGBA32, false);
            output.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            output.Apply();
            File.WriteAllBytes(path, output.EncodeToPNG());
            camera.targetTexture = previousTarget;
            RenderTexture.active = previousActive;
            Object.DestroyImmediate(renderTexture);
            Object.DestroyImmediate(output);
        }

        public static void ValidateReleaseSettingsBatch()
        {
            RequireEqual("ProductName", PlayerSettings.productName, "命运游侠");
            RequireEqual("BundleVersion", PlayerSettings.bundleVersion, "0.1.0");
            RequireEqual("iOSBundleIdentifier", PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS), "com.destinyranger.prototype");
            RequireEqual("DefaultOrientation", PlayerSettings.defaultInterfaceOrientation.ToString(), UIOrientation.LandscapeLeft.ToString());
            if (PlayerSettings.allowedAutorotateToPortrait || PlayerSettings.allowedAutorotateToPortraitUpsideDown)
                throw new System.InvalidOperationException("Portrait autorotation must be disabled for the iPad landscape build.");
            if (!PlayerSettings.allowedAutorotateToLandscapeLeft || !PlayerSettings.allowedAutorotateToLandscapeRight)
                throw new System.InvalidOperationException("Both landscape orientations must be allowed.");
            string projectSettings = File.ReadAllText("ProjectSettings/ProjectSettings.asset");
            RequireYamlFlag(projectSettings, "defaultScreenOrientation: 3", "Default screen orientation must remain landscape left.");
            RequireYamlFlag(projectSettings, "targetDevice: 2", "Target device should remain iPhone+iPad/universal until store targeting is finalized.");
            RequireYamlFlag(projectSettings, "uIRequiresFullScreen: 1", "iOS requiresFullScreen must stay enabled for this full-screen action game.");
            RequireYamlFlag(projectSettings, "uIStatusBarHidden: 1", "iOS status bar should be hidden during gameplay.");
            RequireYamlFlag(projectSettings, "submitAnalytics: 0", "Unity analytics submission must stay disabled for the no-data-collection privacy claim.");
            ValidateNoThirdPartySdkBatch();
            Debug.Log("Destiny Ranger release settings validated.");
        }

        public static void ValidateNoThirdPartySdkBatch()
        {
            RequireFileContains("Packages/manifest.json", "\"com.unity.ugui\"", "Package manifest is missing the expected UGUI dependency.");
            string manifest = File.ReadAllText("Packages/manifest.json");
            string[] forbiddenPackages =
            {
                "com.unity.ads",
                "com.unity.analytics",
                "com.unity.services.analytics",
                "com.unity.purchasing",
                "com.google.firebase",
                "GoogleMobileAds",
                "Facebook",
                "AppsFlyer",
                "Adjust",
                "GameAnalytics",
                "IronSource"
            };
            foreach (string forbidden in forbiddenPackages)
                RequireFileNotContains("Packages/manifest.json", forbidden, "Forbidden monetization/analytics package is present: " + forbidden);

            string services = File.ReadAllText("ProjectSettings/UnityConnectSettings.asset");
            RequireYamlFlag(services, "m_Enabled: 0", "Unity Services must remain disabled for the offline/no-analytics build.");
            RequireYamlFlag(services, "UnityPurchasingSettings:\n    m_Enabled: 0", "Unity Purchasing must remain disabled.");
            RequireYamlFlag(services, "UnityAnalyticsSettings:\n    m_Enabled: 0", "Unity Analytics must remain disabled.");
            RequireYamlFlag(services, "UnityAdsSettings:\n    m_Enabled: 0", "Unity Ads must remain disabled.");
            RequireFileContains("ProjectSettings/ProjectSettings.asset", "submitAnalytics: 0", "Unity analytics submission must stay disabled.");

            string runtime = File.ReadAllText("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs");
            string[] forbiddenRuntimeApis =
            {
                "UnityWebRequest",
                "HttpClient",
                "WebSocket",
                "TcpClient",
                "UdpClient",
                "AppTrackingTransparency",
                "NSUserTrackingUsageDescription",
                "Advertisement.",
                "Analytics.",
                "Purchasing."
            };
            foreach (string forbidden in forbiddenRuntimeApis)
                if (runtime.Contains(forbidden))
                    throw new System.InvalidOperationException("Runtime must stay offline/no-SDK. Forbidden API found: " + forbidden);

            Debug.Log("Destiny Ranger no third-party SDK/privacy dependency scan passed.");
        }

        public static void ValidateHeroAnimationArtBatch()
        {
            RequireTextureGrid(AdventureHeroPath, 8, 4, "Hero 32-frame animation sheet", false);
            RequireTextureGrid(AdventureEnemyPath, 6, 4, "Enemy 24-frame animation sheet");
            RequireFileContains("Assets/DestinyRanger/Docs/ASSET_PROVENANCE.md", "adventure-hero-anim-32-v1.png", "Hero animation art provenance is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/ASSET_PROVENANCE.md", "adventure-enemy-anim-24-v1.png", "Enemy animation art provenance is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/HERO_ANIMATION_SPEC.md", "主角 8×4 动画表", "Hero animation sheet layout spec is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/HERO_ANIMATION_SPEC.md", "16-18", "Hero attack 1 frame spec is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/HERO_ANIMATION_SPEC.md", "22-25", "Hero heavy attack frame spec is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/HERO_ANIMATION_SPEC.md", "28", "Hero skill release frame spec is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/HERO_ANIMATION_SPEC.md", "LightAttackAnimFps", "Hero light attack FPS spec is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/HERO_ANIMATION_SPEC.md", "HeavyAttackAnimFps", "Hero heavy attack FPS spec is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/HERO_ANIMATION_SPEC.md", "SkillAnimFps", "Hero skill FPS spec is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/HERO_ANIMATION_SPEC.md", "约 0.075 秒", "Hero light attack hit-frame timing spec is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/HERO_ANIMATION_SPEC.md", "约 0.12 秒", "Hero heavy attack hit-frame timing spec is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/HERO_ANIMATION_SPEC.md", "UseEmergencyHeroSprite", "Hero emergency visibility fallback spec is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/HERO_ANIMATION_SPEC.md", "HeroBaseVisualScale", "Hero pose punch scale preservation spec is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/HERO_ANIMATION_SPEC.md", "HeroActionBeatText", "Hero action beat HUD spec is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "private const float LightAttackAnimFps = 18f", "Runtime light attack animation FPS changed without spec review.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "private const float HeavyAttackAnimFps = 20f", "Runtime heavy attack animation FPS changed without spec review.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "private const float SkillAnimFps = 16f", "Runtime skill animation FPS changed without spec review.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "private const float HeroBaseVisualScale = 2.28f", "Runtime hero base visual scale constant is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "heroPosePunchTimer <= 0f", "Runtime hero pose punch scale preservation is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "private static readonly int[] HeroAttack1Frames = { 16, 16, 17, 18, 18 }", "Runtime attack 1 frame mapping changed without spec review.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "private static readonly int[] HeroAttack3Frames = { 22, 23, 23, 24, 25, 25 }", "Runtime heavy attack frame mapping changed without spec review.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "StartCoroutine(ResolveMeleeAttack(range, damage, combo, hitDelay))", "Runtime melee hit-frame coroutine is missing.");
            Debug.Log("Destiny Ranger hero/enemy animation art validated.");
        }

        public static void ValidateAppStoreReadinessBatch()
        {
            ValidateReleaseSettingsBatch();
            RequireFileContains("Assets/DestinyRanger/Docs/APP_STORE_READINESS.md", "App Privacy 建议口径：未收集数据", "App privacy statement is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/APP_STORE_READINESS.md", "APP_PRIVACY_LABEL_SPEC.md", "App privacy label spec reference is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/APP_PRIVACY_LABEL_SPEC.md", "Data Collected: No", "App privacy label no-data statement is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/APP_PRIVACY_LABEL_SPEC.md", "Tracking: No", "App privacy tracking statement is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/APP_PRIVACY_LABEL_SPEC.md", "PlayerPrefs", "App privacy label local storage basis is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "App Privacy 标签建议：未收集数据", "Runtime App Privacy label statement is missing.");
            ValidateNoThirdPartySdkBatch();
            ValidateLocalSaveSchemaBatch();
            RequireFileContains("Assets/DestinyRanger/Docs/APP_STORE_READINESS.md", "不含真钱付费或现金价值", "No real-money/cash-value statement is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/APP_STORE_READINESS.md", "当前启封概率：约 25% 三星、57% 二星、18% 逆运祝福", "Rune odds statement is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/APP_STORE_READINESS.md", "RuneOddsText", "In-run rune odds HUD disclosure is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/APP_STORE_READINESS.md", "不选择 Simulated Gambling", "Simulated gambling classification note is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/APP_STORE_READINESS.md", "清除本机数据", "Local data deletion path is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/APP_STORE_READINESS.md", "离线今日试炼", "Offline daily trial disclosure is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/APP_STORE_READINESS.md", "今日试炼本机最佳", "Local daily trial record disclosure is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/APP_STORE_READINESS.md", "推荐追踪", "Tracked local achievement disclosure is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/APP_STORE_READINESS.md", "未解锁成就进度", "Achievement progress disclosure is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/APP_STORE_READINESS.md", "问题诊断", "App Store post-run diagnosis disclosure is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/APP_STORE_READINESS.md", "推荐构筑", "App Store recommended build disclosure is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/APP_STORE_READINESS.md", "可选自动攻击", "Touch assist accessibility statement is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/APP_STORE_READINESS.md", "放宽闪避容错", "Assist dodge accessibility statement is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/APP_STORE_READINESS.md", "舒适低闪", "Low-flash accessibility statement is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/APP_STORE_READINESS.md", "ASSET_PROVENANCE.md", "App Store asset provenance reference is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/APP_STORE_READINESS.md", "RELEASE_METADATA_SPEC.md", "App Store release metadata spec reference is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/APP_STORE_READINESS.md", "LOCAL_SAVE_SPEC.md", "App Store local save spec reference is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/RELEASE_METADATA_SPEC.md", "隐私政策 URL", "Release metadata privacy URL field is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/RELEASE_METADATA_SPEC.md", "支持 URL", "Release metadata support URL field is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/RELEASE_METADATA_SPEC.md", "APP_PRIVACY_LABEL_SPEC.md", "Release metadata App Privacy spec reference is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "隐私政策 URL：", "Runtime privacy policy URL field is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "支持 URL：", "Runtime support URL field is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "ReleaseBlockerStatusText", "Runtime release blocker status text is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "发布阻断：开发版仍含 TODO 链接", "Runtime release blocker warning is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/BUILD_AND_DEVICE_READINESS.md", "SafeAreaRoot", "Device readiness safe area statement is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/BUILD_AND_DEVICE_READINESS.md", "ValidateAppStoreReadinessBatch", "Device readiness App Store validation command is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/BUILD_AND_DEVICE_READINESS.md", "ValidateFinalSubmissionBlockingBatch", "Device readiness final submission blocking command is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/APP_STORE_READINESS.md", "ValidateFinalSubmissionBlockingBatch", "App Store readiness final submission blocking command is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/RELEASE_METADATA_SPEC.md", "ValidateFinalSubmissionBlockingBatch", "Release metadata final submission blocking command is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/BUILD_AND_DEVICE_READINESS.md", "SettingsComfortBadges", "Device readiness settings comfort badge check is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/BUILD_AND_DEVICE_READINESS.md", "Boss 战顶部 HUD", "Device readiness Boss HUD check is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/BUILD_AND_DEVICE_READINESS.md", "HitConfirmText", "Device readiness hit confirmation HUD check is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/BUILD_AND_DEVICE_READINESS.md", "UpdateForegroundReadabilityBand", "Device readiness foreground readability check is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/BUILD_AND_DEVICE_READINESS.md", "HeroContrastSilhouette", "Device readiness hero contrast silhouette check is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/BUILD_AND_DEVICE_READINESS.md", "ComputeCameraLookAhead", "Device readiness camera lookahead check is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/BUILD_AND_DEVICE_READINESS.md", "AddDangerPattern", "Device readiness shape-coded danger warning check is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/BUILD_AND_DEVICE_READINESS.md", "不能只靠红/绿颜色", "Device readiness color-independent warning note is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/BUILD_AND_DEVICE_READINESS.md", "DangerPatternDamageGlyph", "Device readiness danger glyph check is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/BUILD_AND_DEVICE_READINESS.md", "ThreatAlertText", "Device readiness threat alert HUD check is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/BUILD_AND_DEVICE_READINESS.md", "TargetCompassText", "Device readiness target compass check is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/BUILD_AND_DEVICE_READINESS.md", "下一刀 破势", "Device readiness attack combo indicator check is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/BUILD_AND_DEVICE_READINESS.md", "蓝牙/MFi 手柄", "Device readiness controller compatibility check is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/BUILD_AND_DEVICE_READINESS.md", "系统重映射", "Device readiness controller remap note is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/BUILD_AND_DEVICE_READINESS.md", "不硬显示手柄 glyph", "Device readiness controller glyph safety note is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/BUILD_AND_DEVICE_READINESS.md", "推荐追踪", "Device readiness tracked achievement check is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/BUILD_AND_DEVICE_READINESS.md", "ASSET_PROVENANCE.md", "Device readiness asset provenance check is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/BUILD_AND_DEVICE_READINESS.md", "TOUCH_CONTROL_SPEC.md", "Device readiness touch control spec check is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/BUILD_AND_DEVICE_READINESS.md", "FEEDBACK_SPEC.md", "Device readiness feedback spec check is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/BUILD_AND_DEVICE_READINESS.md", "FIRST_RUN_SPEC.md", "Device readiness first-run spec check is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/BUILD_AND_DEVICE_READINESS.md", "ValidateLocalSaveSchemaBatch", "Device readiness local save validation command is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/APP_STORE_READINESS.md", "TOUCH_CONTROL_SPEC.md", "App Store touch control spec reference is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/APP_STORE_READINESS.md", "FEEDBACK_SPEC.md", "App Store feedback spec reference is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/APP_STORE_READINESS.md", "首局说明", "App Store first-run explanation is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/APP_STORE_READINESS.md", "RoomRewardRecommend", "App Store room reward recommendation check is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/FIRST_RUN_SPEC.md", "OnboardingAssistLine", "First-run assist line spec is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/FIRST_RUN_SPEC.md", "BuildFirstRunAssistAdvice", "First-run failure advice spec is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/FIRST_RUN_SPEC.md", "FirstRunNudgeText", "First-run contextual nudge spec is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/FIRST_RUN_SPEC.md", "无真钱付费或现金价值", "First-run SLOT compliance statement is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/TOUCH_CONTROL_SPEC.md", "CombatControlMinTouchSize", "Touch control minimum target spec is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/TOUCH_CONTROL_SPEC.md", "ClampCombatControlPosition", "Touch control clamp spec is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/TOUCH_CONTROL_SPEC.md", "44pt", "Touch target baseline spec is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/TOUCH_CONTROL_SPEC.md", "AttackBufferText", "Touch queued-input indicator spec is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/TOUCH_CONTROL_SPEC.md", "AttackRhythmWindowFill", "Touch attack rhythm window spec is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/TOUCH_CONTROL_SPEC.md", "TouchDownEchoDuration", "Touch down echo duration spec is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/TOUCH_CONTROL_SPEC.md", "将启封", "Touch SLOT near-ready label spec is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/TOUCH_CONTROL_SPEC.md", "JoystickIntentText", "Joystick intent feedback spec is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/FEEDBACK_SPEC.md", "HapticTier.Heavy", "Heavy haptic feedback spec is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/FEEDBACK_SPEC.md", "HapticTier.Reward", "Reward haptic feedback spec is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/FEEDBACK_SPEC.md", "hapticCooldownTimer", "Haptic throttling spec is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/FEEDBACK_SPEC.md", "HitConfirmText", "Hit confirmation HUD spec is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/FEEDBACK_SPEC.md", "HitConfirmImpactFill", "Hit confirmation strength bar spec is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/FEEDBACK_SPEC.md", "UpdateInputBufferIndicators", "Feedback spec queued-input indicator note is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/FEEDBACK_SPEC.md", "AttackRhythmWindowFill", "Feedback spec attack rhythm window note is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/FEEDBACK_SPEC.md", "AttachTouchDownFeedback", "Feedback spec touch-down hook note is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/FEEDBACK_SPEC.md", "sfxHeavyHit", "Feedback spec heavy hit audio note is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/FEEDBACK_SPEC.md", "sfxBossWarn", "Feedback spec Boss warning audio note is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/FEEDBACK_SPEC.md", "sfxPickup", "Feedback spec pickup audio note is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "\"CanopyMaster\"", "Canopy route achievement is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "\"BrokenBridgeMaster\"", "Broken bridge route achievement is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "\"RuneRidgeMaster\"", "Rune ridge route achievement is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "\"RouteArchivist\"", "Route archivist achievement is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/LOCAL_SAVE_SPEC.md", "RouteArchivist", "Local save spec is missing route achievements.");
            RequireFileContains("Assets/DestinyRanger/Docs/ASSET_PROVENANCE.md", "api-image", "Generated asset provenance must mention api-image.");
            RequireFileContains("Assets/DestinyRanger/Docs/ASSET_PROVENANCE.md", "Assets/DestinyRanger/Art/Generated", "Generated asset folder provenance is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/ASSET_PROVENANCE.md", "adventure-stage-forest-long-v1.png", "Stage art provenance is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/ASSET_PROVENANCE.md", "adventure-hud-controls-v2.png", "Mobile HUD control art provenance is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/ASSET_PROVENANCE.md", "adventure-rune-ui-sheet.png", "Rune/SLOT UI provenance is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/ASSET_PROVENANCE.md", "不使用第三方", "Third-party asset restriction is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/ASSET_PROVENANCE.md", "不得出现硬币下注", "SLOT visual compliance rule is missing.");

            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "不接入广告、分析 SDK 或联网排行", "Runtime privacy text must state no ads/analytics/leaderboards.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "当前启封概率约为：三星 25%，二星 57%，逆运祝福 18%", "Runtime odds disclosure is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "SLOT 是战斗内命运符文表现", "Runtime SLOT packaging statement is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "RuneBuildDeltaText", "Runtime SLOT build delta line is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "总构筑 Lv", "Runtime SLOT build level before/after text is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/BUILD_AND_DEVICE_READINESS.md", "RuneBuildDeltaText", "Device readiness SLOT build delta check is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/BUILD_AND_DEVICE_READINESS.md", "RuneAnticipationThreshold", "Device readiness SLOT anticipation threshold check is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/APP_STORE_READINESS.md", "本次构筑变化", "App Store SLOT result readability note is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/APP_STORE_READINESS.md", "临界状态", "App Store SLOT anticipation compliance note is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/BUILD_AND_DEVICE_READINESS.md", "RoomRewardRecommend_0/1/2", "Device readiness reward recommendation check is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/BUILD_AND_DEVICE_READINESS.md", "RoomRewardTypeBadge_0/1/2", "Device readiness reward type badge check is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/BUILD_AND_DEVICE_READINESS.md", "bash Tools/verify_destiny_ranger_static.sh", "Static QA command is missing from device readiness.");
            RequireFileContains("Tools/verify_destiny_ranger_static.sh", "Destiny Ranger static QA passed.", "Static QA script is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/BUILD_AND_DEVICE_READINESS.md", "BatchHeartbeat", "Unity batch heartbeat diagnosis is missing from device readiness.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "AdventureGeneratedForegroundGrassStrip", "Runtime foreground readability layer is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "HeroContrastSilhouette", "Runtime hero contrast silhouette is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "cameraLookAheadX", "Runtime camera lookahead smoothing state is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "UpdateHeroLandingPredictor", "Runtime hero landing predictor is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "PredictedLandingY", "Runtime hero landing predictor platform target is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "EnemyContactAo_CleanEllipse", "Runtime clean enemy foot contact ellipse is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "SyncHeroContrastSilhouette", "Runtime hero contrast silhouette frame sync is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "AddDangerPattern(warn, DangerPatternKind.Stripes", "Runtime rectangular danger stripes are missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "AddDangerPattern(warn, DangerPatternKind.Crosshair", "Runtime circular danger crosshair is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "AddDangerPattern(pulse, DangerPatternKind.ToxicWaves", "Runtime toxic danger wave pattern is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "AboutClearLocalData", "Local data deletion button is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "DestinyRanger.AutoAttack", "Auto attack preference must be saved locally.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "AssistDamageScale", "Assist mode damage scaling is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "AssistDodgeCooldownScale", "Assist mode dodge cooldown easing is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "AssistPerfectDodgeWindowBonus", "Assist mode perfect dodge window easing is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "ControllerAttackDown", "Controller attack input support is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "ControllerMenuDown", "Controller pause/menu support is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "HasConnectedController", "Controller connection status badge support is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "Input.GetJoystickNames", "Controller connection detection is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "KeyCode.JoystickButton0", "Controller button mapping is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "DestinyRanger.Achievement.", "Local achievement storage is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "BuildTrackedAchievementHint", "Tracked local achievement hint is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "EffectsToggle", "Low-flash setting entry is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "UpdateSettingsComfortBadges", "Settings comfort/privacy badge summary is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "HitConfirmBackplate", "Runtime hit confirm backplate is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "破势命中", "Runtime heavy hit confirmation label is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "术式命中", "Runtime skill hit confirmation label is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "OnApplicationPause", "Lifecycle pause handler is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "OnApplicationFocus", "Lifecycle focus handler is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "应用恢复：战斗已冻结，触控输入已清空", "Lifecycle resume pause reason is missing.");

            Debug.Log("Destiny Ranger App Store readiness statements validated.");
        }

        public static void ValidateFinalSubmissionBlockingBatch()
        {
            ValidateAppStoreReadinessBatch();
            ValidateHeroAnimationArtBatch();

            RequireFileNotContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "TODO_PRIVACY_POLICY_URL", "Runtime privacy policy URL placeholder must be replaced before final App Store submission.");
            RequireFileNotContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "TODO_SUPPORT_URL", "Runtime support URL placeholder must be replaced before final App Store submission.");
            RequireFileNotContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "PLACEHOLDER", "Runtime placeholder text must be removed before final App Store submission.");
            RequireFileNotContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "private bool showHeroActionBeatHud = true", "Hero action beat debug HUD must not default on for final App Store submission.");
            RequireFileNotContains("Assets/DestinyRanger/Docs/RELEASE_METADATA_SPEC.md", "TODO_PRIVACY_POLICY_URL", "Release metadata privacy policy URL placeholder must be replaced before final App Store submission.");
            RequireFileNotContains("Assets/DestinyRanger/Docs/RELEASE_METADATA_SPEC.md", "TODO_SUPPORT_URL", "Release metadata support URL placeholder must be replaced before final App Store submission.");

            Debug.Log("Destiny Ranger final submission blockers validated.");
        }

        public static void ValidateLocalSaveSchemaBatch()
        {
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "private const int SaveSchemaVersion = 2", "Runtime save schema version constant is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "private const string SaveSchemaVersionKey = \"DestinyRanger.SaveSchemaVersion\"", "Runtime save schema key is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "EnsureLocalSaveSchema();", "Local save schema check must run before loading PlayerPrefs.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "private void MigrateLocalSaveSchema(int fromVersion)", "Local save migration hook is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "LocalPlayerPrefKeys", "Local PlayerPrefs deletion list is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "CombatButtonOffsetXKey", "Combat button generated PlayerPrefs key helper is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "AchievementKey", "Achievement generated PlayerPrefs key helper is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "DestinyRanger.HeroActionBeatHud", "Hero action beat local setting key is missing.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "PlayerPrefs.DeleteKey(CombatButtonOffsetXKey(id))", "Clear local data must delete per-button X offsets.");
            RequireFileContains("Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs", "PlayerPrefs.DeleteKey(AchievementKey(id))", "Clear local data must delete local achievements.");

            RequireFileContains("Assets/DestinyRanger/Docs/LOCAL_SAVE_SPEC.md", "DestinyRanger.SaveSchemaVersion = 1", "Local save schema version doc is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/LOCAL_SAVE_SPEC.md", "DestinyRanger.HeroActionBeatHud", "Hero action beat local setting doc is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/LOCAL_SAVE_SPEC.md", "EnsureLocalSaveSchema", "Local save schema check doc is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/LOCAL_SAVE_SPEC.md", "MigrateLocalSaveSchema", "Local save migration doc is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/LOCAL_SAVE_SPEC.md", "DestinyRanger.AssistMode", "Local save fixed key doc is missing assist setting.");
            RequireFileContains("Assets/DestinyRanger/Docs/LOCAL_SAVE_SPEC.md", "DestinyRanger.DailyBestKills", "Local save fixed key doc is missing daily record.");
            RequireFileContains("Assets/DestinyRanger/Docs/LOCAL_SAVE_SPEC.md", "DestinyRanger.ControlButtonOffset.<id>.X", "Local save generated button X key doc is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/LOCAL_SAVE_SPEC.md", "DestinyRanger.Achievement.<id>", "Local save generated achievement key doc is missing.");
            RequireFileContains("Assets/DestinyRanger/Docs/LOCAL_SAVE_SPEC.md", "APP_PRIVACY_LABEL_SPEC.md", "Local save spec must point to App Privacy label review.");
            RequireFileContains("Assets/DestinyRanger/Docs/APP_STORE_READINESS.md", "LOCAL_SAVE_SPEC.md", "App Store checklist must reference local save spec.");
            RequireFileContains("Assets/DestinyRanger/Docs/BUILD_AND_DEVICE_READINESS.md", "LOCAL_SAVE_SPEC.md", "Device checklist must reference local save spec.");
            Debug.Log("Destiny Ranger local save schema validated.");
        }

        private static void RequireEqual(string label, string actual, string expected)
        {
            if (actual == expected) return;
            throw new System.InvalidOperationException($"{label} mismatch. Expected '{expected}', got '{actual}'.");
        }

        private static void RequireYamlFlag(string yaml, string expectedLine, string message)
        {
            if (yaml.Contains(expectedLine)) return;
            throw new System.InvalidOperationException(message + " Missing: " + expectedLine);
        }

        private static void RequireFileContains(string path, string expectedText, string message)
        {
            if (!File.Exists(path))
                throw new System.InvalidOperationException("Required readiness file is missing: " + path);
            string text = File.ReadAllText(path);
            if (text.Contains(expectedText)) return;
            throw new System.InvalidOperationException(message + " Missing text: " + expectedText);
        }

        private static void RequireFileNotContains(string path, string forbiddenText, string message)
        {
            if (!File.Exists(path))
                throw new System.InvalidOperationException("Required readiness file is missing: " + path);
            string text = File.ReadAllText(path);
            if (!text.Contains(forbiddenText)) return;
            throw new System.InvalidOperationException(message);
        }

        private static void RequireTextureGrid(string path, int columns, int rows, string label, bool strictGrid = true)
        {
            if (!File.Exists(path))
                throw new System.InvalidOperationException(label + " is missing: " + path);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
                throw new System.InvalidOperationException(label + " importer is missing: " + path);
            if (!importer.isReadable)
                throw new System.InvalidOperationException(label + " must stay readable for runtime frame visibility QA: " + path);
            if (importer.mipmapEnabled)
                throw new System.InvalidOperationException(label + " should keep mipmaps disabled for crisp 2D sprites: " + path);

            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (!texture)
                throw new System.InvalidOperationException(label + " texture failed to load: " + path);
            if (texture.width < columns || texture.height < rows)
                throw new System.InvalidOperationException($"{label} must be a clean {columns}x{rows} frame grid. Actual size: {texture.width}x{texture.height}.");
            if (strictGrid && (texture.width % columns != 0 || texture.height % rows != 0))
                throw new System.InvalidOperationException($"{label} must be a clean {columns}x{rows} frame grid. Actual size: {texture.width}x{texture.height}.");
            if (!strictGrid && (texture.width < columns * 160 || texture.height < rows * 160))
                throw new System.InvalidOperationException($"{label} is too small for runtime slicing. Actual size: {texture.width}x{texture.height}.");

            var pixels = texture.GetPixels32();
            int frameWidth = texture.width / columns;
            int frameHeight = texture.height / rows;
            for (int frame = 0; frame < columns * rows; frame++)
            {
                int column = frame % columns;
                int rowFromTop = frame / columns;
                int minX = column * frameWidth;
                int minY = texture.height - (rowFromTop + 1) * frameHeight;
                int solidPixels = 0;
                for (int y = minY; y < minY + frameHeight; y += 2)
                {
                    int pixelRow = y * texture.width;
                    for (int x = minX; x < minX + frameWidth; x += 2)
                    {
                        if (pixels[pixelRow + x].a > 24 && ++solidPixels >= 80)
                            break;
                    }
                    if (solidPixels >= 80) break;
                }
                if (solidPixels < 80)
                    throw new System.InvalidOperationException($"{label} frame {frame} appears empty or over-trimmed.");
            }
        }

        private static void RequireObject(string name)
        {
            if (GameObject.Find(name))
                return;
            foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
                if (go.name == name)
                    return;
            throw new System.InvalidOperationException("Required runtime object is missing: " + name);
        }

        private static void AssignOptionalSprite(Object target, string fieldName, string path)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (!sprite) return;
            var so = new SerializedObject(target);
            var prop = so.FindProperty(fieldName);
            if (prop == null) return;
            prop.objectReferenceValue = sprite;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void AssignOptionalTexture(Object target, string fieldName, string path)
        {
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (!texture) return;
            var so = new SerializedObject(target);
            var prop = so.FindProperty(fieldName);
            if (prop == null) return;
            prop.objectReferenceValue = texture;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void AssignTextureArrayFromFolder(Object target, string fieldName, string folder, string namePrefix = "")
        {
            if (!Directory.Exists(folder)) return;
            var paths = Directory.GetFiles(folder, "*.png", SearchOption.TopDirectoryOnly)
                .Where(path => string.IsNullOrEmpty(namePrefix) || Path.GetFileNameWithoutExtension(path).ToLowerInvariant().StartsWith(namePrefix.ToLowerInvariant()))
                .OrderBy(path => Path.GetFileNameWithoutExtension(path), System.StringComparer.OrdinalIgnoreCase)
                .ToArray();
            if (paths.Length == 0) return;

            var so = new SerializedObject(target);
            var prop = so.FindProperty(fieldName);
            if (prop == null || !prop.isArray) return;
            prop.arraySize = paths.Length;
            for (int i = 0; i < paths.Length; i++)
                prop.GetArrayElementAtIndex(i).objectReferenceValue = AssetDatabase.LoadAssetAtPath<Texture2D>(paths[i]);
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void EnsureSpriteImport(string path)
        {
            if (!File.Exists(path)) return;
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) return;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.crunchedCompression = false;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.filterMode = FilterMode.Bilinear;
            importer.maxTextureSize = 4096;
            importer.SaveAndReimport();
        }

        private static void EnsureTextureImport(string path)
        {
            if (!File.Exists(path)) return;
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) return;
            importer.textureType = TextureImporterType.Default;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.isReadable = true;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.filterMode = IsStageBackground(path) ? FilterMode.Bilinear : FilterMode.Point;
            importer.maxTextureSize = 4096;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.crunchedCompression = false;
            importer.SaveAndReimport();
        }

        private static void EnsureExternalVfxImport()
        {
            if (!Directory.Exists(ExternalVfxRoot)) return;
            foreach (var path in Directory.GetFiles(ExternalVfxRoot, "*.png", SearchOption.AllDirectories))
                EnsureTextureImport(path);
        }

        private static bool IsStageBackground(string path)
        {
            return path.Contains("stage") || path.Contains("background") || path.Contains("-bg");
        }

        private static Color Parse(string html)
        {
            ColorUtility.TryParseHtmlString(html, out var color);
            return color;
        }
    }
}
