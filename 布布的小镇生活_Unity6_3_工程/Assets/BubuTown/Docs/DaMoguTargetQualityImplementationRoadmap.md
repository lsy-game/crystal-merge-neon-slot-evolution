# DaMogu Target Quality Implementation Roadmap

## Target

Reach the visual direction shown by `DaMogu_TargetThirdPersonSeasideLook_v2.png`: a polished third-person anime life-sim protagonist walking through a bright modern seaside commercial street. Use `DaMogu_FinalModelTurnaroundDirection_v2.png` as the base front/side/back modeling reference, `DaMogu_ThirdPerson3DTargetPreview_v3.png` as the gameplay-view quality target, and `DaMogu_ModelProductionDetailReference_v4.png` as the most specific character production reference.

## Current Gap

The current VRM candidate is useful for proving the pipeline, but it is not the final DaMogu:
- The body is a sample asset, not an original public character.
- The lower-body silhouette is too heavy and dark.
- The face and material response are readable but not polished toon/anime quality.
- The hairstyle is close in color but not the intended clean bob plus small half-up bun.
- The current town scene has modern-city pieces, but the ground, storefront scale, sea view, and street composition need to be tightened around the third-person camera.

The v3 target preview is closer to the desired result, but it is still a visual reference rather than a real Unity asset. The v4 detail reference improves the production read, but its main remaining risks are: face and head ratio must be checked in the actual third-person camera, leg proportions must be validated against a practical Humanoid rig, and hair/bag/skort details must be simplified enough to animate cleanly in-game.

## Phase 1: Character Shape Lock

Goal: make a custom DaMogu base model that reads correctly from behind.

Tasks:
- Create or customize an original VRoid/Blender Humanoid model.
- Use a smaller mature-anime head ratio, not chibi.
- Build short brown bob hair with a small half-up bun.
- Replace long black lower garment with navy skort or short skirt.
- Add bare legs, pink-white sneakers, and small crossbody bag.
- Export as VRM or FBX and retarget to the existing locomotion controller.
- Build the final look as real mesh, hair, textures, rigging, and materials. Do not stack primitive Unity geometry over an unrelated sample model as a final-art shortcut.
- Use v4 to define the first model's face, hair silhouette, outfit panels, shoes, and bag scale.

Acceptance:
- Back silhouette matches the target image at third-person gameplay distance.
- No bean/chibi face impression in close-up.
- No mushroom visual motif.
- Valid Humanoid Avatar in Unity.
- Outfit and hair are part of the actual model, not temporary overlay props.
- In a Unity third-person camera, the model still reads like the v3 gameplay target.

## Phase 2: Locomotion Polish

Goal: make DaMogu feel soft, casual, and life-sim oriented while moving.

Tasks:
- Keep current Idle/Walk/Run/Sprint controller as the technical baseline.
- Replace or tune walk to be gentler and less combat/action oriented.
- Add tired walk for low stamina.
- Add subtle idle breathing and relaxed hand pose.
- Add future hooks for bike mount, bed/sleep, shop work, and minigame stances.

Acceptance:
- Walk loop has no obvious foot sliding.
- Run reads energetic but not aggressive.
- Hair, jacket hem, bag strap, and skirt/skort can receive secondary motion later.

## Phase 3: Third-Person Camera Composition

Goal: match the target image's comfortable behind-the-character composition.

Tasks:
- Camera follow height: about 2.4m to 2.7m.
- Camera distance: about 4.2m to 5.0m.
- Character screen height while walking: about 32% to 45% of vertical screen.
- Keep ocean/street destination visible above the character when possible.
- Avoid camera clipping through storefront props.

Acceptance:
- DaMogu remains centered and readable from behind.
- The town ahead is visible enough to invite walking forward.
- The camera feels calm, not action-game shaky.

## Phase 4: Seaside Commercial Street Upgrade

Goal: make Star Bay Town's first view feel like a cozy modern seaside life game.

Tasks:
- Convert the main street around the player into a clean pedestrian-friendly commercial street.
- Place bakery on the left or near first route, with warm window display.
- Place Star Bay Convenience on the right with modern storefront colors.
- Add cafe/flower shop facades further down the street.
- Add palms, planters, benches, signboards, and warm shop interiors.
- Keep ocean visible at the end of the street.
- Replace flat gray surfaces with tiled paving and curb detail.

Acceptance:
- Screenshot from behind DaMogu immediately reads as modern seaside town.
- First view supports the story: she has arrived to begin independent life.
- Stores have cozy everyday function, not generic skyscraper scenery.

## Phase 5: Public Asset Policy

Goal: keep the future public repo legally clean.

Tasks:
- Treat VRoid sample assets as prototype/reference only unless their conditions are explicitly acceptable for the intended distribution.
- Final public DaMogu should be original.
- Keep private skins and copyrighted/IP references out of the public repository.
- Preserve license/source notes for every external asset used.

Acceptance:
- Public character assets are original or clearly license-compatible.
- Any local-only private skin remains ignored and uncommitted.
