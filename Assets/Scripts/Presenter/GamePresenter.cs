using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using VContainerApp.Services;

namespace VContainerApp.Presenter
{
    public class GamePresenter : IStartable, IDisposable
    {
        [Inject] private ScoreService ScoreService;
        [Inject] private ItemService ItemService;
        [Inject] private SaveService SaveService;
        
        private const int IntervalAutoSaveSeconds = 30;
        
        private CompositeDisposable _disposables = new CompositeDisposable();
        
        public void Start()
        {
            Initialize();

            // 終了時に保存
            Observable.OnceApplicationQuit()
                .Subscribe(_ => SaveService.Save(ScoreService, ItemService))
                .AddTo(_disposables);
            
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            
            // CancellationTokenをDisposeでキャンセルするように登録
            _disposables.Add(Disposable.Create(() => {
                if (!token.IsCancellationRequested)
                {
                    cts.Cancel();
                }
                cts.Dispose();
            }));
            
            AutoSaveAsync(token).Forget();
            
            Debug.Log("GamePresenter initialized");
        }

        private void Initialize()
        {
            if (!SaveService.Load(ScoreService, ItemService))
            {
                Debug.Log("No save data found, starting new game");
            }
        }

        private async UniTaskVoid AutoSaveAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(IntervalAutoSaveSeconds), cancellationToken: token);

                if (token.IsCancellationRequested)
                {
                    break;
                }
                
                SaveService.Save(ScoreService, ItemService);
            }
        }

        public void Dispose() => _disposables?.Dispose();
    }
}