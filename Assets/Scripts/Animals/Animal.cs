using System;
using Stickman;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using Zenject.Signals;

namespace Animals
{
    public class Animal : MonoBehaviour
    {
        [SerializeField] private Transform targetPoint;
        [SerializeField] private float speed;
        private bool _isMoveToTarget;
        private bool _isRotateToTarget;
        private ParticleSystem[] _effects;
        private bool _isDead;
        private bool _isGameActive;
        private SignalBus _signalBus;
        private Vector3 _startPosition;
        private Quaternion _direction;
        [SerializeField] private float rotateSpeed;
        public bool IsDead => _isDead;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void Awake()
        {
            _effects = GetComponentsInChildren<ParticleSystem>(true);
            _signalBus.Subscribe<GameStartSignal>(OnGameStart);
            _signalBus.Subscribe<TimeOverSignal>(OnTimerOver);
            _startPosition = transform.position;
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<GameStartSignal>(OnGameStart);
            _signalBus.Unsubscribe<TimeOverSignal>(OnTimerOver);
        }

        private void OnTimerOver()
        {
            _isGameActive = false;
        }

        private void OnGameStart()
        {
            _isGameActive = true;
        }

        public void OnDeath()
        {
            _isDead = true;
            foreach (var effect in _effects)
            {
                effect.gameObject.SetActive(true);
            }
        }

        private void Update()
        {
            if (!_isGameActive)
                return;
            if (_isMoveToTarget)
            {
                if (_isRotateToTarget)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, _direction, rotateSpeed * Time.deltaTime);
                    if (transform.rotation == _direction)
                    {
                        _isRotateToTarget = false;
                    }

                    return;
                }

                transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);
                if (transform.position == targetPoint.position)
                {
                    _direction = Quaternion.LookRotation(_startPosition - transform.position);
                    _isMoveToTarget = false;
                    _isRotateToTarget = true;
                }
            }
            else
            {
                if (_isRotateToTarget)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, _direction, rotateSpeed * Time.deltaTime);
                    if (transform.rotation == _direction)
                    {
                        _isRotateToTarget = false;
                    }

                    return;
                }

                transform.position = Vector3.MoveTowards(transform.position, _startPosition, speed * Time.deltaTime);
                if (transform.position == _startPosition)
                {
                    _direction = Quaternion.LookRotation(targetPoint.position - transform.position);
                    _isMoveToTarget = true;
                    _isRotateToTarget = true;
                }
            }
        }
    }
}