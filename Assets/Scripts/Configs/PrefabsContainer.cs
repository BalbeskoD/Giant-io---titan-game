using UI;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "PrefabsContainer", menuName = "Configs/PrefabsContainer", order = 0)]
    public class PrefabsContainer : ScriptableObject
    {
        [SerializeField] private Player.Player player;
        [SerializeField] private UiController uiController;

        public Player.Player Player => player;

        public UiController UIController => uiController;
    }
}