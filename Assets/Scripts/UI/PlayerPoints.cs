using System;
using TMPro;
using UnityEngine;
using Zenject;
using Zenject.Signals;

namespace UI
{
    public class PlayerPoints : MonoBehaviour
    {
        private TextMeshProUGUI text;

        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();
            text.text = "0";
            _signalBus.Subscribe<PlayerPointsSignal>(OnPoints);
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<PlayerPointsSignal>(OnPoints);
        }

        private void OnPoints(PlayerPointsSignal signal)
        {
            text.text = signal.Count.ToString();
        }
    }
}