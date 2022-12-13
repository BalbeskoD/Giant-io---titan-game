using UnityEngine;

namespace Stickman
{
    public class StickmanAnimationController : MonoBehaviour
    {
        private static readonly int Walk = Animator.StringToHash("Walk");
        private Animator _animator;
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }
        
        public void ChangeWalkAnimation(float value)
        {
            _animator.SetFloat(Walk, value);
        }
    }
}