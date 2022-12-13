using System;
using UnityEngine;

namespace Stickman
{
    public class StickmanView : MonoBehaviour
    {
        private Stickman _stickman;

        private void Awake()
        {
            _stickman = GetComponentInParent<Stickman>();
        }

        private void OnBecameVisible()
        {
            _stickman.OnVisible();
        }

        private void OnBecameInvisible()
        {
            _stickman.OnInvisible();
        }
    }
}