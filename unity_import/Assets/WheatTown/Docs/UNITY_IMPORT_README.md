# Unity 原生导入准备说明

当前版本已经不是浏览器原型，也不是 Unity 网页壳。

## 当前工程

完整 Unity 工程路径：

```text
/Users/zhendian/Documents/New project/麦穗小镇_Unity工程
```

主场景：

```text
Assets/Scenes/WheatTownBootstrap.unity
```

主脚本：

```text
Assets/WheatTown/Scripts/WheatTownNativeGame.cs
```

## 运行方式

1. 用 Unity Hub 打开 `麦穗小镇_Unity工程`。
2. 打开 `Assets/Scenes/WheatTownBootstrap.unity`。
3. 点击 Play。

运行后不会打开浏览器，不会加载 `index.html`。所有主要页面由 Unity 原生 Canvas 和 C# 脚本生成。

## 已按最终规格书接入的内容

- 430×932 逻辑分辨率。
- 深绿顶部资源栏。
- 深绿底部导航栏：小镇 / 背包 / 任务 / 设置。
- 小镇主场景一屏布局。
- 农田生产链。
- 面包房加工。
- 居民委托。
- 背包网格。
- 任务板。
- 收获台小游戏入口。
- 设置弹窗。

## 美术素材

V2 新生成素材：

```text
Assets/WheatTown/Art/Images/native-v2/
```

包含农田、乳品间、居民小屋、老汤姆、收获台、公告栏、背包图标、任务图标、锁和镰刀等。
