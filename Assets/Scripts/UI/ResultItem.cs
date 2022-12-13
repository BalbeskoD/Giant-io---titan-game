using TMPro;
using UnityEngine;

namespace UI
{
    public class ResultItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI name;
        [SerializeField] private TextMeshProUGUI points;

        public void Setup(string name, int points, Color playerColor)
        {
            this.name.text = name;
            this.points.text = points.ToString();
            if (name == "Player")
            {
                this.name.color = playerColor;
                this.points.color = playerColor;
            }
        }
    }
}