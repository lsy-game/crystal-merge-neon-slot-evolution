#!/usr/bin/env python3
from __future__ import annotations

import json
import math
import shutil
import wave
from pathlib import Path

import numpy as np
from PIL import Image, ImageChops, ImageDraw, ImageEnhance, ImageFilter, ImageFont, ImageOps

PROJECT = Path("/Users/zhendian/Documents/New project/命运游侠_Unity工程")
ASSETS = PROJECT / "Assets"
ART = ASSETS / "DestinyRanger/Art"
GEN = ART / "Generated"
FW = GEN / "FateWeaverFull"
COMMON = ART / "Common"
DATA = ASSETS / "DestinyRanger/Data"
AUDIO = ASSETS / "Audio"
FONTS = ASSETS / "Fonts"
DOCS = ASSETS / "DestinyRanger/Docs"

CHAMBER_BG = GEN / "fate-weaver-chamber-bg.png"
FOREST_BG = GEN / "fate-weaver-battle-forest-bg.png"
ATLAS = GEN / "fate-weaver-fusion-atlas-reference.png"

SPRITE_META = """fileFormatVersion: 2
guid: {guid}
TextureImporter:
  internalIDToNameTable: []
  externalObjects: {{}}
  serializedVersion: 13
  mipmaps:
    mipMapMode: 0
    enableMipMap: 0
    sRGBTexture: 1
    linearTexture: 0
    fadeOut: 0
    borderMipMap: 0
    mipMapsPreserveCoverage: 0
    alphaTestReferenceValue: 0.5
    mipMapFadeDistanceStart: 1
    mipMapFadeDistanceEnd: 3
  bumpmap:
    convertToNormalMap: 0
    externalNormalMap: 0
    heightScale: 0.25
    normalMapFilter: 0
    flipGreenChannel: 0
  isReadable: 1
  streamingMipmaps: 0
  streamingMipmapsPriority: 0
  vTOnly: 0
  ignoreMipmapLimit: 0
  grayScaleToAlpha: 0
  generateCubemap: 6
  cubemapConvolution: 0
  seamlessCubemap: 0
  textureFormat: 1
  maxTextureSize: 4096
  textureSettings:
    serializedVersion: 2
    filterMode: 1
    aniso: 1
    mipBias: 0
    wrapU: 1
    wrapV: 1
    wrapW: 1
  nPOTScale: 0
  lightmap: 0
  compressionQuality: 50
  spriteMode: 1
  spriteExtrude: 1
  spriteMeshType: 1
  alignment: 0
  spritePivot: {{x: 0.5, y: 0.5}}
  spriteBorder: {{x: {border}, y: {border}, z: {border}, w: {border}}}
  spriteGenerateFallbackPhysicsShape: 1
  alphaUsage: 1
  alphaIsTransparency: 1
  spriteTessellationDetail: -1
  textureType: 8
  textureShape: 1
  singleChannelComponent: 0
  flipbookRows: 1
  flipbookColumns: 1
  maxTextureSizeSet: 1
  compressionQualitySet: 0
  textureFormatSet: 0
  ignorePngGamma: 0
  applyGammaDecoding: 0
  swizzle: 50462976
  cookieLightType: 0
  platformSettings:
  - serializedVersion: 3
    buildTarget: DefaultTexturePlatform
    maxTextureSize: 4096
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 1
    compressionQuality: 50
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 0
    ignorePlatformSupport: 0
    androidETC2FallbackOverride: 0
    forceMaximumCompressionQuality_BC6H_BC7: 0
  spriteSheet:
    serializedVersion: 2
    sprites: []
    outline: []
    physicsShape: []
    bones: []
    spriteID: 5e97eb03825dee720800000000000000
    internalID: 0
    vertices: []
    indices: 
    edges: []
    weights: []
    secondaryTextures: []
    nameFileIdTable: {{}}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""

PROFILE_META = """fileFormatVersion: 2
guid: {guid}
NativeFormatImporter:
  externalObjects: {{}}
  mainObjectFileID: 11400000
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""

FURNITURE_SCRIPT_GUID = "763754d325154e4ab4ce963637e4ea49"
PROFILE_SCRIPT_GUID = "3a4917fa2ce74d1c98582922a8d3e301"


def stable_guid(key: str) -> str:
    import hashlib
    return hashlib.md5(key.encode("utf-8")).hexdigest()


def mkdirs():
    for d in [
        FW / "Backgrounds",
        FW / "Characters",
        FW / "Monsters/Forest",
        FW / "SlotMachine",
        FW / "Symbols",
        FW / "UI/BottomMenu",
        FW / "UI/Currency",
        FW / "UI/MapNodes",
        FW / "UI/Panels",
        FW / "UI/Buttons",
        FW / "UI/Bars",
        FW / "Furniture",
        COMMON,
        DATA / "SceneToneProfiles",
        DATA / "Furniture",
        AUDIO / "SFX/SlotMachine",
        AUDIO / "SFX/Combat",
        AUDIO / "SFX/UI",
        AUDIO / "Ambient",
        FONTS,
        DOCS,
    ]:
        d.mkdir(parents=True, exist_ok=True)


def save_png(path: Path, image: Image.Image, border: int = 0):
    path.parent.mkdir(parents=True, exist_ok=True)
    image.convert("RGBA").save(path)
    path.with_suffix(path.suffix + ".meta").write_text(SPRITE_META.format(guid=stable_guid(str(path)), border=border), encoding="utf-8")


def feather(image: Image.Image, strength=.38) -> Image.Image:
    image = image.convert("RGBA")
    a = image.getchannel("A")
    eroded = a.filter(ImageFilter.MinFilter(5))
    edge = Image.eval(ImageChops.subtract(a, eroded), lambda v: int(v * strength))
    image.putalpha(ImageChops.subtract(a, edge))
    return image


def fit(image: Image.Image, size: tuple[int, int], pad=0) -> Image.Image:
    image = image.convert("RGBA")
    bbox = image.getchannel("A").getbbox()
    if bbox:
        image = image.crop(bbox)
    image.thumbnail((size[0] - pad * 2, size[1] - pad * 2), Image.Resampling.LANCZOS)
    out = Image.new("RGBA", size, (0, 0, 0, 0))
    out.alpha_composite(image, ((size[0] - image.width) // 2, (size[1] - image.height) // 2))
    return feather(out)


def tint(image: Image.Image, color: tuple[int, int, int], strength: float, sat=1.0) -> Image.Image:
    arr = np.array(image.convert("RGBA")).astype(np.float32)
    alpha = arr[:, :, 3:4] / 255.0
    rgb = arr[:, :, :3]
    gray = rgb[:, :, 0:1] * .2126 + rgb[:, :, 1:2] * .7152 + rgb[:, :, 2:3] * .0722
    rgb = gray + (rgb - gray) * sat
    rgb = rgb * (1 - strength * alpha) + np.array(color, dtype=np.float32) * (strength * alpha)
    arr[:, :, :3] = rgb
    return Image.fromarray(np.clip(arr, 0, 255).astype(np.uint8), "RGBA")


def matte_to_alpha(image: Image.Image, threshold=34) -> Image.Image:
    image = image.convert("RGBA")
    rgb = np.array(image.convert("RGB")).astype(np.int16)
    corners = np.concatenate([rgb[:10, :10].reshape(-1, 3), rgb[:10, -10:].reshape(-1, 3), rgb[-10:, :10].reshape(-1, 3), rgb[-10:, -10:].reshape(-1, 3)])
    matte = np.median(corners, axis=0)
    dist = np.sqrt(((rgb - matte) ** 2).sum(axis=2))
    alpha = np.clip((dist - threshold) * 6.4, 0, 255).astype(np.uint8)
    result = image.copy()
    result.putalpha(Image.fromarray(alpha, "L").filter(ImageFilter.GaussianBlur(.7)))
    return result


def shadow(size, opacity=.45, stretch=1.0):
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    d = ImageDraw.Draw(img, "RGBA")
    w, h = size
    ew, eh = int(w * .75), int(h * .34 * stretch)
    d.ellipse(((w - ew) // 2, int(h * .45), (w + ew) // 2, int(h * .45) + eh), fill=(0, 0, 0, int(255 * opacity)))
    return img.filter(ImageFilter.GaussianBlur(max(6, w // 30)))


def normalize_backgrounds():
    chamber = Image.open(CHAMBER_BG).convert("RGBA").resize((1290, 2796), Image.Resampling.LANCZOS)
    forest_src = Image.open(FOREST_BG).convert("RGBA")
    forest = ImageOps.fit(forest_src, (1290, 1398), method=Image.Resampling.LANCZOS, centering=(.5, .38))
    save_png(FW / "Backgrounds/fate-weaver-chamber-bg_1290x2796.png", chamber)
    save_png(FW / "Backgrounds/fate-weaver-battle-forest-bg_1290x1398.png", forest)
    save_png(GEN / "fate-weaver-chamber-bg.png", chamber)
    save_png(GEN / "fate-weaver-battle-forest-bg.png", forest)
    for name, main, accent in [
        ("fate-weaver-battle-volcano-bg_1290x1398.png", (60, 30, 20), (210, 74, 30)),
        ("fate-weaver-battle-void-boss-bg_1290x1398.png", (40, 20, 40), (150, 80, 210)),
    ]:
        img = Image.new("RGBA", (1290, 1398), (*main, 255))
        d = ImageDraw.Draw(img, "RGBA")
        for y in range(1398):
            t = y / 1398
            c = tuple(int(main[i] * (1 - t) + (5, 8, 20)[i] * t) for i in range(3))
            d.line((0, y, 1290, y), fill=(*c, 255))
        for i in range(18):
            x = (i * 197) % 1290
            y = 260 + (i * 137) % 760
            d.polygon([(x, y + 320), (x + 120, y), (x + 260, y + 330)], fill=(20, 18, 24, 160))
            d.line((x + 120, y + 20, x + 140, y + 310), fill=(*accent, 160), width=5)
        for i in range(70):
            x = (i * 89) % 1290
            y = (i * 131) % 1398
            r = 2 + i % 6
            d.ellipse((x, y, x + r, y + r), fill=(*accent, 90))
        save_png(FW / "Backgrounds" / name, img)
        save_png(GEN / name.replace("_1290x1398", ""), img)


def scene_profiles():
    specs = [
        ("ChamberToneProfile", (20, 25, 50), (5, 8, 20), .10, .40, (226, 180, 86), "织室"),
        ("ForestToneProfile", (40, 60, 30), (10, 20, 5), .15, .50, (115, 185, 172), "森林"),
        ("VolcanoToneProfile", (60, 30, 20), (20, 5, 5), .12, .45, (220, 86, 42), "火山"),
        ("VoidToneProfile", (40, 20, 40), (15, 5, 15), .18, .55, (170, 100, 220), "虚空"),
    ]
    for name, main, sh, strength, op, hi, cn in specs:
        path = DATA / f"SceneToneProfiles/{name}.asset"
        content = f"""%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 0}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {PROFILE_SCRIPT_GUID}, type: 3}}
  m_Name: {name}
  m_EditorClassIdentifier: 
  sceneMainColor: {{r: {main[0]/255:.7f}, g: {main[1]/255:.7f}, b: {main[2]/255:.7f}, a: 1}}
  sceneShadowColor: {{r: {sh[0]/255:.7f}, g: {sh[1]/255:.7f}, b: {sh[2]/255:.7f}, a: 1}}
  colorOverlayStrength: {strength}
  shadowOpacity: {op}
  ambientTint: {{r: {main[0]/255:.7f}, g: {main[1]/255:.7f}, b: {main[2]/255:.7f}, a: 1}}
  highlightTint: {{r: {hi[0]/255:.7f}, g: {hi[1]/255:.7f}, b: {hi[2]/255:.7f}, a: 1}}
  shadowTint: {{r: {sh[0]/255:.7f}, g: {sh[1]/255:.7f}, b: {sh[2]/255:.7f}, a: 1}}
  foregroundTintStrength: {strength}
  uiTintStrength: {min(.22, strength+.03):.2f}
  saturation: {0.85 if name != 'ChamberToneProfile' else 0.96}
  contrast: 1.05
  lightDirection: {{x: {-0.6 if name == 'ChamberToneProfile' else 0}, y: {0.8 if name == 'ChamberToneProfile' else 1.0}}}
  notes: "{cn} SceneToneProfile: generated for Fate Weaver full asset pass."
"""
        path.write_text(content, encoding="utf-8")
        path.with_suffix(".asset.meta").write_text(PROFILE_META.format(guid=stable_guid(str(path))), encoding="utf-8")


def common_shadow():
    img = Image.new("RGBA", (256, 256), (0, 0, 0, 0))
    cx = cy = 128
    pix = img.load()
    for y in range(256):
        for x in range(256):
            dx = (x - cx) / 118
            dy = (y - cy) / 62
            d = min(1, math.sqrt(dx * dx + dy * dy))
            a = int(180 * max(0, 1 - d) ** 1.8)
            pix[x, y] = (0, 0, 0, a)
    save_png(COMMON / "shadow_default.png", img)


def crop_atlas(box):
    return matte_to_alpha(Image.open(ATLAS).convert("RGBA").crop(box))


def character_base(kind):
    atlas_boxes = {
        "aileen": (0, 0, 300, 520),
        "grick": (300, 35, 585, 590),
        "luna": (0, 0, 300, 520),
    }
    base = crop_atlas(atlas_boxes[kind])
    if kind == "grick":
        base = tint(base, (70, 52, 32), .2, .92)
    if kind == "luna":
        base = ImageOps.mirror(base)
        base = tint(base, (64, 42, 95), .22, .95)
    return base


def warp_frame(image, index, action):
    img = image.copy()
    if action == "idle":
        return ImageOps.pad(img, img.size, color=(0, 0, 0, 0), centering=(.5, .5 + (index % 2) * .01))
    if action == "attack":
        return img.rotate((-8, -3, 5, 11)[index % 4], resample=Image.Resampling.BICUBIC, expand=False)
    if action == "hit":
        return tint(img, (180, 50, 50), .18, .9).rotate((-5, 5)[index % 2], resample=Image.Resampling.BICUBIC, expand=False)
    if action == "skill":
        glow = Image.new("RGBA", img.size, (100, 200, 255, 0))
        d = ImageDraw.Draw(glow, "RGBA")
        d.ellipse((20, 20, img.width - 20, img.height - 20), outline=(100, 200, 255, 45 + index * 35), width=10 + index * 3)
        glow.alpha_composite(img)
        return glow
    if action == "death":
        return tint(img.rotate(28 + index * 12, expand=False), (20, 25, 50), .35 + index * .12, .72)
    return img


def characters():
    actions = {"idle": 4, "attack": 4, "hit": 2, "skill": 3, "death": 3}
    names = {"aileen": "艾琳", "grick": "格里克", "luna": "露娜"}
    for kind, cn in names.items():
        base = tint(character_base(kind), (20, 25, 50), .10, .96)
        for action, count in actions.items():
            for i in range(count):
                frame = fit(warp_frame(base, i, action), (512, 768), 8)
                save_png(FW / f"Characters/{kind}/{kind}_{action}_{i+1:02d}_512x768.png", frame)
                save_png(FW / f"Characters/{kind}/Shadows/{kind}_{action}_{i+1:02d}_shadow.png", shadow((512, 150), .40))


def monster_shape(kind, size, i, action):
    w = h = size
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    d = ImageDraw.Draw(img, "RGBA")
    main = (40, 60, 30)
    if kind == "shadow_small":
        base = crop_atlas((300, 35, 585, 590))
        base = tint(base, main, .15, .85)
        return fit(base.rotate((i % 2) * 3 - 2, expand=False), (w, h), 6)
    colors = {
        "treant": ((66, 82, 42), (22, 40, 18)),
        "toxic_moth": ((74, 118, 82), (120, 75, 150)),
        "gargoyle": ((80, 86, 88), (35, 44, 55)),
        "void_weaver_boss": ((45, 28, 70), (10, 20, 5)),
    }
    c1, c2 = colors[kind]
    amp = 1 + math.sin(i * 1.3) * .03
    if kind == "toxic_moth":
        d.ellipse((w*.18, h*.28, w*.52, h*.62), fill=(*c1, 210))
        d.ellipse((w*.48, h*.28, w*.82, h*.62), fill=(*c1, 210))
        d.ellipse((w*.40, h*.20, w*.60, h*.78), fill=(*c2, 235))
        d.ellipse((w*.45, h*.16, w*.55, h*.26), fill=(160, 210, 120, 240))
    elif kind == "treant":
        d.polygon([(w*.48, h*.12), (w*.30, h*.85), (w*.70, h*.85)], fill=(*c2, 245))
        for x in (.22, .40, .58):
            d.ellipse((w*x, h*.08, w*(x+.28), h*.36), fill=(*c1, 220))
        d.ellipse((w*.39, h*.40, w*.47, h*.48), fill=(120, 210, 95, 220))
        d.ellipse((w*.54, h*.40, w*.62, h*.48), fill=(120, 210, 95, 220))
    elif kind == "gargoyle":
        d.polygon([(w*.50, h*.08), (w*.22, h*.72), (w*.78, h*.72)], fill=(*c1, 240))
        d.polygon([(w*.22, h*.25), (w*.02, h*.55), (w*.32, h*.48)], fill=(*c2, 190))
        d.polygon([(w*.78, h*.25), (w*.98, h*.55), (w*.68, h*.48)], fill=(*c2, 190))
        d.ellipse((w*.39, h*.29, w*.46, h*.36), fill=(100, 200, 255, 230))
        d.ellipse((w*.54, h*.29, w*.61, h*.36), fill=(100, 200, 255, 230))
    else:
        d.ellipse((w*.18, h*.08, w*.82, h*.66), fill=(*c1, 230))
        for n in range(8):
            a = n / 8 * math.tau
            x = w*.5 + math.cos(a) * w*.25
            y = h*.47 + math.sin(a) * h*.18
            d.line((w*.5, h*.48, x, y + h*.30), fill=(*c2, 190), width=max(5, w//35))
        d.ellipse((w*.38, h*.30, w*.45, h*.37), fill=(100, 200, 255, 230))
        d.ellipse((w*.55, h*.30, w*.62, h*.37), fill=(100, 200, 255, 230))
    if action == "attack":
        img = img.rotate((-5, 7, -9, 10, 0, 5)[i % 6], expand=False)
    if action == "death":
        img = tint(img, (10, 20, 5), .28 + i*.04, .75)
    return feather(ImageEnhance.Contrast(img.resize((int(w*amp), h), Image.Resampling.BICUBIC).resize((w, h), Image.Resampling.BICUBIC)).enhance(1.05))


def monsters():
    specs = {
        "shadow_small": (256, {"idle": 2, "attack": 2, "death": 2}),
        "treant": (384, {"idle": 2, "attack": 3, "death": 3}),
        "toxic_moth": (384, {"idle": 2, "attack": 2, "death": 2}),
        "gargoyle": (512, {"idle": 2, "attack": 4, "death": 3}),
        "void_weaver_boss": (768, {"idle": 4, "attack": 6, "hit": 2, "death": 5}),
    }
    for kind, (size, actions) in specs.items():
        for action, count in actions.items():
            for i in range(count):
                img = monster_shape(kind, size, i, action)
                save_png(FW / f"Monsters/Forest/{kind}/{kind}_{action}_{i+1:02d}_{size}x{size}.png", img)
                save_png(FW / f"Monsters/Forest/{kind}/Shadows/{kind}_{action}_{i+1:02d}_shadow.png", shadow((size, max(80, size//4)), .50 if size < 700 else .55))


def slot_machine():
    atlas = Image.open(ATLAS).convert("RGBA")
    frame = matte_to_alpha(atlas.crop((545, 595, 820, 805)))
    for scene, color, crystal in [("chamber", (20, 25, 50), (212, 175, 55)), ("forest", (40, 60, 30), (235, 250, 255))]:
        base = fit(tint(frame, color, .12 if scene == "chamber" else .18, .9), (800, 900), 10)
        layers = {
            "body": base,
            "frame": ImageEnhance.Contrast(base).enhance(1.15),
            "reels": Image.new("RGBA", (800, 900), (0, 0, 0, 0)),
            "slot_base": Image.new("RGBA", (800, 900), (0, 0, 0, 0)),
            "crystal_glow": Image.new("RGBA", (800, 900), (0, 0, 0, 0)),
        }
        d = ImageDraw.Draw(layers["reels"], "RGBA")
        for x in [250, 400, 550]:
            d.rounded_rectangle((x-52, 210, x+52, 680), radius=18, fill=(48, 42, 36, 180), outline=(160, 125, 45, 230), width=6)
        d = ImageDraw.Draw(layers["slot_base"], "RGBA")
        for x in [220, 400, 580]:
            for y in [270, 450, 630]:
                d.rounded_rectangle((x-74, y-74, x+74, y+74), radius=18, fill=(8, 14, 28, 220), outline=(190, 150, 58, 210), width=5)
        d = ImageDraw.Draw(layers["crystal_glow"], "RGBA")
        for x in [220, 400, 580]:
            for y in [270, 450, 630]:
                d.ellipse((x-58, y-58, x+58, y+58), fill=(*crystal, 42))
        for suffix, img in layers.items():
            save_png(FW / f"SlotMachine/{scene}_slot_machine_{suffix}_800x900.png", feather(img))


def symbol_icon(kind, scene, state="normal"):
    src = GEN / "FateWeaverFusion" / f"symbol_{kind}_{'forest' if scene == 'forest' else 'universal'}.png"
    if src.exists():
        img = Image.open(src).convert("RGBA")
    else:
        img = Image.new("RGBA", (180, 180), (0, 0, 0, 0))
    scene_colors = {"chamber": (20, 25, 50), "forest": (40, 60, 30), "volcano": (60, 30, 20), "void": (40, 20, 40)}
    img = fit(tint(img, scene_colors[scene], {"chamber": .10, "forest": .15, "volcano": .12, "void": .18}[scene], .85 if scene != "chamber" else 1.0), (180, 180), 8)
    if state == "disabled":
        img = ImageEnhance.Color(img).enhance(0)
        arr = np.array(img).astype(np.uint8)
        arr[:, :, 3] = (arr[:, :, 3].astype(np.float32) * .5).astype(np.uint8)
        img = Image.fromarray(arr, "RGBA")
    if state == "highlight":
        glow = Image.new("RGBA", (180, 180), (0, 0, 0, 0))
        glow.alpha_composite(img.filter(ImageFilter.GaussianBlur(4)))
        arr = np.array(glow)
        arr[:, :, :3] = (212, 175, 55)
        glow = Image.fromarray(arr, "RGBA")
        glow.alpha_composite(img)
        img = glow
    return feather(img)


def symbols():
    names = ["sword", "staff", "heart", "shield", "skull", "star"]
    for scene in ["chamber", "forest", "volcano", "void"]:
        for name in names:
            save_png(FW / f"Symbols/{scene}/symbol_{name}_{scene}.png", symbol_icon(name, scene))
    for name in names:
        save_png(FW / f"Symbols/disabled/symbol_{name}_disabled.png", symbol_icon(name, "chamber", "disabled"))
        save_png(FW / f"Symbols/highlight/symbol_{name}_highlight.png", symbol_icon(name, "chamber", "highlight"))


def icon_canvas(size=(120, 120)):
    return Image.new("RGBA", size, (0, 0, 0, 0)), ImageDraw.Draw(Image.new("RGBA", size, (0, 0, 0, 0)), "RGBA")


def simple_icon(path, label, size, color=(212, 175, 55), shape="circle"):
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    d = ImageDraw.Draw(img, "RGBA")
    w, h = size
    if shape == "circle":
        d.ellipse((8, 8, w-8, h-8), fill=(15, 24, 42, 200), outline=(*color, 230), width=max(3, w//22))
    elif shape == "diamond":
        d.polygon([(w/2, 6), (w-6, h/2), (w/2, h-6), (6, h/2)], fill=(15, 24, 42, 200), outline=(*color, 230))
    else:
        d.rounded_rectangle((8, 8, w-8, h-8), radius=max(8, w//8), fill=(15, 24, 42, 200), outline=(*color, 230), width=max(3, w//22))
    try:
        font = ImageFont.truetype("/System/Library/Fonts/STHeiti Medium.ttc", max(24, int(h*.36)))
    except Exception:
        font = ImageFont.load_default()
    bbox = d.textbbox((0, 0), label, font=font)
    d.text(((w-(bbox[2]-bbox[0]))/2, (h-(bbox[3]-bbox[1]))/2-2), label, font=font, fill=(240, 235, 220, 255))
    save_png(path, feather(img))


def ui_assets():
    for key, label in [("adventure", "冒"), ("hero", "英"), ("weave", "织"), ("workshop", "工"), ("quest", "任"), ("home", "家")]:
        simple_icon(FW / f"UI/BottomMenu/icon_bottom_{key}_120x120.png", label, (120, 120), shape="circle")
    for key, label, shape in [("coin_gear", "齿", "circle"), ("diamond_prism", "◇", "diamond"), ("fate_thread", "线", "circle")]:
        simple_icon(FW / f"UI/Currency/icon_currency_{key}_64x64.png", label, (64, 64), shape=shape)
    for key, label in [("battle", "战"), ("event", "?"), ("altar", "火"), ("shop", "袋"), ("boss", "骷")]:
        simple_icon(FW / f"UI/MapNodes/icon_map_{key}_120x120.png", label, (120, 120), color=(100, 200, 255), shape="diamond" if key == "boss" else "circle")
    panel = Image.open(GEN / "FateWeaverDeliverables/UI/Panels/panel_chamber_parchment_9slice_900x600_border96.png").convert("RGBA")
    save_png(FW / "UI/Panels/panel_chamber_parchment_9slice_900x600_border96.png", panel, border=96)
    forest_panel = Image.open(GEN / "FateWeaverFusion/ui_panel_forest_frosted_metal.png").convert("RGBA")
    save_png(FW / "UI/Panels/panel_forest_frosted_metal_9slice_900x600_border96.png", fit(forest_panel, (900, 600)), border=96)
    generic = Image.new("RGBA", (900, 600), (16, 20, 34, 215))
    d = ImageDraw.Draw(generic, "RGBA")
    for y in range(0, 600, 8):
        d.line((0, y, 900, y), fill=(100, 200, 255, 18), width=2)
    d.rounded_rectangle((18, 18, 882, 582), radius=30, outline=(212, 175, 55, 190), width=8)
    save_png(FW / "UI/Panels/panel_common_dark_translucent_9slice_900x600_border96.png", generic, border=96)
    base = Image.open(GEN / "FateWeaverDeliverables/UI/Buttons/Primary/primary_button_normal_280x100.png").convert("RGBA")
    states = {
        "normal": base,
        "hover": ImageEnhance.Brightness(base).enhance(1.25),
        "pressed": ImageChops.offset(ImageEnhance.Brightness(base).enhance(.65), 0, 4),
        "disabled": ImageEnhance.Color(ImageEnhance.Brightness(base).enhance(.45)).enhance(.1),
    }
    for state, img in states.items():
        save_png(FW / f"UI/Buttons/primary_button_{state}_280x100.png", img)
    for state, color, scale in [("normal", (100, 200, 255), 1.0), ("highlight", (170, 230, 255), 1.08), ("pressed", (65, 150, 210), .92)]:
        img = Image.new("RGBA", (200, 200), (0, 0, 0, 0))
        d = ImageDraw.Draw(img, "RGBA")
        r = int(82*scale)
        d.ellipse((100-r, 100-r, 100+r, 100+r), fill=(*color, 210), outline=(235, 250, 255, 230), width=7)
        d.ellipse((44, 30, 136, 84), fill=(255, 255, 255, 45))
        save_png(FW / f"UI/Buttons/stop_button_{state}_200x200.png", feather(img))
    for state, b in [("normal", 1.0), ("pressed", .62)]:
        img = Image.new("RGBA", (40, 40), (0, 0, 0, 0))
        d = ImageDraw.Draw(img, "RGBA")
        d.ellipse((3, 3, 37, 37), fill=(30, 36, 54, int(220*b)), outline=(212, 175, 55, 210), width=3)
        d.line((13, 13, 27, 27), fill=(240, 235, 220, 255), width=4)
        d.line((27, 13, 13, 27), fill=(240, 235, 220, 255), width=4)
        save_png(FW / f"UI/Buttons/close_button_{state}_40x40.png", img)
    for name, size, fill, bg in [
        ("hp_bar", (400, 20), (180, 50, 50), (55, 18, 22)),
        ("energy_spindle", (300, 40), (100, 200, 255), (15, 28, 56)),
    ]:
        for part, color in [("frame", (212, 175, 55)), ("fill", fill), ("background", bg)]:
            img = Image.new("RGBA", size, (0, 0, 0, 0))
            d = ImageDraw.Draw(img, "RGBA")
            radius = size[1]//2
            if part == "frame":
                d.rounded_rectangle((0, 0, size[0]-1, size[1]-1), radius=radius, outline=(*color, 230), width=4)
            else:
                d.rounded_rectangle((0, 0, size[0]-1, size[1]-1), radius=radius, fill=(*color, 225))
            save_png(FW / f"UI/Bars/{name}_{part}_{size[0]}x{size[1]}_9slice.png", img, border=8)
    title = Image.new("RGBA", (1600, 400), (0, 0, 0, 0))
    d = ImageDraw.Draw(title, "RGBA")
    try:
        font = ImageFont.truetype("/System/Library/Fonts/Supplemental/Songti.ttc", 220)
    except Exception:
        font = ImageFont.load_default()
    text = "命运纺机"
    bbox = d.textbbox((0, 0), text, font=font)
    x, y = (1600-(bbox[2]-bbox[0]))//2, (400-(bbox[3]-bbox[1]))//2 - 20
    d.text((x+12, y+12), text, font=font, fill=(0, 0, 0, 160))
    d.text((x, y), text, font=font, fill=(212, 175, 55, 255))
    title = title.resize((400, 100), Image.Resampling.LANCZOS)
    save_png(FW / "UI/title_fate_weaver_命运纺机_4x_supersampled_400x100.png", title)


def furniture_assets():
    items = [
        ("window_star_night", "星夜之窗", "window", (350, 550), "window"),
        ("window_aurora", "极光之窗", "window", (350, 550), "window"),
        ("window_abyss_rift", "深渊裂隙窗", "window", (350, 550), "window"),
        ("window_forest_morning", "森林晨光窗", "window", (350, 550), "window"),
        ("window_japanese_paper", "日式纸窗", "window", (350, 550), "window"),
        ("bookcase_oak", "橡木书架", "bookcase", (200, 400), "bookcase"),
        ("bookcase_arcane", "奥秘书架", "bookcase", (200, 400), "bookcase"),
        ("bookcase_crystal", "水晶书架", "bookcase", (200, 400), "bookcase"),
        ("tapestry_fate", "命运织锦", "tapestry", (400, 300), "tapestry"),
        ("tapestry_hero", "勇者战记", "tapestry", (400, 300), "tapestry"),
        ("tapestry_star", "星空挂毯", "tapestry", (400, 300), "tapestry"),
        ("rug_warm", "暖绒地毯", "rug", (700, 500), "rug"),
        ("rug_magic_circle", "魔法阵地毯", "rug", (700, 500), "rug"),
        ("rug_gold_thread", "金线地毯", "rug", (700, 500), "rug"),
        ("decor_candle", "蜡烛台", "decor", (120, 120), "decor"),
        ("decor_crystal_ball", "水晶球", "decor", (120, 120), "decor"),
        ("decor_hourglass", "时光沙漏", "decor", (120, 120), "decor"),
        ("decor_pet_cat", "宠物猫", "decor", (120, 120), "decor"),
        ("decor_weaver_bird", "织布鸟", "decor", (120, 120), "decor"),
        ("display_trophy_case", "战利品展示架", "display", (300, 400), "display"),
    ]
    badge_names = ["影怪", "树精", "毒蛾", "石像鬼", "虚无织影者"]
    for i, cn in enumerate(badge_names):
        items.append((f"boss_badge_{i+1}", f"{cn}徽章", "boss_badge", (64, 64), "badge"))
    for idx, (item_id, cn, category, size, kind) in enumerate(items):
        img = Image.new("RGBA", size, (0, 0, 0, 0))
        d = ImageDraw.Draw(img, "RGBA")
        w, h = size
        color = [(82, 55, 35), (35, 54, 80), (72, 42, 84), (95, 74, 38)][idx % 4]
        if kind == "window":
            d.rounded_rectangle((20, 15, w-20, h-20), radius=36, fill=(12, 22, 44, 225), outline=(212, 175, 55, 220), width=8)
            for n in range(12):
                x = 50 + (n*41) % max(1, w-100); y = 60 + (n*61) % max(1, h-140)
                d.ellipse((x, y, x+5, y+5), fill=(170, 220, 255, 180))
        elif kind == "bookcase":
            d.rounded_rectangle((12, 8, w-12, h-8), radius=12, fill=(*color, 240), outline=(212, 175, 55, 180), width=5)
            for y in range(70, h-35, 75):
                d.rectangle((25, y, w-25, y+8), fill=(180, 135, 50, 170))
                for x in range(34, w-44, 26):
                    d.rectangle((x, y-42, x+16, y), fill=((x*3)%120+40, 45, 70, 230))
        elif kind == "tapestry":
            d.rounded_rectangle((8, 12, w-8, h-18), radius=18, fill=(*color, 230), outline=(212, 175, 55, 170), width=5)
            d.line((w//2, 35, w//2, h-45), fill=(212, 175, 55, 110), width=4)
            d.ellipse((w*.28, h*.25, w*.72, h*.70), outline=(100, 200, 255, 110), width=6)
        elif kind == "rug":
            d.ellipse((20, 80, w-20, h-80), fill=(*color, 225), outline=(212, 175, 55, 180), width=8)
            d.ellipse((75, 130, w-75, h-130), outline=(100, 200, 255, 120), width=8)
        elif kind == "badge":
            d.ellipse((4, 4, w-4, h-4), fill=(22, 28, 44, 235), outline=(212, 175, 55, 220), width=4)
            d.text((w*.35, h*.23), str(idx-19), fill=(240, 235, 220, 255))
        else:
            d.ellipse((10, 10, w-10, h-10), fill=(*color, 220), outline=(212, 175, 55, 200), width=5)
            d.line((w//2, 20, w//2, h-20), fill=(100, 200, 255, 120), width=5)
        img = tint(img, (20, 25, 50), .10, .95)
        sprite_path = FW / f"Furniture/{category}/{item_id}.png"
        shadow_path = FW / f"Furniture/{category}/{item_id}_shadow.png"
        save_png(sprite_path, feather(img))
        save_png(shadow_path, shadow((w, max(64, h//4)), .40, 1.3))
        asset = DATA / f"Furniture/{item_id}.asset"
        asset.write_text(f"""%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 0}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {FURNITURE_SCRIPT_GUID}, type: 3}}
  m_Name: {item_id}
  m_EditorClassIdentifier: 
  itemId: {item_id}
  displayName: {cn}
  category: {category}
  sizePixels: {{x: {w}, y: {h}}}
  unlockCondition: default
  sprite: {{fileID: 21300000, guid: {stable_guid(str(sprite_path))}, type: 3}}
  shadowSprite: {{fileID: 21300000, guid: {stable_guid(str(shadow_path))}, type: 3}}
""", encoding="utf-8")
        asset.with_suffix(".asset.meta").write_text(PROFILE_META.format(guid=stable_guid(str(asset))), encoding="utf-8")


def audio():
    def tone(path, duration, freqs, volume=.35, sr=44100):
        path.parent.mkdir(parents=True, exist_ok=True)
        n = int(duration * sr)
        t = np.arange(n) / sr
        wave_data = np.zeros(n, dtype=np.float32)
        for f in freqs:
            wave_data += np.sin(2 * np.pi * f * t)
        wave_data /= max(1, len(freqs))
        env = np.minimum(1, np.linspace(0, 1, max(1, int(sr*.02))).tolist() + [1]*(n-max(1, int(sr*.04))) + np.linspace(1, 0, max(1, int(sr*.02))).tolist())
        if len(env) != n:
            env = np.ones(n)
        data = np.int16(np.clip(wave_data * env * volume, -1, 1) * 32767)
        with wave.open(str(path), "wb") as wf:
            wf.setnchannels(1); wf.setsampwidth(2); wf.setframerate(sr); wf.writeframes(data.tobytes())
    slot = {
        "slot_activate_gear_accel_0p5s.wav": (.5, [120, 240, 360]),
        "slot_reel_loop_1s.wav": (1.0, [95, 180, 330]),
        "slot_stop_click_0p2s.wav": (.2, [700, 1220]),
        "slot_perfect_line_1s.wav": (1.0, [523, 659, 988]),
        "slot_partial_line_0p3s.wav": (.3, [660, 880]),
        "slot_penalty_glass_0p8s.wav": (.8, [70, 140, 930]),
    }
    combat = {
        "combat_slash_wave_0p5s.wav": (.5, [280, 760]),
        "combat_magic_projectile_0p4s.wav": (.4, [440, 880]),
        "combat_heal_column_0p6s.wav": (.6, [392, 523, 784]),
        "combat_shield_activate_0p5s.wav": (.5, [180, 360, 720]),
        "boss_entrance_2s.wav": (2.0, [45, 90, 135]),
    }
    for i in range(1, 4):
        combat[f"enemy_hit_var{i}_0p2s.wav"] = (.2, [260+i*60, 620+i*90])
        combat[f"enemy_death_var{i}_0p5s.wav"] = (.5, [90+i*30, 180+i*25])
    ui = {
        "ui_button_click_0p1s.wav": (.1, [900, 1400]),
        "ui_popup_open_0p3s.wav": (.3, [280, 440]),
        "ui_coin_gain_0p2s.wav": (.2, [1000, 1300]),
        "ui_item_gain_0p3s.wav": (.3, [660, 990]),
        "ui_victory_sting_2s.wav": (2.0, [523, 659, 784]),
    }
    ambient = {
        "ambient_chamber_loom_fire_loop.wav": (4.0, [80, 120, 240]),
        "ambient_forest_leaves_loop.wav": (4.0, [110, 170, 260]),
    }
    for name, spec in slot.items(): tone(AUDIO / "SFX/SlotMachine" / name, *spec)
    for name, spec in combat.items(): tone(AUDIO / "SFX/Combat" / name, *spec)
    for name, spec in ui.items(): tone(AUDIO / "SFX/UI" / name, *spec)
    for name, spec in ambient.items(): tone(AUDIO / "Ambient" / name, *spec, volume=.18)


def fonts_and_docs():
    for src in [Path("/System/Library/Fonts/STHeiti Medium.ttc"), Path("/System/Library/Fonts/Supplemental/Songti.ttc")]:
        if src.exists():
            shutil.copyfile(src, FONTS / src.name)
    (DOCS / "FONT_RENDERING_PLAN.md").write_text("""# Fate Weaver Typography

Title image: `Assets/DestinyRanger/Art/Generated/FateWeaverFull/UI/title_fate_weaver_命运纺机_4x_supersampled_400x100.png`.

Title font source: Songti TTC copied to `Assets/Fonts/Songti.ttc`. The title PNG is rendered at 1600x400 and downsampled to 400x100 for 4x supersampling, gold fill, dark shadow, transparent PNG.

Body font source: `Assets/Fonts/STHeiti Medium.ttc`. Use TextMeshPro Font Asset Creator, Sampling Point Size 72, Padding 8, Atlas 2048, SDF. TextMeshPro package is present in `Packages/manifest.json`; actual TMP_FontAsset creation still requires opening Unity, currently blocked by local Unity Licensing IPC.
""", encoding="utf-8")


def manifest():
    data = {
        "no_validation_images_generated": True,
        "backgrounds": len(list((FW / "Backgrounds").glob("*.png"))),
        "characters_png": len(list((FW / "Characters").glob("**/*.png"))),
        "monsters_png": len(list((FW / "Monsters").glob("**/*.png"))),
        "slot_machine_png": len(list((FW / "SlotMachine").glob("*.png"))),
        "symbols_png": len(list((FW / "Symbols").glob("**/*.png"))),
        "ui_png": len(list((FW / "UI").glob("**/*.png"))),
        "furniture_png": len(list((FW / "Furniture").glob("**/*.png"))),
        "furniture_assets": len(list((DATA / "Furniture").glob("*.asset"))),
        "audio_wav": len(list(AUDIO.glob("**/*.wav"))),
    }
    (FW / "FULL_ASSET_MANIFEST.json").write_text(json.dumps(data, ensure_ascii=False, indent=2), encoding="utf-8")


def main():
    mkdirs()
    normalize_backgrounds()
    scene_profiles()
    common_shadow()
    characters()
    monsters()
    slot_machine()
    symbols()
    ui_assets()
    furniture_assets()
    audio()
    fonts_and_docs()
    manifest()
    print(FW)


if __name__ == "__main__":
    main()
