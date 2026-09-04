# 星湾镇室内美术资源筛选 v1

日期：2026-08-31

## 筛选标准

- 优先 CC0、MIT、Apache、官方免费包或明确允许商业项目使用的免费资源。
- 优先低模、现代、温馨、可模块化替换材质，适合 Unity 6 / URP。
- 只记录候选，不直接导入，避免许可证或风格混杂。
- 后续资源建议放入 `Assets/BubuTown/External/CC0/` 或单独候选目录。

## 第一优先候选

### Kenney Furniture Kit

- 来源：https://kenney.nl/assets/furniture-kit
- 备用来源：https://poly.pizza/bundle/Furniture-Kit-NoG1sEUD1z
- 许可证：CC0 / Public Domain
- 商用：可以
- 改造：可以
- 格式：OBJ、PNG、Unity package；Poly Pizza 侧提供 FBX / GLTF
- 适合内容：床、桌、椅、沙发、书柜、厨房、浴室、墙面装饰
- 星湾镇用途：最适合先替换旅店房间和三楼小套间灰盒家具，统一低模风格。

### 3D Interior Home Assets

- 来源：https://opengameart.org/node/172343
- 许可证：CC0
- 商用：可以
- 改造：可以
- 格式：ZIP
- 适合内容：低模房间家具、床、床头柜、书架、沙发、长椅、桌子、花瓶、画框、书
- 星湾镇用途：补充墙画、书、花瓶、沙发、床头柜等生活感小件。

### Wooden Furnitures

- 来源：https://opengameart.org/content/wooden-furnitures
- 许可证：CC0
- 商用：可以
- 改造：可以
- 格式：FBX、Blend
- 适合内容：木质桌椅、床、长椅、柜子、架子
- 星湾镇用途：木质自然风格家具包候选，适合旅店和“木质自然”装修风格。

## 可用但需谨慎

### Quaternius Free Assets

- 来源：https://quaternius.com/
- 许可证说明：https://quaternius.com/license.html
- 许可证：QAL，允许免费用于个人、教育、商业项目，可修改；不可把资源本身重新打包转卖或再分发为素材包。
- 商用：可以
- 改造：可以
- 格式：通常有 FBX、OBJ、glTF
- 星湾镇用途：如果需要补充现代小件、灯具、电子设备，可以筛选具体包；不是 CC0，导入时必须保留来源和许可证说明。

### Cute Furniture FREE - Low Poly 3D Models Pack

- 来源：https://www.fab.com/listings/3eca0616-aa73-4348-9af5-cb4c173f237e
- 价格：Free
- 格式：FBX、GLB、OBJ
- 适合内容：卧室、客厅、厨房、浴室、植物、灯、电子设备
- 风险：Fab 页面需要进一步确认具体 license terms；暂不建议直接导入正式目录。

## 系统/工具参考

### SunnyValleyStudio Grid Placement System

- 来源：https://github.com/SunnyValleyStudio/Grid-Placement-System-Unity-2022
- 许可证：MIT
- 用途：学习网格放置、预览物体、对象数据库、放置状态机。

### object_placement_unity

- 来源：https://github.com/manlaig/object_placement_unity
- 许可证：MIT
- 用途：学习运行时碰撞检测、格子匹配、放置验证。

### B 站网格放置教程

- 来源：https://www.bilibili.com/video/BV1K14y1U73E/
- 用途：学习网格位置计算、显示网格、预览、碰撞检测、状态模式和删除物体。

## 推荐下一步

1. 先下载并试导入 Kenney Furniture Kit，放在 `Assets/BubuTown/External/CC0/Kenney/FurnitureKit/`。
2. 只挑旅店和小套间需要的 15-25 个模型，建立中文 prefab 名称，例如 `旅店_单人床_木质`、`小套间_基础桌`、`墙上画框_海景`。
3. 用当前 `StarBayInteriorDecorationPrototype.unity` 替换程序化家具，同时保留 `FurnitureId`，避免玩法脚本重写。
4. 再用 3D Interior Home Assets 补生活小件：画框、书、花瓶、床头柜。
5. 等 B 站视频到位后，把它的搭建流程整理成 Blender/Unity 具体步骤，再决定是否做墙体/门窗模块精化。
