# 命运游侠 Unity 横版试玩原型

当前方向是 4399 页游时代横版动作闯关体验：左侧移动，右侧攻击/跳跃/技能，短关卡清怪推进。

## 当前内容

- 横版森林遗迹推进地图，iPad 横屏参考分辨率 `2732x2048`
- 启动时显式设置 `Application.targetFrameRate`、`vSyncCount = 0` 和匹配固定步长；默认流畅档 60 FPS，也可在设置页切到省电档 30 FPS，兼顾平板触控响应和发热控制
- 玩家光剑角色，使用 32 帧动作图集，支持 4 帧待机、6 帧跑步、跳跃/下落、闪避、三段攻击分帧、受击、胜利和 4 种技能释放帧
- 动画帧率：待机约 5 FPS，跑步约 12 FPS，攻击 18-20 FPS，技能释放约 16 FPS
- 起跳、落地、闪避、跑步脚步和普攻会生成地面尘/光痕；普攻起手有轻微前冲，攻击/闪避/技能起手不复制主角贴图做残影，只用 `SpawnHeroCleanActionStreak`、刀光、脚底刻度和身体压缩/伸展确认动作连续性；设置页可手动开启 `HeroActionBeatText`，在起跳、闪避、攻击起手、真实命中和技能释放时短暂显示动作帧号、FPS 与命中/释放时点，正式默认关闭，角色动作不再只是原地换帧
- 普攻三段有独立可读反馈：一斩/二连/破势短字、脚下光圈和第三段重击爆点，方便移动端小屏幕读出当前段数
- 命中反馈按轻击、破势重击、技能命中分层：停顿、击退、震屏、屏幕闪光、火花尺寸、伤害字颜色/上浮弹出动效和震动强度会随命中级别变化
- 右手战斗区上方有 `HitConfirmText` 命中确认徽章：轻击命中短促显示“命中”并隐藏来源副标题，三连重击显示“破势命中”，技能命中显示“术式命中”，Boss 会显示专用命中词；`HitConfirmImpactFill` 会同步显示命中力度，`HitConfirmTierText` 只用于重击、技能和 Boss，空挥不显示，避免误导
- 角色换帧带运行时可见性兜底：如果某个动作格被抠成空帧，会自动回退到最近有效角色帧；如果图集或切片完全失效，会显示淡蓝占位体，并恢复 SpriteRenderer 的可见性、层级和缩放，避免角色直接消失
- 三连击带约 0.22 秒攻击输入缓存，跳跃/闪避也有短预输入窗口；正式 HUD 隐藏攻击、跳跃和闪避预输入微文案，改由按钮短促回弹、连击点、冷却数字和角色动作确认输入，打击判定仍延迟到挥刀帧，技能效果延迟到释放帧
- 右下攻击按钮支持点按和长按连续攻击，键盘 `J/Space` 也支持按住连砍，降低平板触控疲劳
- 左下摇杆支持短距离横向 swipe 闪避，保留独立闪避键，兼容 Dead Cells 类移动端操作习惯
- 玩家脚下不再使用蓝色光环或黑色脚影，角色必须由清晰动作帧、实体草皮/土层平台和低透明冷色短线读清，避免森林背景里出现贴图块。
- 森林背景上方只在战斗中淡入局部 `ForegroundReadabilitySoftEllipse_NoPlate`，它只做极淡前景分离，不覆盖全关卡、不形成脚底黑影；主角的 `HeroContrastSilhouette` 和 `HeroReadabilityAura_DisabledNoGroundBlob` 保持透明禁用，空中才用 `HeroLandingPredictor` 判断落点；敌人/Boss 的 `EnemyContactAo_DisabledNoGroundBlob` 也保持透明禁用，移动端远景下主要靠实体平台、敌人轮廓、类型标记和受击短线读清角色、敌人、平台和预期落点。
- 蓝色精灵宠物会跟随玩家，并周期性释放协战法球
- 相机通过 `ComputeCameraLookAhead` 跟随角色向右推进：向右移动时提供更多前方视野，回头或后撤时平滑回收，接近房间右边界时降低前视；战斗中 `TryGetCombatCameraThreat` 会按 Boss、精英、弓手、近身敌人优先级轻微把镜头拉向主角和威胁之间，让敌人前摇、平台落点和右手按钮区同时可读；地图内约有 8-10 个可落脚生成石台、7 个分段符文屏障和出口门，不再是一整片平地
- 关卡被切成 8 个推进房间，清掉当前房间敌人后屏障解除，才能继续向右
- 分段符文屏障解除时会播放开门演出：房门门扇滑开、短扫光、门口路线、玩家脚底向右短路线、短音效、震动和轻微屏幕反馈，不再只是直接消失，也不再用世界大字提示推进
- 每个推进房间都有世界门牌和 HUD 房间标题，入口、遭遇、伏击、符文商店、混战、毒素精英、终战准备、Boss/终段战斗的节点更清楚
- 每个房间门牌带左右门扇、锁光、状态文字和特殊房间徽章，会显示封锁/战斗中/可补给/待启封/已开启；商店、精英、Boss 房使用不同颜色和 `RoomGateSpecialBadge`/`RoomGateSpecialBand` 标出“补给/精英/BOSS”，玩家远景下也能提前读出前方房型风险
- 战斗房会随机获得轻量词缀：疾行、强袭、命运涌流、罐藏、精准试炼或稳态；词缀会影响敌人速度/伤害/血量、清房金币/命运能量、限时清房、目标符文罐或极限闪避奖励，进房提示、门牌和房名都会显示当前变化
- 进入新房间会触发一次性入场反馈：世界短字、地面光圈和 HUD 标题高亮；商店房、精英房、Boss 房分别有补给提示、毒圈警告和锁门/震屏提示
- 每个房间清空后都会发一次轻量房间奖励：金币、生命恢复或命运能量，形成“战斗-奖励-推进”的标准循环，但普通房不弹大面板打断移动
- 只有精英房和 Boss 前关键节点清空后会弹出 3 选 1 房间奖励面板，从生命、金币、命运能量、攻击、移速、护盾随机池中抽取，并带普通/稀有/史诗稀有度倍率
- 奖励卡会按稀有度变色，并显示奖励类型图标、`RoomRewardTypeBadge` 类型标签（输出/生存/机动/经济/SLOT）、稀有度顶部条、数值高亮、构筑提示和 `RoomRewardRecommend` 推荐标记；推荐理由会结合当前血量、命运能量、已有符文、商店/Boss 是否临近，说明为什么这张卡适合当前局面；支持触屏点选或键盘 `1/2/3` 快速选择
- 首局有非强制 7 步短任务教学，使用 `1/7 移动`、`2/7 三连攻击`、`3/7 清敌开路` 等短文案覆盖移动、攻击、清房、靠近/启封 `SLOT`、继续推进和商店补给；`FirstRunNudgeText` 会在 SLOT 宝箱、平台小宝箱、商店、首个 SLOT 未启动等卡点给短提示；完成后保存到 `PlayerPrefs`
- 首次从标题页开始时会显示可跳过的“试炼简报”，用 4 个步骤说明战斗清房、房间奖励、`SLOT` 共鸣和商店补给，并提示辅助模式、自动朝向/自动攻击和 `SLOT` 无真钱付费/现金价值；选择开始或不再提示后保存到 `PlayerPrefs`，设置页的 `SettingsOnboardingReplay` 可随时重新查看同一简报，回放模式只返回设置，不会误启动新对局
- 首局体验规格见 `Assets/DestinyRanger/Docs/FIRST_RUN_SPEC.md`；失败结算会通过 `BuildFirstRunAssistAdvice` 给出“开辅助/长按攻击/先启封 SLOT/保留闪避”等操作建议
- 世界内第一屏操作投影使用触屏语义：拖动移动、攻击、跳跃、单个核心技能、`SLOT` 和交互，不在主画面显示键盘字母或多技能长串
- 三关长地图战斗：每关都有多段蘑菇怪、史莱姆、弓箭怪随机遭遇组合，部分推进段出现毒素光环精英，最终关含守护者 Boss
- 敌人使用 24 帧动作图集，晶石近战怪、弓手、史莱姆、石像 Boss 均有待机/移动/攻击/受击或狂暴帧，自动接近/射击/近战攻击，带血条、受击闪白、击退和大号伤害数字
- 敌人攻击前有分层意图提示：普通近战/弓手只用脚底细线、瞄准线、微针倒计时和地面预警，不在头顶刷“警戒/瞄/近战”世界字；精英和 Boss 才允许短动作词。Boss 出招前保留危险范围、砸地/激光/雷击倒计时和顶部应对提示，避免玩家只盯右手按钮时漏看关键前摇
- 高危前摇会额外触发 `ThreatAlertText` 居中 HUD 预告牌：Boss 会显示“巨拳砸地/旋转激光/雷阵召唤”和应对建议，精英才显示短高危提示；普通弓手和近战怪主要靠世界细线、微针倒计时和屏幕方向提示处理，避免小怪群把画面刷满字
- 危险范围不只靠红/绿颜色：`AddDangerPattern` 会给矩形砸地/激光/近战预警加斜纹或短线，给雷击/瞄准预警加十字准星，给毒素光环加波纹线；普通怪内部字形隐藏，Boss/精英才保留必要短字和倒计时，降低色弱和小屏幕误读
- 敌人头顶会显示状态标签：冰环命中会短暂冻结敌人并显示“冻结”，毒素精英常驻“毒素光环”，Boss 半血后常驻“狂暴”；冻结结束后会恢复原本精英/词缀染色
- 命中会累积连击，受击或超时后清空；连击牌使用“12 连击 · 狂热”这类短中文格式，6/12/20 连击只弹“连斩 / 狂热 / 极限连击”短标签，命中同时带短打击停顿、上浮伤害字和技能特效
- 命中反馈包含短暂停顿、镜头抖动、屏幕暖色闪白、命中火花/重击环、敌人受击闪白、血条脉冲、`SpawnEnemyRecoilHitLines` 程序短线和沿受击方向拉出的 `EnemyHitDirectionTrail`；受击硬直用 `EnemyHitStunLockLine`、`EnemyHitStunRootPin_CleanThin_NoYellowPlate`、`EnemyHitStunBraceFootLine_CleanThin_NoYellowPlate` 和重击/技能的 `EnemyHitStunGroundLine_NoYellowPlate` 表达，禁止复制敌人贴图做受击残影或铺地软环
- 技能键按住会显示 `SkillAimPreview`：剑气/炎弹只显示前方细轨迹、末端刻度和脚底锚点，冰环/雷击只显示低透明范围边界；技能释放会在伤害生效前显示短促起手文字、蓄能核心、地面蓄能环和释放方向带；真正释放帧再触发术式核心/地面脉冲、角色压缩伸展、按钮脉冲、轻震屏和技能命中反馈；受击有红色冲击环，击杀有“击破”爆点/短字，形成按下、预览、释放、命中、击杀五段反馈闭环
- Boss 拥有砸地冲击、横向激光、半血雷阵三类招式
- Boss 战会显示顶部专用血条、短阶段 token 和无文字压力条；最终 Boss 存活时 `BossArenaLockdown` 只保留低透明左右锁场细线与地面封锁线，目标栏写“锁场中”，击破后隐藏并进入最终 `SLOT` 收束；阶段从 `P1 · 50%碎甲` 切到 `P2 · 雷阵`，招式和反击窗口用短时提示，不再常驻攻略长句，移动端远景下优先读清 Boss 生命、边界和危险升级
- Boss 击破后会播放专属爆点、震屏、文字提示和最终 `SLOT` 预告；最终启封结束后再播放短胜利演出并进入结算
- 关键清房、商店和 Boss 收官时会出现 `SLOT` 命运宝箱，靠近后点右侧交互按钮启封三列命运符文石，启封面板会显示中心开箱、三列滚动、二星/三星结果高亮、奖励说明、当前流派标签、下一步打法建议、本次构筑变化和无共鸣生命保底
- 击败敌人会积累 `命运能量`，满 100% 时进入临界状态；只有精英/Boss 前关键节点且本关自动奖励 `SLOT` 配额未用完时才召出奖励版 `SLOT`，商店和收官宝箱仍保留。右上 `NextSlotHintText` 只显示“启封 / 商店100 / 差X金 / 临界XX% / 差X%”等短状态，启封键在临界时用“临界”和弱呼吸提示，让战斗中持续形成“快要再抽一次”的期待但不堆长句
- 路线中放置平台小宝箱，攻击或技能打开后掉金币/命运能量；战斗中不直接弹中心 `SLOT` 面板，只有清房后的目标宝箱才可能唤醒奖励版 `SLOT`
- 敌人和平台小宝箱会掉落金币、生命光团和命运碎片，靠近后自动吸附到角色身上，强化战斗收益反馈
- 毒素光环精英会周期性释放绿色毒圈，逼迫玩家走位；击败精英会掉更多金币/经验，并在没有圣殿时召出奖励版 `SLOT`
- 每关第 4 房有符文商人，商店房不会自动跳过；玩家清掉当前区域后靠近并点右侧交互按钮打开补给，可用本局金币回血或召出一次奖励版 `SLOT`；商店商品卡使用 `ShopItemTray` 大卡布局，显示图标、价格、可买/差额/售罄状态、短购买建议和 `ShopHealRecommendBadge` / `ShopRuneRecommendBadge` 推荐角标，`ShopDecisionText` 会根据血量、金币、符文数量和售罄状态显示“低血：先回血 / 成型：买 SLOT / 金币不足：清房开箱”等短决策，回血和奖励 `SLOT` 每次商店各限购一次
- `SLOT` 是战斗内符文奖励表现，不含真钱付费或现金价值；局内 `RuneOddsText` 会在临界、待启封、商店可启封或启封面板打开时显示约 25% 三星、57% 二星、18% 逆运祝福和无付费现金价值说明，符文面板/图鉴也会显示同一口径，避免随机奖励信息不透明
- 内容表现以幻想/卡通动作战斗为准：敌人受击闪白、击退、消散，无血腥、肢解、真实武器教学、下注、赔付、现金兑换或真钱博彩
- 符文共鸣会即时改变打法：剑气变宽/三段剑气飞出、闪系提高机动并只保留极淡脚底电痕、冰环范围变大、雷系触发连锁电弧；无共鸣触发回血保底。每次启封会生成“光剑压制 / 闪避游击 / 冰环控场 / 雷链清场 / 坚盾续航”等流派提示
- 左上 HUD 有剑、闪、冰、雷、盾五个流派徽章，获得符文后亮起并显示等级，战斗中能快速扫读当前构筑
- 键盘测试：`A/D` 或方向键移动，`W/K` 跳跃，`Shift` 闪避，`J/Space` 攻击，`U` 释放当前核心技能，`R` 启封符文，`F` 打开补给商店；`I/O/L` 作为未来技能扩展预留，不在 v0.1 战斗输入里触发；触屏摇杆横向短划可触发闪避
- 蓝牙/MFi 手柄兼容：默认映射为左摇杆移动，`A` 跳跃，`B` 闪避，`X` 攻击，`Y` 释放当前可见核心技能，`View/Menu` 类按钮用于启封、交互和暂停；`L1/R1/L2` 只作为内部扩展技能预留，不在 v0.1 战斗 HUD 常驻显示；实际按钮受系统重映射影响，所以战斗 UI 不硬显示手柄 glyph，只在设置页显示“控制器已连接/触屏优先”，并同步驱动屏幕按钮按下态
- 画面内正式操作 UI：左下触控拖动摇杆，右下为手游式右拇指战斗区：大攻击键贴右下，跳跃和闪避在攻击键左上侧近处，v0.1 只显示 `Skill1` 一个核心技能，`SLOT` 启封/补给键位于单技能上方偏内侧；隐藏的炎弹/冰环/雷击不占用视觉层、射线或按钮循环。主屏幕按钮显示攻击/跳跃/闪避/剑气/SLOT/暂停/设置等移动端语义标签，键盘映射保留在暂停说明里
- 设置页提供右侧战斗按钮“拖动微调”模式：开启后设置面板收起、战斗冻结，玩家可单独拖动攻击、跳跃、闪避、技能和启封按钮，位置偏移保存到 `PlayerPrefs`
- 右侧战斗按钮支持清爽/标准/清晰 3 档透明度，玩家可以在读招视野和按钮可见性之间取舍
- HUD、摇杆和右侧战斗按钮挂在 `SafeAreaRoot` 下，运行时根据 `Screen.safeArea` 自动避开圆角、刘海和系统手势区域；全屏弹窗保持覆盖全屏，闪白覆盖区域不变但强度/持续时间受特效档位控制
- 技能、闪避、商店和 `SLOT` 按钮带冷却/锁定/资源不足提示；正式 HUD 隐藏 `AttackStateText` 和 `DodgeStateText`，`RuneStateText`、`NextSlotHintText`、`RuneOddsText` 和可见的 `Skill1StateText` 只用 `临界 / 启封 / 补给 / 锁定 / 待发` 这类短 token 说明状态；技能在魔力不足时会红暗遮罩并只在中心显示冷色 `MP`，不再额外叠“缺MP”，可见技能可释放时只使用 `Skill1ReadyGlow` 外圈呼吸，隐藏技能的状态字和 ReadyGlow 必须随按钮关闭；`SLOT` 临界、可启封或可补给时有 `RuneReadyGlow`，商店金币不足时显示差额，按钮大小档位变化后文本仍保持居中
- 攻击、跳跃、闪避、技能和交互按钮在触屏点击后都会显示 0.075 秒青白低透明外圈；真实命中后会通过 `PulseHitConfirmButton` 触发二次按钮回弹，普攻/重击回弹攻击键，当前可见核心技能命中回弹 `Skill1`，空挥不触发；攻击键在出刀和冷却窗口使用冷白/青白确认，不再偏暖色
- 所有右侧战斗按钮绑定 `AttachTouchDownFeedback`，PointerDown 阶段先触发外圈青白回弹，`ApplyTouchDownEcho` 最高 alpha `.10f`，随后才由攻击/技能/启封逻辑决定是否进入预输入、冷却、命中确认或 SLOT 面板；按下反馈不调用震动，避免抢占破势、受击和奖励震动节奏
- 顶部有正式血量、魔力、经验条和金币/符文状态，右上有暂停与设置按钮；关卡名、目标、事件提示分层显示并带暗底板，避免亮背景下文字互相压住
- 底部中间有 `TargetCompassText` 方向罗盘：当前房间仍有敌人时优先提示 Boss/精英/弓手等威胁方向和距离；非战斗时提示待启封 `SLOT` 宝箱或平台小宝箱，避免移动端远景和右手操作遮挡下找不到下一个目标
- 右手按钮区支持 `ThumbReachSafeZone` 拇指热区微调提示；真实命中 HUD 通过 `ApplyHitConfirmSafeLayout` 固定避开攻击键、单技能和 `SLOT` 键，显示在右手热区左侧，避免玩家手指盖住“命中/破势/术式”确认
- 顶部关卡条同步显示本局用时和击败数，帮助玩家形成速通和清怪目标
- 右上角局内进度条显示房间进度、相对最佳时间的领先/落后和击杀纪录差距，提升反复挑战目标感
- 顶部中央有路线小地图，显示 8 个推进房间、当前房、商店房、精英房、Boss 房和 `SLOT` 待启封状态；当前房间清房后有平台小宝箱时，小地图内 `PlatformRouteHintText` 才显示“箱 右3m · 跳上打箱 / 攻击开箱”这类短提示，战斗中不刷宝箱文字
- 小地图文字会同步当前房间类型和状态，屏障解除后触发下一房间入场提示；奖励选择时目标栏会提示先完成三选一，推进感更接近正式移动端 Roguelite 关卡
- 教学提示栏会在完成后自动隐藏，避免重复局遮挡战斗视野
- 低血量和受击时有红色屏幕压迫反馈；受击还会在屏幕左/右/上/下边缘显示来源方向提示，并用左下 `HurtRecoveryText` 短暂提示“受击恢复：短暂无敌”，根据 Boss、弹幕、商店补给或盾/冰构筑给下一步动作，帮助移动端玩家快速判断危险状态和回避方向
- 程序化生成短音效和轻量占位循环 BGM，不依赖外部音频资产：普通出刀、轻命中、三连破势、技能释放、Boss 高危预警、拾取、房门开启、SLOT 转动/奖励、商店补给都有分层反馈；正式音乐资产接入后可直接复用分轨设置
- 标题页支持开始试炼、首次试炼简报、设置入口和“关于/隐私”入口
- 标题页和设置页都能打开“关于/隐私”，说明版本、离线运行、PlayerPrefs 本机保存项、无账号/广告/分析 SDK/联网排行、SLOT 无真钱付费或现金价值，以及图片资产来源；开发版面板会显示 `AboutReleaseBlockerText`，提醒 TODO 隐私/支持链接会阻断正式提交；面板提供二次确认的“清除本机数据”，可删除设置、教程状态和成绩记录
- 上架元数据规格见 `Assets/DestinyRanger/Docs/RELEASE_METADATA_SPEC.md`，App Privacy 标签规格见 `Assets/DestinyRanger/Docs/APP_PRIVACY_LABEL_SPEC.md`；正式提交前必须把关于/隐私里的 `TODO_PRIVACY_POLICY_URL` 和 `TODO_SUPPORT_URL` 替换为真实公开链接
- 本地存档规格见 `Assets/DestinyRanger/Docs/LOCAL_SAVE_SPEC.md`；所有 `PlayerPrefs` 固定键、按钮偏移生成键、本地成就生成键、schema 版本和清除行为都必须同步维护
- 标题页显示最佳通关时间、最高击败记录和当前试炼热度，进入游戏前就能看到再挑战目标；`TitleModeGuideText` 会根据辅助模式、热度、今日试炼和本地成就状态给一条推荐说明，例如先首通、练当天固定种子、追求 5 次启封或 H2+ 高热通关
- 标题页显示本地成就进度，并提供“成就”入口查看 14 个离线成就的解锁状态；成就页顶部会根据当前解锁情况显示“推荐追踪”目标和 `BuildRouteArchiveSummary` 路线档案摘要，每个未解锁项通过紧凑单行格式、`AchievementShortDescription` 和 `AchievementProgressText` 显示当前进度，例如房间推进、最佳时间、启封次数、极限闪避、连击、房间奖励、热度目标、树冠/断桥/高脊路线熟练度和 `RouteArchivist` 的 0/3 特殊路线进度，帮助玩家决定下一局追求通关、5 次启封、极限闪避、速通、连击、高热通关或路线收集；成就只保存在设备本地，不接账号、联网排行或 Game Center，后续上架需要时可再映射到平台成就
- 标题页提供“今日试炼”：使用本地日期生成固定离线种子，普通试炼仍随机；今日试炼不接联网排行、不发每日奖励，只作为玩家当天复玩和自我挑战入口；标题页显示今日种子、今日最佳通关时间和今日最高击败，全部只保存在本机 `PlayerPrefs`
- 标题页提供“试炼热度”H0-H3 本地自选挑战：热度会提高敌人生命、伤害和移动速度，HUD 与结算页会显示当前热度，H2 或以上通关可解锁本地“高热通关”成就；热度只作为离线难度选择，不提供排行、奖励发放或付费关联
- 标题页和暂停页都提供“符文图鉴”，可离线查看剑/闪/冰/雷/盾/财和逆运祝福的二星、三星、战术用途、约 25% 三星/57% 二星/18% 逆运祝福概率及 SLOT 合规说明
- 标题页会冻结战斗时间，开始试炼后再生成正式局内状态，避免多个敌人在标题页后台推进
- 应用切后台或失焦时会保存本机设置并进入暂停面板，显示恢复原因，清空长按攻击、摇杆、跳跃缓冲、闪避缓冲和短离地宽容，返回后计时不推进
- 设置页支持辅助模式，开启后玩家受到伤害降低 35%，闪避冷却略短，极限闪避和无敌窗口更宽，适合先熟悉 Boss/精英前摇
- 设置页支持自动朝向，开启后攻击和技能会优先面向最近敌人，并在目标脚下/头顶显示低透明短准星 `AutoAimTargetFootReticle_NoText` / `AutoAimTargetNeedleLine_NoText`，不再叠加“锁定/技能目标”文字，降低移动端右手攻击时的瞄准压力且不遮挡战斗
- 设置页支持自动攻击，开启后只在敌人或平台小宝箱进入近身范围时自动出刀，适合触屏走位优先的玩家；该选项默认关闭，可随时切换
- 设置页支持音效开关、震动、总音量/音效/音乐三条滑杆、总音量快捷档位、标准/舒适低闪特效档、性能档位、按键布局、按钮大小、右手位置档位、右侧按钮透明度、右侧按钮单键拖动微调和重新查看试炼简报；顶部先用短徽章显示标准/低闪、自动/手动朝向、右手按钮状态、前景可读性、控制器连接状态、离线单机和无广告/无分析，下面再显示当前战斗辅助、自动朝向、自动攻击、反馈、触控、性能、安全区和控制器状态，玩家切换后能立即确认实际档位
- 舒适低闪档会限制屏幕闪白峰值与持续时间，并降低震屏和受击红边透明度，保留命中/危险可读性但减少强刺激画面
- 手游右手操作区采用拇指扇形布局：攻击键最大且更靠右，跳跃/闪避在攻击键左侧近处，v0.1 只显示 `Skill1` 一个核心技能，`SLOT` 启封键位于单技能上方偏内侧；攻击键内置三段连斩进度点、“二连 / 三段 / 破势”和 `AttackRhythmWindowFill` 连段节奏条，让玩家知道第三段重击窗口和剩余连段时间；隐藏技能不参与按钮循环、射线、高亮或透明度计算；技能按钮默认隐藏 MP 消耗，只保留图标、冷却数字、中心 `MP` 和锁定状态；可释放时按钮和独立 `ReadyGlow` 外圈低透明呼吸高亮，Boss 专注模式下按钮进一步贴右下，攻击键视觉缩到约 `.88f`，跳闪约 `.32f` 可见度、可释放技能约 `.26f` 可见度，冷却/缺 MP 技能继续降透明；按住时只显示 `SkillAimPreviewGuideLine_NoPlate` 和低透明边界预览实际范围，不生成大块预览板；`SLOT` 启封键中心显示命运能量百分比，微文案只显示“临界 / 启封 / 补给”，在临界、可启封或可补给时显示 `RuneReadyGlow`；标准档也默认略贴右，并保留“靠内/贴右”和单键拖动微调
- 右侧战斗按钮运行时有统一热区和安全边距保护，触控布局规格见 `Assets/DestinyRanger/Docs/TOUCH_CONTROL_SPEC.md`；默认、紧凑、贴右和单键拖动偏移都会经过 `ClampCombatControlPosition`，避免按钮进入系统手势边缘
- 左手移动区采用手游动态摇杆：左下有较大的隐形触控热区，按下时摇杆底盘移动到手指附近并提高透明度，摇杆头按方向缩放/偏移，松手后平滑回到底部默认位置；摇杆会锁定横向/纵向意图，并用 `JoystickIntentText` 短暂显示移动中、横划闪避、上推跳跃、下落或闪避冷却，降低斜向走位误触跳跃或横滑闪避的概率
- 触控战斗支持攻击缓冲、跳跃缓冲、短离地宽容和闪避缓冲；玩家在落地前、攻击冷却中或闪避冷却快结束时提前按键，系统会在短窗口内自动执行，正式 HUD 不显示缓存/预闪等微文案，只用按钮短回弹、连击点和角色动作确认输入，减少“按了没反应”的移动端手感问题又不堆字；跳跃键 PointerDown 立即起跳，短按触发 `CutJumpHeight` 形成低跳，长按达到完整跳高；左摇杆上方向采用单次触发，不会按住后自动连跳
- HUD 右侧提供常驻构筑短条，显示当前主流派、总构筑等级和最近一次符文战术提示，配合 5 个符文徽章让玩家随时知道本局打法变化
- 主角使用 8×4 的 32 帧动作表：待机 5 FPS、跑动 12 FPS、跳跃 14 FPS、轻击 18 FPS、第三段重击 20 FPS、技能 16 FPS；跳跃、短按截跳、落地、闪避、轻击和重击叠加短暂压缩/拉伸动作层，轻击不复制主角贴图、不刷高亮 Echo，第三段只保留短寿命硬边刀线和脚底锚点；`HeroActionBeatText` 仅在调试/设置开启时显示动作帧号，主角渲染层级固定高于伙伴，动作播放结束会回到稳定待机/跑跳帧，避免战斗中模型被宠物或问题帧遮住
- 闪避窗口内擦过敌方攻击会触发“极限闪避”，吞掉本次伤害，播放蓝白闪避确认、短停顿、轻震屏、震动，并奖励少量命运能量；成功时还会擦除身边敌方弹体，避免刚判定成功又被同一波弹幕打中，把技术走位和奖励 `SLOT` 循环连起来
- 低血时除红边外会显示非阻塞战术提示：可补给时提示去商店回血，商店前提示稳住距离推进，已有盾/冰时提示利用容错等闪避冷却
- 命中反馈按轻击/重击/技能分层：近战出刀先给前摇刀光，真实命中时再触发命中帧冲击波、敌人受击回弹、受击方向短线和短震，空挥只保留刀光；第三段重击触发额外硬边刀线、短停顿和硬直确认，技能从起手到释放帧再到命中点都有独立术式核心/方向带/圆环，配合停顿、震屏、闪白、伤害字、连击热度条和按钮回弹提升打击确认，但不靠多张主角贴图制造残影
- 命中确认 HUD 只跟随真实命中触发，和世界伤害字/连击条互补；玩家右手按攻击或技能后不用看画面中心小字，也能在拇指附近通过 `HitConfirmText`、力度条和按钮二次回弹确认是否命中、命中来源和命中强度
- 打击、音效、震动和低闪反馈规格见 `Assets/DestinyRanger/Docs/FEEDBACK_SPEC.md`；运行时使用 `HapticTier` 分层震动和 `hapticCooldownTimer` 节流，避免普通连击在真机上过密震动
- 击杀时普通怪只显示短击破线、小碎片和掉落物，不再在死亡点弹金币/命运长句；精英额外标注 `SLOT`，Boss 标注最终命运，收益确认主要由掉落吸附、顶部资源栏和精英/Boss 世界短字承担
- 金币、生命和命运能量吸附到角色身上时会显示短收取文字，金币为黄色、生命为红色、命运为蓝色，避免玩家只看到光效却不知道实际获得了什么
- 符文罐作为 `SLOT` 入口有独立受击反馈：普攻、重击和技能弹命中只产生冷色小裂纹线、火花、短停顿和轻震屏，不再弹“符文裂纹/破碎符文”世界字；战斗中破碎只给金币/命运能量并继续战斗，清房后的目标宝箱才可能唤醒奖励 `SLOT`
- `SLOT` 启封面板会显示独立共鸣等级：三星标记“核心流派成型”，二星标记“战斗能力强化”，未匹配标记“逆运祝福/保底回血”；`RuneBuildDeltaText` 会显示如 `剑0->2`、`总构筑 Lv1->3`、金币补给或回血保底，让玩家抽完立刻理解本次结果强度和打法变化
- 敌人可读性分为四层：`TargetCompassText` 只提示 Boss、精英、屏外弓手/敌人等高优先级威胁方向；进入攻击距离后普通怪用脚底细线和前方预警表达蓄力，高危前摇时用更细的倒计时针、瞄准线和屏幕方向提示，真正出招时再给红区、瞄准线和 Boss/精英倒计时，方便移动端玩家提前闪避且不让小怪文字污染画面
- `ThreatAlertBackplate` 使用暗底和短淡出，舒适低闪档降低透明度但保留文字；它只在高危窗口短暂出现，不替代地面红区、倒计时和方向提示
- Boss HUD 除专用血条外只常驻短阶段 token、50% 阶段标记和无文字压力条；巨拳砸地、旋转激光、雷阵召唤只在前摇时短时出现，50% 血量进入碎甲狂暴时会播放阶段演出，HUD 从 `P1 · 50%碎甲` 切到 `P2 · 雷阵`，反击窗口只短时提示玩家贴身输出
- 地图推进不是纯平地：每关沿途有可踩平台，部分平台小宝箱会生成在高低平台；远处和战斗中只显示宝箱轮廓、锁孔、接触阴影和高台短线，清房后靠近约 `2.6f` 内才显示“奖励箱/小宝箱”。`PlatformRewardGuide`/`ObjectiveRuneGuide` 会用短线提示高台落点，目标栏、小地图和 `PlatformRouteHintText` 只在清房后接近时显示左右距离和跳上打箱/攻击开箱操作；角色空中会用 `HeroLandingPredictor` 在将要落上的地面或平台投出低透明落脚环，鼓励玩家跳上平台收集 SLOT 触点
- 房间不再只有清怪目标：`疾行`房会显示限时清房倒计时，成功后追加金币和命运能量；`罐藏`房会生成目标符文罐，清怪后必须打破目标罐才会开门；`精准试炼`房在目标栏提示极限闪避挑战，完成后可提高奖励收益
- 设置页提供“恢复默认”，只重置控制、反馈、性能和辅助偏好，不清空教程完成、最佳通关时间或击杀纪录
- 辅助模式、自动朝向、自动攻击、音效开关、震动、总音量/音效/音乐分轨音量、特效强度、性能档位、紧凑布局、按钮大小、右手位置、右侧按钮透明度、右侧按钮单键自定义偏移、试炼热度、试炼简报已读状态和今日试炼本机最佳都会保存到 `PlayerPrefs`
- `PlayerPrefs` 当前使用 `DestinyRanger.SaveSchemaVersion = 1`，启动加载时通过 `EnsureLocalSaveSchema` 进入迁移检查；清除本机数据会删除固定键、单按钮偏移键和 `DestinyRanger.Achievement.<id>` 本地成就键
- 暂停面板支持继续、设置和重新开始；局内打开设置会冻结战斗时间，关闭后按标题/暂停/商店状态恢复输入锁定
- 移动端生命周期已处理：应用切后台、锁屏或失焦时会保存本机设置、清掉触控输入、冻结战斗并在对局中显示暂停面板；返回后需要点“继续战斗”，避免后台继续计时或长按攻击残留
- 胜负结算会先显示“核心：”和“表现：”两行摘要，覆盖本局热度、用时、击败、金币、最高连击、受伤次数、极限闪避、SLOT 启封次数和构筑等级；最近符文和房间奖励各最多显示 3 条，`VictoryStats` 使用 `FitHudText(victoryStatsText, 18, 22)`、紧凑左对齐、`CompactRunLedger` 和 `CompactLedgerEntry` 控制文本长度，避免 iPad 横屏溢出；失败瞬间先播放 `SpawnPlayerDefeatCeremony`，结算页顶部 `VictoryCauseText` 会显示短失败原因或本局亮点，失败时额外显示倒下房间和房间进度百分比；胜负都会根据本局数据给出“下局挑战”“问题诊断”“路线复盘”“构筑复盘”和“推荐构筑”，其中构筑复盘会显示流派、评分、主轴符文和短板，例如少受伤、15 分速通、5 次启封、3 次极限闪避、剑/雷清场、盾/冰容错或闪避游击，最佳记录和本地成就保存到 `PlayerPrefs`
- 今日试炼结算会额外显示是否刷新今日最佳时间或今日最高击败，并显示当前今日最佳 `时间/K击败`；该记录只存在本机，不上传、不排行、不发奖励
- 结算页会自动授予本局挑战徽章和本地成就，例如试炼完成、无伤通关、15 分速通、命运常客、精准游侠、连击压制、清房规划、适应试炼、坚盾续航、高热通关、树冠熟手、断桥熟手、高脊熟手和路线档案，让短局重开有更明确的成就目标
- 结算面板支持再次挑战和返回标题，通关前会先播放“最终命运已启封”的胜利收束，避免 Boss 后直接硬切面板；`VictoryAssistNoteText` 会说明辅助模式只改变动作容错、本地成就仍按同一条件记录，以及再次挑战会沿用当前设置或今日固定种子
- 重新开始或返回标题会重置最大生命、最大魔力、金币、符文等级、局内倍率和奖励履历，避免上一局构筑污染下一局

## 场景

主场景：

```text
Assets/Scenes/DestinyRangerPrototype.unity
```

在 Unity 中打开工程后，选择标题包含 `命运游侠_Unity工程` 的窗口，打开该场景并点击 Play。

## 生成与验证

重新生成场景：

```bash
Unity -batchmode -quit -projectPath "<project>" -executeMethod DestinyRanger.EditorTools.DestinyRangerSceneBuilder.CreatePrototypeSceneBatch
```

生成目标工程自己的预览图并验证关键对象：

```bash
Unity -batchmode -quit -projectPath "<project>" -executeMethod DestinyRanger.EditorTools.DestinyRangerSceneBuilder.RenderPrototypePreviewBatch
```

预览图输出：

```text
/private/tmp/destiny-ranger-preview.png
```

该验证会检查背景、玩家、主角落脚提示、世界层摇杆、攻击按钮、技能按钮、三列 `SLOT` 符文盘、HUD Canvas、关于/隐私入口、符文概率说明、商店无付费声明、敌人意图圈和平台奖励标签是否创建成功，避免多个 Unity 项目同时运行时截错窗口。

验证上架构建设置：

```bash
Unity -batchmode -quit -projectPath "<project>" -executeMethod DestinyRanger.EditorTools.DestinyRangerSceneBuilder.ValidateReleaseSettingsBatch
Unity -batchmode -quit -projectPath "<project>" -executeMethod DestinyRanger.EditorTools.DestinyRangerSceneBuilder.ValidateNoThirdPartySdkBatch
```

该验证会检查产品名、版本号、iOS Bundle Identifier、横屏方向、禁用竖屏旋转、全屏和隐藏状态栏等构建口径。

验证上架合规口径：

```bash
Unity -batchmode -quit -projectPath "<project>" -executeMethod DestinyRanger.EditorTools.DestinyRangerSceneBuilder.ValidateAppStoreReadinessBatch
```

该验证会检查上架文档和运行时文本里的离线隐私、无广告/分析/排行、SLOT 无真钱/现金价值、启封概率、非博彩分类、清除本机数据、自动攻击辅助、舒适低闪和生命周期暂停声明；也会检查 `BUILD_AND_DEVICE_READINESS.md` 中的安全区、设置徽章、Boss HUD、攻击三段提示、无 SDK/联网扫描和完整批处理命令。

最终提交 App Store 前，在真实隐私政策 URL 和支持 URL 已经替换进游戏内“关于/隐私”和 `RELEASE_METADATA_SPEC.md` 后，再运行阻断项验证：

```bash
Unity -batchmode -quit -projectPath "<project>" -executeMethod DestinyRanger.EditorTools.DestinyRangerSceneBuilder.ValidateFinalSubmissionBlockingBatch
```

该验证会复用上架口径、无 SDK/联网、本地存档和动作图集检查，并额外拦截 `TODO_PRIVACY_POLICY_URL`、`TODO_SUPPORT_URL`、运行时 `PLACEHOLDER` 残留和动作帧调试 HUD 默认开启；开发阶段保留 TODO 链接时它应当失败。

验证主角/敌人动作图集：

```bash
Unity -batchmode -quit -projectPath "<project>" -executeMethod DestinyRanger.EditorTools.DestinyRangerSceneBuilder.ValidateHeroAnimationArtBatch
```

该验证会检查主角 8×4 的 32 帧动作表、敌人 6×4 的 24 帧动作表是否存在、保持可读、关闭 mipmap，并且每个帧格都有足够可见像素，防止换图后角色或敌人某些动作帧直接消失。

生成战斗状态预览图并验证战斗关键对象：

```bash
Unity -batchmode -quit -projectPath "<project>" -executeMethod DestinyRanger.EditorTools.DestinyRangerSceneBuilder.RenderCombatPreviewBatch
```

战斗预览图输出：

```text
/private/tmp/destiny-ranger-combat-preview.png
```

该验证会摆出玩家、宠物、近战怪、弓手、Boss、程序短线斩击、宠物弹、雷击细脊线、Boss 预警、命中 HUD 和少量短线确认，用来快速检查战斗画面密度；预览图不得使用大斩击贴图、外部雷击图、预览伤害数字或双行奖励字。

主角逐帧动作、运行时 FPS、命中帧和可见性兜底规格见 `Assets/DestinyRanger/Docs/HERO_ANIMATION_SPEC.md`。继续重做角色美术时，必须保持该规格或同步修改运行时帧表。

## 图片资产

所有正式图片资产均通过 Codex 的 `api-image` 生图技能生成：

- `Assets/DestinyRanger/Art/Generated/adventure-stage-forest.png`
- `Assets/DestinyRanger/Art/Generated/adventure-stage-forest-long-v1.png`
- `Assets/DestinyRanger/Art/Generated/adventure-hero-sheet.png`
- `Assets/DestinyRanger/Art/Generated/adventure-hero-action-sheet-v2.png`
- `Assets/DestinyRanger/Art/Generated/adventure-hero-anim-32-v1.png`
- `Assets/DestinyRanger/Art/Generated/adventure-enemy-sheet.png`
- `Assets/DestinyRanger/Art/Generated/adventure-enemy-boss-action-v2.png`
- `Assets/DestinyRanger/Art/Generated/adventure-enemy-anim-24-v1.png`
- `Assets/DestinyRanger/Art/Generated/adventure-companion-pet-v1.png`
- `Assets/DestinyRanger/Art/Generated/adventure-ui-sheet.png`
- `Assets/DestinyRanger/Art/Generated/adventure-platform-sheet.png`
- `Assets/DestinyRanger/Art/Generated/adventure-control-ui-sheet.png`
- `Assets/DestinyRanger/Art/Generated/adventure-hud-controls-v2.png`
- `Assets/DestinyRanger/Art/Generated/adventure-rune-ui-sheet.png`
- `Assets/DestinyRanger/Art/Generated/adventure-combat-vfx-sheet.png`
- `Assets/DestinyRanger/Art/Generated/destiny-ranger-concept.png`
- `Assets/DestinyRanger/Art/Generated/destiny-ranger-rune-ui.png`
- `Assets/DestinyRanger/Art/Generated/destiny-ranger-rune-icons-sheet.png`

完整来源、用途和上架检查口径见 `Assets/DestinyRanger/Docs/ASSET_PROVENANCE.md`。后续新增图片必须同步登记，避免混入第三方 IP、品牌、素材站或未授权图片。

## 下一步质量目标

- 继续提升角色和怪物逐帧动作质量：补更清晰的起手、收招、受击硬直和技能前摇帧
- 继续强化打击感：不同武器/符文命中特效、分层音效、Boss 大招镜头反馈和真机震动曲线
- 给 8 房间结构继续补商店/精英/Boss 专属房门正式贴图与音效，并继续替换奖励卡占位图标为更精细的正式图标
- 按 iPad 真机安全区和主流移动动作游戏拇指热区再做一轮 UI 微调
