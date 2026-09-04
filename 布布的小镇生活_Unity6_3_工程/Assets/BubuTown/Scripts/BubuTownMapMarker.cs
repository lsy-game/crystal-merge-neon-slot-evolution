using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownMapMarker : MonoBehaviour
    {
        public string MarkerId;
        public string DisplayName;
        public string Category;
        public Vector3 WorldPosition;
        public Color MarkerColor = Color.white;
        [TextArea(2, 4)] public string Description;
    }
}
