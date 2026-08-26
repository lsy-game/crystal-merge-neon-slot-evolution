# OpenGameArt 候选素材计划

## 使用原则

- 只优先使用 `CC0` 候选素材；`CC-BY` 或其它授权素材暂不进入正式工程，除非补齐署名、授权截图和上架说明。
- OpenGameArt 素材先作为结构和风格参考，不直接硬贴进场景。正式进入 `Assets/DestinyRanger/Art/Generated` 前，必须完成统一画风转制、边缘清理、色调融合和实机截图验证。
- 不使用包含第三方 IP、品牌、Logo、其它游戏角色、现实博彩元素或无法确认授权的素材。
- 最终导入 Unity 前先放到候选区，确认尺寸、alpha、切片和内存表现，避免资源导入导致 Unity batch 卡死。

## 当前优先候选

| 候选 | URL | 授权 | 适合用途 | 接入策略 |
|------|-----|------|----------|----------|
| 2D Platformer Forest Pack | https://opengameart.org/content/2d-platformer-forest-pack | CC0 | 森林草地、平台、树木、植物 | 优先参考地板厚度、草皮边缘和植物摆放，转制成当前森林遗迹 Q 版画风。 |
| Free 2D Block Forest Tile Pack | https://opengameart.org/content/free-2d-block-forest-tile-pack | CC0 | 方块草地、土层、基础 tile | 参考实心地板结构，解决“主角脚下透背景”和平台边缘不完整。 |
| Pixel Art Platformer Asset Pack | https://opengameart.org/content/pixel-art-platformer-asset-pack | CC0 | dirt、rocks、ruins、trees、foliage | 只取地形构成和废墟装饰思路，避免直接使用像素风原图导致画风不统一。 |
| 2D Platformer Enemies | https://opengameart.org/content/2d-platformer-enemies | CC0 | 小怪轮廓和低复杂度动作参考 | 仅参考敌人体型和动画分段，正式小怪仍使用项目统一 Q 版透明 PNG 动作表。 |
| Slash | https://opengameart.org/content/slash-0 | CC0 | 斩击特效参考 | 当前已有授权 Spine 特效，OpenGameArt 斩击只作备选，不优先导入。 |

## 转制目标

- 地板：优先生成或转制为厚草皮 + 石块/土层 underside 的 4-8 帧平台图集，主角脚下必须是完整实体块，不允许只靠一条碰撞线。
- 平台：上表面要清楚，侧边和底部有自然块面，不出现黑线、透明框、细长灰线或背景残留。
- 空中装饰：采用完整云雾、树冠、废墟块面贴图，不再用大量细线型程序矩形模拟藤、桥、根须。
- 小怪：轮廓要 Q 版、颜色和当前森林背景统一；受击时应进入硬直，不在受击帧继续攻击。
- 特效：普通攻击、第三段重击、技能释放必须清晰分层；禁止大面积糊光、残影背景板和遮挡主角身体。

## 导入门槛

1. 记录原始 URL、授权类型、下载日期和原始文件名。
2. 候选素材先进入 `Assets/DestinyRanger/Art/Candidates/OpenGameArt`，不得直接覆盖 `Generated` 正式资产。
3. 统一处理为 PNG RGBA 或明确 chroma key，并通过边缘检查；任何文字、小图标或背景板必须移除。
4. 使用 `Tools/prepare_opengameart_platform_candidates.py` 先输出候选平台图集；确认视觉统一后，才能生成正式图集。
5. 生成/转制后的正式图集再登记到 `ASSET_PROVENANCE.md`。
6. 运行 `bash Tools/verify_destiny_ranger_static.sh`。
7. 用 Unity 渲染地图/战斗预览并人工检查：脚下地板完整、无空气墙、无黑线、无脏框、UI 不遮挡角色。

## 下一步

- 先从 `2D Platformer Forest Pack` 和 `Free 2D Block Forest Tile Pack` 中挑地形参考，做一版更厚、更清楚的草石地板/平台图集。
- 再根据 `2D Platformer Enemies` 的简单轮廓节奏，重生成更贴合当前背景的小怪动作表。
