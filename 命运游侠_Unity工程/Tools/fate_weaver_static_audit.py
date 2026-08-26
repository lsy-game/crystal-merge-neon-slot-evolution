#!/usr/bin/env python3
from __future__ import annotations

import json
import re
import sys
import wave
from pathlib import Path

from PIL import Image


PROJECT = Path(__file__).resolve().parents[1]
ART = PROJECT / "Assets/DestinyRanger/Art/Generated/FateWeaverFull"
DATA = PROJECT / "Assets/DestinyRanger/Data"
SCENES = PROJECT / "Assets/Scenes"


errors: list[str] = []


def rel(path: Path) -> str:
    return str(path.relative_to(PROJECT))


def require_file(path: Path) -> None:
    if not path.exists():
        errors.append(f"missing file: {rel(path)}")


def require_image(path: Path, size: tuple[int, int] | None = None) -> None:
    require_file(path)
    if not path.exists():
        return
    try:
        with Image.open(path) as image:
            if image.mode != "RGBA":
                errors.append(f"{rel(path)} expected RGBA, got {image.mode}")
            if size and image.size != size:
                errors.append(f"{rel(path)} expected {size[0]}x{size[1]}, got {image.size[0]}x{image.size[1]}")
    except Exception as exc:
        errors.append(f"{rel(path)} could not be opened as image: {exc}")


def require_sprite_meta(path: Path) -> None:
    meta = path.with_suffix(path.suffix + ".meta")
    require_file(meta)
    if not meta.exists():
        return

    text = meta.read_text(encoding="utf-8")
    if "textureType: 8" not in text:
        errors.append(f"{rel(meta)} expected textureType Sprite")
    if "alphaIsTransparency: 1" not in text:
        errors.append(f"{rel(meta)} expected alphaIsTransparency: 1")
    if "enableMipMap: 0" not in text:
        errors.append(f"{rel(meta)} expected enableMipMap: 0")


def require_sprite_border(path: Path, border: int) -> None:
    meta = path.with_suffix(path.suffix + ".meta")
    require_file(meta)
    if not meta.exists():
        return

    text = meta.read_text(encoding="utf-8")
    pattern = rf"spriteBorder: \{{x: {border}, y: {border}, z: {border}, w: {border}\}}"
    if not re.search(pattern, text):
        errors.append(f"{rel(meta)} expected spriteBorder {border}")


def require_clean_transparent_edge(path: Path) -> None:
    require_file(path)
    if not path.exists():
        return

    with Image.open(path) as image:
        rgba = image.convert("RGBA")
        alpha_min, alpha_max = rgba.getextrema()[3]
        if alpha_min >= 250:
            errors.append(f"{rel(path)} has no transparent alpha pixels")

        pixels = rgba.load()
        width, height = rgba.size
        coords: list[tuple[int, int]] = []
        for x in range(width):
            coords.append((x, 0))
            coords.append((x, height - 1))
        for y in range(1, height - 1):
            coords.append((0, y))
            coords.append((width - 1, y))

        opaque = 0
        bad = 0
        for x, y in coords:
            r, g, b, a = pixels[x, y]
            if a < 250:
                continue

            opaque += 1
            pure_black = r < 8 and g < 8 and b < 8
            pure_white = r > 247 and g > 247 and b > 247
            if pure_black or pure_white:
                bad += 1

        if coords and opaque / len(coords) > 0.95:
            errors.append(f"{rel(path)} has more than 95% opaque edge pixels")
        if opaque and bad / opaque > 0.9:
            errors.append(f"{rel(path)} edge is over 90% pure black/white")


def require_count(folder: Path, pattern: str, count: int) -> list[Path]:
    files = sorted(folder.glob(pattern))
    if len(files) != count:
        errors.append(f"{rel(folder)}/{pattern} expected {count}, got {len(files)}")
    return files


def require_recursive_count(folder: Path, pattern: str, count: int) -> list[Path]:
    files = sorted(folder.rglob(pattern))
    if len(files) != count:
        errors.append(f"{rel(folder)}/**/{pattern} expected {count}, got {len(files)}")
    return files


def require_images(folder: Path, pattern: str, count: int, size: tuple[int, int] | None = None) -> None:
    for path in require_count(folder, pattern, count):
        require_image(path, size)


def require_text_contains(path: Path, needle: str, message: str) -> None:
    require_file(path)
    if path.exists() and needle not in path.read_text(encoding="utf-8"):
        errors.append(message)


def require_no_literal_newline(paths: list[Path]) -> None:
    for path in paths:
        if path.exists() and "\\n" in path.read_text(encoding="utf-8"):
            errors.append(f"{rel(path)} contains literal backslash-n")


def audit_scene_foundation() -> None:
    require_image(ART / "Backgrounds/fate-weaver-chamber-bg_1290x2796.png", (1290, 2796))
    require_image(ART / "Backgrounds/fate-weaver-battle-forest-bg_1290x1398.png", (1290, 1398))
    require_image(ART / "Backgrounds/fate-weaver-battle-volcano-bg_1290x1398.png", (1290, 1398))
    require_image(ART / "Backgrounds/fate-weaver-battle-void-boss-bg_1290x1398.png", (1290, 1398))
    require_image(PROJECT / "Assets/DestinyRanger/Art/Common/shadow_default.png", (256, 256))
    for profile in ("Chamber", "Forest", "Volcano", "Void"):
        require_file(DATA / f"SceneToneProfiles/{profile}ToneProfile.asset")
    require_file(PROJECT / "Assets/DestinyRanger/Scripts/SceneColorTint.cs")
    require_file(PROJECT / "Assets/DestinyRanger/Editor/FateWeaverAssetPostprocessor.cs")

    for path in ART.rglob("*.png"):
        require_sprite_meta(path)
    require_sprite_meta(PROJECT / "Assets/DestinyRanger/Art/Common/shadow_default.png")

    for path in ART.rglob("*.png"):
        if "Backgrounds" not in path.parts:
            require_clean_transparent_edge(path)


def audit_characters() -> None:
    for character in ("aileen", "grick", "luna"):
        root = ART / f"Characters/{character}"
        require_images(root, f"{character}_idle_*_512x768.png", 4, (512, 768))
        require_images(root, f"{character}_attack_*_512x768.png", 4, (512, 768))
        require_images(root, f"{character}_hit_*_512x768.png", 2, (512, 768))
        require_images(root, f"{character}_skill_*_512x768.png", 3, (512, 768))
        require_images(root, f"{character}_death_*_512x768.png", 3, (512, 768))
        require_images(root / "Shadows", f"{character}_*_shadow.png", 16)


def audit_monsters() -> None:
    specs = {
        "shadow_small": (256, 2, 2, 0, 2),
        "treant": (384, 2, 3, 0, 3),
        "toxic_moth": (384, 2, 2, 0, 2),
        "gargoyle": (512, 2, 4, 0, 3),
        "void_weaver_boss": (768, 4, 6, 2, 5),
    }
    for name, (size, idle, attack, hit, death) in specs.items():
        root = ART / f"Monsters/Forest/{name}"
        require_images(root, f"{name}_idle_*_{size}x{size}.png", idle, (size, size))
        require_images(root, f"{name}_attack_*_{size}x{size}.png", attack, (size, size))
        if hit:
            require_images(root, f"{name}_hit_*_{size}x{size}.png", hit, (size, size))
        require_images(root, f"{name}_death_*_{size}x{size}.png", death, (size, size))
        require_images(root / "Shadows", f"{name}_*_shadow.png", idle + attack + hit + death)


def audit_symbols_and_slot_machine() -> None:
    for scene in ("chamber", "forest"):
        for layer in ("body", "frame", "reels", "slot_base", "crystal_glow"):
            require_image(ART / f"SlotMachine/{scene}_slot_machine_{layer}_800x900.png", (800, 900))
    for scene in ("chamber", "forest", "volcano", "void"):
        for symbol in ("sword", "staff", "heart", "shield", "skull", "star"):
            require_image(ART / f"Symbols/{scene}/symbol_{symbol}_{scene}.png", (180, 180))
        require_file(DATA / f"Symbols/{scene.capitalize()}SymbolSet.asset")
    for symbol in ("sword", "staff", "heart", "shield", "skull", "star"):
        require_image(ART / f"Symbols/disabled/symbol_{symbol}_disabled.png", (180, 180))
        require_image(ART / f"Symbols/highlight/symbol_{symbol}_highlight.png", (180, 180))


def audit_ui() -> None:
    for icon in ("adventure", "hero", "home", "quest", "weave", "workshop"):
        require_image(ART / f"UI/BottomMenu/icon_bottom_{icon}_120x120.png", (120, 120))
    for icon in ("coin_gear", "diamond_prism", "fate_thread"):
        require_image(ART / f"UI/Currency/icon_currency_{icon}_64x64.png", (64, 64))
    for icon in ("altar", "battle", "boss", "event", "shop"):
        require_image(ART / f"UI/MapNodes/icon_map_{icon}_120x120.png", (120, 120))
    for panel in ("panel_chamber_parchment_9slice_900x600_border96.png", "panel_forest_frosted_metal_9slice_900x600_border96.png", "panel_common_dark_translucent_9slice_900x600_border96.png"):
        panel_path = ART / f"UI/Panels/{panel}"
        require_image(panel_path, (900, 600))
        require_sprite_border(panel_path, 96)
    for state in ("normal", "hover", "pressed", "disabled"):
        require_image(ART / f"UI/Buttons/primary_button_{state}_280x100.png", (280, 100))
    for state in ("normal", "highlight", "pressed"):
        require_image(ART / f"UI/Buttons/stop_button_{state}_200x200.png", (200, 200))
    require_image(ART / "UI/Buttons/close_button_normal_40x40.png", (40, 40))
    require_image(ART / "UI/Buttons/close_button_pressed_40x40.png", (40, 40))
    for bar_path in require_count(ART / "UI/Bars", "*_9slice.png", 6):
        require_sprite_border(bar_path, 8)
    titles = require_count(ART / "UI", "title_fate_weaver_*_4x_supersampled_400x100.png", 1)
    for title in titles:
        require_image(title, (400, 100))
    require_file(PROJECT / "Assets/Fonts/STHeiti Medium.ttc")
    require_file(PROJECT / "Assets/Fonts/Songti.ttc")
    require_text_contains(PROJECT / "Packages/manifest.json", "com.unity.textmeshpro", "TextMeshPro package missing from manifest")


def audit_furniture() -> None:
    specs = {
        "window": (("window_star_night", "window_aurora", "window_abyss_rift", "window_forest_morning", "window_japanese_paper"), (350, 550)),
        "bookcase": (("bookcase_oak", "bookcase_arcane", "bookcase_crystal"), (200, 400)),
        "tapestry": (("tapestry_fate", "tapestry_hero", "tapestry_star"), (400, 300)),
        "rug": (("rug_warm", "rug_magic_circle", "rug_gold_thread"), (700, 500)),
        "decor": (("decor_candle", "decor_crystal_ball", "decor_hourglass", "decor_pet_cat", "decor_weaver_bird"), (120, 120)),
        "display": (("display_trophy_case",), (300, 400)),
        "boss_badge": (("boss_badge_1", "boss_badge_2", "boss_badge_3", "boss_badge_4", "boss_badge_5"), (64, 64)),
    }
    expected_assets = 0
    for folder, (names, size) in specs.items():
        for name in names:
            expected_assets += 1
            require_image(ART / f"Furniture/{folder}/{name}.png", size)
            require_image(ART / f"Furniture/{folder}/{name}_shadow.png")
            require_file(DATA / f"Furniture/{name}.asset")
    if len(list((DATA / "Furniture").glob("*.asset"))) != expected_assets:
        errors.append(f"Furniture data expected {expected_assets} assets")


def audit_audio() -> None:
    specs = {
        PROJECT / "Assets/Audio/SFX/SlotMachine": 6,
        PROJECT / "Assets/Audio/SFX/Combat": 11,
        PROJECT / "Assets/Audio/SFX/UI": 6,
        PROJECT / "Assets/Audio/Ambient": 2,
    }
    for folder, count in specs.items():
        for wav_path in require_count(folder, "*.wav", count):
            with wave.open(str(wav_path), "rb") as wav:
                if wav.getframerate() != 44100:
                    errors.append(f"{rel(wav_path)} expected 44100 Hz")
                if wav.getnchannels() not in (1, 2):
                    errors.append(f"{rel(wav_path)} expected mono or stereo")
    require_file(DATA / "AudioEventCatalog.asset")


def audit_integration() -> None:
    require_recursive_count(PROJECT / "Assets/DestinyRanger/Animations/FateWeaver", "*.anim", 31)
    require_recursive_count(PROJECT / "Assets/DestinyRanger/Prefabs/FateWeaver", "*.prefab", 29)
    require_no_literal_newline(list((PROJECT / "Assets/DestinyRanger/Animations/FateWeaver").rglob("*.anim")))
    require_no_literal_newline(list((DATA / "Symbols").glob("*.asset")))
    require_file(PROJECT / "Assets/DestinyRanger/Scripts/FateWeaverAudioHub.cs")
    require_file(PROJECT / "Assets/DestinyRanger/Editor/FateWeaverDeliveryValidator.cs")
    require_file(PROJECT / "Assets/DestinyRanger/Editor/FateWeaverPrefabBuilder.cs")
    require_file(PROJECT / "Assets/DestinyRanger/Editor/FateWeaverDeferredIntegrationRunner.cs")
    require_file(PROJECT / "BuildScripts/fate_weaver_build_integrated_assets.sh")
    require_file(PROJECT / "Tools/fate_weaver_requirement_audit.py")
    require_file(PROJECT / "Assets/DestinyRanger/Docs/REQUIREMENT_AUDIT.md")
    require_file(PROJECT / "Assets/DestinyRanger/Docs/UNITY_INTEGRATION_HANDOFF.md")
    require_text_contains(PROJECT / "Assets/DestinyRanger/Editor/FateWeaverDeliveryValidator.cs", "RequireRecursiveSequence(errors, \"Assets/DestinyRanger/Animations/FateWeaver\"", "Delivery validator must recursively check animation clips")
    require_text_contains(PROJECT / "Assets/DestinyRanger/Editor/FateWeaverDeliveryValidator.cs", "ValidateBuiltIntegrationBatch", "Delivery validator missing built integration batch method")
    require_text_contains(PROJECT / "Assets/DestinyRanger/Editor/FateWeaverDeliveryValidator.cs", "FateWeaver_ChamberIntegrated.unity", "Delivery validator missing chamber integrated scene check")
    require_text_contains(PROJECT / "Assets/DestinyRanger/Editor/FateWeaverDeliveryValidator.cs", "FateWeaverAudioHub.prefab", "Delivery validator missing audio hub prefab check")
    require_text_contains(PROJECT / "Assets/DestinyRanger/Editor/FateWeaverPrefabBuilder.cs", "BuildIntegrationPrefabsBatch", "Prefab builder batch method missing")
    require_text_contains(PROJECT / "Assets/DestinyRanger/Editor/FateWeaverPrefabBuilder.cs", "BuildIntegrationPrefabsAndScenesBatch", "Prefab and scene builder batch method missing")
    require_text_contains(PROJECT / "Assets/DestinyRanger/Editor/FateWeaverDeferredIntegrationRunner.cs", "Run Deferred Integration Now", "Deferred runner manual menu item missing")
    require_text_contains(PROJECT / "Assets/DestinyRanger/Editor/FateWeaverDeferredIntegrationRunner.cs", "FateWeaverRunIntegrationBuild.request", "Deferred runner request marker missing")
    require_text_contains(PROJECT / "Assets/DestinyRanger/Editor/FateWeaverDeferredIntegrationRunner.cs", "EditorApplication.update", "Deferred runner must poll for request marker")
    require_text_contains(PROJECT / "Assets/DestinyRanger/Editor/FateWeaverDeferredIntegrationRunner.cs", "EditorApplication.isCompiling", "Deferred runner must wait for editor compilation")
    require_text_contains(PROJECT / "Assets/DestinyRanger/Editor/FateWeaverDeferredIntegrationRunner.cs", "EditorApplication.isPlaying", "Deferred runner must wait for edit mode")
    require_text_contains(PROJECT / "Assets/DestinyRanger/Editor/FateWeaverDeferredIntegrationRunner.cs", "EditorApplication.isPlayingOrWillChangePlaymode", "Deferred runner must avoid play mode transitions")
    require_text_contains(PROJECT / "Assets/DestinyRanger/Docs/UNITY_INTEGRATION_HANDOFF.md", "Run Deferred Integration Now", "Unity integration handoff missing manual runner instructions")
    require_text_contains(PROJECT / "Assets/DestinyRanger/Docs/UNITY_INTEGRATION_HANDOFF.md", "Leave Play Mode", "Unity integration handoff missing play mode warning")
    require_text_contains(PROJECT / "BuildScripts/fate_weaver_build_integrated_assets.sh", "BuildIntegrationPrefabsAndScenesBatch", "Build script missing prefab/scene batch call")
    require_text_contains(PROJECT / "BuildScripts/fate_weaver_build_integrated_assets.sh", "ValidateBuiltIntegrationBatch", "Build script missing built integration validation batch call")
    require_text_contains(PROJECT / "BuildScripts/fate_weaver_build_integrated_assets.sh", "ValidateFullDeliveryBatch", "Build script missing validation batch call")
    require_text_contains(PROJECT / "BuildScripts/fate_weaver_build_integrated_assets.sh", "fate_weaver_requirement_audit.py", "Build script missing requirement audit call")
    require_text_contains(PROJECT / "BuildScripts/fate_weaver_build_integrated_assets.sh", "--write-md", "Build script must refresh requirement audit markdown")
    require_text_contains(PROJECT / "Tools/fate_weaver_requirement_audit.py", "blocked_unity_verification", "Requirement audit must preserve Unity-blocked status")
    require_text_contains(PROJECT / "Assets/DestinyRanger/Docs/REQUIREMENT_AUDIT.md", "Blocked Unity Verification", "Requirement audit markdown missing blocked status")
    require_text_contains(PROJECT / "Assets/DestinyRanger/Editor/FateWeaverPrefabBuilder.cs", 'BuildIntegratedScene("Chamber"', "Chamber integrated scene builder call missing")
    require_text_contains(PROJECT / "Assets/DestinyRanger/Editor/FateWeaverPrefabBuilder.cs", 'BuildIntegratedScene("Forest"', "Forest integrated scene builder call missing")
    require_text_contains(PROJECT / "Assets/DestinyRanger/Editor/FateWeaverPrefabBuilder.cs", "FateWeaver_{displayName}Integrated.unity", "Integrated scene output template missing")
    for scene in ("Chamber", "Forest", "Volcano", "Void"):
        require_text_contains(SCENES / f"FateWeaver_{scene}Tone.unity", "m_Script: {fileID: 11500000, guid: 0fa1f63170a24e449bf0f2f5196d9412, type: 3}", f"SceneColorTint missing in {scene} tone scene")
    for scene in ("Chamber", "Forest"):
        scene_path = SCENES / f"FateWeaver_{scene}Integrated.unity"
        require_file(scene_path)
        require_text_contains(scene_path, "m_Name: Background", f"Background missing in {scene} integrated scene")
        require_text_contains(scene_path, "SpriteRenderer:", f"Background SpriteRenderer missing in {scene} integrated scene")
        require_text_contains(scene_path, "SlotMachine Prefab Anchor", f"SlotMachine anchor missing in {scene} integrated scene")
        require_text_contains(scene_path, "Fate Weaver UI Canvas Prefab Anchor", f"UI anchor missing in {scene} integrated scene")
        require_text_contains(scene_path, "Fate Weaver Audio Hub Prefab Anchor", f"Audio anchor missing in {scene} integrated scene")
        require_text_contains(scene_path, "m_Script: {fileID: 11500000, guid: 0fa1f63170a24e449bf0f2f5196d9412, type: 3}", f"SceneColorTint missing in {scene} integrated scene")
    manifest = ART / "FULL_ASSET_MANIFEST.json"
    require_file(manifest)
    if manifest.exists():
        data = json.loads(manifest.read_text(encoding="utf-8"))
        if not data.get("no_validation_images_generated"):
            errors.append("manifest does not confirm no validation images")
    require_file(PROJECT / "Assets/DestinyRanger/Docs/SELF_CHECK_REPORT.md")


def main() -> int:
    audit_scene_foundation()
    audit_characters()
    audit_monsters()
    audit_symbols_and_slot_machine()
    audit_ui()
    audit_furniture()
    audit_audio()
    audit_integration()
    if errors:
        print("Fate Weaver static audit failed:")
        for error in errors:
            print(f"- {error}")
        return 1
    print("Fate Weaver static audit passed.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
