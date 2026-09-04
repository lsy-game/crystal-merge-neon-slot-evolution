# 大蘑菇 Modly 候选输入包

本目录用于“按 Modly / Hunyuan3D 图生 3D 思路”继续尝试更完整的大蘑菇角色模型。

## 当前输入

- `damogu_modly_front_threeview_clean.png`：主输入，来自最终三视图正面，保留完整双臂、鞋、包和 hoodie 轮廓。
- `damogu_modly_front_detail_clean.png`：辅助主输入，来自建模细化参考正面，衣服和脸部细节更清楚。
- `damogu_modly_back_threeview_clean.png`：背面参考，用于 Blender/手工修模时确认后脑半丸子、包带、hoodie 背面和鞋跟。
- `damogu_modly_side_threeview_clean.png`：侧面参考，当前左边缘带入了前视图手部残影，不建议作为单图生成主输入。
- `damogu_modly_generation_prompt.txt`：给图生 3D 或手工建模工具使用的英文提示词和验收重点。

## 本轮判断

Modly / Hunyuan3D Mini 更适合生成静态造型网格。要做成 Unity 里能自然移动的角色，输出网格仍需要经过 Blender 清理、重拓扑或减面、骨架绑定、蒙皮、Unity Humanoid Avatar 检查和走路动画验证。

2026-09-01 已使用本机 Modly Hunyuan3D Mini 权重从 `damogu_modly_front_threeview_clean.png` 生成第一版 GLB：

- 源文件：`generated/1788268991_63b6dca6.glb`
- Unity 导入文件：`大蘑菇角色美术预览_Unity工程/Assets/BubuTown/Characters/Prototype/ModlyGenerated/DaMogu_ModlyHunyuan3D_FirstPass.glb`
- Unity 检查图：`大蘑菇角色美术预览_Unity工程/Assets/DaMoguArtPreview/Docs/DaMoguModlyHunyuan3D_CandidatePreview.png`
- Unity 检查报告：`大蘑菇角色美术预览_Unity工程/Assets/DaMoguArtPreview/Docs/DaMoguModlyHunyuan3D_CandidateReport.md`

检查结果：该 GLB 是单个静态网格，Unity 里统计为 `1 MeshFilter`、`0 SkinnedMeshRenderer`、`0 Animator`、`0 Avatar`。它可以挂在 `CharacterController` 下整体移动，但还不能像游戏角色一样自然走路、跑跳、摆手或做表情。

2026-09-01 又使用更干净的 `damogu_modly_front_detail_alpha.png` 生成第二版高参数 GLB：

- 源文件：`generated/1788272523_020ba9b2_frontDetail_alpha_balanced.glb`
- 生成参数：steps=30、octree=380、guidance=5.8、vertex_count=22000、seed=20260902
- Unity 导入文件：`大蘑菇角色美术预览_Unity工程/Assets/BubuTown/Characters/Prototype/ModlyGenerated/DaMogu_ModlyHunyuan3D_FrontDetailBalanced.glb`
- Unity 检查图：`大蘑菇角色美术预览_Unity工程/Assets/DaMoguArtPreview/Docs/DaMoguModlyHunyuan3D_FrontDetailBalancedPreview.png`
- Unity 检查报告：`大蘑菇角色美术预览_Unity工程/Assets/DaMoguArtPreview/Docs/DaMoguModlyHunyuan3D_FrontDetailBalancedReport.md`

检查结果：第二版文件体积从 2.1MB 增加到 5.1MB，但 GLB 结构仍是 `1 mesh`、`0 skins`、`0 animations`、`0 materials`。Unity 预览中它仍是静态连体轮廓，衣服、手、头发没有拆成可动画部件。结论是：Modly 可以继续作为外观探索和 Blender 雕刻/重拓扑参考，不应直接作为最终可移动角色导入主工程。

## 可动性结论

“模型能不能运动自如”取决于是否有骨架、蒙皮权重、Humanoid/VRM Avatar、动画和必要的物理骨骼。Modly 生成的 GLB 当前只满足“有静态网格”；它能随 `CharacterController` 整体平移/旋转，但不能自然摆手、迈腿、做表情或让衣服头发随身体动。

要走到游戏可用角色，下一步应选择：

1. 把 Modly 结果当参考，在 Blender 里清网格、拆衣服/头发/包、重拓扑、绑到现有 VRM/Humanoid 骨架，再导 FBX/VRM 回 Unity。
2. 或继续寻找可授权个人项目使用的 `.vroidcustomitem`、`.vroid`、`.blend`、`.fbx`，优先选已经有 skinned mesh / Humanoid / VRM 支持的资源。

## 2026-09-01 后续进展

已按第 1 条路线继续推进：在 `大蘑菇角色美术预览_Unity工程` 中重新导出 Blender skinned mesh，而不是继续把 Modly GLB 直接当可动角色。新版 Blender 脚本增加了 hoodie 侧面厚度、腋下过渡、前袋褶皱、hood 内衬边，并裁掉被衣服覆盖的原 VRM 身体面。Unity 重新导入后 `Avatar isHuman=True`、`isValid=True`，走路采样没有炸形。

当前结论不变：Modly 用作视觉参考和静态雕刻底模；游戏内运动自如的版本继续走 Blender/VRM/Humanoid skinned mesh。
