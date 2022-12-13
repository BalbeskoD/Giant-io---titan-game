using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using Zenject.Signals;
using Random = UnityEngine.Random;

namespace Stickman
{
    public class Stickman : MonoBehaviour
    {
        [SerializeField] private GameObject view;
        private NavMeshAgent _agent;
        private StickmanAnimationController _animationController;
        private StickmanMovePoints[] _points;
        private SignalBus _signalBus;
        private ParticleSystem[] _effects;
        private bool _isDead;
        private bool _isGameActive;

        public bool IsDead => _isDead;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _animationController = GetComponent<StickmanAnimationController>();
            _effects = GetComponentsInChildren<ParticleSystem>(true);
            _points = FindObjectsOfType<StickmanMovePoints>();
            _signalBus.Subscribe<GameStartSignal>(OnGameStart);
            _signalBus.Subscribe<TimeOverSignal>(OnTimerOver);
            _signalBus.Subscribe<GameRestartSignal>(OnGameRestart);
            _signalBus.Subscribe<StartBossActionSignal>(OnBoss);
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
            gameObject.SetActive(true);
            view.SetActive(true);
            _isDead = false;
            _isGameActive = false;
            _agent.enabled = false;
            _animationController.ChangeWalkAnimation(0f);
        }

        private void OnTimerOver()
        {
            _isGameActive = false;
            _agent.enabled = false;
            _animationController.ChangeWalkAnimation(0f);
        }

        private void OnGameStart()
        {
            _isGameActive = true;
            FindNextPoint();
        }

        private void Update()
        {
            if (!_isGameActive)
            {
                return;
            }

            if (!_agent.enabled || !_agent.isOnNavMesh)
                return;

            _animationController.ChangeWalkAnimation(_agent.hasPath && !_agent.pathPending ? 1f : 0f);
            if (_agent.remainingDistance < _agent.stoppingDistance || !_agent.hasPath)
            {
                FindNextPoint();
            }
        }

        private void FindNextPoint()
        {
            if (!_agent.enabled || !_agent.isOnNavMesh)
                return;
            var point = _points[Random.Range(0, _points.Length)];

            _agent.SetDestination(point.transform.position);
        }

        public void OnVisible()
        {
            _agent.enabled = true;
            if (_isGameActive)
            {
                FindNextPoint();
            }
        }

        public void OnInvisible()
        {
            _agent.enabled = false;
        }

        public async void Death()
        {
            if (_isDead)
                return;
            _isDead = true;
            view.SetActive(false);
            foreach (var effect in _effects)
            {
                effect.gameObject.SetActive(true);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            gameObject.SetActive(false);
        }
    }
}