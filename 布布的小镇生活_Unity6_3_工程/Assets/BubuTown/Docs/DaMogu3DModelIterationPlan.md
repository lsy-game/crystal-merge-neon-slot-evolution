# DaMogu 3D Model Iteration Plan

## Intent

DaMogu should gradually reach the polished third-person 3D look shown in `DaMogu_ThirdPerson3DTargetPreview_v3.png`, while staying practical for Unity 6.3, VRM or FBX import, Humanoid retargeting, and cozy life-sim locomotion. The current most detailed production reference is `DaMogu_ModelProductionDetailReference_v4.png`.

## Current Visual Target

- Overall mood: modern anime seaside-town life-sim.
- Camera read: centered third-person back view, calm walking pace, street and ocean visible ahead.
- Character read: pink-white hoodie first, short brown bob and small half-up bun second, navy skort/short skirt and pink-white sneakers third.
- Personality read: independent, gentle, slightly mature, not toddler-like or bean-faced.

## What v3 Gets Right

- Back silhouette is much closer to the user's target screenshot.
- Pink-white hoodie, dark lower garment, bare legs, sneakers, and crossbody bag are clear.
- Hair shape is readable from behind and suitable for third-person framing.
- The street composition supports Star Bay Town's seaside commercial-street direction.

## What Needs More Work

- Front face should become slightly more mature and less doll-like.
- Legs should be checked against a realistic game rig so the walk cycle does not look floaty.
- Hoodie volume should be modeled as soft clothing, not a bulky shell.
- Skort/short skirt needs enough thickness and modesty for running, stairs, bike riding, and camera tilt.
- Crossbody bag and strap should be modeled as separate readable shapes, then given limited secondary motion later.
- Hair should be designed in chunks that can receive light spring-bone motion without clipping the hood.

## v4 Production Notes

- Use the v4 front and back bodies for outfit proportion and silhouette.
- Use the v4 close-up face as the target for a softer mature anime expression, while checking it again from gameplay distance.
- Use the v4 side hair panel as a construction guide, but reduce strand count if needed for clean VRM spring bones.
- Model the hoodie as layered clothing with a hood, cuffs, hem, and zipper read; do not over-model tiny wrinkles.
- Keep the skort or short skirt practical for running, stairs, and future bike riding.
- Build the crossbody bag as a clear separate accessory, with a strap thick enough to read but not so thick that it clips constantly.

## Technical Path

1. Keep the current VRM candidate only as a locomotion pipeline proof.
2. Build an original DaMogu model in VRoid or Blender using the turnaround reference.
3. Export as VRM or FBX with a valid Unity Humanoid Avatar.
4. Retarget to `Bubu_DaMogu_Locomotion.controller`.
5. Verify idle, walk, run, sprint, tired walk, and camera framing in Unity.
6. Add secondary motion only after the base walk/run reads well.

## First Unity Motion Check

- Back-view idle: hair bun, bob shape, hoodie color block, bag strap, and shoes are readable.
- Walk: feet contact the ground cleanly, arms do not clip the hoodie or bag.
- Run: skort/short skirt keeps coverage and does not visually merge into a dark block.
- Turn-in-place or quick turn: hair and bag do not swing through the torso.
- Camera: character occupies about 32% to 45% of screen height during walking.

## Current Motion Proof v0.2

The project now keeps two desktop motion previews generated from Unity Humanoid animation sampling:

- `DaMogu_WalkMotionPreview_v0_2.gif`
- `DaMogu_RunMotionPreview_v0_2.gif`

These previews prove the current Unity-side locomotion pipeline can drive a Humanoid character with walk and run loops. This is not the final DaMogu model. The next production step is replacing the temporary model body with an original VRoid or Blender model that matches v4, then using the same motion-preview process to check whether the walk still reads smoothly.

## Readable Clothing Motion Proof v0.3

The project also keeps a rough readable-clothing motion pass:

- `DaMogu_WalkReadableClothingPreview_v0_3.gif`
- `DaMogu_RunReadableClothingPreview_v0_3.gif`

This pass intentionally adds the outfit in simple, readable layers before visual polish: pink-white hoodie, navy skort/short skirt, bare-leg read, pink-white shoes, brown hair mass, and crossbody strap/bag. It is not final art. Its purpose is to make sure the outfit stays visible during movement before spending time on better modeling.

The next clothing passes should improve one layer at a time:

1. Hoodie body and white upper panel.
2. Skort/short skirt silhouette and coverage.
3. Shoes and foot contact.
4. Hair shape and half-up bun.
5. Crossbody strap and bag scale.
6. Face proportions after the body read is stable.

## Layered Clothing Motion Proof v0.4

The project keeps a cleaner layered-clothing pass:

- `DaMogu_WalkLayeredClothingPreview_v0_4.gif`
- `DaMogu_RunLayeredClothingPreview_v0_4.gif`

This pass responds to the first clothing readability review: the previous arm-attached sleeve capsules created strange extra strips around the upper body, and the early bag block confused the hand area. v0.4 removes those distracting pieces, keeps the body underlay for continuous motion, and focuses on the readable base outfit: white upper hoodie panel, pink lower hoodie panel, navy skort/short skirt, legs, shoes, hair mass, and clearer hands.

The crossbody bag is intentionally deferred. Add it back only after the hand and sleeve area reads cleanly during walk and run.

For each model iteration, review:

- Silhouette during walk from the back.
- Foot contact and visible foot sliding.
- Arm swing clipping against hoodie sleeves and crossbody bag.
- Hair, hood, and bag overlap during walk and run.
- Whether the character still feels cozy and everyday, not combat-focused.
- Whether the hoodie, lower garment, shoes, and bag are readable without pausing the animation.
- Whether any added sleeve, strap, or accessory creates stray strips around the torso or hands.

## Acceptance For First Real Model

- Imports into Unity 6.3 without magenta materials.
- Character height is about 1.58m to 1.65m.
- Camera back view matches the v3 preview's silhouette at normal walking distance.
- Walk loop has no obvious foot sliding at prototype speed.
- Face avoids the earlier bean/chibi impression.
- Public version uses only original or license-compatible assets.
