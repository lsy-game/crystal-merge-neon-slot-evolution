# 大蘑菇可玩 3D 角色当前状态

## 当前结论

已经在 Unity 6.3 工程里跑通了一个可移动、可播放 Humanoid 动画的 3D 角色候选版本。

当前版本用途是验证第三人称移动、镜头、动画控制器、VRM 导入和换皮管线。它不是最终原创大蘑菇模型，也还没有完全达到参考图里的粉白外套、深蓝短裙、粉白运动鞋、斜挎包、短发半丸子头造型。

## 已接入资源

- Unity 工程：`/Users/zhendian/Documents/New project/布布的小镇生活_Unity6_3_工程`
- 主场景：`Assets/Scenes/BubuTownPrototype.unity`
- 运行时玩家根节点：`BubuTown_Prototype_All_Visible_Before_Play/08_Player_And_Runtime/Player_Start_Bubu`
- 当前可动角色子节点：`DaMogu_VRM_Candidate_Runtime`
- VRM 源文件：`Assets/BubuTown/Characters/Prototype/VRoidSamples/DaMogu_VRM_Candidate_A_AvatarSample_A.vrm`
- 导入 prefab：`Assets/BubuTown/Characters/Prototype/VRoidSamples/DaMogu_VRM_Candidate_A_AvatarSample_A.prefab`
- 动画控制器：`Assets/BubuTown/Characters/Prototype/DaMogu_Locomotion/Bubu_DaMogu_Locomotion.controller`
- 预览图：`Assets/BubuTown/Docs/DaMoguVrmCandidatePreview.png`
- 集成报告：`Assets/BubuTown/Docs/DaMoguVrmCandidateIntegrationReport.md`

## 已验证内容

- VRM 候选模型能由 UniVRM 导入为 Unity prefab。
- 模型已经挂到玩家根节点下面。
- Animator 已绑定 `Bubu_DaMogu_Locomotion.controller`。
- Root motion 已关闭，移动由 `BubuTownPlayerController` 驱动。
- `BubuTownLocomotionAnimator` 已和玩家控制器、VRM Animator 连接。
- Unity 批处理验证通过：`Renderers=3, Controller=Bubu_DaMogu_Locomotion`。

## 动作能力

当前 locomotion controller 支持：

- Idle
- WalkForwards
- RunForwards
- SprintForwards

运行时参数：

- `Speed`
- `MoveX`
- `MoveY`
- `Grounded`
- `Sprinting`

这意味着现在已经可以作为游戏内第三人称角色跑起来，用于测试走路、跑步、镜头跟随、场景尺度和交互距离。

## 与目标参考图的差距

当前候选模型的主要差距：

- 下半身轮廓偏重，不是目标图的深蓝短裙或 skort。
- 发型还不是短棕 bob 加小半丸子头。
- 粉白外套只做到了可读的原型材质，不是最终服装网格。
- 斜挎包、粉白运动鞋、外套分色、袖口和裙摆都需要真实建模。
- 角色脸部和材质还需要更柔和的 toon/anime 调整。

## 下一步做成最终大蘑菇的建模要求

要达到参考图质量，下一步应在 VRoid Studio 或 Blender 中制作原创模型：

- 按 `DaMogu_ModelProductionDetailReference_v4.png` 锁定正面、背面、侧面比例。
- 重新制作短棕 bob、侧边发束和小半丸子头。
- 制作粉白连帽外套或风衣，保留清楚的大色块。
- 制作深蓝短裙或 skort，避免长裤轮廓。
- 制作粉白运动鞋、袜子和小斜挎包。
- 绑定 Unity Humanoid Avatar。
- 导出 VRM 或 FBX。
- 复用当前 `Bubu_DaMogu_Locomotion.controller` 做 retarget。

## 版权边界

当前 VRM 候选来自 VRoid sample，用于本地原型验证。它可以帮助确认技术管线，但最终公开版本最好使用原创模型或明确可公开分发的授权资产。

公开仓库不应提交私人皮肤、版权 IP 角色或未确认授权的模型。
