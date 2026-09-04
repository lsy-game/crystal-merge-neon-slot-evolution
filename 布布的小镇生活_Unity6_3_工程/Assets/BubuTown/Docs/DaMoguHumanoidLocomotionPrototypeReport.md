# DaMogu Humanoid Locomotion Prototype Report

Status: imported for temporary animation-pipeline validation.

Source: Unity-Technologies/Standard-Assets-Characters, female third-person rig and locomotion clips.

License: Unity Companion License for Unity-dependent projects. This is suitable as a Unity-only prototype dependency, not the final public character identity.

Imported assets:
- `Rig/defaultfemale_rig.fbx`
- `Animations/Exploration/f@Idle.fbx`
- `Animations/Exploration/f@WalkForwards.fbx`
- `Animations/Exploration/f@RunForwards.fbx`
- `Animations/Exploration/f@SprintForwards.fbx`

Runtime scene hookup:
- Player root: `08_Player_And_Runtime/Player_Start_Bubu`
- Runtime animated child: `Humanoid_Locomotion_Runtime_Prototype`
- Playable visual marker: `DaMogu_Playable_Model_v1`
- Animator controller: `Bubu_DaMogu_Locomotion.controller`
- Parameters driven by `BubuTownLocomotionAnimator`: `Speed`, `MoveX`, `MoveY`, `Grounded`, `Sprinting`

Playable model v1 note: the current in-scene DaMogu visual uses a continuous skinned Humanoid body with a rough recolored DaMogu palette texture, plus small head/hair and bag readability helpers. It is suitable for movement feel, camera distance, and gameplay validation, but it is not the final polished anime model.

Art direction note: the final DaMogu model should still be an original anime-style seaside urban character, then retargeted onto this same Animator pipeline.
