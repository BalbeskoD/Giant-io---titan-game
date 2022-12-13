using System;
using TMPro;
using UnityEngine;

namespace Enemy
{
    public class EnemyUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI name;
        [SerializeField] private TextMeshProUGUI level;

        private Transform _camera;

        private void Start()
        {
            _camera = FindObjectOfType<Camera>().transform;
        }

        public void Setup(string name, Color color)
        {
            this.name.text = name;
            this.name.color = color;
            level.text = "Lv.1";
            level.color = color;
        }

        public void OnLevelUp(int value)
        {
            level.text = $"Lv. {value + 1}";
        }

        private void LateUpdate()
        {
            var lookDirection = (transform.position - _camera.position).normalized;

            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }
}