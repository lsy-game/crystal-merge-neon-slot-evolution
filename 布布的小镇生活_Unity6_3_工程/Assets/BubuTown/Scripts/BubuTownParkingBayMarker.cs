using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownParkingBayMarker : MonoBehaviour
    {
        public string BayId;
        public string ZoneId;
        public bool IsEvCharging;
        public bool IsVisitorReserved;
        public bool IsOccupiedAtPrototypeStart;
        public bool IsQuestReady;
        public Transform VehicleSpawnPoint;
    }
}
