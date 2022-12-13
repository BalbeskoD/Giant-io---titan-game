using System;
using Configs;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Zenject.Signals;

namespace UI
{
    public enum UpgradeType
    {
        None,
        Speed,
        Scale,
        Reward
    }

    public class UpgradeButton : MonoBehaviour
    {
        [SerializeField] private UpgradeType type;
        [SerializeField] private TextMeshProUGUI upgradeText;
        [SerializeField] private TextMeshProUGUI upgradeCost;
        private SignalBus _signalBus;
        private PlayerUpgrade _config;
        private GameManager _gameManager;
        private Button _button;
        private UpgradeItem _upgradeItem;

        [Inject]
        public void Construct(SignalBus signalBus, PlayerUpgrade config, GameManager gameManager)
        {
            _signalBus = signalBus;
            _config = config;
            _gameManager = gameManager;
        }

        private void Awake()
        {
            _button = GetComponentInChildren<Button>();
            _button.onClick.AddListener(Upgrade);
            CheckButtonState();
            _signalBus.Subscribe<CoinsUpdateSignal>(OnCoinsChange);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(Upgrade);
            _signalBus.Unsubscribe<CoinsUpdateSignal>(OnCoinsChange);
        }

        private void Upgrade()
        {
            switch (type)
            {
                case UpgradeType.None:
                    Debug.LogWarning("Wrong upgrade type!");
                    break;
                case UpgradeType.Speed:
                    _config.SpeedUpLevel++;
                    break;
                case UpgradeType.Scale:
                    _config.ScaleUpLevel++;
                    break;
                case UpgradeType.Reward:
                    _config.CoinsUpLevel++;
                    break;
            }

            _signalBus.Fire(new PlayerUpgradeSignal() {UpgradeItem = _upgradeItem});
            CheckButtonState();
        }

        private void CheckButtonState()
        {
            bool isActive = false;
            switch (type)
            {
                case UpgradeType.None:
                    Debug.LogWarning("Wrong upgrade type!");
                    break;
                case UpgradeType.Speed:
                    isActive = _config.SpeedUp.Length - 1 > _config.SpeedUpLevel;
                    break;
                case UpgradeType.Scale:
                    isActive = _config.ScaleUp.Length - 1 > _config.SpeedUpLevel;
                    break;
                case UpgradeType.Reward:
                    isActive = _config.CoinsUp.Length - 1 > _config.CoinsUpLevel;
                    break;
            }

            _button.interactable = isActive;
            if (!isActive)
            {
                _upgradeItem = null;
                upgradeCost.text = string.Empty;
                upgradeText.text = "Max level";
            }
            else
            {
                Setup();
            }
        }

        private void Setup()
        {
            switch (type)
            {
                case UpgradeType.None:
                    Debug.LogWarning("Wrong upgrade type!");
                    break;
                case UpgradeType.Speed:
                    _upgradeItem = _config.SpeedUp[_config.SpeedUpLevel + 1];
                    break;
                case UpgradeType.Scale:
                    _upgradeItem = _config.ScaleUp[_config.ScaleUpLevel + 1];
                    break;
                case UpgradeType.Reward:
                    _upgradeItem = _config.CoinsUp[_config.CoinsUpLevel + 1];
                    break;
            }

            upgradeCost.text = _upgradeItem.UpgradeCost.ToString();
            upgradeText.text = $"x{_upgradeItem.UpgradeValue}";
            _button.interactable = _upgradeItem.UpgradeCost <= _gameManager.Coins;
        }

        private void OnCoinsChange(CoinsUpdateSignal signal)
        {
            CheckButtonState();
        }
    }
}