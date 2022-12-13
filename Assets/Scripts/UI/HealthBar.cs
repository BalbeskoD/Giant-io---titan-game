using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthBar : MonoBehaviour
    {
        private Slider _slider;
        private TextMeshProUGUI _text;

        private void Awake()
        {
            _slider = GetComponentInChildren<Slider>();
            _text = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            _slider.value = 1;
        }

        public void ChangeValue(float value)
        {
            _slider.DOValue(value, 0.2f).OnUpdate(ChangeText);
        }

        private void ChangeText()
        {
            if (_text != null)
            {
                _text.text = ((int) (_slider.value * 100)).ToString();
            }
        }
    }
}