# StarBay Seaside Main Street Rebuild Plan

## Visual Target

The current scene direction follows the user's reference image: a sunny seaside modern commercial street with low-rise shopfronts, warm gray paving, visible ocean at the end of the street, palm trees, planters, benches, and cozy storefront life.

## Implemented First Pass

- New primary scene layer: `01_Map_Greybox/StarBay_Seaside_MainStreet_Rebuild_v1`
- Street spine: warm gray paver run with subtle cross joints and curb lines.
- Left-side storefronts: `Left_CakeShop_LowRise`, `Left_CafeFlower_LowRise`
- Right-side storefronts: `Right_StarBayConvenience_LowRise`, `Right_FurnitureCinema_LowRise`
- Ocean endpoint: `Seaside_Promenade_Ocean_View` with sea plane, sand edge, railing, lighthouse, and distant island shapes.
- Street life props: palm trees, planters, benches, A-frame menu boards, cafe terrace table hints, display windows, and shop color bands.

## Art Direction Notes

- Prefer low-rise mixed-use commercial buildings over tall glass towers.
- Keep storefronts readable from third-person camera height.
- Use soft modern colors: warm gray paving, pale walls, pink bakery accents, blue-green convenience accents, wood details, and sunny seaside lighting.
- Old modern city/high-rise integration assets should no longer define the main camera view. They are kept only as replacement candidates and validation anchors until the old dependency chain is retired.

## Next Iteration

- Replace the rough cube storefronts with free/open commercial-street prefabs that match the reference style.
- Add more real facade detail: awnings, balcony railings, recessed doors, indoor display shelves, readable shop identities.
- Tune street width, ocean distance, and camera height against the third-person player silhouette.
- Gradually remove or archive legacy tower-heavy layers after gameplay anchors and validation rules are updated.
