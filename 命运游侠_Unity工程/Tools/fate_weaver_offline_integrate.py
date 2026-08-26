#!/usr/bin/env python3
from __future__ import annotations

import hashlib
from pathlib import Path

PROJECT = Path("/Users/zhendian/Documents/New project/命运游侠_Unity工程")
ASSETS = PROJECT / "Assets"
FW = ASSETS / "DestinyRanger/Art/Generated/FateWeaverFull"
ANIM = ASSETS / "DestinyRanger/Animations/FateWeaver"
DATA = ASSETS / "DestinyRanger/Data"
SCENES = ASSETS / "Scenes"
AUDIO = ASSETS / "Audio"

SYMBOL_SET_GUID = "6a38cc5d0c6c4a6cb14861de26f7b262"
AUDIO_CATALOG_GUID = "2e746d4dfbc24821a85afec83df29882"
SCENE_COLOR_TINT_GUID = "0fa1f63170a24e449bf0f2f5196d9412"
PROFILE_GUIDS = {
    "chamber": "a212450b66b5021e129b49ec0f4d323d",
    "forest": "22911150f462056712298261e18c9a3c",
    "volcano": "287338d6ead99c5dfb61c2d1fe333e68",
    "void": "34ecced92655eb9f401a1be8cda51c26",
}


def guid(key: str) -> str:
    return hashlib.md5(key.encode("utf-8")).hexdigest()


def read_guid(path: Path) -> str:
    meta = path.with_suffix(path.suffix + ".meta")
    if meta.exists():
        for line in meta.read_text(encoding="utf-8", errors="ignore").splitlines():
            if line.startswith("guid: "):
                return line.split("guid: ", 1)[1].strip()
    g = guid(str(path))
    meta.write_text(f"fileFormatVersion: 2\nguid: {g}\nDefaultImporter:\n  externalObjects: {{}}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n", encoding="utf-8")
    return g


def write_meta(path: Path, g: str):
    path.with_suffix(path.suffix + ".meta").write_text(f"fileFormatVersion: 2\nguid: {g}\nNativeFormatImporter:\n  externalObjects: {{}}\n  mainObjectFileID: 7400000\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n", encoding="utf-8")


def anim_clip_yaml(sprite_paths: list[Path], loop: bool, sample_rate: int) -> str:
    frames = []
    for i, p in enumerate(sprite_paths):
        frames.append(f"    - time: {i / sample_rate:.7f}\n      value: {{fileID: 21300000, guid: {read_guid(p)}, type: 3}}")
    stop = len(sprite_paths) / sample_rate if sprite_paths else 0.1
    frame_yaml = "\n".join(frames)
    mapping_yaml = "".join(f"    - {{fileID: 21300000, guid: {read_guid(p)}, type: 3}}\n" for p in sprite_paths)
    return f"""%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!74 &7400000
AnimationClip:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_Name: GENERATED_CLIP_NAME
  serializedVersion: 7
  m_Legacy: 0
  m_Compressed: 0
  m_UseHighQualityCurve: 1
  m_RotationCurves: []
  m_CompressedRotationCurves: []
  m_EulerCurves: []
  m_PositionCurves: []
  m_ScaleCurves: []
  m_FloatCurves: []
  m_PPtrCurves:
  - curve:
{frame_yaml}
    attribute: m_Sprite
    path: 
    classID: 212
    script: {{fileID: 0}}
  m_SampleRate: {sample_rate}
  m_WrapMode: 0
  m_Bounds:
    m_Center: {{x: 0, y: 0, z: 0}}
    m_Extent: {{x: 0, y: 0, z: 0}}
  m_ClipBindingConstant:
    genericBindings:
    - serializedVersion: 2
      path: 0
      attribute: 0
      script: {{fileID: 0}}
      typeID: 212
      customType: 23
      isPPtrCurve: 1
    pptrCurveMapping:
{mapping_yaml}
  m_AnimationClipSettings:
    serializedVersion: 2
    m_AdditiveReferencePoseClip: {{fileID: 0}}
    m_AdditiveReferencePoseTime: 0
    m_StartTime: 0
    m_StopTime: {stop:.7f}
    m_OrientationOffsetY: 0
    m_Level: 0
    m_CycleOffset: 0
    m_HasAdditiveReferencePose: 0
    m_LoopTime: {1 if loop else 0}
    m_LoopBlend: 0
    m_LoopBlendOrientation: 0
    m_LoopBlendPositionY: 0
    m_LoopBlendPositionXZ: 0
    m_KeepOriginalOrientation: 0
    m_KeepOriginalPositionY: 1
    m_KeepOriginalPositionXZ: 0
    m_HeightFromFeet: 0
    m_Mirror: 0
  m_EditorCurves: []
  m_EulerEditorCurves: []
  m_HasGenericRootTransform: 0
  m_HasMotionFloatCurves: 0
  m_Events: []
"""


def make_anims():
    specs = []
    for char in ["aileen", "grick", "luna"]:
        for action in ["idle", "attack", "hit", "skill", "death"]:
            specs.append((FW / f"Characters/{char}", f"{char}_{action}_*.png", ANIM / f"Characters/{char}/{char}_{action}.anim", action == "idle", 6 if action == "idle" else 10))
    for monster_dir in (FW / "Monsters/Forest").glob("*"):
        if not monster_dir.is_dir():
            continue
        monster = monster_dir.name
        for action in ["idle", "attack", "hit", "death"]:
            frames = sorted(monster_dir.glob(f"{monster}_{action}_*.png"))
            if frames:
                specs.append((monster_dir, f"{monster}_{action}_*.png", ANIM / f"Monsters/Forest/{monster}/{monster}_{action}.anim", action == "idle", 5 if action == "idle" else 9))
    count = 0
    for folder, pattern, out, loop, fps in specs:
        frames = sorted(folder.glob(pattern))
        if not frames:
            continue
        out.parent.mkdir(parents=True, exist_ok=True)
        content = anim_clip_yaml(frames, loop, fps).replace("GENERATED_CLIP_NAME", out.stem)
        out.write_text(content, encoding="utf-8")
        write_meta(out, guid(str(out)))
        count += 1
    return count


def scene_yaml(scene_id: str, profile_guid: str) -> str:
    bg = {
        "chamber": "0.0784314, g: 0.0980392, b: 0.1960784",
        "forest": "0.1568627, g: 0.2352941, b: 0.1176471",
        "volcano": "0.2352941, g: 0.1176471, b: 0.0784314",
        "void": "0.1568627, g: 0.0784314, b: 0.1568627",
    }[scene_id]
    return f"""%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!29 &1
OcclusionCullingSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 2
  m_OcclusionBakeSettings:
    smallestOccluder: 5
    smallestHole: 0.25
    backfaceThreshold: 100
  m_SceneGUID: 00000000000000000000000000000000
  m_OcclusionCullingData: {{fileID: 0}}
--- !u!104 &2
RenderSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 9
  m_Fog: 0
  m_FogColor: {{r: 0.5, g: 0.5, b: 0.5, a: 1}}
  m_FogMode: 3
  m_FogDensity: 0.01
  m_LinearFogStart: 0
  m_LinearFogEnd: 300
  m_AmbientSkyColor: {{r: {bg}, a: 1}}
  m_AmbientEquatorColor: {{r: {bg}, a: 1}}
  m_AmbientGroundColor: {{r: {bg}, a: 1}}
  m_AmbientIntensity: 1
  m_AmbientMode: 0
  m_SubtractiveShadowColor: {{r: 0.42, g: 0.478, b: 0.627, a: 1}}
  m_SkyboxMaterial: {{fileID: 10304, guid: 0000000000000000f000000000000000, type: 0}}
  m_HaloStrength: 0.5
  m_FlareStrength: 1
  m_FlareFadeSpeed: 3
  m_HaloTexture: {{fileID: 0}}
  m_SpotCookie: {{fileID: 10001, guid: 0000000000000000e000000000000000, type: 0}}
  m_DefaultReflectionMode: 0
  m_DefaultReflectionResolution: 128
  m_ReflectionBounces: 1
  m_ReflectionIntensity: 1
  m_CustomReflection: {{fileID: 0}}
  m_Sun: {{fileID: 0}}
  m_UseRadianceAmbientProbe: 0
--- !u!157 &3
LightmapSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 12
  m_GIWorkflowMode: 1
  m_GISettings:
    serializedVersion: 2
    m_BounceScale: 1
    m_IndirectOutputScale: 1
    m_AlbedoBoost: 1
    m_EnvironmentLightingMode: 0
    m_EnableBakedLightmaps: 1
    m_EnableRealtimeLightmaps: 0
  m_LightingDataAsset: {{fileID: 0}}
  m_LightingSettings: {{fileID: 0}}
--- !u!196 &4
NavMeshSettings:
  serializedVersion: 2
  m_ObjectHideFlags: 0
  m_BuildSettings:
    serializedVersion: 3
    agentTypeID: 0
    agentRadius: 0.5
    agentHeight: 2
    agentSlope: 45
    agentClimb: 0.4
    ledgeDropHeight: 0
    maxJumpAcrossDistance: 0
    minRegionArea: 2
    manualCellSize: 0
    cellSize: 0.16666667
    manualTileSize: 0
    tileSize: 256
    buildHeightMesh: 0
    maxJobWorkers: 0
    preserveTilesOutsideBounds: 0
    debug:
      m_Flags: 0
  m_NavMeshData: {{fileID: 0}}
--- !u!1 &100000
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: 100002}}
  - component: {{fileID: 100001}}
  - component: {{fileID: 100003}}
  m_Layer: 0
  m_Name: Main Camera
  m_TagString: MainCamera
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!20 &100001
Camera:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 100000}}
  m_Enabled: 1
  serializedVersion: 2
  m_ClearFlags: 2
  m_BackGroundColor: {{r: {bg}, a: 1}}
  orthographic: 1
  orthographic size: 7
  near clip plane: 0.3
  far clip plane: 1000
  field of view: 60
--- !u!4 &100002
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 100000}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: -10}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {{fileID: 0}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
--- !u!114 &100003
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 100000}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {SCENE_COLOR_TINT_GUID}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  activeProfile: {{fileID: 11400000, guid: {profile_guid}, type: 2}}
  applyOnStart: 1
  includeChildren: 1
--- !u!1660057539 &9223372036854775807
SceneRoots:
  m_ObjectHideFlags: 0
  m_Roots:
  - {{fileID: 100002}}
"""


def make_scenes():
    for scene_id, pg in PROFILE_GUIDS.items():
        out = SCENES / f"FateWeaver_{scene_id.capitalize()}Tone.unity"
        out.write_text(scene_yaml(scene_id, pg), encoding="utf-8")
        out.with_suffix(".unity.meta").write_text(f"fileFormatVersion: 2\nguid: {guid(str(out))}\nDefaultImporter:\n  externalObjects: {{}}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n", encoding="utf-8")


def background_scene_object(bg_path: Path, scale: float, sorting_order: int = -10) -> str:
    bg_guid = read_guid(bg_path)
    return f"""--- !u!1 &200000
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: 200002}}
  - component: {{fileID: 200003}}
  m_Layer: 0
  m_Name: Background
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &200002
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 200000}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: {scale}, y: {scale}, z: {scale}}}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {{fileID: 0}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
--- !u!212 &200003
SpriteRenderer:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 200000}}
  m_Enabled: 1
  m_CastShadows: 0
  m_ReceiveShadows: 0
  m_DynamicOccludee: 1
  m_StaticShadowCaster: 0
  m_MotionVectors: 1
  m_LightProbeUsage: 1
  m_ReflectionProbeUsage: 1
  m_RayTracingMode: 0
  m_RayTraceProcedural: 0
  m_RenderingLayerMask: 1
  m_RendererPriority: 0
  m_Materials:
  - {{fileID: 10754, guid: 0000000000000000f000000000000000, type: 0}}
  m_StaticBatchInfo:
    firstSubMesh: 0
    subMeshCount: 0
  m_StaticBatchRoot: {{fileID: 0}}
  m_ProbeAnchor: {{fileID: 0}}
  m_LightProbeVolumeOverride: {{fileID: 0}}
  m_ScaleInLightmap: 1
  m_ReceiveGI: 1
  m_PreserveUVs: 0
  m_IgnoreNormalsForChartDetection: 0
  m_ImportantGI: 0
  m_StitchLightmapSeams: 1
  m_SelectedEditorRenderState: 0
  m_MinimumChartSize: 4
  m_AutoUVMaxDistance: 0.5
  m_AutoUVMaxAngle: 89
  m_LightmapParameters: {{fileID: 0}}
  m_SortingLayerID: 0
  m_SortingLayer: 0
  m_SortingOrder: {sorting_order}
  m_Sprite: {{fileID: 21300000, guid: {bg_guid}, type: 3}}
  m_Color: {{r: 1, g: 1, b: 1, a: 1}}
  m_FlipX: 0
  m_FlipY: 0
  m_DrawMode: 0
  m_Size: {{x: 12.9, y: 27.96}}
  m_AdaptiveModeThreshold: 0.5
  m_SpriteTileMode: 0
  m_WasSpriteAssigned: 1
  m_MaskInteraction: 0
  m_SpriteSortPoint: 0
"""


def anchor_object(object_id: int, transform_id: int, name: str, position: tuple[float, float, float], scale: float = 1.0) -> str:
    x, y, z = position
    return f"""--- !u!1 &{object_id}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {transform_id}}}
  m_Layer: 0
  m_Name: {name}
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &{transform_id}
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {object_id}}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: {x}, y: {y}, z: {z}}}
  m_LocalScale: {{x: {scale}, y: {scale}, z: {scale}}}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {{fileID: 0}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
"""


def integrated_scene_yaml(scene_id: str, display_name: str, profile_guid: str, bg_path: Path) -> str:
    base = scene_yaml(scene_id, profile_guid)
    prefix = base.split("--- !u!1660057539", 1)[0]
    scale = 0.01
    slot_y = -1.6 if scene_id == "chamber" else -2.25
    return prefix + background_scene_object(bg_path, scale) + anchor_object(
        300000, 300002, f"{display_name} SlotMachine Prefab Anchor", (0, slot_y, 0), 0.01
    ) + anchor_object(
        400000, 400002, "Fate Weaver UI Canvas Prefab Anchor", (0, 0, 0), 1.0
    ) + anchor_object(
        500000, 500002, "Fate Weaver Audio Hub Prefab Anchor", (0, 0, 0), 1.0
    ) + """--- !u!1660057539 &9223372036854775807
SceneRoots:
  m_ObjectHideFlags: 0
  m_Roots:
  - {fileID: 100002}
  - {fileID: 200002}
  - {fileID: 300002}
  - {fileID: 400002}
  - {fileID: 500002}
"""


def make_integrated_scenes():
    specs = (
        ("chamber", "Chamber", PROFILE_GUIDS["chamber"], FW / "Backgrounds/fate-weaver-chamber-bg_1290x2796.png"),
        ("forest", "Forest", PROFILE_GUIDS["forest"], FW / "Backgrounds/fate-weaver-battle-forest-bg_1290x1398.png"),
    )
    for scene_id, display_name, profile_guid, bg_path in specs:
        out = SCENES / f"FateWeaver_{display_name}Integrated.unity"
        out.write_text(integrated_scene_yaml(scene_id, display_name, profile_guid, bg_path), encoding="utf-8")
        out.with_suffix(".unity.meta").write_text(f"fileFormatVersion: 2\nguid: {guid(str(out))}\nDefaultImporter:\n  externalObjects: {{}}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n", encoding="utf-8")


def audio_meta():
    template = """fileFormatVersion: 2
guid: {guid}
AudioImporter:
  externalObjects: {{}}
  serializedVersion: 7
  defaultSettings:
    serializedVersion: 2
    loadType: 0
    sampleRateSetting: 0
    sampleRateOverride: 44100
    compressionFormat: 1
    quality: 1
    conversionMode: 0
  platformSettingOverrides: {{}}
  forceToMono: 0
  normalize: 1
  preloadAudioData: 1
  loadInBackground: 0
  ambisonic: 0
  3D: 1
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""
    for p in AUDIO.glob("**/*.wav"):
        p.with_suffix(".wav.meta").write_text(template.format(guid=guid(str(p))), encoding="utf-8")


def ptr(path: Path, file_id: int) -> str:
    return f"{{fileID: {file_id}, guid: {read_guid(path)}, type: 3}}"


def symbol_catalogs():
    (DATA / "Symbols").mkdir(parents=True, exist_ok=True)
    names = ["sword", "staff", "heart", "shield", "skull", "star"]
    for scene in ["chamber", "forest", "volcano", "void"]:
        paths = [FW / f"Symbols/{scene}/symbol_{n}_{scene}.png" for n in names]
        disabled = [FW / f"Symbols/disabled/symbol_{n}_disabled.png" for n in names]
        highlight = [FW / f"Symbols/highlight/symbol_{n}_highlight.png" for n in names]
        symbols_yaml = "".join(f"  - {ptr(p, 21300000)}\n" for p in paths)
        disabled_yaml = "".join(f"  - {ptr(p, 21300000)}\n" for p in disabled)
        highlight_yaml = "".join(f"  - {ptr(p, 21300000)}\n" for p in highlight)
        out = DATA / f"Symbols/{scene.capitalize()}SymbolSet.asset"
        out.write_text(f"""%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 0}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {SYMBOL_SET_GUID}, type: 3}}
  m_Name: {scene.capitalize()}SymbolSet
  m_EditorClassIdentifier: 
  sceneId: {scene}
  sword: {ptr(paths[0], 21300000)}
  staff: {ptr(paths[1], 21300000)}
  heart: {ptr(paths[2], 21300000)}
  shield: {ptr(paths[3], 21300000)}
  skull: {ptr(paths[4], 21300000)}
  star: {ptr(paths[5], 21300000)}
  symbols:
{symbols_yaml}  disabledSymbols:
{disabled_yaml}  highlightSymbols:
{highlight_yaml}""", encoding="utf-8")
        out.with_suffix(".asset.meta").write_text(f"fileFormatVersion: 2\nguid: {guid(str(out))}\nNativeFormatImporter:\n  externalObjects: {{}}\n  mainObjectFileID: 11400000\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n", encoding="utf-8")


def audio_catalog():
    p = lambda rel: AUDIO / rel
    out = DATA / "AudioEventCatalog.asset"
    out.write_text(f"""%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 0}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {AUDIO_CATALOG_GUID}, type: 3}}
  m_Name: AudioEventCatalog
  m_EditorClassIdentifier: 
  slotActivate: {ptr(p('SFX/SlotMachine/slot_activate_gear_accel_0p5s.wav'), 8300000)}
  slotReelLoop: {ptr(p('SFX/SlotMachine/slot_reel_loop_1s.wav'), 8300000)}
  slotStopClick: {ptr(p('SFX/SlotMachine/slot_stop_click_0p2s.wav'), 8300000)}
  slotPerfectLine: {ptr(p('SFX/SlotMachine/slot_perfect_line_1s.wav'), 8300000)}
  slotPartialLine: {ptr(p('SFX/SlotMachine/slot_partial_line_0p3s.wav'), 8300000)}
  slotPenaltyLine: {ptr(p('SFX/SlotMachine/slot_penalty_glass_0p8s.wav'), 8300000)}
  slashWave: {ptr(p('SFX/Combat/combat_slash_wave_0p5s.wav'), 8300000)}
  magicProjectile: {ptr(p('SFX/Combat/combat_magic_projectile_0p4s.wav'), 8300000)}
  healColumn: {ptr(p('SFX/Combat/combat_heal_column_0p6s.wav'), 8300000)}
  shieldActivate: {ptr(p('SFX/Combat/combat_shield_activate_0p5s.wav'), 8300000)}
  enemyHitVariants:
  - {ptr(p('SFX/Combat/enemy_hit_var1_0p2s.wav'), 8300000)}
  - {ptr(p('SFX/Combat/enemy_hit_var2_0p2s.wav'), 8300000)}
  - {ptr(p('SFX/Combat/enemy_hit_var3_0p2s.wav'), 8300000)}
  enemyDeathVariants:
  - {ptr(p('SFX/Combat/enemy_death_var1_0p5s.wav'), 8300000)}
  - {ptr(p('SFX/Combat/enemy_death_var2_0p5s.wav'), 8300000)}
  - {ptr(p('SFX/Combat/enemy_death_var3_0p5s.wav'), 8300000)}
  bossEntrance: {ptr(p('SFX/Combat/boss_entrance_2s.wav'), 8300000)}
  buttonClick: {ptr(p('SFX/UI/ui_button_click_0p1s.wav'), 8300000)}
  popupOpen: {ptr(p('SFX/UI/ui_popup_open_0p3s.wav'), 8300000)}
  coinGain: {ptr(p('SFX/UI/ui_coin_gain_0p2s.wav'), 8300000)}
  itemGain: {ptr(p('SFX/UI/ui_item_gain_0p3s.wav'), 8300000)}
  victorySting: {ptr(p('SFX/UI/ui_victory_sting_2s.wav'), 8300000)}
  cancelBack: {ptr(p('SFX/UI/ui_cancel_back_0p1s.wav'), 8300000)}
  chamberAmbient: {ptr(p('Ambient/ambient_chamber_loom_fire_loop.wav'), 8300000)}
  forestAmbient: {ptr(p('Ambient/ambient_forest_leaves_loop.wav'), 8300000)}
""", encoding="utf-8")
    out.with_suffix(".asset.meta").write_text(f"fileFormatVersion: 2\nguid: {guid(str(out))}\nNativeFormatImporter:\n  externalObjects: {{}}\n  mainObjectFileID: 11400000\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n", encoding="utf-8")


def main():
    count = make_anims()
    make_scenes()
    make_integrated_scenes()
    audio_meta()
    symbol_catalogs()
    audio_catalog()
    print(f"animation_clips={count}")


if __name__ == "__main__":
    main()
