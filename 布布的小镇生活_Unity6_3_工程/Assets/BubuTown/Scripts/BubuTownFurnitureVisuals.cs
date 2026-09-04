using UnityEngine;

namespace BubuTown
{
    public static class BubuTownFurnitureVisuals
    {
        public enum PlacementKind
        {
            Floor,
            AgainstWall,
            WallMounted
        }

        public static GameObject CreatePlacedFurniture(string furnitureId, Transform parent, Vector3 localPosition)
        {
            var root = new GameObject("Placed_" + furnitureId);
            root.transform.SetParent(parent, false);
            root.transform.localPosition = localPosition;
            root.transform.localRotation = Quaternion.identity;

            if (TryCreateModlyFurniture(root.transform, furnitureId))
            {
                return root;
            }

            switch (furnitureId)
            {
                case "fur_bed_poor":
                    Part(root.transform, "Mattress", PrimitiveType.Cube, new Vector3(0f, 0.25f, 0f), new Vector3(1.6f, 0.25f, 2.2f), new Color(0.95f, 0.58f, 0.68f));
                    Part(root.transform, "Pillow", PrimitiveType.Cube, new Vector3(0f, 0.48f, 0.72f), new Vector3(1.2f, 0.18f, 0.45f), new Color(1f, 0.86f, 0.90f));
                    Part(root.transform, "WoodBase", PrimitiveType.Cube, new Vector3(0f, 0.12f, 0f), new Vector3(1.75f, 0.18f, 2.35f), new Color(0.45f, 0.28f, 0.15f));
                    break;
                case "fur_wood_table":
                    Part(root.transform, "Top", PrimitiveType.Cube, new Vector3(0f, 0.72f, 0f), new Vector3(1.4f, 0.16f, 1.1f), new Color(0.48f, 0.30f, 0.14f));
                    TableLegs(root.transform);
                    break;
                case "fur_small_chair":
                    Part(root.transform, "Seat", PrimitiveType.Cube, new Vector3(0f, 0.45f, 0f), new Vector3(0.8f, 0.16f, 0.8f), new Color(0.95f, 0.74f, 0.26f));
                    Part(root.transform, "Back", PrimitiveType.Cube, new Vector3(0f, 0.88f, 0.36f), new Vector3(0.8f, 0.78f, 0.14f), new Color(0.95f, 0.74f, 0.26f));
                    TableLegs(root.transform, 0.32f, 0.32f, 0.42f);
                    break;
                case "fur_basic_rug":
                    Part(root.transform, "Rug", PrimitiveType.Cube, new Vector3(0f, 0.03f, 0f), new Vector3(2.2f, 0.06f, 1.4f), new Color(0.36f, 0.55f, 0.90f));
                    break;
                case "fur_desk_lamp":
                    Part(root.transform, "Base", PrimitiveType.Cylinder, new Vector3(0f, 0.10f, 0f), new Vector3(0.38f, 0.10f, 0.38f), new Color(0.95f, 0.78f, 0.25f));
                    Part(root.transform, "Stem", PrimitiveType.Cylinder, new Vector3(0f, 0.55f, 0f), new Vector3(0.08f, 0.45f, 0.08f), new Color(0.55f, 0.40f, 0.24f));
                    Part(root.transform, "Shade", PrimitiveType.Cylinder, new Vector3(0f, 1.05f, 0f), new Vector3(0.55f, 0.28f, 0.55f), new Color(1f, 0.88f, 0.42f));
                    break;
                case "fur_small_bookcase":
                    Part(root.transform, "Bookcase", PrimitiveType.Cube, new Vector3(0f, 0.9f, 0f), new Vector3(1.2f, 1.8f, 0.35f), new Color(0.46f, 0.28f, 0.14f));
                    Part(root.transform, "Books_A", PrimitiveType.Cube, new Vector3(-0.28f, 0.75f, -0.2f), new Vector3(0.18f, 0.55f, 0.12f), new Color(0.35f, 0.55f, 0.95f));
                    Part(root.transform, "Books_B", PrimitiveType.Cube, new Vector3(0f, 0.77f, -0.2f), new Vector3(0.18f, 0.48f, 0.12f), new Color(0.95f, 0.45f, 0.45f));
                    Part(root.transform, "Books_C", PrimitiveType.Cube, new Vector3(0.28f, 0.72f, -0.2f), new Vector3(0.18f, 0.60f, 0.12f), new Color(0.35f, 0.75f, 0.42f));
                    break;
                case "fur_pink_wallpaper":
                    Part(root.transform, "WallpaperPanel", PrimitiveType.Cube, new Vector3(0f, 1.1f, 0f), new Vector3(1.9f, 2.2f, 0.08f), new Color(1f, 0.62f, 0.76f));
                    break;
                case "fur_wood_floor":
                    Part(root.transform, "FloorTile", PrimitiveType.Cube, new Vector3(0f, 0.04f, 0f), new Vector3(1.8f, 0.08f, 1.8f), new Color(0.50f, 0.30f, 0.14f));
                    break;
                case "fur_cake_decor":
                    Part(root.transform, "CakeBase", PrimitiveType.Cylinder, new Vector3(0f, 0.20f, 0f), new Vector3(0.65f, 0.20f, 0.65f), new Color(1f, 0.70f, 0.82f));
                    Part(root.transform, "Cream", PrimitiveType.Sphere, new Vector3(0f, 0.45f, 0f), new Vector3(0.55f, 0.24f, 0.55f), Color.white);
                    break;
                case "fur_small_plant":
                    Part(root.transform, "Pot", PrimitiveType.Cylinder, new Vector3(0f, 0.22f, 0f), new Vector3(0.45f, 0.28f, 0.45f), new Color(0.62f, 0.34f, 0.18f));
                    Part(root.transform, "Leaves", PrimitiveType.Sphere, new Vector3(0f, 0.72f, 0f), new Vector3(0.75f, 0.65f, 0.75f), new Color(0.25f, 0.62f, 0.32f));
                    break;
                case "fur_pink_bed":
                    Part(root.transform, "Pink_Heart_Bed_Base", PrimitiveType.Cube, new Vector3(0f, 0.14f, 0f), new Vector3(1.8f, 0.18f, 2.25f), new Color(0.96f, 0.48f, 0.68f));
                    Part(root.transform, "Soft_Pink_Mattress", PrimitiveType.Cube, new Vector3(0f, 0.34f, 0f), new Vector3(1.65f, 0.26f, 2.08f), new Color(1f, 0.72f, 0.82f));
                    Part(root.transform, "Rounded_Pillow", PrimitiveType.Cube, new Vector3(0f, 0.56f, 0.68f), new Vector3(1.15f, 0.18f, 0.46f), new Color(1f, 0.90f, 0.94f));
                    Part(root.transform, "Heart_Marker", PrimitiveType.Sphere, new Vector3(0f, 0.75f, -0.92f), new Vector3(0.26f, 0.18f, 0.08f), new Color(1f, 0.32f, 0.56f));
                    break;
                case "fur_pink_vanity":
                    Part(root.transform, "Vanity_Top", PrimitiveType.Cube, new Vector3(0f, 0.64f, 0f), new Vector3(1.3f, 0.14f, 0.55f), new Color(1f, 0.66f, 0.78f));
                    Part(root.transform, "Mirror", PrimitiveType.Cube, new Vector3(0f, 1.18f, 0.24f), new Vector3(0.74f, 0.78f, 0.06f), new Color(0.72f, 0.90f, 0.98f));
                    Part(root.transform, "Vanity_Drawer", PrimitiveType.Cube, new Vector3(0f, 0.43f, 0f), new Vector3(1.14f, 0.24f, 0.48f), new Color(0.98f, 0.52f, 0.68f));
                    TableLegs(root.transform, 0.5f, 0.18f, 0.55f);
                    break;
                case "fur_natural_bed":
                    Part(root.transform, "Wood_Frame", PrimitiveType.Cube, new Vector3(0f, 0.16f, 0f), new Vector3(1.9f, 0.22f, 2.3f), new Color(0.52f, 0.34f, 0.18f));
                    Part(root.transform, "Linen_Mattress", PrimitiveType.Cube, new Vector3(0f, 0.38f, 0f), new Vector3(1.62f, 0.28f, 2.02f), new Color(0.88f, 0.82f, 0.68f));
                    Part(root.transform, "Natural_Pillow", PrimitiveType.Cube, new Vector3(0f, 0.60f, 0.67f), new Vector3(1.1f, 0.18f, 0.44f), new Color(0.96f, 0.91f, 0.80f));
                    break;
                case "fur_natural_table":
                    Part(root.transform, "Oak_Table_Top", PrimitiveType.Cube, new Vector3(0f, 0.72f, 0f), new Vector3(1.45f, 0.14f, 1.1f), new Color(0.62f, 0.42f, 0.22f));
                    Part(root.transform, "Runner", PrimitiveType.Cube, new Vector3(0f, 0.81f, 0f), new Vector3(1.16f, 0.03f, 0.22f), new Color(0.75f, 0.86f, 0.58f));
                    TableLegs(root.transform, 0.55f, 0.38f, 0.64f);
                    break;
                case "fur_modern_sofa":
                    Part(root.transform, "Sofa_Seat", PrimitiveType.Cube, new Vector3(0f, 0.38f, 0f), new Vector3(2.1f, 0.36f, 0.9f), new Color(0.72f, 0.75f, 0.78f));
                    Part(root.transform, "Sofa_Back", PrimitiveType.Cube, new Vector3(0f, 0.74f, 0.38f), new Vector3(2.1f, 0.72f, 0.18f), new Color(0.58f, 0.62f, 0.66f));
                    Part(root.transform, "Left_Arm", PrimitiveType.Cube, new Vector3(-1.13f, 0.56f, 0f), new Vector3(0.18f, 0.52f, 0.92f), new Color(0.58f, 0.62f, 0.66f));
                    Part(root.transform, "Right_Arm", PrimitiveType.Cube, new Vector3(1.13f, 0.56f, 0f), new Vector3(0.18f, 0.52f, 0.92f), new Color(0.58f, 0.62f, 0.66f));
                    break;
                case "fur_modern_floor_lamp":
                    Part(root.transform, "Slim_Base", PrimitiveType.Cylinder, new Vector3(0f, 0.06f, 0f), new Vector3(0.44f, 0.08f, 0.44f), new Color(0.16f, 0.18f, 0.20f));
                    Part(root.transform, "Slim_Pole", PrimitiveType.Cylinder, new Vector3(0f, 0.82f, 0f), new Vector3(0.07f, 0.78f, 0.07f), new Color(0.16f, 0.18f, 0.20f));
                    Part(root.transform, "Warm_Shade", PrimitiveType.Cylinder, new Vector3(0f, 1.58f, 0f), new Vector3(0.55f, 0.22f, 0.55f), new Color(1f, 0.92f, 0.64f));
                    break;
                case "fur_cake_bed":
                    Part(root.transform, "Cake_Bed_Base", PrimitiveType.Cylinder, new Vector3(0f, 0.16f, 0f), new Vector3(1.72f, 0.18f, 1.72f), new Color(0.95f, 0.62f, 0.44f));
                    Part(root.transform, "Cream_Mattress", PrimitiveType.Cube, new Vector3(0f, 0.40f, 0f), new Vector3(1.62f, 0.28f, 2.0f), new Color(1f, 0.86f, 0.68f));
                    Part(root.transform, "Strawberry_Pillow", PrimitiveType.Sphere, new Vector3(0f, 0.64f, 0.72f), new Vector3(0.64f, 0.22f, 0.36f), new Color(0.95f, 0.22f, 0.30f));
                    break;
                case "fur_cake_wall_shelf":
                    Part(root.transform, "Wall_Shelf", PrimitiveType.Cube, new Vector3(0f, 0f, 0f), new Vector3(1.25f, 0.14f, 0.22f), new Color(1f, 0.78f, 0.48f));
                    Part(root.transform, "Mini_Cake_A", PrimitiveType.Cylinder, new Vector3(-0.36f, 0.18f, -0.01f), new Vector3(0.24f, 0.14f, 0.24f), new Color(1f, 0.60f, 0.76f));
                    Part(root.transform, "Mini_Cake_B", PrimitiveType.Cylinder, new Vector3(0.32f, 0.18f, -0.01f), new Vector3(0.24f, 0.14f, 0.24f), new Color(0.96f, 0.78f, 0.38f));
                    break;
                case "fur_esports_desk":
                    Part(root.transform, "Black_Desk_Top", PrimitiveType.Cube, new Vector3(0f, 0.72f, 0f), new Vector3(1.65f, 0.14f, 0.82f), new Color(0.08f, 0.09f, 0.11f));
                    Part(root.transform, "Monitor", PrimitiveType.Cube, new Vector3(0f, 1.08f, 0.22f), new Vector3(0.92f, 0.48f, 0.08f), new Color(0.05f, 0.08f, 0.12f));
                    Part(root.transform, "Neon_Screen", PrimitiveType.Cube, new Vector3(0f, 1.08f, 0.16f), new Vector3(0.78f, 0.34f, 0.03f), new Color(0.10f, 0.86f, 0.95f));
                    TableLegs(root.transform, 0.62f, 0.28f, 0.64f);
                    break;
                case "fur_esports_wall_poster":
                    Part(root.transform, "Poster_Back", PrimitiveType.Cube, new Vector3(0f, 0f, 0f), new Vector3(1.05f, 1.35f, 0.05f), new Color(0.07f, 0.08f, 0.12f));
                    Part(root.transform, "Poster_Neon_Bar_A", PrimitiveType.Cube, new Vector3(-0.18f, 0.22f, -0.035f), new Vector3(0.12f, 0.72f, 0.035f), new Color(0.95f, 0.22f, 0.85f));
                    Part(root.transform, "Poster_Neon_Bar_B", PrimitiveType.Cube, new Vector3(0.20f, -0.18f, -0.035f), new Vector3(0.12f, 0.72f, 0.035f), new Color(0.10f, 0.86f, 0.95f));
                    break;
                default:
                    Part(root.transform, "未知家具_灰盒主体", PrimitiveType.Cube, new Vector3(0f, 0.3f, 0f), new Vector3(0.8f, 0.6f, 0.8f), new Color(0.72f, 0.72f, 0.72f));
                    break;
            }

            AddFallbackFurniturePolish(root.transform, furnitureId);
            return root;
        }

        private static void AddFallbackFurniturePolish(Transform root, string furnitureId)
        {
            switch (furnitureId)
            {
                case "fur_pink_bed":
                case "fur_target_bed":
                    Part(root, "兜底床_粉色绗缝被面", PrimitiveType.Cube, new Vector3(0f, 0.58f, -0.10f), new Vector3(1.42f, 0.035f, 1.28f), new Color(1f, 0.63f, 0.78f));
                    Part(root, "兜底床_奶油床尾厚边", PrimitiveType.Cube, new Vector3(0f, 0.60f, -0.76f), new Vector3(1.40f, 0.055f, 0.08f), new Color(1f, 0.92f, 0.84f));
                    Part(root, "兜底床_横向软压线一", PrimitiveType.Cube, new Vector3(0f, 0.625f, -0.25f), new Vector3(1.12f, 0.010f, 0.020f), new Color(0.92f, 0.38f, 0.54f, 0.85f));
                    Part(root, "兜底床_横向软压线二", PrimitiveType.Cube, new Vector3(0f, 0.626f, 0.18f), new Vector3(1.04f, 0.010f, 0.020f), new Color(0.92f, 0.38f, 0.54f, 0.85f));
                    Part(root, "兜底床_左圆抱枕", PrimitiveType.Sphere, new Vector3(-0.32f, 0.72f, 0.82f), new Vector3(0.28f, 0.16f, 0.18f), new Color(1f, 0.92f, 0.86f));
                    Part(root, "兜底床_右圆抱枕", PrimitiveType.Sphere, new Vector3(0.32f, 0.72f, 0.82f), new Vector3(0.28f, 0.16f, 0.18f), new Color(1f, 0.92f, 0.86f));
                    Part(root, "兜底床_粉色小抱枕", PrimitiveType.Sphere, new Vector3(0f, 0.78f, 0.50f), new Vector3(0.24f, 0.15f, 0.10f), new Color(1f, 0.52f, 0.70f));
                    break;
                case "fur_natural_bed":
                    Part(root, "兜底自然床_亚麻被面", PrimitiveType.Cube, new Vector3(0f, 0.59f, -0.10f), new Vector3(1.42f, 0.035f, 1.30f), new Color(0.90f, 0.84f, 0.70f));
                    Part(root, "兜底自然床_木质床尾边", PrimitiveType.Cube, new Vector3(0f, 0.55f, -0.86f), new Vector3(1.52f, 0.08f, 0.10f), new Color(0.54f, 0.34f, 0.16f));
                    Part(root, "兜底自然床_浅色长枕", PrimitiveType.Sphere, new Vector3(0f, 0.74f, 0.78f), new Vector3(0.56f, 0.15f, 0.20f), new Color(0.96f, 0.91f, 0.80f));
                    break;
                case "fur_cake_bed":
                    Part(root, "兜底蛋糕床_奶油床罩", PrimitiveType.Cube, new Vector3(0f, 0.60f, -0.08f), new Vector3(1.42f, 0.035f, 1.30f), new Color(1f, 0.86f, 0.68f));
                    Part(root, "兜底蛋糕床_草莓点缀左", PrimitiveType.Sphere, new Vector3(-0.38f, 0.66f, 0.14f), new Vector3(0.10f, 0.08f, 0.10f), new Color(0.95f, 0.20f, 0.30f));
                    Part(root, "兜底蛋糕床_草莓点缀右", PrimitiveType.Sphere, new Vector3(0.36f, 0.66f, -0.30f), new Vector3(0.10f, 0.08f, 0.10f), new Color(0.95f, 0.20f, 0.30f));
                    Part(root, "兜底蛋糕床_奶油滚边", PrimitiveType.Cube, new Vector3(0f, 0.64f, -0.78f), new Vector3(1.34f, 0.045f, 0.07f), new Color(1f, 0.95f, 0.84f));
                    break;
                case "fur_modern_sofa":
                case "fur_target_sofa":
                    Part(root, "兜底沙发_蓝色软包坐垫", PrimitiveType.Cube, new Vector3(0f, 0.59f, -0.12f), new Vector3(1.72f, 0.060f, 0.52f), new Color(0.34f, 0.50f, 0.70f));
                    Part(root, "兜底沙发_靠背布面高光", PrimitiveType.Cube, new Vector3(0f, 0.86f, 0.38f), new Vector3(1.72f, 0.030f, 0.08f), new Color(0.55f, 0.70f, 0.84f, 0.80f));
                    Part(root, "兜底沙发_左坐垫分缝", PrimitiveType.Cube, new Vector3(-0.36f, 0.63f, -0.12f), new Vector3(0.025f, 0.020f, 0.44f), new Color(0.12f, 0.22f, 0.35f));
                    Part(root, "兜底沙发_右坐垫分缝", PrimitiveType.Cube, new Vector3(0.36f, 0.63f, -0.12f), new Vector3(0.025f, 0.020f, 0.44f), new Color(0.12f, 0.22f, 0.35f));
                    Part(root, "兜底沙发_奶油抱枕", PrimitiveType.Sphere, new Vector3(-0.52f, 0.78f, 0.12f), new Vector3(0.20f, 0.16f, 0.08f), new Color(1f, 0.92f, 0.84f));
                    Part(root, "兜底沙发_粉色抱枕", PrimitiveType.Sphere, new Vector3(0.58f, 0.78f, 0.12f), new Vector3(0.19f, 0.15f, 0.08f), new Color(1f, 0.58f, 0.72f));
                    break;
                case "fur_target_coffee_table":
                case "fur_natural_table":
                    Part(root, "兜底茶几_浅色桌垫", PrimitiveType.Cube, new Vector3(0f, 0.82f, 0f), new Vector3(0.86f, 0.020f, 0.54f), new Color(1f, 0.92f, 0.80f));
                    Part(root, "兜底茶几_清漆高光", PrimitiveType.Cube, new Vector3(0.24f, 0.845f, -0.10f), new Vector3(0.38f, 0.010f, 0.12f), new Color(0.94f, 0.72f, 0.42f));
                    Part(root, "兜底茶几_小书封面", PrimitiveType.Cube, new Vector3(-0.30f, 0.86f, 0.16f), new Vector3(0.34f, 0.035f, 0.22f), new Color(0.35f, 0.56f, 0.78f));
                    Part(root, "兜底茶几_小盆栽盆", PrimitiveType.Cylinder, new Vector3(0.32f, 0.90f, 0.14f), new Vector3(0.13f, 0.08f, 0.13f), new Color(0.56f, 0.32f, 0.16f));
                    Part(root, "兜底茶几_小盆栽叶", PrimitiveType.Sphere, new Vector3(0.32f, 1.02f, 0.14f), new Vector3(0.20f, 0.12f, 0.20f), new Color(0.24f, 0.62f, 0.34f));
                    break;
                case "fur_target_nightstand":
                case "fur_pink_vanity":
                    Part(root, "兜底柜体_抽屉正面细框", PrimitiveType.Cube, new Vector3(0f, 0.46f, -0.28f), new Vector3(0.82f, 0.28f, 0.020f), new Color(0.74f, 0.47f, 0.24f));
                    Part(root, "兜底柜体_抽屉分割线", PrimitiveType.Cube, new Vector3(0f, 0.46f, -0.30f), new Vector3(0.68f, 0.020f, 0.014f), new Color(0.42f, 0.25f, 0.12f));
                    Part(root, "兜底柜体_圆拉手", PrimitiveType.Sphere, new Vector3(0f, 0.46f, -0.325f), new Vector3(0.055f, 0.040f, 0.018f), new Color(0.94f, 0.70f, 0.28f));
                    break;
                case "fur_esports_desk":
                    Part(root, "兜底电竞桌_桌面红色灯带", PrimitiveType.Cube, new Vector3(0f, 0.82f, -0.44f), new Vector3(1.50f, 0.025f, 0.035f), new Color(0.95f, 0.10f, 0.18f));
                    Part(root, "兜底电竞桌_屏幕蓝光", PrimitiveType.Cube, new Vector3(0f, 1.08f, 0.13f), new Vector3(0.70f, 0.28f, 0.020f), new Color(0.16f, 0.70f, 1f));
                    Part(root, "兜底电竞桌_键盘", PrimitiveType.Cube, new Vector3(0f, 0.84f, -0.12f), new Vector3(0.58f, 0.035f, 0.18f), new Color(0.03f, 0.04f, 0.05f));
                    break;
            }
        }

        public static Vector2Int FootprintCells(string furnitureId)
        {
            switch (furnitureId)
            {
                case "fur_target_bed":
                case "fur_pink_bed":
                case "fur_natural_bed":
                case "fur_cake_bed":
                    return new Vector2Int(3, 3);
                case "fur_target_sofa":
                case "fur_modern_sofa":
                case "fur_esports_desk":
                    return new Vector2Int(3, 2);
                case "fur_target_kitchen":
                    return new Vector2Int(3, 1);
                case "fur_target_coffee_table":
                case "fur_natural_table":
                case "fur_target_boxes":
                case "fur_pink_vanity":
                    return new Vector2Int(2, 2);
                case "fur_cake_wall_shelf":
                case "fur_esports_wall_poster":
                    return new Vector2Int(2, 1);
                case "fur_modern_floor_lamp":
                    return Vector2Int.one;
                default:
                    return Vector2Int.one;
            }
        }

        public static PlacementKind GetPlacementKind(string furnitureId)
        {
            if (furnitureId.Contains("wall") || furnitureId.Contains("poster"))
            {
                return PlacementKind.WallMounted;
            }

            switch (furnitureId)
            {
                case "fur_target_bed":
                case "fur_pink_bed":
                case "fur_natural_bed":
                case "fur_cake_bed":
                case "fur_target_kitchen":
                case "fur_pink_vanity":
                case "fur_esports_desk":
                case "fur_small_bookcase":
                    return PlacementKind.AgainstWall;
                default:
                    return PlacementKind.Floor;
            }
        }

        private static bool TryCreateModlyFurniture(Transform root, string furnitureId)
        {
            var resourcePath = ResourcePathForFurniture(furnitureId);
            if (string.IsNullOrEmpty(resourcePath))
            {
                return false;
            }

            var prefab = Resources.Load<GameObject>(resourcePath);
            if (prefab == null)
            {
                return false;
            }

            var model = Object.Instantiate(prefab, root);
            model.name = "运行时_" + ChineseModelName(furnitureId);
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = Vector3.one;
            FitModelToBounds(model.transform, TargetBoundsForFurniture(furnitureId));
            ApplyRuntimeTint(model.transform, RuntimeTintForFurniture(furnitureId));
            return true;
        }

        private static string ResourcePathForFurniture(string furnitureId)
        {
            switch (furnitureId)
            {
                case "fur_target_bed":
                case "fur_pink_bed":
                case "fur_natural_bed":
                    return "星湾镇室内家具Modly/目标公寓_粉木单人床_Modly低清试件";
                case "fur_cake_bed":
                    return "星湾镇室内家具Blender/蛋糕奶油床";
                case "fur_target_sofa":
                case "fur_modern_sofa":
                    return "星湾镇室内家具Modly/目标公寓_海蓝小沙发_Modly低清试件";
                case "fur_target_coffee_table":
                case "fur_natural_table":
                    return "星湾镇室内家具Modly/目标公寓_原木小茶几_Modly低清试件";
                case "fur_target_lamp":
                case "fur_modern_floor_lamp":
                    return "星湾镇室内家具Modly/目标公寓_圆润台灯_Modly低清试件";
                case "fur_target_nightstand":
                case "fur_pink_vanity":
                    return furnitureId == "fur_pink_vanity"
                        ? "星湾镇室内家具Blender/云朵粉色梳妆台"
                        : "星湾镇室内家具Modly/目标公寓_原木床头柜_Modly低清试件";
                case "fur_target_kitchen":
                    return "星湾镇室内家具Modly/目标公寓_灰台面粉柜厨房_Modly低清试件";
                case "fur_target_boxes":
                    return "星湾镇室内家具Blender/目标搬家纸箱堆";
                case "fur_esports_desk":
                    return "星湾镇室内家具Blender/电竞霓虹电脑桌";
                case "fur_cake_wall_shelf":
                case "fur_esports_wall_poster":
                    return "星湾镇室内家具Blender/壁挂星星灯";
                default:
                    return string.Empty;
            }
        }

        private static string ChineseModelName(string furnitureId)
        {
            switch (furnitureId)
            {
                case "fur_target_bed":
                    return "目标公寓粉木单人床_Modly模型";
                case "fur_target_nightstand":
                    return "目标公寓原木床头柜_Modly模型";
                case "fur_target_sofa":
                    return "目标公寓海蓝小沙发_Modly模型";
                case "fur_target_coffee_table":
                    return "目标公寓原木小茶几_Modly模型";
                case "fur_target_kitchen":
                    return "目标公寓厨房组合_Modly模型";
                case "fur_target_lamp":
                    return "目标公寓圆润台灯_Modly模型";
                case "fur_target_boxes":
                    return "目标公寓搬家纸箱堆_Blender模型";
                case "fur_pink_bed":
                    return "可爱粉木单人床_Modly模型";
                case "fur_natural_bed":
                    return "木质自然单人床_Modly模型";
                case "fur_cake_bed":
                    return "蛋糕主题床_Modly模型";
                case "fur_modern_sofa":
                    return "海蓝小沙发_Modly模型";
                case "fur_natural_table":
                    return "原木小茶几_Modly模型";
                case "fur_modern_floor_lamp":
                    return "圆润台灯_Modly模型";
                case "fur_pink_vanity":
                    return "云朵粉色梳妆台_Blender模型";
                case "fur_esports_desk":
                    return "电竞霓虹电脑桌_Blender模型";
                case "fur_cake_wall_shelf":
                    return "蛋糕墙上星星搁板_Blender模型";
                case "fur_esports_wall_poster":
                    return "电竞墙上星星灯_Blender模型";
                default:
                    return furnitureId;
            }
        }

        private static Vector3 TargetBoundsForFurniture(string furnitureId)
        {
            switch (furnitureId)
            {
                case "fur_target_bed":
                case "fur_pink_bed":
                case "fur_natural_bed":
                case "fur_cake_bed":
                    return new Vector3(1.8f, 0.95f, 2.25f);
                case "fur_target_sofa":
                case "fur_modern_sofa":
                    return new Vector3(2.05f, 0.92f, 0.95f);
                case "fur_target_coffee_table":
                case "fur_natural_table":
                    return new Vector3(1.25f, 0.52f, 0.95f);
                case "fur_target_lamp":
                case "fur_modern_floor_lamp":
                    return new Vector3(0.56f, 1.45f, 0.56f);
                case "fur_target_nightstand":
                    return new Vector3(0.72f, 0.78f, 0.52f);
                case "fur_pink_vanity":
                    return new Vector3(1.3f, 1.35f, 0.68f);
                case "fur_target_kitchen":
                    return new Vector3(1.75f, 1.14f, 0.68f);
                case "fur_target_boxes":
                    return new Vector3(1.05f, 0.92f, 0.72f);
                case "fur_esports_desk":
                    return new Vector3(1.7f, 1.25f, 0.9f);
                case "fur_cake_wall_shelf":
                case "fur_esports_wall_poster":
                    return new Vector3(1.25f, 0.82f, 0.18f);
                default:
                    return Vector3.one;
            }
        }

        private static Color RuntimeTintForFurniture(string furnitureId)
        {
            switch (furnitureId)
            {
                case "fur_target_sofa":
                case "fur_modern_sofa":
                    return new Color(0.50f, 0.66f, 0.86f, 1f);
                case "fur_target_coffee_table":
                case "fur_target_nightstand":
                case "fur_natural_table":
                    return new Color(0.78f, 0.55f, 0.32f, 1f);
                case "fur_target_lamp":
                case "fur_modern_floor_lamp":
                    return new Color(1f, 0.88f, 0.58f, 1f);
                case "fur_target_kitchen":
                    return new Color(0.92f, 0.72f, 0.76f, 1f);
                case "fur_target_boxes":
                    return new Color(0.70f, 0.48f, 0.28f, 1f);
                case "fur_pink_vanity":
                    return new Color(1f, 0.66f, 0.82f, 1f);
                case "fur_cake_bed":
                    return new Color(1f, 0.74f, 0.58f, 1f);
                case "fur_esports_desk":
                case "fur_esports_wall_poster":
                    return new Color(0.34f, 0.78f, 1f, 1f);
                case "fur_cake_wall_shelf":
                    return new Color(1f, 0.78f, 0.50f, 1f);
                default:
                    return new Color(1f, 0.72f, 0.82f, 1f);
            }
        }

        private static void FitModelToBounds(Transform model, Vector3 targetSize)
        {
            if (!TryGetRendererBounds(model, out var bounds))
            {
                return;
            }

            var safeSize = bounds.size;
            var scale = Mathf.Min(
                targetSize.x / Mathf.Max(0.01f, safeSize.x),
                Mathf.Min(targetSize.y / Mathf.Max(0.01f, safeSize.y), targetSize.z / Mathf.Max(0.01f, safeSize.z)));
            model.localScale *= scale;

            if (!TryGetRendererBounds(model, out bounds))
            {
                return;
            }

            var offset = new Vector3(-bounds.center.x, -bounds.min.y, -bounds.center.z);
            model.position += offset;
        }

        private static bool TryGetRendererBounds(Transform root, out Bounds bounds)
        {
            var renderers = root.GetComponentsInChildren<Renderer>();
            bounds = new Bounds(root.position, Vector3.zero);
            if (renderers.Length == 0)
            {
                return false;
            }

            bounds = renderers[0].bounds;
            for (var i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }
            return true;
        }

        private static void ApplyRuntimeTint(Transform root, Color tint)
        {
            foreach (var renderer in root.GetComponentsInChildren<Renderer>())
            {
                var material = renderer.material;
                material.color = Color.Lerp(material.color, tint, 0.42f);
            }
        }

        private static void TableLegs(Transform parent, float x = 0.55f, float z = 0.40f, float height = 0.62f)
        {
            Part(parent, "Leg_FL", PrimitiveType.Cube, new Vector3(-x, height * 0.5f, -z), new Vector3(0.12f, height, 0.12f), new Color(0.36f, 0.22f, 0.12f));
            Part(parent, "Leg_FR", PrimitiveType.Cube, new Vector3(x, height * 0.5f, -z), new Vector3(0.12f, height, 0.12f), new Color(0.36f, 0.22f, 0.12f));
            Part(parent, "Leg_BL", PrimitiveType.Cube, new Vector3(-x, height * 0.5f, z), new Vector3(0.12f, height, 0.12f), new Color(0.36f, 0.22f, 0.12f));
            Part(parent, "Leg_BR", PrimitiveType.Cube, new Vector3(x, height * 0.5f, z), new Vector3(0.12f, height, 0.12f), new Color(0.36f, 0.22f, 0.12f));
        }

        private static GameObject Part(Transform parent, string name, PrimitiveType primitive, Vector3 localPosition, Vector3 localScale, Color color)
        {
            var part = GameObject.CreatePrimitive(primitive);
            part.name = name;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPosition;
            part.transform.localScale = localScale;
            var renderer = part.GetComponent<Renderer>();
            if (renderer != null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null)
                {
                    shader = Shader.Find("Standard");
                }

                renderer.material = new Material(shader) { color = color };
            }
            return part;
        }
    }
}
