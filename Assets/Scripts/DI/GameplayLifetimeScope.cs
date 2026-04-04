using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameplayLifetimeScope : LifetimeScope
{
    [SerializeField] private BoardConfig _boardConfig;
    [SerializeField] private BoardView _boardView;
    [SerializeField] private PieceTray _pieceTray;
    [SerializeField] private ScoreUI _scoreUI;
    [SerializeField] private GameOverUI _gameOverUI;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(_boardConfig);
        builder.RegisterInstance(_boardView);
        builder.RegisterInstance(_pieceTray);
        builder.RegisterInstance(_scoreUI);
        builder.RegisterInstance(_gameOverUI);

        builder.Register<BoardManager>(Lifetime.Singleton);
        builder.Register<MergeResolver>(Lifetime.Singleton);
        builder.Register<LineClearResolver>(Lifetime.Singleton);
        builder.Register<PlacementHandler>(Lifetime.Singleton);

        builder.RegisterEntryPoint<GameplayInitializer>();
    }
}
