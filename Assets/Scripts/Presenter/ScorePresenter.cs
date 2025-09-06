using System;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using VContainerApp.Services;
using VContainerApp.UI;

namespace VContainerApp.Presenter
{
    public class ScorePresenter : IStartable, IDisposable
    {
        [Inject] private ScoreService ScoreService;
        [Inject] private ScoreView ScoreView;
        
        private CompositeDisposable _disposables = new();
        
        public void Start()
        {
            ScoreService.OnScoreChanged
                .Subscribe(score => ScoreView.UpdateScore(score))
                .AddTo(_disposables);
            
            ScoreService.OnClickPowerChanged
                .Subscribe(power => ScoreView.UpdateClickPower(power))
                .AddTo(_disposables);
            
            ScoreView.UpdateScore(ScoreService.Score);
            ScoreView.UpdateClickPower(ScoreService.ClickPower);
            
            Debug.Log("ScorePresenter initialized");
        }
        
        public void Dispose() => _disposables?.Dispose();
    }
}
