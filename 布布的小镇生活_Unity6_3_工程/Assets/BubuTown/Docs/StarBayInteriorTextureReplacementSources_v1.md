# 星湾镇室内贴图替换来源 v1

## 使用方式

- 当前版本按视频中的思路，先替换大面积表面：墙面、地板、窗帘布料、风格样板。
- 贴图只下载 1K JPG，用于原型阶段快速迭代。
- 材质由 `BubuTownStarBayInteriorPrototypeBuilder` 自动创建，并设置基础平铺与法线贴图。
- 为避免真实贴图过脏，当前主场景已使用 Unity 内生成的卡通化贴图作为主材质，Poly Haven 贴图保留为外部 CC0 来源与部分风格样板参考。

## Poly Haven CC0 贴图

- `diagonal_parquet`: 木地板、木地板风格样板。
  - https://polyhaven.com/a/diagonal_parquet
- `long_white_tiles`: 三楼小套间毛坯地砖。
  - https://polyhaven.com/a/long_white_tiles
- `plaster_grey_04`: 三楼小套间灰白毛坯墙面。
  - https://polyhaven.com/a/plaster_grey_04
- `beige_wall_001`: 旅店暖色墙面、部分自然风墙纸样板。
  - https://polyhaven.com/a/beige_wall_001
- `gingham_check`: 粉色墙纸、窗帘布料、蛋糕主题样板。
  - https://polyhaven.com/a/gingham_check

## Kenney CC0 家具

- `Furniture Pack`: 床、桌、椅、沙发、茶几、灯、厨房设备、盆栽等低模家具。
  - https://opengameart.org/content/furniture-kit
  - 本地许可证：`Assets/BubuTown/External/CC0/Kenney/FurnitureKit/Raw/License.txt`

## Poly Haven CC0 高质量模型

- `Sofa_01`: 客厅现代沙发。
  - https://polyhaven.com/a/Sofa_01
- `CoffeeTable_01`: 客厅茶几、旅店小桌。
  - https://polyhaven.com/a/CoffeeTable_01
- `WoodenChair_01`: 旅店靠窗椅。
  - https://polyhaven.com/a/WoodenChair_01
- `ClassicNightstand_01`: 旅店床头柜。
  - https://polyhaven.com/a/ClassicNightstand_01
- `potted_plant_01`: 客厅阳台盆栽。
  - https://polyhaven.com/a/potted_plant_01
- 这些模型使用 1K FBX/JPG，材质由 `BubuTownStarBayInteriorPrototypeBuilder` 自动绑定；对象名统一带 `高质量`、`CC0_PolyHaven` 中文标记，方便在 Unity 里搜索。

## Blender 生成主题家具

- 生成脚本：`Tools/GenerateStarBayBlenderFurniture.py`
- 输出目录：`Assets/BubuTown/External/Generated/StarBayFurnitureBlender/Models/`
- 当前生成模型：
  - `可爱粉色软包床.fbx`
  - `粉色圆角床头柜.fbx`
  - `云朵粉色梳妆台.fbx`
  - `粉色爱心小沙发.fbx`
  - `草莓圆桌.fbx`
  - `爱心绒毛地毯.fbx`
  - `壁挂星星灯.fbx`
  - `蛋糕奶油床.fbx`
  - `电竞霓虹电脑桌.fbx`
  - `现代圆角沙发.fbx`
  - `圆润小台灯.fbx`
  - `目标粉木单人床.fbx`
  - `目标原木床头柜.fbx`
  - `目标原木小茶几.fbx`
  - `目标海蓝小沙发.fbx`
  - `目标灰台面厨房.fbx`
  - `目标搬家纸箱堆.fbx`
- 用途：补足网上通用模型不够“星湾镇”的部分，先建立可爱粉色、蛋糕、电竞、现代简约的专属家具语言。模型和对象均为中文命名，Unity 中可直接搜索。
- 当前重点样板：`08_可爱粉色精修样板间`，作为后续主题房间完成度标杆。
- 当前目标房间：`09_目标图方向三楼单身公寓`，用目标图的灰砖、温白墙、大窗、粉木床、蓝沙发、原木茶几、纸箱和盆栽作为后续接入正式室内玩法的视觉标杆。

## Modly 图生 3D 输入图

- 本地扩展：`/Users/zhendian/Documents/Modly/extensions/hunyuan3d-mini/`
- 扩展说明：Hunyuan3D 2 Mini，输入图片输出 mesh；纹理需要后续接 Hunyuan3D Paint 或在 Unity/Blender 中单独处理。
- 输入图目录：`Assets/BubuTown/Textures/StarBayInterior/ModlyInput/`
- Modly 工作区整理包：`/Users/zhendian/Documents/Modly/workspace/StarBayInteriorFurnitureInputs/`
- 第一、二批输入图：
  - `目标公寓_粉木单人床_Modly输入图.png`
  - `目标公寓_海蓝小沙发_Modly输入图.png`
  - `目标公寓_原木小茶几_Modly输入图.png`
  - `目标公寓_灰台面粉柜厨房_Modly输入图.png`
  - `目标公寓_原木床头柜_Modly输入图.png`
  - `目标公寓_圆润台灯_Modly输入图.png`
  - `目标公寓_搬家纸箱堆_Modly输入图.png`
  - `目标公寓_窗帘落地窗_Modly输入图.png`
  - `星湾镇_Modly家具输入图_联系表_v2.png`
- 接入计划：`Assets/BubuTown/Docs/StarBayInteriorModlyImageTo3DPlan_v1.md`
- 已生成并接入的试件：
  - `Assets/BubuTown/External/Generated/StarBayFurnitureModly/Models/目标公寓_原木床头柜_Modly低清试件.fbx`
  - `Assets/BubuTown/External/Generated/StarBayFurnitureModly/Models/目标公寓_粉木单人床_Modly低清试件.fbx`
  - `Assets/BubuTown/External/Generated/StarBayFurnitureModly/Models/目标公寓_海蓝小沙发_Modly低清试件.fbx`
  - `Assets/BubuTown/External/Generated/StarBayFurnitureModly/Models/目标公寓_原木小茶几_Modly低清试件.fbx`
  - `Assets/BubuTown/External/Generated/StarBayFurnitureModly/Models/目标公寓_灰台面粉柜厨房_Modly低清试件.fbx`
  - `Assets/BubuTown/External/Generated/StarBayFurnitureModly/Models/目标公寓_圆润台灯_Modly低清试件.fbx`
  - `Assets/BubuTown/External/Generated/StarBayFurnitureModly/Models/目标公寓_搬家纸箱堆_Modly低清试件.fbx`
- 已镜像到运行时 `Resources` 的试件：
  - `Assets/Resources/星湾镇室内家具Modly/目标公寓_粉木单人床_Modly低清试件.fbx`
  - `Assets/Resources/星湾镇室内家具Modly/目标公寓_海蓝小沙发_Modly低清试件.fbx`
  - `Assets/Resources/星湾镇室内家具Modly/目标公寓_原木小茶几_Modly低清试件.fbx`
  - `Assets/Resources/星湾镇室内家具Modly/目标公寓_圆润台灯_Modly低清试件.fbx`
  - `Assets/Resources/星湾镇室内家具Modly/目标公寓_原木床头柜_Modly低清试件.fbx`
  - `Assets/Resources/星湾镇室内家具Modly/目标公寓_灰台面粉柜厨房_Modly低清试件.fbx`
  - `Assets/Resources/星湾镇室内家具Modly/目标公寓_搬家纸箱堆_Modly低清试件.fbx`

## Blender 生成家具运行时镜像

- `Assets/Resources/星湾镇室内家具Blender/蛋糕奶油床.fbx`
- `Assets/Resources/星湾镇室内家具Blender/电竞霓虹电脑桌.fbx`
- `Assets/Resources/星湾镇室内家具Blender/云朵粉色梳妆台.fbx`
- `Assets/Resources/星湾镇室内家具Blender/壁挂星星灯.fbx`
- 用途：让运行时装修模式优先显示主题家具模型，降低方块拼装感；缺失时仍回退到程序化模型，保证原型可运行。

## 贴图板覆盖

- 目标公寓窗外已采用 Quad 贴图板覆盖：`StarBayWindowBackdrop` 材质读取 `starbay_window_backdrop_painted_v1.png`。
- 用途：在正式模型全部到位前，先用大图覆盖低模/程序块区域，提高第一眼观感。
- 下一步适用区域：床品表面、厨房柜门正面、墙上画框、装饰书本和小海报。

## 本地路径

- 贴图目录：`Assets/BubuTown/Textures/StarBayInterior/CC0_PolyHaven/`
- 高质量模型目录：`Assets/BubuTown/External/CC0/PolyHaven/Models/`
- 高质量模型材质目录：`Assets/BubuTown/Materials/StarBayInterior/PolyHaven/`
- Blender 生成家具目录：`Assets/BubuTown/External/Generated/StarBayFurnitureBlender/Models/`
- Modly 生成家具目录：`Assets/BubuTown/External/Generated/StarBayFurnitureModly/Models/`
- Modly 运行时家具镜像：`Assets/Resources/星湾镇室内家具Modly/`
- Blender 运行时家具镜像：`Assets/Resources/星湾镇室内家具Blender/`
- Modly 原始 GLB 目录：`Assets/BubuTown/External/Generated/StarBayFurnitureModly/RawGLB/`
- Modly 输入图目录：`Assets/BubuTown/Textures/StarBayInterior/ModlyInput/`
- 星湾镇生成贴图目录：`Assets/BubuTown/Textures/StarBayInterior/Generated/`
- 中文家具 Prefab：`Assets/BubuTown/Prefabs/室内家具CC0_Kenney/`
- 室内原型场景：`Assets/Scenes/StarBayInteriorDecorationPrototype.unity`

## 当前生成贴图

- `clean_plaster_wall.png`: 干净灰白毛坯墙。
- `clean_grid_tile.png`: 干净浅色地砖。
- `pink_curtain_check.png`: 粉色格纹窗帘。
- `pink_heart_wallpaper.png`: 可爱粉色心形墙纸。
- `pink_room_wallpaper.png`: 可爱粉色精修房间细心形墙纸。
- `cake_sprinkle_wallpaper.png`: 蛋糕主题糖针墙纸。
- `esports_neon_wallpaper.png`: 电竞霓虹墙纸。
- `esports_dark_floor.png`: 电竞深色地板。
- `hotel_bedding_stripe.png`: 旅店床品条纹布料。
- `starbay_window_backdrop_painted_v1.png`: 目标公寓窗外星湾镇海边街景大图贴片。
