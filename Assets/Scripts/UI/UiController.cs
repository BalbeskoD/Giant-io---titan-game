using System;
using Enums;
using UnityEngine;

namespace UI
{
    public class UiController : MonoBehaviour
    {
        private MenuPanel _menuPanel;
        private GamePanel _gamePanel;
        private LostPanel _lostPanel;
        private WinPanel _winPanel;
        private BossPanel _bossPanel;
        
        public Joystick Joystick { get; private set; }

        private void Awake()
        {
            _menuPanel = GetComponentInChildren<MenuPanel>(true);
            _gamePanel = GetComponentInChildren<GamePanel>(true);
            _lostPanel = GetComponentInChildren<LostPanel>(true);
            _winPanel = GetComponentInChildren<WinPanel>(true);
            _bossPanel = GetComponentInChildren<BossPanel>(true);
            Joystick = GetComponentInChildren<Joystick>();
        }

        public void OnGameStateChange(GameStates gameStates)
        {
            _menuPanel.gameObject.SetActive(gameStates == GameStates.Menu);
            _gamePanel.gameObject.SetActive(gameStates == GameStates.Game);
            _lostPanel.gameObject.SetActive(gameStates == GameStates.Lost);
            _winPanel.gameObject.SetActive(gameStates == GameStates.Win);
            _bossPanel.gameObject.SetActive(gameStates == GameStates.Boss);
        }
    }
}