using System;
using System.Linq;
using Configs;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;
using Zenject.Signals;

namespace UI
{
    public class WinPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI rewardText;
        [SerializeField] private Button restartButton;

        [SerializeField] private ResultItem resultItem;
        [SerializeField] private Transform result;
        [SerializeField] private Color playerColor;
        
        private SignalBus _signalBus;

        private int _rewardCount;
        private PlayerUpgrade _playerUpgrade;
        private GameManager _gameManager;
        private GameSettings _gameSettings;

        [Inject]
        public void Construct(SignalBus signalBus, PlayerUpgrade playerUpgrade, GameManager gameManager, GameSettings gameSettings)
        {
            _signalBus = signalBus;
            _playerUpgrade = playerUpgrade;
            _gameManager = gameManager;
            _gameSettings = gameSettings;
        }

        private void Awake()
        {
            restartButton = GetComponentInChildren<Button>();
            restartButton.onClick.AddListener(Restart);
            _signalBus.Subscribe<LevelWinSignal>(OnLevelWin);
        }

        private void OnDestroy()
        {
            restartButton.onClick.RemoveAllListeners();
            _signalBus.Unsubscribe<LevelWinSignal>(OnLevelWin);
        }

        private void Restart()
        {
            _signalBus.Fire(new GetRewardSignal() {RewardCount = _rewardCount});
            _signalBus.Fire<GameRestartSignal>();
        }

        private void OnLevelWin(LevelWinSignal signal)
        {
            if (_gameManager.Level == 1)
            {
                _rewardCount = _gameSettings.PlayerFirstReward;
            }
            else
            {
                _rewardCount = (int) (signal.LevelResult["Player"] / 2 * _playerUpgrade.CoinsUp[_playerUpgrade.ScaleUpLevel].UpgradeValue);
            }

            rewardText.text = _rewardCount.ToString();
            var data = signal.LevelResult;

            var orderByDescending = data.OrderByDescending(x => x.Value);
            var counter = 0;
            foreach (Transform child in result)
            {
                Destroy(child.gameObject);
            }

            foreach (var pair in orderByDescending)
            {
                var result = Instantiate(resultItem, this.result);
                result.Setup(pair.Key, pair.Value, playerColor);
                if (++counter > 6)
                {
                    break;
                }
            }
        }
    }
}