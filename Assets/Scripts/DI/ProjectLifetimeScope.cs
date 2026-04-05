using UnityEngine;
using VContainer;
using VContainer.Unity;
using NumbersBlast.Audio;
using NumbersBlast.Data;

namespace NumbersBlast.DI
{
    public class ProjectLifetimeScope : LifetimeScope
    {
        [SerializeField] private AudioConfig _audioConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            if (_audioConfig != null)
                builder.RegisterInstance(_audioConfig);

            builder.Register<AudioManager>(Lifetime.Singleton);
        }
    }
}
