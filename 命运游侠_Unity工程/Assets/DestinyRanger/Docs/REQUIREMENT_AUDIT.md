# Fate Weaver Requirement Audit

Generated from `Tools/fate_weaver_requirement_audit.py`.

This audit maps the staged delivery requirements to current project evidence. It does not replace Unity Editor import, preview, or PlayMode verification.

## Summary

- Complete File Level: 9
- Blocked Unity Verification: 7
- Missing Or Incomplete: 0

## Items

| Stage | Status | Requirement | Evidence |
| --- | --- | --- | --- |
| Stage 1 | Blocked Unity Verification | Four scene backgrounds exist at required dimensions and import as sprites. | `Assets/DestinyRanger/Art/Generated/FateWeaverFull/Backgrounds/fate-weaver-chamber-bg_1290x2796.png`<br>`Assets/DestinyRanger/Art/Generated/FateWeaverFull/Backgrounds/fate-weaver-battle-forest-bg_1290x1398.png`<br>`Assets/DestinyRanger/Art/Generated/FateWeaverFull/Backgrounds/fate-weaver-battle-volcano-bg_1290x1398.png`<br>`Assets/DestinyRanger/Art/Generated/FateWeaverFull/Backgrounds/fate-weaver-battle-void-boss-bg_1290x1398.png` |
| Stage 1 | Complete File Level | SceneToneProfile assets exist for chamber, forest, volcano, and void. | `Assets/DestinyRanger/Data/SceneToneProfiles/ChamberToneProfile.asset`<br>`Assets/DestinyRanger/Data/SceneToneProfiles/ForestToneProfile.asset`<br>`Assets/DestinyRanger/Data/SceneToneProfiles/VoidToneProfile.asset`<br>`Assets/DestinyRanger/Data/SceneToneProfiles/VolcanoToneProfile.asset` |
| Stage 1 | Complete File Level | SceneColorTint, Tintable tag, common shadow, and art postprocessor exist. | `Assets/DestinyRanger/Scripts/SceneColorTint.cs`<br>`ProjectSettings/TagManager.asset`<br>`Assets/DestinyRanger/Art/Common/shadow_default.png`<br>`Assets/DestinyRanger/Editor/FateWeaverAssetPostprocessor.cs` |
| Stage 2 | Complete File Level | Aileen, Grick, and Luna animation frame sets and shadow frames exist. | `Assets/DestinyRanger/Art/Generated/FateWeaverFull/Characters/` |
| Stage 2 | Complete File Level | Five forest monster frame sets and shadows exist. | `Assets/DestinyRanger/Art/Generated/FateWeaverFull/Monsters/Forest/` |
| Stage 2 | Blocked Unity Verification | Offline AnimationClip assets exist for generated character and monster frames. | `Assets/DestinyRanger/Animations/FateWeaver/` |
| Stage 3 | Complete File Level | Chamber and forest slot machine layers exist. | `Assets/DestinyRanger/Art/Generated/FateWeaverFull/SlotMachine/` |
| Stage 3 | Complete File Level | Twenty-four scene symbol PNGs, six disabled symbols, and six highlight symbols exist. | `Assets/DestinyRanger/Art/Generated/FateWeaverFull/Symbols/` |
| Stage 3 | Blocked Unity Verification | SceneSymbolSet assets exist for scene-specific symbol binding. | `Assets/DestinyRanger/Data/Symbols/ChamberSymbolSet.asset`<br>`Assets/DestinyRanger/Data/Symbols/ForestSymbolSet.asset`<br>`Assets/DestinyRanger/Data/Symbols/VoidSymbolSet.asset`<br>`Assets/DestinyRanger/Data/Symbols/VolcanoSymbolSet.asset` |
| Stage 4 | Complete File Level | UI icons, panels, buttons, bars, and title art exist. | `Assets/DestinyRanger/Art/Generated/FateWeaverFull/UI/` |
| Stage 4 | Blocked Unity Verification | TextMeshPro package declaration and font source files exist. | `Packages/manifest.json`<br>`Assets/Fonts/STHeiti Medium.ttc`<br>`Assets/Fonts/Songti.ttc`<br>`Assets/DestinyRanger/Docs/FONT_RENDERING_PLAN.md` |
| Stage 5 | Complete File Level | Furniture PNGs, shadows, and FurnitureItem data assets exist. | `Assets/DestinyRanger/Art/Generated/FateWeaverFull/Furniture/`<br>`Assets/DestinyRanger/Data/Furniture/` |
| Stage 6 | Blocked Unity Verification | Audio files and AudioEventCatalog exist. | `Assets/Audio/`<br>`Assets/DestinyRanger/Data/AudioEventCatalog.asset` |
| Stage 7 | Blocked Unity Verification | Integration builder, deferred runner, full delivery validator, static audit, handoff doc, and build script exist. | `Assets/DestinyRanger/Editor/FateWeaverPrefabBuilder.cs`<br>`Assets/DestinyRanger/Editor/FateWeaverDeferredIntegrationRunner.cs`<br>`Assets/DestinyRanger/Editor/FateWeaverDeliveryValidator.cs`<br>`Tools/fate_weaver_static_audit.py`<br>`BuildScripts/fate_weaver_build_integrated_assets.sh`<br>`Assets/DestinyRanger/Docs/UNITY_INTEGRATION_HANDOFF.md`<br>`Assets/DestinyRanger/Docs/SELF_CHECK_REPORT.md` |
| Stage 7 | Blocked Unity Verification | Integration prefabs exist for slot machines, UI canvas, audio hub, and furniture. | `Assets/DestinyRanger/Prefabs/FateWeaver/` |
| Stage 7 | Complete File Level | Chamber and forest integrated Unity scenes exist. | `Assets/Scenes/FateWeaver_ChamberIntegrated.unity`<br>`Assets/Scenes/FateWeaver_ForestIntegrated.unity` |

## Unity-Blocked Verification

The `Blocked Unity Verification` items have file-level evidence but still require Unity Editor import, prefab generation, visual preview, AudioImporter preview, TMP FontAsset generation, or PlayMode checks. Current local Unity batch mode is blocked before project import by Licensing IPC. Use `Assets/DestinyRanger/Docs/UNITY_INTEGRATION_HANDOFF.md` and the Unity menu `Destiny Ranger/Fate Weaver/Run Deferred Integration Now` once the target project is open in the editor.
