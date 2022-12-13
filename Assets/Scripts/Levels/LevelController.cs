using System;
using System.Collections.Generic;
using Buildings;
using Player;
using UnityEngine;
using Zenject;
using Zenject.Signals;
using Random = UnityEngine.Random;

namespace Levels
{
    public class LevelController : MonoBehaviour
    {
        private PlayerSpawnPoint[] playerSpawnPoint;
        private SignalBus _signalBus;
        private readonly Dictionary<GameObject, StartState> allObjects = new Dictionary<GameObject, StartState>();

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void Awake()
        {
            playerSpawnPoint = FindObjectsOfType<PlayerSpawnPoint>();
            _signalBus.Subscribe<GameRestartSignal>(OnGameRestart);
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<GameRestartSignal>(OnGameRestart);
        }

        private void Start()
        {
            Transform t;
            var stickmans = FindObjectsOfType<Stickman.Stickman>();
            var titans = FindObjectsOfType<Enemy.Enemy>();
            var buildings = FindObjectsOfType<BuildItem>();
            foreach (var stickman in stickmans)
            {
                t = stickman.transform;
                allObjects.Add(stickman.gameObject, new StartState() {StartPosition = t.position, StartRotation = t.rotation, StartScale = t.localScale});
            }

            foreach (var enemy in titans)
            {
                t = enemy.transform;
                allObjects.Add(enemy.gameObject, new StartState() {StartPosition = t.position, StartRotation = t.rotation, StartScale = t.localScale});
            }

            foreach (var buildItem in buildings)
            {
                t = buildItem.transform;
                allObjects.Add(buildItem.gameObject, new StartState() {StartPosition = t.position, StartRotation = t.rotation, StartScale = t.localScale});
            }
        }

        private void OnGameRestart()
        {
            GameObject g;
            Transform t;
            StartState s;
            foreach (var pair in allObjects)
            {
                g = pair.Key;
                t = g.transform;
                s = pair.Value;
                g.SetActive(true);
                t.position = s.StartPosition;
                t.rotation = s.StartRotation;
                t.localScale = s.StartScale;
            }
        }

        public Transform GetRandomPoint()
        {
            return playerSpawnPoint[Random.Range(0, playerSpawnPoint.Length)].transform;
        }
    }

    [Serializable]
    public class StartState
    {
        public Vector3 StartPosition;
        public Quaternion StartRotation;
        public Vector3 StartScale;
    }
}