# 命运游侠 v0.1 主角动作帧规格

## 目标

主角动作必须先满足移动端动作游戏的读招、响应和命中确认，再追求更复杂的动画量。当前 32 帧表使用 `Assets/DestinyRanger/Art/Generated/adventure-hero-anim-32-v1.png`，运行时按主角 8×4 动画表切片。

## 帧表分配

| 帧号 | 动作 | 用途 |
|------|------|------|
| 0-3 | 待机循环 | 呼吸、光刃轻摆、站姿稳定。 |
| 4-9 | 跑步循环 | 6 帧循环，脚步节奏和地面尘同步。 |
| 10 | 起跳 | 跳跃起手，身体上提。 |
| 11 | 下落 | 空中下落和落地前姿态。 |
| 12-13 | 受击 | 受击后仰和回正，配合闪白、击退和红边。 |
| 14 | 胜利/结算 | Boss 击败后短暂展示。 |
| 15 | 闪避 | 冲刺压低重心，配合残影和无敌窗口。 |
| 16-18 | 一斩 | 起手、命中、收招。 |
| 19-21 | 二连 | 起手、命中、收招，颜色提示偏金。 |
| 22-25 | 三连破势 | 蓄势、重斩命中、剑光拉出、收招。 |
| 26-27 | 技能蓄势 | 所有主动技能共用前摇。 |
| 28 | 剑气释放 | 直线剑气释放帧。 |
| 29 | 炎弹释放 | 抬手/挥剑发射帧。 |
| 30 | 冰环释放 | 原地扩散释放帧。 |
| 31 | 雷击释放 | 竖向引雷释放帧。 |

## 运行时帧率

| 动作 | 运行时常量 | FPS | 说明 |
|------|------------|-----|------|
| 待机 | `IdleAnimFps` | 5 | 稳定不抢眼，避免标题页和商店页烦躁。 |
| 跑步 | `RunAnimFps` | 12 | 配合 `RunFootstepInterval`，在 60 FPS 下有清楚脚步节奏。 |
| 跳跃 | `JumpAnimFps` | 14 | 起跳反馈短，避免按下后拖沓。 |
| 受击 | `HurtAnimFps` | 12 | 受击读得出，但不长时间锁死角色。 |
| 一斩/二连 | `LightAttackAnimFps` | 18 | 轻攻击命中帧约 0.075 秒后结算。 |
| 三连破势 | `HeavyAttackAnimFps` | 20 | 重攻击命中帧约 0.12 秒后结算，并有更强镜头/停顿。 |
| 技能 | `SkillAnimFps` | 16 | 前摇 2 帧，释放帧后 0.125 秒结算技能。 |

## 打击帧规则

- 一斩、二连在第 2 张动作帧附近结算命中，触发 `ApplyAttackHitFrameImpulse`、`MeleeImpactCore`、`MeleeImpactBand`、轻命中 HitStop 和小幅镜头震动。
- 三连破势使用第 2-3 张动作帧作为重命中窗口，触发更大的 `HeavyHitFrameLunge`、重命中 HitStop、强镜头震动和短闪白。
- 技能必须按“按钮脉冲 -> 角色蓄势帧 -> 地面/方向预告 -> 释放帧 -> 伤害/弹体/范围效果”顺序确认输入。
- 攻击输入允许 `AttackInputBufferDuration` 的预输入窗口，但命中仍必须等到动作帧的 `ResolveMeleeAttack` 延迟后结算；预输入只能影响下一次动作触发，不能让伤害提前发生。
- `HeroBaseVisualScale` 是主角基础视觉缩放；`RestoreHeroRenderer` 不能在 `heroPosePunchTimer` 仍然生效时重置缩放，否则攻击、落地和技能释放的身体压缩/伸展会被换帧吃掉。
- 闪避、攻击和技能起手都禁止复制主角贴图做残影；如需连续性提示只能使用短寿命、低透明的程序化硬边线条。三连优先使用 `HeroHeavyAttackCleanPoseLine`、`PlayerAttackUsesProceduralCleanSlashOnly`、命中火花和身体压缩/伸展；技能起手只保留 `HeroSkillSharpCastCore`、`HeroSkillClearHandTick`、`HeroSkillPoseReadableWhiteEdge`、`HeroSkillHandSparkPin_ThinNoBlur` 这类少量锚点，脚底暗块使用 `HeroSkillFootAnchorAo_DisabledNoGroundBlob` 透明禁用，避免三连和技能穿插时出现抠图不净、脚底黑影或重影观感。
- 技能释放帧必须额外生成 `HeroSkillReleaseFrameSnap`、`HeroSkillReleaseFootLock`，并通过 `SpawnHeroSkillCrispPoseBracket` 加上 `HeroSkillCrispFrontEdge`、`HeroSkillCrispBackEdge`、`HeroSkillCrispFootPin`；冰环/雷击这种原地范围技能还要生成 `HeroSkillReleaseCenterTick`，冰环用 `FrostNovaCastPosePin` 锁住脚底，雷击用 `ThunderCrispVerticalSpine` 和短分支线替代外部模糊序列，帮助移动端远景下确认哪一帧真正释放技能，但不能扩大成遮挡角色的大面积糊光。
- `HeroActionBeatText` 是动作节拍 HUD：起跳、闪避、一斩/二连/三连、真实命中帧和技能释放帧都必须通过 `ShowHeroActionBeat` 短暂显示动作名、帧号区间、动作 FPS、当前主角帧和命中/释放时间点，方便真机调帧并确认角色没有丢帧；该显示由设置页 `HeroActionBeatToggle` 控制，正式默认关闭。
- 闪避帧必须和无敌窗口、完美闪避窗口、残影、擦弹清弹反馈同步，不能只位移不换帧。
- 跑步、起跳、落地必须有地面尘或光痕；平台跳跃时必须通过 `HeroLandingPredictor` 给将落到地面或平台的低透明落脚环。

## 可见性兜底

- `ValidateHeroAnimationArtBatch` 必须检查 8×4 主角表和 6×4 敌人表不是空帧。
- 运行时 `SetHeroFrame` 必须调用 `HeroFrameHasVisibleBody`；如果某帧透明或裁切失败，回退到最近有效角色帧。
- 如果图集或切片完全失效，`UseEmergencyHeroSprite` 必须显示淡蓝占位体，避免主角在真机上消失。

## 后续美术升级要求

- 下一版主角重绘仍保持 32 帧兼容，除非同步修改代码帧表和本规格。
- 每个攻击动作必须有清楚的剪影变化：起手收紧、命中打开、收招回正。
- 技能释放帧要让剑气、炎弹、冰环、雷击四种技能能在角色姿态上被区分，不只靠特效颜色。
- 所有主角帧要保证 2732×2048 iPad 横屏下能从背景、伙伴和特效中读出来。
