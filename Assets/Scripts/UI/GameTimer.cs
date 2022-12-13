using System;
using Configs;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;
using Zenject.Signals;

namespace UI
{
    public class GameTimer : MonoBehaviour
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
        private bool _isTimerOver = true;

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
            _signalBus.Subscribe<GameStartSignal>(OnGameStart);
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<GameStartSignal>(OnGameStart);
        }

        private void OnGameStart()
        {
            _isTimerOver = false;
        }

        private void OnEnable()
        {
            _lastTime = Time.unscaledTime;
            _lastTimeLose = 0;
        }

        private void Update()
        {
            if (_isTimerOver)
                return;
            _timer = _gameSettings.LevelSessionTime - Time.unscaledTime + _lastTime - _lastTimeLose;
            _minutes = Mathf.FloorToInt(_timer / 60f);
            _seconds = Mathf.FloorToInt(_timer - _minutes * 60);
            //if ((int) _timer < 11 && _prevValue != (int) _timer)
            //{
            //  var start = _textMeshPro.fontSize;
            //  var target = _textMeshPro.fontSize * 1.2f;
            // Sequence s = DOTween.Sequence();
            //    s.Append(DOTween.To(() => _textMeshPro.fontSize, x => _textMeshPro.fontSize = x, target, 0.5f));
            //   s.Append(DOTween.To(() => _textMeshPro.fontSize, x => _textMeshPro.fontSize = x, start, 0.5f));
            //  }

            _textMeshPro.text = string.Format("{0:00}:{1:00}", _minutes, _seconds);
            if (_timer <= 10)
            {
                _signalBus.Fire<Timer10>();
            }

            if (_timer <= 0)
            {
                _isTimerOver = true;
                _signalBus.Fire<TimeOverSignal>();
            }

            _prevValue = (int) _timer;
        }
    }
}