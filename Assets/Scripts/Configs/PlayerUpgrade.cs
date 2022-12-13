using System;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "PlayerUpgrade", menuName = "Configs/PlayerUpgrade", order = 0)]
    public class PlayerUpgrade : ScriptableObject
    {
        private const string SPEED_UP_KEY = "SpeedUp";
        private const string COINS_UP_KEY = "CoinsUp";
        private const string SCALE_UP_KEY = "ScaleUp";

        [SerializeField] private UpgradeItem[] speedUp;
        [SerializeField] private UpgradeItem[] coinsUp;
        [SerializeField] private UpgradeItem[] scaleUp;

        public UpgradeItem[] SpeedUp => speedUp;

        public UpgradeItem[] CoinsUp => coinsUp;

        public UpgradeItem[] ScaleUp => scaleUp;

        public int SpeedUpLevel
        {
            get => PlayerPrefs.GetInt(SPEED_UP_KEY, 0);
            set => PlayerPrefs.SetInt(SPEED_UP_KEY, value);
        }

        public int ScaleUpLevel
        {
            get => PlayerPrefs.GetInt(SCALE_UP_KEY, 0);
            set => PlayerPrefs.SetInt(SCALE_UP_KEY, value);
        }

        public int CoinsUpLevel
        {
            get => PlayerPrefs.GetInt(COINS_UP_KEY, 0);
            set => PlayerPrefs.SetInt(COINS_UP_KEY, value);
        }
    }

    [Serializable]
    public class UpgradeItem
    {
        [SerializeField] private float upgradeValue;
        [SerializeField] private int upgradeCost;

        public float UpgradeValue => upgradeValue;

        public int UpgradeCost => upgradeCost;
    }
}