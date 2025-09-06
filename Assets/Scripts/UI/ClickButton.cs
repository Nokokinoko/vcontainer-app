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
        
        Vector3 _originalScale;

        private Subject<Unit> _clickedSubject = new();
        public IObservable<Unit> OnClicked => _clickedSubject.AsObservable();
        
        private CompositeDisposable _disposables = new();
        
        private const float DurationClickEffect = 0.2f;

        private void Awake()
        {
            _originalScale = Transform.localScale;
            
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
            var targetScale = _originalScale * 1.1f;
            
            // 拡大
            var elapsed = 0f;
            while (elapsed < DurationClickEffect)
            {
                elapsed += Time.deltaTime;
                var t = elapsed / DurationClickEffect;
                Transform.localScale = Vector3.Lerp(_originalScale, targetScale, t);
                
                await UniTask.Yield(token);
            }

            // 縮小
            elapsed = 0f;
            while (elapsed < DurationClickEffect)
            {
                elapsed += Time.deltaTime;
                var t = elapsed / DurationClickEffect;
                Transform.localScale = Vector3.Lerp(targetScale, _originalScale, t);
                
                await UniTask.Yield(token);
            }
            
            Transform.localScale = _originalScale;
        }
    }
}