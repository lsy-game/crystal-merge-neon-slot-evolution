using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BubuTown
{
    public sealed class BubuTownInteriorDecorationPrototype : MonoBehaviour
    {
        [System.Serializable]
        public sealed class FinishSet
        {
            public string DisplayName;
            public Material WallMaterial;
            public Material FloorMaterial;
            public Color LightColor = Color.white;
            public float LightIntensity = 1f;
        }

        private sealed class PlacedFurnitureRecord
        {
            public GameObject Instance;
            public string FurnitureId;
            public int AnchorIndex;
            public int QuarterTurns;
            public readonly List<int> Cells = new List<int>();
        }

        public Transform Player;
        public Transform PlacementRoot;
        public Transform PreviewRoot;
        public Renderer[] WallRenderers;
        public Renderer FloorRenderer;
        public Light[] RoomLights;
        public Text HudText;
        public Vector2Int GridSize = new Vector2Int(8, 6);
        public float CellSize = 0.75f;
        public bool GridVisible = true;
        public FinishSet[] FinishSets = new FinishSet[0];

        private readonly List<GameObject> gridLines = new List<GameObject>();
        private readonly HashSet<int> occupiedCells = new HashSet<int>();
        private readonly List<PlacedFurnitureRecord> placedRecords = new List<PlacedFurnitureRecord>();
        private readonly List<string> furnitureIds = new List<string>
        {
            "fur_target_bed",
            "fur_target_nightstand",
            "fur_target_sofa",
            "fur_target_coffee_table",
            "fur_target_kitchen",
            "fur_target_lamp",
            "fur_target_boxes",
            "fur_pink_bed",
            "fur_pink_vanity",
            "fur_natural_bed",
            "fur_natural_table",
            "fur_modern_sofa",
            "fur_modern_floor_lamp",
            "fur_cake_bed",
            "fur_cake_wall_shelf",
            "fur_esports_desk",
            "fur_esports_wall_poster"
        };

        private int furnitureIndex;
        private int placementIndex = 19;
        private int rotationQuarterTurns;
        private int finishIndex;
        private int placedCount;
        private string lastMessage = string.Empty;

        private void Start()
        {
            CreateGridLines();
            ApplyFinish();
            RefreshPreview();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveSelection(-1, 0);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveSelection(1, 0);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveSelection(0, 1);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveSelection(0, -1);
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                rotationQuarterTurns = (rotationQuarterTurns + 1) % 4;
                RefreshPreview();
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                CycleFurniture(-1);
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                CycleFurniture(1);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                PlaceCurrentFurniture();
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                UndoLastPlaced();
            }
            if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
            {
                DeleteFurnitureAtSelection();
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                PickUpFurnitureAtSelection();
            }
            if (Input.GetMouseButtonDown(0))
            {
                SetPlacementIndexFromMouse();
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                GridVisible = !GridVisible;
                foreach (var line in gridLines)
                {
                    if (line != null)
                    {
                        line.SetActive(GridVisible);
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                CycleFinish(1, true, false, false);
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                CycleFinish(1, false, true, false);
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                CycleFinish(1, false, false, true);
            }

            UpdateHud();
        }

        private void MoveSelection(int columnDelta, int rowDelta)
        {
            var columns = Mathf.Max(1, GridSize.x);
            var rows = Mathf.Max(1, GridSize.y);
            var row = placementIndex / columns;
            var col = placementIndex % columns;
            col = Mathf.Clamp(col + columnDelta, 0, columns - 1);
            row = Mathf.Clamp(row + rowDelta, 0, rows - 1);
            placementIndex = row * columns + col;
            RefreshPreview();
        }

        private void CycleFurniture(int direction)
        {
            furnitureIndex = PositiveModulo(furnitureIndex + direction, furnitureIds.Count);
            RefreshPreview();
        }

        private void CycleFinish(int direction, bool walls, bool floor, bool light)
        {
            if (FinishSets == null || FinishSets.Length == 0)
            {
                return;
            }

            finishIndex = PositiveModulo(finishIndex + direction, FinishSets.Length);
            ApplyFinish(walls, floor, light);
        }

        private void ApplyFinish()
        {
            ApplyFinish(true, true, true);
        }

        private void ApplyFinish(bool walls, bool floor, bool light)
        {
            if (FinishSets == null || FinishSets.Length == 0)
            {
                return;
            }

            var finish = FinishSets[Mathf.Clamp(finishIndex, 0, FinishSets.Length - 1)];
            if (walls && WallRenderers != null)
            {
                foreach (var renderer in WallRenderers)
                {
                    if (renderer != null && finish.WallMaterial != null)
                    {
                        renderer.sharedMaterial = finish.WallMaterial;
                    }
                }
            }
            if (floor && FloorRenderer != null && finish.FloorMaterial != null)
            {
                FloorRenderer.sharedMaterial = finish.FloorMaterial;
            }
            if (light && RoomLights != null)
            {
                foreach (var roomLight in RoomLights)
                {
                    if (roomLight == null)
                    {
                        continue;
                    }

                    roomLight.color = finish.LightColor;
                    roomLight.intensity = finish.LightIntensity;
                }
            }
        }

        private void PlaceCurrentFurniture()
        {
            if (PlacementRoot == null)
            {
                return;
            }

            var furnitureId = furnitureIds[Mathf.Clamp(furnitureIndex, 0, furnitureIds.Count - 1)];
            var coveredCells = CoveredCells(furnitureId, placementIndex, rotationQuarterTurns);
            if (!IsPlacementValid(furnitureId, placementIndex, rotationQuarterTurns, coveredCells))
            {
                SetMessage("当前位置不能摆放");
                RefreshPreview();
                return;
            }

            var placed = BubuTownFurnitureVisuals.CreatePlacedFurniture(furnitureId, PlacementRoot, LocalPositionForIndex(placementIndex));
            placed.name = "已摆放_" + placedCount + "_" + FurnitureDisplayName(furnitureId);
            placed.transform.localRotation = Quaternion.Euler(0f, rotationQuarterTurns * 90f, 0f);
            if (BubuTownFurnitureVisuals.GetPlacementKind(furnitureId) == BubuTownFurnitureVisuals.PlacementKind.WallMounted)
            {
                SnapWallFurniture(placed.transform);
            }

            var record = new PlacedFurnitureRecord
            {
                Instance = placed,
                FurnitureId = furnitureId,
                AnchorIndex = placementIndex,
                QuarterTurns = rotationQuarterTurns
            };
            record.Cells.AddRange(coveredCells);
            placedRecords.Add(record);

            foreach (var cell in coveredCells)
            {
                occupiedCells.Add(cell);
            }
            placedCount++;
            SetMessage("已摆放: " + FurnitureDisplayName(furnitureId));
            CycleFurniture(1);
        }

        private void UndoLastPlaced()
        {
            if (placedRecords.Count == 0)
            {
                SetMessage("还没有可撤销的家具");
                return;
            }

            var record = placedRecords[placedRecords.Count - 1];
            RemoveRecord(record);
            SetMessage("已撤销: " + FurnitureDisplayName(record.FurnitureId));
            RefreshPreview();
        }

        private void DeleteFurnitureAtSelection()
        {
            var record = FindRecordAtCell(placementIndex);
            if (record == null)
            {
                SetMessage("当前格没有可删除家具");
                return;
            }

            RemoveRecord(record);
            SetMessage("已删除: " + FurnitureDisplayName(record.FurnitureId));
            RefreshPreview();
        }

        private void PickUpFurnitureAtSelection()
        {
            var record = FindRecordAtCell(placementIndex);
            if (record == null)
            {
                SetMessage("当前格没有可拾起家具");
                return;
            }

            var pickedFurnitureId = record.FurnitureId;
            var pickedQuarterTurns = record.QuarterTurns;
            RemoveRecord(record);
            furnitureIndex = Mathf.Max(0, furnitureIds.IndexOf(pickedFurnitureId));
            rotationQuarterTurns = pickedQuarterTurns;
            SetMessage("已拾起，可移动后重新摆放: " + FurnitureDisplayName(pickedFurnitureId));
            RefreshPreview();
        }

        private PlacedFurnitureRecord FindRecordAtCell(int cellIndex)
        {
            for (var i = placedRecords.Count - 1; i >= 0; i--)
            {
                var record = placedRecords[i];
                if (record != null && record.Cells.Contains(cellIndex))
                {
                    return record;
                }
            }

            return null;
        }

        private void RemoveRecord(PlacedFurnitureRecord record)
        {
            if (record == null)
            {
                return;
            }

            foreach (var cell in record.Cells)
            {
                occupiedCells.Remove(cell);
            }
            placedRecords.Remove(record);
            if (record.Instance != null)
            {
                Destroy(record.Instance);
            }
        }

        private void SetPlacementIndexFromMouse()
        {
            if (!TryRaycastGridIndex(out var gridIndex))
            {
                return;
            }

            placementIndex = gridIndex;
            var columns = Mathf.Max(1, GridSize.x);
            SetMessage("已选中格子: " + (gridIndex % columns + 1) + "," + (gridIndex / columns + 1));
            RefreshPreview();
        }

        private bool TryRaycastGridIndex(out int gridIndex)
        {
            gridIndex = placementIndex;
            if (PlacementRoot == null || Camera.main == null)
            {
                return false;
            }

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var placementPlane = new Plane(PlacementRoot.up, PlacementRoot.position);
            if (!placementPlane.Raycast(ray, out var distance))
            {
                return false;
            }

            var localHit = PlacementRoot.InverseTransformPoint(ray.GetPoint(distance));
            var columns = Mathf.Max(1, GridSize.x);
            var rows = Mathf.Max(1, GridSize.y);
            var halfWidth = columns * CellSize * 0.5f;
            var halfDepth = rows * CellSize * 0.5f;
            var normalizedX = localHit.x + halfWidth;
            var normalizedZ = localHit.z + halfDepth;
            if (normalizedX < 0f || normalizedX >= columns * CellSize || normalizedZ < 0f || normalizedZ >= rows * CellSize)
            {
                return false;
            }

            var col = Mathf.Clamp(Mathf.FloorToInt(normalizedX / CellSize), 0, columns - 1);
            var row = Mathf.Clamp(Mathf.FloorToInt(normalizedZ / CellSize), 0, rows - 1);
            gridIndex = row * columns + col;
            return true;
        }

        private void RefreshPreview()
        {
            if (PreviewRoot == null || furnitureIds.Count == 0)
            {
                return;
            }

            for (var i = PreviewRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(PreviewRoot.GetChild(i).gameObject);
            }

            var furnitureId = furnitureIds[Mathf.Clamp(furnitureIndex, 0, furnitureIds.Count - 1)];
            PreviewRoot.localPosition = LocalPositionForIndex(placementIndex);
            PreviewRoot.localRotation = Quaternion.Euler(0f, rotationQuarterTurns * 90f, 0f);
            var preview = BubuTownFurnitureVisuals.CreatePlacedFurniture(furnitureId, PreviewRoot, Vector3.zero);
            preview.name = "预览_" + FurnitureDisplayName(furnitureId);
            var isValid = IsPlacementValid(furnitureId, placementIndex, rotationQuarterTurns);
            SetPreviewMaterial(preview.transform, isValid);
            if (BubuTownFurnitureVisuals.GetPlacementKind(furnitureId) == BubuTownFurnitureVisuals.PlacementKind.WallMounted)
            {
                SnapWallFurniture(PreviewRoot);
            }
        }

        private void SetPreviewMaterial(Transform root, bool isValid)
        {
            var tint = isValid ? new Color(0.38f, 0.82f, 0.55f, 0.62f) : new Color(1f, 0.28f, 0.22f, 0.72f);
            foreach (var renderer in root.GetComponentsInChildren<Renderer>())
            {
                var material = renderer.material;
                material.color = Color.Lerp(material.color, tint, 0.55f);
            }
        }

        private void SnapWallFurniture(Transform target)
        {
            var local = target.localPosition;
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
                target.localRotation = Quaternion.Euler(0f, 90f, 0f);
            }
            else if (min == east)
            {
                local.x = halfWidth - 0.05f;
                target.localRotation = Quaternion.Euler(0f, -90f, 0f);
            }
            else if (min == south)
            {
                local.z = -halfDepth + 0.05f;
                target.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else
            {
                local.z = halfDepth - 0.05f;
                target.localRotation = Quaternion.Euler(0f, 180f, 0f);
            }

            target.localPosition = local;
        }

        private void CreateGridLines()
        {
            if (PlacementRoot == null)
            {
                return;
            }

            var columns = Mathf.Max(1, GridSize.x);
            var rows = Mathf.Max(1, GridSize.y);
            var width = columns * CellSize;
            var depth = rows * CellSize;
            for (var x = 0; x <= columns; x++)
            {
                CreateGridLine(new Vector3(-width * 0.5f + x * CellSize, 0.018f, 0f), new Vector3(0.018f, 0.018f, depth));
            }
            for (var z = 0; z <= rows; z++)
            {
                CreateGridLine(new Vector3(0f, 0.02f, -depth * 0.5f + z * CellSize), new Vector3(width, 0.018f, 0.018f));
            }
        }

        private void CreateGridLine(Vector3 localPosition, Vector3 localScale)
        {
            var line = GameObject.CreatePrimitive(PrimitiveType.Cube);
            line.name = "运行时装修网格线";
            line.transform.SetParent(PlacementRoot, false);
            line.transform.localPosition = localPosition;
            line.transform.localScale = localScale;
            var renderer = line.GetComponent<Renderer>();
            if (renderer != null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null)
                {
                    shader = Shader.Find("Standard");
                }

                renderer.material = new Material(shader) { color = new Color(0.2f, 0.55f, 0.95f, 0.45f) };
            }
            line.SetActive(GridVisible);
            gridLines.Add(line);
        }

        private Vector3 LocalPositionForIndex(int index)
        {
            var columns = Mathf.Max(1, GridSize.x);
            index = Mathf.Clamp(index, 0, columns * Mathf.Max(1, GridSize.y) - 1);
            var row = index / columns;
            var col = index % columns;
            return new Vector3((col - GridSize.x * 0.5f + 0.5f) * CellSize, 0.04f, (row - GridSize.y * 0.5f + 0.5f) * CellSize);
        }

        private bool IsPlacementValid(string furnitureId, int index, int quarterTurns)
        {
            return IsPlacementValid(furnitureId, index, quarterTurns, CoveredCells(furnitureId, index, quarterTurns));
        }

        private bool IsPlacementValid(string furnitureId, int index, int quarterTurns, List<int> coveredCells)
        {
            if (coveredCells.Count == 0)
            {
                return false;
            }

            foreach (var cell in coveredCells)
            {
                if (occupiedCells.Contains(cell))
                {
                    return false;
                }
            }

            var placementKind = BubuTownFurnitureVisuals.GetPlacementKind(furnitureId);
            if (placementKind == BubuTownFurnitureVisuals.PlacementKind.Floor)
            {
                return true;
            }

            return TouchesWall(coveredCells);
        }

        private List<int> CoveredCells(string furnitureId, int index, int quarterTurns)
        {
            var columns = Mathf.Max(1, GridSize.x);
            var rows = Mathf.Max(1, GridSize.y);
            var row = Mathf.Clamp(index, 0, columns * rows - 1) / columns;
            var col = Mathf.Clamp(index, 0, columns * rows - 1) % columns;
            var footprint = BubuTownFurnitureVisuals.FootprintCells(furnitureId);
            if (Mathf.Abs(quarterTurns) % 2 == 1)
            {
                footprint = new Vector2Int(footprint.y, footprint.x);
            }

            var startCol = col - footprint.x / 2;
            var startRow = row - footprint.y / 2;
            var result = new List<int>(footprint.x * footprint.y);
            for (var dz = 0; dz < footprint.y; dz++)
            {
                for (var dx = 0; dx < footprint.x; dx++)
                {
                    var coveredCol = startCol + dx;
                    var coveredRow = startRow + dz;
                    if (coveredCol < 0 || coveredCol >= columns || coveredRow < 0 || coveredRow >= rows)
                    {
                        return new List<int>();
                    }

                    result.Add(coveredRow * columns + coveredCol);
                }
            }

            return result;
        }

        private bool TouchesWall(List<int> cells)
        {
            var columns = Mathf.Max(1, GridSize.x);
            var rows = Mathf.Max(1, GridSize.y);
            foreach (var cell in cells)
            {
                var row = cell / columns;
                var col = cell % columns;
                if (col == 0 || col == columns - 1 || row == 0 || row == rows - 1)
                {
                    return true;
                }
            }

            return false;
        }

        private int PositiveModulo(int value, int modulo)
        {
            return (value % modulo + modulo) % modulo;
        }

        private void SetMessage(string message)
        {
            lastMessage = message;
        }

        private void UpdateHud()
        {
            if (HudText == null || furnitureIds.Count == 0)
            {
                return;
            }

            var finishName = FinishSets != null && FinishSets.Length > 0 ? FinishSets[Mathf.Clamp(finishIndex, 0, FinishSets.Length - 1)].DisplayName : "默认";
            var columns = Mathf.Max(1, GridSize.x);
            var selectedFurniture = FindRecordAtCell(placementIndex);
            var selectedText = selectedFurniture == null ? "空格" : FurnitureDisplayName(selectedFurniture.FurnitureId);
            HudText.text =
                "窗口 C 室内装修原型\n" +
                "鼠标点格/方向键移格  E摆放  R旋转90度  Z/X换家具\n" +
                "Q撤销  M拾起重放  Delete删除  G网格  C墙纸  V地板  B灯光\n" +
                "当前家具: " + FurnitureDisplayName(furnitureIds[furnitureIndex]) + PlacementHint(furnitureIds[furnitureIndex]) + "\n" +
                "当前风格: " + finishName + "  已摆放: " + placedRecords.Count + "  选中格: " + (placementIndex % columns + 1) + "," + (placementIndex / columns + 1) + " / " + selectedText +
                (string.IsNullOrEmpty(lastMessage) ? string.Empty : "\n" + lastMessage);
        }

        private string PlacementHint(string furnitureId)
        {
            var placementKind = BubuTownFurnitureVisuals.GetPlacementKind(furnitureId);
            if (placementKind == BubuTownFurnitureVisuals.PlacementKind.WallMounted)
            {
                return " / 墙上";
            }
            if (placementKind == BubuTownFurnitureVisuals.PlacementKind.AgainstWall)
            {
                return " / 靠墙";
            }

            return " / 地面";
        }

        private string FurnitureDisplayName(string furnitureId)
        {
            switch (furnitureId)
            {
                case "fur_target_bed":
                    return "目标公寓_粉木单人床";
                case "fur_target_nightstand":
                    return "目标公寓_原木床头柜";
                case "fur_target_sofa":
                    return "目标公寓_海蓝小沙发";
                case "fur_target_coffee_table":
                    return "目标公寓_原木小茶几";
                case "fur_target_kitchen":
                    return "目标公寓_厨房组合";
                case "fur_target_lamp":
                    return "目标公寓_圆润台灯";
                case "fur_target_boxes":
                    return "目标公寓_搬家纸箱堆";
                case "fur_pink_bed":
                    return "可爱粉色床";
                case "fur_pink_vanity":
                    return "可爱梳妆台";
                case "fur_natural_bed":
                    return "木质自然床";
                case "fur_natural_table":
                    return "木质小餐桌";
                case "fur_modern_sofa":
                    return "现代简约沙发";
                case "fur_modern_floor_lamp":
                    return "现代落地灯";
                case "fur_cake_bed":
                    return "蛋糕主题床";
                case "fur_cake_wall_shelf":
                    return "蛋糕墙上搁板";
                case "fur_esports_desk":
                    return "电竞电脑桌";
                case "fur_esports_wall_poster":
                    return "电竞墙上海报";
                default:
                    return furnitureId;
            }
        }
    }
}
