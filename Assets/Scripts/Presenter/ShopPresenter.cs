using System;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using VContainerApp.Services;
using VContainerApp.UI;

namespace VContainerApp.Presenter
{
    public class ShopPresenter : IStartable, IDisposable
    {
        [Inject] private ScoreService ScoreService;
        [Inject] private ItemService ItemService;
        [Inject] private ShopView ShopView;
        [Inject] private SaveService SaveService;
        
        private CompositeDisposable _disposables = new();
        
        public void Start()
        {
            ShopView.OnItemPurchaseRequested
                .Subscribe(item => HandleItemPurchase(item))
                .AddTo(_disposables);

            ScoreService.OnScoreChanged
                .Subscribe(_ => UpdateShop())
                .AddTo(_disposables);
            
            ItemService.OnItemsUpdated
                .Subscribe(_ => UpdateShop())
                .AddTo(_disposables);
            
            UpdateShop();
            Debug.Log("ShopPresenter initialized");
        }

        private void HandleItemPurchase(ShopItem item)
        {
            if (ItemService.PurchaseItem(item, ScoreService))
            {
                // 購入成功
                SaveService.Save(ScoreService, ItemService);
                
                // UI更新はObservableで行う
                Debug.Log($"Successfully purchased {item.Name}");
            }
            else
            {
                // 購入失敗
                Debug.Log($"Failed to purchase {item.Name}");
            }
        }
        
        private void UpdateShop() => ShopView.UpdateItemList(ItemService.Items, ScoreService.Score);

        public void Dispose() => _disposables?.Dispose();
    }
}
