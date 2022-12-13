using System;
using Common;
using Configs;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using EPOOutline;
using Managers;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using Zenject.Signals;
using Buildings;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class Enemy : MonoBehaviour
    {
        private EnemySettings _enemySettings;
        [SerializeField] private GameObject view;
        [SerializeField] private LayerMask bot;
        private EnemyUI _enemyUI;
        private ParticleSystem[] _effects;
        private EnemyMoveController _moveController;
        private EnemyAnimationController _animationController;
        private Level _level;
        private SignalBus _signalBus;
        private int _levelProgress;
        private int _levelCompletedValue;
        private Crown _crown;

        private bool _isPlayHitAnimation;
        private bool _isActive;
        private int _totalPoint;
        private string _name;
        private Color _color;

        private Vector3 _startScale;
        private GameManager _gameManager;
        private GameSettings _gameSettings;

        private Rigidbody _rigidbody;
        private Outlinable _outlinable;

        public bool IsActive => _isActive;

        public Color Color => _color;

        [Inject]
        public void Construct(SignalBus signalBus, GameManager gameManager, EnemySettings enemySettings, GameSettings gameSettings)
        {
            _signalBus = signalBus;
            _gameManager = gameManager;
            _enemySettings = enemySettings;
            _gameSettings = gameSettings;
        }

        private void Awake()
        {
            _effects = GetComponentsInChildren<ParticleSystem>();
            _enemyUI = GetComponentInChildren<EnemyUI>();
            _moveController = GetComponent<EnemyMoveController>();
            _animationController = GetComponent<EnemyAnimationController>();
            _outlinable = view.GetComponent<Outlinable>();
            _level = GetComponent<Level>();
            _rigidbody = GetComponent<Rigidbody>();
            _crown = GetComponentInChildren<Crown>();
            _crown.gameObject.SetActive(false);
            _signalBus.Subscribe<GameStartSignal>(OnGameStart);
            _signalBus.Subscribe<TimeOverSignal>(OnTimerOver);
            _signalBus.Subscribe<GameRestartSignal>(OnGameRestart);
            _signalBus.Subscribe<StartBossActionSignal>(OnBoss);
        }

        private void Start()
        {
            _moveController.SetSpeed(_enemySettings.LevelProgressItems[_level.Value].LevelSpeed);
            SetupView();
            _startScale = transform.localScale;
            _totalPoint = 0;
            _levelCompletedValue = _enemySettings.LevelProgressItems[_level.Value].LevelProgress;
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<GameStartSignal>(OnGameStart);
            _signalBus.Unsubscribe<TimeOverSignal>(OnTimerOver);
            _signalBus.Unsubscribe<GameRestartSignal>(OnGameRestart);
            _signalBus.Unsubscribe<StartBossActionSignal>(OnBoss);
        }

        private void OnBoss()
        {
            gameObject.SetActive(false);
        }

        private void OnGameRestart()
        {
            _level.Value = 0;
            _levelProgress = 0;
            _levelCompletedValue = _enemySettings.LevelProgressItems[_level.Value].LevelProgress;
            _moveController.SetSpeed(_enemySettings.LevelProgressItems[_level.Value].LevelSpeed);
            SetupView();
            _totalPoint = 0;
            _isActive = false;
            _rigidbody.isKinematic = true;
            StopMove();
        }

        private void SetupView()
        {
            var data = _gameManager.GetEnemyNames();
            _name = data.name;
            _color = data.color;
            _enemyUI.Setup(_name, _color);
            _outlinable.OutlineParameters.Color = _color;
            gameObject.SetActive(true);
            view.SetActive(true);
        }

        private void OnGameStart()
        {
            _isActive = true;

            StartMove();
        }

        private void StartMove()
        {
            _rigidbody.isKinematic = false;
            _moveController.StartMove();
        }

        private void OnTimerOver()
        {
            StopMove();
        }

        private void StopMove()
        {
            _moveController.StopMove();
        }

        public async void OnDeath()
        {
            _rigidbody.isKinematic = true;
            _isActive = false;
            _animationController.OnDeath();
            StopMove();
            PlayDeathEffects();

            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            view.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        //private void FlyAway(Vector3 direction)
        //{
        // transform.DOJump(transform.position + direction * 5, 2, 1, 1f).OnComplete(() => gameObject.SetActive(false));
        // }

        private void PlayDeathEffects()
        {
            foreach (var effect in _effects)
            {
                effect.gameObject.SetActive(true);
            }
        }

        public void OnLevelUp()
        {
            _moveController.SetSpeed(_enemySettings.LevelProgressItems[_level.Value].LevelSpeed);
            _enemyUI.OnLevelUp(_level.Value);
        }

        public void AddPoints(int value)
        {
            _levelProgress += value;
            _totalPoint += value;
            _signalBus.Fire(new PointsChangeSignal() {Name = _name, SortItem = new SortItem() {Crown = _crown, Points = _totalPoint}});
            if (_levelProgress >= _levelCompletedValue)
            {
                if (++_level.Value < _enemySettings.LevelProgressItems.Length)
                {
                    _levelProgress = 0;
                    _levelCompletedValue = _enemySettings.LevelProgressItems[_level.Value].LevelProgress;
                    transform.DOScale(_startScale * (float) Math.Pow(_enemySettings.ScaleFactor, _level.Value + 1), 0.5f).SetEase(Ease.OutBack);
                    OnLevelUp();
                }
            }
        }

        private void TryKillEnemy()
        {
            var colliders = Physics.OverlapSphere(transform.position + transform.forward * transform.localScale.x, 1f * transform.localScale.x, bot);
            foreach (var bot in colliders)
            {
                bot.GetComponentInParent<Stickman.Stickman>().Death();
            }

            AddPoints(colliders.Length);
        }

        public (string name, int points) OnLevelEnd()
        {
            return new(_name, _totalPoint);
        }

        public async void KillPlayer(Player.Player player)
        {
            if (!_isPlayHitAnimation)
            {
                _isPlayHitAnimation = true;
                _animationController.MakeStomp();
                await UniTask.Delay(TimeSpan.FromSeconds(0.35f));
                player.OnDeath();
                AddPoints(_gameSettings.PointsForTitan);
                await UniTask.Delay(TimeSpan.FromSeconds(0.15f));
                _isPlayHitAnimation = false;
            }
        }

        public async void KillTitan(Enemy titan)
        {
            if (!_isPlayHitAnimation)
            {
                _isPlayHitAnimation = true;
                //_animationController.MakeStomp();
                //await UniTask.Delay(TimeSpan.FromSeconds(0.35f));
                titan.OnDeath();
                AddPoints(_gameSettings.PointsForTitan);
                await UniTask.Delay(TimeSpan.FromSeconds(0.15f));
                _isPlayHitAnimation = false;
            }
        }

        public void PlayHitStickmanAnimation()
        {
           
            var colliders = Physics.OverlapSphere(transform.position + transform.forward * transform.localScale.x, 1f * transform.localScale.x, bot);
            foreach (var bot in colliders)
            {
                bot.GetComponentInParent<Stickman.Stickman>().Death();
                AddPoints(1);
            }
        }

        public void PlayHitBuildingAnimation(BuildItem build)
        {
            AddPoints(build.CostValue);
            build.DestroyItSelf(); ;
        }
    }
}