using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Common
{
    public class LeaderboardItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI position;
        [SerializeField] private TextMeshProUGUI name;
        [SerializeField] private TextMeshProUGUI points;
        [SerializeField] private Image image;

        public void Setup(Leaderboard leaderboard, LeaderboardItemData data, bool isPlayer)
        {
            position.text = data.Position;
            name.text = data.Name;
            points.text = data.Points;
            if (isPlayer)
            {
                image.color = leaderboard.PlayerColor;
                position.color = leaderboard.OtherColor;
                name.color = leaderboard.OtherColor;
                points.color = leaderboard.OtherColor;
            }
            else
            {
                image.color = leaderboard.ImageColor;
                position.color = leaderboard.OtherColor;
                name.color = leaderboard.OtherColor;
                points.color = leaderboard.OtherColor;
            }
        }

        public void OnRestart()
        {
            position.text = string.Empty;
            name.text =string.Empty;
            points.text = string.Empty;
            points.color = Color.black;
        }
    }

    [Serializable]
    public class LeaderboardItemData
    {
        public string Position;
        public string Name;
        public string Points;
    }
}