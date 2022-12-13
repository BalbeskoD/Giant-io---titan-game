using System;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "PlayerSettings", menuName = "Configs/PlayerSettings", order = 0)]
    public class PlayerSettings : ScriptableObject
    {
        [SerializeField] private Vector3 playerBossScale = new Vector3(10, 10, 10);
        [SerializeField] private float rotationSpeed = 10;
        [SerializeField] private float scaleFactor = 1.1f;
        [SerializeField] private bool allTimeMove;
        [SerializeField] private int[] levelProgress;
        [SerializeField] private Vector3[] cameraOffset;
        [SerializeField] private float[] levelSpeed;
        [SerializeField] private LevelProgressItem[] levelProgressItems;
       

        public float RotationSpeed => rotationSpeed;
        public float ScaleFactor => scaleFactor;

        public bool AllTimeMove => allTimeMove;

        public int[] LevelProgress => levelProgress;

        public Vector3[] CameraOffset => cameraOffset;

        public float[] LevelSpeed => levelSpeed;

        public LevelProgressItem[] LevelProgressItems
        {
            get => levelProgressItems;
            set => levelProgressItems = value;
        }

        public Vector3 PlayerBossScale => playerBossScale;
    }

    [Serializable]
    public class LevelProgressItem
    {
        [SerializeField] private int levelProgress;
        [SerializeField] private Vector3 levelCameraOffset;
        [SerializeField] private float levelSpeed;

        public int LevelProgress
        {
            get => levelProgress;
            set => levelProgress = value;
        }

        public Vector3 LevelCameraOffset
        {
            get => levelCameraOffset;
            set => levelCameraOffset = value;
        }

        public float LevelSpeed
        {
            get => levelSpeed;
            set => levelSpeed = value;
        }
    }
}