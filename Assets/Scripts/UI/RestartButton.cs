using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Zenject.Signals;

namespace UI
{
    public class RestartButton : MonoBehaviour
    {
        private Button _button;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void Start()
        {
            _button.onClick.AddListener(Restart);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(Restart);
        }

        private void Restart()
        {
            _signalBus.Fire<GameRestartSignal>();
        }
    }
}