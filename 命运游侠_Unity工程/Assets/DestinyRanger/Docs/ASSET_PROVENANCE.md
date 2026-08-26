# 命运游侠 v0.1 图片资产来源清单

## 总原则

- 正式图片资产统一放在 `Assets/DestinyRanger/Art/Generated`。
- 当前正式图片资产均通过 Codex 的 `api-image` 生图流程生成或延展，不使用第三方游戏、影视、动漫、品牌、商标、截图、素材站或未授权图片。
- 市面游戏只作为玩法、UI 密度、触屏热区和打击反馈参考，不复制具体角色、图标、场景、Logo、UI 皮肤或可识别构图。
- 上架截图、宣传图和商店素材必须使用本工程内资产和 Unity 实机/编辑器渲染画面制作。
- 后续新增图片必须先记录到本文件；若不是 `api-image` 生成资产，必须补充明确授权来源、授权范围和可上架证明。
- OpenGameArt 等外部素材站只允许先进入候选/参考流程，具体候选和接入门槛见 `Assets/DestinyRanger/Docs/OPENGAMEART_ASSET_PLAN.md`；正式导入前必须确认 CC0 或补齐署名授权，并在本文件登记。

## 当前生成资产

| 文件 | 用途 | 验证重点 |
|------|------|----------|
| `Assets/DestinyRanger/Art/Generated/adventure-stage-forest.png` | 早期森林关卡背景 | 仅作历史/备用背景，不作为主预览优先资产。 |
| `Assets/DestinyRanger/Art/Generated/adventure-stage-forest-long-v1.png` | 横向推进主关卡长背景 | 与可踩平台、符文屏障、出口门搭配后不应像纯平地。 |
| `Assets/DestinyRanger/Art/Generated/adventure-hero-sheet.png` | 早期主角静态/基础动作表 | 备用，不作为当前主角动画优先表。 |
| `Assets/DestinyRanger/Art/Generated/adventure-hero-action-sheet-v2.png` | 早期主角攻击动作表 | 备用，用于比对打击姿态。 |
| `Assets/DestinyRanger/Art/Generated/adventure-hero-anim-32-v1.png` | 当前主角 8×4 动画表 | 必须覆盖待机、跑步、跳跃/下落、攻击、闪避、技能、受击/胜利等关键姿态；角色不能在游戏中消失或被伙伴遮挡。 |
| `Assets/DestinyRanger/Art/Generated/adventure-enemy-sheet.png` | 早期敌人基础图 | 备用。 |
| `Assets/DestinyRanger/Art/Generated/adventure-enemy-boss-action-v2.png` | 早期敌人/Boss 动作概念 | 备用，用于后续 Boss 动作细化。 |
| `Assets/DestinyRanger/Art/Generated/adventure-enemy-anim-24-v10-candidate.png` | 敌人 6×4 动画表候选 | 2026-08-17 通过 `api-image` 生成 v10：红帽蘑菇近战、蓝绿花弓手、蓝晶壳软泥、苔石遗迹守卫四行各 6 帧；已作为备用，不再由场景构建器默认引用。 |
| `Assets/DestinyRanger/Art/Generated/adventure-enemy-anim-24-v11-candidate.png` | 敌人 6×4 动画表弃用候选 | 2026-08-17 通过 `api-image` 生成 v11：动作和造型较好，但背景是渐变色而非纯抠底色，存在背景板风险，不接入正式场景。 |
| `Assets/DestinyRanger/Art/Generated/adventure-enemy-anim-24-v12-candidate.png` | 敌人 6×4 动画表备用 | 2026-08-17 通过 `api-image` 重新生成 v12：纯洋红 `#FF00FF` 抠底背景，红帽蘑菇近战、蓝花弓手、蓝晶壳软泥、苔石守卫四行各 6 帧；已降为备用，不再由场景构建器默认引用。 |
| `Assets/DestinyRanger/Art/Generated/adventure-enemy-anim-24-v13-clean-qstyle.png` | 敌人 6×4 动画表备用 | 2026-08-17 通过 `api-image` 基于森林背景风格参考生成 v13：纯洋红 `#FF00FF` 抠底背景，蘑菇近战、蓝叶弓手、低矮藤蔓爬虫、苔石守卫四行各 6 帧；已降为备用，不再由场景构建器默认引用。 |
| `Assets/DestinyRanger/Art/Generated/adventure-enemy-anim-24-v14-qstyle-clean.png` | 敌人 6×4 动画表备用 | 2026-08-18 通过 `api-image` 重新生成 v14：红晶哥布林近战、蓝叶弓手、水灵软泥、苔石守卫四行各 6 帧；已降为备用，不再由场景构建器默认引用。 |
| `Assets/DestinyRanger/Art/Generated/adventure-enemy-anim-24-v18-adventure-king-qstyle.png` | 敌人 6×4 动画表弃用候选 | 2026-08-18 通过 `api-image` 生成 v18：Q 版风格方向更接近横版冒险，但背景为多色渐变，背景板风险较高，不接入正式场景。 |
| `Assets/DestinyRanger/Art/Generated/adventure-enemy-anim-24-v19-flat-cyan-qstyle.png` | 敌人 6×4 动画表备用 | 2026-08-18 通过 `api-image` 重新生成 v19：红晶近战、苔藓弓手、蓝叶水灵、苔石巨像四行各 6 帧；已降为备用，不再由场景构建器默认引用。 |
| `Assets/DestinyRanger/Art/Generated/adventure-enemy-anim-24-v20-clean-browser-qstyle-alpha.png` | 当前敌人 6×4 动画表 | 2026-08-18 通过 `api-image` 参考当前森林遗迹背景色调生成 v20，并本地清理为 alpha 透明 PNG：红晶近战、苔叶弓手、水灵软泥、苔石守卫四行各 6 帧；轮廓更接近移动端横版 Q 版冒险风格，禁止出现背景板、文字、黄板或重影。 |
| `Assets/DestinyRanger/Art/Generated/adventure-companion-pet-v1.png` | 伙伴/宠物图 | 必须在排序层级和缩放上不遮挡主角。 |
| `Assets/DestinyRanger/Art/Generated/adventure-ui-sheet.png` | 通用 UI 装饰 | 只用于工程内幻想动作风格 UI。 |
| `Assets/DestinyRanger/Art/Generated/adventure-platform-sheet.png` | 早期可踩平台、地形装饰 | 备用。 |
| `Assets/DestinyRanger/Art/Generated/adventure-platform-sheet-v4-clean.png` | 可踩平台 4×1 候选 | 2026-08-18 通过 `api-image` 生成：纯洋红抠底，质感较好，但顶部有少量过细草尖/横向杂点，作为备用候选，不再由场景构建器默认引用。 |
| `Assets/DestinyRanger/Art/Generated/adventure-platform-sheet-v5-clean-v2.png` | 可踩平台 4×1 备用图集 | 2026-08-18 通过 `api-image` 生成：纯洋红 `#FF00FF` 抠底背景，厚草苔石平台完整，但实机预览仍能看到少量平台外框感；已降为备用，不再由场景构建器默认引用。 |
| `Assets/DestinyRanger/Art/Generated/adventure-platform-sheet-v6-no-blackline.png` | 可踩平台 4×1 备用图集 | 2026-08-18 通过 `api-image` 生成：RGBA 透明背景，黑线和横向残影更少，主体为明亮草皮浮岛；已降为备用。 |
| `Assets/DestinyRanger/Art/Generated/adventure-platform-sheet-v7-solid-qstyle.png` | 可踩平台 4×1 备用图集 | 2026-08-18 通过 `api-image` 基于森林背景风格参考生成：厚草苔石实心平台，但实机预览中存在少量横向残线/背景框观感，已降为备用。 |
| `Assets/DestinyRanger/Art/Generated/adventure-platform-sheet-v8-painted-solid.png` | 当前可踩平台 4×1 图集 | 2026-08-23 使用 OpenGameArt CC0 候选结构参考，并通过 `api-image` 生成统一画风草石平台概念后本地裁切成 4×1 透明图集：草皮、石块和土层更厚实，目标是让主角脚下完整覆盖、平台边缘清晰、无背景框。 |
| `Assets/DestinyRanger/Art/Generated/adventure-ground-wall-v2-painted-solid.png` | 当前主地面墙体图 | 2026-08-23 从 `adventure-platform-sheet-v8-painted-solid.png` 的长地面块派生并本地重排为透明主地面覆盖图：用于替换旧主地面墙体，让玩家脚下的大地板和空中平台保持同一草石材质语言。 |
| `Assets/DestinyRanger/Art/Generated/adventure-control-ui-sheet.png` | 早期触屏按钮 | 备用。 |
| `Assets/DestinyRanger/Art/Generated/adventure-hud-controls-v2.png` | 当前移动端 HUD/按钮 | 右侧攻击、跳跃、闪避、技能和 SLOT 启封按钮应靠近右拇指热区且避开安全区。 |
| `Assets/DestinyRanger/Art/Generated/adventure-rune-ui-sheet.png` | SLOT/命运符文界面 | 必须表达符文启封，不出现真钱、下注、赔率赔付或现金兑换暗示。 |
| `Assets/DestinyRanger/Art/Generated/adventure-combat-vfx-sheet.png` | 斩击、命中、雷、电、冰、闪避等特效 | 标准档需要明确打击感；舒适低闪档需要降低闪白和震屏但保留可读性。 |
| `Assets/DestinyRanger/Art/Generated/destiny-ranger-concept.png` | 早期概念图 | 仅内部概念参考。 |
| `Assets/DestinyRanger/Art/Generated/destiny-ranger-sprite-sheet.png` | 早期角色/物件表 | 备用。 |
| `Assets/DestinyRanger/Art/Generated/destiny-ranger-rune-ui.png` | 早期符文 UI | 备用。 |
| `Assets/DestinyRanger/Art/Generated/destiny-ranger-rune-icons-sheet.png` | 符文图标表 | 图标含义需要和剑气、闪避、冰、暴击、回旋、电弧、属性增益对应。 |

## 当前授权外部特效资产

以下素材来自用户提供的已授权 Spine 特效素材库，当前优先以 `0/*.png` 序列帧方式接入，不依赖 Spine runtime；`.skel/.atlas/.spine` 原始文件暂不导入工程。正式上架前需保留原授权文件或采购记录。

| 文件夹 | 来源目录 | 当前用途 | 验证重点 |
|------|------|------|------|
| `Assets/DestinyRanger/Art/ExternalVfx/E53138_QuickSlash` | `032 MYNSZD E53138/0` | 普攻快速横斩刀光 | 不遮挡主角身体，挥砍方向跟随角色朝向。 |
| `Assets/DestinyRanger/Art/ExternalVfx/E53137_SwordArc` | `031 MYNSZD E53137/0` | 第三段攻击和剑气轮廓 | 与主角光剑色调统一，不能像独立贴图浮在前景。 |
| `Assets/DestinyRanger/Art/ExternalVfx/E53073_LightningSword` | `020 MYNSZD E53073/0` | `jian` 剑气飞行、`shandian` 雷击/雷阵 | 按文件名前缀筛选使用，避免不同 Spine 部件混播。 |
| `Assets/DestinyRanger/Art/ExternalVfx/E53110_HeavyBurst` | `025 MYNSZD E53110/0` | 第三段重击、精英破势爆点 | 只取连续爆点组，控制透明度和震屏，避免画面过脏。 |
| `Assets/DestinyRanger/Art/ExternalVfx/E53069_RuneEnergy` | `016 MYNSZD E53069/0` | SLOT 启封、符文罐破裂、逆运祝福 | 光效应保持“命运符文”包装，不出现博彩机台元素。 |
| `Assets/DestinyRanger/Art/ExternalVfx/E53130_BossImpact` | `027 MYNSZD E53130/0` | Boss 砸地、Boss 击破、大型爆发 | 爆点需要服务招式读性，不遮挡红色危险预警。 |

## 上架检查口径

- App Store 审核备注中可说明：游戏主体美术为项目原创生成资产，位于 `Assets/DestinyRanger/Art/Generated`；授权外部战斗特效位于 `Assets/DestinyRanger/Art/ExternalVfx`，不包含第三方 IP、品牌素材或现实博彩素材。
- `SLOT` 相关图片只包装为“命运符文启封”，不得出现硬币下注、真钱面额、赌场标识、现金提款、奖金池、赔率牌或真实机台元素。
- 对外商店页截图优先展示：横版推进地图、主角动作帧、右手移动端按钮、符文启封界面、Boss 战和结算构筑总结。
- 每次新增或替换图片后，必须重新运行 `ValidateHeroAnimationArtBatch` 和 `ValidateAppStoreReadinessBatch`，并人工检查主角可见性、按钮安全区、低闪模式和 SLOT 口径。
- 主角帧表必须同步满足 `Assets/DestinyRanger/Docs/HERO_ANIMATION_SPEC.md`，包括 32 帧分配、动作 FPS、命中帧和技能释放帧。
