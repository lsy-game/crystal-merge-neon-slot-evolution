# 命运游侠 v0.1 本地存档规格

## 存储范围

- 存储方式：Unity `PlayerPrefs`。
- 存储位置：设备本机。
- 网络行为：不上传、不同步、不接账号、不接云存档、不接联网排行。
- 当前 schema：`DestinyRanger.SaveSchemaVersion = 1`。
- 迁移入口：运行时启动加载设置前调用 `EnsureLocalSaveSchema`，旧版本通过 `MigrateLocalSaveSchema` 处理。
- 清除入口：标题页或设置页打开“关于/隐私”，二次确认“清除本机数据”。

## 固定键

以下键必须保留在运行时 `LocalPlayerPrefKeys` 中，清除本机数据时统一删除。

| Key | 内容 |
| --- | --- |
| `DestinyRanger.SaveSchemaVersion` | 本地存档 schema 版本 |
| `DestinyRanger.AssistMode` | 辅助模式 |
| `DestinyRanger.AutoAttack` | 自动攻击 |
| `DestinyRanger.AutoAim` | 自动朝向 |
| `DestinyRanger.Sound` | 音效开关 |
| `DestinyRanger.Haptics` | 震动开关 |
| `DestinyRanger.EffectsIntensity` | 标准/舒适低闪 |
| `DestinyRanger.MasterVolume` | 总音量 |
| `DestinyRanger.SfxVolume` | 音效音量 |
| `DestinyRanger.MusicVolume` | 音乐音量 |
| `DestinyRanger.CompactControls` | 紧凑按钮布局 |
| `DestinyRanger.ControlSizeMode` | 按钮大小档位 |
| `DestinyRanger.ControlReachMode` | 右手按钮位置档位 |
| `DestinyRanger.ControlOpacityMode` | 右侧按钮透明度档位 |
| `DestinyRanger.CombatControlOffsetX` | 右侧按钮组横向偏移 |
| `DestinyRanger.CombatControlOffsetY` | 右侧按钮组纵向偏移 |
| `DestinyRanger.TrialHeatLevel` | 本地试炼热度 |
| `DestinyRanger.FrameRateMode` | 60 FPS / 30 FPS 档位 |
| `DestinyRanger.HeroActionBeatHud` | 开发调帧用动作帧显示，正式默认关闭 |
| `DestinyRanger.OnboardingSeen` | 首局简报已读 |
| `DestinyRanger.TutorialCompleted` | 教学完成 |
| `DestinyRanger.BestClearTime` | 本机最佳通关时间 |
| `DestinyRanger.BestRunKills` | 本机最高击败数 |
| `DestinyRanger.DailySeed` | 今日试炼本地日期种子 |
| `DestinyRanger.DailyBestTime` | 今日试炼本机最佳时间 |
| `DestinyRanger.DailyBestKills` | 今日试炼本机最高击败数 |

## 生成键族

这些键由运行时函数生成，不能手写散落到其他模块。

| 函数 | Key 形态 | 内容 |
| --- | --- | --- |
| `CombatButtonOffsetXKey(id)` | `DestinyRanger.ControlButtonOffset.<id>.X` | 单个右手按钮横向微调 |
| `CombatButtonOffsetYKey(id)` | `DestinyRanger.ControlButtonOffset.<id>.Y` | 单个右手按钮纵向微调 |
| `AchievementKey(id)` | `DestinyRanger.Achievement.<id>` | 本地成就解锁状态 |

当前 `CombatControlIds`：`Attack`、`Jump`、`Dodge`、`RuneOpen`、`Skill1`、`Skill2`、`Skill3`、`Skill4`。

当前 `AchievementIds`：`ClearTrial`、`NoDamageClear`、`FastClear15`、`RuneRegular`、`PerfectRanger`、`ComboPress`、`RoomPlanner`、`AdaptiveTrial`、`ShieldSustain`、`HeatClear`、`CanopyMaster`、`BrokenBridgeMaster`、`RuneRidgeMaster`、`RouteArchivist`。

## 迁移规则

- 新增固定 PlayerPrefs 键时，必须同步更新 `LocalPlayerPrefKeys`、本文档和 `ValidateLocalSaveSchemaBatch`。
- 新增生成键族时，必须提供统一 key 函数，并在 `ClearLocalData` 中通过 ID 列表删除。
- 修改键名时，必须在 `MigrateLocalSaveSchema` 中读取旧键、写入新键，再删除旧键。
- 新增会影响隐私口径的数据时，必须同步更新 `APP_PRIVACY_LABEL_SPEC.md`、`APP_STORE_READINESS.md`、游戏内 `AboutPrivacyText` 和上架元数据。

## 清除后状态

“清除本机数据”会删除固定键、单个按钮偏移键和本地成就键，并把内存状态恢复为默认设置、未读简报、未完成教学、无最佳成绩、无今日最佳和无本地成就。当前对局临时状态不作为长期存档处理。
