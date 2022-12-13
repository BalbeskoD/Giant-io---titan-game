using System;
using System.Linq;
using Configs;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;
using Zenject.Signals;

namespace UI
{
    public class LostPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI rewardText;
        [SerializeField] private Button restartButton;

        [SerializeField] private ResultItem resultItem;
        [SerializeField] private Transform result;
        [SerializeField] private Color playerColor;
        private SignalBus _signalBus;

        private int _rewardCount;
        private PlayerUpgrade _playerUpgrade;

        [Inject]
        public void Construct(SignalBus signalBus, PlayerUpgrade playerUpgrade)
        {
            _signalBus = signalBus;
            _playerUpgrade = playerUpgrade;
        }

        private void Awake()
        {
            restartButton = GetComponentInChildren<Button>();
            restartButton.onClick.AddListener(Restart);
            _signalBus.Subscribe<PlayerFailSignal>(OnLevelLose);
        }

        private void OnDestroy()
        {
            restartButton.onClick.RemoveAllListeners();
            _signalBus.Unsubscribe<PlayerFailSignal>(OnLevelLose);
        }

        private void Restart()
        {
            _signalBus.Fire<GameRestartSignal>();
            _signalBus.Fire(new GetRewardSignal() {RewardCount = _rewardCount});
        }

        private void OnLevelLose(PlayerFailSignal signal)
        {
            _rewardCount = (int) (signal.LevelResult["Player"] / 2 * _playerUpgrade.CoinsUp[_playerUpgrade.ScaleUpLevel].UpgradeValue);
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