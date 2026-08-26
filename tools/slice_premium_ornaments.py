from pathlib import Path

from PIL import Image


ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / "assets/images/source/premium-ornament-sheet-greenscreen.png"
OUT_DIR = ROOT / "assets/images/premium-ornaments"

ITEMS = [
    ("wheat-corner.webp", 0, 0),
    ("wood-divider.webp", 1, 0),
    ("vine-corner.webp", 2, 0),
    ("gem-rivets.webp", 0, 1),
    ("cream-gold-label.webp", 1, 1),
    ("ribbon-cap.webp", 2, 1),
]


def remove_green_screen(image: Image.Image) -> Image.Image:
    image = image.convert("RGBA")
    pixels = image.load()
    width, height = image.size
    for y in range(height):
        for x in range(width):
            r, g, b, a = pixels[x, y]
            green_dominance = g - max(r, b)
            if g > 120 and green_dominance > 35:
                alpha = max(0, min(255, int((95 - green_dominance) * 2.8)))
                pixels[x, y] = (r, g, b, min(a, alpha))
            elif g > 90 and green_dominance > 12:
                pixels[x, y] = (r, max(0, g - 55), b, a)
    return image


def trim(image: Image.Image, pad: int = 10) -> Image.Image:
    alpha = image.getchannel("A")
    bbox = alpha.getbbox()
    if not bbox:
        return image
    left, top, right, bottom = bbox
    left = max(0, left - pad)
    top = max(0, top - pad)
    right = min(image.width, right + pad)
    bottom = min(image.height, bottom + pad)
    return image.crop((left, top, right, bottom))


def main() -> None:
    OUT_DIR.mkdir(parents=True, exist_ok=True)
    sheet = Image.open(SOURCE).convert("RGBA")
    cell_w = sheet.width // 3
    cell_h = sheet.height // 2
    for name, col, row in ITEMS:
        crop = sheet.crop((col * cell_w, row * cell_h, (col + 1) * cell_w, (row + 1) * cell_h))
        transparent = trim(remove_green_screen(crop), pad=12)
        transparent.save(OUT_DIR / name, "WEBP", quality=92, method=6)
        transparent.save(OUT_DIR / name.replace(".webp", ".png"), "PNG")
        print(OUT_DIR / name)


if __name__ == "__main__":
    main()
