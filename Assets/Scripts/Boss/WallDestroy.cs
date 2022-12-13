using System;
using Configs;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Zenject.Signals;

namespace Helpers
{
    public class WallDestroy : MonoBehaviour
    {
        [SerializeField] private GameObject wallPrefab;
        [SerializeField] private Transform destroyPoint;
        [SerializeField] private GameObject[] walls;

        private SignalBus _signalBus;
        private BossFightSettings _bossFightSettings;

        [Inject]
        public void Construct(SignalBus signalBus, BossFightSettings bossFightSettings)
        {
            _signalBus = signalBus;
            _bossFightSettings = bossFightSettings;
        }

        private void Awake()
        {
            //_signalBus.Subscribe<StartBossActionSignal>(OnBossStart);
            _signalBus.Subscribe<GameRestartSignal>(OnRestart);
        }

        private void OnDestroy()
        {
           //_signalBus.Unsubscribe<StartBossActionSignal>(OnBossStart);
           _signalBus.Unsubscribe<GameRestartSignal>(OnRestart);
        }

        private async void OnBossStart()
        {
            foreach (var wall in walls)
            {
                var rigs = wall.GetComponentsInChildren<Rigidbody>();
                foreach (var rig in rigs)
                {
                    rig.isKinematic = false;
                }
            }

            await UniTask.WaitForFixedUpdate();
            foreach (var wall in walls)
            {
                var rigs = wall.GetComponentsInChildren<Rigidbody>();
                var meshs = wall.GetComponentsInChildren<MeshRenderer>();
                foreach (var rig in rigs)
                {
                    rig.AddExplosionForce(1000, destroyPoint.position, 10);
                    
                }
                foreach (var mesh in meshs)
                {
                    Destroy(mesh, _bossFightSettings.PartsDestroyTime);

                }
                
            }
        }
        
        public async void DestroyWalls()
        {
            foreach (var wall in walls)
            {
                var rigs = wall.GetComponentsInChildren<Rigidbody>();
                foreach (var rig in rigs)
                {
                    rig.isKinematic = false;
                }
            }

            await UniTask.WaitForFixedUpdate();
            foreach (var wall in walls)
            {
                var rigs = wall.GetComponentsInChildren<Rigidbody>();
                var meshs = wall.GetComponentsInChildren<MeshRenderer>();
                foreach (var rig in rigs)
                {
                    rig.AddExplosionForce(1000, destroyPoint.position, 10);
                    
                }
                foreach (var mesh in meshs)
                {
                    Destroy(mesh, _bossFightSettings.PartsDestroyTime);

                }
                
            }
        }


        private void OnRestart()
        {
            foreach (var wall in walls)
            {
                Destroy(wall.transform.GetChild(0).gameObject);
                var i = Instantiate(wallPrefab, wall.transform);
                i.transform.localPosition = Vector3.zero;
                i.transform.rotation = wall.transform.rotation;
                i.transform.localScale = Vector3.one;
            }
        }
    }
}