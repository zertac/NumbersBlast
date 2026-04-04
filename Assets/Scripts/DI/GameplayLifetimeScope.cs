using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameplayLifetimeScope : LifetimeScope
{
    [SerializeField] private BoardConfig _boardConfig;
    [SerializeField] private BoardView _boardView;
    [SerializeField] private PieceTray _pieceTray;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(_boardConfig);
        builder.RegisterInstance(_boardView);
        builder.RegisterInstance(_pieceTray);

        builder.Register<BoardManager>(Lifetime.Singleton);
        builder.Register<MergeResolver>(Lifetime.Singleton);
        builder.Register<LineClearResolver>(Lifetime.Singleton);
        builder.Register<PlacementHandler>(Lifetime.Singleton);

        builder.RegisterEntryPoint<GameplayInitializer>();
    }
}
