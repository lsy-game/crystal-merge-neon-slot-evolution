# Bubu Town Modern Visual Style Guide

Purpose: keep every public-facing city asset cohesive, modern, readable, and safe for a public GitHub build.

## Current Baseline

- Primary imported kit: `Assets/ithappy/Cartoon_City_Free`
- Target look: modern cozy low-poly city, clean silhouettes, smooth filtered textures, soft URP lighting, readable street-level scale.
- Avoid: pixel-art textures, visibly blocky low-resolution materials, realistic scanned assets, horror/grunge sets, medieval/fantasy props, and unrelated sci-fi clutter.

## Free Resource Candidate Rules

- Use as reference first; import only after visual and license review.
- Prefer CC0 or clear free commercial-use terms.
- Keep original license text or source URL in project documentation before importing.
- Do not commit private skins, copyrighted IP references, or account-only downloads that cannot be redistributed.
- Match the existing Cartoon City Free proportions, color saturation, material smoothness, and simplified geometry.
- Import a small test slice before adding a whole pack.
- Run the modern visual quality gate after every import.

## Candidate Resource Directions

- ITHappy Cartoon City Free: current baseline and first-pass source for city objects. Source: `https://ithappystudios.com/free/cartoon-city-free`
- Kenney City Kit Commercial: CC0, modern commercial buildings, good for skyline and storefront layout reference. Source: `https://kenney.nl/assets/city-kit-commercial`
- Kenney City Kit Roads: CC0, road signs, traffic lights, sidewalks, and modular road pieces. Source: `https://kenney.nl/assets/city-kit-roads`
- Quaternius Downtown City MegaKit: CC0/free city block models; stronger downtown direction, should be used carefully because its facade density is more realistic than the current cozy baseline. Source: `https://quaternius.com/packs/downtowncitymegakit.html`
- Quaternius furniture/interior kits: good future candidate for home decoration placeholders if proportions are softened to match the town. Source hub: `https://quaternius.com`

## Style Admission Checklist

- License is documented and compatible with public distribution.
- Asset scale fits player third-person walking distance.
- Texture filtering is not Point and mipmaps are enabled where applicable.
- Materials render in Unity 6 URP without magenta or InternalErrorShader fallback.
- Color palette stays warm modern, not one-note neon, beige-only, or dark cyberpunk.
- The asset supports the MVP loop: quest, shopping, home return, furniture placement, transit, school, park, or garage navigation.
