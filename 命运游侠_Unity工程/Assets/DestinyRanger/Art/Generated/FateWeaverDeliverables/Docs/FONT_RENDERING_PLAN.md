# Fate Weaver Font Rendering Plan

## 标题：命运纺机

- 推荐字体：Cinzel / Songti SC Bold 风格。当前机器未发现项目内 Cinzel 文件，Unity 工程可先用系统 `/System/Library/Fonts/Supplemental/Songti.ttc` 生成 TextMeshPro Font Asset。
- 生成方式：Unity 打开后执行 `Destiny Ranger/Typography/Create TMP Font Assets`，或在 TextMeshPro Font Asset Creator 中使用 Songti / Cinzel TTF/OTF，Sampling Point Size 90，Padding 9，Atlas 2048。
- 清晰度方案：标题字号 72-80，SDF 渲染，金色 `#D4AF37`，黑色 3px 投影，移动端 Canvas Scaler 使用 1290x2796 reference resolution。

## 正文

- 推荐字体：Source Han Sans SC / PingFang SC / STHeiti。当前机器可用 `/System/Library/Fonts/STHeiti Medium.ttc`。
- 渲染方式：TextMeshProUGUI，正文 32-40，小字 24-28，SDF atlas 2048，Fallback Font Assets 包含 Songti 与 STHeiti。
- TextMeshPro 状态：Package `com.unity.textmeshpro` 已在 `Packages/manifest.json` 配置；本轮因 Unity Licensing IPC 无法进入编辑器，未能实际生成 TMP_FontAsset 文件。已交付编辑器脚本入口，Unity 授权恢复后可一键生成。
