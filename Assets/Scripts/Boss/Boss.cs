using System;
using Buildings;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Helpers;
using UnityEngine;
using Zenject;
using Zenject.Signals;
using Random = UnityEngine.Random;

namespace Boss
{
    public class Boss : MonoBehaviour
    {
        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int FightIdle = Animator.StringToHash("FightIdle");
        private static readonly int BreakWall = Animator.StringToHash("BreakWall");
        private static readonly int Walk = Animator.StringToHash("Walk");
        private static readonly int Hit = Animator.StringToHash("Hit");
        private static readonly int PunchLeft = Animator.StringToHash("PunchLeft");
        private static readonly int PunchRight = Animator.StringToHash("PunchRight");
        private static readonly int Death = Animator.StringToHash("Death");

        [SerializeField] private float kickDelay = 0.25f;
        [SerializeField] private float hitDelay = 0.25f;
        [SerializeField] private ParticleSystem kickFx;
        [SerializeField] private ParticleSystem hitFx;
        [SerializeField] private ParticleSystem[] startFx;
        [SerializeField] private ParticleSystem castleFx;

        [SerializeField] private GameObject stickmanView;
        [SerializeField] private GameObject bossView;

        private BossFightPoint _fightPoint;
        private Animator _animator;
        private Vector3 _startPosition;
        private Quaternion _startRotation;
        private SignalBus _signalBus;

        private Rigidbody _rigidbody;

        private int _prevKick;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            _fightPoint = FindObjectOfType<BossFightPoint>();
            _startPosition = bossView.transform.position;
            _startRotation = bossView.transform.rotation;

            _signalBus.Subscribe<StartBossActionSignal>(OnStartAction);
            _signalBus.Subscribe<GameRestartSignal>(OnRestart);
            _rigidbody = GetComponentInChildren<Rigidbody>();

            _rigidbody.isKinematic = true;


            stickmanView.SetActive(true);
            bossView.SetActive(false);
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<StartBossActionSignal>(OnStartAction);
            _signalBus.Unsubscribe<GameRestartSignal>(OnRestart);
        }

        private void OnStartAction()
        {
            StartAction().Forget();
        }

        private void OnRestart()
        {
            bossView.transform.position = _startPosition;
            bossView.transform.rotation = _startRotation;
            stickmanView.SetActive(true);
            bossView.SetActive(false);

            _rigidbody.isKinematic = true;


            _animator.enabled = true;
        }

        private async UniTask StartAction()
        {
            foreach (var fx in startFx)
            {
                fx.gameObject.SetActive(true);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(1.4f));

            stickmanView.gameObject.SetActive(false);
            castleFx.gameObject.SetActive(true);
            bossView.gameObject.SetActive(true);
            bossView.transform.localScale = Vector3.one;
            var target = FindObjectOfType<Player.Player>().transform.localScale;
            var castel = FindObjectOfType<Castel>();
            castel.DoDestroy();
            _animator.SetTrigger(BreakWall);
            await bossView.transform.DOScale(new Vector3(9.75f, 9.75f, 9.75f), 0.5f);
            target = transform.position;
            target.y = 0;
            bossView.transform.DOMove(target, 0.75f).SetEase(Ease.InOutCirc);
            bossView.transform.DOMove(_fightPoint.transform.position, 0.75f);
            await bossView.transform.DORotateQuaternion(_fightPoint.transform.rotation, 0.75f);
        }

        public void OnWin()
        {
            _animator.SetTrigger(Idle);
        }

        public void OnFail()
        {
            var wall = FindObjectOfType<WallDestroy>();
            _animator.enabled = false;

            _rigidbody.isKinematic = false;
            _rigidbody.AddForce((_rigidbody.position - wall.transform.position).normalized * 500, ForceMode.Impulse);
            wall.DestroyWalls();
        }

        public async void OnGetHit()
        {
            _animator.SetTrigger(Hit);
            hitFx.gameObject.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(hitDelay));
        }

        public async void MadePunch()
        {
            _animator.SetTrigger(_prevKick == 0 ? PunchRight : PunchLeft);
            _prevKick = _prevKick == 0 ? 1 : 0;
            await UniTask.Delay(TimeSpan.FromSeconds(kickDelay));
            //kickFx.gameObject.SetActive(true);
        }
    }
}