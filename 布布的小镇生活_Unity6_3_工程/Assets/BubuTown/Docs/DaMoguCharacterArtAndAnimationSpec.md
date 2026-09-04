# DaMogu Character Art And Animation Spec

## Character Direction

DaMogu is the original player character for Star Bay Town. The name is only a name; the character must not use mushroom visual motifs.

Current accepted direction:
- Female-presenting young adult.
- Soft mature anime face, avoiding a bean-like chibi face.
- Everyday modern seaside-town mood.
- Pink-and-white casual windbreaker.
- Clean white inner top.
- Dark navy skort or short skirt silhouette.
- Pink-white sneakers.
- Small crossbody bag.
- Rolling suitcase for the opening arrival sequence.
- Warm, independent, cozy personality: she came to Star Bay Town by choice to start a comfortable life on her own.

Reference concept images in project:
- `Assets/BubuTown/Characters/PublicOriginal/DaMogu/Concept/DaMogu_OriginalCharacterConcept_v0_9.png`
- `Assets/BubuTown/Characters/PublicOriginal/DaMogu/Concept/DaMogu_ThirdPersonGameplayMockup_v1_0.png`
- `Assets/BubuTown/Characters/PublicOriginal/DaMogu/Concept/DaMogu_TurnaroundModelSheet_v1_1.png`
- `Assets/BubuTown/Characters/PublicOriginal/DaMogu/Concept/DaMogu_LocomotionExpressionSheet_v1_2.png`
- `Assets/BubuTown/Characters/PublicOriginal/DaMogu/Concept/DaMogu_TargetThirdPersonSeasideLook_v2.png`
- `Assets/BubuTown/Characters/PublicOriginal/DaMogu/Concept/DaMogu_FinalModelTurnaroundDirection_v2.png`
- `Assets/BubuTown/Characters/PublicOriginal/DaMogu/Concept/DaMogu_ThirdPerson3DTargetPreview_v3.png`
- `Assets/BubuTown/Characters/PublicOriginal/DaMogu/Concept/DaMogu_ModelProductionDetailReference_v4.png`

## Target Third-Person Look

The final target is a polished modern seaside life-sim presentation, not the current temporary VRM sample quality.

Target composition:
- Third-person back camera with DaMogu centered in the lower middle of the frame.
- Bright modern seaside commercial street, with the ocean visible at the end of the street.
- Ground plane uses clean, warm-gray paving instead of flat graybox asphalt.
- Storefronts read as small everyday businesses: bakery, convenience store, cafe, flower shop, and lifestyle retail.
- Lighting is clear daytime sun with soft shadows, not dark or neon-heavy.

Target DaMogu silhouette:
- Human young woman, no mushroom motifs.
- Mature-cute anime proportions: head smaller than chibi, body around 1.58m to 1.65m.
- Short brown bob hair with a small half-up bun readable from behind.
- Pink-and-white hoodie/windbreaker as the first-read color block.
- Dark navy short skirt or skort, bare legs, pink-white sneakers.
- Small beige/pink crossbody bag with a diagonal strap.
- Movement silhouette should remain light and casual; avoid long black trousers, heavy skirts, or robe-like lower garments.

The generated target visual file `DaMogu_TargetThirdPersonSeasideLook_v2.png` should be treated as an internal art-direction target, not as a final game render or a licensed source model.

The generated turnaround file `DaMogu_FinalModelTurnaroundDirection_v2.png` is the current primary modeling reference for an original DaMogu model. Use it to guide VRoid, Blender, or commissioned/custom model work: front, side, and back proportions should stay consistent, with the same pink-white hoodie, navy skort/short skirt silhouette, bare legs, sneakers, short brown bob, small half-up bun, and crossbody bag.

The generated preview file `DaMogu_ThirdPerson3DTargetPreview_v3.png` is the current gameplay-view quality target. It is useful for checking camera composition, back silhouette, color blocking, and seaside-town mood. It should not be treated as proof that the final Unity model already exists.

The generated detail file `DaMogu_ModelProductionDetailReference_v4.png` is the current production-detail reference. It clarifies the front/back full-body read, face maturity, side hair construction, hoodie paneling, skort/short skirt shape, sneakers, and crossbody bag. Preserve its main silhouette, but simplify hair chunks, clothing folds, bag strap thickness, and accessory details enough for a practical animated Unity model.

## Third-Person Readability

The playable model should be readable at normal third-person life-game distance:
- Camera height target: around 2.4m to 2.7m above world ground relative to the player follow rig.
- Camera distance target: around 4.2m to 5.0m behind the player.
- Character screen height target: roughly 32% to 45% of vertical screen height during walking.
- Back silhouette must remain clear: hair shape, pink-white jacket block, dark lower garment, and light shoes should all read from behind.
- The outfit should avoid tiny details that only work in close-up.

## 3D Model Requirements

The final DaMogu model should be prepared as a Unity-friendly Humanoid character:
- Format: FBX, VRM, or Blender source exported to FBX.
- Rig: Unity Humanoid-compatible skeleton.
- Avatar: valid Humanoid Avatar in Unity.
- Root motion: off for the first prototype; movement is driven by `BubuTownPlayerController`.
- Scale: about 1.55m to 1.65m in world units.
- Pivot/root: feet on ground, forward along Unity +Z after import.
- Materials: URP Lit or toon-compatible materials, no magenta fallback.
- Textures: original or properly licensed, committed only if public-safe.

## Animation Baseline

The current prototype validates the animation pipeline with temporary Unity Standard Assets character data:
- Runtime animated child: `Humanoid_Locomotion_Runtime_Prototype`
- Controller: `Assets/BubuTown/Characters/Prototype/DaMogu_Locomotion/Bubu_DaMogu_Locomotion.controller`
- Driven parameters: `Speed`, `MoveX`, `MoveY`, `Grounded`, `Sprinting`
- Required first-pass clips: Idle, WalkForwards, RunForwards, SprintForwards
- Current desktop motion previews: `DaMogu_WalkMotionPreview_v0_2.gif`, `DaMogu_RunMotionPreview_v0_2.gif`
- Current readable-clothing previews: `DaMogu_WalkReadableClothingPreview_v0_3.gif`, `DaMogu_RunReadableClothingPreview_v0_3.gif`
- Current layered-clothing previews: `DaMogu_WalkLayeredClothingPreview_v0_4.gif`, `DaMogu_RunLayeredClothingPreview_v0_4.gif`

This temporary character is not DaMogu's final public identity. The final original model should retarget onto the same Animator pipeline.

The current VRM candidate proves that a VRM Humanoid model can be imported, shown in Unity 6.3, and connected to the locomotion controller. It does not meet the final look target because its lower-body silhouette is too heavy, its outfit is not the intended short-skirt seaside casual look, and its material response is only a readable prototype conversion.

Early DaMogu-specific animation targets:
- Idle: relaxed, curious, calm breathing.
- Walk: soft life-game pacing, readable arm swing, no aggressive adventure-game posture.
- Run: energetic but casual, not combat-focused.
- Tired walk: lower speed, slight shoulder drop, occasional yawn gesture.
- Arrival: holding or pulling the pink suitcase for the opening hotel/town arrival scene.

## First Modeling Pass

When creating or selecting the final model, prioritize:
1. Face and hair silhouette that still feels mature enough for third-person.
2. Back-view outfit readability.
3. Clean Humanoid retargeting.
4. Smooth walk/run blend without foot sliding.
5. Outfit physics later: hair tips, jacket hem, bag strap, skirt/skort secondary motion.

The first real model should use `DaMogu_ModelProductionDetailReference_v4.png` as the most specific visual guide, while using `DaMogu_ThirdPerson3DTargetPreview_v3.png` to verify how the model reads in actual gameplay camera framing.

Do not try to reach the final look by adding primitive overlay geometry on top of an unrelated sample body. The final quality target needs real character mesh, hair geometry, textures, rigging, and materials built around the accepted turnaround.

## Do Not Use

- Mushroom hat or mushroom-shaped hair.
- Oversized toddler/chibi head.
- Existing IP characters.
- Public repository commits of private or copyrighted skins.
- Logos, brand marks, or direct copies from downloaded reference characters.
