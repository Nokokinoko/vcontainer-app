using System;
using UniRx;
using UnityEngine;

namespace VContainerApp.Services
{
    public class ScoreService
    {
        private Subject<int> _scoreSubject = new();
        public IObservable<int> OnScoreChanged => _scoreSubject.AsObservable();
        
        private Subject<int> _clickPowerSubject = new();
        public IObservable<int> OnClickPowerChanged => _clickPowerSubject.AsObservable();

        public int Score { get; private set; } = 0;
        public int ClickPower { get; private set; } = 1;
        
        /// <summary>
        /// スコア加算
        /// </summary>
        public void AddScore(int amount = 1)
        {
            var add = amount * ClickPower;
            Score += add;
            _scoreSubject.OnNext(Score);
            
            Debug.Log($"Score increased by {add}, new score: {Score}");
        }

        /// <summary>
        /// ClickPower加算
        /// </summary>
        public void IncreaseClickPower(int amount)
        {
            ClickPower += amount;
            _clickPowerSubject.OnNext(ClickPower);
            
            Debug.Log($"Click power increased by {amount}, new click power: {ClickPower}");
        }
        
        /// <summary>
        /// 購入可否
        /// </summary>
        public bool CanAfford(int cost) => cost <= Score;

        /// <summary>
        /// スコア消費
        /// </summary>
        public bool SpendScore(int cost)
        {
            if (!CanAfford(cost))
            {
                return false;
            }

            Score -= cost;
            _scoreSubject.OnNext(Score);
            
            return true;
        }
        
        /// <summary>
        /// ロード
        /// </summary>
        public void LoadData(int score, int clickPower)
        {
            Score = score;
            ClickPower = clickPower;
            
            _scoreSubject.OnNext(Score);
            _clickPowerSubject.OnNext(ClickPower);
        }
        
        public void Dispose()
        {
            _scoreSubject?.Dispose();
            _clickPowerSubject?.Dispose();
        }
    }
}
