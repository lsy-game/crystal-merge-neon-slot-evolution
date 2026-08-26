#!/usr/bin/env python3
from __future__ import annotations

import json
from pathlib import Path

import numpy as np
from PIL import Image, ImageChops, ImageFilter

PROJECT = Path("/Users/zhendian/Documents/New project/命运游侠_Unity工程")
GENERATED = PROJECT / "Assets/DestinyRanger/Art/Generated"
OUT = GENERATED / "FateWeaverFusion"
PREVIEW = GENERATED / "FateWeaverFusionPreviews"
ATLAS = GENERATED / "fate-weaver-fusion-atlas-reference.png"
CHAMBER_BG = GENERATED / "fate-weaver-chamber-bg.png"
BATTLE_BG = GENERATED / "fate-weaver-battle-forest-bg.png"


def load_palette():
    report = json.loads((OUT / "palette_report.json").read_text(encoding="utf-8"))
    def parse(hex_color):
        hex_color = hex_color.strip("#")
        return tuple(int(hex_color[i:i + 2], 16) for i in (0, 2, 4))
    return [parse(v) for v in report["chamber_palette"]], [parse(v) for v in report["forest_palette"]]


def matte_to_alpha(crop: Image.Image, threshold: int = 34) -> Image.Image:
    crop = crop.convert("RGBA")
    rgb = np.array(crop.convert("RGB")).astype(np.int16)
    # The atlas uses a neutral gray studio matte. Estimate it from crop corners.
    samples = np.concatenate([
        rgb[:10, :10].reshape(-1, 3),
        rgb[:10, -10:].reshape(-1, 3),
        rgb[-10:, :10].reshape(-1, 3),
        rgb[-10:, -10:].reshape(-1, 3),
    ])
    matte = np.median(samples, axis=0)
    dist = np.sqrt(((rgb - matte) ** 2).sum(axis=2))
    alpha = np.clip((dist - threshold) * 6.2, 0, 255).astype(np.uint8)
    alpha = Image.fromarray(alpha, "L").filter(ImageFilter.GaussianBlur(0.7))
    result = crop.copy()
    result.putalpha(alpha)
    return trim(result)


def trim(image: Image.Image) -> Image.Image:
    alpha = image.getchannel("A")
    bbox = alpha.getbbox()
    if not bbox:
        return image
    pad = 8
    box = (
        max(0, bbox[0] - pad),
        max(0, bbox[1] - pad),
        min(image.width, bbox[2] + pad),
        min(image.height, bbox[3] + pad),
    )
    return image.crop(box)


def feather(image: Image.Image, pixels: int = 2, strength: float = .42) -> Image.Image:
    alpha = image.getchannel("A")
    eroded = alpha.filter(ImageFilter.MinFilter(pixels * 2 + 1))
    edge = Image.eval(ImageChops.subtract(alpha, eroded), lambda v: int(v * strength))
    image = image.copy()
    image.putalpha(ImageChops.subtract(alpha, edge))
    return image


def tint(image: Image.Image, env, opacity: float, saturation: float = 1.0, light=None) -> Image.Image:
    arr = np.array(image.convert("RGBA")).astype(np.float32)
    a = arr[:, :, 3:4] / 255.0
    rgb = arr[:, :, :3]
    gray = (rgb[:, :, 0:1] * .2126 + rgb[:, :, 1:2] * .7152 + rgb[:, :, 2:3] * .0722)
    rgb = gray + (rgb - gray) * saturation
    rgb = rgb * (1 - opacity * a) + np.array(env, dtype=np.float32) * (opacity * a)
    if light is not None:
        h, w = arr.shape[:2]
        yy, xx = np.mgrid[0:h, 0:w]
        mask = (1 - (xx / max(1, w)) * .55 - (yy / max(1, h)) * .35).clip(0, 1)[:, :, None]
        rgb = rgb * (1 - mask * .08 * a) + np.array(light, dtype=np.float32) * (mask * .08 * a)
    arr[:, :, :3] = rgb
    return Image.fromarray(np.clip(arr, 0, 255).astype(np.uint8), "RGBA")


def save(name: str, image: Image.Image):
    path = OUT / name
    feather(image).save(path)
    return path


def boost_alpha(image: Image.Image, factor: float) -> Image.Image:
    image = image.convert("RGBA")
    arr = np.array(image).astype(np.float32)
    arr[:, :, 3] = np.clip(arr[:, :, 3] * factor, 0, 255)
    return Image.fromarray(arr.astype(np.uint8), "RGBA")


def crop(atlas, box, name, env, opacity=.12, saturation=1.0, light=None):
    image = matte_to_alpha(atlas.crop(box))
    image = tint(image, env, opacity, saturation, light)
    return save(name, image)


def shadow(name, size, box, alpha=90, blur=22):
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    layer = Image.new("RGBA", size, (0, 0, 0, 0))
    from PIL import ImageDraw
    d = ImageDraw.Draw(layer, "RGBA")
    d.ellipse(box, fill=(0, 0, 0, alpha))
    img.alpha_composite(layer.filter(ImageFilter.GaussianBlur(blur)))
    img.save(OUT / name)


def composite(bg_path, placements, out):
    bg = Image.open(bg_path).convert("RGBA").resize((1290, 2796), Image.Resampling.LANCZOS)
    for path, pos, scale in placements:
        item = Image.open(path).convert("RGBA")
        item = item.resize((round(item.width * scale), round(item.height * scale)), Image.Resampling.LANCZOS)
        bg.alpha_composite(item, pos)
    bg.save(out)


def main():
    chamber, forest = load_palette()
    warm = (226, 180, 86)
    cold = (115, 185, 172)
    atlas = Image.open(ATLAS).convert("RGBA")

    crop(atlas, (0, 0, 300, 520), "aileen_chamber.png", chamber[0], .10, .96, warm)
    shadow("aileen_chamber_shadow.png", (360, 100), (42, 28, 320, 78), 96, 18)
    monster = matte_to_alpha(atlas.crop((300, 35, 585, 590)))
    monster = tint(monster, forest[-1], .10, .92, cold)
    save("forest_shadow_slime.png", boost_alpha(monster, 1.55))
    shadow("forest_shadow_slime_shadow.png", (360, 95), (30, 18, 330, 70), 105, 18)

    crop(atlas, (0, 1040, 305, 1380), "furniture_chamber_window.png", chamber[0], .08, 1.0, warm)
    crop(atlas, (300, 1000, 735, 1325), "furniture_chamber_rug.png", chamber[0], .10, .95, warm)
    crop(atlas, (720, 990, 1024, 1370), "furniture_chamber_bookcase.png", chamber[0], .12, .92, warm)

    crop(atlas, (0, 600, 200, 890), "ui_panel_chamber_parchment.png", chamber[1], .08, .96, warm)
    crop(atlas, (545, 595, 820, 805), "ui_panel_forest_frosted_metal.png", forest[-1], .18, .82, cold)
    crop(atlas, (220, 800, 525, 895), "ui_button_chamber_brass.png", chamber[1], .08, .98, warm)
    crop(atlas, (545, 785, 835, 895), "ui_button_forest_cold_brass.png", forest[-1], .18, .82, cold)

    symbol_boxes = {
        "sword": (0, 1345, 170, 1536),
        "staff": (170, 1345, 340, 1536),
        "heart": (340, 1345, 510, 1536),
        "shield": (510, 1345, 680, 1536),
        "skull": (680, 1345, 850, 1536),
        "star": (850, 1345, 1024, 1536),
    }
    universal = []
    forest_paths = []
    for name, box in symbol_boxes.items():
        universal.append(crop(atlas, box, f"symbol_{name}_universal.png", chamber[0], .06, 1.0, warm))
        forest_paths.append(crop(atlas, box, f"symbol_{name}_forest.png", forest[-1], .20, .82, cold))

    for sheet_name, paths in [("symbols_universal_sheet.png", universal), ("symbols_forest_sheet.png", forest_paths)]:
        sheet = Image.new("RGBA", (768, 512), (0, 0, 0, 0))
        for i, path in enumerate(paths):
            icon = Image.open(path).convert("RGBA").resize((230, 230), Image.Resampling.LANCZOS)
            sheet.alpha_composite(icon, ((i % 3) * 256 + 13, (i // 3) * 256 + 13))
        sheet.save(OUT / sheet_name)

    shadow("furniture_chamber_floor_shadow.png", (700, 260), (120, 70, 610, 165), 70, 34)
    composite(
        CHAMBER_BG,
        [
            (OUT / "furniture_chamber_floor_shadow.png", (210, 1975), .9),
            (OUT / "furniture_chamber_rug.png", (300, 2160), .92),
            (OUT / "furniture_chamber_bookcase.png", (880, 610), .95),
            (OUT / "aileen_chamber_shadow.png", (185, 1905), 1.08),
            (OUT / "aileen_chamber.png", (70, 920), 1.7),
            (OUT / "ui_panel_chamber_parchment.png", (95, 110), 2.4),
        ],
        PREVIEW / "validation_chamber_fusion.png",
    )
    composite(
        BATTLE_BG,
        [
            (OUT / "forest_shadow_slime_shadow.png", (730, 1045), 1.4),
            (OUT / "forest_shadow_slime.png", (640, 625), 1.55),
            (OUT / "symbols_forest_sheet.png", (345, 1775), .78),
            (OUT / "ui_panel_forest_frosted_metal.png", (285, 1350), 2.45),
        ],
        PREVIEW / "validation_forest_fusion.png",
    )
    print("Extracted atlas sprites to", OUT)


if __name__ == "__main__":
    main()
