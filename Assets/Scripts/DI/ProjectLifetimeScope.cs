using UnityEngine;
using VContainer;
using VContainer.Unity;
using NumbersBlast.Audio;
using NumbersBlast.Data;
using NumbersBlast.Multiplayer;

namespace NumbersBlast.DI
{
    /// <summary>
    /// Root DI scope that registers project-wide singletons surviving across all scenes.
    /// </summary>
    public class ProjectLifetimeScope : LifetimeScope
    {
        [SerializeField] private AudioConfig _audioConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            if (_audioConfig != null)
                builder.RegisterInstance(_audioConfig);

            builder.Register<AudioManager>(Lifetime.Singleton);
            builder.Register<GameModeHolder>(Lifetime.Singleton);
        }
    }
}
