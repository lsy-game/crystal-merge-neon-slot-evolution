#!/usr/bin/env python3
"""Prepare OpenGameArt terrain candidates for Destiny Ranger review.

This script intentionally writes to the candidate folder, not to the formal
Generated art folder. Formal runtime import should happen only after visual QA
and provenance registration.
"""

from __future__ import annotations

import argparse
from pathlib import Path

from PIL import Image, ImageChops, ImageEnhance, ImageFilter, ImageOps


PROJECT = Path(__file__).resolve().parents[1]
CANDIDATES = PROJECT / "Assets/DestinyRanger/Art/Candidates/OpenGameArt"
OUT = CANDIDATES / "Prepared"


def load_rgba(path: Path) -> Image.Image:
    image = Image.open(path).convert("RGBA")
    if image.width < 16 or image.height < 16:
        raise ValueError(f"candidate image too small: {path}")
    return image


def trim_alpha(image: Image.Image) -> Image.Image:
    alpha = image.getchannel("A")
    bbox = alpha.getbbox()
    if not bbox:
        return image
    return image.crop(bbox)


def feather_alpha(image: Image.Image, pixels: int = 2) -> Image.Image:
    image = image.convert("RGBA")
    alpha = image.getchannel("A")
    eroded = alpha.filter(ImageFilter.MinFilter(pixels * 2 + 1))
    edge = ImageChops.subtract(alpha, eroded)
    softened = ImageChops.subtract(alpha, ImageEnhance.Brightness(edge).enhance(0.35))
    image.putalpha(softened)
    return image


def fit_cell(image: Image.Image, size: tuple[int, int]) -> Image.Image:
    image = trim_alpha(image)
    image = feather_alpha(image)
    image = ImageOps.contain(image, (size[0] - 24, size[1] - 24), Image.Resampling.LANCZOS)
    canvas = Image.new("RGBA", size, (0, 0, 0, 0))
    canvas.alpha_composite(image, ((size[0] - image.width) // 2, (size[1] - image.height) // 2))
    return canvas


def environment_tint(image: Image.Image) -> Image.Image:
    """Nudge candidate terrain toward current forest-ruins palette."""
    image = image.convert("RGBA")
    tinted = Image.new("RGBA", image.size, (58, 112, 86, 0))
    tinted.putalpha(ImageEnhance.Brightness(image.getchannel("A")).enhance(0.10))
    merged = Image.alpha_composite(image, tinted)
    merged = ImageEnhance.Color(merged).enhance(0.92)
    merged = ImageEnhance.Contrast(merged).enhance(1.05)
    return merged


def make_sheet(inputs: list[Path], out_path: Path, cell: tuple[int, int] = (512, 320)) -> None:
    if not inputs:
        raise ValueError("no candidate images passed")
    cols = min(4, len(inputs))
    sheet = Image.new("RGBA", (cell[0] * cols, cell[1]), (0, 0, 0, 0))
    for index, path in enumerate(inputs[:cols]):
        prepared = environment_tint(fit_cell(load_rgba(path), cell))
        sheet.alpha_composite(prepared, (cell[0] * index, 0))
    out_path.parent.mkdir(parents=True, exist_ok=True)
    sheet.save(out_path, optimize=True)


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("images", nargs="+", type=Path, help="CC0 terrain candidate PNG files")
    parser.add_argument(
        "--out",
        type=Path,
        default=OUT / "opengameart-platform-candidate-sheet.png",
        help="candidate output sheet path",
    )
    args = parser.parse_args()
    paths = [p if p.is_absolute() else PROJECT / p for p in args.images]
    make_sheet(paths, args.out if args.out.is_absolute() else PROJECT / args.out)
    print(args.out)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
