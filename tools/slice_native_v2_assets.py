from pathlib import Path

from PIL import Image


ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / "assets/images/source/native-v2-farm-sheet-greenscreen.png"
WEB_OUT = ROOT / "assets/images/native-v2"
UNITY_OUT = ROOT / "麦穗小镇_Unity工程/Assets/WheatTown/Art/Images/native-v2"

ITEMS = [
    ("plot_empty.png", 0, 0),
    ("plot_growing_1.png", 1, 0),
    ("plot_growing_2.png", 2, 0),
    ("plot_ready_wheat.png", 3, 0),
    ("plot_ready_apple.png", 0, 1),
    ("building_dairy.png", 1, 1),
    ("house_mia.png", 2, 1),
    ("house_tom.png", 3, 1),
    ("machine_harvest.png", 0, 2),
    ("board_orders.png", 1, 2),
    ("icon_bag.png", 2, 2),
    ("icon_quest.png", 3, 2),
    ("npc_tom_idle.png", 0, 3),
    ("icon_complete_bubble.png", 1, 3),
    ("icon_lock.png", 2, 3),
    ("icon_sickle.png", 3, 3),
]


def remove_green_screen(image: Image.Image) -> Image.Image:
    image = image.convert("RGBA")
    pixels = image.load()
    width, height = image.size
    for y in range(height):
        for x in range(width):
            r, g, b, a = pixels[x, y]
            green_dominance = g - max(r, b)
            if g > 135 and green_dominance > 45:
                pixels[x, y] = (r, g, b, 0)
            elif g > 100 and green_dominance > 18:
                alpha = max(0, min(255, int((100 - green_dominance) * 3.0)))
                pixels[x, y] = (r, max(0, g - 70), b, min(a, alpha))
    return image


def trim(image: Image.Image, pad: int = 10) -> Image.Image:
    bbox = image.getchannel("A").getbbox()
    if not bbox:
        return image
    left, top, right, bottom = bbox
    return image.crop((
        max(0, left - pad),
        max(0, top - pad),
        min(image.width, right + pad),
        min(image.height, bottom + pad),
    ))


def main() -> None:
    WEB_OUT.mkdir(parents=True, exist_ok=True)
    UNITY_OUT.mkdir(parents=True, exist_ok=True)
    sheet = Image.open(SOURCE).convert("RGBA")
    cell_w = sheet.width // 4
    cell_h = sheet.height // 4
    for name, col, row in ITEMS:
        crop = sheet.crop((col * cell_w, row * cell_h, (col + 1) * cell_w, (row + 1) * cell_h))
        sprite = trim(remove_green_screen(crop), pad=12)
        for out_dir in (WEB_OUT, UNITY_OUT):
            out = out_dir / name
            sprite.save(out)
            print(out)


if __name__ == "__main__":
    main()
