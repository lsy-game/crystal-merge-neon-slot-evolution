from pathlib import Path
from PIL import Image, ImageFilter

ROOT = Path(__file__).resolve().parents[1]
IMAGES = ROOT / "assets" / "images"


def remove_green_screen(image: Image.Image) -> Image.Image:
    """Remove only bright chroma green, preserve natural sage/leaf greens, and defringe edges."""
    image = image.convert("RGBA")
    output = Image.new("RGBA", image.size)
    cleaned = []
    for red, green, blue, _ in image.getdata():
        dominance = green - max(red, blue)
        brightness = green
        if brightness > 175 and dominance > 70 and green > red * 1.45 and green > blue * 1.45:
            strength = min(1.0, max(0.0, (dominance - 70) / 90))
            strength = max(strength, min(1.0, (brightness - 175) / 70))
            alpha = round(255 * (1.0 - strength))
            if alpha < 12:
                cleaned.append((0, 0, 0, 0))
                continue
            # Remove reflected chroma from antialiased edge pixels.
            green = min(green, round(max(red, blue) * 1.12))
            cleaned.append((red, green, blue, alpha))
        else:
            cleaned.append((red, green, blue, 255))
    output.putdata(cleaned)
    # Neutralize remaining green spill only on pixels touching transparency.
    alpha_channel = output.getchannel("A")
    nearby_transparency = alpha_channel.filter(ImageFilter.MinFilter(5))
    final_pixels = []
    for (red, green, blue, alpha), neighborhood_alpha in zip(output.getdata(), nearby_transparency.getdata()):
        if alpha and neighborhood_alpha < 240 and green > max(red, blue) + 18 and green > 105:
            green = min(green, round(max(red, blue) * 1.06))
        final_pixels.append((red, green, blue, alpha))
    output.putdata(final_pixels)
    return output


def crop_grid(
    source: Path,
    columns: int,
    rows: int,
    names: list[str],
    out_dir: Path,
    chroma_key: bool = False,
    trim: bool = False,
) -> None:
    image = Image.open(source).convert("RGBA")
    if chroma_key:
        image = remove_green_screen(image)
    out_dir.mkdir(parents=True, exist_ok=True)
    width, height = image.size
    for index, name in enumerate(names):
        column, row = index % columns, index // columns
        left = round(column * width / columns)
        right = round((column + 1) * width / columns)
        top = round(row * height / rows)
        bottom = round((row + 1) * height / rows)
        cell = image.crop((left, top, right, bottom))
        if trim:
            bounds = cell.getchannel("A").getbbox()
            if bounds:
                padding = 4
                bounds = (
                    max(0, bounds[0] - padding),
                    max(0, bounds[1] - padding),
                    min(cell.width, bounds[2] + padding),
                    min(cell.height, bounds[3] + padding),
                )
                cell = cell.crop(bounds)
        cell.save(out_dir / f"{name}.webp", "WEBP", quality=94, method=6)


crop_grid(
    IMAGES / "source" / "symbol-sheet-greenscreen.png",
    4,
    2,
    ["wheat", "apple", "milk", "bread", "gem", "wild", "scatter", "gift"],
    IMAGES / "symbols",
    chroma_key=True,
)

crop_grid(
    IMAGES / "source" / "town-sheet-greenscreen.png",
    3,
    2,
    ["bakery", "market", "workshop", "vault", "lighthouse", "mia"],
    IMAGES / "buildings",
    chroma_key=True,
)

crop_grid(
    IMAGES / "source" / "ui-frame-sheet-greenscreen.png",
    3,
    2,
    ["slot-frame", "panel-frame", "topbar-frame", "button-frame", "progress-frame", "card-frame"],
    IMAGES / "ui",
    chroma_key=True,
    trim=True,
)

crop_grid(
    IMAGES / "source" / "mobile-hud-sheet-greenscreen.png",
    3,
    2,
    ["resource-pill", "utility-button", "spin-ring", "reel-window", "nav-plate", "info-panel"],
    IMAGES / "mobile-ui",
    chroma_key=True,
    trim=True,
)

crop_grid(
    IMAGES / "source" / "journey-ui-sheet-greenscreen.png",
    3,
    2,
    ["order-board", "harvest-chest", "event-scroll", "milestone-medal", "summary-ledger", "journey-sign"],
    IMAGES / "journey-ui",
    chroma_key=True,
    trim=True,
)

premium_icon_source = IMAGES / "source" / "premium-icon-sheet-greenscreen.png"
if premium_icon_source.exists():
    crop_grid(
        premium_icon_source,
        4,
        2,
        ["coin", "wood", "ore", "settings", "slot", "journey", "town", "gift"],
        IMAGES / "premium-icons",
        chroma_key=True,
        trim=True,
    )

premium_frame_source = IMAGES / "source" / "premium-frame-sheet-greenscreen.png"
if premium_frame_source.exists():
    crop_grid(
        premium_frame_source,
        3,
        2,
        ["resource-pill", "title-plaque", "utility-button", "reel-frame", "card-frame", "nav-tab"],
        IMAGES / "premium-ui",
        chroma_key=True,
        trim=True,
    )

premium_polish_source = IMAGES / "source" / "premium-polish-sheet-greenscreen.png"
if premium_polish_source.exists():
    crop_grid(
        premium_polish_source,
        3,
        2,
        ["harvest-console", "info-card", "dialogue-card", "symbol-tile", "status-strip", "primary-button"],
        IMAGES / "premium-polish",
        chroma_key=True,
        trim=True,
    )

# The generated ledger sits close to the next grid cell. Remove the isolated
# ornament fragment at the far-right edge before shipping the keyed asset.
ledger_path = IMAGES / "journey-ui" / "summary-ledger.webp"
ledger = Image.open(ledger_path).convert("RGBA")
ledger_pixels = list(ledger.getdata())
cutoff = round(ledger.width * 0.92)
for y in range(ledger.height):
    for x in range(cutoff, ledger.width):
        index = y * ledger.width + x
        red, green, blue, alpha = ledger_pixels[index]
        ledger_pixels[index] = (red, green, blue, 0)
ledger.putdata(ledger_pixels)
ledger_bounds = ledger.getchannel("A").getbbox()
if ledger_bounds:
    ledger = ledger.crop(ledger_bounds)
ledger.save(ledger_path, "WEBP", quality=94, method=6)

background = Image.open(IMAGES / "town-background.png").convert("RGB")
background.save(IMAGES / "town-background.webp", "WEBP", quality=88, method=6)

portrait_background = Image.open(IMAGES / "town-scene-portrait.png").convert("RGB")
portrait_background.save(IMAGES / "town-scene-portrait.webp", "WEBP", quality=90, method=6)

slot_background = Image.open(IMAGES / "slot-background-portrait.png").convert("RGB")
slot_background.save(IMAGES / "slot-background-portrait.webp", "WEBP", quality=90, method=6)

premium_slot_background = IMAGES / "premium-slot-background.png"
if premium_slot_background.exists():
    background = Image.open(premium_slot_background).convert("RGB")
    background.save(IMAGES / "premium-slot-background.webp", "WEBP", quality=92, method=6)

lobby_background = IMAGES / "lobby-background.png"
if lobby_background.exists():
    background = Image.open(lobby_background).convert("RGB")
    background.save(IMAGES / "lobby-background.webp", "WEBP", quality=92, method=6)

print("Generated game-ready WebP assets in", IMAGES)
