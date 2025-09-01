using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using VContainerApp.Services;

namespace VContainerApp.UI
{
    public class ShopView : MonoBehaviour
    {
        [SerializeField] private GameObject ShopPanel;
        [SerializeField] private Button OpenButton;
        [SerializeField] private Button CloseButton;
        
        [Space]
        [SerializeField] private Transform ItemParent; // ScrollView/Viewport/Content
        [SerializeField] private GameObject ItemPrefab;
        
        private Subject<ShopItem> _itemPurchaseRequestedSubject = new();
        public IObservable<ShopItem> OnItemPurchaseRequested => _itemPurchaseRequestedSubject.AsObservable();

        private List<ShopItemUI> _itemUIs = new();
        private CompositeDisposable _disposables = new();

        private void Awake()
        {
            OpenButton.OnClickAsObservable()
                .Subscribe(_ => ShopPanel.SetActive(true))
                .AddTo(_disposables);

            CloseButton.OnClickAsObservable()
                .Subscribe(_ => ShopPanel.SetActive(false))
                .AddTo(_disposables);
            
            ShopPanel.SetActive(false);

            this.OnDestroyAsObservable()
                .Subscribe(_ => {
                    _itemPurchaseRequestedSubject?.Dispose();
                    _disposables?.Dispose();
                })
                .AddTo(this);
        }

        /// <summary>
        /// アイテムリスト更新
        /// </summary>
        public void UpdateItemList(List<ShopItem> items, int score)
        {
            // 削除
            _itemUIs.ForEach(itemUI => DestroyImmediate(itemUI.gameObject));
            _itemUIs.Clear();
            
            // 作成
            items.ForEach(item =>
            {
                var itemObj = Instantiate(ItemPrefab, ItemParent);
                var itemUI = itemObj.GetComponent<ShopItemUI>();

                if (itemUI != null)
                {
                    itemUI.Setup(item, item.Cost <= score);
                    
                    itemUI.OnPurchaseRequested
                        .Subscribe(_ => _itemPurchaseRequestedSubject.OnNext(item))
                        .AddTo(_disposables);
                    
                    _itemUIs.Add(itemUI);
                }
            });
        }
    }
}