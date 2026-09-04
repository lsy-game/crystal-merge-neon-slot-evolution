# 《布布的小镇生活》Unity 原型交接说明

这是独立 Unity 工程，不依附旧的 WheatTown 或 DestinyRanger 工程。

## 工程信息

- Unity 版本：6000.3.23f1
- 升级方式：从 `布布的小镇生活_Unity工程` 复制出来的 Unity 6.3 测试副本，原 2022.3 工程未被覆盖。
- 原型场景：`Assets/Scenes/BubuTownPrototype.unity`
- 场景生成器：`Assets/BubuTown/Editor/BubuTownSceneBuilder.cs`
- 核心脚本：`Assets/BubuTown/Scripts/`

## 当前阶段

本阶段先满足“所有东西不按 Play 也能在 Hierarchy 里面看到”。场景现在由程序化城市底座、核心可交互建筑、Kenney CC0 城市模型层混合构成；仍有少量功能占位物件保留给任务、门、NPC、家具和存档闭环验证。

2026-08-26 更新：地图视觉从早期草地灰盒升级为现代化城镇街区底座，新增完整铺装、深色沥青道路、路缘、人行道、斑马线、车道线、公交站、现代路灯、玻璃店面、高楼楼群和窗户阵列。世界文字标签已挂 `BubuTownProximityLabel`，Play 时默认远处隐藏，主角靠近后才显示，避免画面被文字覆盖。

2026-08-27 更新：继续强化现代城市环境，新增连续商业街、周边背景高楼、停车位、车辆、自行车道标记、街边花箱、地铁入口、道路方向箭头、阳台、空调外机、广告灯箱和屋顶天线。所有世界 `TextMesh` 标签仍保留在 Hierarchy 中，但 Renderer 默认关闭，只有运行时主角靠近时才显示。预览图输出到 `Assets/BubuTown/Docs/ModernCityPreview.png`。

2026-08-27 资源导入更新：已从 Kenney 官方下载并导入 4 个免费 CC0 城市资源包：`City Kit Commercial 2.1`、`City Kit Roads 2.1`、`City Kit Suburban 2.0`、`City Kit Industrial 1.0`。资源路径为 `Assets/AssetStoreImported/ModernCity/Kenney/`，各包保留 `License.txt`。已运行 `BubuTown/Integrate Asset Store Modern City Prefabs`，把 80 个真实 FBX 城市模型混合进 `18_AssetStore_ModernCity_Integration` 场景分组，形成高楼外圈、商业内街、住宅补边、道路模块和街景细节层。

2026-08-27 近景现代化更新：核心可交互建筑高度和体量已提升，新增现代裙楼、玻璃角厅、分层楼板、街级灯带、入口铺装、学校玻璃学习中庭、家具店展示橱窗、小屋阳台露台和居民楼共享阳台；地面新增细密铺装缝、街区边缘铺砖和店铺前场铺装。新增街道高度截图输出 `Assets/BubuTown/Docs/StreetLevelPreview.png`，用于检查玩家近景视角。

2026-08-27 道具近景更新：NPC 已从单圆柱占位升级为低模角色组合，带脸部、眼睛、鞋、背包和职业小配件；前 5 个任务物件已升级为可识别的参观点、面粉袋、铅笔盒、落叶堆和小蛋糕组合；家具店商品已升级为展台陈列，床、桌、椅、地毯、台灯、书柜、装饰蛋糕和盆栽都有不同低模造型。新增道具细节截图输出 `Assets/BubuTown/Docs/PropsDetailPreview.png`。

2026-08-27 现代质感更新：程序化材质从糖果色/平涂进一步调整为更现代的深色沥青、冷灰混凝土、深玻璃、金属和发光灯带；核心建筑新增电子广告屏、竖向灯带、屋顶玻璃冠和幕墙分格；地图层新增数字广告牌、玻璃商业亭、路边信息屏和充电桩。`BubuTownAssetStoreCityIntegrator` 已预留 ITHappy `Cartoon City Free` 常见导入目录，等账号下载后可直接扫描并混合。

当前可见对象包括：

- 100m x 100m 现代城镇地面
- 中央广场、四向街道、环形道路、人行道、斑马线、公交站和现代路灯
- 蛋糕店、学校、家具店、玩家小屋、居民区、公园
- 现代化建筑外观：玻璃店面、招牌背板、电子广告屏、竖向灯带、窗户阵列、现代裙楼、玻璃角厅、分层楼板、屋顶玻璃冠、结构柱和周边高楼群
- 城市细节：深色沥青、细密铺装缝、街区边缘铺砖、连续商业街、背景楼、停车位、停靠车辆、自行车道、地铁入口、交通灯、数字广告牌、玻璃商业亭、充电桩、花箱、阳台、空调外机、广告灯箱和屋顶设备
- 免费真实模型：Kenney CC0 城市建筑、摩天楼、道路、路灯、遮阳棚、花箱和街边模型已导入并混合到场景中
- 第一版 5 个 NPC，已使用低模角色组合和职业小配件替代单圆柱占位
- 第一批 10 个任务标记，前 5 个标为优先闭环
- 公告牌任务批次：公告牌会按当前进度登记可接任务；Q001-Q005 立即可接，完成 3 个前置任务开放 Q006/Q007，完成前 5 个且温馨度 4 开放 Q008，温馨度 12 开放 Q009/Q010。
- 前 5 个任务还有具体可见目标物件：Q001 的 5 个小镇参观点、面粉袋、铅笔盒、三堆落叶、小蛋糕和配送目标，均保留交互根对象并添加低模视觉子物体。
- 欢迎来到小镇：Q001 主标记只作为任务说明，实际通过中央广场、蛋糕店、学校、家具店、玩家小屋 5 个可见参观点完成。
- 任务步骤：`BubuTownQuestStepMarker` 支持步骤数量和前置步骤要求，Q004 需要清理 3 堆落叶，Q005 必须先取蛋糕再送达。
- 课后小测验：学校细节分组里有 `After_School_Quiz_Station`，靠近按 E 开始 Q008 三选一测验，再按 1/2/3 作答；题目和选项在 Hierarchy 标签中可见，答对才完成任务。
- 小镇寻宝：公园细节分组里有 `Q009_Hidden_Treasure_Chest` 和 `Golden_Coin`，靠近按 E 会完成 Q009 原型寻宝。
- 今天也要回家：小屋室内有 `Go_Home_Today_End_Point`，靠近按 E 会完成 Q010、推进到下一天并保存。
- 我的第一个家：进入玩家小屋门会自动接取并完成 Q006。
- 家具店试营业：购买小椅子会自动接取 Q007，回小屋装修网格摆放小椅子会完成 Q007。
- 毛坯小屋内部、装修网格、基础灯、睡袋/纸箱
- 第一版 10 个家具商品，家具店陈列使用独立展台和不同低模家具造型。
- 家具解锁：家具店所有商品都在 Hierarchy 里可见，但购买会按小屋温馨度开放；0 度开放基础家具，4 度开放台灯/小书柜，12 度开放墙纸/地板/蛋糕摆件，20 度开放小盆栽。
- 极简 UI：`Bubu_HUD_Canvas`、小地图面板、右上菜单按钮、金币显示和底部提示条都是真实 Hierarchy 对象。
- 世界文字显示：所有 `TextMesh` 世界标签都保留在 Hierarchy 中，但 Renderer 默认关闭，运行时由 `BubuTownProximityLabel` 控制，主角靠近约 1.15-2m 后才显示。
- 互动提示：`Interaction_Prompt_Panel` 是独立 Hierarchy 对象，只在靠近 NPC、门、任务物品、家具或装修网格时显示；底部消息条保留给任务完成、奖励、保存等反馈。
- 展开菜单：`Expanded_Menu_Panel` 下有任务、背包、家具目录、装修、地图、设置、拍照、保存、小屋进度和关系 10 个真实子面板，默认 inactive，Play 时按 Tab/Esc 切换。
- 对话 UI：`Dialogue_Panel` 是真实 Hierarchy 对象，NPC 交互时显示名字、好感等级、对白和任务提示。
- NPC 关系：完成任务会给任务发布者增加好感，`Relationships_Panel` 会显示 0-3 的陌生/认识/熟悉/朋友状态。
- 好感对白：5 个 NPC 都有 0-3 级不同对白，运行时会按当前好感显示；每个 NPC 子物体下都有 `Friendship_Dialogue_Plan` 可见说明。
- 皮肤替换：`09_Character_Skin_System` 和每个角色的 `BubuTownSkinSlot` 都在 Hierarchy 中可见；公开原创资源放 `Assets/BubuTown/Characters/PublicOriginal/`，私人本地皮肤目录已被 Git 忽略。
- 地图系统：`10_Map_System` 下有 `Minimap_Camera` 和 `Map_Markers`，核心地点都有真实地图标记对象。
- 小屋进度系统：`11_Home_Progress_System` 下有 `Home_Progress_Manager`、可见温馨度规划板和 4/12/20 三个里程碑对象。
- 温馨度解锁：摆放家具达到 4/12/20 温馨度会触发一次性解锁消息，已解锁里程碑会写入存档并显示在小屋进度面板。
- 任务引导系统：`12_Quest_Guidance_System` 下有 `Quest_Guidance_Manager`、Q001-Q007/Q009/Q010 的世界引导标记；Q001 会按 5 个参观点顺序提示，HUD 有 `Current_Quest_Guide_Panel` 显示下一步提示。
- 小镇细节道具：`13_Town_Detail_Props` 下按地点放了广场长椅/路灯、蛋糕店展示台、学校跳格子、家具店户外样品、小屋邮箱、居民区围栏和公园野餐道具。
- 拍照模式：`14_Photo_Mode_System` 下有 `Photo_Mode_Manager` 和 3 个推荐拍照点；P 开关拍照模式，[/] 切换机位，拍照时隐藏常驻 HUD 并显示 `Photo_Mode_Panel`。
- 设置系统：`15_Settings_System` 下有 `Settings_Manager` 和 F1/F2/F3 三个可见设置块；可切换镜头灵敏度、跑步速度和极简 HUD 显示状态，并写入存档。
- 朋友来访系统：`16_Friend_Visit_System` 下有 `Friend_Visit_Manager` 和 5 个 `Home_Visitor_Spots`，温馨度 20 后小屋进度面板会显示朋友来访开放状态；站位对象在不按 Play 时也全部可见，靠近来访点按 E 会给出邀请/未开放反馈。
- 可运行对象：`Player_Start_Bubu`、`GameState_Manager`、`Interaction_Manager`、`Runtime_HUD_Manager`、`Save_Bootstrap_Manager`
- 可见门链接：玩家小屋门可进入室内，`Home_Interior_Exit_Door` 可返回小镇。
- 装修模式：靠近 `Decoration_Grid_8x6` 第一次按 E 打开装修模式，方向键移动预览格，Z/X 切换背包里未摆放家具，之后按 E 摆放当前家具。
- 装修预览：`Decoration_Grid_8x6/Placement_Ghost_Preview` 是真实 Hierarchy 对象，Play 时显示当前选中的未摆放家具；方向键选择 8x6 网格位置，Z/X 换家具，R 旋转 90 度，E 确认摆放。
- 家具摆放：每种家具 ID 会生成不同的简化 3D 组合造型，存档恢复也使用相同视觉定义，并记录网格格子和 90 度旋转方向。
- 家具选择：背包面板显示已买家具的“待摆放/已摆放”状态，装修面板显示当前选中的家具、待摆放数量和 Z/X 快捷键。
- 温馨度：摆放家具会按家具 ID 增加小屋温馨度，HUD 的 `Home_Progress_Panel` 会显示第 1 天目标和当前进度。

## Unity 菜单

打开工程后可运行：

- `BubuTown/Create Visible Prototype Scene`
- `BubuTown/Validate Visible Prototype Scene`
- `BubuTown/Integrate Asset Store Modern City Prefabs`
- `BubuTown/Capture Modern City Preview`
- `BubuTown/Capture Street Level Preview`
- `BubuTown/Capture Props Detail Preview`

## 后续开发顺序

1. 继续把剩余功能标记、室内装修件和角色外观替换为免费或原创 3D 资源。
2. 把 OnGUI 临时 HUD 替换为正式极简 UI。
3. 给任务增加更明确的步骤状态、引导箭头和奖励表现。
4. 给装修模式继续增加撤销、鼠标选格、家具拾起/移动和更正式的家具选择面板。
5. 不提交私人皮肤或版权 IP 素材。
