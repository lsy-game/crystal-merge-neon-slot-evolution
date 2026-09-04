# 星湾镇室内与装修原型

- Scene: `Assets/Scenes/StarBayInteriorDecorationPrototype.unity`
- Preview: `Assets/BubuTown/Docs/StarBayInteriorDecorationPrototypePreview_v1.png`
- Desktop preview: `/Users/zhendian/Desktop/星湾镇_室内装修原型预览_v1.png`

## 窗口 C 已交付范围

- 星湾旅店房间灰盒：床、门、窗、小桌、行李、床头灯。
- 商住楼三楼第一间小套间：灰色毛坯地板、空墙、基础吸顶灯、厨房区、客厅/卧室分区、纸箱和睡袋。
- 装修模式原型：主角站在屋内，显示网格，`E` 摆放，方向键移动，`R` 90 度旋转，`Z/X` 切换家具。
- 墙上物品会吸附到最近墙面并抬到墙面高度。
- 第一版家具风格：可爱粉色、木质自然、现代简约、蛋糕主题、电竞主题。
- 墙纸、地板、灯光预设可用 `C`、`V`、`B` 切换。

## 美术资源

- 已下载并解压 Kenney Furniture Pack：`Assets/BubuTown/External/CC0/Kenney/FurnitureKit/Raw/`。
- 许可证文件：`Assets/BubuTown/External/CC0/Kenney/FurnitureKit/Raw/License.txt`，标注 Creative Commons Zero, CC0。
- 场景内关键家具已替换为 Kenney FBX：床、圆桌、椅子、水槽、灶台、冰箱、沙发、茶几、植物、灯。
- 已生成中文命名 Prefab 库：`Assets/BubuTown/Prefabs/室内家具CC0_Kenney/`。

## 当前美术迭代

- 小套间主材质已换为星湾镇卡通化贴图：干净灰白墙、浅色地砖、粉色格纹窗帘、粉色心形墙纸样板。
- 旅店使用木地板贴图、暖色墙面、条纹床品、窗帘褶皱和基础软装。
- 已继续模型化门窗和厨房：门套、窗框、厨房上下柜、电视柜、墙上书本摆件。
- 已新增 `06_装修后主题房间预览`，包含可爱粉色、木质自然、电竞主题三套预览角。

## 接入说明

- 程序化灰盒仍作为主结构；Kenney CC0 模型作为候选美术层，后续可以逐步替换为正式家具 Prefab。
- Hierarchy 名称已改为中文，方便在 Unity 里搜索：`星湾镇_室内与装修原型`、`三楼毛坯小套间`、`星湾旅店房间灰盒`、`装修模式_运行控制器`。
- 后续家具目录可继续使用这些运行时 ID：`fur_pink_bed`, `fur_pink_vanity`, `fur_natural_bed`, `fur_natural_table`, `fur_modern_sofa`, `fur_modern_floor_lamp`, `fur_cake_bed`, `fur_cake_wall_shelf`, `fur_esports_desk`, `fur_esports_wall_poster`。
