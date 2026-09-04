# 星湾镇室内 Modly 图生 3D 计划 v1

## 流程

- 先生成单物体、白底、三分之四视角输入图，保证轮廓清楚。
- 用 Modly 的 Hunyuan3D 2 Mini 扩展把图片转为 GLB mesh。
- 导入 Unity 前先在独立校准区检查：朝向、落地、比例、碰撞盒。
- 校准通过后再替换 `09_目标图方向三楼单身公寓` 里的同名家具。
- 纹理第一版先用 Unity 材质重染；如果后续装好 Hunyuan3D Paint，再补贴图烘焙。

## 第一批输入图

| 家具 | 输入图 | 替换目标 | 建议 Unity 尺寸 |
| --- | --- | --- | --- |
| 粉木单人床 | `Assets/BubuTown/Textures/StarBayInterior/ModlyInput/目标公寓_粉木单人床_Modly输入图.png` | `目标公寓_粉木单人床_组合模型` | `1.34 x 0.70 x 1.78` |
| 海蓝小沙发 | `Assets/BubuTown/Textures/StarBayInterior/ModlyInput/目标公寓_海蓝小沙发_Modly输入图.png` | `目标公寓_海蓝沙发_CC0_Kenney稳定摆放` | `1.62 x 0.70 x 0.72` |
| 原木小茶几 | `Assets/BubuTown/Textures/StarBayInterior/ModlyInput/目标公寓_原木小茶几_Modly输入图.png` | `目标公寓_原木茶几_CC0_Kenney稳定摆放` | `0.88 x 0.36 x 0.58` |
| 灰台面粉柜厨房 | `Assets/BubuTown/Textures/StarBayInterior/ModlyInput/目标公寓_灰台面粉柜厨房_Modly输入图.png` | `目标公寓_CC0厨房组合` | `1.85 x 1.10 x 0.62` |

## 第二批输入图

| 家具 | 输入图 | 替换目标 | 建议 Unity 尺寸 |
| --- | --- | --- | --- |
| 原木床头柜 | `Assets/BubuTown/Textures/StarBayInterior/ModlyInput/目标公寓_原木床头柜_Modly输入图.png` | `目标公寓_原木床头柜` | `0.42 x 0.48 x 0.38` |
| 圆润台灯 | `Assets/BubuTown/Textures/StarBayInterior/ModlyInput/目标公寓_圆润台灯_Modly输入图.png` | `目标公寓_床头暖光台灯` | `0.24 x 0.46 x 0.24` |
| 搬家纸箱堆 | `Assets/BubuTown/Textures/StarBayInterior/ModlyInput/目标公寓_搬家纸箱堆_Modly输入图.png` | `目标公寓_搬家纸箱堆` | `0.88 x 0.80 x 0.52` |
| 窗帘落地窗 | `Assets/BubuTown/Textures/StarBayInterior/ModlyInput/目标公寓_窗帘落地窗_Modly输入图.png` | `目标公寓_大窗与窗帘组合` | `2.70 x 1.65 x 0.18` |

## 本机 Modly 路径

- 工作区：`/Users/zhendian/Documents/Modly/workspace/`
- 模型权重：`/Users/zhendian/Documents/Modly/models/hunyuan3d-mini/generate/`
- Hunyuan3D Mini 扩展：`/Users/zhendian/Documents/Modly/extensions/hunyuan3d-mini/`
- 本地运行时端口：`http://127.0.0.1:8765`，只有 Modly 应用启动时才会开启。
- 当前整理包：`/Users/zhendian/Documents/Modly/workspace/StarBayInteriorFurnitureInputs/`
- 联系表：`Assets/BubuTown/Textures/StarBayInterior/ModlyInput/星湾镇_Modly家具输入图_联系表_v2.png`

## 已生成试件

| 家具 | GLB | FBX | 状态 |
| --- | --- | --- | --- |
| 原木床头柜 | `Assets/BubuTown/External/Generated/StarBayFurnitureModly/RawGLB/目标公寓_原木床头柜_Modly低清试件.glb` | `Assets/BubuTown/External/Generated/StarBayFurnitureModly/Models/目标公寓_原木床头柜_Modly低清试件.fbx` | 已接入目标公寓，落地稳定 |
| 粉木单人床 | `Assets/BubuTown/External/Generated/StarBayFurnitureModly/RawGLB/目标公寓_粉木单人床_Modly低清试件.glb` | `Assets/BubuTown/External/Generated/StarBayFurnitureModly/Models/目标公寓_粉木单人床_Modly低清试件.fbx` | 已接入目标公寓，落地稳定 |
| 海蓝小沙发 | `Assets/BubuTown/External/Generated/StarBayFurnitureModly/RawGLB/目标公寓_海蓝小沙发_Modly低清试件.glb` | `Assets/BubuTown/External/Generated/StarBayFurnitureModly/Models/目标公寓_海蓝小沙发_Modly低清试件.fbx` | 已接入目标公寓，落地稳定 |
| 原木小茶几 | `Assets/BubuTown/External/Generated/StarBayFurnitureModly/RawGLB/目标公寓_原木小茶几_Modly低清试件.glb` | `Assets/BubuTown/External/Generated/StarBayFurnitureModly/Models/目标公寓_原木小茶几_Modly低清试件.fbx` | 已接入目标公寓，落地稳定 |
| 灰台面粉柜厨房 | `Assets/BubuTown/External/Generated/StarBayFurnitureModly/RawGLB/目标公寓_灰台面粉柜厨房_Modly低清试件.glb` | `Assets/BubuTown/External/Generated/StarBayFurnitureModly/Models/目标公寓_灰台面粉柜厨房_Modly低清试件.fbx` | 已接入目标公寓，落地稳定 |
| 圆润台灯 | `Assets/BubuTown/External/Generated/StarBayFurnitureModly/RawGLB/目标公寓_圆润台灯_Modly低清试件.glb` | `Assets/BubuTown/External/Generated/StarBayFurnitureModly/Models/目标公寓_圆润台灯_Modly低清试件.fbx` | 已接入目标公寓床头柜，落点稳定 |
| 搬家纸箱堆 | `Assets/BubuTown/External/Generated/StarBayFurnitureModly/RawGLB/目标公寓_搬家纸箱堆_Modly低清试件.glb` | `Assets/BubuTown/External/Generated/StarBayFurnitureModly/Models/目标公寓_搬家纸箱堆_Modly低清试件.fbx` | 已接入目标公寓，远景识别需继续优化 |

## 当前注意事项

- 不直接把未校准的高质量模型塞进房间；上一轮已经证明外部模型轴向不稳会导致沙发倒地、床品浮空。
- Modly Mini 主要输出 mesh；贴图/材质要单独做。
- 家具进入 Unity 后必须先通过 `FitToBounds` 和 `DropToLocalGround`，再人工截图确认。
- 图片转 3D 后先放到独立校准区，不直接替换房间，避免再次出现床浮空、沙发倒地、桌子方向错乱。
- 纸箱堆第一版进房间后在俯视图中仍偏团块；下一轮建议重绘输入图为更硬边、更分层的正交三分之四纸箱，再生成第二版。
