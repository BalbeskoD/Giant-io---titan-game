using System;
using UnityEngine;
using Zenject;
using Zenject.Signals;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class EnemyAnimationController : MonoBehaviour
    {
        private static readonly int StartMove = Animator.StringToHash("StartMove");
        private static readonly int StopMove = Animator.StringToHash("StopMove");
        private static readonly int Stomp = Animator.StringToHash("Stomp");
        private static readonly int Hit = Animator.StringToHash("Hit");
        private static readonly int KillEnemy = Animator.StringToHash("KillEnemy");
        private static readonly int Death = Animator.StringToHash("Death");
        private Animator _animator;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _signalBus.Subscribe<GameStartSignal>(OnGameStart);
            _signalBus.Subscribe<GameEndSignal>(OnGameEnd);
            _signalBus.Subscribe<GameRestartSignal>(OnGameEnd);
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<GameStartSignal>(OnGameStart);
            _signalBus.Unsubscribe<GameEndSignal>(OnGameEnd);
            _signalBus.Unsubscribe<GameRestartSignal>(OnGameEnd);
        }

        private void OnGameEnd()
        {
            _animator.SetTrigger(StopMove);
        }

        private void OnGameStart()
        {
            _animator.SetTrigger(StartMove);
        }

        public void MakeStomp()
        {
            var rnd = Random.Range(0, 2);
            _animator.SetTrigger(Hit);
            _animator.SetInteger(Stomp, rnd);
        }

        public void HitTitan()
        {
            var rnd = Random.Range(2, 4);
            _animator.SetTrigger(Hit);
            _animator.SetInteger(KillEnemy, rnd);
        }

        public void OnDeath()
        {
            _animator.SetTrigger(Death);
        }
    }
}