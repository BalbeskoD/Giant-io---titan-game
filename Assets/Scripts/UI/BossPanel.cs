using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BossPanel : MonoBehaviour
    {
        [SerializeField] private HealthBar player;
        [SerializeField] private HealthBar boss;
        [SerializeField] private Slider fightSlider;
        [SerializeField] private GameObject tutorial;

        public float Progress { get; private set; }
        private Tween _tween;

        private void OnEnable()
        {
            player.gameObject.SetActive(false);
            boss.gameObject.SetActive(false);
            Progress = 0;
            fightSlider.value = Progress;
            tutorial.SetActive(true);
        }

        public void SetPlayerHeath(float value)
        {
            player.ChangeValue(value);
        }

        public void SetBossValue(float value)
        {
            boss.ChangeValue(value);
        }

        public void ShowUi()
        {
            player.gameObject.SetActive(true);
            boss.gameObject.SetActive(true);
        }

        private void Update()
        {
            if (Progress > 0)
            {
                fightSlider.value = Progress;
                Progress -= Time.deltaTime * 0.5f;
            }
        }

        public void AddProgress()
        {
            if (tutorial.activeInHierarchy)
            {
                tutorial.SetActive(false);
            }
            
            _tween?.Kill(true);
            Progress = Mathf.MoveTowards(Progress, 0.93f, 0.2f);
           
        }
    }
}