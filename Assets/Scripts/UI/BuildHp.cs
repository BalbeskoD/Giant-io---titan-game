using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Zenject.Signals;

namespace UI
{
    public class BuildHp : MonoBehaviour
    {
        private Slider _slider;
        private SignalBus _signalBus;
        private Tween _tween;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void Awake()
        {
            
            _slider = GetComponentInChildren<Slider>();
            _signalBus.Subscribe<BuildHitSignal>(OnHit);
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<BuildHitSignal>(OnHit);
        }

        private async void OnHit(BuildHitSignal signal)
        {
            _tween?.Kill();
            if (signal.To <= 0)
            {
                return;
            }
            gameObject.SetActive(true);
            gameObject.transform.position = Camera.main.WorldToScreenPoint(signal.Target.position);
            _slider.value = signal.From;
            _tween = _slider.DOValue(signal.To, 0.01f);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            gameObject.SetActive(false);
        }
    }
}