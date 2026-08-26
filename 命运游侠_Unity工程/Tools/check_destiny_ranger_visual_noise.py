#!/usr/bin/env python3
"""Detect large warm/yellow gameplay plates in Destiny Ranger preview PNGs.

The gameplay art can contain warm stones, foliage, coins, and reward accents.
This check is intentionally narrow: it flags only high-saturation warm/orange
connected components that are large enough to read as a pasted background plate.
"""

from __future__ import annotations

import argparse
import struct
import sys
import zlib
from collections import deque
from pathlib import Path


PNG_SIGNATURE = b"\x89PNG\r\n\x1a\n"


def read_png_rgba(path: Path) -> tuple[int, int, bytearray]:
    data = path.read_bytes()
    if not data.startswith(PNG_SIGNATURE):
        raise ValueError(f"{path} is not a PNG")

    offset = len(PNG_SIGNATURE)
    width = height = None
    color_type = None
    bit_depth = None
    compressed = bytearray()

    while offset < len(data):
        if offset + 8 > len(data):
            raise ValueError("truncated PNG chunk")
        length = struct.unpack(">I", data[offset : offset + 4])[0]
        chunk_type = data[offset + 4 : offset + 8]
        chunk_data = data[offset + 8 : offset + 8 + length]
        offset += 12 + length

        if chunk_type == b"IHDR":
            width, height, bit_depth, color_type = struct.unpack(">IIBB", chunk_data[:10])
        elif chunk_type == b"IDAT":
            compressed.extend(chunk_data)
        elif chunk_type == b"IEND":
            break

    if width is None or height is None:
        raise ValueError("missing PNG IHDR")
    if bit_depth != 8 or color_type not in (2, 6):
        raise ValueError(f"unsupported PNG format: bit_depth={bit_depth}, color_type={color_type}")

    channels = 4 if color_type == 6 else 3
    stride = width * channels
    raw = zlib.decompress(bytes(compressed))
    rows: list[bytearray] = []
    cursor = 0
    prev = bytearray(stride)

    for _ in range(height):
        filter_type = raw[cursor]
        cursor += 1
        row = bytearray(raw[cursor : cursor + stride])
        cursor += stride
        unfilter(row, prev, channels, filter_type)
        rows.append(row)
        prev = row

    rgba = bytearray(width * height * 4)
    dst = 0
    for row in rows:
        for x in range(width):
            src = x * channels
            rgba[dst : dst + 3] = row[src : src + 3]
            rgba[dst + 3] = row[src + 3] if channels == 4 else 255
            dst += 4
    return width, height, rgba


def unfilter(row: bytearray, prev: bytearray, bpp: int, filter_type: int) -> None:
    for i in range(len(row)):
        left = row[i - bpp] if i >= bpp else 0
        up = prev[i]
        upper_left = prev[i - bpp] if i >= bpp else 0
        if filter_type == 0:
            value = row[i]
        elif filter_type == 1:
            value = row[i] + left
        elif filter_type == 2:
            value = row[i] + up
        elif filter_type == 3:
            value = row[i] + ((left + up) >> 1)
        elif filter_type == 4:
            value = row[i] + paeth(left, up, upper_left)
        else:
            raise ValueError(f"unsupported PNG filter {filter_type}")
        row[i] = value & 255


def paeth(a: int, b: int, c: int) -> int:
    p = a + b - c
    pa = abs(p - a)
    pb = abs(p - b)
    pc = abs(p - c)
    if pa <= pb and pa <= pc:
        return a
    return b if pb <= pc else c


def warm_plate_pixel(r: int, g: int, b: int, a: int) -> bool:
    if a < 180:
        return False
    # Saturated yellow/orange AI backdrops and attack plates. Natural stone and
    # foliage usually fail the saturation or blue-channel tests.
    return (
        r >= 178
        and 104 <= g <= 224
        and b <= 92
        and r + 18 >= g
        and g >= b + 38
        and max(r, g, b) - min(r, g, b) >= 86
    )


def generated_backdrop_plate_pixel(r: int, g: int, b: int, a: int) -> bool:
    if a < 210:
        return False
    # Cream/yellow-green generated backdrops often survive as flat rectangles
    # even when they are less saturated than pure yellow/orange plates.
    cream = (
        r >= 214
        and g >= 194
        and 104 <= b <= 166
        and abs(r - g) <= 42
        and r + g >= b * 2 + 128
    )
    yellow_green = (
        r >= 188
        and g >= 180
        and g <= 232
        and b <= 112
        and r + 28 >= g
        and g >= b + 58
    )
    return cream or yellow_green


def ignored_ui_zone(x: int, y: int, width: int, height: int) -> bool:
    # Keep reward/HUD gold out of the gameplay plate audit.
    if y < int(height * 0.12):
        return True
    if y > int(height * 0.86):
        return True
    # Mobile joystick and lower-left tutorial labels are warm and semi-opaque
    # but they are UI, not pasted gameplay plates.
    if x < int(width * 0.18) and y > int(height * 0.60):
        return True
    if x > int(width * 0.86) and y > int(height * 0.68):
        return True
    return False


def find_warm_components(width: int, height: int, rgba: bytearray) -> list[dict[str, float]]:
    mask = bytearray(width * height)
    for y in range(height):
        for x in range(width):
            if ignored_ui_zone(x, y, width, height):
                continue
            i = (y * width + x) * 4
            if warm_plate_pixel(rgba[i], rgba[i + 1], rgba[i + 2], rgba[i + 3]) or generated_backdrop_plate_pixel(rgba[i], rgba[i + 1], rgba[i + 2], rgba[i + 3]):
                mask[y * width + x] = 1

    seen = bytearray(width * height)
    components: list[dict[str, float]] = []
    for start in range(width * height):
        if not mask[start] or seen[start]:
            continue
        queue = deque([start])
        seen[start] = 1
        area = 0
        min_x = max_x = start % width
        min_y = max_y = start // width
        while queue:
            node = queue.popleft()
            x = node % width
            y = node // width
            area += 1
            min_x = min(min_x, x)
            max_x = max(max_x, x)
            min_y = min(min_y, y)
            max_y = max(max_y, y)
            for nx, ny in ((x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1)):
                if nx < 0 or nx >= width or ny < 0 or ny >= height:
                    continue
                ni = ny * width + nx
                if mask[ni] and not seen[ni]:
                    seen[ni] = 1
                    queue.append(ni)

        box_w = max_x - min_x + 1
        box_h = max_y - min_y + 1
        fill = area / float(max(1, box_w * box_h))
        components.append(
            {
                "area": area,
                "x": min_x,
                "y": min_y,
                "w": box_w,
                "h": box_h,
                "fill": fill,
            }
        )
    return components


def ignored_natural_floor_band(component: dict[str, float], width: int, height: int) -> bool:
    # Rune preview keeps the painted floor visible behind the modal. Its warm,
    # thin baseline can be saturated enough to look like a plate to the pixel
    # heuristic, but it is not a generated UI/VFX rectangle.
    return (
        component["y"] > height * 0.80
        and component["h"] < height * 0.04
        and component["w"] < width * 0.32
        and component["fill"] < 0.62
    )


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("png", type=Path)
    parser.add_argument("--max-area", type=int, default=9000)
    parser.add_argument("--min-fill", type=float, default=0.34)
    args = parser.parse_args()

    width, height, rgba = read_png_rgba(args.png)
    components = find_warm_components(width, height, rgba)
    offenders = [
        c
        for c in components
        if c["area"] > args.max_area
        and c["fill"] >= args.min_fill
        and c["w"] > 70
        and c["h"] > 28
        and not ignored_natural_floor_band(c, width, height)
    ]
    if offenders:
        print("warm/yellow gameplay plate candidates:")
        for c in sorted(offenders, key=lambda item: item["area"], reverse=True)[:8]:
            print(
                f"- area={int(c['area'])} box={int(c['w'])}x{int(c['h'])} "
                f"at ({int(c['x'])},{int(c['y'])}) fill={c['fill']:.2f}"
            )
        return 1

    largest = max((c["area"] for c in components), default=0)
    print(f"Destiny Ranger visual noise QA passed. largest_warm_component={largest}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
