using UnityEngine;
using VContainer;
using VContainer.Unity;
using NumbersBlast.Audio;
using NumbersBlast.Data;
using NumbersBlast.UI;

namespace NumbersBlast.DI
{
    /// <summary>
    /// DI scope for the main menu scene. Registers UI management, audio, and menu-specific components.
    /// </summary>
    public class MainMenuLifetimeScope : LifetimeScope
    {
        [SerializeField] private UIConfig _uiConfig;
        [SerializeField] private AudioConfig _audioConfig;
        [SerializeField] private Transform _popupContainer;
        [SerializeField] private MainMenuUI _mainMenuUI;

        protected override void Configure(IContainerBuilder builder)
        {
            if (_audioConfig != null)
                builder.RegisterInstance(_audioConfig);

            builder.Register<AudioManager>(Lifetime.Singleton);


            builder.Register<IUIManager, UIManager>(Lifetime.Singleton)
                .WithParameter("popupContainer", _popupContainer)
                .WithParameter("config", _uiConfig);

            builder.RegisterComponent(_mainMenuUI);
        }
    }
}
