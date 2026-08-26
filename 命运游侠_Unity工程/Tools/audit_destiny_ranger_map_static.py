#!/usr/bin/env python3
"""Static movement/readability audit for Destiny Ranger side-scroller maps.

This catches route risks without requiring Unity batch mode. It intentionally
checks the C# source that owns the generated map layouts and movement constants.
"""

from __future__ import annotations

import re
import sys
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
SCRIPT = ROOT / "Assets/DestinyRanger/Scripts/DestinyRangerSideScroller.cs"

MAX_OPTIONAL_PLATFORM_GAP = 6.10
MAX_OPTIONAL_PLATFORM_CLIMB = 2.05
MIN_FLOATING_PLATFORM_CLEARANCE = 2.40
MIN_PLATFORM_CHEST_LIFT = 0.34
MIN_AIR_DECOR_RAISED_Y = 13.90
MIN_BRIDGE_ROPE_AIR_Y = 13.95
MIN_ROUTE_HANGING_DECOR_Y = 13.90
MIN_RUNE_AIR_DECOR_Y = 8.20
MIN_ROUTE_HINT_AIR_Y = 6.70
MAX_CLEAN_WORLD_VFX_BUDGET = 40
MAX_HERO_ACTION_VFX_LIFE_CAP = 0.055
MAX_PLATFORM_EDGE_ASSIST = 0.035
MAX_PLATFORM_SNAP_INSET = 0.050
MAX_MOVEMENT_STUCK_SUPPORT_CEILING_BONUS = 0.065
MIN_FALSE_CLAMP_ESCAPE = 0.68
MIN_FALSE_RIGHT_CLAMP_RECOVER_BOOST = 1.20
MIN_MOVEMENT_STUCK_OPEN_ROUTE_BOOST = 1.30
MIN_OPEN_ROUTE_RIGHT_INTENT_FALLBACK_BOOST = 3.00
MAX_OPEN_ROUTE_RIGHT_INTENT_GATE_GRACE = 0.40
MIN_LOCKED_GATE_ALLOWANCE = 5.70
MIN_STALE_BEHIND_UNLOCK_MARGIN = 0.80
MIN_STALE_BEHIND_GATE_MARGIN = 0.14
MIN_GROUND_COVERAGE_MARGIN = 0.20
MIN_RUNE_SHRINE_COOLDOWN = 300.0
MIN_RUNE_CHEST_DIRECT_OPEN_COOLDOWN = 420.0
MAX_PLATFORM_RUNE_CHEST_SCALE = 0.48


def fail(message: str) -> None:
    print(f"map-static-audit failed: {message}", file=sys.stderr)
    raise SystemExit(1)


def parse_float_constant(source: str, name: str) -> float:
    match = re.search(rf"{re.escape(name)}\s*=\s*([-.\d]+)f", source)
    if not match:
        fail(f"missing float constant {name}")
    return float(match.group(1))


def parse_int_constant(source: str, name: str) -> int:
    match = re.search(rf"{re.escape(name)}\s*=\s*(\d+)", source)
    if not match:
        fail(f"missing int constant {name}")
    return int(match.group(1))


def parse_segment_rights(source: str) -> list[float]:
    match = re.search(r"SegmentRights\s*=\s*\{([^}]+)\}", source)
    if not match:
        fail("missing SegmentRights")
    values: list[float] = []
    for raw in match.group(1).split(","):
        raw = raw.strip()
        if raw == "RightBound":
            values.append(parse_float_constant(source, "RightBound"))
        elif raw:
            values.append(float(raw.rstrip("f")))
    if len(values) < 2:
        fail("SegmentRights must contain at least two rooms")
    return values


def extract_layout_body(source: str, method: str) -> str:
    start = source.find(f"private void {method}()")
    if start < 0:
        fail(f"missing layout method {method}")
    brace = source.find("{", start)
    depth = 0
    for index in range(brace, len(source)):
        char = source[index]
        if char == "{":
            depth += 1
        elif char == "}":
            depth -= 1
            if depth == 0:
                return source[brace + 1 : index]
    fail(f"unterminated layout method {method}")
    return ""


def parse_platforms(body: str) -> list[tuple[str, float, float, float, float]]:
    pattern = re.compile(
        r'AddPlatform\("([^"]+)",\s*\d+,\s*new Vector2\(([-.\d]+)f?,\s*([-.\d]+)f?\),\s*new Vector2\(([-.\d]+)f?,\s*([-.\d]+)f?\)\)'
    )
    result = []
    for name, x, y, width, height in pattern.findall(body):
        result.append((name, float(x), float(y), float(width), float(height)))
    if not result:
        fail("no AddPlatform calls parsed")
    return result


def audit_layout(name: str, platforms: list[tuple[str, float, float, float, float]], platform_air_lift: float) -> list[str]:
    warnings: list[str] = []
    ordered = sorted(platforms, key=lambda item: item[1])
    prev_right = None
    prev_top = -2.83  # GroundY + player support offset in runtime.
    for platform_name, x, y, width, height in ordered:
        left = x - width * 0.5
        right = x + width * 0.5
        top = y + platform_air_lift + height * 0.5 + 0.08
        clearance = top - prev_top if prev_right is None else top - (-2.83)
        if clearance < MIN_FLOATING_PLATFORM_CLEARANCE:
            warnings.append(f"{name}/{platform_name}: platform too close to ground clearance={clearance:.2f}")
        if width < 2.05:
            warnings.append(f"{name}/{platform_name}: platform too narrow width={width:.2f}")
        if prev_right is not None:
            gap = max(0.0, left - prev_right)
            climb = top - prev_top
            if gap > MAX_OPTIONAL_PLATFORM_GAP:
                warnings.append(f"{name}/{platform_name}: route gap too wide gap={gap:.2f}")
            if climb > MAX_OPTIONAL_PLATFORM_CLIMB:
                warnings.append(f"{name}/{platform_name}: climb too steep climb={climb:.2f}")
        prev_right = max(prev_right if prev_right is not None else right, right)
        prev_top = top
    return warnings


def require_float_literal_at_least(source: str, label: str, value: str, minimum: float, problems: list[str]) -> None:
    literal = f"GroundY + {value}f"
    if literal not in source:
        problems.append(f"missing raised placement for {label}: {literal}")
        return
    actual = float(value)
    if actual < minimum:
        problems.append(f"{label} too close to floor: {actual:.2f} < {minimum:.2f}")


def extract_method_body_by_signature(source: str, signature: str) -> str:
    start = source.find(signature)
    if start < 0:
        fail(f"missing method signature {signature}")
    brace = source.find("{", start)
    depth = 0
    for index in range(brace, len(source)):
        char = source[index]
        if char == "{":
            depth += 1
        elif char == "}":
            depth -= 1
            if depth == 0:
                return source[brace + 1 : index]
    fail(f"unterminated method signature {signature}")
    return ""


def parse_const_float(body: str, name: str) -> float:
    match = re.search(rf"const float {re.escape(name)}\s*=\s*([-.\d]+)f", body)
    if not match:
        fail(f"missing const float {name}")
    return float(match.group(1))


def parse_ground_wall_tile_count(body: str) -> int:
    match = re.search(r"for\s*\(int i = 0; i < (\d+); i\+\+\)", body)
    if not match:
        fail("missing ground wall tile loop")
    return int(match.group(1))


def audit_ground_wall_coverage(source: str, left_bound: float, right_bound: float) -> list[str]:
    body = extract_method_body_by_signature(source, "private void BuildMainGroundWallArt()")
    tile_width = parse_const_float(body, "tileWidth")
    start_x = parse_const_float(body, "startX")
    count = parse_ground_wall_tile_count(body)
    step = tile_width - 0.06
    left = start_x - tile_width * 0.5
    right = start_x + (count - 1) * step + tile_width * 0.5

    problems: list[str] = []
    if left > left_bound - MIN_GROUND_COVERAGE_MARGIN:
        problems.append(f"ground wall art does not cover left bound: left={left:.2f} bound={left_bound:.2f}")
    if right < right_bound + MIN_GROUND_COVERAGE_MARGIN:
        problems.append(f"ground wall art does not cover right bound: right={right:.2f} bound={right_bound:.2f}")
    if "AdventureSolidGround_FullOpaqueSectionBacking" not in source:
        problems.append("missing opaque solid ground backing")
    return problems


def main() -> int:
    source = SCRIPT.read_text(encoding="utf-8")
    segment_rights = parse_segment_rights(source)
    right_bound = parse_float_constant(source, "RightBound")
    if abs(segment_rights[-1] - right_bound) > 0.01:
        fail("last SegmentRights entry must be RightBound")
    left_bound = parse_float_constant(source, "LeftBound")

    edge_assist = parse_float_constant(source, "PlatformLandingEdgeAssistX")
    snap_inset = parse_float_constant(source, "PlatformLandingEdgeSnapInset")
    support_ceiling_bonus = parse_float_constant(source, "MovementStuckSupportCeilingBonus")
    false_clamp = parse_float_constant(source, "FalseClampEscapeForwardAllowance")
    false_right_boost = parse_float_constant(source, "FalseRightClampRecoverBoost")
    open_route_boost = parse_float_constant(source, "MovementStuckOpenRouteBoost")
    open_route_right_intent_boost = parse_float_constant(source, "OpenRouteRightIntentFallbackBoost")
    open_route_right_intent_gate_grace = parse_float_constant(source, "OpenRouteRightIntentGateGrace")
    open_route_intent_soft_nudge = parse_float_constant(source, "OpenRouteIntentSoftNudge")
    locked_allowance = parse_float_constant(source, "LockedGateCombatReachAllowance")
    fallback_allowance = parse_float_constant(source, "LockedGateFallbackReachAllowance")
    stale_unlock_margin = parse_float_constant(source, "StaleBehindEnemyUnlockMargin")
    stale_gate_margin = parse_float_constant(source, "StaleBehindEnemyGateMargin")
    rune_shrine_cooldown = parse_float_constant(source, "RuneShrineSpawnCooldownSeconds")
    direct_chest_cooldown = parse_float_constant(source, "RuneChestDirectOpenCooldownSeconds")
    platform_chest_scale = parse_float_constant(source, "PlatformRuneChestVisualScale")
    chest_lift = parse_float_constant(source, "PlatformRewardChestLift")
    clean_vfx_budget = parse_int_constant(source, "CleanWorldVfxBudget")
    hero_action_life_cap = parse_float_constant(source, "HeroActionVfxLifeCap")

    problems: list[str] = []
    if edge_assist > MAX_PLATFORM_EDGE_ASSIST:
        problems.append(f"edge assist too sticky: {edge_assist:.3f}")
    if snap_inset > MAX_PLATFORM_SNAP_INSET:
        problems.append(f"snap inset too sticky: {snap_inset:.3f}")
    if support_ceiling_bonus > MAX_MOVEMENT_STUCK_SUPPORT_CEILING_BONUS:
        problems.append(f"movement stuck support ceiling bonus too high: {support_ceiling_bonus:.3f}")
    if false_clamp < MIN_FALSE_CLAMP_ESCAPE:
        problems.append(f"false clamp escape too small: {false_clamp:.2f}")
    if false_right_boost < MIN_FALSE_RIGHT_CLAMP_RECOVER_BOOST:
        problems.append(f"false right clamp recover boost too small: {false_right_boost:.2f}")
    if open_route_boost < MIN_MOVEMENT_STUCK_OPEN_ROUTE_BOOST:
        problems.append(f"open route stuck boost too small: {open_route_boost:.2f}")
    if open_route_right_intent_boost < MIN_OPEN_ROUTE_RIGHT_INTENT_FALLBACK_BOOST:
        problems.append(f"right intent open route boost too small: {open_route_right_intent_boost:.2f}")
    if open_route_right_intent_gate_grace > MAX_OPEN_ROUTE_RIGHT_INTENT_GATE_GRACE:
        problems.append(f"right intent gate grace too large: {open_route_right_intent_gate_grace:.2f}")
    if open_route_intent_soft_nudge < 0.16:
        problems.append(f"open route intent soft nudge too small: {open_route_intent_soft_nudge:.2f}")
    if locked_allowance < MIN_LOCKED_GATE_ALLOWANCE or fallback_allowance < MIN_LOCKED_GATE_ALLOWANCE:
        problems.append(
            f"locked combat allowances too small: combat={locked_allowance:.2f} fallback={fallback_allowance:.2f}"
        )
    if stale_unlock_margin < MIN_STALE_BEHIND_UNLOCK_MARGIN:
        problems.append(f"stale behind unlock margin too small: {stale_unlock_margin:.2f}")
    if stale_gate_margin < MIN_STALE_BEHIND_GATE_MARGIN:
        problems.append(f"stale behind gate margin too small: {stale_gate_margin:.2f}")
    if rune_shrine_cooldown < MIN_RUNE_SHRINE_COOLDOWN:
        problems.append(f"SLOT shrine cooldown too frequent: {rune_shrine_cooldown:.0f}s")
    if direct_chest_cooldown < MIN_RUNE_CHEST_DIRECT_OPEN_COOLDOWN:
        problems.append(f"direct chest open cooldown too frequent: {direct_chest_cooldown:.0f}s")
    if platform_chest_scale > MAX_PLATFORM_RUNE_CHEST_SCALE:
        problems.append(f"platform rune chest too large: scale={platform_chest_scale:.2f}")
    if chest_lift < MIN_PLATFORM_CHEST_LIFT:
        problems.append(f"platform reward chest too embedded: lift={chest_lift:.2f}")
    if clean_vfx_budget > MAX_CLEAN_WORLD_VFX_BUDGET:
        problems.append(f"clean world vfx budget too high: {clean_vfx_budget:.0f}")
    if hero_action_life_cap > MAX_HERO_ACTION_VFX_LIFE_CAP:
        problems.append(f"hero action vfx life cap too long: {hero_action_life_cap:.3f}")

    layout_methods = [
        "BuildMapLayout_ForestApproach",
        "BuildMapLayout_CanopyClimb",
        "BuildMapLayout_BrokenBridge",
        "BuildMapLayout_RuneRidge",
    ]
    for method in layout_methods:
        body = extract_layout_body(source, method)
        problems.extend(audit_layout(method, parse_platforms(body), parse_float_constant(source, "PlatformRouteAirLift")))
    problems.extend(audit_ground_wall_coverage(source, left_bound, right_bound))

    required_logic = [
        "if (!canAdvance && RoomBlockingEnemyCount(currentSegment) <= 0)",
        "ReconcileMovementSegmentWithPlayerPosition();",
        "IntentAwareMovementRight(beforeX, x)",
        "IntentAwareMovementRight(before.x, 1f)",
        "if (horizontalInput > MovementStuckRecoverInputThreshold && ShouldForceOpenRouteOnRightIntent(playerX, movementRight))",
        "return OpenRouteRightIntentFallback(playerX, movementRight);",
        "return Mathf.Max(CurrentMovementRight(), VisibleCombatApproachMovementRight(playerX))",
        "OptionalRoomObjectiveAllowsAdvance(segment)",
        "CurrentGateActuallyLocksPlayer(currentSegment)",
        "private bool CurrentGateActuallyLocksPlayer(int segment)",
        "if (RoomBlockingEnemyCount(segment) <= 0) return false;",
        "StaleBehindEnemyLockShouldCatchUp(segment, player.transform.position.x)",
        "i == currentSegment && CurrentGateActuallyLocksPlayer(i)",
        "PlatformRewardChestY(x)",
        "BuildMainGroundAdventureCrossSection();",
        "private void BuildMainGroundAdventureCrossSection()",
        "BuildMainGroundAdventureBlockSegments();",
        "private void BuildMainGroundAdventureBlockSegments()",
        "BuildMainGroundStorybookStoneRhythm();",
        "private void BuildMainGroundStorybookStoneRhythm()",
        "AdventureMainGroundCartoonTopGrassCrown_NoCollision",
        "AdventureMainGroundCartoonSolidSoilFace_NoCollision",
        "AdventureMainGroundPlayableMass_UnbrokenNoBackgroundLeak",
        "AdventureMainGroundPlayableGrassCrownContinuous_NoCollision",
        "AdventureMainGroundPlayableFrontBlock_NoCollision_",
        "AdventureMainGroundRoundedStoneBlock_NoCollision_",
        "AdventureMainGroundStorybookStoneCell_NoCollision_",
        "AdventureMainGroundStorybookStoneCellMoss_NoCollision_",
        "AdventureMainGroundStorybookDeepSoilBreak_NoBackgroundLeak_",
        "AdventureMainGroundStorybookRootPocket_NoCollision_",
        "AdventureMainGroundFrontGrassBlade_NoCollision_",
        "AdventureMainGroundHangingRootThread_NoCollision_",
        "AddForegroundProp(\"WhiteFenceSection_Left_NoCollision\"",
        "AddForegroundProp(\"MossStonePillar_MidBack_NoCollision\"",
        "AddForegroundProp(\"VineCurtain_ArchBlend_NoCollision\"",
        "private void AddForegroundProp(string name, int spriteIndex, Vector2 pos, Vector2 size, int order, float alpha)",
        "Mathf.Clamp(alpha, .12f, .52f)",
        "BuildPlatformStageModuleSilhouette(name, pos, size, hasPlatformArt)",
        "AdventurePlatformModuleSolidCore_NoCollision",
        "AdventurePlatformModuleTopGrassCushion_NoCollision",
        "AdventurePlatformModuleStoneBand_NoCollision",
        "AdventurePlatformModuleBottomChunk_NoCollision",
        "float alpha = hasPlatformArt ? .085f : .30f",
        "float edgeAlpha = hasPlatformArt ? .12f : .44f",
        "float grassAlpha = hasPlatformArt ? .16f : .54f",
        "BuildPlatformAdventureCartoonCrossSection(name, pos, size, hasPlatformArt)",
        "AdventureCartoonSolidSoilFace_NoBackgroundLeak",
        "AdventureCartoonGrassCapReadable",
        "AdventureCartoonFootContactShade",
        "BuildPlatformAdventurePlayableBlockFace(name, pos, size, hasPlatformArt)",
        "private void BuildPlatformAdventurePlayableBlockFace",
        "AdventurePlayableBlockContinuousGrassCrown_NoCollision",
        "AdventurePlayableBlockOpaqueSoilFace_NoBackgroundLeak",
        "AdventurePlayableBlockLowerStoneMass_NoBackgroundLeak",
        "AdventurePlayableBlockReadableStoneSlab_NoCollision_",
        "BuildPlatformReadableWalkableCap(name, pos, size, hasPlatformArt)",
        "private void BuildPlatformReadableWalkableCap",
        "AdventureReadableWalkableGrassCap_NoCollision",
        "AdventureReadableWalkableSoilLip_NoCollision",
        "AdventureReadableWalkableFootAo_NoCollision",
        "AdventureReadableWalkableStoneFace_NoCollision_",
        "BuildPlatformAdventureStorybookTileFace(name, pos, size, hasPlatformArt)",
        "private void BuildPlatformAdventureStorybookTileFace",
        "AdventureStorybookTileGrassShelf_NoCollision",
        "AdventureStorybookTileSoilMass_NoBackgroundLeak",
        "AdventureStorybookTileStoneBelly_NoBackgroundLeak",
        "AdventureStorybookTileRoundedStone_NoCollision_",
        "BuildPlatformWalkableReadabilityTicks(name, pos, size, hasPlatformArt)",
        "private void BuildPlatformWalkableReadabilityTicks",
        "AdventureWalkableCenterReadCap_NoCollision",
        "AdventureWalkableLeftEdgeTick_NoCollision",
        "AdventureWalkableRightEdgeTick_NoCollision",
        "AdventureWalkableFootingCenterAo_NoCollision",
        "BuildPlatformFinalPlayableTileRead(name, pos, size, hasPlatformArt)",
        "private void BuildPlatformFinalPlayableTileRead",
        "AdventureSolidFinalWalkableGrassBlanket_NoCollision",
        "AdventureSolidFinalSoilWall_NoBackgroundLeak",
        "AdventureSolidFinalStoneBelly_NoBackgroundLeak",
        "AdventureSolidFinalBottomSeal_NoBackgroundLeak",
        "AdventureSolidFinalRoundedStoneBlock_NoCollision_",
        "AdventureWalkableMossStepTick_NoCollision_",
        "AdventureWalkableTopGrassNeedle_NoCollision_",
        "float artBlend = hasPlatformArt ? .82f : 1f",
        "float alpha = hasPlatformArt ? .22f : .42f",
        "BuildAdventureAirCanopyComposition(root)",
        "BuildLayoutThemeDressing(layout)",
        "BuildForestApproachThemeDressing(parent)",
        "BuildCanopyClimbThemeDressing(parent)",
        "BuildBrokenBridgeThemeDressing(parent)",
        "BuildRuneRidgeThemeDressing(parent)",
        "BuildLayoutThemeGroundStoryLayer(parent, layout)",
        "private void BuildLayoutThemeGroundStoryLayer(Transform parent, int layout)",
        "BuildAdventureRouteFootlineMemoryLayer(parent, layout)",
        "private void BuildAdventureRouteFootlineMemoryLayer(Transform parent, int layout)",
        "AdventureRouteMemory_UniversalGrassRhythm_NoCollision",
        "AdventureRouteMemory_ForestTinyMushroomFootline_NoCollision",
        "AdventureRouteMemory_CanopyRootToeRhythm_NoCollision",
        "AdventureRouteMemory_BrokenBridgeStoneFootline_NoCollision",
        "AdventureRouteMemory_RuneShardFootline_NoCollision",
        "BuildForestGroundStoryLayer(parent)",
        "BuildCanopyGroundStoryLayer(parent)",
        "BuildBrokenBridgeGroundStoryLayer(parent)",
        "BuildRuneRidgeGroundStoryLayer(parent)",
        "LayoutThemeGround_ForestPicketFenceRun_NoCollision",
        "LayoutThemeGround_CanopyRootFootClusterA_NoCollision",
        "LayoutThemeGround_BrokenBridgeLowRubbleA_NoCollision",
        "LayoutThemeGround_RuneObeliskA_NoCollision",
        "AddProceduralPicketFenceCluster",
        "BuildRuneGroundObelisk",
        "BuildLayoutThemeMidgroundStoryBackdrop(parent, layout)",
        "private void BuildLayoutThemeMidgroundStoryBackdrop(Transform parent, int layout)",
        "LayoutThemeMidground_ForestVillageRoofline_NoCollision",
        "LayoutThemeMidground_CanopyTreeVillage_NoCollision",
        "LayoutThemeMidground_BrokenBridgeLargeArch_NoCollision",
        "LayoutThemeMidground_RuneRidgeDistantPillars_NoCollision",
        "BuildMidgroundForestVillageRoofline",
        "BuildMidgroundCanopyVillage",
        "BuildMidgroundBrokenBridgeTrace",
        "BuildMidgroundRuneRidgePillars",
        "BuildLayoutThemeSkyDressing(parent, layout)",
        "BuildAdventureRouteStorybookLandmarks(parent, layout)",
        "private void BuildAdventureRouteStorybookLandmarks",
        "StorybookLandmark_StartMushroomHome_NoCollision",
        "StorybookLandmark_CanopyRootArch_NoCollision",
        "StorybookLandmark_BrokenBridgeDebrisA_NoCollision",
        "StorybookLandmark_RuneShrubA_NoCollision",
        "BuildStorybookMushroomHouse",
        "BuildStorybookFenceCluster",
        "BuildStorybookStumpCluster",
        "BuildStorybookBrokenBridgeDebris",
        "LayoutThemeSky_ForestCrownLeftHigh_NoCollision",
        "LayoutThemeSky_CanopyLeafBridgeHigh_NoCollision",
        "LayoutThemeSky_BrokenBridgeArchTraceHigh_NoCollision",
        "LayoutThemeSky_RuneRidgeFloatingRunesHigh_NoCollision",
        "AirDecor_TreetopCluster_CeilingLeft_NoCollision",
        "AirDecor_UpperCanopyRim_LeftHigh_NoCollision",
        "AirDecor_NearCanopyFrame_LeftHigh_NoCollision",
        "AirDecor_VineCurtain_DepthMid_NoCollision",
        "AirDecor_RuinedArchSilhouette_BackHigh_NoCollision",
        "LayoutTheme_ForestFlowerBank_Left_NoCollision",
        "LayoutTheme_CanopyVerticalRootA_NoCollision",
        "LayoutTheme_BrokenBridgeRopeHighA_NoCollision",
        "LayoutTheme_RuneRidgeCrystalHighA_NoCollision",
        "_RootMark_NoCollision",
        "_LeafStep_NoCollision",
        "_FootCue_NoYellowPlate_NoCollision",
        "_StoneA_NoCollision",
        "_ColdGlow_NoYellowPlate_NoCollision",
        "_FlatStone_NoCollision",
        "_GrassCap_NoCollision",
        "_RopeA_NoCollision",
        "_Crystal_NoCollision_",
        "_Core_NoYellowPlate_NoCollision_",
        "RouteDressing_PlatformFootGrass_NoCollision_",
        "RouteDressing_PlatformLandingToeL_NoCollision_",
        "RouteDressing_PlatformLandingToeR_NoCollision_",
        "float midY = Mathf.Lerp(prev.Top, next.Top, .5f) + .74f",
        "RouteDressing_PlatformGapMossEcho_NoCollision_",
        "RouteDressing_PlatformGapStoneEcho_NoCollision_",
        "RouteDressing_PlatformGapUnderShade_NoCollision_",
        "RouteDressing_PlatformJumpArcLeaf_NoCollision_",
        "float arcY = Mathf.Lerp(prev.Top, next.Top, t) + .98f + Mathf.Sin(t * Mathf.PI) * .30f",
        "IsRecoverableFalseRightClamp(pos.x, movementRight)",
        "IsRecoverableFalseRightClamp(playerX, movementRight)",
        "RecoverableFalseClampMovementRight(pos.x, movementRight)",
        "return RecoverableFalseClampMovementRight(playerX, movementRight)",
        "private float ClampIntentAwarePlayerX(float playerX, float targetX, float horizontalIntent)",
        "return Mathf.Clamp(targetX, LeftBound, IntentAwareMovementRight(playerX, horizontalIntent));",
        "ClampIntentAwarePlayerX(pos.x, pos.x + facing * distance, facing)",
        "ClampIntentAwarePlayerX(pos.x, pos.x + attackFacing * distance, attackFacing)",
        "private bool TryFindMovementStuckRecovery",
        "private bool TryEmergencyOpenRouteRelease(Vector3 pos, float horizontalInput, float movementRight, out Vector3 recovered)",
        "TryEmergencyOpenRouteRelease(pos, horizontalInput, movementRight, out recovered)",
        "MovementStuckRecoverMinAdvance",
        "MovementStuckSupportCeilingBonus",
        "PlatformSupportUnstickTolerance + MovementStuckSupportCeilingBonus",
        "MovementStuckOpenRouteBoost",
        "GateEdgeSoftReleaseNudge",
        "OpenRouteIntentSoftNudge",
        "GateEdgeSoftReleaseRange",
        "ApplyGateEdgeSoftReleaseNudge(ref pos, x, beforeX, movementRight)",
        "ApplyOpenRouteIntentUnstickNudge(ref pos, x, beforeX, movementRight)",
        "private void ApplyGateEdgeSoftReleaseNudge(ref Vector3 pos, float horizontalInput, float beforeX, float movementRight)",
        "private void ApplyOpenRouteIntentUnstickNudge(ref Vector3 pos, float horizontalInput, float beforeX, float movementRight)",
        "OpenRouteEmergencyReleaseNudge",
        "PlatformEdgeHorizontalEscapeNudge",
        "PlatformEdgeHorizontalEscapeGrace",
        "PlatformEdgeEscapeSupportToleranceBonus",
        "PlatformEdgeStuckRecoveryOutset",
        "ApplyPlatformEdgeHorizontalEscape(ref pos, x, beforeX, movementRight)",
        "private void ApplyPlatformEdgeHorizontalEscape(ref Vector3 pos, float horizontalInput, float beforeX, float movementRight)",
        "float edgeEscapeMovementRight = Mathf.Max(movementRight, IntentAwareMovementRight(beforeX, horizontalInput))",
        "float movementRight = IntentAwareMovementRight(before.x, direction)",
        "CurrentStandingPlatformAt(new Vector3(beforeX, pos.y, pos.z), PlatformSupportUnstickTolerance + PlatformEdgeEscapeSupportToleranceBonus)",
        "ApplyPlatformEdgeHorizontalEscape(ref pos, direction, before.x, movementRight)",
        "TryRecoverOffPlatformEdge(pos, direction, movementRight, supportCeiling, out recovered)",
        "private bool TryRecoverOffPlatformEdge(Vector3 pos, float direction, float movementRight, float supportCeiling, out Vector3 recovered)",
        "PlatformSupportUnstickTolerance + PlatformEdgeEscapeSupportToleranceBonus + MovementStuckSupportCeilingBonus",
        "platform.Right + PlatformLandingEdgeAssistX + PlatformEdgeStuckRecoveryOutset",
        "platform.Left - PlatformLandingEdgeAssistX - PlatformEdgeStuckRecoveryOutset",
        "QuickRunInputWindow",
        "QuickRunHoldDuration",
        "QuickRunSpeedMultiplier",
        "QuickRunJoystickThreshold",
        "QuickRunFootstepInterval",
        "QuickRunCameraLookAheadBonus",
        "QuickRunRoomRatingMinSeconds",
        "QuickRunAirCarryDuration",
        "QuickRunAirCarryMinInput",
        "QuickRunLandingCarryDuration",
        "QuickRunLandingEdgeAssistBonus",
        "roomQuickRunStart",
        "quickRunTimeThisRun",
        "quickRunCameraTimer",
        "quickRunAirCarryTimer",
        "quickRunAirCarryDirection",
        "UpdateQuickRunIntent(x, down, dt)",
        "private void UpdateQuickRunIntent(float horizontalInput, bool down, float dt)",
        "UpdateQuickRunAirCarry(x, down, dt)",
        "private void UpdateQuickRunAirCarry(float horizontalInput, bool down, float dt)",
        "QuickRunAirCarryActive(x, down)",
        "private bool QuickRunAirCarryActive(float horizontalInput, bool down)",
        "StartQuickRunJumpCarry();",
        "private void StartQuickRunJumpCarry()",
        "QuickRunJumpCarryFootLine_NoYellowPlate",
        "QuickRunJumpCarryLiftTick_NoYellowPlate",
        "ContinueQuickRunAfterLanding(x, quickRunAirCarry)",
        "private void ContinueQuickRunAfterLanding(float horizontalInput, bool carriedQuickRun)",
        "QuickRunLandingCarryFootLine_NoYellowPlate",
        "QuickRunLandingCarryForwardTick_NoYellowPlate",
        "float landingEdgeAssistX = PlatformLandingEdgeAssistX + (quickRunAirCarry ? QuickRunLandingEdgeAssistBonus : 0f)",
        "pos.x >= p.Left - landingEdgeAssistX",
        "pos.x <= p.Right + landingEdgeAssistX",
        "TrackQuickRunRoomUsage(quickRun, dt)",
        "private void TrackQuickRunRoomUsage(bool quickRun, float dt)",
        "baseDistance += QuickRunCameraLookAheadBonus * Mathf.Clamp01(magnitude)",
        "JoystickFullTiltQuickRun(horizontalInput)",
        "private bool JoystickFullTiltQuickRun(float horizontalInput)",
        "ActivateQuickRun(dir)",
        "private bool QuickRunActive(float horizontalInput, bool down)",
        "private bool SwiftObjectiveSucceeded(int segment)",
        "QuickRunSecondsInRoom(segment) >= QuickRunRoomRatingMinSeconds",
        "private float QuickRunSecondsInRoom(int segment)",
        "QuickRunStartFootLine_NoYellowPlate",
        "QuickRunSpeedTick_NoYellowPlate",
        "ResetQuickRunIntent();",
        "joystickIntentText.text = \"疾跑\";",
        "BuildPlatformAdventureKingLandingLanguage(name, pos, size, hasPlatformArt)",
        "private void BuildPlatformAdventureKingLandingLanguage(string name, Vector2 pos, Vector2 size, bool hasPlatformArt)",
        "_AdventureKingPlayableTopMass_NoCollision",
        "_AdventureKingFootReadBrightLip_NoCollision",
        "_AdventureKingOpaqueStoneUnderTop_NoBackgroundLeak",
        "BuildPlatformAdventureKingSolidSideLanguage(name, pos, size, hasPlatformArt)",
        "private void BuildPlatformAdventureKingSolidSideLanguage(string name, Vector2 pos, Vector2 size, bool hasPlatformArt)",
        "_AdventureKingSolidReadableSoilWall_NoBackgroundLeak",
        "_AdventureKingSolidReadableGrassCrown_NoCollision",
        "_AdventureKingSolidReadableRoundedStone_NoCollision_",
        "BuildEnemySceneIntegratedPolish(root.transform, kind, elite, sr.sortingOrder)",
        "private void BuildEnemySceneIntegratedPolish(Transform parent, EnemyKind kind, bool elite, int baseOrder)",
        "EnemySceneIntegratedContactShadow_CleanEllipse_NoEchoBody",
        "name.IndexOf(\"NoEchoBody\", StringComparison.Ordinal) >= 0",
        "name.IndexOf(\"CleanEllipse\", StringComparison.Ordinal) >= 0",
        "ClampCrispMotionVfx(name, ref size, ref color, ref life)",
        "private static void ClampCrispMotionVfx(string name, ref Vector2 size, ref Color color, ref float life)",
        "OpenRouteStuckRecoveryRight(pos.x, movementRight)",
        "private float OpenRouteStuckRecoveryRight(float playerX, float movementRight)",
        "if (CurrentGateActuallyLocksPlayer(currentSegment)) return movementRight;",
        "open-route-stuck-widen",
        "OpenRouteRightIntentFallbackBoost",
        "PlayerHasOnlyOptionalBlockersAhead(currentSegment, playerX)",
        "private bool PlayerHasOnlyOptionalBlockersAhead(int segment, float playerX)",
        "OpenRouteRightIntentGateGrace",
        "recoverableOpenRouteRightLimit",
        "ShouldForceOpenRouteOnRightIntent(pos.x, movementRight)",
        "OpenRouteRightIntentFallback(pos.x, movementRight)",
        "open-route-right-intent-",
        "open-route-intent-soft-nudge",
        "DebugCanOpenRouteIntentSoftNudge",
        "gate-edge-soft-release",
        "DebugCanGateEdgeSoftReleaseNudge",
        "visual-ground-pocket-recover",
        "StaleBehindEnemyLockShouldCatchUp(currentSegment, player.transform.position.x)",
        "StaleBehindEnemyLockShouldCatchUp(segment, player.transform.position.x)",
        "GateEdgeSoftUnlockShouldCatchUp(currentSegment, player.transform.position.x)",
        "GateEdgeSoftUnlockShouldCatchUp(segment, player.transform.position.x)",
        "private bool GateEdgeSoftUnlockShouldCatchUp(int segment, float playerX)",
        "GateEdgeSoftUnlockRange",
        "GateEdgeSoftUnlockBehindMargin",
        "noticeText.text = \"门未开 · 回头清敌\"",
        "SpawnGateOpenPassableThresholdCue(pos)",
        "private void SpawnGateOpenPassableThresholdCue(Vector3 gatePosition)",
        "GateOpenPassableThresholdGap_NoYellowPlate",
        "GateOpenPassableThresholdToeTickA_NoYellowPlate",
        "GateOpenPassableRightEdgeRelease_NoYellowPlate",
        "ProceedRouteDestinationPin_NoYellowPlate",
        "GateOpenPlayerProceedDestinationPin_NoYellowPlate",
        "LastEnemyRoomClearPlayerDestinationPin_NoTexturePlate",
        "RoomObjectiveProceedDestinationPin_NoYellowPlate",
        "RoomObjectiveHudOnlyHintTick_NoYellowPlate",
        "private bool StaleBehindEnemyLockShouldCatchUp(int segment, float playerX)",
        "blockers.All(e => e.Root.transform.position.x < playerX - StaleBehindEnemyUnlockMargin)",
        "AdventureGroundWallArt_FullBottomCoverage_",
        "AdventureGroundWallArt_BottomSeal_NoBackgroundLeak",
        "AdventureSolidGround_FullOpaqueSectionBacking",
        "RuneShrineSpawnCooldownSeconds = 300f",
        "RuneChestDirectOpenCooldownSeconds = 420f",
        "MaxAutomaticRuneShrinesPerStage = 1",
        "automaticRuneShrinesThisStage = 0;",
        "private bool AutomaticRuneShrineQuotaReady()",
        "private void ConsumeAutomaticRuneShrineQuota()",
        "if (!AutomaticRuneShrineQuotaReady())",
        "ConsumeAutomaticRuneShrineQuota();",
        "MaxCriticalRoomRewardsPerRun = 2",
        "PlatformRuneChestVisualScale = .46f",
        "fateEnergy = Mathf.Min(99f, fateEnergy + (RuneSpawnCooldownReady() ? 32f : 18f))",
        "bool spawnedRuneShrine = TrySpawnFateShrineAfterSafeRoomClear(segment);",
        "if (!spawnedRuneShrine) MaybeOfferRoomRewardChoice(segment);",
        "private bool TrySpawnFateShrineAfterSafeRoomClear",
        "if (rollingRune || shrineAvailable || choosingRoomReward || segment == ShopRoomIndex || segment == BossRoomIndex) return false;",
        "if (!RoomClearAllowsAutomaticRuneShrine(segment)) return false;",
        "private bool CurrentSegmentAllowsAutomaticRuneShrine()",
        "private static bool RoomClearAllowsAutomaticRuneShrine(int segment)",
        "private static string RoomClearRewardWorldToken(int segment, float extraFate)",
        "return extraFate > 0f ? \"+金币 +命运↑\" : \"+金币 +命运\";",
        "return segment == EliteRoomIndex || segment == BossRoomIndex - 1;",
        "if (!CurrentSegmentAllowsAutomaticRuneShrine())",
        "noticeText.text = \"命运能量临界：留到关键房启封\"",
        "private static bool RoomRewardChoiceAllowedForSegment(int segment)",
        "if (roomRewardsChosenThisRun >= MaxCriticalRoomRewardsPerRun) return;",
        "if (!RoomRewardChoiceAllowedForSegment(segment)) return;",
        "RoomRewardTitlePlate_",
        "RoomRewardValuePlate_",
        "RoomRewardSynergyPlate_",
        "RoomRewardRecommendPlate_",
        "roomRewardRecommendPlateImages[i].color = option.Recommended ? new Color(.09f, .070f, .018f, .34f) : Color.clear",
        "FitHudText(label, 38, 52)",
        "label.rectTransform.sizeDelta = new Vector2(386, 118)",
        "fateEnergy = Mathf.Min(99f, fateEnergy + (roomClear ? 12f : 7f))",
        "TitleFrame\", root, new Vector2(0, -890), new Vector2(1760, 1360)",
        "TitleCopyPlate\", root, new Vector2(0, -724), new Vector2(1520, 304)",
        "TitleLongCopyContainmentPlate\", root, new Vector2(0, -782), new Vector2(1460, 144)",
        "TitleButtonContainmentPlate\", root, new Vector2(0, -1100), new Vector2(1460, 520)",
        "FitHudText(titleStatsText, 30, 38)",
        "FitHudText(titleModeGuideText, 26, 34)",
        "label.resizeTextForBestFit = true",
        "return $\"最佳 {bestTime} · K{bestKills} · {TrialHeatLabel()} · {daily} · 成就 {unlockedAchievements.Count}/{AchievementIds.Length}\"",
        "return $\"今日 {time}/K{kills}\"",
        "PauseFrame\", root, Vector2.zero, new Vector2(980, 620)",
        "PauseHintContainmentPlate",
        "PauseButtonContainmentPlate",
        "FitHudText(pauseReasonText, 22, 29)",
        "FitHudText(pauseHint, 22, 28)",
        "触屏为主：右侧攻击/跳跃/闪避/技能/启封",
        "AchievementFrame\", root, Vector2.zero, new Vector2(1320, 1160)",
        "AchievementSummaryContainmentPlate",
        "AchievementListContainmentPlate",
        "AchievementNoteContainmentPlate",
        "BuildAchievementSummaryText",
        "FitHudText(achievementSummaryText, 22, 28)",
        "achievementBodyText.lineSpacing = .80f",
        "FitHudText(achievementBodyText, 15, 20)",
        "CompactAchievementProgressText",
        "RuneCodexFrame\", root, Vector2.zero, new Vector2(1340, 1160)",
        "RuneCodexSummaryContainmentPlate",
        "RuneCodexListContainmentPlate",
        "RuneCodexComplianceContainmentPlate",
        "RuneCodexSummaryText",
        "FitHudText(runeCodexSummaryText, 23, 30)",
        "runeCodexBodyText.lineSpacing = .86f",
        "FitHudText(runeCodexBodyText, 18, 24)",
        "SettingsFrame\", root, Vector2.zero, new Vector2(1380, 1300)",
        "SettingsBadgeContainmentPlate",
        "SettingsStatusContainmentPlate",
        "SettingsVolumeContainmentPlate",
        "SettingsAssistContainmentPlate",
        "SettingsControlContainmentPlate",
        "SettingsActionContainmentPlate",
        "FitHudText(settingsComfortBadgesText, 19, 25)",
        "settingsStatusText.lineSpacing = .84f",
        "FitHudText(settingsStatusText, 18, 25)",
        "AboutFrame\", root, Vector2.zero, new Vector2(1320, 1120)",
        "AboutBodyContainmentPlate",
        "AboutComplianceContainmentPlate",
        "AboutReleaseBlockerContainmentPlate",
        "AboutClearStateContainmentPlate",
        "AboutActionContainmentPlate",
        "FitHudText(body, 21, 28)",
        "FitHudText(compliance, 20, 26)",
        "FitHudText(aboutReleaseBlockerText, 17, 23)",
        "FitHudText(aboutClearStateText, 18, 23)",
        "OnboardingFrame\", root, new Vector2(0, -700), new Vector2(1380, 1040)",
        "OnboardingLoopContainmentPlate",
        "OnboardingStepContainmentPlate",
        "OnboardingAssistContainmentPlate",
        "OnboardingActionContainmentPlate",
        "OnboardingStepCard_",
        "FitHudText(loopText, 32, 44)",
        "FitHudText(assistLine, 21, 27)",
        "FitHudText(titleText, 26, 34)",
        "FitHudText(detail, 19, 25)",
        "ShopFrame\", root, new Vector2(0, -720), new Vector2(1320, 860)",
        "ShopOfferContainmentPlate",
        "ShopDecisionContainmentPlate",
        "ShopItemTray\", root, new Vector2(0, -832), new Vector2(1140, 318)",
        "ShopComplianceContainmentPlate",
        "ShopActionContainmentPlate",
        "FitHudText(shopOfferText, 30, 40)",
        "FitHudText(shopDecisionText, 24, 32)",
        "FitHudText(shopComplianceText, 20, 26)",
        "FitHudText(adviceText, 17, 23)",
        "VictoryFrame\", root, Vector2.zero, new Vector2(1380, 1180)",
        "VictorySummaryContainmentPlate",
        "VictoryStatsContainmentPlate",
        "VictoryActionContainmentPlate",
        "FitHudText(victoryCauseText, 24, 31)",
        "FitHudText(victoryStatsText, 17, 21)",
        "FitHudText(victoryAssistNoteText, 18, 23)",
        "CompactRunLedger(runeHistory, 2, \"未启封\")",
        "CompactRunLedger(rewardHistory, 2, \"未选择\")",
        "const int maxLength = 26",
        "HeroActionVfxAlphaScale",
        "Mathf.Min(HeroActionVfxLifeCap",
        "HeroNoEchoBodyLifeCap",
        "HeroNoEchoBodyAlphaCap",
        "HeroActionLineHeightCap",
        "ComboFinisherLineWidthCap",
        "private static bool IsHeroActionMotionVfxName(string name)",
        "bool heroAction = IsHeroActionMotionVfxName(name)",
        "noEchoBody ? HeroNoEchoBodyLifeCap",
        "new Vector2(1.16f, .014f), new Color(.96f, 1f, 1f, .15f * alphaScale)",
        "SpawnHeroComboCrispFrameClamp(impactPos, attackFacing)",
        "HeroComboCrispFrameClampFront_NoEchoBody_NoYellowPlate",
        "HeroComboCrispFootLock_NoEchoBody_NoYellowPlate",
        "SpawnHeroSkillReleaseCrispFrameClamp(skill, castFacing, color, power)",
        "HeroSkillReleaseHandFrameClamp_NoEchoBody_NoYellowPlate",
        "HeroSkillReleaseFootFrameClamp_NoEchoBody_NoYellowPlate",
        "SpawnHeroSkillFrameReadabilityTriad(skill, castFacing, color, power)",
        "HeroSkillFrameReadHandPin_NoEchoBody_NoYellowPlate_ThinNoBlur",
        "HeroSkillFrameReadFootPlant_NoEchoBody_NoYellowPlate_ThinNoBlur",
        "SpawnHeroSkillReleaseThreeBeatRead(skill, castFacing, color, power)",
        "private void SpawnHeroSkillReleaseThreeBeatRead(SkillKind skill, int castFacing, Color color, float power)",
        "HeroSkillThreeBeatWindupHand_NoEchoBody_NoYellowPlate_ThinNoBlur",
        "HeroSkillThreeBeatReleaseSpine_NoEchoBody_NoYellowPlate_ThinNoBlur",
        "HeroSkillThreeBeatRecoveryFoot_NoEchoBody_NoYellowPlate_ThinNoBlur",
        "HeroSkillThreeBeatForwardReleaseTick_NoEchoBody_NoYellowPlate_ThinNoBlur",
        "HeroSkillThreeBeatAreaCenterTick_NoEchoBody_NoYellowPlate_ThinNoBlur",
        "SpawnSkillImpactReadabilitySnap(skill, position, hitDir, boss)",
        "SkillImpactReadContactStop_NoYellowPlate_ThinNoBlur",
        "SkillImpactReadGroundPin_NoYellowPlate_ThinNoBlur",
        "SpawnSkillImpactThreePointSnap(skill, position, hitDir, boss)",
        "private void SpawnSkillImpactThreePointSnap(SkillKind skill, Vector3 position, Vector2 hitDir, bool boss)",
        "SkillImpactThreePointContactStop_NoYellowPlate_ThinNoBlur",
        "SkillImpactThreePointEnemyAxis_NoYellowPlate_ThinNoBlur",
        "SkillImpactThreePointGroundSnap_NoYellowPlate_ThinNoBlur",
        "EnemyActionSuppressed(e)",
        "private static bool CanEnemyStartAttack(Fighter e)",
        "return !EnemyActionSuppressed(e) && e.AttackTimer <= 0f && !e.Attacking;",
        "else if (CanEnemyStartAttack(e))",
        "attack-startup-suppressed-",
        "DebugAuditEnemyWindupHitCancelCase",
        "windup-hit-cancel-",
        "SpawnMinorEnemyWindupPoseLockCue(e, dir, true, warnColor)",
        "SpawnMinorEnemyWindupPoseLockCue(e, dir, false, warnColor)",
        "EnemyWindupPoseLockAimNeedle_NoTexturePlate",
        "EnemyWindupPoseLockMeleeNeedle_NoTexturePlate",
        "EnemyWindupPoseLockFootBrace_NoTexturePlate",
        "EnemyWindupPoseLockBodyPin_NoTexturePlate",
        "EnemySlimeReadMarkGloss_Cute",
        "EnemyMushroomReadMarkCapStripe_Cute",
        "EnemyArcherReadMarkFeather_CuteThin",
        "BuildEnemyCuteBodyLanguage(root.transform, kind, elite)",
        "EnemyCuteArcherTinyQuiver_ThinNoPlate",
        "EnemyCuteSlimeSoftBellySquash_ThinNoPlate",
        "EnemyCuteMushroomCapDotL_NoPlate",
        "BuildEnemyStorybookPoseAnchors(root.transform, kind, elite)",
        "EnemyStorybookSoftFootOval_NoPlate",
        "EnemyStorybookArcherTinyCape_ThinNoPlate",
        "EnemyStorybookSlimeSquashBase_ThinNoPlate",
        "EnemyStorybookMushroomCapRim_NoPlate",
        "BuildEnemyAdventureBodyVolumes(root.transform, kind, elite)",
        "private void BuildEnemyAdventureBodyVolumes(Transform parent, EnemyKind kind, bool elite)",
        "EnemyAdventureArcherBodyCoat_NoPlate",
        "EnemyAdventureSlimeRoundBody_NoPlate",
        "EnemyAdventureMushroomCapVolume_NoPlate",
        "EnemyAdventureBodyContactAo_NoPlate",
        "BuildEnemyAdventureKingStyleSilhouette(root.transform, kind, elite)",
        "private void BuildEnemyAdventureKingStyleSilhouette(Transform parent, EnemyKind kind, bool elite)",
        "EnemyAdventureKingFootContactAo_NoPlate",
        "EnemyAdventureKingHitSquashAxis_NoPlate",
        "EnemyAdventureKingArcherBowCurve_NoPlate",
        "EnemyAdventureKingSlimeReadableBlob_NoPlate",
        "EnemyAdventureKingMushroomWideCap_NoPlate",
        "BuildEnemyCartoonReactionAnchors(root.transform, kind, elite)",
        "private void BuildEnemyCartoonReactionAnchors(Transform parent, EnemyKind kind, bool elite)",
        "EnemyCartoonReactionFaceFocus_NoPlate",
        "EnemyCartoonReactionHitSquashGuide_NoPlate",
        "EnemyCartoonReactionFootLock_NoPlate",
        "EnemyCartoonReactionBowHand_NoPlate",
        "EnemyCartoonReactionSquashCheek_NoPlate",
        "EnemyCartoonReactionCapBounceRim_NoPlate",
        "BuildEnemyReadableFaceAndFooting(root.transform, kind, elite)",
        "private void BuildEnemyReadableFaceAndFooting(Transform parent, EnemyKind kind, bool elite)",
        "EnemyReadableFootingNoGroundBlob_NoPlate",
        "EnemyReadableArcherEyeLine_NoPlate",
        "EnemyReadableSlimeFaceDotL_NoPlate",
        "EnemyReadableMushroomFaceLine_NoPlate",
        "BuildEnemyAdventureKingRoleReadabilityBadges(root.transform, kind, elite)",
        "private void BuildEnemyAdventureKingRoleReadabilityBadges(Transform parent, EnemyKind kind, bool elite)",
        "EnemyRoleAdventureKingFootPin_NoPlate",
        "EnemyRoleArcherReadableBowArc_NoPlate",
        "EnemyRoleArcherArrowNock_NoPlate",
        "EnemyRoleSlimeRoundCheekBlob_NoPlate",
        "EnemyRoleSlimeElasticBaseWide_NoPlate",
        "EnemyRoleMushroomReadableWideCap_NoPlate",
        "EnemyRoleMushroomTinyFootPair_NoPlate",
        "EnemyRoleEliteLeafCrownTick_NoPlate",
        "BuildEnemyStorybookReadableOverlay(root.transform, kind, elite, sr.sortingOrder)",
        "private void BuildEnemyStorybookReadableOverlay(Transform parent, EnemyKind kind, bool elite, int baseOrder)",
        "EnemyStorybookReadableArcherBlueCoat_NoPlate",
        "EnemyStorybookReadableSlimeRoundBody_NoPlate",
        "EnemyStorybookReadableMushroomRedCap_NoPlate",
        "EnemyStorybookReadableFootContact_NoPlate",
        "BuildEnemyFinalStorybookSilhouette(root.transform, kind, elite, sr.sortingOrder)",
        "private void BuildEnemyFinalStorybookSilhouette(Transform parent, EnemyKind kind, bool elite, int baseOrder)",
        "EnemyFinalStorybookFootPlant_NoPlate",
        "EnemyFinalStorybookArcherBlueHood_NoPlate",
        "EnemyFinalStorybookArcherBowString_NoPlate",
        "EnemyFinalStorybookSlimeBodyBlob_NoPlate",
        "EnemyFinalStorybookSlimeElasticFoot_NoPlate",
        "EnemyFinalStorybookMushroomCap_NoPlate",
        "EnemyFinalStorybookMushroomStem_NoPlate",
        "BuildEnemyFinalCutePlayableSilhouette(root.transform, kind, elite, sr.sortingOrder)",
        "private void BuildEnemyFinalCutePlayableSilhouette(Transform parent, EnemyKind kind, bool elite, int baseOrder)",
        "EnemyFinalCutePlayableFootLock_NoPlate",
        "EnemyFinalCuteArcherBowReadableCurve_NoPlate",
        "EnemyFinalCuteSlimeRoundMass_NoPlate",
        "EnemyFinalCuteMushroomCapMass_NoPlate",
        "BuildEnemyAdventureKingSceneFitLayer(root.transform, kind, elite, sr.sortingOrder)",
        "private void BuildEnemyAdventureKingSceneFitLayer(Transform parent, EnemyKind kind, bool elite, int baseOrder)",
        "EnemyAdventureKingSceneFitContactAo_CleanEllipse_NoEchoBody",
        "EnemyAdventureKingSceneFitArcherBowArc_NoPlate",
        "EnemyAdventureKingSceneFitSlimeSquashBody_NoPlate",
        "EnemyAdventureKingSceneFitMushroomWideCap_NoPlate",
        "EnemyEnvironmentSilhouetteIdleAlpha = 0f",
        ".24f * eliteBoost), bodyOrder);",
        ".25f * eliteBoost), bodyOrder);",
        "BuildEnemyHitIdentityAnchors(root.transform, kind, elite, sr.sortingOrder)",
        "private void BuildEnemyHitIdentityAnchors(Transform parent, EnemyKind kind, bool elite, int baseOrder)",
        "EnemyHitIdentityFreezeFootAnchor_NoPlate",
        "EnemyHitIdentityArcherBowStringAnchor_NoPlate",
        "EnemyHitIdentitySlimeSquashAnchor_NoPlate",
        "EnemyHitIdentityMushroomCapBrace_NoPlate",
        "BuildEnemyAdventureMotionBodyLanguage(root.transform, kind, elite, sr.sortingOrder)",
        "private void BuildEnemyAdventureMotionBodyLanguage(Transform parent, EnemyKind kind, bool elite, int baseOrder)",
        "public bool WasMoving;",
        "e.WasMoving = false;",
        "e.WasMoving = true;",
        "private void UpdateEnemyMotionPose(Fighter e, float dt)",
        "float step = Mathf.Abs(Mathf.Sin(e.MoveAnimTimer * 12.6f))",
        "float kindBounce = e.Kind == EnemyKind.Slime ? .040f : e.Kind == EnemyKind.Mushroom ? .032f : .024f",
        "EnemyAdventureMotionFootAnchor_NoPlate",
        "EnemyAdventureMotionBodyCenter_NoPlate",
        "EnemyAdventureMotionArcherBowArc_NoPlate",
        "EnemyAdventureMotionArcherCapeSweep_NoPlate",
        "EnemyAdventureMotionSlimeElasticBody_NoPlate",
        "EnemyAdventureMotionSlimeSquashFoot_NoPlate",
        "EnemyAdventureMotionMushroomCapMass_NoPlate",
        "EnemyAdventureMotionMushroomFootPair_NoPlate",
        "SpawnEnemyHitStunBodyClamp(e, hitDir, tier)",
        "EnemyHitStunBodyClamp_NoEchoBody_NoYellowPlate",
        "EnemyHitStunFootClamp_NoEchoBody_NoYellowPlate",
        "e.HitStun = Mathf.Max(e.HitStun, EnemyHitStunDuration(e, tier))",
        "hit-stun-never-downgrades",
        "chainedHitEnemy.HitStun = Mathf.Max(chainedHitEnemy.HitStun, EnemyHitStunDuration(chainedHitEnemy, HitTier.Light))",
        "EnemyPostHitRecoveryAfterStunMin",
        "ArcherPostHitRecoveryAfterStunMin",
        "BossPostHitRecoveryAfterStunMin",
        "EnemyPostHitPlayerClearBuffer",
        "EnemyPostHitPlayerClearStrength",
        "EnemyPostHitPlayerClearMaxStep",
        "bool postHitLocked = e.HitStun > 0f || e.AttackLockTimer > 0f",
        "float playerBuffer = postHitLocked ? EnemyPostHitPlayerClearBuffer : EnemyPlayerReadabilityBuffer",
        "float strength = postHitLocked ? EnemyPostHitPlayerClearStrength : EnemyPlayerReadabilityStrength",
        "post-hit-player-clearance-stronger",
        "normal-post-stun-recovery-buffer",
        "archer-post-stun-aim-reset-buffer",
        "return Mathf.Max(delay, EnemyHitStunDuration(e, tier) + recoveryBuffer)",
        "SpawnEnemyHitIdentityFreezeCue(e, hitDir, tier)",
        "private void SpawnEnemyHitIdentityFreezeCue(Fighter e, Vector2 hitDir, HitTier tier)",
        "EnemyHitIdentityFreezeFootAnchor_NoYellowPlate",
        "EnemyHitIdentityArcherStringSnap_NoYellowPlate",
        "EnemyHitIdentitySlimeSquashSnap_NoYellowPlate",
        "EnemyHitIdentityMushroomCapStop_NoYellowPlate",
        "SpawnEnemyCuteBreakBits(e, deathPos, color)",
        "EnemyCuteBreakSlimeBubble_NoPlate",
        "EnemyCuteBreakArcherFeatherNeedle_NoPlate",
        "EnemyCuteBreakMushroomCapChip_NoPlate",
        "e.AttackLockTimer = Mathf.Max(e.AttackLockTimer, Mathf.Min(duration, e.Boss ? .30f : .64f))",
    ]
    for text in required_logic:
        if text not in source:
            problems.append(f"missing anti-air-wall logic: {text}")

    forbidden_dirty_route_names = [
        "_RootMark\",",
        "_LeafStep\",",
        "_FootCue\",",
        "_StoneA\",",
        "_ColdGlow\",",
        "_FlatStone\",",
        "_GrassCap\",",
        "_RopeA\",",
        "_Crystal_\" + i",
        "RouteDressing_PlatformFootGrass_\" + i",
    ]
    for text in forbidden_dirty_route_names:
        if text in source:
            problems.append(f"dirty route/decor child name lacks NoCollision suffix: {text}")

    raised_air_decor = {
        "AirDecor_SkyDepthWash_NoCollision": "14.08",
        "AirDecor_PaintedCanopy_Left_NoCollision": "14.08",
        "AirDecor_PaintedCanopy_Mid_NoCollision": "14.16",
        "AirDecor_PaintedCanopy_Right_NoCollision": "14.06",
        "AirDecor_PaintedFloatingStone_Mid_NoCollision": "14.12",
        "AirDecor_VineCurtain_DepthMid_NoCollision": "14.16",
        "AirDecor_VineCurtain_DepthRight_NoCollision": "14.18",
        "AirDecor_RuinedArchSilhouette_BackHigh_NoCollision": "14.02",
        "AirDecor_NearCanopyFrame_LeftHigh_NoCollision": "13.96",
        "AirDecor_NearCanopyFrame_RightHigh_NoCollision": "14.14",
        "AirDecor_PaintedCloudMist_Mid_NoCollision": "14.08",
        "AirDecor_PaintedCloudMist_Left_NoCollision": "14.04",
        "AirDecor_PaintedRuinArch_Back_NoCollision": "14.02",
        "AirDecor_TreetopCluster_CeilingLeft_NoCollision": "14.10",
        "AirDecor_TreetopCluster_CeilingRight_NoCollision": "14.08",
        "AirDecor_UpperCanopyRim_RightHigh_NoCollision": "14.12",
        "AirDecor_TreetopCluster_DepthLeafShelfA_NoCollision": "14.12",
        "AirDecor_TreetopCluster_DepthLeafShelfB_NoCollision": "14.16",
        "LayoutThemeSky_ForestCrownLeftHigh_NoCollision": "14.08",
        "LayoutThemeSky_ForestCrownRightHigh_NoCollision": "14.18",
        "LayoutThemeSky_CanopyLeafBridgeHigh_NoCollision": "14.10",
        "LayoutThemeSky_CanopyVineClusterHigh_NoCollision": "14.22",
        "LayoutThemeSky_BrokenBridgeArchTraceHigh_NoCollision": "14.06",
        "LayoutThemeSky_BrokenBridgeFarArchTraceHigh_NoCollision": "14.18",
        "LayoutThemeSky_RuneRidgeFloatingRunesHigh_NoCollision": "14.12",
        "LayoutThemeSky_RuneRidgeFarRunesHigh_NoCollision": "14.22",
        "AirDecor_CeilingContinuityLeafMass_MidHigh_NoCollision": "14.32",
        "AirDecor_CeilingContinuityBranch_MidHigh_NoCollision": "14.28",
        "AirDecor_CeilingContinuityRuinTrace_MidHigh_NoCollision": "14.18",
        "AirDecor_CeilingContinuityTinyStone_RightHigh_NoCollision": "14.10",
    }
    for label, value in raised_air_decor.items():
        require_float_literal_at_least(source, label, value, MIN_AIR_DECOR_RAISED_Y, problems)

    bridge_rope_decor = {
        "RouteDressing_BrokenBridgeRopeCenter_NoCollision": "14.04",
        "RouteDressing_BrokenBridgeRopeB_NoCollision": "14.00",
        "LayoutTheme_BrokenBridgeRopeHighA_NoCollision": "14.08",
        "LayoutTheme_BrokenBridgeRopeHighB_NoCollision": "14.04",
        "LayoutTheme_BrokenBridgeRopeHighC_NoCollision": "14.06",
    }
    for label, value in bridge_rope_decor.items():
        require_float_literal_at_least(source, label, value, MIN_BRIDGE_ROPE_AIR_Y, problems)

    route_hanging_decor = {
        "RouteDressing_CanopyRootsA_NoCollision": "13.96",
        "RouteDressing_CanopyRootsB_NoCollision": "14.14",
        "RouteDressing_CanopyRootsC_NoCollision": "13.98",
        "LayoutTheme_CanopyVerticalRootA_NoCollision": "14.14",
        "LayoutTheme_CanopyVerticalRootB_NoCollision": "14.28",
        "LayoutTheme_CanopyVerticalRootC_NoCollision": "14.10",
    }
    for label, value in route_hanging_decor.items():
        require_float_literal_at_least(source, label, value, MIN_ROUTE_HANGING_DECOR_Y, problems)

    rune_air_decor = {
        "LayoutTheme_RuneRidgeCrystalHighA_NoCollision": "8.30",
        "LayoutTheme_RuneRidgeCrystalHighB_NoCollision": "8.48",
        "LayoutTheme_RuneRidgeMistLine_NoCollision": "8.66",
    }
    for label, value in rune_air_decor.items():
        require_float_literal_at_least(source, label, value, MIN_RUNE_AIR_DECOR_Y, problems)

    route_hint_air_decor = {
        "RoomRouteClimbHint_CanopyA_NoCollision": "6.92",
        "RoomRouteClimbHint_CanopyB_NoCollision": "7.15",
        "RoomRouteClimbHint_CanopyC_NoCollision": "6.98",
        "RoomRouteRuneHighTrail_A_NoCollision": "6.88",
        "RoomRouteRuneHighTrail_B_NoCollision": "6.88",
        "RoomRouteRuneHighTrail_C_NoCollision": "6.88",
    }
    for label, value in route_hint_air_decor.items():
        require_float_literal_at_least(source, label, value, MIN_ROUTE_HINT_AIR_Y, problems)

    if problems:
        for problem in problems:
            print(problem, file=sys.stderr)
        return 1

    print(
        "Destiny Ranger map static audit passed. "
        f"rooms={len(segment_rights)} edgeAssist={edge_assist:.3f} snapInset={snap_inset:.3f}"
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
