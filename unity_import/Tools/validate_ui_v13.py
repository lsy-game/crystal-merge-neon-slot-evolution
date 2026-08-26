#!/usr/bin/env python3
"""Repeatable checks for Wheat Town's text-free generated UI sprites."""

from __future__ import annotations

import argparse
from collections import Counter
from pathlib import Path

from PIL import Image


def flat_data(image: Image.Image):
    getter = getattr(image, "get_flattened_data", None)
    return getter() if getter is not None else image.getdata()


def palette(image: Image.Image, count: int = 5) -> list[str]:
    rgba = image.convert("RGBA")
    pixels = [rgb for *rgb, alpha in flat_data(rgba) if alpha >= 220]
    reduced = Image.new("RGB", (len(pixels), 1))
    reduced.putdata([tuple(p) for p in pixels])
    quantized = reduced.quantize(colors=count, method=Image.Quantize.MEDIANCUT).convert("RGB")
    colors = Counter(flat_data(quantized)).most_common(count)
    return ["#{:02X}{:02X}{:02X}".format(*rgb) for rgb, _ in colors]


def validate(path: Path) -> dict[str, object]:
    image = Image.open(path).convert("RGBA")
    width, height = image.size
    alpha = image.getchannel("A")
    values = list(flat_data(alpha))
    border = []
    border.extend(flat_data(alpha.crop((0, 0, width, 1))))
    border.extend(flat_data(alpha.crop((0, height - 1, width, height))))
    border.extend(flat_data(alpha.crop((0, 0, 1, height))))
    border.extend(flat_data(alpha.crop((width - 1, 0, width, height))))
    soft = sum(1 for value in values if 0 < value < 255)
    visible = sum(1 for value in values if value > 0) / len(values)

    errors = []
    if image.mode != "RGBA":
        errors.append("not RGBA")
    if max(border, default=0) != 0:
        errors.append("outer border is not fully transparent")
    if soft < 500:
        errors.append("insufficient anti-aliased alpha edge")
    if visible >= 0.96:
        errors.append("nearly full rectangular coverage; possible pasted background")
    if min(width, height) < 560:
        errors.append("source resolution is too small")

    return {
        "file": path.name,
        "size": f"{width}x{height}",
        "mode": image.mode,
        "border_alpha_max": max(border, default=0),
        "soft_edge_pixels": soft,
        "visible_coverage": round(visible, 4),
        "dominant_palette": palette(image),
        "errors": errors,
    }


def contrast_ratio(first: str, second: str) -> float:
    def luminance(value: str) -> float:
        channels = [int(value[index:index + 2], 16) / 255 for index in (1, 3, 5)]
        channels = [channel / 12.92 if channel <= 0.04045 else ((channel + 0.055) / 1.055) ** 2.4 for channel in channels]
        return 0.2126 * channels[0] + 0.7152 * channels[1] + 0.0722 * channels[2]

    a, b = sorted((luminance(first), luminance(second)), reverse=True)
    return (a + 0.05) / (b + 0.05)


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("folder", type=Path)
    parser.add_argument("--background", type=Path)
    args = parser.parse_args()

    if args.background:
        background = Image.open(args.background).convert("RGB")
        print({"background": args.background.name, "dominant_palette": palette(background)})

    reports = [validate(args.folder / name) for name in ("login_frame.png", "task_panel.png", "seed_sheet.png")]
    failed = False
    for report in reports:
        print(report)
        failed |= bool(report["errors"])

    pairs = {
        "warm_white_on_forest": ("#FFF8DE", "#2E4A2B"),
        "dark_green_on_ivory": ("#233426", "#FFF6DB"),
        "dark_green_on_orange": ("#18261B", "#E88D2F"),
    }
    for name, (foreground, background) in pairs.items():
        ratio = contrast_ratio(foreground, background)
        print({"contrast": name, "ratio": round(ratio, 2), "pass_AA": ratio >= 4.5})
        failed |= ratio < 4.5

    return 1 if failed else 0


if __name__ == "__main__":
    raise SystemExit(main())
