# 命运游侠 v0.1 上架准备清单

## 当前隐私口径

- 运行方式：离线单机。
- 账号：不创建账号，不要求登录。
- 网络：当前试玩版不接入联网排行、云存档、广告、分析 SDK 或远程配置。
- 本机保存：只使用 `PlayerPrefs` 保存设置、试炼热度、教程状态、最佳通关时间、最高击败记录、今日试炼本机最佳和本地成就。
- 本地存档规格：固定键、生成键族、schema 版本和清除行为记录在 `Assets/DestinyRanger/Docs/LOCAL_SAVE_SPEC.md`；当前 `DestinyRanger.SaveSchemaVersion = 1`。
- 删除路径：标题页或设置页打开“关于/隐私”，点“清除本机数据”，二次确认后删除上述 `PlayerPrefs` 项并恢复默认设置。
- App Privacy 建议口径：未收集数据。
- 隐私标签规格：`Assets/DestinyRanger/Docs/APP_PRIVACY_LABEL_SPEC.md` 记录 App Store Connect 建议填写、依据和需要重新评估的触发条件。
- 依赖验证：`ValidateNoThirdPartySdkBatch` 会扫描 `Packages/manifest.json`、`UnityConnectSettings.asset`、`ProjectSettings.asset` 和运行时代码，确认无广告、分析、IAP、第三方统计 SDK 或联网 API。
- 上架元数据：正式提交前必须按 `Assets/DestinyRanger/Docs/RELEASE_METADATA_SPEC.md` 准备隐私政策 URL 和支持 URL，并替换游戏内 `TODO_PRIVACY_POLICY_URL` / `TODO_SUPPORT_URL` 占位。
- 最终门禁：真实公开链接替换完成后，必须运行 `ValidateFinalSubmissionBlockingBatch`；如果运行时代码或上架元数据里仍有隐私/支持链接占位，它会直接失败。

## 随机奖励口径

- `SLOT` 是战斗内命运符文表现，不含真钱付费或现金价值。
- 当前只消耗本局金币、命运能量、房间奖励或战斗掉落触发。
- 当前启封概率：约 25% 三星、57% 二星、18% 逆运祝福。
- 无共鸣时触发逆运祝福，恢复生命作为保底。
- 若未来加入 IAP 或任何付费随机虚拟物品，必须在购买前披露概率并重新审查平台政策。

## 内容分级建议口径

- 暴力表现：幻想/卡通动作战斗，敌人受击闪白、击退、消散，不表现血液、肢解或现实伤害细节。
- 武器表现：玩家使用幻想光剑和魔法技能，不提供真实武器教学。
- 博彩表现：不选择 Gambling；不选择 Simulated Gambling。`SLOT` 只作为符文奖励 UI，不模拟下注、赔付、现金兑换或现实博彩。
- 随机奖励：可披露为 chance-based rewards / loot boxes 风格的战斗内随机虚拟奖励，无 IAP。
- 建议分级预期：按 App Store Connect 问卷实填结果决定，当前内容目标为适合 9+ 或同等低龄向幻想动作评级，而不是 13+/17+ 博彩向评级。

## 审核备注建议

- 游戏类型：横版动作 Roguelite，核心循环为战斗清房、房间奖励、符文启封、构筑变化。
- 随机元素说明：符文启封只影响本局战斗构筑，不可兑换现金或现实物品。
- 随机结果说明：符文启封后会显示本次构筑变化、总构筑等级前后变化、金币补给或逆运回血保底；局内 `RuneOddsText` 会在临界、待启封、商店可启封或启封面板打开时显示三星/二星/逆运概率和无付费现金价值说明；标题页/暂停页的符文图鉴也会在 `RuneCodexCompliance` 显示概率和无现金价值说明，帮助玩家理解随机结果的战斗意义。
- SLOT 期待说明：右上 `NextSlotHintText` 只提示本局命运能量、临界状态、商店差额和平台/极限闪避充能来源，不涉及真钱、广告激励、现实奖品或跨局付费收益；`RuneAnticipationThreshold` 只改变 UI 期待感，奖励仍等 100% 才召出。
- 内容分级说明：幻想动作战斗，无血腥或真实博彩；SLOT 是符文奖励表现，不是 simulated gambling。
- 数据说明：所有记录仅保存在设备本地。
- 链接说明：隐私政策 URL 和支持 URL 将在正式提交前替换为公开可访问链接，且与游戏内“关于/隐私”面板一致。
- 发布阻断提示：开发版“关于/隐私”面板显示 `AboutReleaseBlockerText`，通过 `ReleaseBlockerStatusText` 提醒仍含 TODO 链接且动作帧显示正式默认关闭；正式提交前该提示应在替换真实链接后同步调整，并通过 `ValidateFinalSubmissionBlockingBatch`。
- 操作说明：iPad 横屏，左侧虚拟摇杆，右侧攻击/跳跃/闪避/技能/启封按钮。
- 手柄说明：支持蓝牙/MFi 控制器作为可选输入，触屏仍是默认主操作；战斗 UI 不硬显示固定手柄 glyph，避免与系统重映射不一致，手柄输入会同步屏幕按钮反馈。
- 素材来源说明：正式图片资产为项目原创生成资产，来源和用途记录在 `Assets/DestinyRanger/Docs/ASSET_PROVENANCE.md`；不使用第三方游戏、影视、动漫、品牌、商标、截图、素材站或未授权图片。
- 可访问性说明：设置页提供辅助模式、自动朝向、可选自动攻击、震动开关、音量分轨、按钮大小/位置/透明度、单键拖动微调和舒适低闪档；辅助模式会降低受到伤害并放宽闪避容错；触控布局规格记录在 `Assets/DestinyRanger/Docs/TOUCH_CONTROL_SPEC.md`，右侧战斗按钮有最小热区、安全边距保护、`ThumbReachSafeZone` 拇指热区微调提示和 `ApplyHitConfirmSafeLayout` 命中 HUD 避让，左摇杆会用 `JoystickIntentText` 确认移动、横划闪避和上推跳跃意图；打击反馈规格记录在 `Assets/DestinyRanger/Docs/FEEDBACK_SPEC.md`，震动可关闭且按事件分层节流；世界层只保留极淡前景可读性带、空中落脚环、Boss 锁场边界、实体草皮/土层平台和受击方向短线，主角/敌人脚下黑影与蓝光环保持透明禁用，降低复杂背景下的识别压力；危险区使用斜纹、十字准星、波纹线、`!!`/“瞄”/“毒”符号和倒计时文字，不只靠红/绿颜色表达；受击后 `HurtRecoveryText` 会短暂提示短暂无敌恢复窗口和下一步动作；Boss 战顶部 `BossResponseHintText` 会在高危预告之外持续提示保留闪避、收招反击和符文技能窗口；右手战斗区左上方提供真实命中确认 HUD 和二次按钮回弹，空挥不显示，帮助移动端玩家确认输入结果；设置顶部徽章会即时显示标准/低闪、自动/手动朝向、右手按钮状态、控制器连接状态、离线单机和无广告/无分析；标题页成就面板只显示本地 PlayerPrefs 进度。
- 复玩说明：失败瞬间会先播放 `SpawnPlayerDefeatCeremony`，结算页 `VictoryCauseText` 会在胜利时显示本局亮点、失败时显示失败原因和优先改进动作；结算页会显示本局徽章、符文履历、房间奖励选择、基于本局表现生成的“下局挑战”“问题诊断”“路线复盘”“构筑复盘”和“推荐构筑”；构筑复盘会给当前流派、评分、主轴符文和短板；路线复盘和本地路线成就会追踪树冠阶梯、断桥跳点、符文高脊和 `RouteArchivist` 特殊路线收集；`VictoryAssistNoteText` 会说明辅助模式只改变动作容错、本地成就仍按同一条件记录；成就页会显示本地“推荐追踪”目标和未解锁成就进度，今日试炼还会显示是否刷新本机今日最佳，用于说明短局 Roguelite 的复玩目标。
- 首局说明：首次开始试炼会显示可跳过简报，提示核心循环、辅助模式、自动朝向/自动攻击和 `SLOT` 无真钱付费/现金价值；设置页 `SettingsOnboardingReplay` 可重新查看简报，回放模式通过 `onboardingReplayMode` 返回设置，不启动新对局；局内 `FirstRunNudgeText` 会在 SLOT 宝箱、平台小宝箱、商店和首个 SLOT 未启动时给短提示；失败结算会显示基于表现生成的操作建议。
- 路线说明：当前房间有平台小宝箱时，`PlatformRouteHintText` 在小地图内提示“箱 右3m · 跳上打箱 / 攻击开箱”这类短状态，帮助玩家理解地图不是纯平地，并把平台奖励和 SLOT 启封循环连接起来。
- 房门说明：商店、精英和 Boss 房门有 `RoomGateSpecialBadge` 与 `RoomGateSpecialBand`，用“补给/精英/BOSS”持续提示房型，不只依赖颜色或小地图节点。
- 离线今日试炼说明：标题页“今日试炼”只使用设备本地日期生成固定种子，并只保存今日试炼本机最佳；不接账号、联网排行、竞赛奖励或抽奖活动。
- 试炼热度说明：标题页“试炼热度”是 H0-H3 离线自选难度，只提高敌人生命、伤害和移动速度；`TitleModeGuideText` 会提示 H2+ 是挑战热度、今日试炼只保存本机最佳，并明确热度不改变 SLOT 概率，不接排行，不发竞赛奖励，不与付费内容关联。

## 上架前必须验证

- Unity 正确项目：`/Users/zhendian/Documents/New project/命运游侠_Unity工程`
- 构建设置验证：`DestinyRanger.EditorTools.DestinyRangerSceneBuilder.ValidateReleaseSettingsBatch`
- 无 SDK/联网验证：`DestinyRanger.EditorTools.DestinyRangerSceneBuilder.ValidateNoThirdPartySdkBatch`
- 上架口径验证：`DestinyRanger.EditorTools.DestinyRangerSceneBuilder.ValidateAppStoreReadinessBatch`
- 最终提交阻断验证：`DestinyRanger.EditorTools.DestinyRangerSceneBuilder.ValidateFinalSubmissionBlockingBatch`
- 本地存档验证：`DestinyRanger.EditorTools.DestinyRangerSceneBuilder.ValidateLocalSaveSchemaBatch`
- 预览构建：`DestinyRanger.EditorTools.DestinyRangerSceneBuilder.RenderPrototypePreviewBatch`
- 战斗预览：`DestinyRanger.EditorTools.DestinyRangerSceneBuilder.RenderCombatPreviewBatch`
- 检查关于/隐私入口：标题页和设置页均可打开。
- 检查 App Privacy 标签：按 `APP_PRIVACY_LABEL_SPEC.md` 填写“未收集数据”；若接入 SDK、联网、账号、Game Center、IAP、推送或上传任何本机记录，必须重新评估。
- 检查上架链接：正式提交前 `TODO_PRIVACY_POLICY_URL` 和 `TODO_SUPPORT_URL` 必须替换为公开可访问的真实链接。
- 检查发布阻断提示：开发阶段 `AboutReleaseBlockerText` 必须清楚提示 TODO 链接会阻断提交，正式提交前必须与真实链接状态一致。
- 检查开发调帧显示：`HeroActionBeatHud` 必须默认关闭；设置页可手动开启用于真机调帧，但正式包不能默认显示内部帧号/FPS 调试信息。
- 检查最终门禁：只有在真实隐私政策 URL 和支持 URL 已填入游戏内“关于/隐私”与 `RELEASE_METADATA_SPEC.md` 后，才运行 `ValidateFinalSubmissionBlockingBatch`；开发阶段保留 TODO 时该验证应失败。
- 检查今日试炼：同一天重复进入“今日试炼”显示同一今日种子，并在标题页显示今日最佳通关时间/最高击败；普通“开始试炼”仍生成随机种子；两者均不联网、不排行。
- 检查标题页模式说明：`TitleModeGuideText` 显示普通/今日/热度/辅助推荐，不遮挡开始按钮，并包含本机最佳、不联网排行和 SLOT 概率不变口径。
- 检查清除本机数据：第一次点击给确认提示，第二次点击清除设置、教程状态和成绩记录。
- 检查本地存档 schema：新增 `PlayerPrefs` 键必须同步 `LOCAL_SAVE_SPEC.md`、`LocalPlayerPrefKeys` 和 `ValidateLocalSaveSchemaBatch`；清除后标题页统计、今日最佳、教程状态和本地成就都恢复默认。
- 检查符文面板、局内 `RuneOddsText` 和符文图鉴：显示概率、无真钱付费、无现金价值、无广告激励或联网排行。
- 检查素材来源：`ASSET_PROVENANCE.md` 已覆盖当前 `Assets/DestinyRanger/Art/Generated` 正式图片，且无第三方 IP、品牌、素材站或未授权图片。
- 检查商店面板：显示只消耗本局金币，并用 `ShopDecisionText`、`ShopHealAdvice` / `ShopRuneAdvice` 解释回血、启封、离开或攒金币的局内战术用途。
- 检查房间奖励面板：`RoomRewardTypeBadge` 区分输出/生存/机动/经济/SLOT，`RoomRewardRecommend` 会根据当前局势标出一张推荐奖励，并保留三选一自由选择。
- 检查角色可见性：主角不被伙伴或特效遮挡。
- 检查右手按钮：iPad 横屏下不贴安全区边缘，不遮挡关键战斗区域。
- 检查舒适低闪：屏幕闪白、震屏、低血红边明显弱于标准档，但命中和危险提示仍可读。
- 检查设置徽章：切换低闪、自动朝向、右手位置或按钮微调后，设置顶部徽章能即时反映舒适/朝向/按钮/离线无广告状态。
- 检查应用生命周期：对局中切后台/锁屏后返回，游戏停在暂停面板并显示“应用恢复：战斗已冻结，触控输入已清空”，计时不推进，长按攻击、摇杆、跳跃缓冲、闪避缓冲和短离地宽容不残留；商店、奖励选择、关于/隐私等模态界面返回后保持冻结。
- 检查结算复玩：死亡时先显示倒下来源反馈；胜利和失败结算都显示 `VictoryCauseText`、“下局挑战”“问题诊断”“推荐构筑”和 `VictoryAssistNoteText`，并且内容与本局表现相关，不是固定文案。
- 检查成就追踪：标题页打开成就面板时显示“推荐追踪”和未解锁成就进度，且该目标会随本地成就解锁情况变化；不接账号、联网排行或 Game Center。
- 检查今日结算：今日试炼结束后结算页显示今日记录是否刷新和当前今日最佳 `时间/K击败`，且没有联网排行、分数排名或奖励领取入口。
