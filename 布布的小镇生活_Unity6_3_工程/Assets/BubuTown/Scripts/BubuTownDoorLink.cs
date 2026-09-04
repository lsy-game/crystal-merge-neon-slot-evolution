using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownDoorLink : MonoBehaviour
    {
        public Transform TargetPoint;
        public Vector3 TargetOffset = Vector3.zero;
        public string MessageAfterUse = "已切换区域。";
    }
}
