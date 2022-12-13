using Common;
using UnityEngine;

namespace Player
{
    public class PlayerAnimationController : MonoBehaviour
    {
        private static readonly int Walk = Animator.StringToHash("Walk");
        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int Stomp = Animator.StringToHash("Stomp");
        private static readonly int Hit = Animator.StringToHash("Hit");
        private static readonly int KillEnemy = Animator.StringToHash("KillEnemy");
        private static readonly int Death = Animator.StringToHash("Death");
        private static readonly int Punch = Animator.StringToHash("Punch");

        private static readonly int FightIdle = Animator.StringToHash("FightIdle");
        private static readonly int PunchLeft = Animator.StringToHash("PunchLeft");
        private static readonly int PunchRight = Animator.StringToHash("PunchRight");
        private static readonly int WinBoss = Animator.StringToHash("WinBoss");
        private static readonly int FinalAttack = Animator.StringToHash("FinalAttack");

        private Animator _animator;

        private int _prevHit;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void ChangeWalkAnimation(float value)
        {
            _animator.SetFloat(Walk, value);
        }

        public void SetIdle()
        {
            _animator.SetFloat(Walk, 0);
            _animator.SetTrigger(Idle);
        }

        public void OnDeath()
        {
            _animator.SetTrigger(Death);
        }

        public void PunchBoss()
        {
            _animator.SetTrigger(_prevHit == 0 ? PunchRight : PunchLeft);
            _prevHit = _prevHit == 0 ? 1 : 0;
        }

        public void GetHitFromBoss()
        {
            _animator.SetTrigger(Hit);
        }

        public void BossFightIdle()
        {
            _animator.ResetTrigger(WinBoss);
            _animator.SetTrigger(FightIdle);
        }

        public void PlayWinBossAnimation()
        {
            _animator.SetTrigger(WinBoss);
            _animator.SetFloat(Walk, 0);
        }

        public void BossFinalAttack()
        {
            _animator.SetTrigger(FinalAttack);
        }
    }
}