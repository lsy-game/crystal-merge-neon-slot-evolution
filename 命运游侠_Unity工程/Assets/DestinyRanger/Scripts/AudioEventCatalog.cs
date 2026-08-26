using UnityEngine;

namespace DestinyRanger
{
    [CreateAssetMenu(menuName = "Destiny Ranger/Audio Event Catalog")]
    public sealed class AudioEventCatalog : ScriptableObject
    {
        [Header("Slot Machine")]
        public AudioClip slotActivate;
        public AudioClip slotReelLoop;
        public AudioClip slotStopClick;
        public AudioClip slotPerfectLine;
        public AudioClip slotPartialLine;
        public AudioClip slotPenaltyLine;

        [Header("Combat")]
        public AudioClip slashWave;
        public AudioClip magicProjectile;
        public AudioClip healColumn;
        public AudioClip shieldActivate;
        public AudioClip[] enemyHitVariants;
        public AudioClip[] enemyDeathVariants;
        public AudioClip bossEntrance;

        [Header("UI")]
        public AudioClip buttonClick;
        public AudioClip popupOpen;
        public AudioClip coinGain;
        public AudioClip itemGain;
        public AudioClip victorySting;
        public AudioClip cancelBack;

        [Header("Ambient")]
        public AudioClip chamberAmbient;
        public AudioClip forestAmbient;
    }
}
