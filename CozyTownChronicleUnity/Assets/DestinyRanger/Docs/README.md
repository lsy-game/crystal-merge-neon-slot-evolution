# 命运游侠·试炼版 v0.1 Unity 原型

这是一个可导入 Unity 的横屏 iPad 2D 动作 Roguelite 原型包，目标验证：

> 战斗房间获取命运符文，每次符文共鸣都明显改变打法。

## 已包含

- 单局 8 房间流程：教学、普通战斗、精英、商店、Boss、结算
- 玩家移动、闪避、光剑三连斩
- 碎晶仆从、弧光射手、毒素精英、守护者巨像、浮游炮
- 命运符文系统：首次保底剑气分裂，后续加权三格共鸣
- 核心符文效果：剑气分裂、闪避留痕、冰霜新星、暴击过载、回旋剑意、连锁电弧
- 属性符文：锋锐、坚盾、疾步、贪婪
- 商店：回血、购买破碎符文
- 最终命运评价：流派称号、符文列表、击败数、金币、时间
- `api-image` 生图技能生成的概念图、角色/物件资产表、符文 UI 参考图、符文图标资产表

## 创建场景

在 Unity 中打开本工程后：

1. 等待脚本编译完成。
2. 菜单选择 `Destiny Ranger/Create Prototype Scene`。
3. 打开 `Assets/Scenes/DestinyRangerPrototype.unity`。
4. 点击 Play。

也可以用 batchmode 调用：

```bash
Unity -batchmode -quit -projectPath "<project>" -executeMethod DestinyRanger.EditorTools.DestinyRangerSceneBuilder.CreatePrototypeSceneBatch
```

## 操作

- 键盘：`WASD/方向键` 移动，`Space` 攻击，`Left Shift` 或 `J` 闪避，`K` 释放冰霜新星，`N` 进入下一房间。
- 触屏/鼠标：左下摇杆区域移动，右下攻击/闪避/技能按钮操作。

## 设计取向

本原型参考了优秀动作 Roguelite 的通用设计经验：

- 房间推进与清场节奏：短房间、清场奖励、Boss 收束。
- 本局构筑：奖励不只是数值增长，而是改变攻击、闪避、击杀和技能事件。
- 移动端可读性：敌我颜色、危险预警、按钮数量都保持克制。

没有使用赌场化表达：界面叫“命运符文/启封/共鸣”，不使用老虎机、拉杆、筹码或真实博彩符号。

## 图片资产来源

所有正式图片资产均通过 Codex 的 `api-image` 生图技能生成，当前路径：

- `Assets/DestinyRanger/Art/Generated/destiny-ranger-concept.png`
- `Assets/DestinyRanger/Art/Generated/destiny-ranger-rune-ui.png`
- `Assets/DestinyRanger/Art/Generated/destiny-ranger-sprite-sheet.png`
- `Assets/DestinyRanger/Art/Generated/destiny-ranger-rune-icons-sheet.png`

当前可玩原型仍使用运行时代码生成的几何体来保证碰撞和手感稳定；上面的生图资产已经作为 Unity Sprite 导入，并作为后续切图母版和 UI 风格参考使用。

## 后续优先级

1. 手感调参：攻击范围、闪避距离、敌人前摇、Boss 安全窗口。
2. 把本地 PNG 符文图标绑定到 UI 转轮。
3. 将符文定义拆成 ScriptableObject 数据资产。
4. 加入音效、震屏、粒子和受伤闪白恢复。
5. 扩展第二把武器和更多主动技能。
