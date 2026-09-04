# DaMogu VRM Candidate Integration Report

Status: AvatarSample_A is installed as the current third-person DaMogu visual candidate for movement and camera evaluation.

Purpose: replace the rejected primitive/recolored prototype with a real anime-style Humanoid model so walking, running, camera height, and body readability can be judged in the Star Bay Town scene.

Source: `madjin/vrm-samples`, `vroid/stable/AvatarSample_A.vrm`.

License note: VRoid sample models can be used under the VRoid sample conditions. Copyright is not waived, and this candidate is a prototype/reference asset rather than the final original public DaMogu character.

Unity import route: UniVRM v0.131.2 through UPM packages `com.vrmc.gltf` and `com.vrmc.univrm`, matching the source model's VRM 0.x format.

Runtime scene hookup:
- Player root: `08_Player_And_Runtime/Player_Start_Bubu`
- Runtime visual child: `DaMogu_VRM_Candidate_Runtime`
- Candidate marker: `DaMogu_VRM_Candidate_A_AvatarSample_A`
- Animator controller: `Bubu_DaMogu_Locomotion.controller`

Motion preview outputs:
- `/Users/zhendian/Desktop/星湾镇_大蘑菇_VRM候选A_Walk动画预览.gif`
- `/Users/zhendian/Desktop/星湾镇_大蘑菇_VRM候选A_Run动画预览.gif`

Current art judgment:
- Better than the rejected primitive/recolored prototype because it is a real skinned anime Humanoid model.
- Still not final DaMogu: pants silhouette is too heavy for a cozy modern-life protagonist, the face/material response needs a softer toon setup, and the hairstyle should be redesigned toward a cleaner mature-cute look.
- The animation pipeline is useful, but final polish should add dedicated idle, walk, jog, tired walk, interact, sleep, bike mount, and work-minigame poses.

Next art pass recommendation: use this candidate to confirm locomotion feel, then create an original DaMogu VRoid/Blender model with a more mature face, cleaner hair silhouette, lighter lower-body silhouette, and pink-white casual seaside outfit.
