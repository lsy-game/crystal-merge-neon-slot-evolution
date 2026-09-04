# 大蘑菇 Blender Skinned Mesh 原型

生成时间：2026-09-01

## 文件

- `DaMogu_Blender_SkinnedMeshPrototype.blend`：可继续在 Blender 中编辑的源文件。
- `DaMogu_Blender_SkinnedMeshPrototype.fbx`：Unity Humanoid 验证用 FBX。
- `DaMogu_Blender_SkinnedMeshPrototype.glb`：通用 glTF/GLB 预览与备份。

## 当前内容

这版基于现有 VRM 候选 A 的骨架生成了真正的 skinned mesh 原型：

- 粉白 hoodie 外壳
- 白色帽子体积
- 左右蓬袖和粉色袖口
- 真实网格衣服细节：软领口、肩袖连接、袖口罗纹、衣摆罗纹、斜插袋、口袋面片、拉链齿、抽绳、前后拼接线、后背布褶、衣摆软褶、袖子斜向压痕、帽子中缝
- 所有自生成 skinned mesh 已补自动 UV，Unity 预览可使用细微重复布纹贴图
- Blender 预览材质带轻量程序噪声 bump，用于判断布料颗粒方向
- 收窄后的深蓝 skort 与内短裤
- 裸腿覆盖层
- 粉白鞋点缀
- 斜挎包和包带
- 后脑半扎丸子和编发提示

Unity 导入验证脚本位于：

`Assets/DaMoguArtPreview/Editor/DaMoguBlenderSkinnedMeshUnityImporter.cs`

该脚本会把 FBX 配置为 Humanoid，并生成：

- `Assets/Scenes/DaMoguBlenderSkinnedMeshPreview.unity`
- `/Users/zhendian/Desktop/大蘑菇角色美术预览输出/大蘑菇_BlenderFBX_Unity导入预览.png`
- `/Users/zhendian/Desktop/大蘑菇角色美术预览输出/大蘑菇_BlenderFBX_Unity走路检查图.png`

## 已知问题

这不是最终美术成品，而是“Blender 真网格 + Unity 可动”链路验证和衣服细化原型。当前已能在 Unity 中作为有效 Humanoid FBX 跑走路动作，Unity 预览材质也已优先使用 `VRM/MToon` 或 `Standard` 来显示衣服厚度与布纹；但 hoodie 大形仍偏硬，hood 背面还像大白块，袖子/肩部权重还需要手工修，发型只是结构提示。

下一步如果继续朝目标三视图推进，应优先在 Blender 里手工编辑：

- hoodie 纸样轮廓：把后背和侧面从梯形硬壳改成更贴近参考图的柔软外套弧面。
- 袖子垂感：用真实 A-pose/动作姿态检查袖口和手腕，重刷 upper/lower arm 权重。
- 布料细节：把当前程序化压线合并成更干净的口袋、拉链、罗纹和缝线网格，减少细碎小条。
- UV/贴图：把当前自动投影 UV 改成手工 UV 岛，让衣服纹理按袖子、衣身、帽子分别走向。
- hood 和头发：用更完整的 hood 内外层和真实发束替换当前提示性几何。

## 2026-09-01 Unity 侧骨骼跟随补强

本轮没有重新导出 Blender FBX，因为 Blender 5.2.1 命令行启动卡在 Metal/GPU backend 初始化；但 Unity 导入验证链路继续可用，`Avatar isHuman=True`、`isValid=True`。

Unity 导入器现在会在走路采样后追加一层骨骼跟随补强，用来临时改善背影读图：

- 原 FBX hood 保留，只叠加细小背部缝线，避免新增白色硬块。
- 原偏硬包体/包带隐藏，改用更小的圆润包体和更细包带贴近右髋。
- 手腕袖口挂到左右手骨骼附近，走路截图里能跟着采样姿势。
- 后脑编发提示缩小，避免和已有半丸子头叠成双团。

这些补强仍是 Unity 预览层，不是最终 Blender 源网格。后续要达到目标三视图，仍应回到 Blender 源文件手工修衣服纸样、权重和发束。

## 2026-09-01 Unity 眼睛材质与截图验证

本轮继续使用现有 FBX，没有重新导出源网格。Unity 导入器新增：

- 从 VRoid 纹理文件夹加载眼白、虹膜、高光、眼线、睫毛、眉毛和嘴巴贴图。
- 暖棕虹膜优先使用 `DaMogu_VRM_Candidate_A.Textures_Overrides_eyeiris_warmbrown.png`。
- 眼部和表情贴片改为透明材质，并把虹膜/高光/眼线子网格轻微前移，避免再次出现白眼。
- 保留原 hoodie shell，撤回会形成硬方块的大面积 Unity 布片。

已验证输出：

- `Assets/DaMoguArtPreview/Docs/DaMoguBlenderSkinnedMesh_UnityFaceCheck.png`
- `Assets/DaMoguArtPreview/Docs/DaMoguBlenderSkinnedMesh_UnityPolishSheet.png`
- `Assets/DaMoguArtPreview/Docs/DaMoguBlenderSkinnedMesh_UnityWalkCycle.png`

Unity 日志仍确认 `Avatar isHuman=True`、`isValid=True`。当前主要美术问题仍在源网格：hoodie 纸样偏硬、skort/腿部穿插、包带和发束需要真正权重，Unity 导入层只能做临时显示修复。

## 2026-09-01 Blender 源网格衣服体积修正

本轮回到 `Tools/Blender/build_damogu_blender_skinned_mesh.py`，不再把 Modly 静态网格当成直接可动角色，也不继续依赖 Unity 临时大面片。

已重新导出：

- `DaMogu_Blender_SkinnedMeshPrototype.blend`
- `DaMogu_Blender_SkinnedMeshPrototype.fbx`
- `DaMogu_Blender_SkinnedMeshPrototype.glb`

本轮源网格变化：

- hoodie shell 从 12x27 加密到 17x33，并调整前后深度、侧边圆角和下摆收束，减少硬纸板感。
- 新增左右侧面深度面片、白色腋下过渡面、侧缝 piping、前袋弧形受力褶和口袋阴影，目标是让衣服读成有厚度的布料，而不是一张贴图。
- 收窄下摆罗纹体积，避免侧面变成过厚圆桶。
- 重做 hood 背部采样，增加粉色内衬边，减轻背后“大白块”感。
- 导入 VRM 后裁掉被 hoodie/skort 遮住的原身体髋部和上腿面，解决背面裙下肤色块外露。
- `add_ellipsoid_mesh` 改为单顶点极点拓扑，Unity 导入时不再刷出大量自交面丢弃警告。

已验证：

- Blender 5.2.1 LTS batchmode 成功渲染正面、背面、侧面预览。
- Unity batchmode 导入新版 FBX，日志确认 `Avatar isHuman=True`、`isValid=True`。
- 已重跑 `DaMoguBlenderSkinnedMesh_UnityPolishSheet.png` 和 `DaMoguBlenderSkinnedMesh_UnityWalkCycle.png`，走路采样没有炸形。

当前仍未达到目标三视图。主要差距是 hoodie 正面仍偏硬壳、袖子仍有圆筒感、裙褶还不够像参考图、发型还是提示级结构。下一步最应该在 Blender 中继续收衣服大形：降低 hoodie 正面硬边，重做袖孔和袖口下垂，再把程序化小细条合并成更干净的真实缝线/压线网格。

## 2026-09-01 上胸遮挡与配件体积修正

本轮继续按“Modly 当静态参考、Blender 做可动真网格、Unity 验证 Humanoid”的路线推进，没有把不稳定候选导入主游戏工程。

已重新导出：

- `DaMogu_Blender_SkinnedMeshPrototype.blend`
- `DaMogu_Blender_SkinnedMeshPrototype.fbx`
- `DaMogu_Blender_SkinnedMeshPrototype.glb`

本轮源网格变化：

- 扩大 `mask_imported_body_under_custom_clothes()` 的上胸遮挡范围，导出日志中被衣服覆盖而隐藏的原 `Body` 面从 1962 增加到 3076，修掉 hoodie 正面胸部体块顶出来的问题。
- hoodie shell 继续加密到 19x37，并把上身前方深度外移，减少原身体和外套互相穿插。
- 袖子改为 10 圈、24 段的软截面，加入腋下下垂、袖身压缩褶和纵向软凹槽，目标是降低圆筒袖感。
- 斜挎包从圆球感改成圆角矩形软包，并加入顶端拉链脊、前袋口和浅浮雕口袋。
- 球鞋增加白色厚底、粉色鞋头和鞋带提示，让第三人称背影里脚部更接近参考图。
- 测试过额外前发条和手指覆盖片，但预览中会读成硬杆或多余手指，已撤回，暂时保留原手网格和现有发型结构。

Unity 侧验证：

- Unity 导入器曾测试 `Standard` 不透明材质回退，但整体过暗；最终仍优先使用 `VRM/MToon`，只保留 `Standard` fallback 的强制不透明设置。
- `DaMoguBlenderSkinnedMesh_UnityPolishSheet.png` 和 `DaMoguBlenderSkinnedMesh_UnityWalkCycle.png` 已重跑。
- Unity batchmode 日志确认 `Avatar isHuman=True`、`isValid=True`，走路采样没有炸形。

当前差距：

- hoodie 正面仍有硬壳和碎条感，下一轮应优先重做成更干净的连续衣片，而不是继续叠小线条。
- 袖孔、肩部和袖口还需要按动作姿态手工调权重。
- 发型和 skort 仍是原型级结构，距离目标图的半扎编发、蓬松短发和规整裙褶还有明显差距。

## 2026-09-01 连续衣片与旧 Unity 补丁清理

本轮继续只在 `大蘑菇角色美术预览_Unity工程` 中处理角色模型本体，主游戏工程未导入候选资产。

已重新导出：

- `DaMogu_Blender_SkinnedMeshPrototype.blend`
- `DaMogu_Blender_SkinnedMeshPrototype.fbx`
- `DaMogu_Blender_SkinnedMeshPrototype.glb`

本轮源网格变化：

- hoodie shell 加密到 21x41，并新增 `Soft hoodie cloth thickness` solidify 厚度，让主衣片更像独立外套网格。
- 粉白分界线从 18 段独立小条改成连续管线，减少 Unity 截图里的碎片感。
- 收窄衣身正面深度、侧边圆角和下摆宽度，减轻侧面“硬桶壳”观感。
- 口袋从大片覆盖面改为更克制的口袋开口线和阴影线，避免正面读成贴片。
- 撤掉早期肩侧白色补块和肩身过渡 patch，减少 T-pose 与三视图里的方块感。

Unity 侧变化：

- `DaMoguBlenderSkinnedMeshUnityImporter.cs` 新增 `UseLegacyBoneDrivenPolish = false`，默认关闭早期 Unity 骨骼跟随补强层。
- 当前三视图和走路图主要检查 Blender FBX 本体，不再叠加 Unity 临时肩块、袖子球体、额外包体和袖口。
- Unity 预览材质继续保持不透明 MToon，并略微调整 hoodie 粉白材质的阴影和布纹对比。

已验证：

- Blender 5.2.1 LTS batchmode 正常渲染正面、背面、侧面预览，并导出 `.blend/.fbx/.glb`。
- Unity 2022.3.62f3c1 batchmode 重跑 `DaMoguBlenderSkinnedMesh_UnityPolishSheet.png` 和 `DaMoguBlenderSkinnedMesh_UnityWalkCycle.png`。
- Unity 日志确认 `Avatar isHuman=True`、`isValid=True`，没有 C# 编译错误，走路采样没有炸形。

当前结论：

- 这版比上一轮更接近“可动真实 FBX 本体”，因为旧 Unity 补丁不再遮挡判断。
- hoodie 大形仍明显偏程序化，侧面厚度和前胸硬壳感还没达到目标图；下一步应继续重建 hoodie 纸样和袖孔，而不是恢复 Unity 临时补丁。

## 2026-09-01 肩颈、袖子遮罩与 hood 收形回合

本轮继续只在独立角色预览工程中处理 `DaMogu_Blender_SkinnedMeshPrototype`，主游戏工程未导入候选资产。

源网格变化：

- hoodie 主体仍保持 21x41，但把上沿从“抬高的直肩线”改成向袖孔下坠的肩坡；前后深度重新分配，前片从胸口到下摆有轻微弧度，减少侧面平板感。
- 新增 `DaMogu_Blender_Soft_White_Upper_Chest_Lining`，用真正 skinned mesh 遮住前胸开口处的灰色基础身体边缘。
- 扩大 `mask_imported_body_under_custom_clothes()`：新增 `arm_under_sleeve`，导出时被衣服遮住的原 `Body` 面从 3654 增至 5340，减少 Unity 动画姿态里的裸手臂穿帮。
- 袖子根部权重更偏向 upper arm，腕部权重更偏向 hand/lower arm，降低走路采样时袖筒滞在胸口或手腕断开的风险。
- hood 从 7x25 改为 9x29，整体压低、收窄，并加入 `Soft hood cloth thickness`，让背后帽子不再像一整块大白板。
- 后领 `Back_Hood_Lower_Collar_Rim` 上移并加粗一点，作为折中方案压住后颈断口。

已验证：

- Blender 5.2.1 LTS batchmode 成功刷新 `.blend/.fbx/.glb`，正面/背面/侧面预览图也已输出到 `/Users/zhendian/Desktop/大蘑菇角色美术预览输出/`。
- Unity 2022.3.62f3c1 batchmode 已重跑 `DaMoguBlenderSkinnedMesh_UnityPolishSheet.png` 和 `DaMoguBlenderSkinnedMesh_UnityWalkCycle.png`。
- Unity 日志确认 `Avatar isHuman=True`、`isValid=True`，无 C# 编译错误，走路周期没有炸骨骼。

当前结论：

- 袖子覆盖和可动性比旧版稳，三视图也不再依赖 Unity 临时补丁。
- 仍未达到目标图：帽衫领口现在偏露肩，后领和袖孔还不像真正运动外套；下一轮最该在 Blender 里重做肩颈拓扑和袖孔纸样，而不是继续微调贴图颜色。

## 2026-09-01 肩片权重、领口内衬与 Modly 路线复核

本轮继续沿“Modly 作为静态造型参考，Blender 产出可动 skinned mesh，Unity Humanoid 验证”的路线推进，仍只修改 `大蘑菇角色美术预览_Unity工程`。

资料结论：

- Unity Humanoid Avatar 需要骨架映射和有效 Avatar，Unity 导入端必须看到 `isHuman=True`、`isValid=True` 才能安全继续走第三人称动画。
- UniVRM、VRM Add-on for Blender、VRoid custom item 路线都指向同一个核心：衣服要像衣服，需要独立网格、厚度、边缘、权重和动作姿态验证。
- Modly/Hunyuan3D 当前产物仍更适合作静态外形探索；本地两版候选 GLB 没有 skin、animation、Animator 或 Avatar，不能直接变成自然走路的人物。

本轮 Blender 修改：

- `mask_imported_body_under_custom_clothes()` 进一步覆盖肩背、领口和袖根下方区域，导出日志中被隐藏的原 `Body` 面为 6001。
- 新增 `hoodie_upper_shell_weights()`，让 hoodie 顶部外侧顶点混入 upper arm 权重，避免 Unity 手臂放下后肩部布料停在 T-pose。
- hoodie shell 从 21x41 调整为 23x45，并重做上缘宽度、肩坡、前后深度和实体厚度。
- 新增 `Back_White_Shoulder_Yoke`、左右 `Wrapped_White_Shoulder_Cap`，并把外侧肩片权重提高到 upper arm，减少肩片变成水平硬翼的问题。
- 新增 `Pink_Inner_Neck_Cushion` 和 `Back_Inner_Neck_Cushion`，让领口更像有内衬厚边的 hood，而不是贴在皮肤上的线条。
- 袖子根部略外移，袖身半径和腕口 cuff 重新调过，保证肩片和袖筒衔接更稳。

已验证：

- Blender 5.2.1 LTS batchmode 正常刷新 `.blend/.fbx/.glb` 和正面/背面/侧面预览。
- Unity 2022.3.62f3c1 batchmode 重跑 `DaMoguBlenderSkinnedMesh_UnityPolishSheet.png` 与 `DaMoguBlenderSkinnedMesh_UnityWalkCycle.png`。
- Unity 日志确认 `Avatar isHuman=True`、`isValid=True`，没有 C# 编译错误，走路周期未炸形。
- Unity Hub 记录中 `大蘑菇角色美术预览_Unity工程` 仍是独立预览项目，主游戏 `布布的小镇生活_Unity6_3_工程` 未导入本轮不稳定候选。

当前差距：

- 领口和后背 hood 还是偏程序化，未达到目标图那种自然布料堆叠。
- 肩片已经比硬翼阶段稳定，但袖孔纸样仍需要手工重拓扑。
- 下一步应该继续把 hoodie 上半身从多块补丁整理成更连续的衣服网格，再推进发型和手部。

## 2026-09-01 连续前片、后颈桥接与发尾修正

本轮继续只修改独立角色预览工程，主游戏工程未导入当前候选。

Blender 修改：

- 新增 `DaMogu_Blender_L_Continuous_White_Front_Lapel` 和 `DaMogu_Blender_R_Continuous_White_Front_Lapel`，把前胸白色区域从零散边线改为左右连续布片。
- 新增 `DaMogu_Blender_Back_Continuous_Hood_Collar_Bridge` 和 `Back_Collar_Bridge_Lower_Fold`，补后颈 hood/衣领过渡，减少背面脖子和衣服之间的断层。
- 保留上一轮 `hoodie_upper_shell_weights()`，继续让外侧肩片混入 upper arm 权重，避免 Unity idle 姿态下肩布停在 T-pose。
- 将导入 VRM 和新增发束的发色压深到目标图更接近的巧克力棕。
- 测试过较长的后颈发束；Unity 三视图中会读成硬竖条，已缩短为 5 条较短的 `Back_Nape_Hair_Lock`，只作为发尾层次提示。

已验证：

- Blender 5.2.1 LTS batchmode 成功导出 `.blend/.fbx/.glb`，并刷新正面/背面/侧面预览。
- Unity 2022.3.62f3c1 batchmode 重跑三视图和走路图。
- 日志确认 `Avatar isHuman=True`、`isValid=True`，没有 C# 编译错误，也没有新增自交丢面警告。

当前结论：

- 这一版比上一轮更像“可动 FBX 本体上有真实衣片和发尾层次”，不是只靠贴图或 Unity 临时补丁。
- hoodie 领口/袖孔仍然明显程序化，下一步要继续把肩颈和袖孔做成真正连续拓扑，减少外露碎边和补丁感。

## 2026-09-01 清理披肩感与可动性复核

本轮继续只在独立角色美术预览工程里操作，主游戏工程未导入当前候选。

本轮尝试和取舍：

- 试加过一层 `Continuous_White_Hoodie_Upper_Body` 和左右袖孔过渡片，目标是让白色肩胸区域更连续。
- Unity 三视图和走路图验证后发现，这类分离肩片在 idle/walk 姿态里会读成外翻披肩，虽然 Avatar 仍然有效，但美术方向不对。
- 已暂停调用会造成披肩感的独立连续上片、后肩 yoke 和 sleeve-hole gusset，只保留更克制的前胸连续 lapel、后颈 hood/collar bridge、领口内衬和主 hoodie shell。
- `hoodie_upper_shell_weights()` 中上臂权重降到更保守范围，让主衣身肩部主要跟 upper chest，避免衣身被手臂动画拉飞；袖筒本体仍跟随 upper/lower arm。
- Unity 预览材质把 hoodie 粉色整体调淡，避免衣服像高饱和色块；skort modesty panels 已接入，裙裤内层比前一版完整。

已验证：

- Blender 5.2.1 LTS batchmode 成功刷新 `.blend/.fbx/.glb`，并更新桌面统一输出文件夹里的正面/背面/侧面真实网格预览。
- Unity 2022.3.62f3c1 batchmode 重跑 `DaMoguBlenderSkinnedMesh_UnityPolishSheet.png` 与 `DaMoguBlenderSkinnedMesh_UnityWalkCycle.png`。
- Unity 日志确认 `Avatar isHuman=True`、`isValid=True`，没有 C# 编译错误，走路周期没有炸形。

当前判断：

- 这一版比“堆肩片”的中间尝试更干净，也继续保持可动；但仍没有达到目标图的衣服质感。
- 下一步不要继续叠独立补片，应该在 Blender 中重做真正连续的 hoodie 肩颈/袖孔拓扑，把袖筒和衣身做成更可信的一组 skinned mesh。

## 2026-09-01 人物本体四十次强化：撤掉硬肩片并收窄内衫块

本轮继续只处理 `大蘑菇角色美术预览_Unity工程`，主游戏 `布布的小镇生活_Unity6_3_工程` 未导入当前候选。

资料复核：

- Unity Humanoid 路线要求模型有可映射的人形骨架和有效 Avatar，导入后仍以 `isHuman=True`、`isValid=True` 作为继续推进的底线。
- UniVRM、VRM Add-on for Blender、VRoid custom item 的共同思路是：角色可以从 VRM/VRoid/AI 静态稿出发，但可动服装必须回到 Blender/VRM 的真实网格、蒙皮权重、材质和动作验证。
- Modly/Hunyuan3D 类图生 3D 更适合作静态造型参考；当前本地候选没有 skin/animation/Animator/Avatar，不直接作为最终游戏角色。

Blender 修改：

- `hoodie_shell` 顶部加宽，让主 hoodie 网格自己覆盖肩部，而不是依赖独立硬肩片遮挡。
- 暂停调用 `create_hoodie_raglan_shoulder_caps()`，避免 Unity idle/walk 中外侧肩片读成硬披肩。
- `create_hoodie_front_lapel_panels()` 撤掉左右大面片，只保留较细的衣襟折线。
- `create_hoodie_upper_chest_lining()` 从白色胸前大补片改为小型领口阴影线，减少“胸前硬壳/内衫贴片”观感。
- `create_neck_and_inner_shirt_fill()` 保留颈部遮挡和细领口阴影，去掉大块内衫面片；当前目标图是闭合拉链 hoodie，不需要大面积露内衫。
- 新增 `create_hoodie_soft_upper_chest_contours()`，补白粉分界厚度、细微胸前布褶和凹线。
- 袖口 cuff 加宽加厚，弱化手腕和袖筒之间的突兀断层。

已验证：

- Blender 5.2.1 LTS batchmode 成功导出 `.blend/.fbx/.glb`，并刷新桌面统一输出文件夹里的正面、背面、侧面预览。
- Unity 2022.3.62f3c1 batchmode 重跑三视图和走路图；日志确认 `Avatar isHuman=True`、`isValid=True`，没有 C# 编译错误，走路周期没有炸形。

当前判断：

- 这一版比上一版干净，胸前灰白硬块明显收掉，肩部也不再依赖大块独立肩片。
- 但离目标图仍有明显差距：hoodie 肩颈/袖孔仍偏程序化，手部还偏细，发型还只是基础发束补强。
- 下一步最该做真正的 hoodie 连续拓扑/重拓扑：衣身、袖孔、帽领、下摆用一套干净网格连接，再转移/手调权重，而不是继续叠小补片。

## 2026-09-01 人物本体四十一次强化：肩顶包覆与口袋浮雕

本轮继续只处理独立角色预览工程，主游戏 `布布的小镇生活_Unity6_3_工程` 未导入当前候选。Unity Hub 已复核：角色预览工程仍为 Unity `2022.3.62f3c1`，主游戏仍为 Unity `6000.3.23f1`。

资料复核：

- Unity Humanoid Avatar、UniVRM、VRM Add-on for Blender、VRoid custom item 的路线都指向同一个结论：角色可以用 AI/Modly/VRoid 做外观起点，但游戏内自由走跑需要骨架、蒙皮权重、有效 Avatar/VRM、材质和动作验证。
- Modly/Hunyuan3D 类图生 3D 更适合作为静态外形参考；当前本地生成的 GLB 没有 skin、animation、Animator 或 Avatar，不能直接替代可动角色。
- 参考入口：Unity Humanoid Avatar 文档、UniVRM GitHub、VRM Add-on for Blender GitHub、VRoid custom item 文档、Modly 文档。

Blender 修改：

- 新增 `create_hoodie_shoulder_top_wrap()`，给左右肩顶加更克制的白色包覆面、软肩缝和袖孔 piping，用主 hoodie 顶部加宽方案替代上一轮容易变成披肩的独立硬肩片。
- 新增 `create_hoodie_front_pocket_patches()`，把口袋从两条线提升为左右浅浮雕口袋布片，并补外圈缝线和内侧软袋口。
- 在拉链两侧新增细的 `Zipper_Cloth_Edge`，让前中线不只是一排齿，而有衣襟厚度。
- 袖口 cuff 保持加宽加厚，避免手腕和袖筒之间的断层；继续不叠额外手指补片，避免再次出现怪手。
- 保持大块内衫面片和大块 lapel 面片撤销状态，只保留小领口阴影、白粉分界厚度和胸前细布褶。

已验证：

- Blender 5.2.1 LTS batchmode 成功刷新 `.blend/.fbx/.glb`，并更新桌面统一输出文件夹里的正面、背面、侧面真实网格预览。
- Unity 2022.3.62f3c1 batchmode 重跑 `DaMoguBlenderSkinnedMesh_UnityPolishSheet.png` 与 `DaMoguBlenderSkinnedMesh_UnityWalkCycle.png`。
- Unity 日志确认 `Avatar isHuman=True`、`isValid=True`，没有 C# 编译错误，walk cycle 可以跑。

当前判断：

- 这一版衣服比上一版多了口袋体积、拉链边和肩顶包覆，已经不是“方块/贴图/静态 GLB”路线，而是可以随 Humanoid 动作运行的 skinned mesh 基线。
- 但它仍未达到目标三视图：hoodie 肩颈、袖孔、帽领和侧面衣服体积仍然偏程序化，手部比例和头发层次也还需要继续修。
- 下一步最该做真正的 hoodie 连续拓扑/retopo：把衣身、袖筒、帽领、口袋、下摆做成更完整的一套衣服网格，再手调权重和材质，而不是继续无限叠补丁。

## 2026-09-01 人物本体四十二次强化：环绕式 hoodie shell 与圆角软包

本轮继续只在独立角色预览工程中推进人物美术，主游戏工程未导入当前候选。

Blender 修改：

- 将 `create_hoodie_shell()` 从“前后两张衣片加侧边面”的结构，改为 72 段环绕身体的连续成衣网格。
- 新 shell 使用一圈闭合截面表达前胸、侧面和后背，白粉分界仍按高度和左右弧度分材质；下摆两行继续使用 cuff 材质。
- 调整前后深度、侧面缩进、胸前布料鼓起、肩线下坠和领口下切，让侧面不再那么像一块直板。
- 新增 `create_rounded_satchel_mesh()`，把斜挎包主体从 8 顶点软化方盒改为 36 段 superellipse 圆角软包；保留包面拉链、前袋和斜挎带。

已验证：

- `build_damogu_blender_skinned_mesh.py` 通过 compile 语法检查。
- Blender 5.2.1 LTS batchmode 成功刷新 `.blend/.fbx/.glb`，并更新桌面统一输出文件夹里的正面、背面、侧面真实网格预览。
- Unity 2022.3.62f3c1 batchmode 重跑三视图和走路图。
- Unity 日志确认 `Avatar isHuman=True`、`isValid=True`，没有 C# 编译错误，walk cycle 继续稳定。

当前判断：

- 这一轮比上一版更接近“衣服本身是一圈包住身体的 mesh”，不再只是前后平面叠线条；斜挎包也从硬方块往目标图的软圆角包靠近。
- 仍然没有达到目标图：肩颈、帽领和袖孔还残留硬边，袖子与衣身还没有真正拓扑连接，手部和发型仍需后续强化。
- 下一步应继续沿真 retopo/custom item 方向，把 hoodie 上半身、袖孔、帽领和袖筒做成一套干净连续的 skinned mesh，并减少当前分离肩领装饰块。

## 2026-09-01 人物本体四十三次强化：去硬肩片与软化帽领

本轮继续只在独立角色预览工程中推进人物本体，主游戏工程未导入当前候选。

Blender 修改：

- 重写 `create_hoodie_shoulder_top_wrap()` 的实际输出，取消上一轮大面积 `Continuous_Shoulder_Top_Wrap` 肩顶面片导出。
- 新增左右 `Soft_Front_Setin_Sleeve_Blend` 和 `Soft_Back_Setin_Sleeve_Blend`，用更小的袖孔过渡布片替代硬披肩式白块。
- 将 `Soft_Shoulder_Top_Seam` 和 `Soft_Armhole_Setin_Piping` 收窄、压低，减少侧面和背面突出的硬边。
- 将 `create_hoodie_back_neck_bridge()` 的后领桥接改为更窄的 3x17 网格，厚度和 bevel 都降低。
- 收细并压低前后 hood collar rim、粉色内领 cushion 和肩部 set-in seam，避免帽领像几根粗管叠在脖子周围。

已验证：

- `build_damogu_blender_skinned_mesh.py` 通过 compile 语法检查。
- Blender 5.2.1 LTS batchmode 成功导出 `.blend/.fbx/.glb`，并刷新正面、背面、侧面源网格预览。
- Unity 2022.3.62f3c1 batchmode 重跑三视图和走路图。
- Unity 日志确认 `Avatar isHuman=True`、`isValid=True`，没有 C# 编译错误，walk cycle 继续稳定。

当前判断：

- 这一轮是一次“减法式”提升：肩顶不再读成大块硬披肩，帽领和袖孔比上一版更克制，整体更接近可以继续 retopo 的可动角色基线。
- 仍未达到目标图：T-pose 下袖筒和衣身仍是分离结构，Unity 侧面仍能看出袖孔/帽领硬边，头发与手部也还没有到目标精度。
- 下一步应继续把袖筒根部和 hoodie 上身合并成真正的一体化拓扑，同时弱化分离装饰线，逐步从程序生成形态转向可手工精修的 custom item。

## 2026-09-01 人物本体四十四次强化：袖根桥接网格

本轮继续只在独立角色预览工程中推进人物本体，主游戏工程未导入当前候选。

Blender 修改：

- 新增 `create_hoodie_sleeve_root_bridges()`，为左右袖根生成 `Integrated_Hoodie_Sleeve_Root_Bridge`。
- 新桥接网格沿前侧、肩顶、后侧做 15x7 的薄面片，从 hoodie 侧边过渡到袖筒根部。
- 桥接权重从 upper chest/chest/neck 平滑混到对应 upper arm，目标是在 Unity idle/walk 下同时跟随衣身和袖子。
- 新增 `Integrated_Sleeve_Root_Soft_Seam` 作为很细的袖根缝线，不再使用大面积硬肩片遮盖。

已验证：

- `build_damogu_blender_skinned_mesh.py` 通过 compile 语法检查。
- Blender 5.2.1 LTS batchmode 成功导出 `.blend/.fbx/.glb`，并刷新正面、背面、侧面源网格预览。
- Unity 2022.3.62f3c1 batchmode 重跑三视图和走路图。
- Unity 日志确认 `Avatar isHuman=True`、`isValid=True`，没有 C# 编译错误，walk cycle 继续稳定。

当前判断：

- 这一轮是结构性小进步：袖根不再只是袖筒和衣身靠近摆放，而是多了一层可动桥接网格。
- 视觉提升比较克制，优点是没有把肩顶重新做成硬披肩；缺点是袖筒与衣身仍不是同一个连续 mesh。
- 下一步应继续把桥接网格扩展成真正的一体化 hoodie/sleeve 拓扑，并继续减少前胸和侧面的碎线条感。

## 2026-09-01 人物本体四十五次强化：软化布料细线与收窄衣身

本轮继续只在独立角色预览工程中推进人物本体，主游戏工程未导入当前候选。

资料复核：

- 继续对照 Unity Humanoid Avatar、UniVRM、VRM Add-on for Blender、VRoid custom item、Blender 权重/数据传递相关资料，以及 B 站常见角色衣服绑定教程的路线。
- 结论保持一致：Modly/AI 3D 可以当外观草稿或基础体块，但要在 Unity 中自由走跑，仍必须做独立衣服网格、骨骼、skin weights、Avatar/VRM 和动作检查。

Blender 修改：

- 新增衣服细节收敛规则，让 hoodie/袖子/拉链/口袋/领口的管状线和凸条更贴近衣身，避免侧面读成漂浮硬杆。
- 收窄 `create_sleeve()` 的袖筒半径和袖口尺寸，降低 sleeve bevel；袖根权重更多混到 upper chest，动作时不会只像硬管跟随上臂。
- 收窄 `create_hoodie_shell()` 的前后深度和整体衣身宽度，降低 shell 厚度与 bevel，让侧面不再像厚方盒。
- 暂停调用下摆碎褶和背部竖向碎线，只保留粉白分界、口袋、拉链、帽领、包带等关键结构。
- 同步调淡 Blender 和 Unity 预览里的 hoodie 粉色、袖褶、缝线、拉链材质，降低“贴上线条”的视觉噪声。

已验证：

- `build_damogu_blender_skinned_mesh.py` 通过 compile 语法检查。
- Blender 5.2.1 LTS batchmode 成功刷新 `.blend/.fbx/.glb`，并更新桌面统一输出文件夹中的正面、背面、侧面真实网格图。
- Unity 2022.3.62f3c1 batchmode 重跑三视图和走路图。
- Unity 日志确认 `Avatar isHuman=True`、`isValid=True`，没有 C# 编译错误，walk cycle 继续稳定。

当前判断：

- 这一版比四十四次更干净：背部和下摆碎线减少，袖子更收敛，衣身侧面厚度下降，可以作为继续打磨的可动基线。
- 仍未达到目标图：肩颈、袖孔、帽领和正面胸口仍偏程序化硬壳，头发、脸部精度和手部比例也还需要继续做。
- 下一步最该进入真正 hoodie retopo/custom item：把衣身、袖孔、袖筒、帽领、口袋、下摆做成一套连续 skinned mesh，再用 Blender 权重转移和手修权重来提高动作里的自然度。

## 2026-09-01 人物本体四十六次强化：统一 hoodie 与袖子可动网格

本轮继续只在独立角色预览工程中推进人物模型本体，主游戏工程未导入当前候选。

工程与资料复核：

- Unity Hub 中主游戏仍是 `布布的小镇生活_Unity6_3_工程`，Unity `6000.3.23f1`；角色预览仍是 `大蘑菇角色美术预览_Unity工程`，Unity `2022.3.62f3c1`。
- 继续沿 Modly/AI 外形参考 + Blender skinned mesh + Unity Humanoid 验证路线推进；静态 GLB 仍不能替代骨架、skin weights、Avatar/VRM 和 Animator 验证。

Blender 修改：

- 新增 `create_unified_hoodie_body_and_sleeves()`，把 hoodie 衣身和左右袖筒放进同一个可蒙皮 mesh，替代上一轮分离的 shell/sleeve 主体。
- 取消旧的 `create_hoodie_shell()` 和左右 `create_sleeve()` 主调用，保留口袋、拉链、下摆、包带、鞋子、头发等目标图关键细节。
- 袖根补洞从重复双面三角面改成有效的内环 + 中心盖面，修掉 Blender glTF 导出的 `Mesh ... is not valid` 风险。
- 尝试接入旧 shoulder yoke/raglan 后发现正面会变成硬方块，已撤掉；改用 `create_hoodie_continuous_upper_panels()` 与 `create_hoodie_armhole_transitions()` 做上胸和袖孔过渡。
- 同步调淡 Unity/Blender hoodie 粉色、袖褶、拉链和口袋材质，减少高饱和硬线条感。

已验证：

- `build_damogu_blender_skinned_mesh.py` 通过 compile 语法检查。
- Blender 5.2.1 LTS batchmode 成功刷新 `.blend/.fbx/.glb`，没有再出现 mesh invalid 警告，并更新桌面统一输出文件夹中的正面、背面、侧面真实网格图。
- Unity 2022.3.62f3c1 batchmode 成功重跑三视图和 walk cycle。
- Unity 日志确认 `Avatar isHuman=True`、`isValid=True`，没有 C# 编译错误；walk cycle 能继续采样播放。

最新输出：

- `/Users/zhendian/Desktop/大蘑菇角色美术预览输出/大蘑菇_Blender真实网格_正面预览.png`
- `/Users/zhendian/Desktop/大蘑菇角色美术预览输出/大蘑菇_Blender真实网格_侧面预览.png`
- `/Users/zhendian/Desktop/大蘑菇角色美术预览输出/大蘑菇_BlenderFBX_Unity三视图补强检查.png`
- `/Users/zhendian/Desktop/大蘑菇角色美术预览输出/大蘑菇_BlenderFBX_Unity走路检查图.png`

当前判断：

- 这一轮把主衣身与袖子推进到了“同一 skinned mesh 基线”，比纯贴图、方块占位或静态 Modly GLB 更接近可动角色资产。
- Unity 中角色可以被 Humanoid/Animator 驱动，但美术仍没到目标三视图：上胸白色区域偏硬，袖孔和帽领仍有程序化边，手、头发和材质质感还要继续精修。
- 下一步应继续在 Blender 里做真正 hoodie retopo/custom item，把肩颈、袖孔、帽领和下摆从补丁式面片收敛成干净连续拓扑，再手修权重。

## 2026-09-01 人物本体四十七次强化：软领口、双面布料与 Modly 候选评估

本轮继续只在独立角色预览工程中推进人物模型本体，主游戏工程未导入当前候选。

Blender 修改：

- 将 `create_hoodie_continuous_upper_panels()` 的上胸白色布片从完整封闭面改为带前领口开口的薄布片，降低厚度和 bevel，减少 Unity 中像硬披肩/硬纸板的观感。
- 收细 `create_hoodie_armhole_transitions()` 的袖孔过渡片和袖孔 piping，降低补片厚度，让侧面袖根少一些塑料圈感。
- 保持 `create_unified_hoodie_body_and_sleeves()` 的同一 skinned mesh 主体，继续让衣身和袖子共用一套可动画网格基线。

Unity 修改：

- 在 `DaMoguBlenderSkinnedMeshUnityImporter.cs` 中新增双面预览材质判断。
- 对 hoodie、skort、bag、strap、cloth ridge、sleeve crease、stitching、zipper 和 hair 使用双面显示；皮肤仍保持普通不透明显示。
- 这比复制反面网格更稳，不会重新引入 mesh invalid 风险。

Modly 候选评估：

- 重新渲染了本地 `DaMogu_ModlyHunyuan3D_FrontDetailBalanced.glb` 与 `DaMogu_ModlyHunyuan3D_FirstPass.glb`。
- `FrontDetailBalanced` 的 hoodie 袖子、下摆、帽绳和斜挎包整体体块更自然，可以作为后续 Blender retopo 参考。
- 两个 Modly 候选仍是静态 `trimesh` GLB，无 Humanoid 骨骼、skin weights、Avatar 或 animation；不能直接替代当前可动角色。
- 本地 `modly_runtime` 未找到 `model.fp16.safetensors`，因此当前不能离线继续生成新 Hunyuan3D/Modly 候选；若要再跑新候选，需要补齐模型权重。

已验证：

- `build_damogu_blender_skinned_mesh.py` 通过 compile 语法检查。
- Blender 5.2.1 LTS batchmode 成功刷新 `.blend/.fbx/.glb`，没有 mesh invalid 警告，并更新角色真实网格预览图。
- Unity 2022.3.62f3c1 batchmode 成功重跑三视图和 walk cycle。
- Unity 日志确认 `Avatar isHuman=True`、`isValid=True`，没有 C# 编译错误；walk cycle 继续可采样播放。

最新输出：

- `/Users/zhendian/Desktop/大蘑菇角色美术预览输出/大蘑菇_BlenderFBX_Unity三视图补强检查.png`
- `/Users/zhendian/Desktop/大蘑菇角色美术预览输出/大蘑菇_BlenderFBX_Unity走路检查图.png`
- `/Users/zhendian/Desktop/大蘑菇角色美术预览输出/大蘑菇_Modly候选_FrontDetailBalanced_预览.png`
- `/Users/zhendian/Desktop/大蘑菇角色美术预览输出/大蘑菇_Modly候选_FirstPass_预览.png`

当前判断：

- 这一轮把 Unity 预览中的薄布片破面风险降低了，也让上胸和袖孔比四十六次更薄、更软。
- Modly 候选证明“整体衣服体块”方向是对的，但它不能直接动；下一步应把 `FrontDetailBalanced` 的 hoodie 体块感手工迁移到当前 Blender skinned mesh，尤其是袖根、下摆和帽绳。
- 仍未达到目标三视图，后续需要继续真正 retopo/custom item，而不是只靠程序补片叠加。

## 2026-09-01 人物本体四十八次强化：Modly 软衣服体块迁移

本轮继续只在独立角色预览工程中推进人物模型本体，主游戏工程未导入当前候选。

Blender 修改：

- 新增 `create_hoodie_modly_soft_cloth_volume()`，把 Modly `FrontDetailBalanced` 候选里更自然的 hoodie 下摆卷边、口袋下弧线、侧身软折和下摆罗纹转成可动 skinned mesh 几何。
- 新增 `create_soft_ribbon_along_polyline()`，将前后斜挎包带从直硬片改成贴身曲线窄软带；包体略缩小，颜色调为浅米粉。
- 重新启用 `create_hand_overlays()`，扩大手掌、缩短并加粗手指、补指尖圆角，缓解动作图里手部过细的问题。
- 后脑小发髻去掉强对比深色中心，改为同发色小结和更细包裹发束，降低背面圆扣感。

已验证：

- `build_damogu_blender_skinned_mesh.py` 通过 compile 语法检查。
- Blender 5.2.1 LTS batchmode 成功刷新 `.blend/.fbx/.glb`。
- Unity 2022.3.62f3c1 batchmode 成功重跑三视图和 walk cycle。
- Unity 日志确认 `Avatar isHuman=True`、`isValid=True`，没有 C# 编译错误。

最新输出：

- `/Users/zhendian/Desktop/大蘑菇角色美术预览输出/大蘑菇_Blender真实网格_正面预览.png`
- `/Users/zhendian/Desktop/大蘑菇角色美术预览输出/大蘑菇_Blender真实网格_背面预览.png`
- `/Users/zhendian/Desktop/大蘑菇角色美术预览输出/大蘑菇_Blender真实网格_侧面预览.png`
- `/Users/zhendian/Desktop/大蘑菇角色美术预览输出/大蘑菇_BlenderFBX_Unity三视图补强检查.png`
- `/Users/zhendian/Desktop/大蘑菇角色美术预览输出/大蘑菇_BlenderFBX_Unity走路检查图.png`

当前判断：

- 这轮让衣服、包带和手部更像真实几何，且保持 Humanoid 可动验证通过。
- 肩颈白色区、帽领、袖孔和头发仍没到目标图精度；继续靠小补片收益会越来越低。
- 下一步应进入真正 hoodie custom item/retopo：把上胸、帽领、袖根和袖筒做成连续衣服拓扑，再手修权重。

## 2026-09-01 人物本体四十九次强化：拆袖筒清理自相交

四十八次的衣身与袖筒合并 mesh 在 Unity FBX 导入阶段仍有少量 self-intersecting polygons。本轮把它改成更接近真实 custom item 的分件结构：

- `DaMogu_Blender_Unified_Hoodie_Body_And_Sleeves` 拆为只负责主衣身的 `DaMogu_Blender_Continuous_Hoodie_Body_Shell`。
- 左右袖筒恢复独立 `DaMogu_Blender_L/R_Puffy_Sleeve`，保留上白下粉和袖口材质分段，降低 bevel 以减少 Unity 三角化风险。
- 袖孔视觉连接继续由薄 gusset、piping、肩线、白粉分界和 Modly 软衣服细节承担，避免一个大网格在肩腋位置互相穿插。
- 最新验证通过：Python compile、Blender 5.2.1 LTS 导出、Unity 2022.3.62f3c1 三视图和 walk cycle 均成功；日志里 `Avatar isHuman=True`、`isValid=True`，没有 self-intersecting、Failed、Exception 或 C# error。

当前这一版更适合继续做手工 retopo/weight paint 的基线。它仍没达到目标图，尤其是肩颈白色衣片、帽领体积、手部比例、半扎发束和整体材质精致度还需要下一轮继续细修。

## 2026-09-01 人物本体五十二次强化：模型先行与贴图化细节

本轮按“先做干净人物/衣服模型，再把细节像贴图一样贴上去”的方向调整，不再把所有缝线、拉链齿和浅褶皱都做成凸出的硬几何。

- 新增 `is_flat_garment_detail()`，把拉链布带、拉链齿、浅缝线、口袋线、浅褶皱、白粉分界阴影等转换成贴近衣服表面的薄片线。
- 衣服的真实体积仍保留在衣身、袖筒、帽子、下摆、袖口和包带上；这些不能纯贴图化，否则第三人称侧面和走路姿态会穿帮。
- 上胸白色区收窄抬高，新增软开领帽领片；外侧肩片和袖孔权重更多跟随 upper arm。
- 手掌略圆、手指缩短加粗，并补腕到掌心过渡。
- 验证通过：Python compile、Blender 5.2.1 LTS 导出、Unity 2022.3.62f3c1 三视图和 walk cycle 均成功；日志里 `Avatar isHuman=True`、`isValid=True`，没有 self-intersecting、Failed、Exception 或 C# error。

当前这版说明方向是对的：突兀小几何少了，Unity 可动链路也保住了。下一步应把这种薄片模拟替换成真正 UV/贴图/法线贴图，并继续把 hoodie 肩颈、帽领、袖孔做成干净 retopo/custom item。

## 2026-09-01 人物本体五十三次强化：手部圆润化与近景检查

根据“手还是太奇怪”的反馈，本轮先把手部从分指覆盖件改成更稳的第三人称小手：

- `mask_imported_body_under_custom_clothes()` 新增 `hand_under_soft_overlay`，隐藏原 VRM Body 里会露成长条/圈状的手部和手指面。
- `create_hand_overlays()` 撤掉四根管状手指和指尖球，只保留连续圆润的 `Soft_Palm_Overlay`、腕部过渡和短拇指提示。
- `DaMoguBlenderSkinnedMeshUnityImporter.cs` 新增 `CaptureBlenderSkinnedMeshUnityHandCheck()`，可以单独输出手部袖口近景。
- 已刷新 `DaMogu_Blender_SkinnedMeshPrototype.blend`、`.fbx`、`.glb`；FBX 当前约 22 MB。
- Unity 2022.3.62f3c1 验证通过：手部近景、三视图和 walk cycle 都成功输出，日志里 `Avatar isHuman=True`、`isValid=True`，没有 C# error、Failed 或 Exception。

当前取舍：手现在不再是怪手指，但也变成更卡通的圆手。它适合作为可动原型继续推进衣服/头发；真正接近目标三视图的手部，应在 Blender 中手工建完整五指拓扑并重刷权重。

## 2026-09-01 人物本体五十六次强化：低模五指手与掌心体积

本轮从“圆润小手”继续推进到“低模五指手”，仍只改本独立预览工程。

- 新增 `create_skinned_finger_forms()`：左右四指、拇指和指尖圆角都生成真实 mesh，并按 `J_Bip_L/R_Index*`、`Middle*`、`Ring*`、`Little*`、`Thumb*` 建 vertex group。
- `create_hand_overlays()` 新增 `Soft_Palm_Cushion`，让掌心有体积；四指收短加粗并和掌心搭接，避免上一版近景里只有几根细线。
- `DaMoguBlenderSkinnedMeshUnityImporter.cs` 的手部检查截图隐藏包和斜挎带，检查用手指放松强度降到 `0.08`。
- 已刷新 `DaMogu_Blender_SkinnedMeshPrototype.blend`、`.fbx`、`.glb`；Unity 手部近景、三视图和 walk cycle 均验证通过，`Avatar isHuman=True`、`isValid=True`。

当前取舍：手部已经能读出五指并保持可动，但仍是低模游戏原型。要达到目标参考图的手部精度，下一步应把手腕、掌心、指根做成一套连续拓扑，并手刷权重和 UV。

## 2026-09-01 人物本体五十八次强化：hoodie 贴身化、袖筒收细与手部近景复查

本轮继续根据参考图和 Unity 动作截图修人物本体，重点回应“衣服很臃肿、没有贴在身体的感觉”。仍只处理本独立预览工程。

- `build_damogu_blender_skinned_mesh.py` 收窄 `DaMogu_Blender_Continuous_Hoodie_Body_Shell`，降低前后厚度、侧面鼓包和下摆外扩，让 hoodie 更贴近身体曲线。
- 拉链、口袋、抽绳、下摆罗纹、白粉分界、浅褶皱等细节被移动到更贴近衣服表面的位置，减少悬浮补片感。
- 左右 `Puffy_Sleeve`、袖口、袖侧褶皱和长缝线同步收细收扁，保留宽松袖的外形但降低臃肿圆管感。
- 低模五指手继续细修：指长缩短、指根加厚、掌心和腕口补过渡桥接，并修掉小指指尖近景自相交风险。
- `DaMoguBlenderSkinnedMeshUnityImporter.cs` 的手部检查现在只显示手、袖筒、袖口和手腕相关 renderer，方便对手和袖口做单独视觉检查。
- 已刷新 `DaMogu_Blender_SkinnedMeshPrototype.blend`、`.fbx`、`.glb`；Unity 2022.3.62f3c1 手部近景、三视图和 walk cycle 均验证通过，日志里 `Avatar isHuman=True`、`isValid=True`，无 self-intersecting、C# error、Failed 或 Exception。

当前取舍：衣服比上一版更贴身、更像穿在身体上，手也更适合第三人称运动原型；但这仍不是目标图级别的成品服装。下一步应继续做真正 hoodie retopo/custom item，把帽领、肩颈白片、袖根和上胸做成连续拓扑，再补 UV、法线贴图和权重。

## 2026-09-02 人物本体五十九次强化：脸部位置复查与 hoodie 不透明内衬链路

根据“脸没有放对位置”和“网上查怎么让东西到正确位置”的反馈，本轮补查了 Blender/Unity 角色服装定位资料，并把工作流收敛为：先用原衣服表面作为 Shrinkwrap/投射参照，再沿最近表面转移权重，最后在 Unity 里确认 SkinnedMeshRenderer、Humanoid Avatar 和材质渲染模式。

- `DaMoguBlenderSkinnedMeshUnityImporter.cs` 将 custom hoodie 从 MToon 贴图材质切换为强制不透明的 `Unlit/Texture` 贴图材质，并在导入前把 custom hoodie PNG alpha 扁平化为 255，避免 hoodie 在头像检查里读成半透明罩层。
- 原 VRoid Tops 不再被重命名成透明隐藏层，而是保留为 hoodie 内衬/填充层，解决肩袖交界处因 custom shell 覆盖不完整而露出黑洞或肤色块的问题。
- 保留 `get_imported_tops_surface_data()` 的原衣服表面投射路线，后续口袋、袖子、缝线和 lower sleeve retopo 仍能投到正确的 hoodie 表面，并继承接近原衣服的权重。
- 本轮误试过“抽出 custom shell 后删除全部原 Tops 面”，Unity 三视图证明会在肩胸区域出现缺面黑洞，已撤回该调用，只保留函数作为后续手工 retopo 时可选工具。

已验证：

- `build_damogu_blender_skinned_mesh.py` 通过 compile 语法检查。
- Blender 5.2.1 LTS batchmode 成功刷新 `.blend/.fbx/.glb`。
- Unity 2022.3.62f3c1 batchmode 成功重跑导入预览、三视图、walk cycle、头像检查和手部检查。
- Unity 日志确认 `Avatar isHuman=True`、`isValid=True`，没有 C# 编译错误、FBX 导入失败或 mesh self-intersecting 报错。

当前判断：

- 脸部位置在 Blender 源图和 Unity 头像检查中均为头部中心位置；此前“脸/衣服不对”的主要视觉原因是 hoodie 材质半透明和内衬层处理不稳。
- 这版已经恢复可动链路并修掉透明/黑洞回归，但 hoodie 仍偏程序化，白粉分界、袖根、帽领和手部精度仍达不到目标图。
- 下一步继续朝真正 VRoid custom item/Blender skinned mesh 方向推进：把 hoodie 上胸、袖孔、帽领和袖筒合成更干净的连续拓扑，再做贴图/法线/手修权重。

## 2026-09-03 人物本体六十次强化：位置校准、Tops 剥离与材质分层

根据“自己去网上查怎么做到正确位置”的反馈，本轮继续参考 Blender Shrinkwrap/Data Transfer/Armature 和 Unity SkinnedMeshRenderer 的做法，把定位逻辑改成：原 VRoid Tops 只在 Blender 中作为表面投射和权重参照，Unity 最终预览里剥离原 Tops，只保留新的 custom hoodie shell、粉色覆盖片、缝线、口袋、袖口和手部低模网格。

- `create_extracted_vroid_hoodie_custom_shell()` 继续从原 Tops 抽出带骨骼权重的 hoodie 壳，但最终可见层改为白色基础壳加粉色贴合覆盖片，避免旧 Tops/UV/材质分区在胸口和袖子产生白色碎块或三角齿。
- `mask_imported_body_under_custom_clothes()` 只遮 `Body_00_SKIN`，不再误删 Tops 参照面，避免抽壳数量突然下降和肩胸区域黑洞。
- `DaMoguBlenderSkinnedMeshUnityImporter.cs` 恢复“检测到 `Custom_Hoodie_Shell` 就剥离 Body 里的 Tops 子网格”的 Unity 侧逻辑，解决原 Tops 与新 hoodie 壳叠穿造成的白块。
- 手部继续收敛：扩大原手遮罩到 2128 面，缩小掌心和四指，删除自交的 `Lowpoly_Thumb_Pad`，改成更短更圆的拇指管。
- 已刷新 `DaMogu_Blender_SkinnedMeshPrototype.blend`、`.fbx`、`.glb`，并输出新的头像、三视图、walk cycle 和手部袖口检查图。

已验证：

- `build_damogu_blender_skinned_mesh.py` 通过 compile 语法检查。
- Blender 5.2.1 LTS batchmode 导出成功；日志确认 `masked 2876 imported Body faces`、`masked 2128 imported hand/finger faces`、`extracted 1758 vertices and 2996 faces`、`projected 1130 hoodie detail control points`。
- Unity 2022.3.62f3c1 batchmode 导入和截图成功，`Avatar isHuman=True`、`isValid=True`，walk cycle、头像和手部检查图均生成。

当前判断：

- 脸的位置已经对齐，问题不是脸骨骼错位；之前看起来错位主要来自衣服透明层、原 Tops 叠穿和旧材质/UV 造成的视觉干扰。
- 白色乱块已通过 Unity 剥离 Tops 与白色基础壳/粉色覆盖片分层解决，衣服比上一版更像贴在身体上的可动网格。
- 手部近景比上一版少了长拇指和旧手块，但仍是第三人称低模原型，离目标图的完整五指拓扑还差一步。
- Unity 导入日志仍有 `DaMogu_Blender_Custom_Hoodie_Shell` 的 self-intersecting polygon 警告；下一轮应重点清理 hoodie shell 拓扑，减少被 Unity 丢面的风险，再继续做布料法线、袖根和手部权重。

## 2026-09-03 人物本体六十一次强化：正面衣片法线与原手回退

根据“脸没有放对位置”和“去网上查怎么做到正确位置”的反馈，本轮继续对照 Blender Shrinkwrap/Data Transfer/Armature、Unity SkinnedMeshRenderer 以及开源权重转移思路复核。最终确认：脸部坐标没有偏，主要问题来自衣服层叠、正面衣片法线方向、Unity 材质剔除和自制低模手替换。

- 暂停 `USE_EXTRACTED_HOODIE_CUSTOM_SHELL`，避免抽壳 hoodie 在 Unity 中产生白块和自交丢面；改为保留原 VRoid Tops 作为白色基础层、表面参照和权重参照。
- `trim_imported_vroid_hoodie_base_for_skort()` 保留并扩展到底摆与下袖，当前导出裁掉 1462 个会被粉色外层覆盖的 Tops 面，降低 z-fighting。
- 恢复四边面输出，撤回全局三角化；全局三角化曾让 Unity `self-intersecting` 从 156 增到 396，并在截图里出现黑色三角片。
- 正面粉色 retopo 衣片改为稳定手工曲面，不再逐点吸附到容易折叠的原 Tops 表面；同时翻转正面衣片 winding，让法线朝 Unity 正面相机，修掉正面蓝色漏底。
- 贴身粉色外层和下袖 retopo 不再额外加厚，改用薄外层布片 + 材质布纹表现，减少 Unity 导入丢面。
- 手部撤回自制低模替换：`USE_REPLACE_IMPORTED_HANDS_WITH_CUSTOM=False`、`USE_STYLIZED_HAND_OVERLAYS=False`，保留原 VRoid 手部拓扑和手指骨骼，只执行 `polish_imported_hand_shapes()` 的缩短与圆润处理。

已验证：

- `build_damogu_blender_skinned_mesh.py` 通过 Python compile 语法检查。
- Blender 5.2.1 LTS batchmode 成功刷新 `.blend/.fbx/.glb`。
- Unity 2022.3.62f3c1 batchmode 成功重跑三视图、walk cycle、头像和手部袖口检查。
- Unity 日志确认 `Avatar isHuman=True`、`isValid=True`，没有 C# error、Failed、Exception 或 mesh `self-intersecting`。

当前判断：

- 脸的位置是正确的；此前看起来“不在正确位置”，是衣服前片漏面、材质层和 z-fighting 把上半身视觉关系打乱。
- 保留原手比自制低模手更适合动画，近景能读出手指结构；但袖口切面仍硬，手指姿态还需要下一轮按动作继续调。
- hoodie 已经回到可动稳定基线，但美术距离目标图仍有明显差距：前片偏平、粉色下摆过深、帽领/袖根还不够自然，下一步应继续做真正连续 hoodie retopo、UV、法线贴图和手刷权重。

## 2026-09-03 人物本体六十二次强化：投射规则、权重稳定与浅粉 hoodie 修正

根据“自己去网上查怎么做到正确位置”的反馈，本轮复查了 Blender Shrinkwrap、Data Transfer、Armature Modifier、Unity SkinnedMeshRenderer 以及开源权重转移工具的工作流。结论是：衣服、手和脸不能靠截图贴片硬摆，必须以身体/原衣服表面和 Humanoid 骨架为基准，先定位再转权重，最后用 Unity 动作截图复核。

- 网格权重：`create_skinned_mesh()` 的 modifier 顺序改为先生成 bevel/weighted normal，再挂 Armature，避免 FBX 导出时 modifier 新顶点丢权重；贴身粉色 retopo 外片和下袖关闭额外 bevel，优先保证 Unity 可动稳定。
- 投射规则：新增 `should_project_hoodie_detail_control()`，上半身缝线继续参考原 Tops 表面；下摆、口袋、腹部褶皱不再投射到已裁掉的原 Tops 下缘，改为跟随新粉色前片坐标，减少“细节浮在前面”的错位。
- 轮廓修正：粉色前/后衣片继续收窄腰线、降低正面外凸，袖子末端改为手腕附近手工圆筒，避免被投射吸歪后在 Unity 中自相交。
- 材质修正：Blender 和 Unity 侧粉色 hoodie、口袋、袖口、布褶阴影同步调浅；Unity 的浅粉 hoodie 预览改用带程序布纹的 `Unlit/Texture`，保留布纹但不被 Standard 光照压成暗玫粉。
- 手部修正：继续使用原 VRoid 手部拓扑，只在 Blender 中缩短/圆润；Unity `RelaxedHandPoseBlend` 设为 `0.12`，只做很轻的手指放松，避免再次出现爪形。

已验证：

- `build_damogu_blender_skinned_mesh.py` 通过 Python compile 语法检查。
- Blender 5.2.1 LTS batchmode 成功刷新 `.blend/.fbx/.glb`；日志确认 `trimmed 1462 imported Tops faces`、`masked 2876 imported Body faces`、`projected 390 hoodie detail control points`。
- Unity 2022.3.62f3c1 batchmode 成功生成三视图、walk cycle、头像和手部袖口检查图。
- Unity 日志确认 `Avatar isHuman=True`、`isValid=True`，没有 C# error、Failed、Exception、mesh `self-intersecting` 或 `vertices with no weight`。

当前判断：

- 脸部在 Unity 头像检查里仍是居中且跟头骨稳定的；现在主要差距不是脸“没放对”，而是目标图级别的五官/刘海美术还没有完整重做。
- 衣服颜色和底层稳定性已改善，但 lower hoodie 还是偏程序化整块，侧面还没有参考图那种连续裁片和厚薄变化。
- 下一步不要再堆临时贴片，应该继续推进真正的 hoodie custom item/Blender skinned mesh：连续上胸-袖根-帽领拓扑、手工 UV、法线/布料纹理、以及对手腕和袖口的手刷权重。

## 2026-09-03 人物本体六十三次强化：衣摆位置对齐与手腕袖口收形

根据“自己去网上查怎么做到正确位置”的反馈，本轮继续按 Blender Shrinkwrap/Data Transfer/Surface Deform、Unity SkinnedMeshRenderer 和开源 weight-transfer 工作流校准：所有衣服附属线、口袋、拉链和袖口必须跟随同一套主衣片坐标与骨骼权重，否则 Unity 动起来后就会像贴片漂在身体外面。

- hoodie 主粉色 retopo 前片、后片和侧片整体改短：下摆从旧的 0.81-0.84 区间抬到 0.86-0.89 区间，宽度和前方 y 深度同步收窄，让 skirt/skort 露出更多，不再像粉色围裙盖住下半身。
- 旧的 `Hybrid_Short_Front_Hem_Roll`、`Hybrid_Short_Back_Hem_Roll` 和拉链底端同步移动到新下摆高度，避免历史补件还停在旧坐标造成“衣服细节不在正确位置”。
- 口袋、口袋缝线、下摆罗纹和腹部软褶整体上移并减淡，从低位大贴片改成更小的 hoodie 正面细节。
- 手部继续保留原 VRoid 可动手指拓扑，只把手腕和指尖缩放参数放缓；进口 hoodie wrist cuff 和目标下袖 cuff 收窄，减少袖口硬管压住手背的观感。

已验证：

- `build_damogu_blender_skinned_mesh.py` 通过 Python compile 语法检查。
- Blender 5.2.1 LTS batchmode 成功刷新 `.blend/.fbx/.glb` 和正面、背面、侧面预览。
- Unity 2022.3.62f3c1 batchmode 成功生成三视图、walk cycle、头像和手部袖口检查图。
- Unity 日志确认 `Avatar isHuman=True`、`isValid=True`，没有 C# error、Failed、Exception、mesh `self-intersecting` 或 `vertices with no weight`。

当前判断：

- 脸部位置没有再错位，头像检查里五官仍在头部中心；当前问题主要是发型和五官精度还不到目标参考图。
- 衣摆比六十二次更接近正确高度，裙子露出更多，走路时也没有炸形；但 hoodie 仍是程序化 retopo 面片，目标图那种柔软裁片、真实厚度和布料法线还没有完成。
- 手腕袖口比上一轮少一点硬，但原 VRoid 手本身仍偏细，最终要达到目标图需要真正手部 retopo 或手工权重/指形修模。

## 2026-09-03 人物本体六十四次强化：retopo hoodie 极薄厚度与下摆内收

本轮继续针对“为什么位置不对、为什么衣服像贴图”的问题收敛。复核当前生成顺序后确认：现在可见层是原 VRoid Tops 上半身基底、旧 finishing 衣摆/拉链、目标 retopo 粉色外片和目标细节覆盖层叠在一起；如果主衣片与控制线不共用同一套坐标，Unity 动起来后就会显得细节漂浮。

- `create_surface_fitted_hoodie_retopo_shell()` 的主粉色前片、后片和侧片新增极薄 `Retopo garment soft thickness`，把单面片改成有一点厚度的 skinned cloth 网格。
- 前片、后片和侧片下摆增加轻微内收与柔和弧线，减少侧视图里直板方盒子的轮廓。
- 继续保留原 VRoid 手部和 Humanoid 骨架，未恢复会造成怪手的自制低模覆盖层。
- 本轮参考的落地原则是：衣服网格先按身体/衣服表面定位，再使用同一套骨架权重驱动；Unity 端以 SkinnedMeshRenderer/Humanoid 导入和动作截图验证，而不是只看静态图。

已验证：

- `build_damogu_blender_skinned_mesh.py` 通过 Python compile 语法检查。
- Blender 5.2.1 LTS batchmode 成功刷新 `.blend/.fbx/.glb`，并输出正面、背面、侧面预览图。
- Unity 2022.3.62f3c1 batchmode 成功生成三视图、walk cycle、头像和手部袖口检查图。
- Unity 日志确认 `Avatar isHuman=True`、`isValid=True`，没有 C# error、Failed、Exception、mesh `self-intersecting` 或 `vertices with no weight`。

当前判断：

- 这一步让 lower hoodie 比六十三次更有边缘厚度，位置依旧稳定，动画没有新增导入风险。
- 美术还没有达到目标参考图：上身 hoodie 仍有工程化分层感，侧面仍偏平，手和头发还是源 VRoid 原型加少量补件。
- 下一步更应该做“真正连续 hoodie custom item”：把原 Tops 上半身基底、retopo 粉片、旧 finishing 逐步合并成一套连续 skinned mesh，再做手工 UV/法线和袖根/手腕权重。

## 2026-09-04 人物本体六十五次强化：原生可动手、袖口连接与脸部定位复核

根据“手还是怪、脸没有放对位置、网上查怎么做到正确位置”的反馈，本轮继续按 Blender Shrinkwrap/Data Transfer/Surface Deform、Unity SkinnedMeshRenderer/Humanoid 以及开源权重转移工作流复核。结论保持不变：人物要在游戏里运动自然，关键是保留同一套骨架和可动拓扑，再把衣服做成带权重的独立网格，不能用截图硬贴。

- 手部策略回到可动画基础：`USE_REPLACE_IMPORTED_HANDS_WITH_CUSTOM=False`、`USE_STYLIZED_HAND_OVERLAYS=False`，不再遮掉 VRoid 原手，也不再用球形/低模假手替换；`polish_imported_hand_shapes()` 改为轻微缩短指尖和圆润手掌，避免把手指压成团。
- Unity 手姿态收敛：`RelaxedHandPoseBlend` 提高到 `0.90`，同时取消 thumb/index/little spread 的额外张开，走路和检查图里手指更接近自然下垂。
- 袖口错位修正：关闭 Unity 临时生成的 wrist cuff，避免和 Blender 导出的袖口叠层；手部检查图现在会保留 `Custom_Hoodie_Shell`/`Hoodie_Shell`，不再只显示裸手臂和孤立袖口。
- 衣服体积补强：打开 `USE_TARGET_LOWER_SLEEVE_TRANSITION_SHELL=True`，额外生成左右下袖过渡 skinned mesh，从袖筒逐渐收到袖口，动作时以 lower-arm 权重为主、hand 权重参与。
- hoodie 材质尝试：测试过只把 hoodie 主壳改成 Unity Standard 不透明布料材质，但会出现黑色光照块；已回退到 MToon/角色着色器，并单独降低 custom hoodie 的提亮和 shade，让最终版保持稳定无黑斑。
- 刘海复核：脸部不做平移，Unity 日志显示 face/eye center 相对 head bone 稳定；仅在 Unity 预览中轻微上提和左右打开前刘海，减少遮眼造成的“脸没对上”的观感。

已验证：

- `build_damogu_blender_skinned_mesh.py` 通过 Python compile 语法检查。
- Blender 5.2.1 LTS batchmode 成功刷新 `.blend/.fbx/.glb`，并输出正面、背面、侧面预览图。
- Unity 2022.3.62f3c1 batchmode 成功生成三视图、walk cycle、头像和手部袖口检查图。
- Unity 日志确认 `Avatar isHuman=True`、`isValid=True`，脸部坐标稳定；最后一轮 ReviewBundle 成功完成。日志里有 Android ADB 的 `protocol fault` 提示，但与角色导入/截图无关，没有 C# 编译错误或模型导入中断。

当前判断：

- 现在这版已经不是方块/假手路线，而是可动 VRoid/Humanoid 基础加 Blender skinned mesh 衣服补件；可以继续在 Unity 里跑走路检查。
- 手部比球形替换版自然，但还没有到目标图那种精修手型；要继续提升，需要在 Blender 里对原手拓扑做手工修模，或换一套质量更好的手部 skinned mesh 并转权重。
- 衣服比之前多了下袖过渡和布纹/缝线，但上半身仍有程序化分层感；下一步应把 hoodie 主壳、下袖、帽领、口袋和拉链真正合并成一套连续 custom item，再做手工 UV/法线贴图和权重刷细。

## 2026-09-04 人物本体六十六次强化：去贴片、压布料材质与手腕清理

根据“继续改进、衣服不像贴图、手太怪”的反馈，本轮重点从增加细节改为减掉错误层级。复核 Unity/Blender 预览后确认：之前最大的问题不是背景，也不是骨架，而是 hoodie 胸前、口袋、下袖和肩颈同时叠了多层补面，近景会像半透明贴片；手腕处还被皮肤遮罩切得太靠外，造成袖口和手之间的缺口。

- 新增并关闭 `USE_TARGET_FILLED_POCKET_PATCHES=False`，目标 hoodie 只保留口袋开口、下缘缝线和少量布纹线，不再生成整片袋鼠口袋布面。
- 新增并关闭 `USE_FITTED_SHOULDER_NECK_FILL_PANEL=False`，保留细的领口边、肩缝线，不再把大片白色肩颈补面铺到上胸，减少“方块补丁”感。
- 关闭 `USE_IMPORTED_HOODIE_WRIST_CUFF_LIP=False` 和 `USE_TARGET_LOWER_SLEEVE_TRANSITION_SHELL=False`，移除与主 hoodie shell 权重不一致的浮动袖口/下袖过渡层，避免手部近景出现碎片袖口。
- `mask_imported_body_under_custom_clothes()` 的袖子皮肤遮罩从 `abs(x)<=0.565` 收回到 `abs(x)<=0.532`，手腕皮肤保留到袖口下方，修掉手腕顶部缺口。
- `polish_imported_hand_shapes()` 放松手掌 y/z 压缩，并把拇指略收近，保留原 VRoid 可动手指拓扑，不回到低模假手。
- hoodie 烘焙纹理和 Unity MToon 材质继续压实：粉色、缝线、阴影和布纹对比略加强，custom hoodie 的 shadow/indirect 参数调低提亮，避免白粉外套发光发飘。
- 内短裤收窄、上移，深色材质从近黑改成深 navy，减少走路检查里裙摆下方的黑块穿插。

已验证：

- `build_damogu_blender_skinned_mesh.py` 通过 Python compile 语法检查。
- Blender 5.2.1 LTS batchmode 成功刷新 `.blend/.fbx/.glb`，并输出正面、背面、侧面预览图。
- Unity 2022.3.62f3c1 batchmode 成功生成三视图、walk cycle、头像和手部袖口检查图。
- Unity 日志确认 `Avatar isHuman=True`、`isValid=True`，最后一轮 ReviewBundle 成功完成；没有 C# 编译失败、异常、无权重或自交报错。

当前判断：

- 手腕和袖口近景比六十五次干净，不再有浮动袖口碎片，也没有皮肤遮罩切口；手型仍是原 VRoid 手加轻微修形，距离目标参考图的精修手还需要后续 retopo/手工 sculpt。
- hoodie 胸前的大面积贴片感已经减轻，整体更像一件衣服；但目标图那种真正定制外套的柔软厚度、袖根结构、帽领体积和布料法线还没有完全达到。
- 下一步最该继续做的是一套真正连续的 hoodie custom item：只保留一个主 skinned mesh，手工做袖根、袖口、帽领、拉链、口袋厚度和 UV/normal，而不是继续堆零散补件。
