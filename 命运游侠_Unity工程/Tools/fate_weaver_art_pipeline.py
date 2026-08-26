#!/usr/bin/env python3
from __future__ import annotations

import json
import math
from collections import Counter
from pathlib import Path

import numpy as np
from PIL import Image, ImageDraw, ImageFilter


PROJECT = Path("/Users/zhendian/Documents/New project/命运游侠_Unity工程")
GENERATED = PROJECT / "Assets/DestinyRanger/Art/Generated"
OUT = GENERATED / "FateWeaverFusion"
PREVIEW = PROJECT / "Assets/DestinyRanger/Art/Generated/FateWeaverFusionPreviews"

CHAMBER_BG = GENERATED / "fate-weaver-chamber-bg.png"
BATTLE_BG = GENERATED / "fate-weaver-battle-forest-bg.png"


def rgba(hex_color: str, alpha: int = 255) -> tuple[int, int, int, int]:
    hex_color = hex_color.strip("#")
    return tuple(int(hex_color[i:i + 2], 16) for i in (0, 2, 4)) + (alpha,)


def blend(a, b, t):
    return tuple(int(a[i] * (1 - t) + b[i] * t) for i in range(3))


def top_palette(path: Path, k: int = 3) -> list[tuple[int, int, int]]:
    image = Image.open(path).convert("RGB")
    w, h = image.size
    sample_w = 180
    sample_h = max(1, round(h * sample_w / w))
    image = image.resize((sample_w, sample_h), Image.Resampling.BILINEAR)
    pixels = np.array(image).reshape((-1, 3))
    # Quantize to stable buckets, then choose separated dominant colors.
    buckets = (pixels // 16) * 16
    counts = Counter(map(tuple, buckets.tolist()))
    selected: list[tuple[int, int, int]] = []
    for color, _ in counts.most_common(64):
        if sum(color) < 22:
            continue
        if all(math.dist(color, other) > 36 for other in selected):
            selected.append(color)
        if len(selected) == k:
            break
    return selected[:k]


def apply_environment_tint(image: Image.Image, env: tuple[int, int, int], opacity: float) -> Image.Image:
    image = image.convert("RGBA")
    arr = np.array(image).astype(np.float32)
    alpha = arr[:, :, 3:4] / 255.0
    tint = np.array(env, dtype=np.float32)
    arr[:, :, :3] = arr[:, :, :3] * (1.0 - opacity * alpha) + tint * (opacity * alpha)
    return Image.fromarray(np.clip(arr, 0, 255).astype(np.uint8), "RGBA")


def feather_alpha(image: Image.Image, pixels: int = 2, strength: float = 0.42) -> Image.Image:
    image = image.convert("RGBA")
    alpha = image.getchannel("A")
    eroded = alpha.filter(ImageFilter.MinFilter(pixels * 2 + 1))
    edge = Image.eval(ImageChops.subtract(alpha, eroded), lambda v: int(v * strength))
    new_alpha = ImageChops.subtract(alpha, edge)
    image.putalpha(new_alpha)
    return image


try:
    from PIL import ImageChops
except ImportError as exc:
    raise RuntimeError("Pillow ImageChops is required") from exc


def shadow(size: tuple[int, int], ellipse: tuple[int, int, int, int], blur: int, alpha: int, skew: int = 0) -> Image.Image:
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    d.ellipse(ellipse, fill=(0, 0, 0, alpha))
    img = img.filter(ImageFilter.GaussianBlur(blur))
    if skew:
        img = img.transform(size, Image.Transform.AFFINE, (1, skew / 100.0, -skew, 0, 1, 0), resample=Image.Resampling.BICUBIC)
    return img


def save_asset(name: str, image: Image.Image, do_feather: bool = True) -> Path:
    OUT.mkdir(parents=True, exist_ok=True)
    if do_feather:
        image = feather_alpha(image)
    path = OUT / name
    image.save(path)
    return path


def draw_aileen(chamber_palette):
    env = chamber_palette[0]
    warm = (226, 180, 79)
    cool = (70, 130, 180)
    img = Image.new("RGBA", (520, 820), (0, 0, 0, 0))
    d = ImageDraw.Draw(img, "RGBA")
    # Cloak and skirt.
    d.polygon([(245, 180), (140, 640), (380, 640), (310, 180)], fill=rgba("#44345C", 245))
    d.polygon([(205, 210), (130, 520), (250, 475)], fill=rgba("#77504D", 230))
    d.polygon([(315, 210), (390, 520), (260, 475)], fill=rgba("#263B5B", 230))
    # Boots and arms.
    d.rounded_rectangle((190, 600, 232, 760), radius=18, fill=rgba("#1E2434", 245))
    d.rounded_rectangle((288, 600, 330, 760), radius=18, fill=rgba("#1B2636", 245))
    d.line((160, 310, 105, 500), fill=rgba("#D9A55B", 230), width=26)
    d.line((360, 310, 430, 500), fill=rgba("#70B8DC", 220), width=24)
    # Torso armor.
    d.rounded_rectangle((190, 220, 330, 430), radius=42, fill=rgba("#6C5576", 245), outline=rgba("#D4AF37", 210), width=5)
    d.polygon([(195, 230), (245, 415), (190, 420)], fill=rgba("#D9AA5C", 75))
    d.polygon([(330, 230), (280, 415), (335, 420)], fill=rgba("#5EA8D0", 80))
    # Head and hair.
    d.ellipse((192, 88, 328, 230), fill=rgba("#D8A27C", 255))
    d.pieslice((170, 58, 350, 250), 190, 350, fill=rgba("#2B2438", 255))
    d.polygon([(200, 120), (135, 300), (230, 230)], fill=rgba("#2B2438", 250))
    d.polygon([(310, 120), (385, 300), (285, 230)], fill=rgba("#1B2E45", 250))
    # Face lighting and features.
    d.ellipse((205, 102, 270, 218), fill=(*warm, 42))
    d.ellipse((260, 102, 323, 218), fill=(*cool, 36))
    d.ellipse((225, 155, 238, 168), fill=rgba("#0A0F1E", 255))
    d.ellipse((285, 155, 298, 168), fill=rgba("#0A0F1E", 255))
    d.arc((232, 170, 292, 205), 20, 150, fill=rgba("#5B2D3A", 255), width=3)
    # Sword/book prop.
    d.line((102, 505, 88, 215), fill=rgba("#C9D7DF", 240), width=12)
    d.line((88, 215, 115, 255), fill=rgba("#D4AF37", 230), width=7)
    # Painterly color blocks.
    for i in range(24):
        x = 160 + (i * 41) % 220
        y = 250 + (i * 67) % 360
        d.ellipse((x, y, x + 45, y + 80), fill=(*blend(env, (255, 255, 255), .18), 16))
    img = apply_environment_tint(img, env, .12)
    save_asset("aileen_chamber.png", img)
    save_asset("aileen_chamber_shadow.png", shadow((520, 180), (95, 35, 430, 120), 24, 95, skew=-8), do_feather=False)


def symbol_icon(kind: str, palette, forest=False) -> Image.Image:
    env = palette[0]
    img = Image.new("RGBA", (256, 256), (0, 0, 0, 0))
    d = ImageDraw.Draw(img, "RGBA")
    if kind == "sword":
        d.polygon([(124, 30), (144, 30), (139, 158), (129, 210), (119, 158)], fill=rgba("#C8D1D6", 255))
        d.line((133, 40, 133, 170), fill=rgba("#F3F6F0", 190), width=5)
        d.rounded_rectangle((82, 160, 184, 180), radius=8, fill=rgba("#D4AF37", 255))
        d.rounded_rectangle((120, 176, 146, 228), radius=8, fill=rgba("#4C3446", 255))
    elif kind == "staff":
        d.line((84, 224, 168, 46), fill=rgba("#8F5BD2", 255), width=18)
        d.ellipse((134, 28, 202, 96), fill=rgba("#C850B4", 220), outline=rgba("#F1B9FF", 220), width=5)
        d.ellipse((154, 48, 182, 76), fill=rgba("#F1E6FF", 190))
    elif kind == "heart":
        d.ellipse((58, 64, 132, 142), fill=rgba("#E83445", 255))
        d.ellipse((124, 64, 198, 142), fill=rgba("#F24A57", 255))
        d.polygon([(48, 112), (208, 112), (128, 222)], fill=rgba("#E83445", 255))
        d.ellipse((40, 48, 216, 224), outline=rgba("#FF98A0", 95), width=10)
    elif kind == "shield":
        d.polygon([(128, 28), (206, 62), (188, 168), (128, 226), (68, 168), (50, 62)], fill=rgba("#2F78C9", 255), outline=rgba("#BFD9EE", 230))
        d.polygon([(128, 42), (188, 70), (174, 158), (128, 210)], fill=rgba("#58B8DF", 100))
    elif kind == "skull":
        d.ellipse((64, 44, 192, 174), fill=rgba("#15191C", 255), outline=rgba("#63C474", 170), width=5)
        d.rounded_rectangle((88, 150, 168, 218), radius=18, fill=rgba("#111417", 255))
        d.ellipse((88, 96, 118, 128), fill=rgba("#70D982", 210))
        d.ellipse((138, 96, 168, 128), fill=rgba("#70D982", 210))
        for x in (104, 126, 148):
            d.line((x, 166, x, 207), fill=rgba("#3E8C53", 160), width=4)
    else:
        pts = []
        for i in range(10):
            a = -math.pi / 2 + i * math.pi / 5
            r = 96 if i % 2 == 0 else 38
            pts.append((128 + math.cos(a) * r, 128 + math.sin(a) * r))
        d.polygon(pts, fill=rgba("#F3C74C", 255), outline=rgba("#FFF2A0", 230))
        d.line((128, 20, 128, 236), fill=rgba("#FFF4B8", 135), width=5)
        d.line((20, 128, 236, 128), fill=rgba("#FFF4B8", 90), width=4)
    tint = .16 if forest else .08
    img = apply_environment_tint(img, env, tint)
    if forest:
        arr = np.array(img).astype(np.float32)
        arr[:, :, :3] *= .85
        img = Image.fromarray(np.clip(arr, 0, 255).astype(np.uint8), "RGBA")
    return img


def draw_symbols(chamber_palette, forest_palette):
    kinds = ["sword", "staff", "heart", "shield", "skull", "star"]
    for kind in kinds:
        save_asset(f"symbol_{kind}_universal.png", symbol_icon(kind, chamber_palette, False))
        save_asset(f"symbol_{kind}_forest.png", symbol_icon(kind, forest_palette, True))
    for suffix, palette, forest in [("universal", chamber_palette, False), ("forest", forest_palette, True)]:
        sheet = Image.new("RGBA", (768, 512), (0, 0, 0, 0))
        for i, kind in enumerate(kinds):
            sheet.alpha_composite(symbol_icon(kind, palette, forest), ((i % 3) * 256, (i // 3) * 256))
        save_asset(f"symbols_{suffix}_sheet.png", sheet)


def draw_monster(forest_palette):
    env = forest_palette[0]
    img = Image.new("RGBA", (460, 420), (0, 0, 0, 0))
    d = ImageDraw.Draw(img, "RGBA")
    base = blend((45, 150, 105), env, .32)
    dark = blend((35, 30, 70), env, .35)
    d.ellipse((68, 125, 390, 340), fill=(*base, 235), outline=(*blend(base, (230, 255, 235), .35), 180), width=5)
    d.ellipse((100, 82, 245, 245), fill=(*base, 215))
    d.ellipse((205, 76, 356, 250), fill=(*blend(base, (80, 190, 150), .35), 205))
    d.polygon([(70, 244), (28, 318), (112, 304)], fill=(*dark, 190))
    d.polygon([(380, 246), (430, 315), (344, 306)], fill=(*dark, 190))
    d.ellipse((144, 160, 178, 196), fill=rgba("#B8FFE0", 230))
    d.ellipse((270, 160, 304, 196), fill=rgba("#B8FFE0", 230))
    d.arc((185, 205, 265, 260), 20, 160, fill=rgba("#16342B", 230), width=7)
    d.ellipse((105, 105, 350, 320), outline=rgba("#B8FFE0", 45), width=18)
    img = apply_environment_tint(img, env, .15)
    save_asset("forest_shadow_slime.png", img)
    save_asset("forest_shadow_slime_shadow.png", shadow((460, 130), (56, 25, 408, 100), 22, 100), do_feather=False)


def draw_furniture(chamber_palette):
    env = chamber_palette[0]
    warm = (218, 166, 68)
    cool = (50, 90, 132)
    # Window
    img = Image.new("RGBA", (420, 660), (0, 0, 0, 0))
    d = ImageDraw.Draw(img, "RGBA")
    d.rounded_rectangle((28, 28, 392, 632), radius=56, fill=rgba("#111A31", 235), outline=(*warm, 210), width=18)
    for x in (145, 275):
        d.line((x, 48, x, 612), fill=(*warm, 190), width=10)
    d.line((50, 330, 370, 330), fill=(*warm, 180), width=10)
    for i in range(12):
        x = 70 + (i * 43) % 280
        y = 95 + (i * 71) % 430
        d.ellipse((x, y, x + 8, y + 8), fill=rgba("#F4EBC8", 180))
    d.rectangle((42, 42, 378, 618), outline=(*cool, 45), width=22)
    save_asset("furniture_chamber_window.png", apply_environment_tint(img, env, .08))
    # Rug
    img = Image.new("RGBA", (620, 340), (0, 0, 0, 0))
    d = ImageDraw.Draw(img, "RGBA")
    d.ellipse((30, 45, 590, 300), fill=rgba("#623B55", 235), outline=(*warm, 210), width=12)
    d.ellipse((100, 92, 520, 252), outline=rgba("#2B4B77", 170), width=16)
    d.line((90, 170, 530, 170), fill=rgba("#D4AF37", 130), width=7)
    save_asset("furniture_chamber_rug.png", apply_environment_tint(img, env, .12))
    # Bookcase
    img = Image.new("RGBA", (360, 780), (0, 0, 0, 0))
    d = ImageDraw.Draw(img, "RGBA")
    d.rounded_rectangle((35, 28, 325, 748), radius=28, fill=rgba("#211726", 245), outline=(*warm, 185), width=12)
    for y in range(145, 700, 120):
        d.rectangle((55, y, 305, y + 12), fill=(*warm, 150))
    colors = ["#5A3347", "#283C5E", "#6A5120", "#1E4D43"]
    for row, y in enumerate(range(68, 650, 120)):
        x = 70
        for i in range(7):
            w = 18 + (i * 7 + row * 5) % 18
            d.rectangle((x, y + (i % 3) * 5, x + w, y + 74), fill=rgba(colors[(i + row) % len(colors)], 230))
            x += w + 9
    save_asset("furniture_chamber_bookcase.png", apply_environment_tint(img, env, .1))
    save_asset("furniture_chamber_floor_shadow.png", shadow((700, 260), (120, 70, 610, 165), 34, 70, skew=-16), do_feather=False)


def draw_ui_textures(chamber_palette, forest_palette):
    def texture(name, palette, base, accent):
        img = Image.new("RGBA", (720, 320), (*base, 218))
        d = ImageDraw.Draw(img, "RGBA")
        for y in range(0, 320, 6):
            c = blend(base, palette[y // 6 % len(palette)], .09)
            d.line((0, y, 720, y + ((y // 6) % 3)), fill=(*c, 55), width=2)
        for i in range(90):
            x = (i * 97) % 720
            y = (i * 53) % 320
            r = 8 + i % 17
            d.ellipse((x, y, x + r, y + r // 2), fill=(*blend(base, accent, .28), 20))
        d.rounded_rectangle((14, 14, 706, 306), radius=30, outline=(*accent, 210), width=7)
        d.rounded_rectangle((30, 30, 690, 290), radius=22, outline=(*palette[0], 80), width=10)
        save_asset(name, img)
    texture("ui_panel_chamber_parchment.png", chamber_palette, blend((54, 43, 36), chamber_palette[0], .22), (212, 175, 55))
    texture("ui_panel_forest_frosted_metal.png", forest_palette, blend((31, 47, 48), forest_palette[0], .35), (100, 200, 255))
    texture("ui_button_chamber_brass.png", chamber_palette, blend((152, 113, 38), chamber_palette[0], .15), (240, 210, 116))
    texture("ui_button_forest_cold_brass.png", forest_palette, blend((108, 99, 58), forest_palette[0], .35), (170, 220, 210))


def composite_preview(bg_path: Path, placements: list[tuple[Path, tuple[int, int], float]], out_path: Path):
    bg = Image.open(bg_path).convert("RGBA").resize((1290, 2796), Image.Resampling.LANCZOS)
    for path, pos, scale in placements:
        item = Image.open(path).convert("RGBA")
        item = item.resize((round(item.width * scale), round(item.height * scale)), Image.Resampling.LANCZOS)
        bg.alpha_composite(item, pos)
    PREVIEW.mkdir(parents=True, exist_ok=True)
    bg.save(out_path)


def write_report(chamber_palette, forest_palette):
    report = {
        "chamber_background": str(CHAMBER_BG),
        "chamber_palette": ["#%02X%02X%02X" % c for c in chamber_palette],
        "chamber_light": "upper-left warm gold, cool blue bounce on right side",
        "forest_background": str(BATTLE_BG),
        "forest_palette": ["#%02X%02X%02X" % c for c in forest_palette],
        "forest_light": "cold green top light plus diffuse environment bounce",
        "edge_feather": "1-2 px alpha falloff, about 42% edge reduction",
        "ao_shadow": "separate soft ellipse shadow PNGs for character, monster, and furniture grounding",
    }
    (OUT / "palette_report.json").write_text(json.dumps(report, ensure_ascii=False, indent=2), encoding="utf-8")
    md = [
        "# Fate Weaver Art Fusion Report",
        "",
        f"- Chamber palette: {', '.join(report['chamber_palette'])}",
        f"- Forest palette: {', '.join(report['forest_palette'])}",
        "- Chamber lighting: upper-left warm gold, cool blue bounce.",
        "- Forest lighting: cold green top light, lowered saturation, purple-green shadow bias.",
        "- Export: PNG-24 RGBA, transparent alpha where sprite assets require it.",
        "- Edge treatment: generated alpha sprites are feathered; Unity editor postprocessor also feathers imported fusion sprites.",
    ]
    (OUT / "ART_FUSION_REPORT.md").write_text("\n".join(md), encoding="utf-8")


def main():
    OUT.mkdir(parents=True, exist_ok=True)
    PREVIEW.mkdir(parents=True, exist_ok=True)
    chamber_palette = top_palette(CHAMBER_BG)
    forest_palette = top_palette(BATTLE_BG)
    draw_aileen(chamber_palette)
    draw_symbols(chamber_palette, forest_palette)
    draw_monster(forest_palette)
    draw_furniture(chamber_palette)
    draw_ui_textures(chamber_palette, forest_palette)
    composite_preview(
        CHAMBER_BG,
        [
            (OUT / "furniture_chamber_floor_shadow.png", (210, 1975), .9),
            (OUT / "furniture_chamber_rug.png", (310, 2130), .9),
            (OUT / "furniture_chamber_bookcase.png", (910, 600), .85),
            (OUT / "aileen_chamber_shadow.png", (210, 1860), .9),
            (OUT / "aileen_chamber.png", (190, 1230), .9),
            (OUT / "ui_panel_chamber_parchment.png", (285, 145), 1.0),
        ],
        PREVIEW / "validation_chamber_fusion.png",
    )
    composite_preview(
        BATTLE_BG,
        [
            (OUT / "forest_shadow_slime_shadow.png", (730, 1030), 1.2),
            (OUT / "forest_shadow_slime.png", (700, 735), 1.2),
            (OUT / "symbols_forest_sheet.png", (345, 1775), .78),
            (OUT / "ui_panel_forest_frosted_metal.png", (285, 1350), 1.0),
        ],
        PREVIEW / "validation_forest_fusion.png",
    )
    write_report(chamber_palette, forest_palette)
    print("CHAMBER_PALETTE", ["#%02X%02X%02X" % c for c in chamber_palette])
    print("FOREST_PALETTE", ["#%02X%02X%02X" % c for c in forest_palette])
    print("OUT", OUT)
    print("PREVIEW", PREVIEW)


if __name__ == "__main__":
    main()
