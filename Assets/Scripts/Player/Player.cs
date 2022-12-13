using System;
using System.Collections.Generic;
using System.Linq;
using Boss;
using Buildings;
using Common;
using Configs;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Enums;
using Levels;
using Managers;
using UI;
using UnityEngine;
using Zenject;
using Zenject.Signals;
using Enemy = Enemy.Enemy;
using Random = UnityEngine.Random;

namespace Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private LayerMask bot;
        [SerializeField] private GameObject baseView;
        [SerializeField] private GameObject stickmanView;
        [SerializeField] private ParticleSystem startEffect;
        [SerializeField] private ParticleSystem startEffect2;
        [SerializeField] private ParticleSystem levelUpEffect;
        [SerializeField] private ParticleSystem levelUpEffect2;

        [SerializeField] private float kickDelay = 0.25f;
        [SerializeField] private float hitDelay = 0.25f;
        [SerializeField] private ParticleSystem kickFx;
        [SerializeField] private ParticleSystem hitFx;
        [SerializeField] private ParticleSystem bossWinEffect;
        [SerializeField] private ParticleSystem bossWinEffect2;
        [SerializeField] private ParticleSystem bossWinEffect3;
        [SerializeField] private ParticleSystem bossWinEffect4;

        private GameManager _gameManager;
        private PlayerSettings _playerSettings;
        private PlayerCameraController _cameraController;
        private PlayerAnimationController _animationController;
        private UiController _uiController;
        private SignalBus _signalBus;
        private Joystick _joystick;
        private Rigidbody _rigidbody;
        private PlayerView _playerView;
        private bool _isActive;
        private Level _level;
        private int _levelProgress;
        private int _levelCompletedValue;
        private PlayerUpgrade _playerUpgrade;
        private int _totalPointsOnLevel;
        private Crown _crown;

        private float ProgressValue => (float) _levelProgress / _levelCompletedValue;

        public Transform CameraPoint => _playerView.CameraPoint;

        public bool IsActive => _isActive;

        private float _speed;
        private bool _isPlayHitAnimation;
        private Vector3 _startScale;
        private GameSettings _gameSettings;
        private bool death;

        private bool _isBoss;

        [Inject]
        public void Construct(PlayerSettings playerSettings, UiController ui, SignalBus signalBus, PlayerUpgrade playerUpgrade, GameSettings gameSettings, GameManager gameManager)
        {
            _playerSettings = playerSettings;
            _uiController = ui;
            _signalBus = signalBus;
            _playerUpgrade = playerUpgrade;
            _gameSettings = gameSettings;
            _gameManager = gameManager;
        }

        public void Init()
        {
            _joystick = _uiController.Joystick;
            _rigidbody = GetComponent<Rigidbody>();
            _playerView = GetComponentInChildren<PlayerView>();
            _cameraController = GetComponent<PlayerCameraController>();
            _animationController = GetComponent<PlayerAnimationController>();
            _level = GetComponent<Level>();
            _crown = GetComponentInChildren<Crown>();
            _crown.gameObject.SetActive(false);
        }

        private void Start()
        {
            _startScale = transform.localScale;
            startEffect.gameObject.SetActive(false);
            _level.Value = 0;
            _levelProgress = 0;
            _levelCompletedValue = _playerSettings.LevelProgress[_level.Value];
            CheckUpgrade();
            var randomPoint = FindObjectOfType<LevelController>().GetRandomPoint();
            transform.position = randomPoint.position;

            if (_level.Value < _playerSettings.LevelProgress.Length)
            {
                _signalBus.Fire(new PlayerLevelValueSignal() {Level = _level.Value});
                _signalBus.Fire(new PlayerProgressBarSignal() {Value = ProgressValue});
            }
            else
            {
                _signalBus.Fire<PlayerMaxLevelSignal>();
            }

            _animationController.SetIdle();

            baseView.SetActive(false);
            stickmanView.SetActive(true);


            _signalBus.Subscribe<TimeOverSignal>(OnTimerOver);
            _signalBus.Subscribe<PlayerUpgradeSignal>(OnPlayerUpgrade);
            _signalBus.Subscribe<GameRestartSignal>(OnGameRestart);
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<TimeOverSignal>(OnTimerOver);
            _signalBus.Unsubscribe<PlayerUpgradeSignal>(OnPlayerUpgrade);
            _signalBus.Unsubscribe<GameRestartSignal>(OnGameRestart);
        }

        private void OnGameRestart()
        {
            death = false;
            bossWinEffect.gameObject.SetActive(false);
            bossWinEffect2.gameObject.SetActive(false);
            bossWinEffect3.gameObject.SetActive(false);
            bossWinEffect4.gameObject.SetActive(false);
            _isBoss = false;
            _isActive = false;
            transform.localScale = _startScale;
            _levelProgress = 0;
            _level.Value = 0;
            _totalPointsOnLevel = 0;
            startEffect.gameObject.SetActive(false);
            _level.Value = PlayerPrefs.GetInt("PlayerLevel", 0);
            _levelProgress = PlayerPrefs.GetInt("PlayerProgress", 0);
            _levelCompletedValue = _playerSettings.LevelProgress[_level.Value];
            CheckUpgrade();
            var randomPoint = FindObjectOfType<LevelController>().GetRandomPoint();
            transform.position = randomPoint.position;

            if (_level.Value < _playerSettings.LevelProgress.Length)
            {
                _signalBus.Fire(new PlayerLevelValueSignal() {Level = _level.Value});
                _signalBus.Fire(new PlayerProgressBarSignal() {Value = ProgressValue});
            }
            else
            {
                _signalBus.Fire<PlayerMaxLevelSignal>();
            }

            _signalBus.Fire<HideTimer>();
            _rigidbody.isKinematic = true;
            _animationController.SetIdle();
            transform.DORotate(new Vector3(0, -90, 0), 0.1f);
            baseView.SetActive(false);
            stickmanView.SetActive(true);
            _cameraController.OnRestart();
            _crown.gameObject.SetActive(false);
        }

        private void FixedUpdate()
        {
            if (death)  return;
            if (!_isActive || _isBoss) return;
            
            var direction = new Vector3(_joystick.Direction.x, 0, _joystick.Direction.y);
            if (direction == Vector3.zero)
            {
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            }

            else if (_playerSettings.AllTimeMove)
            {
                if (direction != Vector3.zero)
                {
                    _rigidbody.MoveRotation(Quaternion.Lerp(_rigidbody.rotation, Quaternion.LookRotation(direction), _playerSettings.RotationSpeed * Time.deltaTime));
                }

                _rigidbody.MovePosition(_rigidbody.position + _rigidbody.transform.forward * (_speed * Time.deltaTime));
            }
            else if (direction != Vector3.zero)
            {
                _rigidbody.MoveRotation(Quaternion.Lerp(_rigidbody.rotation, Quaternion.LookRotation(direction), _playerSettings.RotationSpeed * Time.deltaTime));
                _rigidbody.MovePosition(_rigidbody.position + _rigidbody.transform.forward * (direction.magnitude * _speed * Time.deltaTime));
            }

            _animationController.ChangeWalkAnimation(direction.magnitude);
        }

        public void AddPoints(int value)
        {
            _levelProgress += value;
            _totalPointsOnLevel += value;

            _signalBus.Fire(new PointsChangeSignal() {Name = "Player", SortItem = new SortItem() {Crown = _crown, Points = _totalPointsOnLevel}});
            //_signalBus.Fire(new PlayerPointsSignal() {Count = _totalPointsOnLevel});
            if (_levelProgress >= _levelCompletedValue)
            {
                if (++_level.Value < _playerSettings.LevelProgress.Length)
                {
                    _levelProgress = 0;
                    _levelCompletedValue = _playerSettings.LevelProgress[_level.Value];
                    _cameraController.LevelUpView(_playerSettings.CameraOffset[_level.Value]);
                    _signalBus.Fire(new PlayerLevelValueSignal() {Level = _level.Value});
                    CheckUpgrade(true);
                }
                else
                {
                    _signalBus.Fire<PlayerMaxLevelSignal>();
                    _signalBus.Fire(new LevelWinSignal() {LevelResult = GetLevelInfo()});
                }

                levelUpEffect.gameObject.SetActive(true);
                levelUpEffect2.gameObject.SetActive(true);
            }

            _signalBus.Fire(new PlayerProgressBarSignal() {Value = ProgressValue});
        }

        public void OnGameStateChange(GameStates gameStates)
        {
            _isActive = gameStates == GameStates.Game;
            if (_isActive)
            {
                Transformation();
            }
        }

        private void Transformation()
        {
            startEffect.gameObject.SetActive(true);
            startEffect.Play();
            startEffect2.gameObject.SetActive(true);
            startEffect2.Play();
            stickmanView.SetActive(false);
            baseView.SetActive(true);

            var startScale = baseView.transform.localScale;
            baseView.transform.localScale = Vector3.zero;
            baseView.transform.DOScale(startScale, 0.5f).SetDelay(0.2f);
        }

        public async void OnDeath()
        {
            _animationController.OnDeath();
            death = true;
            await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
            baseView.SetActive(false);
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            _signalBus.Fire(new PlayerFailSignal() {LevelResult = GetLevelInfo()});
        }

        public void PlayHitAnimation(BuildItem buildItem)
        {
            if (buildItem != null)
            {
                if (buildItem.Hit(_level.Value))
                {
                    AddPoints(buildItem.CostValue);
                    var trans = transform;
                    var pos = trans.position + trans.forward * trans.localScale.x;
                    buildItem.DestroyItSelf();
                    _signalBus.Fire(new PlayerBuildDestroySignal() {costValue = buildItem.CostValue, Position = pos});
                }
                else
                {
                    var direction = transform.position - buildItem.transform.position;
                    direction.y = transform.position.y;
                    direction.Normalize();
                    _rigidbody.velocity = direction * 8f;
                }
            }
            else if (buildItem == null)
            {
                var trans = transform;
                var pos = trans.position + trans.forward * trans.localScale.x;
                var colliders = Physics.OverlapSphere(pos, 1f * trans.localScale.x, bot);
                if (colliders.Length > 0)
                    _signalBus.Fire(new StickmanKillSignal() {Count = colliders.Length, Position = pos});
                AddPoints(1);
            }
        }

        private void OnTimerOver()
        {
            _joystick.Restart();
            _rigidbody.isKinematic = true;
            _animationController.SetIdle();
            _isActive = false;
            if (_level.Value < _gameSettings.PlayerBossLevel)
            {
                _signalBus.Fire(new LevelWinSignal() {LevelResult = GetLevelInfo()});
            }
            else
            {
                StartBossAction();
            }
        }

        private Dictionary<string, int> GetLevelInfo()
        {
            var dictionary = new Dictionary<string, int>();
            dictionary.Add("Player", _totalPointsOnLevel);
            var enemy = FindObjectsOfType<global::Enemy.Enemy>(true);
            foreach (var e in enemy)
            {
                var data = e.OnLevelEnd();
                if (!dictionary.ContainsKey(data.name))
                    dictionary.Add(data.name, data.points);
            }

            return dictionary;
        }

        private void OnPlayerUpgrade()
        {
            CheckUpgrade();
        }

        private void CheckUpgrade(bool withAnimation = false)
        {
            if (!withAnimation)
            {
                transform.localScale = _startScale * (1 + (_level.Value > 0 ? _playerSettings.ScaleFactor * (_level.Value + 1) : 1)) * _playerUpgrade.ScaleUp[_playerUpgrade.ScaleUpLevel].UpgradeValue;
            }
            else
            {
                var target = _startScale * (1 + (_level.Value > 0 ? _playerSettings.ScaleFactor * (_level.Value + 1) : 1)) * _playerUpgrade.ScaleUp[_playerUpgrade.ScaleUpLevel].UpgradeValue;
                transform.DOScale(target, 0.5f).SetEase(Ease.OutBack);
            }

            _speed = _playerSettings.LevelSpeed[_level.Value < _playerSettings.LevelSpeed.Length ? _level.Value : _playerSettings.LevelSpeed.Length - 1] *
                     _playerUpgrade.SpeedUp[_playerUpgrade.ScaleUpLevel].UpgradeValue;
        }

        public async void KillTitan(global::Enemy.Enemy enemy)
        {
            if (!_isPlayHitAnimation)
            {
                _isPlayHitAnimation = true;
                //_animationController.MakeStump(1);
                //await UniTask.Delay(TimeSpan.FromSeconds(1.2f));
                enemy.OnDeath();
                _cameraController.ShakeOnKillTitan();
                AddPoints(_gameSettings.PointsForTitan);
                _signalBus.Fire(new TitanKillSignal() {Points = _gameSettings.PointsForTitan, Position = enemy.transform.position});
                await UniTask.Delay(TimeSpan.FromSeconds(0.15f));
                _isPlayHitAnimation = false;
            }
        }

        private async void StartBossAction()
        {
            _crown.gameObject.SetActive(false);
            _gameManager.ChangeGameState(GameStates.Boss);
            _isBoss = true;
            await _cameraController.MoveToBossStartAction();
            _signalBus.Fire<StartBossActionSignal>();
            await _cameraController.MoveToFightPoint();
            
            var fight = FindObjectOfType<BossPlayerFightPoint>();
            transform.position = fight.StartPoint.position;
            transform.rotation = fight.StartPoint.rotation;
            transform.localScale = _playerSettings.PlayerBossScale;


            await _cameraController.MoveToFightPoint2();
            await UniTask.Delay(TimeSpan.FromSeconds(3));
            _animationController.ChangeWalkAnimation(1);
            await transform.DOMove(fight.FightPoint.position, 1);
            transform.DORotate(new Vector3(0, 10, 0), 0.1f);
            _animationController.BossFightIdle();
            _signalBus.Fire<StartBossFightSignal>();
        }

        public async void PunchBoss()
        {
            _animationController.PunchBoss();
            //await UniTask.Delay(TimeSpan.FromSeconds(hitDelay));
            _cameraController.ShakeOnBossHit();
            //kickFx.gameObject.SetActive(true);
        }

        public async void GetHitFromBoss()
        {
            _animationController.GetHitFromBoss();
            hitFx.gameObject.SetActive(true);
            _cameraController.ShakeOnBossHit();
            //await UniTask.Delay(TimeSpan.FromSeconds(hitDelay));
        }

        public async void OnBossWin(int winPoints, float endDelay)
        {
            _cameraController.WinBoss();
            await transform.DORotate(new Vector3(0, 90, 0), 1f);
            _animationController.PlayWinBossAnimation();
            _totalPointsOnLevel += winPoints;
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            bossWinEffect.gameObject.SetActive(true);
            bossWinEffect2.gameObject.SetActive(true);
            bossWinEffect3.gameObject.SetActive(true);
            bossWinEffect4.gameObject.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(endDelay));
            _signalBus.Fire(new LevelWinSignal() {LevelResult = GetLevelInfo()});
        }

        public async void FinalAttackBoss()
        {
            //await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
            _cameraController.MoveToFightPoint3();
            _animationController.BossFinalAttack();
            DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0.3f, 0.3f);
            await UniTask.Delay(TimeSpan.FromSeconds(0.35f));
            kickFx.gameObject.SetActive(true);
            DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0.05f, 0.05f);
            await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
            
            DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1f, 0.25f);
        }
    }
}