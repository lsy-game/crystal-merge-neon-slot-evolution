#!/usr/bin/env python3
"""Check generated terrain art invariants that caused previous visual regressions."""

from __future__ import annotations

import sys
from pathlib import Path

from PIL import Image


ROOT = Path(__file__).resolve().parents[1]
PLATFORM = ROOT / "Assets/DestinyRanger/Art/Generated/adventure-platform-sheet-v8-painted-solid.png"
GROUND_WALL = ROOT / "Assets/DestinyRanger/Art/Generated/adventure-ground-wall-v2-painted-solid.png"


def fail(message: str) -> None:
    print(f"generated-art-check failed: {message}", file=sys.stderr)
    raise SystemExit(1)


def require_rgba(path: Path, expected_size: tuple[int, int] | None = None) -> Image.Image:
    if not path.exists():
        fail(f"missing file: {path.relative_to(ROOT)}")
    image = Image.open(path).convert("RGBA")
    if expected_size and image.size != expected_size:
        fail(f"{path.name} size={image.size}, expected={expected_size}")
    alpha = image.getchannel("A")
    if not alpha.getbbox():
        fail(f"{path.name} has no visible alpha content")
    corners = [
        image.getpixel((0, 0))[3],
        image.getpixel((image.width - 1, 0))[3],
        image.getpixel((0, image.height - 1))[3],
        image.getpixel((image.width - 1, image.height - 1))[3],
    ]
    if any(value != 0 for value in corners):
        fail(f"{path.name} has non-transparent corners: {corners}")
    return image


def audit_platform_sheet(image: Image.Image) -> None:
    cell_w = image.width // 4
    if image.width % 4 != 0:
        fail("platform sheet width must divide into four cells")
    for index in range(4):
        cell = image.crop((index * cell_w, 0, (index + 1) * cell_w, image.height))
        bbox = cell.getchannel("A").getbbox()
        if not bbox:
            fail(f"platform cell {index} is empty")
        visible_w = bbox[2] - bbox[0]
        visible_h = bbox[3] - bbox[1]
        if visible_w < cell_w * 0.30 or visible_h < image.height * 0.36:
            fail(f"platform cell {index} too thin: visible={visible_w}x{visible_h}")


def audit_ground_wall(image: Image.Image) -> None:
    alpha_bbox = image.getchannel("A").getbbox()
    if not alpha_bbox:
        fail("ground wall is empty")
    visible_h = alpha_bbox[3] - alpha_bbox[1]
    if visible_h < image.height * 0.70:
        fail(f"ground wall not tall enough for full foot coverage: visible_h={visible_h}")
    # Catch a repeated bright grass row in the lower half, which reads like a dirty reflection band.
    lower = image.crop((0, image.height // 2, image.width, image.height)).convert("RGBA")
    bright_green = 0
    opaque = 0
    data = lower.tobytes()
    for index in range(0, len(data), 4):
        r, g, b, a = data[index], data[index + 1], data[index + 2], data[index + 3]
        if a < 40:
            continue
        opaque += 1
        if g > 125 and g > r * 1.18 and g > b * 1.25:
            bright_green += 1
    if opaque and bright_green / opaque > 0.18:
        fail(f"ground wall lower half has too much repeated green banding: {bright_green / opaque:.2%}")


def main() -> int:
    platform = require_rgba(PLATFORM, (2560, 320))
    ground = require_rgba(GROUND_WALL, (1536, 500))
    audit_platform_sheet(platform)
    audit_ground_wall(ground)
    print("Destiny Ranger generated art QA passed.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
