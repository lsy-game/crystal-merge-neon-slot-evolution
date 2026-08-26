# Crystal Merge: Neon Slot Evolution

一个霓虹水晶主题的休闲合成游戏项目。仓库包含完整项目备份包，也包含可在手机浏览器中打开的 WebGame 试玩版。

## Online Demo

GitHub Pages 试玩链接：

https://lsy-game.github.io/crystal-merge-neon-slot-evolution/

第一次开启或刚更新 GitHub Pages 时，链接可能会短暂显示 `404`。这通常不是文件丢了，而是 GitHub 还在生成网页，等待 1 到 5 分钟后刷新即可。

## Downloads

完整项目备份包：

`backups/合成进化全部内容.zip`

WebGame 试玩压缩包：

`backups/合成进化-Web试玩包-clean.zip`

## Project Contents

- `docs/`：WebGame 试玩版，可通过 GitHub Pages 发布。
- `backups/合成进化全部内容.zip`：完整项目资料，适合备份、迁移电脑和后续继续开发。
- `backups/合成进化-Web试玩包-clean.zip`：只包含网页试玩需要的文件，适合单独下载或部署。

## 说明

- 当前备份包中没有 Android `.apk`、iOS `.ipa` 或 `.aab`，所以它不是直接安装到手机的安装包。
- 如果想让玩家最方便地体验，推荐使用 GitHub Pages 在线试玩链接。
- 网页试玩版会默认使用竖屏手机外框，普通电脑浏览器打开也会尽量保持这个比例。
- 如果之后要发布手机安装版，Android 需要导出 `.apk`，iPhone 需要 TestFlight 或 App Store。

## 后续怎么修改

如果只是修改网页试玩版，主要改 `docs/` 里面的文件。改完后同步到 GitHub，试玩链接会自动更新。

如果是修改完整工程，先在本地继续开发，再重新导出完整 zip，替换 `backups/合成进化全部内容.zip`，然后同步到 GitHub。

常规 Git 同步命令：

```bash
cd "/Users/zhendian/Documents/New project/CrystalMergeNeonSlotEvolution"
git add .
git commit -m "Update game"
git push
```

如果 `git push` 因网络问题失败，可以改用 GitHub 网页上传，或者再次使用 GitHub API 上传。

## GitHub Pages 设置

如果在线试玩链接还打不开，需要在 GitHub 仓库页面开启 Pages：

1. 打开仓库 `Settings`
2. 左侧找到 `Pages`
3. `Build and deployment` 选择 `Deploy from a branch`
4. `Branch` 选择 `main`
5. 文件夹选择 `/docs`
6. 点击 `Save`

保存后等待 1 到 3 分钟，再打开在线试玩链接。

## Recommended Repository Name

GitHub 仓库名建议使用：

`crystal-merge-neon-slot-evolution`

项目展示名可以继续使用：

`Crystal Merge: Neon Slot Evolution`
