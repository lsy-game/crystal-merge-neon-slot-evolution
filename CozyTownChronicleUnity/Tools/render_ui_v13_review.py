#!/usr/bin/env python3
"""Render 1290x2796 visual QA boards using the same v13 art and runtime layout.

These images are deterministic layout proofs, not substitutes for Unity device captures.
All labels are drawn separately to enforce the project's no-baked-text rule.
"""

from __future__ import annotations

from pathlib import Path
from PIL import Image, ImageDraw, ImageFont, ImageFilter


ROOT = Path(__file__).resolve().parents[1]
ART = ROOT / "Assets" / "WheatTown" / "Art" / "Images"
UI = ROOT / "Assets" / "Resources" / "WheatTown" / "generated-ui-v13"
OUT = ROOT / "QA" / "ui-v13"
SCALE = 3
W, H = 430 * SCALE, 932 * SCALE

FOREST = "#213A2B"
DARK = "#233426"
BODY = "#493F30"
IVORY = "#FFF6DB"
PAPER = "#FFF8E8"
GOLD = "#B88743"
ORANGE = "#E88D2F"
MUTED = "#8A857A"

REGULAR = "/System/Library/Fonts/Supplemental/Trebuchet MS.ttf"
BOLD = "/System/Library/Fonts/Supplemental/Trebuchet MS Bold.ttf"


def font(size: int, bold: bool = False) -> ImageFont.FreeTypeFont:
    return ImageFont.truetype(BOLD if bold else REGULAR, size * SCALE)


def reference_xy(x: float, y: float) -> tuple[int, int]:
    """Unity reference-space center -> target image center."""
    return round((x + 215) * SCALE), round((466 - y) * SCALE)


def rect_from_center(x: float, y: float, w: float, h: float) -> tuple[int, int, int, int]:
    cx, cy = reference_xy(x, y)
    sw, sh = round(w * SCALE), round(h * SCALE)
    return cx - sw // 2, cy - sh // 2, cx + sw // 2, cy + sh // 2


def cover(path: Path) -> Image.Image:
    src = Image.open(path).convert("RGB")
    ratio = max(W / src.width, H / src.height)
    resized = src.resize((round(src.width * ratio), round(src.height * ratio)), Image.Resampling.LANCZOS)
    x = (resized.width - W) // 2
    y = (resized.height - H) // 2
    return resized.crop((x, y, x + W, y + H)).convert("RGBA")


def text_center(draw: ImageDraw.ImageDraw, box: tuple[int, int, int, int], value: str,
                size: int, fill: str, bold: bool = False) -> None:
    f = font(size, bold)
    left, top, right, bottom = draw.textbbox((0, 0), value, font=f)
    x = box[0] + ((box[2] - box[0]) - (right - left)) / 2
    y = box[1] + ((box[3] - box[1]) - (bottom - top)) / 2 - top
    draw.text((round(x), round(y)), value, font=f, fill=fill)


def text_left(draw: ImageDraw.ImageDraw, box: tuple[int, int, int, int], value: str,
              size: int, fill: str, bold: bool = False, pad: int = 0) -> None:
    f = font(size, bold)
    _, top, _, bottom = draw.textbbox((0, 0), value, font=f)
    y = box[1] + ((box[3] - box[1]) - (bottom - top)) / 2 - top
    draw.text((box[0] + pad * SCALE, round(y)), value, font=f, fill=fill)


def paste_art(canvas: Image.Image, path: Path, box: tuple[int, int, int, int], shadow: bool = True) -> None:
    art = Image.open(path).convert("RGBA").resize((box[2] - box[0], box[3] - box[1]), Image.Resampling.LANCZOS)
    if shadow:
        alpha = art.getchannel("A")
        shadow_layer = Image.new("RGBA", art.size, (53, 42, 26, 0))
        shadow_layer.putalpha(alpha.filter(ImageFilter.GaussianBlur(4 * SCALE)).point(lambda p: round(p * .24)))
        canvas.alpha_composite(shadow_layer, (box[0] + 4 * SCALE, box[1] + 6 * SCALE))
    canvas.alpha_composite(art, (box[0], box[1]))


def rounded(draw: ImageDraw.ImageDraw, box: tuple[int, int, int, int], fill: str,
            outline: str = GOLD, width: int = 1, radius: int = 8) -> None:
    draw.rounded_rectangle(box, radius=radius * SCALE, fill=fill, outline=outline, width=width * SCALE)


def button(draw: ImageDraw.ImageDraw, x: float, y: float, w: float, h: float, label: str,
           fill: str, text_fill: str = IVORY, size: int = 16) -> None:
    box = rect_from_center(x, y, w, h)
    rounded(draw, box, fill, "#C89A53", 1, 9)
    # Narrow top highlight and bottom contact shade keep it dimensional without a pasted black plate.
    draw.line((box[0] + 8 * SCALE, box[1] + 3 * SCALE, box[2] - 8 * SCALE, box[1] + 3 * SCALE),
              fill="#FFF0B8", width=SCALE)
    draw.line((box[0] + 8 * SCALE, box[3] - 3 * SCALE, box[2] - 8 * SCALE, box[3] - 3 * SCALE),
              fill="#74502F", width=SCALE)
    text_center(draw, box, label, size, text_fill, True)


def input_box(draw: ImageDraw.ImageDraw, y: float, label: str, placeholder: str) -> None:
    box = rect_from_center(0, y, 284, 52)
    rounded(draw, box, "#FFFBF0", "#B88743", 1, 6)
    label_box = (box[0] + 12 * SCALE, box[1], box[0] + 68 * SCALE, box[3])
    value_box = (box[0] + 75 * SCALE, box[1], box[2] - 12 * SCALE, box[3])
    text_left(draw, label_box, label, 16, FOREST, True)
    text_left(draw, value_box, placeholder, 17, "#756C5D")


def login_review() -> Image.Image:
    canvas = cover(ART / "town-background-v3-portrait.png")
    wash = Image.new("RGBA", canvas.size, (44, 30, 18, 58))
    canvas.alpha_composite(wash)
    draw = ImageDraw.Draw(canvas)
    text_center(draw, rect_from_center(0, 306, 330, 44), "Wheat Town", 31, IVORY, True)
    text_center(draw, rect_from_center(0, 270, 360, 28), "Cozy Harvest · Town Life · Casual Growth", 15, "#FFF0CA")
    frame = rect_from_center(0, -60, 372, 612)
    paste_art(canvas, UI / "login_frame.png", frame)
    draw = ImageDraw.Draw(canvas)
    text_center(draw, rect_from_center(0, 180, 290, 42), "Guest Sign In", 28, IVORY, True)
    text_center(draw, rect_from_center(0, 102, 300, 28), "Letters and numbers only", 17, BODY)
    # Child positions include the LoginCard's -60 reference-space offset.
    input_box(draw, 42, "ID", "Account ID")
    input_box(draw, -22, "Pass", "Password")
    # Keep the complete agreement row within the parchment's inner safe area.
    checkbox = rect_from_center(-130, -86, 24, 24)
    rounded(draw, checkbox, "#FFF8E7", GOLD, 1, 4)
    text_left(draw, rect_from_center(25, -86, 250, 28), "I agree to Privacy and Terms", 15, DARK)
    button(draw, 0, -154, 276, 58, "Guest Login", ORANGE, IVORY, 18)
    button(draw, 0, -218, 190, 46, "Log in", "#FFF2D2", DARK, 16)
    button(draw, -82, -280, 150, 44, "Continue Save", "#2E4A2B", IVORY, 14)
    button(draw, 94, -280, 126, 44, "Log out", "#7E4630", IVORY, 14)
    return canvas


def hud(draw: ImageDraw.ImageDraw) -> None:
    top = rect_from_center(0, 435, 430, 62)
    draw.rectangle(top, fill="#193E2FEF")
    text_left(draw, rect_from_center(-142, 435, 92, 30), "Wheat", 16, IVORY, True)
    for x, label in [(-54, "5,433"), (38, "23"), (130, "7")]:
        box = rect_from_center(x, 435, 86, 38)
        rounded(draw, box, "#FFF4D8", "#C49349", 1, 6)
        text_center(draw, box, label, 16, DARK, True)


def task_review() -> Image.Image:
    canvas = cover(ART / "town-v7" / "town-main-clean-v7.png")
    draw = ImageDraw.Draw(canvas)
    hud(draw)
    header = rect_from_center(0, 292, 386, 60)
    rounded(draw, header, "#244B35", "#C7984A", 1, 8)
    text_left(draw, rect_from_center(-148, 292, 82, 34), "Tasks", 22, IVORY, True)
    for x, label, active in [(-66, "Orders", True), (18, "Friends", False), (102, "Route", False)]:
        box = rect_from_center(x, 292, 76, 36)
        rounded(draw, box, "#FFF8DE" if active else "#44624B", "#D2AC67", 1, 6)
        text_center(draw, box, label, 14, DARK if active else IVORY, True)
    panel = rect_from_center(0, 22, 366, 500)
    paste_art(canvas, UI / "task_panel.png", panel)
    draw = ImageDraw.Draw(canvas)
    text_center(draw, rect_from_center(0, 232, 250, 38), "Today Orders", 24, IVORY, True)
    for y, title, desc, ready in [(139, "Bakery Order", "Need Bread x1", False), (59, "Dairy Order", "Need Cheese x1", False)]:
        row = rect_from_center(0, y, 320, 72)
        rounded(draw, row, "#FFF8E8E8", "#B88743", 1, 8)
        text_left(draw, (row[0] + 18 * SCALE, row[1], row[2] - 88 * SCALE, row[1] + 37 * SCALE), title, 19, DARK, True)
        text_left(draw, (row[0] + 18 * SCALE, row[1] + 32 * SCALE, row[2] - 88 * SCALE, row[3]), desc, 17, BODY)
        bx = (row[2] - 82 * SCALE, row[1] + 9 * SCALE, row[2] - 8 * SCALE, row[3] - 9 * SCALE)
        rounded(draw, bx, MUTED if not ready else ORANGE, "#766D5F", 1, 7)
        text_center(draw, bx, "Wait" if not ready else "Send", 16, IVORY, True)
    text_center(draw, rect_from_center(0, -48, 304, 30), "Orders give coins and materials", 17, BODY)
    button(draw, 0, -120, 210, 52, "Open Album", ORANGE, IVORY, 17)
    button(draw, 0, -300, 230, 50, "Back to Town", "#2E4A2B", IVORY, 17)
    return canvas


def seed_review() -> Image.Image:
    canvas = cover(ART / "town-v7" / "town-main-clean-v7.png")
    canvas.alpha_composite(Image.new("RGBA", canvas.size, (18, 32, 24, 102)))
    draw = ImageDraw.Draw(canvas)
    hud(draw)
    frame = rect_from_center(0, -262, 402, 238)
    paste_art(canvas, UI / "seed_sheet.png", frame)
    draw = ImageDraw.Draw(canvas)
    text_center(draw, rect_from_center(0, -176, 230, 34), "Choose Seed", 23, IVORY, True)
    text_center(draw, rect_from_center(0, -207, 200, 24), "Plot 1", 15, "#6F522A", True)
    for x, name, meta, selected in [(-88, "Wheat", "12s · Free", True), (88, "Apple", "Unlock later", False)]:
        card = rect_from_center(x, -265, 154, 92)
        rounded(draw, card, "#FFF8E8F0", "#B88743", 1, 10)
        text_left(draw, (card[0] + 15 * SCALE, card[1] + 5 * SCALE, card[2] - 5 * SCALE, card[1] + 44 * SCALE), name, 18, DARK if selected else "#56564E", True)
        text_left(draw, (card[0] + 15 * SCALE, card[1] + 40 * SCALE, card[2] - 5 * SCALE, card[3] - 5 * SCALE), meta, 15, "#6F522A" if selected else "#756F61")
    button(draw, 0, -349, 220, 46, "Plant Wheat", ORANGE, IVORY, 17)
    button(draw, 174, -175, 38, 38, "X", "#7E4630", IVORY, 16)
    return canvas


def main() -> None:
    OUT.mkdir(parents=True, exist_ok=True)
    renders = {
        "01_login_1290x2796.png": login_review(),
        "02_tasks_1290x2796.png": task_review(),
        "03_seed_sheet_1290x2796.png": seed_review(),
    }
    for name, image in renders.items():
        path = OUT / name
        image.convert("RGB").save(path, quality=95)
        print(path)


if __name__ == "__main__":
    main()
