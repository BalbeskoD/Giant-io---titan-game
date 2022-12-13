using System;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Zenject.Signals;

namespace UI
{
    public class ProgressBar : MonoBehaviour
    {
        private Slider _slider;
        private TextMeshProUGUI _levelText;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void Awake()
        {
            _slider = GetComponent<Slider>();
            _levelText = GetComponentInChildren<TextMeshProUGUI>();
            _signalBus.Subscribe<PlayerProgressBarSignal>(SetValue);
            _signalBus.Subscribe<PlayerLevelValueSignal>(SetLevel);
            _signalBus.Subscribe<PlayerMaxLevelSignal>(OnMaxLevel);
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<PlayerProgressBarSignal>(SetValue);
            _signalBus.Unsubscribe<PlayerLevelValueSignal>(SetLevel);
            _signalBus.Unsubscribe<PlayerMaxLevelSignal>(OnMaxLevel);
        }

        private void SetValue(PlayerProgressBarSignal signal)
        {
            _slider.value = signal.Value;
        }

        private void SetLevel(PlayerLevelValueSignal signal)
        {
            _levelText.text = $"Lv. {signal.Level + 1}";
        }

        private void OnMaxLevel()
        {
            gameObject.SetActive(false);
        }
    }
}