using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;
using Zenject.Signals;

namespace UI
{
    public class GoodEffect : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI goodText;
        [SerializeField] private float goodEffectMoveUpDelta = 250;
        [SerializeField] private float goodEffectMoveUpDuration = 2;
        private float _goodEffectStartPosition;
        private Camera _camera;
        private SignalBus _signalBus;
        private Tween _tween;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void Start()
        {
            _camera = FindObjectOfType<Camera>();
            _goodEffectStartPosition = transform.localPosition.y;
            _signalBus.Subscribe<PlayerBuildDestroySignal>(OnBuildDestroy);
            _signalBus.Subscribe<PlayerLevelValueSignal>(OnPlayerLeveUp);
            _signalBus.Subscribe<StickmanKillSignal>(OnSticmanDestroy);
            _signalBus.Subscribe<TitanKillSignal>(OnTitanKill);
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<PlayerBuildDestroySignal>(OnBuildDestroy);
            _signalBus.Unsubscribe<PlayerLevelValueSignal>(OnPlayerLeveUp);
            _signalBus.Unsubscribe<StickmanKillSignal>(OnSticmanDestroy);
            _signalBus.Unsubscribe<TitanKillSignal>(OnTitanKill);
        }

        private void OnTitanKill(TitanKillSignal signal)
        {
            goodText.text = $"+{signal.Points}";
            MoveGoodTextFromPos(signal.Position);
        }

        private void OnPlayerLeveUp(PlayerLevelValueSignal level)
        {
            goodText.text = $"Level up!";
            MoveGoodText();
        }

        private void OnBuildDestroy(PlayerBuildDestroySignal costValue)
        {
            goodText.text = $"+{costValue.costValue}";
            MoveGoodTextFromPos(costValue.Position);
        }

        private void OnSticmanDestroy(StickmanKillSignal progressValue)
        {
            goodText.text = $"+{progressValue.Count}";
            MoveGoodTextFromPos(progressValue.Position);
        }

        private void MoveGoodText()
        {
            _tween?.Kill(true);
            gameObject.SetActive(true);
            _tween = transform.DOLocalMoveY(_goodEffectStartPosition + goodEffectMoveUpDelta, goodEffectMoveUpDuration)
                .OnComplete(() =>
                {
                    transform.localPosition = new Vector3(0, _goodEffectStartPosition, 0);
                    gameObject.SetActive(false);
                });
        }

        private void MoveGoodTextFromPos(Vector3 pos)
        {
            _tween?.Kill(true);
            gameObject.SetActive(true);

            transform.position = _camera.WorldToScreenPoint(pos);
            _tween = transform.DOLocalMoveY(_goodEffectStartPosition + goodEffectMoveUpDelta, goodEffectMoveUpDuration)
                .OnComplete(() =>
                {
                    transform.localPosition = new Vector3(0, _goodEffectStartPosition, 0);
                    gameObject.SetActive(false);
                });
        }
    }
}