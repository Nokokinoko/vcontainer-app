using System;
using UnityEngine;

namespace VContainerApp.Services
{
    public class ScoreService
    {
        public event Action<int> OnScoreChanged;
        public event Action<int> OnClickPowerChanged;

        public int Score { get; private set; } = 0;
        public int ClickPower { get; private set; } = 1;
        
        public void AddScore(int amount = 1)
        {
            var add = amount * ClickPower;
            Score += add;
            OnScoreChanged?.Invoke(Score);
            
            Debug.Log($"Score increased by {add}, new score: {Score}");
        }

        public void IncreaseClickPower(int amount)
        {
            ClickPower += amount;
            OnClickPowerChanged?.Invoke(ClickPower);
            
            Debug.Log($"Click power increased by {amount}, new click power: {ClickPower}");
        }
        
        public bool CanAfford(int cost) => cost <= Score;

        public bool SpendScore(int cost)
        {
            if (!CanAfford(cost))
            {
                return false;
            }

            Score -= cost;
            OnScoreChanged?.Invoke(Score);
            
            return true;
        }
        
        public void LoadData(int score, int clickPower)
        {
            Score = score;
            ClickPower = clickPower;
            OnScoreChanged?.Invoke(Score);
            OnClickPowerChanged?.Invoke(ClickPower);
        }
    }
}
