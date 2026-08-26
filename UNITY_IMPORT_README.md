# Unity 导入说明

当前项目已经同时提供两种 Unity 交付形态：

```text
麦穗小镇_Unity工程/   # 可直接用 Unity 2022.3 打开的中文名工程骨架
unity_import/    # 可拷贝到其他 Unity 工程的导入包
```

## 直接打开 Unity 工程

推荐优先使用：

```text
麦穗小镇_Unity工程/
```

已生成并加入 Build Settings 的启动场景：

```text
麦穗小镇_Unity工程/Assets/Scenes/WheatTownBootstrap.unity
```

运行场景后，点击 `Open Wheat Town` 会通过 `Application.OpenURL` 打开本地 HTML 游戏入口：

```text
Assets/StreamingAssets/WheatTownWeb/index.html
```

已用 Unity 2022.3.62f3c1 batchmode 验证项目可打开、脚本可编译、场景可生成。验证日志：

```text
麦穗小镇_Unity工程/unity-verify-final.log
```

## Unity 可导入包

如果要合入别的 Unity 项目，使用：

```text
unity_import/
```

## 已准备好的 Unity 可用资源

- `index.html`：完整竖屏游戏原型入口
- `game.js`：登录、协议、大厅、收获、日程、城镇逻辑
- `styles.css`：竖屏 UI 与大厅/登录样式
- `assets/images/`：全部 WebP/PNG 美术资源
- `assets/images/lobby-background.webp`：登录与大厅背景
- `previews/redesign-auth.png`：登录页验证截图
- `previews/redesign-lobby.png`：大厅页验证截图
- `previews/redesign-slot.png`：收获页验证截图

## 推荐 Unity 集成方式

### 方式 A：WebView 导入

适合最快把当前原型放进 Unity。把 `unity_import/Assets` 合入目标 Unity 工程，Unity 通过 WebView 或外部浏览器打开 `index.html`。

本项目已生成目标结构：

```text
unity_import/Assets/
  StreamingAssets/
    WheatTownWeb/
      index.html
      game.js
      styles.css
      assets/
  WheatTown/
    Art/
    Docs/
    Editor/
    Scripts/
```

其中：

- `Assets/WheatTown/Scripts/WheatTownWebLauncher.cs` 是 Unity 启动脚本
- `Assets/WheatTown/Editor/WheatTownSceneBuilder.cs` 可重新生成启动场景
- `Assets/Scenes/WheatTownBootstrap.unity` 是已生成的启动场景

没有 WebView 插件时，它会通过 `Application.OpenURL` 打开本地 HTML；如果接入 UniWebView、Vuplex WebView 或平台原生 WebView，可以加载同一个本地 URL。

### 方式 B：Unity 原生重做 UI

适合正式产品化。将 `assets/images/` 导入 Unity Sprite，按现有 HTML/CSS 页面作为 UI 蓝图，在 Canvas 里重建登录页、大厅、收获页、日程页和城镇页。

建议 Unity 场景：

```text
Assets/Scenes/Login.unity
Assets/Scenes/Lobby.unity
Assets/Scenes/Game.unity
```

## 当前限制

当前 Unity 版本是“Web 包导入 + 启动场景”方案，还没有内置第三方 WebView 插件。因此它能验证 Unity 工程导入和本地入口打开；若要在 App 内完全嵌入显示，需要再接入 WebView 插件，或把 HTML/CSS UI 进一步重做成 Unity 原生 Canvas。
