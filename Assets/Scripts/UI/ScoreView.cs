using TMPro;
using UnityEngine;

namespace VContainerApp.UI
{
    public class ScoreView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI ScoreText;
        [SerializeField] private TextMeshProUGUI ClickPowerText;
        
        public void UpdateScore(int score) => ScoreText.text = $"Score: {score:N0}";
        public void UpdateClickPower(int clickPower) => ClickPowerText.text = $"Power: {clickPower:N0}";
    }
}
