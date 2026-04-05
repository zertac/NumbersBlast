using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameplayLifetimeScope : LifetimeScope
{
    [SerializeField] private BoardConfig _boardConfig;
    [SerializeField] private BoardView _boardView;
    [SerializeField] private PieceTray _pieceTray;
    [SerializeField] private ScoreUI _scoreUI;
    [SerializeField] private TutorialOverlay _tutorialOverlay;
    [SerializeField] private TutorialConfig _tutorialConfig;
    [SerializeField] private FeedbackConfig _feedbackConfig;
    [SerializeField] private UIConfig _uiConfig;
    [SerializeField] private Transform _popupContainer;
    [SerializeField] private AudioConfig _audioConfig;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(_boardConfig);
        builder.RegisterInstance(_boardView);
        builder.RegisterInstance(_pieceTray);
        builder.RegisterInstance(_scoreUI);
        builder.RegisterInstance(_tutorialOverlay);
        builder.RegisterInstance(_tutorialConfig);
        builder.RegisterInstance(_feedbackConfig);

        if (_audioConfig != null)
            builder.RegisterInstance(_audioConfig);

        builder.Register<AudioManager>(Lifetime.Singleton);
        builder.Register<UIManager>(Lifetime.Singleton)
            .WithParameter("popupContainer", _popupContainer)
            .WithParameter("config", _uiConfig);

        builder.Register<FeedbackManager>(Lifetime.Singleton);
        builder.Register<GameStateManager>(Lifetime.Singleton);
        builder.Register<BoardManager>(Lifetime.Singleton);
        builder.Register<MergeResolver>(Lifetime.Singleton);
        builder.Register<LineClearResolver>(Lifetime.Singleton);
        builder.Register<PlacementHandler>(Lifetime.Singleton);
        builder.Register<TutorialManager>(Lifetime.Singleton);

        builder.RegisterEntryPoint<GameplayInitializer>();
    }
}
