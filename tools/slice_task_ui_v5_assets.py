from pathlib import Path
from PIL import Image

ROOT = Path("/Users/zhendian/Documents/New project")
SRC = ROOT / "assets/images/source/task-ui-v5-greenscreen.png"
OUTS = [
    ROOT / "assets/images/task-v5",
    ROOT / "麦穗小镇_Unity工程/Assets/WheatTown/Art/Images/task-v5",
    ROOT / "unity_import/Assets/WheatTown/Art/Images/task-v5",
]

ASSETS = {
    "task_order_board_frame.png": (0, 0, 512, 512),
    "task_commission_envelope.png": (512, 0, 1024, 512),
    "task_daily_route_scroll.png": (1024, 0, 1536, 512),
    "task_quest_row_frame.png": (0, 512, 512, 1024),
    "task_milestone_badge.png": (512, 512, 1024, 1024),
    "task_collection_book.png": (1024, 512, 1536, 1024),
}


def chroma_key_and_crop(img: Image.Image) -> Image.Image:
    rgba = img.convert("RGBA")
    pixels = rgba.load()
    width, height = rgba.size
    for y in range(height):
        for x in range(width):
            r, g, b, a = pixels[x, y]
            if g > 150 and r < 100 and b < 100 and g > r * 1.7 and g > b * 1.7:
                pixels[x, y] = (0, 255, 0, 0)
    bbox = rgba.getbbox()
    if not bbox:
        return rgba
    left = max(0, bbox[0] - 18)
    top = max(0, bbox[1] - 18)
    right = min(width, bbox[2] + 18)
    bottom = min(height, bbox[3] + 18)
    return rgba.crop((left, top, right, bottom))


def main():
    source = Image.open(SRC)
    for out in OUTS:
        out.mkdir(parents=True, exist_ok=True)
    for name, box in ASSETS.items():
        asset = chroma_key_and_crop(source.crop(box))
        for out in OUTS:
            asset.save(out / name)
            print(out / name)


if __name__ == "__main__":
    main()
