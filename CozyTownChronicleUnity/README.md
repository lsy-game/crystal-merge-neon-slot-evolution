# 麦穗小镇 Unity 工程骨架

这是当前网页原型的 Unity 工程骨架，已导入到 `Assets/StreamingAssets/WheatTownWeb`。

## 打开方式

用 Unity Hub 或 Unity 2022.3.62f3c1 打开本目录：

```text
麦穗小镇_Unity工程
```

## 启动场景

启动场景已生成：

```text
Assets/Scenes/WheatTownBootstrap.unity
```

并已写入 Build Settings：

```text
ProjectSettings/EditorBuildSettings.asset
```

运行场景后，点击 `Open Wheat Town` 会通过 `Application.OpenURL` 打开本地网页原型。

如需重新生成场景，打开工程后执行菜单：

```text
WheatTown/Create Bootstrap Scene
```

## 内嵌 WebView

当前工程未绑定第三方 WebView 插件。若需要 App 内嵌显示，应接入 UniWebView、Vuplex WebView 或平台原生 WebView，并加载同一个本地入口：

```text
Application.streamingAssetsPath/WheatTownWeb/index.html
```

## 已包含

- 假登录
- 游客登录
- 隐私协议
- 用户协议
- 大厅
- 收获页
- 日程页
- 城镇页
- 设置

## 验证状态

已通过 Unity 2022.3.62f3c1 batchmode 验证：

- 工程可打开
- 脚本可编译
- 启动场景可生成
- 启动场景已加入 Build Settings

日志文件：

```text
unity-create-scene.log
unity-verify-final.log
```
