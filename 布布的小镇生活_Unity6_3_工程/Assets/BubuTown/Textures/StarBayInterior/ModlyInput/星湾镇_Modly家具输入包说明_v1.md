# 星湾镇 Modly 家具输入包说明 v1

这些图片用于 Modly / Hunyuan3D Mini 的 image-to-3D 生成。每张图都保持白底、单物体、三分之二视角，方便生成较稳定的 GLB 轮廓。

## 输入图

- `目标公寓_粉木单人床_Modly输入图.png`
- `目标公寓_海蓝小沙发_Modly输入图.png`
- `目标公寓_原木小茶几_Modly输入图.png`
- `目标公寓_灰台面粉柜厨房_Modly输入图.png`
- `目标公寓_原木床头柜_Modly输入图.png`
- `目标公寓_圆润台灯_Modly输入图.png`
- `目标公寓_搬家纸箱堆_Modly输入图.png`
- `目标公寓_窗帘落地窗_Modly输入图.png`

## 建议生成设置

- Quality: Balanced 或 High
- Mesh Resolution: Medium，确认外形后再对重点家具用 High
- Guidance Scale: 5.5 到 7.0
- Seed: 先固定一个 seed 便于复现，满意后记录到家具接入表

## 导入 Unity 前检查

- 模型底部是否落在地面。
- 模型正面是否朝向房间相机或可通过 90 度旋转修正。
- 尺寸是否符合三楼小套间：床约 `1.34 x 0.70 x 1.78`，沙发约 `1.62 x 0.70 x 0.72`。
- 没有穿墙、浮空、倒地后，再替换 `09_目标图方向三楼单身公寓` 中的同名家具。
