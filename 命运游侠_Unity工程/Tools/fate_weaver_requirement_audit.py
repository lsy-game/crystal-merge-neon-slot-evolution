#!/usr/bin/env python3
from __future__ import annotations

import json
import sys
from argparse import ArgumentParser
from dataclasses import dataclass
from pathlib import Path


PROJECT = Path(__file__).resolve().parents[1]
ART = PROJECT / "Assets/DestinyRanger/Art/Generated/FateWeaverFull"
DATA = PROJECT / "Assets/DestinyRanger/Data"
SCENES = PROJECT / "Assets/Scenes"


@dataclass(frozen=True)
class Requirement:
    stage: str
    item: str
    status: str
    evidence: tuple[str, ...]


STATUS_LABELS = {
    "complete_file_level": "Complete File Level",
    "blocked_unity_verification": "Blocked Unity Verification",
    "missing_or_incomplete": "Missing Or Incomplete",
}


def exists(path: str) -> bool:
    return (PROJECT / path).exists()


def count(pattern: str) -> int:
    return len(list(PROJECT.glob(pattern)))


def evidence_exists(*paths: str) -> tuple[str, ...]:
    return tuple(path for path in paths if exists(path))


def req(stage: str, item: str, expected: bool, evidence: tuple[str, ...], unity_blocked: bool = False) -> Requirement:
    if expected and unity_blocked:
        status = "blocked_unity_verification"
    elif expected:
        status = "complete_file_level"
    else:
        status = "missing_or_incomplete"
    return Requirement(stage, item, status, evidence)


def build_requirements() -> list[Requirement]:
    requirements: list[Requirement] = []

    requirements.append(req(
        "Stage 1",
        "Four scene backgrounds exist at required dimensions and import as sprites.",
        all(exists(path) for path in (
            "Assets/DestinyRanger/Art/Generated/FateWeaverFull/Backgrounds/fate-weaver-chamber-bg_1290x2796.png",
            "Assets/DestinyRanger/Art/Generated/FateWeaverFull/Backgrounds/fate-weaver-battle-forest-bg_1290x1398.png",
            "Assets/DestinyRanger/Art/Generated/FateWeaverFull/Backgrounds/fate-weaver-battle-volcano-bg_1290x1398.png",
            "Assets/DestinyRanger/Art/Generated/FateWeaverFull/Backgrounds/fate-weaver-battle-void-boss-bg_1290x1398.png",
        )),
        evidence_exists(
            "Assets/DestinyRanger/Art/Generated/FateWeaverFull/Backgrounds/fate-weaver-chamber-bg_1290x2796.png",
            "Assets/DestinyRanger/Art/Generated/FateWeaverFull/Backgrounds/fate-weaver-battle-forest-bg_1290x1398.png",
            "Assets/DestinyRanger/Art/Generated/FateWeaverFull/Backgrounds/fate-weaver-battle-volcano-bg_1290x1398.png",
            "Assets/DestinyRanger/Art/Generated/FateWeaverFull/Backgrounds/fate-weaver-battle-void-boss-bg_1290x1398.png",
        ),
        unity_blocked=True,
    ))
    requirements.append(req(
        "Stage 1",
        "SceneToneProfile assets exist for chamber, forest, volcano, and void.",
        count("Assets/DestinyRanger/Data/SceneToneProfiles/*ToneProfile.asset") == 4,
        tuple(str(path.relative_to(PROJECT)) for path in sorted((DATA / "SceneToneProfiles").glob("*.asset"))),
    ))
    requirements.append(req(
        "Stage 1",
        "SceneColorTint, Tintable tag, common shadow, and art postprocessor exist.",
        all(exists(path) for path in (
            "Assets/DestinyRanger/Scripts/SceneColorTint.cs",
            "ProjectSettings/TagManager.asset",
            "Assets/DestinyRanger/Art/Common/shadow_default.png",
            "Assets/DestinyRanger/Editor/FateWeaverAssetPostprocessor.cs",
        )),
        evidence_exists(
            "Assets/DestinyRanger/Scripts/SceneColorTint.cs",
            "ProjectSettings/TagManager.asset",
            "Assets/DestinyRanger/Art/Common/shadow_default.png",
            "Assets/DestinyRanger/Editor/FateWeaverAssetPostprocessor.cs",
        ),
    ))

    requirements.append(req(
        "Stage 2",
        "Aileen, Grick, and Luna animation frame sets and shadow frames exist.",
        count("Assets/DestinyRanger/Art/Generated/FateWeaverFull/Characters/*/*_*.png") >= 48
        and count("Assets/DestinyRanger/Art/Generated/FateWeaverFull/Characters/*/Shadows/*_shadow.png") == 48,
        ("Assets/DestinyRanger/Art/Generated/FateWeaverFull/Characters/",),
    ))
    requirements.append(req(
        "Stage 2",
        "Five forest monster frame sets and shadows exist.",
        count("Assets/DestinyRanger/Art/Generated/FateWeaverFull/Monsters/Forest/*/*_*.png") >= 46
        and count("Assets/DestinyRanger/Art/Generated/FateWeaverFull/Monsters/Forest/*/Shadows/*_shadow.png") == 46,
        ("Assets/DestinyRanger/Art/Generated/FateWeaverFull/Monsters/Forest/",),
    ))
    requirements.append(req(
        "Stage 2",
        "Offline AnimationClip assets exist for generated character and monster frames.",
        count("Assets/DestinyRanger/Animations/FateWeaver/**/*.anim") == 31,
        ("Assets/DestinyRanger/Animations/FateWeaver/",),
        unity_blocked=True,
    ))

    requirements.append(req(
        "Stage 3",
        "Chamber and forest slot machine layers exist.",
        count("Assets/DestinyRanger/Art/Generated/FateWeaverFull/SlotMachine/*_800x900.png") == 10,
        ("Assets/DestinyRanger/Art/Generated/FateWeaverFull/SlotMachine/",),
    ))
    requirements.append(req(
        "Stage 3",
        "Twenty-four scene symbol PNGs, six disabled symbols, and six highlight symbols exist.",
        count("Assets/DestinyRanger/Art/Generated/FateWeaverFull/Symbols/chamber/*.png") == 6
        and count("Assets/DestinyRanger/Art/Generated/FateWeaverFull/Symbols/forest/*.png") == 6
        and count("Assets/DestinyRanger/Art/Generated/FateWeaverFull/Symbols/volcano/*.png") == 6
        and count("Assets/DestinyRanger/Art/Generated/FateWeaverFull/Symbols/void/*.png") == 6
        and count("Assets/DestinyRanger/Art/Generated/FateWeaverFull/Symbols/disabled/*.png") == 6
        and count("Assets/DestinyRanger/Art/Generated/FateWeaverFull/Symbols/highlight/*.png") == 6,
        ("Assets/DestinyRanger/Art/Generated/FateWeaverFull/Symbols/",),
    ))
    requirements.append(req(
        "Stage 3",
        "SceneSymbolSet assets exist for scene-specific symbol binding.",
        count("Assets/DestinyRanger/Data/Symbols/*SymbolSet.asset") == 4,
        tuple(str(path.relative_to(PROJECT)) for path in sorted((DATA / "Symbols").glob("*.asset"))),
        unity_blocked=True,
    ))

    requirements.append(req(
        "Stage 4",
        "UI icons, panels, buttons, bars, and title art exist.",
        count("Assets/DestinyRanger/Art/Generated/FateWeaverFull/UI/**/*.png") == 33,
        ("Assets/DestinyRanger/Art/Generated/FateWeaverFull/UI/",),
    ))
    requirements.append(req(
        "Stage 4",
        "TextMeshPro package declaration and font source files exist.",
        exists("Packages/manifest.json")
        and exists("Assets/Fonts/STHeiti Medium.ttc")
        and exists("Assets/Fonts/Songti.ttc")
        and exists("Assets/DestinyRanger/Docs/FONT_RENDERING_PLAN.md"),
        evidence_exists(
            "Packages/manifest.json",
            "Assets/Fonts/STHeiti Medium.ttc",
            "Assets/Fonts/Songti.ttc",
            "Assets/DestinyRanger/Docs/FONT_RENDERING_PLAN.md",
        ),
        unity_blocked=True,
    ))

    requirements.append(req(
        "Stage 5",
        "Furniture PNGs, shadows, and FurnitureItem data assets exist.",
        count("Assets/DestinyRanger/Art/Generated/FateWeaverFull/Furniture/**/*.png") == 50
        and count("Assets/DestinyRanger/Data/Furniture/*.asset") == 25,
        ("Assets/DestinyRanger/Art/Generated/FateWeaverFull/Furniture/", "Assets/DestinyRanger/Data/Furniture/"),
    ))

    requirements.append(req(
        "Stage 6",
        "Audio files and AudioEventCatalog exist.",
        count("Assets/Audio/**/*.wav") == 25 and exists("Assets/DestinyRanger/Data/AudioEventCatalog.asset"),
        ("Assets/Audio/", "Assets/DestinyRanger/Data/AudioEventCatalog.asset"),
        unity_blocked=True,
    ))

    requirements.append(req(
        "Stage 7",
        "Integration builder, deferred runner, full delivery validator, static audit, handoff doc, and build script exist.",
        all(exists(path) for path in (
            "Assets/DestinyRanger/Editor/FateWeaverPrefabBuilder.cs",
            "Assets/DestinyRanger/Editor/FateWeaverDeferredIntegrationRunner.cs",
            "Assets/DestinyRanger/Editor/FateWeaverDeliveryValidator.cs",
            "Tools/fate_weaver_static_audit.py",
            "BuildScripts/fate_weaver_build_integrated_assets.sh",
            "Assets/DestinyRanger/Docs/UNITY_INTEGRATION_HANDOFF.md",
            "Assets/DestinyRanger/Docs/SELF_CHECK_REPORT.md",
        )),
        evidence_exists(
            "Assets/DestinyRanger/Editor/FateWeaverPrefabBuilder.cs",
            "Assets/DestinyRanger/Editor/FateWeaverDeferredIntegrationRunner.cs",
            "Assets/DestinyRanger/Editor/FateWeaverDeliveryValidator.cs",
            "Tools/fate_weaver_static_audit.py",
            "BuildScripts/fate_weaver_build_integrated_assets.sh",
            "Assets/DestinyRanger/Docs/UNITY_INTEGRATION_HANDOFF.md",
            "Assets/DestinyRanger/Docs/SELF_CHECK_REPORT.md",
        ),
        unity_blocked=True,
    ))
    requirements.append(req(
        "Stage 7",
        "Integration prefabs exist for slot machines, UI canvas, audio hub, and furniture.",
        count("Assets/DestinyRanger/Prefabs/FateWeaver/**/*.prefab") == 29,
        ("Assets/DestinyRanger/Prefabs/FateWeaver/",),
        unity_blocked=True,
    ))
    requirements.append(req(
        "Stage 7",
        "Chamber and forest integrated Unity scenes exist.",
        exists("Assets/Scenes/FateWeaver_ChamberIntegrated.unity")
        and exists("Assets/Scenes/FateWeaver_ForestIntegrated.unity"),
        evidence_exists(
            "Assets/Scenes/FateWeaver_ChamberIntegrated.unity",
            "Assets/Scenes/FateWeaver_ForestIntegrated.unity",
        ),
    ))

    return requirements


def write_markdown(requirements: list[Requirement], path: Path) -> None:
    counts: dict[str, int] = {}
    for item in requirements:
        counts[item.status] = counts.get(item.status, 0) + 1

    lines = [
        "# Fate Weaver Requirement Audit",
        "",
        "Generated from `Tools/fate_weaver_requirement_audit.py`.",
        "",
        "This audit maps the staged delivery requirements to current project evidence. It does not replace Unity Editor import, preview, or PlayMode verification.",
        "",
        "## Summary",
        "",
    ]

    for status in ("complete_file_level", "blocked_unity_verification", "missing_or_incomplete"):
        lines.append(f"- {STATUS_LABELS[status]}: {counts.get(status, 0)}")

    lines.extend([
        "",
        "## Items",
        "",
        "| Stage | Status | Requirement | Evidence |",
        "| --- | --- | --- | --- |",
    ])

    for item in requirements:
        evidence = "<br>".join(f"`{entry}`" for entry in item.evidence) if item.evidence else ""
        lines.append(f"| {item.stage} | {STATUS_LABELS[item.status]} | {item.item} | {evidence} |")

    lines.extend([
        "",
        "## Unity-Blocked Verification",
        "",
        "The `Blocked Unity Verification` items have file-level evidence but still require Unity Editor import, prefab generation, visual preview, AudioImporter preview, TMP FontAsset generation, or PlayMode checks. Current local Unity batch mode is blocked before project import by Licensing IPC. Use `Assets/DestinyRanger/Docs/UNITY_INTEGRATION_HANDOFF.md` and the Unity menu `Destiny Ranger/Fate Weaver/Run Deferred Integration Now` once the target project is open in the editor.",
        "",
    ])

    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text("\n".join(lines), encoding="utf-8")


def main() -> int:
    parser = ArgumentParser()
    parser.add_argument("--write-md", type=Path, help="Write a Markdown requirement audit report.")
    args = parser.parse_args()

    requirements = build_requirements()
    failures = [item for item in requirements if item.status == "missing_or_incomplete"]
    if args.write_md:
        write_markdown(requirements, args.write_md)
    print(json.dumps([item.__dict__ for item in requirements], ensure_ascii=False, indent=2))
    if failures:
        return 1
    return 0


if __name__ == "__main__":
    sys.exit(main())
