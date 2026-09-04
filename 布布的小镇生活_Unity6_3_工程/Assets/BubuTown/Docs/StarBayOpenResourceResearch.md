# StarBay Open Resource Research

## Current Standard

Use the generated StarBay concept image and the user's seaside street reference as the art target:

- Low-rise seaside commercial street.
- Clean modern shopfronts, not tower-heavy downtown blocks.
- Readable glass storefronts, awnings, balcony rails, planters, warm gray pavers, and ocean at the end of the street.
- Third-person player-scale readability is more important than distant skyline complexity.

## Best-Fit Free Resources

### Kenney City Kit Commercial

- Source: https://kenney.nl/assets/city-kit-commercial
- License: Creative Commons CC0.
- Why it fits: commercial buildings, awnings, overhangs, shop-like parts, and low-poly structures that can be used as base meshes.
- Current use: selected commercial building GLB models are used as underlay candidates in `BubuTownStarBayCleanSceneBuilder`.

### Kenney Modular Buildings

- Source: https://kenney.nl/assets/modular-buildings
- License: Creative Commons CC0.
- Why it fits: modular windows, roofs, wall pieces, and building parts are better for making more detailed facades than a single monolithic low-poly building.
- Planned use: import and combine into reusable StarBay facade modules after the first clean street pass is verified.

### Kenney City Kit Roads/Suburban

- Source family: https://kenney.nl/assets
- License: Creative Commons CC0.
- Why it fits: road lights, electricity poles, planters, fences, small trees, sidewalk pieces, and suburban building silhouettes help fill the scene without changing the style.
- Current use: local `Kenney/Roads` and `Kenney/Suburban` assets are already available in the project tree.

## Rejected For Now

- Tower-heavy modern city kits: useful as far background or replacement candidates, but they should not define the main StarBay street mood.
- Random free Sketchfab/marketplace models without clear redistribution terms: only use after checking license and source.
- Photorealistic megacity packs: they fight the cozy seaside town identity.

## Next Resource Step

After the clean Unity scene opens successfully, choose 2-4 Kenney base building meshes that match the concept image, then layer custom facade modules over them:

- Shopfront glass and mullions.
- Pink bakery awning and pastry display.
- Blue-green convenience sign band.
- Cafe flower planters and terrace.
- Furniture/cinema poster panel.
- Roof parapets, balcony rails, AC units, and flower boxes.
