using Configs;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;
using Zenject.Signals;

namespace UI
{
    public class GameTimer10 : MonoBehaviour
    {
        private TextMeshProUGUI _textMeshPro;

        private float _lastTime;

        private int _minutes;
        private int _seconds;
        private float _timer;
        private float _lastTimeLose;

        private SignalBus _signalBus;
        private GameSettings _gameSettings;
        private int _prevValue;

        [Inject]
        public void Construct(SignalBus signalBus, GameSettings gameSettings)
        {
            _signalBus = signalBus;
            _gameSettings = gameSettings;
        }

       
        private void Awake()
        {
            _textMeshPro = GetComponent<TextMeshProUGUI>();
            _lastTime = Time.unscaledTime;
            _signalBus.Subscribe<Timer10>(ShowTimer10);
            _signalBus.Subscribe<HideTimer>(HideTimer10);
            _textMeshPro.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<HideTimer>(HideTimer10);
            _signalBus.Unsubscribe<Timer10>(ShowTimer10);
        }
        private void OnEnable()
        {
            _lastTime = Time.unscaledTime;
            _lastTimeLose = 0;
        }
        private void HideTimer10()
        {
            _textMeshPro.gameObject.SetActive(false);
        }

        private void ShowTimer10()
        {
            _textMeshPro.gameObject.SetActive(true);
        }
        private void Update()
        {
            _timer = 10 - Time.unscaledTime + _lastTime - _lastTimeLose;
            _minutes = Mathf.FloorToInt(_timer / 60f);
            _seconds = Mathf.FloorToInt(_timer - _minutes * 60);
           

            _textMeshPro.text = string.Format("{0:0}", _seconds);
            
            if (_timer <= 0)
            {
                _signalBus.Fire<TimeOverSignal>();
            }

            _prevValue = (int) _timer;
        }
    }
}