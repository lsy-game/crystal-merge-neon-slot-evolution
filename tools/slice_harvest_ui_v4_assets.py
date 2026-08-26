from pathlib import Path
from PIL import Image


ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / "assets/images/source/harvest-ui-v4-greenscreen.png"
OUTPUTS = [
    ROOT / "assets/images/harvest-v4",
    ROOT / "麦穗小镇_Unity工程/Assets/WheatTown/Art/Images/harvest-v4",
    ROOT / "unity_import/Assets/WheatTown/Art/Images/harvest-v4",
]

SPRITES = {
    "harvest_console_frame.png": (0, 0, 512, 512),
    "harvest_cell_tile.png": (512, 0, 1024, 512),
    "harvest_energy_bar.png": (1024, 0, 1536, 512),
    "harvest_button_round.png": (0, 512, 512, 1024),
    "harvest_info_plaque.png": (512, 512, 1024, 1024),
    "harvest_back_plaque.png": (1024, 512, 1536, 1024),
}


def remove_green(image: Image.Image) -> Image.Image:
    rgba = image.convert("RGBA")
    pixels = rgba.load()
    for y in range(rgba.height):
        for x in range(rgba.width):
            r, g, b, a = pixels[x, y]
            if g > 125 and g > r * 1.18 and g > b * 1.18:
                pixels[x, y] = (r, g, b, 0)
    return rgba


def trim(image: Image.Image, padding: int = 18) -> Image.Image:
    bbox = image.getchannel("A").getbbox()
    if not bbox:
        return image
    left, top, right, bottom = bbox
    return image.crop((
        max(0, left - padding),
        max(0, top - padding),
        min(image.width, right + padding),
        min(image.height, bottom + padding),
    ))


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
