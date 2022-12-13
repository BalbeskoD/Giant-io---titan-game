using Configs;
using UnityEngine;
using Zenject;

namespace Managers
{
    public class GameSetup: IInitializable
    {
        private readonly GameSettings _gameSettings;

        public GameSetup(GameSettings gameSettings)
        {
            _gameSettings = gameSettings;
        }

        public void Initialize()
        {
            Application.targetFrameRate = _gameSettings.TargetFps;
            Input.multiTouchEnabled = _gameSettings.MultiTouchEnable;
            QualitySettings.vSyncCount = 0;
            Physics.reuseCollisionCallbacks = true;
        }

       
    }
}