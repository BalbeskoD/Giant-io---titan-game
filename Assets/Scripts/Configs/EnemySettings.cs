using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Configs
{
    [CreateAssetMenu(fileName = "EnemySettings", menuName = "Configs/EnemySettings", order = 0)]
    public class EnemySettings : ScriptableObject
    {
        [SerializeField] private string[] names;
        [SerializeField] private Color[] colors;
        [SerializeField] private EnemyLevelProgressItem[] levelProgressItems;
        [SerializeField] private float scaleFactor;
        public float ScaleFactor => scaleFactor;
        public EnemyLevelProgressItem[] LevelProgressItems => levelProgressItems;

        public string[] Names => names;

        public Color[] Colors => colors;
    }

    [Serializable]
    public class EnemyLevelProgressItem
    {
        [SerializeField] private int levelProgress;
        [SerializeField] private float levelSpeed;

        public int LevelProgress => levelProgress;

        public float LevelSpeed => levelSpeed;
    }
}