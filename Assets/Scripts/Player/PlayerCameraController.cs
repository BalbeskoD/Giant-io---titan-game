using System;
using System.Threading.Tasks;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Helpers;
using UnityEngine;

namespace Player
{
    public class PlayerCameraController : MonoBehaviour
    {
        [SerializeField] private CameraShakeConfig buildShake;
        [SerializeField] private CameraShakeConfig stickmanShake;
        [SerializeField] private CameraShakeConfig killTitanShake;
        [SerializeField] private CameraShakeConfig bossShake;
        private SceneCameraContainer _sceneCameraContainer;
        private Player _player;

        private void Awake()
        {
            _sceneCameraContainer = FindObjectOfType<SceneCameraContainer>();
            _player = GetComponent<Player>();
        }

        private void Start()
        {
            _sceneCameraContainer.SetupCameraOnPlayer(_player.CameraPoint);
        }

        public void LevelUpView(Vector3 offset)
        {
            _sceneCameraContainer.LevelUp(offset);
        }

        public void ShakeOnBuild()
        {
            _sceneCameraContainer.Shake(buildShake);
        }

        public void ShakeOnStickman()
        {
            _sceneCameraContainer.Shake(stickmanShake);
        }

        public void ShakeOnKillTitan()
        {
            _sceneCameraContainer.Shake(killTitanShake);
        }

        public void ShakeOnBossHit()
        {
            _sceneCameraContainer.ShakeOnBoss(bossShake);
        }

        public void OnRestart()
        {
            _sceneCameraContainer.OnRestart();
            _sceneCameraContainer.SetupCameraOnPlayer(_player.CameraPoint);
        }

        public async UniTask MoveToBossStartAction()
        {
            await _sceneCameraContainer.StartBossAction();
        }

        public async UniTask MoveToFightPoint()
        {
            await _sceneCameraContainer.BossAction();
        }
        

            public async UniTask MoveToFightPoint2()
        {
            await _sceneCameraContainer.BossAction2();
        }

        public async UniTask MoveToFightPoint3()
        {
            await _sceneCameraContainer.BossAction3();
        }
        public void WinBoss()
        {
            _sceneCameraContainer.WinBoss();
        }
        
        public void LostBoss()
        {
            _sceneCameraContainer.LostBoss();
        }
    }

    [Serializable]
    public class CameraShakeConfig
    {
        [SerializeField] private Vector3 shake;
        [SerializeField] private int loopCount;
        [SerializeField] private float duration;

        public Vector3 Shake => shake;

        public int LoopCount => loopCount;

        public float Duration => duration;
    }
}