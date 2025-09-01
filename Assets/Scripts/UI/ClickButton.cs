using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace VContainerApp.UI
{
    public class ClickButton : MonoBehaviour
    {
        [SerializeField] private Button Button;
        [SerializeField] private Transform Transform;

        private Subject<Unit> _clickedSubject = new();
        public IObservable<Unit> OnClicked => _clickedSubject.AsObservable();
        
        private CompositeDisposable _disposables = new();

        private void Awake()
        {
            Button.OnClickAsObservable()
                .Subscribe(_ => HandleClickAsync(destroyCancellationToken).Forget())
                .AddTo(_disposables);

            this.OnDestroyAsObservable()
                .Subscribe(_ => {
                    _clickedSubject?.Dispose();
                    _disposables?.Dispose();
                })
                .AddTo(this);
        }
        
        private async UniTask HandleClickAsync(CancellationToken token)
        {
            await PlayClickEffectAsync(token);
            
            _clickedSubject.OnNext(Unit.Default);
        }

        /// <summary>
        /// 押下アニメーション
        /// </summary>
        private async UniTask PlayClickEffectAsync(CancellationToken token)
        {
            // 簡易的に独自実装
            var originalScale = Transform.localScale;
            var targetScale = originalScale * 1.1f;
            
            // 拡大
            var elapsed = 0f;
            var duration = 0.5f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = elapsed / duration;
                Transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
                
                await UniTask.Yield(token);
            }

            // 縮小
            elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = elapsed / duration;
                Transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
                
                await UniTask.Yield(token);
            }
            
            Transform.localScale = originalScale;
        }
    }
}