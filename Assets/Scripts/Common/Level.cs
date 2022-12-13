using System;
using UnityEngine;

namespace Common
{
    public class Level : MonoBehaviour
    {
        public event Action<int> OnLevelChange;

        private int _level;

        public int Value
        {
            get => _level;
            set
            {
                if(value == _level)
                    return;
                _level = value;
                OnLevelChange?.Invoke(_level);
            }
        }
    }
}