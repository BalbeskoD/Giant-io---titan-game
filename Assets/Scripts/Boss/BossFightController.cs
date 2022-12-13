using System;
using System.Collections;
using Configs;
using Cysharp.Threading.Tasks;
using UI;
using UnityEngine;
using Zenject;
using Zenject.Signals;

namespace Boss
{
    public class BossFightController : MonoBehaviour
    {
        private BossPanel _bossPanel;
        private BossFightSettings _bossFightSettings;
        private Player.Player _player;
        private Boss _boss;
        private SignalBus _signalBus;

        private Coroutine _bossDelayCor;
        private bool _isFight;
        private bool _isHit;

        private int _playerKickCount;
        private int _bossKickCount;
        private float _winCount;

        private int _playerClickCount;
        private bool _isFirstClick;

        private int _playerKickCountInRow;
        private int _bossKickCountInRow;

        private float _progress;

        private bool _isBossFirstKick;
        private float _timeToKick = 3;
        private float _timer;

        [Inject]
        public void Construct(SignalBus signalBus, BossFightSettings settings)
        {
            _signalBus = signalBus;
            _bossFightSettings = settings;
        }

        private void Awake()
        {
            _signalBus.Subscribe<StartBossFightSignal>(OnStartFight);
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<StartBossFightSignal>(OnStartFight);
        }

        private void OnStartFight()
        {
            _player = FindObjectOfType<Player.Player>();
            _boss = FindObjectOfType<Boss>();
            _bossPanel = FindObjectOfType<BossPanel>();
            _bossPanel.ShowUi();
            _playerKickCount = 0;
            _bossKickCount = 0;
            _progress = 0;
            _isFirstClick = true;
            _isBossFirstKick = true;
            _timeToKick = 3;
            _winCount = _bossFightSettings.HitCountToWin;
            Fight();
        }

        private void Fight()
        {
            _isHit = false;
            _isFight = true;
            _isFirstClick = true;
        }

        private void Update()
        {
            if (!_isFight)
            {
                return;
            }

            _timer += Time.deltaTime;
            _progress = _bossPanel.Progress;
            if (_timer >= _timeToKick && _progress < 0.1f && _isBossFirstKick && !_isHit)
            {
                _isBossFirstKick = false;
                _timeToKick = 1.5f;
                StartCoroutine(Hit(BossKick, PlayerHit));
            }
            else if (_timer >= _timeToKick && _progress < 0.1f && !_isHit)
            {
                _playerKickCountInRow = 0;
                _bossKickCountInRow = 0;
                StartCoroutine(Hit(BossKick, PlayerHit));
            }

            if (Input.GetMouseButtonDown(0))
            {
                _bossPanel.AddProgress();
                if (_isFirstClick)
                {
                    TryKickBoss();
                }
                else if (!_isHit)
                {
                    MakeDecision();
                }
            }
        }

        private void TryKickBoss()
        {
            _isFirstClick = false;
            if (!_isHit)
            {
                StartCoroutine(Hit(PlayerKick, BossHit));
            }
        }

        private void BossKick()
        {
            _bossKickCount++;
            _bossKickCountInRow++;
            _boss.MadePunch();
        }

        private void PlayerKick()
        {
            _playerKickCount++;
            _playerKickCountInRow++;
            if (_playerKickCount < _bossFightSettings.HitCountToWin)
            {
                _player.PunchBoss();
            }
            else
            {
                _player.FinalAttackBoss();
            }
        }

        private void BossHit()
        {
            _bossPanel.SetPlayerHeath(1f - _playerKickCount / _winCount);
            if (_playerKickCount < _bossFightSettings.HitCountToWin)
            {
                _boss.OnGetHit();
            }
            else
            {
                _boss.OnFail();
                StopFight(true);
            }
        }

        private void PlayerHit()
        {
            _bossPanel.SetBossValue(1f - _bossKickCount / _winCount);
            if (_bossKickCount < _bossFightSettings.HitCountToWin)
            {
                _player.GetHitFromBoss();
            }
            else
            {
                _boss.OnWin();
                StopFight(false);
            }
        }

        private IEnumerator Hit(Action action, Action callback)
        {
            _isBossFirstKick = false;
            _timer = 0;
            _isHit = true;
            action();
            yield return new WaitForSeconds(_bossFightSettings.HitDelay);
            callback();
            yield return new WaitForSeconds(_bossFightSettings.KickDelay);
            _isHit = false;
            MakeDecision();
        }

        private void MakeDecision()
        {
            if (_progress >= 0.5f && _playerKickCount < _bossKickCount)
            {
                StartCoroutine(Hit(PlayerKick, BossHit));
            }
            else if (_progress >= 0.5f && _playerKickCountInRow > 2)
            {
                _playerKickCountInRow = 0;
                StartCoroutine(Hit(BossKick, PlayerHit));
            }
            else if (_progress >= 0.5f && _playerKickCountInRow > 0 && _bossKickCountInRow > 0)
            {
                _bossKickCountInRow = 0;
                StartCoroutine(Hit(PlayerKick, BossHit));
            }
            else if (_progress >= 0.5f && _playerKickCountInRow >= 0)
            {
                StartCoroutine(Hit(PlayerKick, BossHit));
            }
            else if (_progress is < 0.5f and > 0.1f && _playerKickCountInRow == 0 && _bossKickCountInRow > 0)
            {
                _bossKickCountInRow = 0;
                StartCoroutine(Hit(PlayerKick, BossHit));
            }
            else if (_progress is < 0.5f and > 0.1f && _playerKickCountInRow > 0 && _bossKickCountInRow == 0)
            {
                StartCoroutine(Hit(BossKick, PlayerHit));
            }
            else if (_progress is < 0.5f and > 0.1f && _playerKickCountInRow > 0 && _bossKickCountInRow > 0)
            {
                _playerKickCountInRow = 0;
                StartCoroutine(Hit(BossKick, PlayerHit));
            }
            else if (_progress is < 0.5f and > 0.1f)
            {
                _playerKickCountInRow = 0;
                StartCoroutine(Hit(BossKick, PlayerHit));
            }
        }

        private void StopFight(bool isWin)
        {
            _isFight = false;
            StopAllCoroutines();

            if (isWin)
            {
                _player.OnBossWin(_bossFightSettings.WinPoints, _bossFightSettings.EndDelay);
            }
            else
            {
                _player.OnDeath();
            }
        }
    }
}