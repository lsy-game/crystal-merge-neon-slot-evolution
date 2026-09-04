# 星湾镇多窗口成果 GitHub 整合索引 v1

更新日期：2026-09-04

## 主工程

后续换电脑继续开发时，优先打开这个 Unity 工程：

`/Users/zhendian/Documents/New project/布布的小镇生活_Unity6_3_工程`

Unity 版本：`6000.3.23f1`

## 已整合窗口

### 室内窗口

位置：

`Assets/Scenes/StarBayInteriorDecorationPrototype.unity`

主要脚本：

`Assets/BubuTown/Editor/BubuTownStarBayInteriorPrototypeBuilder.cs`

主要模型与贴图：

`Assets/BubuTown/External/Generated/StarBayFurnitureBlender/Models`

`Assets/BubuTown/External/Generated/StarBayFurnitureModly`

`Assets/BubuTown/External/Generated/Textures`

当前室内原型包含第一间三楼小套间、旅店房间、家具摆放网格、90 度旋转、贴墙物品、墙纸/地板/灯光切换，以及可爱粉色、木质自然、现代简约、蛋糕主题、电竞主题家具方向。

### NPC 人物模型窗口

已复制进主工程：

`Assets/BubuTown/Integrated/NPC人物模型窗口/星湾镇NPC人物模型原型`

包含 NPC 预览场景、prefab、材质、贴图、站牌版、投影贴图版、Modly 生成/优化尝试、行走探测资源和说明文档。

### 主角美术模型窗口

位置：

`Assets/BubuTown/Characters/Prototype`

包含大蘑菇主角 VRoid 样例、贴图覆盖、URP 材质、Modly 候选图、Modly 生成 GLB、Blender/FBX 原型、行走控制器和探测 prefab。

### 美术窗口

位置：

`Assets/BubuTown/Characters/PublicOriginal`

`Assets/BubuTown/Docs`

包含主角概念图、三视图/目标效果图、视觉风格说明、街道/家具/室内阶段性预览和资源使用记录。

## GitHub 同步说明

仓库远端：

`https://github.com/lsy-game/my-game-backup.git`

本次只保存可迁移的工程资产、脚本、场景、材质、模型、贴图和文档；Unity 自动生成目录如 `Library`、`Temp`、`Logs`、崩溃 dump、本地安装包和导出包不会进入 Git。

换电脑后建议步骤：

1. 克隆 GitHub 仓库。
2. 用 Unity Hub 打开 `布布的小镇生活_Unity6_3_工程`。
3. 等 Unity 重新生成 `Library`。
4. 从 `Assets/BubuTown/Docs/星湾镇_多窗口成果GitHub整合索引_v1.md` 找各窗口成果位置。
5. 室内原型优先打开 `Assets/Scenes/StarBayInteriorDecorationPrototype.unity`。
