#!/usr/bin/env python3
"""Generate Star Bay interior GLB meshes from Modly/Hunyuan3D Mini input images."""

from __future__ import annotations

import argparse
import io
import random
import sys
from pathlib import Path

from PIL import Image


MODEL_DIR = Path("/Users/zhendian/Documents/Modly/models/hunyuan3d-mini/generate")
HY3DGEN_DIR = MODEL_DIR / "_hy3dgen"


def preprocess_image(path: Path, skip_rembg: bool) -> Image.Image:
    if skip_rembg:
        return Image.open(path).convert("RGBA")

    import rembg

    img = Image.open(path)
    if img.mode == "RGBA" and img.getextrema()[3][0] < 255:
        return img
    return rembg.remove(img).convert("RGBA")


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--image", required=True, type=Path)
    parser.add_argument("--out", required=True, type=Path)
    parser.add_argument("--steps", type=int, default=10)
    parser.add_argument("--octree", type=int, default=256)
    parser.add_argument("--guidance", type=float, default=5.5)
    parser.add_argument("--seed", type=int, default=-1)
    parser.add_argument("--skip-rembg", action="store_true")
    args = parser.parse_args()

    if not (MODEL_DIR / "hunyuan3d-dit-v2-mini/model.fp16.safetensors").exists():
        raise FileNotFoundError("Hunyuan3D Mini weights are missing in the Modly models folder.")

    if str(HY3DGEN_DIR) not in sys.path:
        sys.path.insert(0, str(HY3DGEN_DIR))

    import torch
    from hy3dgen.shapegen import Hunyuan3DDiTFlowMatchingPipeline

    if sys.platform == "darwin" and torch.backends.mps.is_available():
        device = "mps"
    elif torch.cuda.is_available():
        device = "cuda"
    else:
        device = "cpu"

    dtype = torch.float16 if device == "cuda" else torch.float32
    seed = random.randint(0, 2**32 - 1) if args.seed == -1 else args.seed

    print(f"[StarBayModlyMesh] Loading Hunyuan3D Mini on {device}...")
    pipeline = Hunyuan3DDiTFlowMatchingPipeline.from_pretrained(
        str(MODEL_DIR),
        subfolder="hunyuan3d-dit-v2-mini",
        use_safetensors=True,
        device=device,
        dtype=dtype,
    )

    print(f"[StarBayModlyMesh] Preprocessing {args.image}...")
    image = preprocess_image(args.image, args.skip_rembg)

    print(
        "[StarBayModlyMesh] Generating mesh "
        f"steps={args.steps} octree={args.octree} guidance={args.guidance} seed={seed}..."
    )
    with torch.no_grad():
        generator = torch.Generator().manual_seed(seed)
        outputs = pipeline(
            image=image,
            num_inference_steps=args.steps,
            octree_resolution=args.octree,
            guidance_scale=args.guidance,
            num_chunks=4000,
            generator=generator,
            output_type="trimesh",
        )

    args.out.parent.mkdir(parents=True, exist_ok=True)
    outputs[0].export(str(args.out))
    print(f"[StarBayModlyMesh] Wrote {args.out}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
