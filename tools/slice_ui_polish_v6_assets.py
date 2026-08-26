#!/usr/bin/env python3
from pathlib import Path
from collections import deque
from PIL import Image

ROOT = Path(__file__).resolve().parents[1]
SRC = ROOT / "assets/images/source/ui-polish-v6-greenscreen.png"
OUT_DIRS = [
    ROOT / "assets/images/ui-polish-v6",
    ROOT / "麦穗小镇_Unity工程/Assets/WheatTown/Art/Images/ui-polish-v6",
    ROOT / "unity_import/Assets/WheatTown/Art/Images/ui-polish-v6",
]

NAMES_BY_POSITION = [
    "v6_main_panel_frame",
    "v6_info_card_frame",
    "v6_title_plaque",
    "v6_primary_button",
    "v6_item_slot_frame",
    "v6_secondary_button",
    "v6_notification_badge",
    "v6_progress_bar_frame",
]


def is_green(r, g, b):
    return g > 180 and r < 90 and b < 90


def find_components(img):
    w, h = img.size
    px = img.load()
    mask = bytearray(w * h)
    for y in range(h):
        for x in range(w):
            r, g, b, a = px[x, y]
            if a > 0 and not is_green(r, g, b):
                mask[y * w + x] = 1

    seen = bytearray(w * h)
    boxes = []
    for y in range(h):
        for x in range(w):
            idx = y * w + x
            if not mask[idx] or seen[idx]:
                continue
            q = [(x, y)]
            seen[idx] = 1
            minx = maxx = x
            miny = maxy = y
            count = 0
            while q:
                cx, cy = q.pop()
                count += 1
                minx = min(minx, cx)
                maxx = max(maxx, cx)
                miny = min(miny, cy)
                maxy = max(maxy, cy)
                for nx, ny in ((cx + 1, cy), (cx - 1, cy), (cx, cy + 1), (cx, cy - 1)):
                    if 0 <= nx < w and 0 <= ny < h:
                        ni = ny * w + nx
                        if mask[ni] and not seen[ni]:
                            seen[ni] = 1
                            q.append((nx, ny))
            if count > 1000:
                pad = 8
                boxes.append((
                    max(0, minx - pad),
                    max(0, miny - pad),
                    min(w, maxx + 1 + pad),
                    min(h, maxy + 1 + pad),
                    count,
                ))
    return sorted(boxes, key=lambda b: (b[1], b[0]))


def chroma_key(crop):
    crop = crop.convert("RGBA")
    px = crop.load()
    w, h = crop.size
    for y in range(h):
        for x in range(w):
            r, g, b, a = px[x, y]
            if is_green(r, g, b):
                px[x, y] = (0, 0, 0, 0)
            elif g > 135 and g > r * 1.35 and g > b * 1.35:
                # Soft green spill cleanup around anti-aliased edges.
                alpha = max(0, min(255, int(a * (1 - min(1, (g - max(r, b)) / 170)))))
                px[x, y] = (r, min(g, max(r, b) + 18), b, alpha)
    return crop


def green_opaque_count(path):
    img = Image.open(path).convert("RGBA")
    return sum(
        1
        for r, g, b, a in img.getdata()
        if a > 8 and g > 180 and r < 90 and b < 90
    )


def main():
    img = Image.open(SRC).convert("RGBA")
    boxes = find_components(img)
    if len(boxes) != len(NAMES_BY_POSITION):
        raise SystemExit(f"Expected {len(NAMES_BY_POSITION)} components, found {len(boxes)}: {boxes}")

    for out_dir in OUT_DIRS:
        out_dir.mkdir(parents=True, exist_ok=True)

    for name, box in zip(NAMES_BY_POSITION, boxes):
        crop = chroma_key(img.crop(box[:4]))
        for out_dir in OUT_DIRS:
            crop.save(out_dir / f"{name}.png")

    for out_dir in OUT_DIRS:
        print(out_dir)
        for name in NAMES_BY_POSITION:
            path = out_dir / f"{name}.png"
            print(f"  {path.name}: {Image.open(path).size}, green_opaque={green_opaque_count(path)}")


if __name__ == "__main__":
    main()
