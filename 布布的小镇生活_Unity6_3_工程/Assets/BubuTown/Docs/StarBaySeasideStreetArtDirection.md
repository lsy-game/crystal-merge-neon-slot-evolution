# Star Bay Seaside Main Street Art Direction

This note records the target scene direction for the current Unity 6.3 prototype pass.

## Visual Target

- A bright seaside modern commercial street.
- Low to mid-rise mixed-use buildings on both sides.
- Ground-floor shops with large clean glass, readable signs, awnings, warm interiors, and human-scale props.
- Upper floors should feel like small apartments or offices, with balconies, curtains, AC units, window frames, and light facade panels.
- The street should stay pedestrian-friendly and clean, with stone pavers instead of a car-first asphalt road.
- The far end must keep an open view to the ocean, with a promenade, railings, palms, benches, and pale blue sea haze.

## Current Prototype Rule

- The clean StarBay scene should not use the old messy overlapping imported buildings by default.
- Existing imported assets may remain in the project for reference or future replacement.
- Any new asset must support the sunny seaside commercial street style before being placed in the main scene.

## Free Asset Candidates

| Source | License Direction | Best Use | Risk |
| --- | --- | --- | --- |
| Kenney City Kit Commercial | CC0/public-safe candidate | Modular storefronts, shop props, clean low-poly city pieces | Too low-poly if used raw; needs material polish |
| Quaternius Downtown City MegaKit | CC0/public-safe candidate | Wider modern building variety, street props, vehicles | Style may need unification with StarBay palette |
| Unity Asset Store free city packs | Package-specific license | Supplemental buildings/props after manual review | Many packs are old, dark, or visually inconsistent |

## Replacement Order

1. Replace one foreground shop bay at a time, starting with the bakery and convenience store.
2. Keep the procedural paver street and ocean end until model replacements prove they improve the scene.
3. Replace upper facade modules after the ground-floor storefronts feel correct.
4. Add palms, benches, planters, street lamps, and shop display props only when they do not block the third-person view.
5. Avoid imported buildings that hide the ocean opening or make the street look like a dense downtown corridor.

## Acceptance Checks

- From the third-person camera, the player can immediately read: modern shops, clean street, ocean at the end.
- The street has enough detail close to the player, but no noisy wires or oversized props.
- Storefront glass, signs, awnings, and upper windows share one visual language.
- No magenta missing materials, point-filtered textures, copyrighted IP marks, or local-only private skins in public-safe content.
