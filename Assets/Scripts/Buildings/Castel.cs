using System;
using UnityEngine;
using Zenject;
using Zenject.Signals;

namespace Buildings
{
    public class Castel : MonoBehaviour
    {
        [SerializeField] private GameObject view;
        [SerializeField] private ParticleSystem[] destroyFx;

        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void Awake()
        {
            _signalBus.Subscribe<GameRestartSignal>(OnRestart);
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<GameRestartSignal>(OnRestart);
        }

        private void OnRestart()
        {
            view.SetActive(true);
        }

        public void DoDestroy()
        {
            view.SetActive(false);
            foreach (var fx in destroyFx)
            {
                fx.gameObject.SetActive(true);
            }
        }
    }
}