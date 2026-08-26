# 新版 UI 生图记录

## 登录与大厅背景

输出：`lobby-background.png`

最终提示词：

> Use case: stylized-concept. Asset type: full-screen portrait lobby and login background for a premium casual farming mobile game. Create a new ORIGINAL 9:19.5 vertical background: a warm harvest town plaza at sunset, golden wheat fields and a river valley behind, elegant timber signboard space near the top, cozy stone path in the foreground, flower beds and market baskets at the sides, distant lighthouse and farm buildings. Leave broad clean areas: top center for game title, lower third for login buttons, middle-left for a friendly resident character overlay. Polished painterly 2D mobile-game illustration, high quality, bright and welcoming, coherent with existing Wheat Town assets. Background only, no UI panels, no buttons, no text, no logo, no watermark, no gambling/casino imagery, no reels, no cards.

参考图角色：`town-scene-portrait.png` 仅作为项目色板和田园世界观参考。

## 高级转盘背景

输出：`premium-slot-background.png`

最终提示词：

> Use case: stylized-concept. Asset type: full-screen portrait environment background beneath a premium casual farming slot-game UI. Create a new ORIGINAL high-fidelity 9:19.5 vertical background: an elegant open-air harvest conservatory on the edge of a golden wheat valley, seen from inside. Refined walnut rafters and cream plaster pillars frame only the outer edges, with delicate climbing jasmine, baskets, linen, ceramic pots and small wildflowers. Beyond them, a winding turquoise river, layered wheat hills, tiny farmhouses and a distant lighthouse sit in warm late-morning sunlight. Polished painterly mobile-game illustration, richer material texture and depth than the input, coherent with the town scene. Preserve a broad calm low-contrast center and clean lower-middle floor area for gameplay panels. Place detail and contrast mostly at the top corners, narrow side edges and bottom corners. Soft atmospheric perspective, warm bounce light, subtle vignette. Background only: no UI, panels, frames, reels, buttons, symbols, text, numbers, characters, logos or watermark. Avoid photorealism, dark casino mood, excessive gold ornament, clutter in the central 70%, or copying any reference literally.

参考图角色：旧转盘背景用于界面遮挡区判断；城镇场景用于项目色板与世界观统一。

## 高级资源与导航图标

输出：`source/premium-icon-sheet-greenscreen.png`

最终提示词：

> Create a strict 4-column by 2-row sprite sheet containing exactly eight isolated ORIGINAL premium casual-farming mobile game icons on uniform pure chroma green #00FF00. Top row from left: embossed wheat coin, bundled planks, blue-gray/turquoise ore, brass/cream settings gear. Bottom row from left: harvest 3-reel slot, daily calendar, town hall, green/gold gift. One icon centered in each equal cell, generous margins, crisp silhouette at 32 pixels, consistent polished painterly 2D style, warm walnut, cream, matte gold, forest green and turquoise accents. No text, letters, numbers, people, scenery, frames, brands, logos, particles, duplicate objects or shadows on the green. The background is exactly flat RGB 0,255,0 with no gradient, texture, green reflections or ornamentation.

切图输出：`premium-icons/coin.webp`、`premium-icons/wood.webp`、`premium-icons/ore.webp`、`premium-icons/settings.webp`、`premium-icons/slot.webp`、`premium-icons/journey.webp`、`premium-icons/town.webp`、`premium-icons/gift.webp`

## 高级 HUD 边框

输出：`source/premium-frame-sheet-greenscreen.png`

最终提示词：

> Create a strict 3-column by 2-row sprite sheet containing exactly six isolated ORIGINAL premium casual-farming HUD frames on uniform pure chroma green #00FF00. One centered asset per equal cell with generous margins. Top row: slim horizontal resource capsule with small circular icon socket and clean cream center; medium title plaque with shallow walnut edge and tiny wheat crest; compact rounded-square utility button bezel. Bottom row: wide thin 5-by-3 reel-window frame with a very large open center; simple rounded information-card frame with thin even nine-slice-friendly corners and large open center; compact bottom-navigation selected-tab plate with clean cream center. Polished painterly 2D mobile game quality, coherent walnut grain, cream enamel, matte antique gold, tiny forest-green enamel details. Thin, quiet, consistent borders, not ornate casino decoration. No text, letters, numbers, icons, characters, scenery, logos, watermark, extra objects or shadows on green. Background exactly flat RGB 0,255,0 with no gradient, texture, reflections or green ornamentation.

切图输出：`premium-ui/resource-pill.webp`、`premium-ui/title-plaque.webp`、`premium-ui/utility-button.webp`、`premium-ui/reel-frame.webp`、`premium-ui/card-frame.webp`、`premium-ui/nav-tab.webp`

## 二级美术精修组件

输出：`source/premium-polish-sheet-greenscreen.png`

最终提示词：

> Create a strict 3-column by 2-row sprite sheet containing exactly six isolated ORIGINAL premium casual-farming mobile game UI polish assets on uniform pure chroma green #00FF00. One centered asset per equal cell, generous margins, no overlap. Top row from left: (1) a large vertical harvest console body frame for a portrait mobile game, walnut carved side rails, cream top plaque, subtle matte-gold trim, broad open transparent-looking center area for a 5-by-3 harvest grid, lower empty control shelf; (2) a refined cream parchment information card frame with thin walnut edge, soft inner paper texture, nine-slice friendly corners; (3) a cozy resident dialogue card frame with small circular portrait socket on the left, cream text area, tiny wheat and leaf ornaments. Bottom row from left: (4) a single rounded harvest symbol tile plate, cream enamel center, thin gold rim, subtle inset shadow; (5) a slim dark walnut status strip frame with gold inset line and clean empty center; (6) a circular orange primary harvest button face with antique-gold rim and small wheat crest, no text. Style: polished painterly 2D premium casual farming game, coherent with warm walnut, cream enamel, matte antique gold, forest green accents, mobile-readable silhouettes, richer and cleaner than prototype UI. No text, letters, numbers, icons except tiny wheat ornaments, characters, scenery, logos, watermark, particles, extra loose objects, or shadows on green. Background exactly flat RGB 0,255,0 with no gradient, texture, reflection or green spill.

切图输出：`premium-polish/harvest-console.webp`、`premium-polish/info-card.webp`、`premium-polish/dialogue-card.webp`、`premium-polish/symbol-tile.webp`、`premium-polish/status-strip.webp`、`premium-polish/primary-button.webp`

接入说明：符号格底板在小尺寸下纹理过重，本轮未接入；其余组件用于收获台外壳、状态条、主按钮、日程/城镇/居民卡片。

## 轻量绿幕 HUD

输出：`source/mobile-hud-sheet-greenscreen.png`

最终提示词：

> Create an ORIGINAL lightweight mobile game HUD asset sheet for a cozy pastoral town slot game. Strict 3 columns by 2 rows, exactly six isolated UI assets, one centered asset per equal cell, generous pure green margin around every asset. Top row: (1) slim rounded resource counter capsule, (2) small rounded-square utility button bezel, (3) large circular primary spin-button ring. Bottom row: (4) thin rounded rectangular 5-by-3 slot-window frame, wide horizontal aspect ratio and open empty center, (5) compact rounded bottom-navigation bar plate, (6) compact rounded bottom-sheet information panel frame with open empty center. Style: premium casual mobile game, warm carved walnut, cream enamel, restrained antique gold and tiny wheat accents, clean readable silhouette, materially lighter and simpler than ornate casino UI, consistent perspective, crisp 2D game asset render. Absolutely no text, letters, numbers, symbols, icons, logos, characters, scenery, shadows outside each object's own edge, or extra decoration. Background must be exactly uniform pure chroma key green RGB 0,255,0 (#00FF00), flat unlit, no gradient, no texture, no green reflection, no green ornamentation. Assets must not touch cell boundaries.

参考图角色：项目原有 UI 的材质与色板参考；新素材要求更轻、更简洁。

## 竖屏城镇场景

输出：`town-scene-portrait.png`

最终提示词：

> Create a new ORIGINAL portrait mobile-game background for a cozy pastoral town-management game, using the provided image only as a color palette and soft painterly rendering reference, not as a composition to copy. Full-screen vertical 9:19.5 composition. Golden wheat valley at late morning, winding pale stone path and river, distant lighthouse hill, warm timber fences, flowers, trees and soft atmospheric depth. Design five clearly readable empty construction terraces/pads integrated into the landscape: upper left, upper right, middle left, middle right, and a prominent upper-center hill pad. Leave the lower 30 percent visually calmer and less detailed for a translucent contextual upgrade panel. No buildings on the pads, no characters, no UI, no text, no logos, no icons, no borders. Bright friendly premium casual-game illustration, clear foreground/midground/background separation, painterly but crisp enough for mobile.

参考图角色：项目原有田园背景的色板与绘制质感参考。

## UI 调研来源

- Hay Day 官方网站：https://supercell.com/en/games/hayday/
- Hay Day Scenic Mode：https://support.supercell.com/hay-day/en/articles/scenic-mode.html
- Hay Day Edit Mode：https://support.supercell.com/hay-day/en/articles/edit-mode.html
- Township App Store：https://apps.apple.com/us/app/township/id638689075?platform=iphone
- Merge Mansion App Store：https://apps.apple.com/us/app/merge-mansion-puzzles-story/id1484442152
- Royal Match App Store：https://apps.apple.com/us/app/royal-match/id1482155847

## 转盘页整体背景

输出：`slot-background-portrait.png`

最终提示词：

> Use case: stylized-concept. Asset type: full-screen portrait background beneath an existing mobile slot-game interface. Primary request: Create a new original, premium pastoral background that makes the current Wheat Town slot screen feel richer and less plain while keeping the gameplay UI highly readable. Image 1 is the current screen composition reference only—understand where the top HUD, large slot machine, lower town preview, resident panel, and bottom navigation sit, but DO NOT reproduce any UI. Image 2 is the project's painterly color palette and world-art reference. Scene: a cozy open-sided timber harvest pavilion overlooking a golden wheat valley and a winding river, with warm cream plaster, walnut beams, small climbing vines, wildflowers, soft distant hills and a tiny lighthouse far away. Polished premium casual mobile game illustration, painterly 2D with crisp material rendering. Vertical 9:19.5. Keep the central 70% calm, softly illuminated and low-contrast; place richer detail mainly at the upper corners, narrow side edges, and bottom corners. Bright late-morning golden light. Background only; no interface, panels, frames, reels, buttons, symbols, text, numbers, characters, logos or watermark.

参考图角色：当前页面仅作为界面占位构图参考；城镇场景仅作为项目色板与绘制风格参考。

## 田园日程绿幕组件

输出：`source/journey-ui-sheet-greenscreen.png`

最终提示词：

> Create six ORIGINAL polished pastoral game UI assets in a strict 3-column by 2-row grid, one centered object per equal cell with generous empty green margins. Top row: a warm walnut order-board frame with three pinned parchment areas, a closed harvest reward chest, and a compact event parchment frame. Bottom row: a circular milestone medallion, an open daily-summary ledger, and a slim journey header sign. Premium friendly casual farming-game UI, painterly 2D render, crisp mobile silhouette, warm walnut, cream, antique gold and forest-green accents. Exactly six isolated assets; no words, letters, numbers, reward icons, currency symbols, logos, characters, scenery, watermark or loose objects. Background exactly uniform chroma-key green RGB 0,255,0 (#00FF00), flat and unlit, with no gradient, texture, green reflection or green ornamentation.

参考图角色：项目现有轻量 HUD 的材质、色板与移动端清晰度参考。

日程结构参考：

- Hay Day Task Events：https://support.supercell.com/hay-day/en/articles/task-events.html
- Hay Day Daily Quests：https://support.supercell.com/hay-day/en/articles/daily-quests.html
- Hay Day Goals and Rewards：https://support.supercell.com/hay-day/en/articles/goals-and-rewards.html
- Merge Mansion 基础任务闭环：https://support.metacoregames.com/hc/en/merge-mansion/articles/welcome-to-merge-mansion-76
