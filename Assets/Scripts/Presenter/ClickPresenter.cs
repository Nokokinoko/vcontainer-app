using System;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using VContainerApp.Services;
using VContainerApp.UI;

namespace VContainerApp.Presenter
{
    public class ClickPresenter : IStartable, IDisposable
    {
        [Inject] private ScoreService ScoreService;
        [Inject] private ClickButton ClickButton;
        [Inject] private SaveService SaveService;
        [Inject] private ItemService ItemService;
        
        private CompositeDisposable _disposables = new();
        
        public void Start()
        {
            ClickButton.OnClicked
                .Subscribe(_ => HandleClick())
                .AddTo(_disposables);
            
            Debug.Log("ClickPresenter initialized");
        }

        private void HandleClick()
        {
            // スコア加算
            ScoreService.AddScore();
            
            // 定期的な自動保存
            if (ScoreService.Score % 100 == 0)
            {
                SaveService.Save(ScoreService, ItemService);
                Debug.Log("Game auto-saved");
            }
        }
        
        public void Dispose() => _disposables?.Dispose();
    }
}
