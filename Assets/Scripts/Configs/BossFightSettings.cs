using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "BossFightSettings", menuName = "Configs/BossFightSettings", order = 0)]
    public class BossFightSettings : ScriptableObject
    {
        [SerializeField] private int hitCountToWin = 5;
        [SerializeField] private float kickDelay = 1.2f;
        [SerializeField] private float hitDelay = 0.5f;
        [SerializeField] private float bossHitOffset = 0.3f;
        [SerializeField] private int winPoints = 100;
        [SerializeField] private float endDelay = 1.5f;
        [SerializeField] private float partsDestroyTime = 1f;

        public int HitCountToWin => hitCountToWin;

        public float KickDelay => kickDelay;

        public float BossHitOffset => bossHitOffset;

        public float HitDelay => hitDelay;

        public int WinPoints => winPoints;

        public float EndDelay => endDelay;
        public float PartsDestroyTime => partsDestroyTime;
    }
}