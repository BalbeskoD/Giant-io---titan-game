using System;
using UnityEngine;

namespace Common
{
    public enum AnimationState
    {
        None,
        Idle,
        Run
    }

    public abstract class AnimationBaseController : MonoBehaviour
    {
        protected Animator _animator;
        protected AnimationState _animationState;
        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int Walk = Animator.StringToHash("Walk");

        protected AnimationState AnimationState
        {
            get => _animationState;

            set
            {
                if (_animationState == value)
                    return;
                _animationState = value;
                switch (_animationState)
                {
                    case AnimationState.Idle:
                        _animator.SetTrigger(Idle);
                        break;
                    case AnimationState.Run:
                        _animator.SetTrigger(Walk);
                        break;
                    default:
                        print("Wrong animation state: " + _animationState);
                        break;
                }
            }
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

       

        public virtual void ChangeAnimation(AnimationState state)
        {
            AnimationState = state;
        }
    }
}