from pathlib import Path
from PIL import Image


ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / "assets/images/source/native-npc-v4-greenscreen.png"
OUTPUTS = [
    ROOT / "assets/images/native-v4",
    ROOT / "麦穗小镇_Unity工程/Assets/WheatTown/Art/Images/native-v4",
    ROOT / "unity_import/Assets/WheatTown/Art/Images/native-v4",
]

SPRITES = {
    "npc_mia_full.png": (0, 0, 512, 512),
    "npc_tom_full.png": (512, 0, 1024, 512),
    "icon_favor_heart.png": (0, 512, 512, 1024),
    "icon_commission_mark.png": (512, 512, 1024, 1024),
}


def remove_green(image: Image.Image) -> Image.Image:
    rgba = image.convert("RGBA")
    pixels = rgba.load()
    width, height = rgba.size
    for y in range(height):
        for x in range(width):
            r, g, b, a = pixels[x, y]
            # Chroma-key the generated green screen, including slightly anti-aliased edges.
            if g > 135 and g > r * 1.25 and g > b * 1.25:
                pixels[x, y] = (r, g, b, 0)
    return rgba


def trim(image: Image.Image, padding: int = 18) -> Image.Image:
    alpha = image.getchannel("A")
    bbox = alpha.getbbox()
    if not bbox:
        return image
    left, top, right, bottom = bbox
    left = max(0, left - padding)
    top = max(0, top - padding)
    right = min(image.width, right + padding)
    bottom = min(image.height, bottom + padding)
    return image.crop((left, top, right, bottom))


def main() -> None:
    source = Image.open(SOURCE)
    for out in OUTPUTS:
        out.mkdir(parents=True, exist_ok=True)

    for filename, box in SPRITES.items():
        sprite = trim(remove_green(source.crop(box)))
        for out in OUTPUTS:
            sprite.save(out / filename)
            print(out / filename)


if __name__ == "__main__":
    main()
