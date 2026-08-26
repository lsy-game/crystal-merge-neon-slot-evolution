#!/usr/bin/env python3
from __future__ import annotations

import json
from pathlib import Path

from PIL import Image, ImageEnhance, ImageFilter, ImageOps, ImageDraw, ImageChops

PROJECT = Path("/Users/zhendian/Documents/New project/命运游侠_Unity工程")
SRC = PROJECT / "Assets/DestinyRanger/Art/Generated/FateWeaverFusion"
OUT = PROJECT / "Assets/DestinyRanger/Art/Generated/FateWeaverDeliverables"


def ensure():
    for d in [
        OUT / "Characters/Aileen",
        OUT / "Symbols/Forest",
        OUT / "Furniture/Chamber",
        OUT / "UI/Panels",
        OUT / "UI/Buttons/Primary",
        OUT / "Docs",
    ]:
        d.mkdir(parents=True, exist_ok=True)


def feather_alpha(image: Image.Image, pixels: int = 2, strength: float = 0.42) -> Image.Image:
    image = image.convert("RGBA")
    alpha = image.getchannel("A")
    eroded = alpha.filter(ImageFilter.MinFilter(pixels * 2 + 1))
    edge = Image.eval(ImageChops.subtract(alpha, eroded), lambda v: int(v * strength))
    image.putalpha(ImageChops.subtract(alpha, edge))
    return image


def contain(image: Image.Image, size: tuple[int, int], pad: int = 0) -> Image.Image:
    image = image.convert("RGBA")
    box = image.getchannel("A").getbbox()
    if box:
        image = image.crop(box)
    target = (size[0] - pad * 2, size[1] - pad * 2)
    image.thumbnail(target, Image.Resampling.LANCZOS)
    canvas = Image.new("RGBA", size, (0, 0, 0, 0))
    canvas.alpha_composite(image, ((size[0] - image.width) // 2, (size[1] - image.height) // 2))
    return feather_alpha(canvas)


def save(path: Path, image: Image.Image):
    path.parent.mkdir(parents=True, exist_ok=True)
    image.save(path)


def soft_shadow_for(sprite: Image.Image, size: tuple[int, int], y_bias: float = .72) -> Image.Image:
    alpha = sprite.convert("RGBA").getchannel("A")
    box = alpha.getbbox()
    shadow = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(shadow, "RGBA")
    if box:
        width = max(34, int((box[2] - box[0]) * .82))
        height = max(14, int(width * .22))
        cx = size[0] // 2
        cy = int(size[1] * y_bias)
        draw.ellipse((cx - width // 2, cy - height // 2, cx + width // 2, cy + height // 2), fill=(0, 0, 0, 92))
    return shadow.filter(ImageFilter.GaussianBlur(max(8, size[0] // 24)))


def make_character():
    sprite = Image.open(SRC / "aileen_chamber.png").convert("RGBA")
    sprite = contain(sprite, (512, 768), 10)
    save(OUT / "Characters/Aileen/aileen_idle_chamber_rgba.png", sprite)
    save(OUT / "Characters/Aileen/aileen_idle_chamber_shadow.png", soft_shadow_for(sprite, (512, 150), .55))


def make_symbols():
    names = ["sword", "staff", "heart", "shield", "skull", "star"]
    cn = {
        "sword": "剑",
        "staff": "杖",
        "heart": "心",
        "shield": "盾",
        "skull": "骷髅",
        "star": "星",
    }
    for name in names:
        src = Image.open(SRC / f"symbol_{name}_forest.png").convert("RGBA")
        save(OUT / f"Symbols/Forest/symbol_forest_{name}_{cn[name]}_180x180.png", contain(src, (180, 180), 12))


def make_furniture():
    specs = [
        ("window", "furniture_chamber_window.png", (420, 600), "window_shadow.png"),
        ("rug", "furniture_chamber_rug.png", (620, 340), "rug_shadow.png"),
        ("bookcase", "furniture_chamber_bookcase.png", (360, 760), "bookcase_shadow.png"),
    ]
    for name, src_name, size, shadow_name in specs:
        sprite = contain(Image.open(SRC / src_name), size, 0)
        save(OUT / f"Furniture/Chamber/{name}_chamber_rgba.png", sprite)
        if name == "rug":
            shadow = Image.open(SRC / "furniture_chamber_floor_shadow.png").convert("RGBA")
            shadow = contain(shadow, (620, 180), 0)
        else:
            shadow = soft_shadow_for(sprite, (size[0], max(90, size[1] // 5)), .58)
        save(OUT / f"Furniture/Chamber/{shadow_name}", shadow)


def nine_slice_panel():
    src = Image.open(SRC / "ui_panel_chamber_parchment.png").convert("RGBA")
    src = ImageOps.contain(src, (900, 600), Image.Resampling.LANCZOS)
    canvas = Image.new("RGBA", (900, 600), (0, 0, 0, 0))
    canvas.alpha_composite(src, ((900 - src.width) // 2, (600 - src.height) // 2))
    draw = ImageDraw.Draw(canvas, "RGBA")
    # Strengthen borders so Unity slicing remains readable after scaling.
    draw.rounded_rectangle((24, 24, 876, 576), radius=34, outline=(212, 175, 55, 230), width=12)
    draw.rounded_rectangle((58, 58, 842, 542), radius=22, outline=(18, 26, 42, 120), width=7)
    save(OUT / "UI/Panels/panel_chamber_parchment_9slice_900x600_border96.png", feather_alpha(canvas))


def button_states():
    base_src = Image.open(SRC / "ui_button_chamber_brass.png").convert("RGBA")
    base = ImageOps.contain(base_src, (280, 100), Image.Resampling.LANCZOS)
    canvas = Image.new("RGBA", (280, 100), (0, 0, 0, 0))
    canvas.alpha_composite(base, ((280 - base.width) // 2, (100 - base.height) // 2))
    draw = ImageDraw.Draw(canvas, "RGBA")
    draw.rounded_rectangle((8, 8, 272, 92), radius=20, outline=(139, 105, 20, 230), width=5)
    draw.rounded_rectangle((18, 16, 262, 86), radius=16, outline=(244, 220, 116, 105), width=4)
    states = {
        "normal": canvas,
        "hover": ImageEnhance.Brightness(canvas).enhance(1.14),
        "pressed": ImageEnhance.Brightness(ImageEnhance.Contrast(canvas).enhance(.92)).enhance(.78),
        "disabled": ImageEnhance.Color(ImageEnhance.Brightness(canvas).enhance(.58)).enhance(.22),
    }
    for state, image in states.items():
        save(OUT / f"UI/Buttons/Primary/primary_button_{state}_280x100.png", feather_alpha(image))


def docs():
    font_doc = """# Fate Weaver Font Rendering Plan

## 标题：命运纺机

- 推荐字体：Cinzel / Songti SC Bold 风格。当前机器未发现项目内 Cinzel 文件，Unity 工程可先用系统 `/System/Library/Fonts/Supplemental/Songti.ttc` 生成 TextMeshPro Font Asset。
- 生成方式：Unity 打开后执行 `Destiny Ranger/Typography/Create TMP Font Assets`，或在 TextMeshPro Font Asset Creator 中使用 Songti / Cinzel TTF/OTF，Sampling Point Size 90，Padding 9，Atlas 2048。
- 清晰度方案：标题字号 72-80，SDF 渲染，金色 `#D4AF37`，黑色 3px 投影，移动端 Canvas Scaler 使用 1290x2796 reference resolution。

## 正文

- 推荐字体：Source Han Sans SC / PingFang SC / STHeiti。当前机器可用 `/System/Library/Fonts/STHeiti Medium.ttc`。
- 渲染方式：TextMeshProUGUI，正文 32-40，小字 24-28，SDF atlas 2048，Fallback Font Assets 包含 Songti 与 STHeiti。
- TextMeshPro 状态：Package `com.unity.textmeshpro` 已在 `Packages/manifest.json` 配置；本轮因 Unity Licensing IPC 无法进入编辑器，未能实际生成 TMP_FontAsset 文件。已交付编辑器脚本入口，Unity 授权恢复后可一键生成。
"""
    (OUT / "Docs/FONT_RENDERING_PLAN.md").write_text(font_doc, encoding="utf-8")
    manifest = {
        "characters": ["Characters/Aileen/aileen_idle_chamber_rgba.png", "Characters/Aileen/aileen_idle_chamber_shadow.png"],
        "forest_symbols_180x180": sorted(str(p.relative_to(OUT)) for p in (OUT / "Symbols/Forest").glob("*.png")),
        "furniture": sorted(str(p.relative_to(OUT)) for p in (OUT / "Furniture/Chamber").glob("*.png")),
        "ui_panels": ["UI/Panels/panel_chamber_parchment_9slice_900x600_border96.png"],
        "primary_button_states": sorted(str(p.relative_to(OUT)) for p in (OUT / "UI/Buttons/Primary").glob("*.png")),
        "nine_slice_border_pixels": 96,
        "no_validation_images_generated_by_this_script": True,
    }
    (OUT / "deliverables_manifest.json").write_text(json.dumps(manifest, ensure_ascii=False, indent=2), encoding="utf-8")


def main():
    ensure()
    make_character()
    make_symbols()
    make_furniture()
    nine_slice_panel()
    button_states()
    docs()
    print(OUT)


if __name__ == "__main__":
    main()
