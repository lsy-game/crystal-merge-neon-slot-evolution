#!/usr/bin/env python3
"""Turn AI chroma-green UI renders into cropped, premultiplied-safe RGBA PNGs."""

from collections import deque
from pathlib import Path
import sys

from PIL import Image


def is_green(pixel: tuple[int, int, int, int]) -> bool:
    r, g, b, _ = pixel
    return g >= 115 and g > r * 1.10 and g > b * 1.18 and g - min(r, b) >= 44


def chroma_key(source: Path, destination: Path, padding: int = 12) -> None:
    image = Image.open(source).convert("RGBA")
    width, height = image.size
    pixels = image.load()
    connected = bytearray(width * height)
    queue: deque[tuple[int, int]] = deque()

    def add(x: int, y: int) -> None:
        index = y * width + x
        if connected[index] or not is_green(pixels[x, y]):
            return
        connected[index] = 1
        queue.append((x, y))

    for x in range(width):
        add(x, 0)
        add(x, height - 1)
    for y in range(height):
        add(0, y)
        add(width - 1, y)

    while queue:
        x, y = queue.popleft()
        if x:
            add(x - 1, y)
        if x + 1 < width:
            add(x + 1, y)
        if y:
            add(x, y - 1)
        if y + 1 < height:
            add(x, y + 1)

    output = Image.new("RGBA", image.size, (0, 0, 0, 0))
    out = output.load()
    for y in range(height):
        for x in range(width):
            r, g, b, _ = pixels[x, y]
            if not connected[y * width + x]:
                out[x, y] = (r, g, b, 255)
                continue

            # Recover foreground colour from a pixel composited over RGB(0,255,0).
            alpha = max(r, b) / 255.0
            # The image model leaves low-level blue/red texture in the green plate.
            # Treat it as background; retaining it produces the familiar purple halo.
            if alpha <= 0.18:
                out[x, y] = (0, 0, 0, 0)
                continue
            rr = int(max(0, min(255, round(r / alpha))))
            bb = int(max(0, min(255, round(b / alpha))))
            gg = int(max(0, min(255, round((g - (1.0 - alpha) * 255.0) / alpha))))
            out[x, y] = (rr, gg, bb, int(round(alpha * 255.0)))

    bbox = output.getbbox()
    if bbox is None:
        raise RuntimeError(f"No foreground found in {source}")
    left = max(0, bbox[0] - padding)
    top = max(0, bbox[1] - padding)
    right = min(width, bbox[2] + padding)
    bottom = min(height, bbox[3] + padding)
    output = output.crop((left, top, right, bottom))
    destination.parent.mkdir(parents=True, exist_ok=True)
    output.save(destination, optimize=True)
    print(f"{source.name}: {width}x{height} -> {output.width}x{output.height} RGBA")


def main() -> None:
    if len(sys.argv) != 3:
        raise SystemExit("usage: chroma_key_ui.py SOURCE DESTINATION")
    chroma_key(Path(sys.argv[1]), Path(sys.argv[2]))


if __name__ == "__main__":
    main()
