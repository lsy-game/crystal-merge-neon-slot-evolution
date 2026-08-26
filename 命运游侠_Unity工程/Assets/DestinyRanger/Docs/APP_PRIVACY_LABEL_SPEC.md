# 命运游侠 v0.1 App Privacy 标签规格

## App Store Connect 建议填写

- Data Collected: No.
- Tracking: No.
- Third-Party Advertising: No.
- Developer's Advertising or Marketing: No.
- Analytics: No.
- Product Personalization: No.
- App Functionality data collected off device: No.

## 依据

- 当前版本离线运行，不创建账号。
- 当前版本不接入广告、分析 SDK、联网排行、云存档、远程配置或账号系统。
- 当前版本只用 `PlayerPrefs` 在本机保存设置、试炼热度、教程状态、最佳通关时间、最高击败记录、今日试炼本机最佳、本地成就和触控按钮偏移。
- `PlayerPrefs` 内容不上传、不共享、不用于跨 App 跟踪，也不发送到开发者服务器。
- 玩家可在游戏内“关于/隐私”面板二次确认后清除本机数据。

## 必须保持一致的地方

- 运行时 `AboutPrivacyText` 必须显示“App Privacy 标签建议：未收集数据”。
- `APP_STORE_READINESS.md` 必须说明 App Privacy 建议口径为“未收集数据”。
- `RELEASE_METADATA_SPEC.md` 的隐私政策页面要求必须覆盖本机 `PlayerPrefs` 保存项和清除路径。
- `LOCAL_SAVE_SPEC.md` 新增任何长期保存键后，必须重新检查本文件。

## 需要重新评估的情况

- 接入广告、分析、崩溃上报、远程配置、账号、云存档、联网排行、社交分享或服务器日志。
- 上传 PlayerPrefs、设备标识、地区、诊断信息、支付信息或用户生成内容。
- 接入 Game Center、IAP、订阅、推送通知或第三方 SDK。
- 将 `SLOT` 或其他随机奖励与真钱购买、广告激励、现实奖品或跨局付费收益绑定。

发生以上任一变化时，不能继续使用“未收集数据”口径，必须更新 App Store Connect 隐私标签、隐私政策、`APP_STORE_READINESS.md` 和运行时关于/隐私面板。
