# Map Variation Spec

v0.1 uses seeded side-scroller routes so the player keeps moving forward instead of fighting on one flat strip. Each stage rebuilds `GeneratedMapLayout` from the current run seed and stage index, then places progress gates, room markers, the boss lock, and SLOT platform rune chests on the active platforms.

The active route is stored in `activeMapLayoutIndex`. HUD and settlement copy must expose it through `MapLayoutShortName`, `MapLayoutGoalLine`, and `MapLayoutRunReview`, so players understand the current run's route theme instead of feeling like platform placement changed silently.

`BuildPlatformRouteBreadcrumbs` keeps the old route hint object names (`PlatformRouteBreadcrumbTick_NoYellowPlate`, `PlatformRouteBreadcrumbLine_NoYellowPlate`, and `PlatformRouteRewardNudge_NoYellowPlate`) as a controlled entry point, but runtime now suppresses persistent world-route breadcrumbs. Route guidance should come from camera framing, platform silhouettes, short landing prediction, and brief clear-room/open-gate cues so the map does not look covered in UI marks or black/blue residual lines.

Each seeded route should keep the visible platform count tight enough for a mobile side-scroller camera: the baseline forest route uses about 8 main platforms, special routes use about 9-10 at most, and extra high/low filler platforms should be removed before adding more UI cues. The goal is a readable forward path with optional high rewards, not a screen full of similar stone sprites.

## Runtime Layouts

- `BuildMapLayout_ForestApproach`: baseline forward route with low and mid platforms.
- `BuildMapLayout_CanopyClimb`: vertical climb route for jump timing and elevated platform rune chests.
- `BuildAdventureAirCanopyComposition`: shared upper-scene composition for all map variants, adding no-collision treetop ceiling, canopy rim, near canopy frame, vine curtains, and distant ruin silhouettes so the top half reads like a finished adventure stage instead of empty background.
- `BuildLayoutThemeDressing`: per-route no-collision visual dressing for forest flowers, canopy roots, broken bridge ropes, and rune ridge crystals/mist. Air-themed pieces must sit clearly above the walkable floor, while ground flowers/mushrooms stay low as footing context.
- `BuildLayoutThemeSkyDressing`: per-route high landmark silhouettes so each run reads as a different adventure stage: forest crown, canopy leaf bridge, broken bridge arch traces, and floating rune shards. These are visual-only `NoCollision` objects and must stay above combat/platform readability.
- `BuildMapLayout_BrokenBridge`: staggered gap route with shorter landing platforms.
- `BuildMapLayout_RuneRidge`: high ridge route emphasizing reward lanes and return drops.

## SLOT Placement

`SpawnRuneVesselOnPlatform` uses `PlatformRewardChestY(x)` after the layout is rebuilt, so platform rune chests sit above the current map variant's walkable top instead of embedding into the platform art or floating at old positions. Preview objects are still named `PreviewMapRuneJar_Left/Mid/Right` for compatibility and exist only for screenshot validation.

Player-owned skill projectiles must also open platform rune chests. `UpdateHitEffects` checks `runeVessels` before enemies, calls `DamageRuneVessel` with `SourceSkill`, then removes non-piercing projectiles. This makes skills a valid way to trigger SLOT progress instead of forcing every chest to be opened by melee.

## Screenshot Checks

Use the editor batch methods below, always with the exact `命运游侠_Unity工程` project path:

- `DestinyRanger.EditorTools.DestinyRangerSceneBuilder.RenderMapPreviewBatch` -> `/private/tmp/destiny-ranger-map-preview.png`
- `DestinyRanger.EditorTools.DestinyRangerSceneBuilder.RenderSettingsPreviewBatch` -> `/private/tmp/destiny-ranger-settings-preview.png`
- `DestinyRanger.EditorTools.DestinyRangerSceneBuilder.RenderCombatPreviewBatch` -> `/private/tmp/destiny-ranger-combat-preview.png`
- `python3 Tools/check_destiny_ranger_visual_noise.py /private/tmp/destiny-ranger-combat-preview.png` must pass after combat preview rendering; it rejects large high-saturation warm/yellow gameplay plates while ignoring allowed HUD/reward zones.

Screenshot acceptance:

- Active map shows multiple platform heights and at least three readable platform rune chest targets.
- Low floating platforms keep a visible air gap above the solid ground. Static QA rejects platforms that sit too close to the floor, and platform rune chests must be lifted above the walkable surface.
- Air decoration and broken-bridge rope dressing must read as suspended scenery, not floor clutter. Static QA locks the raised placements for `AirDecor_PaintedFloatingStone_Mid_NoCollision`, vine curtains, high ruin silhouettes, and bridge ropes.
- Each layout has a distinct high landmark from `LayoutThemeSky_*_NoCollision`; it should add stage identity without becoming a UI plate, route marker, or collision surface.
- Elevated/reward-lane platforms show sparse cold route breadcrumbs, not yellow blocks, large arrows, coin trails, or text.
- Sustained right movement after a room is cleared must never look like an invisible wall. False right clamps are recovered by `IsRecoverableFalseRightClamp` and `RecoverableFalseClampMovementRight`, while the real world boundary remains locked.
- Combat preview uses the same clean in-game VFX grammar as gameplay: `PreviewSlashCoreLine_NoYellowPlate` / `PreviewSlashEdgeLine_NoYellowPlate` / `PreviewThunderSpine_NoYellowPlate` / `PreviewThunderContact_NoYellowPlate`, no full-size slash bitmap, no external thunder sheet, no double-line reward text.
- The visual noise audit reports no `warm/yellow gameplay plate candidates`; this is the automated check for movement, skill, and enemy-attack yellow background boards.
- `RunProgressText` and `RouteMinimapText` show the current route name.
- `ObjectiveText` uses `MapLayoutGoalLine` after a room is cleared, and platform-rune rooms mention that skill projectiles can open chests.
- `VictoryStats` includes `MapLayoutRunReview` so the run summary explains how route choice affected SLOT speed and risk.
- Right-side mobile controls do not cover hit confirmation, objective text, or SLOT hints.
- Settings panel shows `HeroActionBeatToggle` without text overlap.
- SLOT remains framed as combat rune opening, with no paid wager or cash-out language.
