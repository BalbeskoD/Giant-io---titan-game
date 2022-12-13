using System;
using Cysharp.Threading.Tasks;
using Enums;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class MenuPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coinsText;
        private GameManager _gameManager;
        private Button _startButton;
        private bool _firstEnter;

        [Inject]
        public void Construct(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        private void Awake()
        {
            _startButton = GetComponentInChildren<Button>();
        }

        private void OnEnable()
        {
            coinsText.text = _gameManager.Coins.ToString();
        }

        private void OnDisable()
        {
            if (_firstEnter)
            {
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(true);
                }

                _firstEnter = false;
            }
        }

        private void Start()
        {
            _startButton.onClick.AddListener(OnStartButtonClick);
            if (PlayerPrefs.GetInt("FirstStart", 0) == 0)
            {
                _firstEnter = true;
                OnFirstEnter();
            }
        }

        private void OnDestroy()
        {
            _startButton.onClick.RemoveListener(OnStartButtonClick);
        }

        private void Update()
        {
            if (!_firstEnter)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                OnStartButtonClick();
                PlayerPrefs.SetInt("FirstStart", 1);
            }
        }

        private void OnStartButtonClick()
        {
            _gameManager.ChangeGameState(GameStates.Game);
        }

        private async void OnFirstEnter()
        {
          
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }

            await UniTask.Yield();
            
            FindObjectOfType<GamePanel>(true).gameObject.SetActive(true);
        }
    }
}