using System;
using Common;
using Cysharp.Threading.Tasks;
using EPOOutline;
using UnityEngine;
using Zenject;
using Zenject.Signals;

namespace Buildings
{
    public class BuildItem : MonoBehaviour
    {
        [SerializeField] private int levelForDestroy = 3;
        [SerializeField] private int costValue = 1;
        private Outlinable _outlinable;
        private MeshRenderer[] _views;
        private Collider[] _colliders;
        private ParticleSystem[] _effects;

        private bool _isDestroy;
        private SignalBus _signalBus;
        private Level _level;

        public int CostValue => costValue;

        private float _maxHp;
        private float _currentHp;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void Awake()
        {
            _views = GetComponentsInChildren<MeshRenderer>();
            _effects = GetComponentsInChildren<ParticleSystem>(true);
            _colliders = GetComponentsInChildren<Collider>();
            _outlinable = GetComponent<Outlinable>();

            _signalBus.Subscribe<GameRestartSignal>(OnGameRestart);
        }

        private void Start()
        {
            var player = FindObjectOfType<Player.Player>();
            _level = player.GetComponent<Level>();
            _level.OnLevelChange += OnPlayerLevelChange;
            _maxHp = _currentHp = levelForDestroy;
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<GameRestartSignal>(OnGameRestart);
            _level.OnLevelChange -= OnPlayerLevelChange;
        }

        private void OnPlayerLevelChange(int obj)
        {
            CheckOutline();
        }

        private void CheckOutline()
        {
            if (levelForDestroy <= _level.Value)
            {
                CheckOutlineForCanBeDestroy(true);
            }
            else
            {
                CheckOutlineForCanBeDestroy(true);
            }
        }

        private void CheckOutlineForCanBeDestroy(bool isActiveOutline)
        {
            if (_outlinable != null)
                _outlinable.OutlineParameters.Color = isActiveOutline ? Color.green : Color.red;
        }

        private void OnGameRestart()
        {
            _maxHp = _currentHp = levelForDestroy;
            _isDestroy = false;
            foreach (var view in _views)
            {
                view.enabled = true;
            }

            foreach (var col in _colliders)
            {
                col.enabled = true;
            }
        }

        public async void DestroyItSelf()
        {
            if (_isDestroy)
                return;

            _isDestroy = true;
            foreach (var view in _views)
            {
                view.enabled = false;
            }

            foreach (var col in _colliders)
            {
                col.enabled = false;
            }

            PlayEffect();
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            gameObject.SetActive(false);
        }

        private void PlayEffect()
        {
            foreach (var effect in _effects)
            {
                effect.gameObject.SetActive(true);
            }
        }

        public bool Hit(int level)
        {
            var prev = _currentHp;
            
            _currentHp -= (level + 1);
            if (_currentHp <= 0)
                return true;
            _signalBus.Fire(new BuildHitSignal() {From = prev / (float) _maxHp, To = _currentHp / (float) _maxHp, Target = transform});
            return false;
        }
    }
}