# StarBay NPC Modly Rig Pipeline v1

## 当前验证

- 本地 `Models/Generated/*.glb` 是 Modly/Hunyuan3D 生成的单网格资产：`skins=0`，`animations=0`。
- 它们可以在 Unity 里作为整体物体移动、旋转、碰撞，但不能自然摆臂、迈腿、眨眼或做表情。
- `Models/TexturedProjection/*.obj` 已经比方块/程序体更像角色，但前投影会导致侧面贴图拖影，衣服和头发仍然像“画在表面上”。

## 要变成可自由运动角色

1. 先确定一版更像参考图的静态造型：头身比、短发团子、粉白连帽外套、深蓝裙裤、粉白运动鞋、斜挎包。
2. 在 Blender/Modly/VRoid 路线里把衣服拆成真实分件网格：外套主体、帽子、袖口、下摆罗纹、拉链、抽绳、裙裤褶片、鞋底和鞋面。
3. 给角色做 Humanoid 骨架，至少包含 hips/spine/chest/neck/head、左右上臂/前臂/手、左右大腿/小腿/脚。
4. 做自动权重后手动修权重：手指、袖口、裙裤边缘、肩膀、头发尾端和斜挎包带是重点。
5. 导入 Unity 后配置 Avatar，再接 Idle/Walk/Run 动作片段和第三人称控制器。

## 美术继续细化的优先级

- 衣服不要只靠贴图：需要真实厚度、袖口和下摆凸起、拉链窄条、口袋开口边、帽子内外层。
- 头发不要整块糊住：需要刘海束、侧边麻花/发辫束、后脑小丸子、发尾片状层次。
- 手部要优先修比例和权重：手掌略小、手腕自然收窄，手指分开并能随动作弯曲。
- 包带最好独立网格并绑定到胸/肩/胯附近骨骼，不要贴在身体纹理上。

## 可执行路线

- 短期：继续用 Modly 静态网格做造型探索，在 Unity 里验证轮廓、比例和色彩。
- 中期：用 Blender 给最佳静态网格自动绑骨，导出 FBX，检查 Unity 是否能识别 SkinnedMeshRenderer。
- 长期：如果想接近参考图，需要做真正的 VRoid custom item 或 Blender 分件 skinned mesh，而不是只把图片投影到一整块模型上。

## 2026-09-01 RigProbe 可动性实测

本轮把截图里的三个 Modly/Hunyuan3D 静态单网格候选做成了“临时骨架/权重走路探针”，用于回答“这种方式做出来后能不能动起来”。

- 新增 Blender 工具：`/Users/zhendian/Documents/New project/星月湾小镇房屋试看版/Tools/Blender/build_modly_npc_rig_probe.py`。
- 输入资产：`TownAdmin_Modly_FirstPass.glb`、`HotelOwner_Modly_FirstPass.glb`、`BakeryOwner_Modly_FirstPass.glb`，原始状态都是 `skins=0`、`animations=0`。
- 输出资产：`Assets/StarBayTown/NPCPrototype/Models/RiggedProbe/*_RigProbe.blend/.fbx/.glb`。
- 骨骼结果：三个人都生成 17 根基础人形骨骼，并生成 `RigProbe_WalkInPlace` 循环动画片段。
- Unity 结果：新增 `BuildModlyRigProbeWalkPreview.cs`，导入 FBX 后为三个人生成 `AnimatorController`，并创建 `StarBayNPC_ModlyRigProbeWalk.unity`。
- 验证结果：Unity 2022.3.62f3c1 batchmode 通过；日志确认 `TownAdmin`、`HotelOwner`、`BakeryOwner` 均为 `Avatar isHuman=True`、`isValid=True`。
- 预览图：`Assets/StarBayTown/NPCPrototype/Docs/StarBayNPC_ModlyRigProbeWalkPreview.png`，桌面同步输出到 `/Users/zhendian/Desktop/星月湾小镇NPC可动性测试输出/StarBayNPC_ModlyRigProbeWalkPreview.png`。

结论：这种生成模型可以通过 Blender 自动骨架和权重处理变成 Unity 可动 Humanoid，并能播放基础走路循环；但这只是可行性探针，不是最终绑定。因为原模型是一整块高面数融合网格，衣摆、手、头发、托盘/道具在走路时容易被一起拉伸。后续若要正式使用，应把角色拆成身体、衣服、头发、道具等分件网格，再在 Blender 里手工 retopo、UV、权重和减面。

## 2026-09-02 GameProbe 共享动画补充

本轮继续把 RigProbe 往更接近游戏入场的方向推进：

- 新增 `RiggedGameProbe` 轻量输出，使用 Blender Decimate 将三个 NPC 从约 6.5-7.6 万顶点压到约 1.84-2.12 万顶点。
- Unity 场景 `StarBayNPC_ModlyGameProbeSharedWalk.unity` 中，三个 NPC 都挂同一个来自 `TownAdmin` 的 `RigProbe_WalkInPlace` 片段。
- Unity 日志确认三者仍为 `Avatar isHuman=True`、`isValid=True`，并且控制器使用共享 walk clip。
- 预览图：`/Users/zhendian/Desktop/星月湾小镇NPC可动性测试输出/StarBayNPC_ModlyGameProbeSharedWalkPreview.png`。

结论：生成模型路线不仅能“单独绑骨后动”，也能进入 Humanoid 共享动画管线。它可以作为后续 NPC/角色方案继续推进；正式使用前仍必须做拆件、贴图、权重修正和进一步减面。
