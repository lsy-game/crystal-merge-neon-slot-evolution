# 命运游侠 v0.1 上架元数据规格

## 必填链接

当前工程是离线试玩验证版，正式提交 App Store 前必须准备并替换以下链接：

- 隐私政策 URL：`TODO_PRIVACY_POLICY_URL`
- 支持 URL：`TODO_SUPPORT_URL`
- 营销/官网 URL：可选，若没有正式官网可不填，但不能使用无关页面。

这些 URL 必须在 App Store Connect 元数据中填写，并在游戏内“关于/隐私”面板保持一致。

开发版运行时“关于/隐私”面板会显示 `AboutReleaseBlockerText`，提醒当前仍含 TODO 链接。替换真实链接时必须同步更新 `ReleaseBlockerStatusText` 的状态文案，避免正式包仍显示开发阻断提示。

替换真实链接后运行最终提交阻断验证：

```bash
Unity -batchmode -quit -projectPath "<project>" -executeMethod DestinyRanger.EditorTools.DestinyRangerSceneBuilder.ValidateFinalSubmissionBlockingBatch
```

该验证会扫描运行时“关于/隐私”文本和本文件，任何 `TODO_PRIVACY_POLICY_URL` 或 `TODO_SUPPORT_URL` 残留都视为不能提交。

## 隐私政策页面必须说明

- 游戏离线运行，不创建账号。
- 当前版本不接入广告、分析 SDK、联网排行、云存档或远程配置。
- 本机只使用 `PlayerPrefs` 保存设置、教程状态、成绩记录、今日试炼本机最佳和本地成就。
- 玩家可在游戏内“关于/隐私”面板二次确认后清除本机数据。
- `SLOT` 是战斗内命运符文表现，不含真钱付费、现金价值、广告激励或现实物品兑换。

## App Privacy 标签

- App Store Connect 建议按 `Assets/DestinyRanger/Docs/APP_PRIVACY_LABEL_SPEC.md` 填写“未收集数据”。
- 如果未来接入广告、分析、账号、联网排行、云存档、IAP、Game Center、推送通知或任何第三方 SDK，必须先更新 App Privacy 标签、隐私政策和游戏内“关于/隐私”口径。

## 支持页面必须说明

- 游戏名：命运游侠。
- 版本：v0.1 试玩验证版。
- 支持范围：启动、触控、音效/震动、清除本机数据、SLOT 规则和设备兼容问题。
- 联系方式或反馈入口必须真实可用，不能是占位文字。

## 上架前禁止项

- 不能把 `TODO_PRIVACY_POLICY_URL` 或 `TODO_SUPPORT_URL` 原样提交到 App Store Connect。
- 不能使用和本游戏无关的网站、临时网盘、私人不可访问页面或需要登录才能查看的隐私/支持页面。
- 不能在商店页截图或描述中暗示 `SLOT` 有现金、下注、提款、奖金池或现实奖品。

## 人工检查

- App Store Connect 的隐私政策 URL 和支持 URL 可在无登录状态打开。
- 游戏内“关于/隐私”显示的链接与 App Store Connect 一致。
- 清除本机数据路径、SLOT 无现金价值口径和 App Privacy “未收集数据”口径一致。
- `APP_PRIVACY_LABEL_SPEC.md`、`APP_STORE_READINESS.md` 和运行时 `AboutPrivacyText` 的隐私标签口径一致。
- `ValidateFinalSubmissionBlockingBatch` 通过，确认链接占位和运行时占位文案已清理。
