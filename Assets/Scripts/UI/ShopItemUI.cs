using System;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using VContainerApp.Services;

namespace VContainerApp.UI
{
    public class ShopItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI Name;
        [SerializeField] private TextMeshProUGUI Cost;
        [SerializeField] private TextMeshProUGUI Description;
        [SerializeField] private TextMeshProUGUI PurchaseCount;
        [SerializeField] private Button PurchaseButton;
        
        private Subject<Unit> _purchaseRequested = new();
        public IObservable<Unit> OnPurchaseRequested => _purchaseRequested.AsObservable();
        
        private CompositeDisposable _disposables = new();

        private void Awake()
        {
            PurchaseButton.OnClickAsObservable()
                .Subscribe(_ => _purchaseRequested.OnNext(Unit.Default))
                .AddTo(_disposables);

            this.OnDestroyAsObservable()
                .Subscribe(_ => {
                    _purchaseRequested?.Dispose();
                    _disposables?.Dispose();
                })
                .AddTo(this);
        }
        
        public void Setup(ShopItem item, bool canAfford)
        {
            Name.text = item.Name;
            Cost.text = $"Cost: {item.Cost}";
            Description.text = item.Description;
            PurchaseCount.text = $"Owned: {item.PurchaseCount}";

            PurchaseButton.interactable = canAfford;
        }
    }
}