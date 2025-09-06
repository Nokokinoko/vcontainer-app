using UnityEngine;
using VContainer;
using VContainer.Unity;
using VContainerApp.Presenter;
using VContainerApp.Services;
using VContainerApp.UI;

namespace VContainerApp
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private ScoreView ScoreView;
        [SerializeField] private ClickButton ClickButton;
        [SerializeField] private ShopView ShopView;

        protected override void Awake()
        {
            if (Parent == null)
            {
                base.Awake();
            }
        }

        protected override void Configure(IContainerBuilder builder)
        {
            // Services
            builder.Register<ScoreService>(Lifetime.Singleton);
            builder.Register<ItemService>(Lifetime.Singleton);
            builder.Register<SaveService>(Lifetime.Singleton);
            
            // Views
            builder.RegisterInstance(ScoreView);
            builder.RegisterInstance(ClickButton);
            builder.RegisterInstance(ShopView);
            
            // Presenters
            builder.RegisterEntryPoint<ScorePresenter>();
            builder.RegisterEntryPoint<ClickPresenter>();
            builder.RegisterEntryPoint<ShopPresenter>();
            builder.RegisterEntryPoint<GamePresenter>();
        }
    }
}
