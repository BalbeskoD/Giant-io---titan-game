using System;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Player;
using UnityEngine;

namespace Helpers
{
    public class SceneCameraContainer : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera gameCamera;
        [SerializeField] private CinemachineVirtualCamera startBossCamera;
        [SerializeField] private CinemachineVirtualCamera bossCamera;
        [SerializeField] private CinemachineVirtualCamera bossCamera2;
        [SerializeField] private CinemachineVirtualCamera bossCamera3;
        [SerializeField] private CinemachineVirtualCamera winBossCamera;
        [SerializeField] private CinemachineVirtualCamera lostBossCamera;

        private CinemachineTransposer _transposer;
        private CinemachineComposer _composer;
        private CinemachineComposer _bossComposer;
        private Tween _shakeTween;
        private Vector3 _startOffset;

        private Transform _player;

        public void SetupCameraOnPlayer(Transform player)
        {
            _player = player;
            var brain = GetComponent<CinemachineBrain>();
            brain.enabled = false;
            _transposer = gameCamera.GetCinemachineComponent<CinemachineTransposer>();
            _composer = gameCamera.GetCinemachineComponent<CinemachineComposer>();
            _bossComposer = bossCamera.GetCinemachineComponent<CinemachineComposer>();
           
            gameCamera.Follow = player;
            gameCamera.LookAt = player;
            gameCamera.gameObject.SetActive(true);
            brain.enabled = true;
            _startOffset = _transposer.m_FollowOffset;
            startBossCamera.gameObject.SetActive(false);
            bossCamera.gameObject.SetActive(false);
        }

        public void LevelUp(Vector3 vector3)
        {
            DOTween.To(() => _transposer.m_FollowOffset, x => _transposer.m_FollowOffset = x, vector3, 0.5f);
        }

        public void Shake(CameraShakeConfig config)
        {
            _shakeTween?.Kill();
            _shakeTween = DOTween.To(() => _composer.m_TrackedObjectOffset, x => _composer.m_TrackedObjectOffset = x, config.Shake, config.Duration).SetLoops(config.LoopCount, LoopType.Yoyo)
                .OnComplete(() => DOTween.To(() => _composer.m_TrackedObjectOffset, x => _composer.m_TrackedObjectOffset = x, Vector3.zero, config.Duration));
        }

        public async void OnRestart()
        {
            var brain = GetComponent<CinemachineBrain>();
            brain.enabled = false;
            await UniTask.Yield();
            _transposer.m_FollowOffset = new Vector3(0, 12, -10); 
            gameCamera.gameObject.SetActive(true);
            startBossCamera.gameObject.SetActive(false);
            bossCamera.gameObject.SetActive(false);
            winBossCamera.gameObject.SetActive(false);
            lostBossCamera.gameObject.SetActive(false);
        }

        public async UniTask StartBossAction()
        {
            bossCamera2.gameObject.SetActive(true);
            gameCamera.gameObject.SetActive(false);
            await UniTask.Delay(TimeSpan.FromSeconds(2));
        }

        public async UniTask BossAction()
        {
            bossCamera2.gameObject.SetActive(true);
            gameCamera.gameObject.SetActive(false);
            await UniTask.Delay(TimeSpan.FromSeconds(0.8f));
        }

        public async UniTask BossAction2()
        {
            bossCamera.gameObject.SetActive(true);
            bossCamera2.gameObject.SetActive(false);
            await UniTask.Delay(TimeSpan.FromSeconds(0.7f));
        }
        public async UniTask BossAction3()
        {
            bossCamera3.gameObject.SetActive(true);
            bossCamera.gameObject.SetActive(false);
            await UniTask.Delay(TimeSpan.FromSeconds(0.7f));
        }

        public void WinBoss()
        {
            bossCamera3.gameObject.SetActive(false);
            winBossCamera.gameObject.SetActive(true);
            winBossCamera.Follow = _player;
            winBossCamera.LookAt = _player;
        }
        
        public void LostBoss()
        {
            bossCamera.gameObject.SetActive(false);
            lostBossCamera.gameObject.SetActive(true);
            winBossCamera.Follow = _player;
            winBossCamera.LookAt = _player;
        }

        public void ShakeOnBoss(CameraShakeConfig config)
        {
            _shakeTween?.Kill();
            _shakeTween = DOTween.To(() => _bossComposer.m_TrackedObjectOffset, x => _bossComposer.m_TrackedObjectOffset = x, config.Shake, config.Duration).SetLoops(config.LoopCount, LoopType.Yoyo)
                .OnComplete(() => DOTween.To(() => _bossComposer.m_TrackedObjectOffset, x => _bossComposer.m_TrackedObjectOffset = x, Vector3.zero, config.Duration));
        }
    }
}