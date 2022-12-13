using System;
using Stickman;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Enemy
{
    public enum EnemyMoveType
    {
        None,
        MoveToPoint,
        MoveToNearestStickman,
        MoveToRandomStickman
    }

    public class EnemyMoveController : MonoBehaviour
    {
        [SerializeField] private EnemyMoveType moveType;
        [SerializeField] private float searchRadius = 10;
        [SerializeField] private LayerMask layer;
        [SerializeField] private LayerMask titan;
        private NavMeshAgent _agent;
        private bool _isActive;
        private Transform _targetObject;
        private Vector3 _targetPosition;
        private Common.Level _level;
        private StickmanMovePoints[] _points;
        private Common.Level _titanTarget;
        public bool HasPath => _agent.hasPath && (_targetObject || _titanTarget) && !_agent.pathPending;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _points = FindObjectsOfType<StickmanMovePoints>();
            _level = GetComponent<Common.Level>();
        }

        public void SetSpeed(float speed)
        {
            _agent.speed = speed;
        }

        public void StartMove()
        {
            _isActive = true;
            _agent.enabled = true;
            FindPath();
        }

        private void Update()
        {
            if (!_isActive)
                return;

            if (LookForTitan())
            {
                return;
            }


            if (!_agent.hasPath || !_targetObject || !_targetObject.gameObject.activeInHierarchy)
            {
                FindPath();

                return;
            }
            // else
            // {
            //     if (_agent.pathPending)
            //     {
            //     
            //         _agent.transform.Translate( _agent.transform.forward * (_agent.speed * Time.deltaTime));
            //     }
            // }

            if (moveType == EnemyMoveType.MoveToPoint)
            {
                if (_agent.remainingDistance <= _agent.stoppingDistance)
                {
                    FindPath();
                }
            }
            else if (moveType == EnemyMoveType.MoveToNearestStickman || moveType == EnemyMoveType.MoveToRandomStickman)
            {
                if (Vector3.SqrMagnitude(_targetPosition - _targetObject.position) > 1)
                {
                    FindPath();
                }
            }
        }

        private void CheckTargetTitan()
        {
            if (_titanTarget.Value >= _level.Value)
            {
                _titanTarget = null;
            }
            else
            {
                _agent.destination = _titanTarget.transform.position;
            }
        }

        private void FindPath()
        {
            switch (moveType)
            {
                case EnemyMoveType.None:
                    Debug.LogWarning("No move type for titan: " + gameObject.name, this);
                    break;
                case EnemyMoveType.MoveToPoint:
                    MoveToRandomPoint();
                    break;
                case EnemyMoveType.MoveToNearestStickman:
                    MoveToNearestBot();
                    break;
                case EnemyMoveType.MoveToRandomStickman:
                    MoveToRandomBot();
                    break;
            }
        }

        private bool LookForTitan()
        {
            var colliders = Physics.OverlapSphere(transform.position, searchRadius, titan);
            _titanTarget = null;
            if (colliders.Length > 1)
            {
                foreach (var col in colliders)
                {
                    var level = col.GetComponentInParent<Common.Level>();
                    if (level.Value < _level.Value)
                    {
                        _titanTarget = level;
                    }
                }

                if (_titanTarget)
                {
                    _agent.SetDestination(_titanTarget.transform.position);
                }
            }

            return _titanTarget != null;
        }

        private void MoveToRandomBot()
        {
            var colliders = Physics.OverlapSphere(transform.position, searchRadius, layer);

            if (colliders.Length > 0)
            {
                _targetObject = colliders[Random.Range(0, colliders.Length)].transform;
                _targetPosition = _targetObject.position;
            }

            _agent.SetDestination(_targetPosition);
        }

        private void MoveToNearestBot()
        {
            var pos = transform.position;
            var colliders = Physics.OverlapSphere(pos, searchRadius, layer);
            var max = float.MaxValue;
            foreach (var collider1 in colliders)
            {
                var dist = Vector3.SqrMagnitude(pos - collider1.transform.position);
                if (dist < max)
                {
                    max = dist;
                    _targetObject = collider1.transform;
                    _targetPosition = _targetObject.position;
                }
            }

            _agent.SetDestination(_targetPosition);
        }

        private void MoveToRandomPoint()
        {
            var point = _points[Random.Range(0, _points.Length)];
            _targetObject = point.transform;
            _targetPosition = _targetObject.position;
            _agent.SetDestination(_targetPosition);
        }

        public void StopMove()
        {
            _isActive = false;
            _agent.enabled = false;
        }
    }
}