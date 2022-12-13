using System;
using Buildings;
using Common;
using UnityEngine;

namespace Player
{
    public class PlayerCollisionDetector : MonoBehaviour
    {
        private Player _player;
        private Level _level;

        private void Awake()
        {
            _player = GetComponent<Player>();
            _level = GetComponent<Level>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_player.IsActive)
                return;
            var people = other.GetComponentInParent<Stickman.Stickman>();
            if (people && !people.IsDead)
            {
                people.Death();
                _player.PlayHitAnimation(null);
            }

            var build = other.GetComponent<BuildItem>();
            if (build)
            {
                _player.PlayHitAnimation(build);
            }

            var enemyLevel = other.GetComponentInParent<Level>();
            if (enemyLevel && enemyLevel.Value < _level.Value)
            {
                var enemy = enemyLevel.GetComponentInParent<Enemy.Enemy>();
                _player.KillTitan(enemy);
            }
        }
    }
}