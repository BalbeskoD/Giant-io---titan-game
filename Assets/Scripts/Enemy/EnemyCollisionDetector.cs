using Common;
using UnityEngine;
using Buildings;

namespace Enemy
{
    public class EnemyCollisionDetector : MonoBehaviour
    {
        private Enemy _player;
        private Level _level;

        private void Awake()
        {
            _player = GetComponent<Enemy>();
            _level = GetComponent<Level>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_player.IsActive)
                return;
            var people = other.GetComponentInParent<Stickman.Stickman>();
            if (people && !people.IsDead)
            {
                _player.PlayHitStickmanAnimation();
            }

            var build = other.GetComponent<BuildItem>();
            if (build)
            {
                _player.PlayHitBuildingAnimation(build);
            }

            var enemy = other.GetComponentInParent<Level>();
            if (enemy && enemy.Value < _level.Value)
            {
                
                var titan = enemy.GetComponent<Enemy>();
                if (titan)
                {
                    _player.KillTitan(titan);
                }
                var player = enemy.GetComponent<Player.Player>();
                if (player)
                {
                    _player.KillPlayer(player);
                }

                _player.AddPoints(enemy.Value);
            }
        }
    }
}