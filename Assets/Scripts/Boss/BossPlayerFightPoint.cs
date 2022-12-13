using UnityEngine;

namespace Boss
{
    public class BossPlayerFightPoint : MonoBehaviour
    {
        [SerializeField] private Transform startPoint;
        [SerializeField] private Transform fightPoint;

        public Transform StartPoint => startPoint;

        public Transform FightPoint => fightPoint;
    }
}