using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace VContainerApp.Services
{
    [Serializable]
    public class ShopItem
    {
        public string Name;
        public int BaseCost;
        public int ClickPowerIncrease;
        public string Description;
        public int PurchaseCount = 0;
        
        public int Cost => Mathf.RoundToInt(BaseCost * Mathf.Pow(1.15f, PurchaseCount));
    }
    
    public class ItemService
    {
        private Subject<Unit> _itemsUpdatedSubject = new();
        public IObservable<Unit> OnItemsUpdated => _itemsUpdatedSubject.AsObservable();
        
        public List<ShopItem> Items { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ItemService()
        {
            InitializeItems();
        }
        
        /// <summary>
        /// 初期化
        /// </summary>
        private void InitializeItems()
        {
            Items = new List<ShopItem>
            {
                new ShopItem { Name = "Better Click", BaseCost = 10, ClickPowerIncrease = 1, Description = "Click Power +1" },
                new ShopItem { Name = "Strong Finger", BaseCost = 50, ClickPowerIncrease = 5, Description = "Click Power +5" },
                new ShopItem { Name = "Power Glove", BaseCost = 200, ClickPowerIncrease = 20, Description = "Click Power +20" },
                new ShopItem { Name = "Mega Clicker", BaseCost = 1000, ClickPowerIncrease = 100, Description = "Click Power +100" },
            };
        }
        
        /// <summary>
        /// アイテム購入
        /// </summary>
        public bool PurchaseItem(ShopItem item, ScoreService scoreService)
        {
            if (!scoreService.CanAfford(item.Cost))
            {
                Debug.Log($"Cannot afford {item.Name}. Cost: {item.Cost}, Score: {scoreService.Score}");
                return false;
            }

            if (!scoreService.SpendScore(item.Cost))
            {
                return false;
            }
            
            scoreService.IncreaseClickPower(item.ClickPowerIncrease);
            item.PurchaseCount++;
            
            Debug.Log($"Purchased {item.Name}! Power increased by {item.ClickPowerIncrease}");
            
            _itemsUpdatedSubject.OnNext(Unit.Default);
            return true;
        }

        /// <summary>
        /// ロード
        /// </summary>
        public void LoadData(List<int> purchaseCounts)
        {
            for (int i = 0; i < Items.Count && i < purchaseCounts.Count; i++)
            {
                Items[i].PurchaseCount = purchaseCounts[i];
            }
            
            _itemsUpdatedSubject.OnNext(Unit.Default);
        }
        
        /// <summary>
        /// 購入回数
        /// </summary>
        public List<int> GetPurchaseCounts()
        {
            var counts = new List<int>();
            Items.ForEach(item => counts.Add(item.PurchaseCount));
            return counts;
        }

        public void Dispose()
        {
            _itemsUpdatedSubject?.Dispose();
        }
    }
}
