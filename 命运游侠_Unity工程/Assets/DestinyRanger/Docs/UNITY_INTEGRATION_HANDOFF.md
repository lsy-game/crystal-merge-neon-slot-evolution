# Fate Weaver Unity Integration Handoff

Date: 2026-08-02

This handoff is for the usable asset delivery pass. It does not request or generate validation screenshots.

## Current State

- All file-level art, UI, furniture, audio, tone profile, symbol catalog, and animation clip assets are present.
- Runtime and editor C# compile checks pass outside Unity batch mode.
- `Tools/fate_weaver_static_audit.py` passes.
- `Tools/fate_weaver_requirement_audit.py` records no missing file-level deliverables.
- Unity batch mode currently fails before project import with `IPC channel to LicensingClient doesn't exist; aborting`.

## Integration Entry Points

Use one of these inside the target Unity project:

1. Automatic request marker:
   - Marker file: `Temp/FateWeaverRunIntegrationBuild.request`
   - When Unity imports `Assets/DestinyRanger/Editor/FateWeaverDeferredIntegrationRunner.cs`, it checks the marker after script reload and then polls once per second while the editor is idle.
   - The runner waits while Unity is compiling, updating assets, in Play Mode, or about to change Play Mode, then runs the integration build.

2. Manual menu:
   - `Destiny Ranger/Fate Weaver/Run Deferred Integration Now`
   - Runs the same build and validation path without relying on the marker.
   - Leave Play Mode before running this menu item.

3. Builder menu:
   - `Destiny Ranger/Fate Weaver/Build Integration Prefabs And Scenes`
   - Then run `Destiny Ranger/Fate Weaver/Validate Built Integration` and `Destiny Ranger/Fate Weaver/Validate Full Delivery`.

## Expected Generated Unity Outputs

After successful editor integration, these files should exist:

- `Assets/DestinyRanger/Prefabs/FateWeaver/SlotMachine/ChamberSlotMachine.prefab`
- `Assets/DestinyRanger/Prefabs/FateWeaver/SlotMachine/ForestSlotMachine.prefab`
- `Assets/DestinyRanger/Prefabs/FateWeaver/UI/FateWeaverUiCanvas.prefab`
- `Assets/DestinyRanger/Prefabs/FateWeaver/Audio/FateWeaverAudioHub.prefab`
- `Assets/DestinyRanger/Prefabs/FateWeaver/Furniture/*.prefab`
- `Assets/Scenes/FateWeaver_ChamberIntegrated.unity`
- `Assets/Scenes/FateWeaver_ForestIntegrated.unity`
- `Assets/DestinyRanger/Docs/UNITY_DEFERRED_RUN_REPORT.md`

The deferred run report status should be `complete`. If it is `failed`, the report contains the Unity exception text.

The two integrated scene files may already exist as offline fallback shells. Those shells contain the main camera, `SceneColorTint`, background SpriteRenderer, and named SlotMachine/UI/Audio prefab anchors. A successful Unity editor integration run should overwrite or upgrade them with fully instantiated prefab objects.

## No-Screenshot Boundary

Do not run `RenderPrototypePreviewBatch` for this handoff. The requested delivery is usable assets and Unity integration, not validation images.
