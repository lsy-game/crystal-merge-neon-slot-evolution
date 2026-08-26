# OpenGameArt Candidate Sources

Downloaded files are not committed here yet. This file records the vetted source pool and how each item may be used after conversion.

| Source | URL | License Seen | Candidate Use | Conversion Notes |
|--------|-----|--------------|---------------|------------------|
| 2D Platformer Forest Pack | https://opengameart.org/content/2d-platformer-forest-pack | CC0 | Forest platform silhouettes, grass caps, bushes, trunk/leaf staging. | Download candidate: `https://opengameart.org/sites/default/files/2D%20Platformer%20Forest%20Pack%20%28Tio%20Aimar%29.zip`. Use as structure reference for thick grass/stone floors. Convert to the current bright Q-style forest ruins palette before runtime use. |
| Free 2D Block Forest Tile Pack | https://opengameart.org/content/free-2d-block-forest-tile-pack | CC0 | Blocky grass and dirt tile readability. | Download candidate: `https://opengameart.org/sites/default/files/Block_Forest_2.zip`. Strong candidate for solving solid foot-ground clarity. Repaint or regenerate into non-pixel, painterly platform chunks. |
| Pixel Art Platformer Asset Pack | https://opengameart.org/content/pixel-art-platformer-asset-pack | CC0 | Dirt, rocks, ruins, foliage composition. | Download candidate: `https://opengameart.org/sites/default/files/glax-old-platformer-assets.zip`. Use for layout ideas only; direct pixel art import would clash with the current generated art style. |
| 2D Platformer Enemies | https://opengameart.org/content/2d-platformer-enemies | CC0 | Enemy silhouette and simple animation-cycle reference. | Use only as shape/timing reference; final enemies must remain project-style transparent PNG sprites. |
| Slash | https://opengameart.org/content/slash-0 | CC0 | Slash VFX fallback reference. | Lower priority because the project already uses user-authorized Spine VFX sequences. |

## Formal Import Checklist

- Verify the source page still lists `CC0` before downloading.
- Save the original archive name and download date.
- Keep raw files in this candidate folder, not in `Generated`.
- Do not reference candidate files from runtime scripts or scene builder.
- Convert selected terrain into a single project-style PNG sheet and register the generated/converted output in `Assets/DestinyRanger/Docs/ASSET_PROVENANCE.md`.

## Prepared Candidate Outputs

| File | Inputs | Status | Notes |
|------|--------|--------|-------|
| `Prepared/opengameart-platform-candidate-sheet.png` | `Block_Forest_2` terrain PNGs | Rejected for formal use | Structurally solid but too flat/bright; keep only as a first-pass readability check. |
| `Prepared/destiny-ranger-platform-concept-from-oga.png` | OpenGameArt CC0 tile reference + forest palette reference | Review only | AI-generated concept sheet for thicker painterly grass/stone/earth platforms. |
| `Prepared/destiny-ranger-platform-concept-from-oga-transparent.png` | Same as above, with connected background removed locally | Candidate for next Unity scene pass | Transparent PNG candidate. Needs slicing and in-scene collision verification before moving into `Generated`. |
| `Prepared/destiny-ranger-platform-concept-from-oga-alpha-preview.jpg` | Transparent PNG composited over checker/gray background | QA preview | Confirms the review background was removed and edges are usable for visual testing. |
