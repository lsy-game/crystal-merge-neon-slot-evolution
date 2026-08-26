# 命运游侠 v0.1 移动端触控布局规格

## 对标目标

触控布局按 iPad 横屏右手拇指区设计，参考移动端动作 Roguelite 的可自定义按钮、长按攻击、自动朝向/自动攻击和冷却可读性标准。默认布局必须能直接玩，设置项用于适配不同手型，而不是弥补默认布局问题。

## 默认右手战斗区

- 攻击键最大，位于右下主拇指热区，并尽量贴近右侧安全边距，符合手游右手主按键预期。
- 跳跃和闪避靠近攻击键左上侧，保证攻击后能快速切到跳跃/闪避；默认布局使用“双层弧形”，不能把防御动作推到屏幕中部，也不能因为右手位置档位把跳跃键压到攻击键下方。
- 正式触控 HUD 只显示一个核心技能：`Skill1` 剑气，`Skill2` 炎弹、`Skill3` 冰环与 `Skill4` 雷击作为内部扩展技能暂时隐藏，避免右手区和技能特效过载；`SLOT` 启封/补给键保持在技能区外侧，战斗中能看到但不会抢占攻击键；`ApplyVisibleSkillSet` 必须通过 `MaxVisibleCombatSkills = 1` 锁住显示数量，`ControlReachOffsetForButton` 必须按攻击、跳闪、技能、SLOT 分组缩放右手偏移，避免整组等量右移造成按钮重叠。
- 主屏幕按钮只显示移动端语义：攻击、跳跃、闪避、剑气、炎弹、冰环、雷击、启封/补给/充能；键盘提示只放在暂停说明。
- 攻击键支持点按和长按连续攻击，三段连斩进度点直接显示在攻击键内。
- 攻击键内保留 `AttackRhythmWindowFill` 连段节奏条和三颗连击点，正式 HUD 隐藏 `AttackRhythmWindowText`，用颜色和进度表达当前 `comboTimer` 剩余时间；第二段时节奏条变蓝强调破势窗口，但不再堆“连段 / 快断 / 破势”小字。
- 攻击、跳跃和闪避支持触屏预输入，但正式 HUD 隐藏攻击、跳跃和闪避预输入微文案；`AttackBufferText`、`JumpBufferText`、`DodgeBufferText` 作为内部锚点保留，默认置空，避免按钮内文字挤成一团。输入确认改由按钮短促回弹、连击点、冷却数字和角色实际动作表达。
- 跳跃键必须由 `AttachJumpButtonEvents` 在 PointerDown 阶段立刻 `RequestJump`，PointerUp/PointerExit 调用 `CutJumpHeight`；短按低跳、长按高跳，不能等松手才起跳。
- 技能键必须绑定 `AttachSkillPreviewEvents`：PointerDown 显示 `SkillAimPreviewGuideLine_NoPlate`、`SkillAimPreviewEdgeLine`、`SkillAimPreviewMidTick_NoPlate` 或 `SkillAimPreviewThinAreaLine` 这类青白细线，PointerUp/PointerExit、释放技能、弹窗或输入锁定时调用 `HideSkillAimPreview`；冷却中或魔力不足时不能显示可释放范围。
- 可见技能键必须支持短预输入：`RequestSkill` 在冷却即将结束或普攻/动作锁末端时写入 `SkillInputBufferDuration` 或 `SkillChainInputBufferDuration`，`CastSkill` 通过 `SkillChainWindowLocked` 避免技能抢掉当前刀帧，`TryConsumeBufferedSkills` 一次只消费一个可用技能；等待期间 `Skill1StateText` 只显示“待发”，隐藏技能不得显示冷却、MP 或高亮，排队和释放只生成 `SkillChainQueuedTick` / `SkillChainQueuedRing`、`SkillBufferConsumedTick` / `SkillBufferConsumedRing` 和脚底短线反馈。
- 攻击、跳跃、闪避、技能和 `SLOT` 键必须绑定 `AttachTouchDownFeedback`，按下瞬间触发 `TouchDownEchoDuration = .075f` 的外圈青白回弹；`ApplyTouchDownEcho` 最高 alpha `.10f`，缩放只允许 `1.015 + .026` 的小幅扩张，该反馈只做视觉确认，不抢占真实命中、受击或奖励震动。

## 热区和安全区

- `CombatControlMinTouchSize` 是右侧战斗按钮最小热区，当前不低于 122px，仍高于 44pt 基础触控要求。
- `CombatControlSafeMargin` 是右手按钮距离安全区边缘的最小中心边距，当前为 96px，底部手势边距为 124px，攻击键仍贴近右侧但必须完整显示并离开底部系统手势区。
- `ClampCombatControlPosition` 必须在 `SetControlButton` 中统一调用，保证默认布局、紧凑布局、右手位置档位和单键拖动微调都会被夹紧。
- 所有右侧按钮挂在 `SafeAreaRoot` 下，运行时跟随 `Screen.safeArea`，不能进入圆角、刘海或系统手势边缘。
- `ThumbReachSafeZone` 只在单键拖动微调时显示，用低透明区域标出右手拇指热区；`ThumbReachSafeZoneText` 必须提示按钮留在热区内且 `HitConfirmText` 会避让右手热区，避免玩家调完布局后真实命中 HUD 被手指或技能按钮挡住。
- 标准/紧凑布局默认坐标必须偏右但分层清楚：攻击键接近安全右边界但不能裁切，跳跃/闪避贴近攻击键左上，单个可见技能更靠右上，`RuneOpen` 宝箱键靠上偏内侧形成小竖列，标准基准为 `Attack (-36,188)`、`Jump (-226,218)`、`Dodge (-178,356)`、`Skill1 (-100,462)`、`RuneOpen (-198,536)`；紧凑基准为 `Attack (-34,178)`、`Jump (-218,202)`、`Dodge (-170,332)`、`Skill1 (-96,438)`、`RuneOpen (-190,512)`；隐藏的 `Skill2` / `Skill3` / `Skill4` 不能占用视觉层。`ApplyHitConfirmSafeLayout` 要把 `HitConfirmText` 推到弧线左侧，不能压在攻击键或技能键上。Boss 专注模式下按钮通过 `BossFocusButtonOffset` 分组贴右下，攻击偏移基准为 `new Vector2(122f, -22f)`，攻击键视觉缩到 88%，技能和跳闪进一步收缩到约 `.80f/.76f`，攻击/跳闪/技能透明度约 `.42f/.28f/.26f`，命中确认必须额外左移并上移，且底板/力度条透明度降低，避免成为右手区第二层常驻 HUD。

## 左手移动摇杆

- `JoystickTouchZone` 必须覆盖左下大热区，不能要求玩家精准按住摇杆圆盘。
- `MoveDynamicJoystickBase` 会在按下时把底盘移动到拇指附近，松手后回到默认位置。
- `UpdateJoystickIntentLock` 会锁定横向、上推、下拉或斜向意图，减少斜推时误触跳跃或横划闪避。
- `JoystickIntentText` 只在拖动摇杆时显示“移动中 / 横划闪避 / 上推跳跃 / 下落 / 闪避冷却”等短反馈，用于确认当前触控意图，不作为常驻教程。

## 状态反馈

- 攻击、跳跃、闪避、技能和 `SLOT` 按钮在触屏点击后只能显示 0.075 秒青白低透明外圈反馈，不允许出现米黄/金色边圈或大面积背景板。
- `PulseTouchDownFeedback` 必须在 PointerDown 阶段触发，让玩家手指盖住按钮主体时仍能看到外圈响应；`ResetTouchDownEcho` 必须在回弹结束后复位外圈，外圈 alpha 约 `.11f`。
- 预输入还在等待消费时，对应按钮只保留短促缩放/回弹和冷却数字，不显示攻击、跳跃、闪避微文案。
- 技能按钮不常驻显示 MP 消耗，布局调试也不把成本压进按钮；冷却中显示中心数字和冷色暗遮罩，魔力不足时显示 `MP`、红暗遮罩和降透明标签；短技能缓冲存在时只显示“待发”短词和冷色按钮高亮。
- 技能按住预览必须跟随玩家位置和朝向：剑气/炎弹显示前方轨迹，冰环/雷击显示范围圈；预览只解释范围，不提前造成伤害或消耗资源。
- `SLOT` 键必须按上下文显示启封、补给或充能；未满但接近启封时按钮微文案只显示“临界”，具体命运能量百分比留给中心冷却数字和右上短状态。
- 命运能量达到 `RuneAnticipationThreshold` 后，`SLOT` 键显示“将启封”，`RuneStateText` 只显示“临界”，`RuneReadyGlow` 提前低强度呼吸；具体百分比只留给中心冷却数字和右上短状态，实际奖励仍必须等 100% 才召出。
- 可释放/可交互按钮必须有独立外圈呼吸高亮：正式可见技能只使用 `Skill1ReadyGlow`，`Skill2ReadyGlow` / `Skill3ReadyGlow` / `Skill4ReadyGlow` 必须随隐藏按钮关闭，`SLOT`/补给使用 `RuneReadyGlow`；内部保留的火球信号仍需使用 `#7EDCFF` 冷色口径，避免未来恢复时出现橙黄背景板；冷却、魔力不足、锁定或不可交互时高亮必须隐藏，不可用状态不能只靠小字判断。

## 自定义和保存

- 设置页提供紧凑/标准布局、按钮大小、右手位置、透明度和单键拖动微调。
- 单键拖动只保存偏移，不改变按钮语义；保存到 `PlayerPrefs` 后再次进入游戏应恢复。
- 清除本机数据会清除控制偏好和单键偏移，但不影响应用内无联网/无账号/无广告口径。

## 人工真机检查

- iPad 横屏下右手自然握持，拇指能触达攻击、跳跃、闪避和至少两个常用技能。
- 攻击键不会贴到系统手势边缘；任何右手位置档位和单键拖动都不能把按钮拖出安全区。
- 按钮透明度最低档仍能读出图标、文字、冷却和可用状态。
- 快速点按测试：攻击冷却中提前点下一刀应显示 `AttackBufferText`，空中提前按跳跃应显示 `JumpBufferText`，闪避冷却中提前点闪避应显示 `DodgeBufferText`，技能冷却快结束或普攻动作末端提前点技能应显示“待发”并在可用/动作空出后自动释放一次；窗口结束或动作触发后文字消失。
- 变量跳跃测试：轻点跳跃键必须立刻起跳并快速切短上升弧线，长按跳跃键必须达到完整高度；PointerExit、暂停、重开后不能残留 `touchJumpHeld` 导致自动连跳。
- 平台边缘测试：跳向平台左右边缘时，脚点差约 `PlatformLandingEdgeAssistX` 以内且方向指向平台内侧，应轻微吸入并只显示短小 `PlatformLandingEdgeAssistPin_NoYellowPlate`；从侧面或下方撞平台不能被吸上去，也不能出现空气墙或长横向脚下线。
- 平台下穿测试：站在高台上按住摇杆下方向再点跳跃，角色必须下穿当前平台并显示 `PlatformDropThroughFootLine_NoYellowPlate` / `PlatformDropThroughDownTick_NoYellowPlate`；松开下方向后普通跳跃仍是起跳，地面、Boss 锁场和房间门不能被下穿绕过。
- 技能预览测试：按住剑气/炎弹显示前方细轨迹和末端刻度，按住冰环/雷击显示低透明范围边界；松手、拖出按钮、打开 `SLOT`/商店/暂停或技能进入冷却后，`SkillAimPreview` 必须消失且不造成提前伤害。
- 按下反馈测试：快速点攻击、跳跃、闪避、技能和 `SLOT` 键时，按钮外圈应立即出现 0.10 秒青白回弹，最高 alpha `.10f`；没有真实命中时不触发 `HitConfirmText`，也不抢占重击/奖励震动节奏。
- 右手热区测试：开启“拖动微调”时必须显示 `ThumbReachSafeZone` 和 `ThumbReachSafeZoneText`；切换标准/紧凑、按钮大小、右手位置和单键偏移后，`ApplyHitConfirmSafeLayout` 仍要把 `HitConfirmText`、`HitConfirmTierText` 和 `HitConfirmImpactFill` 放在右手热区左侧，不与攻击键、跳闪、单技能或 `SLOT` 键重叠；进入 Boss 专注模式后，命中确认还要继续左上避让并降低暗底存在感。
- 默认握持测试：右手拇指不移动掌根时应能覆盖攻击、跳跃、闪避和单个可见技能；向内滑动一小段能触达 `SLOT`，不会再被隐藏技能占视觉空间。
- 摇杆意图测试：左下大热区按下后底盘移动到拇指附近；横向短划时 `JoystickIntentText` 显示“横划闪避”或“闪避冷却”，上推显示“上推跳跃”，松手后文本消失。
- 状态读法测试：攻击键只用三颗连击点和 `AttackRhythmWindowFill` 表示段数/破势窗口，正式 HUD 隐藏 `AttackStateText` 与 `AttackRhythmWindowText`；闪避只显示中心冷却数字，不显示 `DodgeStateText`；启封键显示 `RuneStateText` 的“临界 / 启封 / 补给”且可用时显示 `RuneReadyGlow`，技能键状态小字只显示“锁定 / 待发”；魔力不足时只用中心 `MP`、暗遮罩和标签降透明表达，不能额外叠“缺MP”；可释放时显示对应 `SkillReadyGlow`，冷却或魔力不足时隐藏。
- SLOT 临界测试：命运能量达到 `RuneAnticipationThreshold` 时，启封键显示“将启封”，右上 `NextSlotHintText` 显示“临界XX%”，按钮可有弱呼吸但不能提前打开启封面板。
- Boss 战、平台跳跃和 `SLOT` 宝箱附近，按钮不遮挡角色、主要敌人预警或符文启封面板。
