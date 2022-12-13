using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Configs/GameSettings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        [SerializeField] private int targetFps = 60;
        [SerializeField] private bool multiTouchEnable = false;
        [SerializeField] private int levelSessionTime = 90;
        [SerializeField] private int pointsForTitan = 20;
        [SerializeField] private int playerBossLevel = 6;
        [SerializeField] private int playerFirstReward = 75;

        public int TargetFps => targetFps;

        public bool MultiTouchEnable => multiTouchEnable;

        public int LevelSessionTime => levelSessionTime;

        public int PointsForTitan => pointsForTitan;
        public int PlayerBossLevel => playerBossLevel;

        public int PlayerFirstReward => playerFirstReward;
    }
}