using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownNavigationAnchor : MonoBehaviour
    {
        public string AnchorId;
        public string DisplayName;
        public string ZoneId;
        public string Category;
        public string[] ConnectedAnchorIds = new string[0];
        public bool IsTransitNode;
        public bool IsUndergroundAccess;
        public bool IsQuestRelevant;
        public Vector3 WorldTarget;
    }
}
