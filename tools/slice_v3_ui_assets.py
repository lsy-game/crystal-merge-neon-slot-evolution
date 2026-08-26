from pathlib import Path

from PIL import Image


ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / "assets/images/source/v3-ui-decor-greenscreen.png"
WEB_OUT = ROOT / "assets/images/v3-ui"
UNITY_OUT = ROOT / "麦穗小镇_Unity工程/Assets/WheatTown/Art/Images/v3-ui"


ITEMS = [
    ("v3_panel_ornate.png", (35, 15, 655, 510)),
    ("v3_dialog_scroll.png", (660, 82, 1030, 500)),
    ("v3_status_bar_green.png", (1018, 88, 1530, 230)),
    ("v3_nav_plaque_green.png", (1016, 250, 1510, 520)),
    ("v3_button_large_gold.png", (55, 510, 480, 680)),
    ("v3_button_small_gold.png", (480, 535, 740, 675)),
    ("v3_tab_left.png", (740, 535, 1000, 685)),
    ("v3_tab_right.png", (1008, 535, 1292, 685)),
    ("v3_corner_wheat_set.png", (60, 680, 410, 960)),
    ("v3_wood_divider.png", (440, 760, 1005, 900)),
    ("v3_settings_medallion.png", (1028, 700, 1295, 940)),
    ("v3_badge_red.png", (1290, 780, 1425, 905)),
]


def remove_green_screen(image: Image.Image) -> Image.Image:
    image = image.convert("RGBA")
    pixels = image.load()
    width, height = image.size
    for y in range(height):
        for x in range(width):
            r, g, b, a = pixels[x, y]
            green_dominance = g - max(r, b)
            if g > 150 and green_dominance > 55:
                pixels[x, y] = (r, g, b, 0)
            elif g > 115 and green_dominance > 25:
                alpha = max(0, min(255, int((105 - green_dominance) * 4.0)))
                pixels[x, y] = (r, max(0, g - 90), b, min(a, alpha))
    return image


def trim(image: Image.Image, pad: int = 10) -> Image.Image:
    bbox = image.getchannel("A").getbbox()
    if not bbox:
        return image
    left, top, right, bottom = bbox
    return image.crop(
        (
            max(0, left - pad),
            max(0, top - pad),
            min(image.width, right + pad),
            min(image.height, bottom + pad),
        )
    )


def main() -> None:
    WEB_OUT.mkdir(parents=True, exist_ok=True)
    UNITY_OUT.mkdir(parents=True, exist_ok=True)
    sheet = Image.open(SOURCE).convert("RGBA")
    for name, box in ITEMS:
        sprite = trim(remove_green_screen(sheet.crop(box)), pad=14)
        for out_dir in (WEB_OUT, UNITY_OUT):
            out = out_dir / name
            sprite.save(out)
            print(out)


if __name__ == "__main__":
    main()
