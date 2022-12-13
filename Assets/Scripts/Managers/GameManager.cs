using System;
using System.Collections.Generic;
using System.Threading;
using Configs;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Enums;
using UI;
using UnityEngine;
using Zenject;
using Zenject.Signals;
using Random = UnityEngine.Random;

namespace Managers
{
    public class GameManager : IInitializable, IDisposable, ITickable
    {
        private const string COINS_KEY = "Coins";
        private const string LEVEL_KEY = "Level";
        private const string LEVEL_COUNT_KEY = "LevelCount";

        private readonly DiContainer _diContainer;
        private readonly GameSettings _gameSettings;
        private readonly PrefabsContainer _prefabsContainer;
        private readonly SignalBus _signalBus;
        private readonly PlayerUpgrade _playerUpgrade;
        private readonly EnemySettings _enemySettings;

        private Player.Player _player;
        private UI.UiController _ui;
        private CancellationTokenSource _cancellationTokenSource;
        private GameStates _gameStates;
        private List<string> _names;
        private List<Color> _colors;
        private float _timer;
        private bool _isGame;

        private GameStates GameState
        {
            get => _gameStates;
            set
            {
                if (_gameStates == value)
                    return;

                _gameStates = value;
                _ui.OnGameStateChange(_gameStates);
                _player.OnGameStateChange(_gameStates);
                switch (_gameStates)
                {
                    case GameStates.Menu:
                        break;
                    case GameStates.Game:
                        _signalBus.Fire<GameStartSignal>();
                        OnGameStart();
                        break;
                    case GameStates.Win:
                        OnGameEnd(true);
                        Level++;
                        _signalBus.Fire<GameEndSignal>();
                        break;
                    case GameStates.Lost:
                        OnGameEnd(false);
                        _signalBus.Fire<GameEndSignal>();
                        break;
                    case GameStates.Boss:
                        break;
                    default:
                        Debug.LogError("Wrong state: " + _gameStates);
                        break;
                }
            }
        }

        public int Coins
        {
            get => PlayerPrefs.GetInt(COINS_KEY, 0);
            private set
            {
                PlayerPrefs.SetInt(COINS_KEY, value);
                _signalBus.Fire(new CoinsUpdateSignal() {CoinsCount = value});
            }
        }

        public int Level
        {
            get => PlayerPrefs.GetInt(LEVEL_KEY, 1);
            private set => PlayerPrefs.SetInt(LEVEL_KEY, value);
        }

        public int LevelCount
        {
            get => PlayerPrefs.GetInt(LEVEL_COUNT_KEY, 1);
            private set => PlayerPrefs.SetInt(LEVEL_COUNT_KEY, value);
        }

        public GameManager(DiContainer diContainer, GameSettings gameSettings, PrefabsContainer prefabsContainer, SignalBus signalBus, PlayerUpgrade playerUpgrade, EnemySettings enemySettings)
        {
            _diContainer = diContainer;
            _gameSettings = gameSettings;
            _prefabsContainer = prefabsContainer;
            _signalBus = signalBus;
            _playerUpgrade = playerUpgrade;
            _enemySettings = enemySettings;
        }

        public async void Initialize()
        {
            var ui = _diContainer.InstantiatePrefab(_prefabsContainer.UIController);
            _ui = ui.GetComponent<UiController>();
            _diContainer.BindInstance(_ui).AsSingle().NonLazy();
            var player = _diContainer.InstantiatePrefab(_prefabsContainer.Player);
            _player = player.GetComponent<Player.Player>();
            _player.Init();
            _diContainer.BindInstance(_player).AsSingle().NonLazy();
            _cancellationTokenSource = new CancellationTokenSource();
            await UniTask.Yield();
            ChangeGameState(GameStates.Menu);
            SubscribeSignals();
        }

        public (string name, Color color) GetEnemyNames()
        {
            if (_names == null || _names.Count == 0)
            {
                _names = new List<string>();
                _names.AddRange(_enemySettings.Names);
            }

            if (_colors == null || _colors.Count == 0)
            {
                _colors = new List<Color>();
                _colors.AddRange(_enemySettings.Colors);
            }

            var name = _names[Random.Range(0, _names.Count)];
            _names.Remove(name);
            var color = _colors[Random.Range(0, _colors.Count)];
            _colors.Remove(color);
            return (name, color);
        }

        public void Dispose()
        {
            UnsubscribeSignals();
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        private void SubscribeSignals()
        {
            _signalBus.Subscribe<LevelWinSignal>(OnTimeOver);
            _signalBus.Subscribe<PlayerFailSignal>(OnFail);
            _signalBus.Subscribe<GetRewardSignal>(OnGetReward);
            _signalBus.Subscribe<PlayerUpgradeSignal>(OnPlayerUpgrade);
            _signalBus.Subscribe<GameRestartSignal>(OnGameRestart);
        }

        private void UnsubscribeSignals()
        {
            _signalBus.Unsubscribe<LevelWinSignal>(OnTimeOver);
            _signalBus.Unsubscribe<PlayerFailSignal>(OnFail);
            _signalBus.Unsubscribe<GetRewardSignal>(OnGetReward);
            _signalBus.Unsubscribe<PlayerUpgradeSignal>(OnPlayerUpgrade);
            _signalBus.Unsubscribe<GameRestartSignal>(OnGameRestart);
        }

        private void OnGameRestart()
        {
            _names = new List<string>();
            _names.AddRange(_enemySettings.Names);
            ChangeGameState(GameStates.Menu);
            Resources.UnloadUnusedAssets();
            GC.Collect();
            DOTween.Clear();
        }

        private void OnPlayerUpgrade(PlayerUpgradeSignal signal)
        {
            Coins -= signal.UpgradeItem.UpgradeCost;
        }

        private void OnGetReward(GetRewardSignal signal)
        {
            Coins += signal.RewardCount;
        }

        private void OnTimeOver(LevelWinSignal winSignal)
        {
            ChangeGameState(GameStates.Win);
        }

        private void OnFail()
        {
            ChangeGameState(GameStates.Lost);
        }

        public void ChangeGameState(GameStates gameStates)
        {
            GameState = gameStates;
        }

        private void OnGameStart()
        {
            var data = new Dictionary<string, object>
            {
                {"level_number", Level},
                {"level_count", LevelCount++},
            };
            AppMetrica.Instance.ReportEvent("level_start", data);
            AppMetrica.Instance.SendEventsBuffer();
            _isGame = true;
            _timer = 0;
        }

        private void OnGameEnd(bool isWin)
        {
            var data = new Dictionary<string, object>
            {
                {"level_number", Level},
                {"level_count", LevelCount},
                {"result", isWin ? "win" : "lose"},
                {"time", (int) _timer},
            };
            AppMetrica.Instance.ReportEvent("level_finish", data);
            AppMetrica.Instance.SendEventsBuffer();
            _isGame = false;
        }

        public void Tick()
        {
            if (!_isGame)
                return;
            _timer += Time.deltaTime;
        }
    }
}