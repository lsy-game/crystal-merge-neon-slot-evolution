using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownSaveBootstrap : MonoBehaviour
    {
        public BubuTownGameState State;
        public BubuTownDecorationGrid HomeGrid;

        private void Awake()
        {
            if (State != null)
            {
                State.LoadState();
                foreach (var settings in FindObjectsOfType<BubuTownSettingsSystem>())
                {
                    settings.Apply();
                }
            }
        }

        private void Start()
        {
            RestorePlacedFurniture();
        }

        private void OnApplicationQuit()
        {
            if (State != null)
            {
                State.SaveState();
            }
        }

        private void RestorePlacedFurniture()
        {
            if (State == null || HomeGrid == null)
            {
                return;
            }

            for (var i = 0; i < State.PlacedFurnitureIds.Count; i++)
            {
                var furnitureId = State.PlacedFurnitureIds[i];
                if (HomeGrid.transform.Find("Placed_" + furnitureId) != null)
                {
                    continue;
                }

                CreatePlacedFurnitureVisual(HomeGrid.transform, furnitureId, HomeGrid.LocalPositionForIndex(State.PlacedFurnitureCellAt(i)), State.PlacedFurnitureRotationAt(i));
            }
        }

        public static GameObject CreatePlacedFurnitureVisual(Transform parent, string furnitureId, Vector3 localPosition)
        {
            return CreatePlacedFurnitureVisual(parent, furnitureId, localPosition, 0);
        }

        public static GameObject CreatePlacedFurnitureVisual(Transform parent, string furnitureId, Vector3 localPosition, int rotationQuarterTurns)
        {
            var visual = BubuTownFurnitureVisuals.CreatePlacedFurniture(furnitureId, parent, localPosition);
            visual.transform.localRotation = Quaternion.Euler(0f, Mathf.Clamp(rotationQuarterTurns, 0, 3) * 90f, 0f);
            return visual;
        }
    }
}
