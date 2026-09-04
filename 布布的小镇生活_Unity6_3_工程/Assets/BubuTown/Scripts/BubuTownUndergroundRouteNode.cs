using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownUndergroundRouteNode : MonoBehaviour
    {
        public string NodeId;
        public string DisplayName;
        public string ZoneId;
        public string[] ConnectedNodeIds = new string[0];
        public bool IsTransferPoint;
        public bool IsQuestReady;
    }
}
