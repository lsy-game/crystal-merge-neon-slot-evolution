using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownDecorationGrid : MonoBehaviour
    {
        public Vector2Int GridSize = new Vector2Int(8, 6);
        public float CellSize = 1f;
        public bool SnapToGrid = true;
        public bool RotateInNinetyDegreeSteps = true;
        public bool SavePlacedFurniture = true;
        public Transform PreviewRoot;
        public int PreviewRotationQuarterTurns;
        public int PreviewPlacementIndex;
        public string PreviewFurnitureId;

        public int CellCount
        {
            get { return Mathf.Max(1, GridSize.x) * Mathf.Max(1, GridSize.y); }
        }

        public void RotatePreviewClockwise()
        {
            PreviewRotationQuarterTurns = (PreviewRotationQuarterTurns + 1) % 4;
            if (PreviewRoot != null)
            {
                PreviewRoot.localRotation = Quaternion.Euler(0f, PreviewRotationQuarterTurns * 90f, 0f);
            }
        }

        public void ShowPreview(string furnitureId, int placementIndex)
        {
            if (PreviewRoot == null)
            {
                return;
            }

            ClearPreview();
            PreviewFurnitureId = furnitureId;
            PreviewPlacementIndex = ClampPlacementIndex(placementIndex);
            if (string.IsNullOrEmpty(furnitureId))
            {
                PreviewRoot.gameObject.SetActive(false);
                return;
            }

            PreviewRoot.gameObject.SetActive(true);
            PreviewRoot.localPosition = LocalPositionForIndex(PreviewPlacementIndex);
            PreviewRoot.localRotation = Quaternion.Euler(0f, PreviewRotationQuarterTurns * 90f, 0f);
            var preview = BubuTownFurnitureVisuals.CreatePlacedFurniture(furnitureId, PreviewRoot, Vector3.zero);
            preview.name = "Preview_" + furnitureId;
            if (BubuTownFurnitureVisuals.GetPlacementKind(furnitureId) == BubuTownFurnitureVisuals.PlacementKind.WallMounted)
            {
                SnapWallPreviewRoot();
            }
            TintPreview(preview.transform, IsPlacementValid(furnitureId, PreviewPlacementIndex, PreviewRotationQuarterTurns));
        }

        public void MovePreview(int columnDelta, int rowDelta)
        {
            var columns = Mathf.Max(1, GridSize.x);
            var rows = Mathf.Max(1, GridSize.y);
            var row = PreviewPlacementIndex / columns;
            var col = PreviewPlacementIndex % columns;
            col = Mathf.Clamp(col + columnDelta, 0, columns - 1);
            row = Mathf.Clamp(row + rowDelta, 0, rows - 1);
            ShowPreview(PreviewFurnitureId, row * columns + col);
        }

        public void ClearPreview()
        {
            if (PreviewRoot == null)
            {
                return;
            }

            for (var i = PreviewRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(PreviewRoot.GetChild(i).gameObject);
            }
        }

        public Vector3 LocalPositionForIndex(int index)
        {
            index = ClampPlacementIndex(index);
            var columns = Mathf.Max(1, GridSize.x);
            var row = index / columns;
            var col = index % columns;
            return new Vector3((col - GridSize.x * 0.5f + 0.5f) * CellSize, 0.04f, (row - GridSize.y * 0.5f + 0.5f) * CellSize);
        }

        public bool IsPreviewPlacementValid()
        {
            return IsPlacementValid(PreviewFurnitureId, PreviewPlacementIndex, PreviewRotationQuarterTurns);
        }

        public bool IsPlacementValid(string furnitureId, int index, int quarterTurns)
        {
            if (string.IsNullOrEmpty(furnitureId) || !FootprintFitsGrid(furnitureId, index, quarterTurns))
            {
                return false;
            }

            var placementKind = BubuTownFurnitureVisuals.GetPlacementKind(furnitureId);
            if (placementKind == BubuTownFurnitureVisuals.PlacementKind.Floor)
            {
                return true;
            }

            return FootprintTouchesWall(furnitureId, index, quarterTurns);
        }

        private bool FootprintFitsGrid(string furnitureId, int index, int quarterTurns)
        {
            var rect = FootprintRect(furnitureId, index, quarterTurns);
            return rect.xMin >= 0 && rect.yMin >= 0 && rect.xMax <= Mathf.Max(1, GridSize.x) && rect.yMax <= Mathf.Max(1, GridSize.y);
        }

        private bool FootprintTouchesWall(string furnitureId, int index, int quarterTurns)
        {
            var rect = FootprintRect(furnitureId, index, quarterTurns);
            return rect.xMin <= 0 || rect.yMin <= 0 || rect.xMax >= Mathf.Max(1, GridSize.x) || rect.yMax >= Mathf.Max(1, GridSize.y);
        }

        private RectInt FootprintRect(string furnitureId, int index, int quarterTurns)
        {
            var columns = Mathf.Max(1, GridSize.x);
            var rows = Mathf.Max(1, GridSize.y);
            index = Mathf.Clamp(index, 0, columns * rows - 1);
            var row = index / columns;
            var col = index % columns;
            var footprint = BubuTownFurnitureVisuals.FootprintCells(furnitureId);
            if (Mathf.Abs(quarterTurns) % 2 == 1)
            {
                footprint = new Vector2Int(footprint.y, footprint.x);
            }

            return new RectInt(col - footprint.x / 2, row - footprint.y / 2, footprint.x, footprint.y);
        }

        private void SnapWallPreviewRoot()
        {
            if (PreviewRoot == null)
            {
                return;
            }

            var local = PreviewRoot.localPosition;
            var halfWidth = GridSize.x * CellSize * 0.5f;
            var halfDepth = GridSize.y * CellSize * 0.5f;
            var west = Mathf.Abs(local.x + halfWidth);
            var east = Mathf.Abs(local.x - halfWidth);
            var south = Mathf.Abs(local.z + halfDepth);
            var north = Mathf.Abs(local.z - halfDepth);
            var min = Mathf.Min(Mathf.Min(west, east), Mathf.Min(south, north));
            local.y = 1.55f;
            if (min == west)
            {
                local.x = -halfWidth + 0.05f;
                PreviewRoot.localRotation = Quaternion.Euler(0f, 90f, 0f);
            }
            else if (min == east)
            {
                local.x = halfWidth - 0.05f;
                PreviewRoot.localRotation = Quaternion.Euler(0f, -90f, 0f);
            }
            else if (min == south)
            {
                local.z = -halfDepth + 0.05f;
                PreviewRoot.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else
            {
                local.z = halfDepth - 0.05f;
                PreviewRoot.localRotation = Quaternion.Euler(0f, 180f, 0f);
            }

            PreviewRoot.localPosition = local;
        }

        private void TintPreview(Transform root, bool isValid)
        {
            var tint = isValid ? new Color(0.38f, 0.82f, 0.55f, 0.62f) : new Color(1f, 0.28f, 0.22f, 0.72f);
            foreach (var renderer in root.GetComponentsInChildren<Renderer>())
            {
                var material = renderer.material;
                material.color = Color.Lerp(material.color, tint, 0.55f);
            }
        }

        private int ClampPlacementIndex(int index)
        {
            return Mathf.Clamp(index, 0, CellCount - 1);
        }
    }
}
