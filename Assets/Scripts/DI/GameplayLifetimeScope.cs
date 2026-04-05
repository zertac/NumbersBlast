using UnityEngine;
using VContainer;
using VContainer.Unity;
using NumbersBlast.Audio;
using NumbersBlast.Board;
using NumbersBlast.Core;
using NumbersBlast.Data;
using NumbersBlast.Feedback;
using NumbersBlast.Gameplay;
using NumbersBlast.Multiplayer;
using NumbersBlast.Piece;
using NumbersBlast.StateMachine;
using NumbersBlast.Tutorial;
using NumbersBlast.UI;

namespace NumbersBlast.DI
{
    public class GameplayLifetimeScope : LifetimeScope
    {
        [SerializeField] private BoardConfig _boardConfig;
        [SerializeField] private BoardView _boardView;
        [SerializeField] private PieceTray _pieceTray;
        [SerializeField] private ScoreUI _scoreUI;
        [SerializeField] private GameplayHUD _gameplayHUD;
        [SerializeField] private TutorialOverlay _tutorialOverlay;
        [SerializeField] private TutorialConfig _tutorialConfig;
        [SerializeField] private FeedbackConfig _feedbackConfig;
        [SerializeField] private UIConfig _uiConfig;
        [SerializeField] private AudioConfig _audioConfig;
        [SerializeField] private Transform _popupContainer;
        [SerializeField] private MultiplayerConfig _multiplayerConfig;
        [SerializeField] private MultiplayerHUD _multiplayerHUD;
        [SerializeField] private OpponentVisualPlayer _opponentVisualPlayer;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_boardConfig);
            builder.RegisterInstance(_boardView);
            builder.RegisterInstance(_pieceTray);
            builder.RegisterInstance(_scoreUI);
            builder.RegisterInstance(_gameplayHUD);
            builder.RegisterInstance(_tutorialOverlay);
            builder.RegisterInstance(_tutorialConfig);
            builder.RegisterInstance(_feedbackConfig);

            if (_audioConfig != null)
                builder.RegisterInstance(_audioConfig);

            builder.Register<UIManager>(Lifetime.Singleton)
                .WithParameter("popupContainer", _popupContainer)
                .WithParameter("config", _uiConfig);

            // AudioManager: only register if not already provided by ProjectLifetimeScope
            if (AudioManager.Instance == null)
                builder.Register<AudioManager>(Lifetime.Singleton);
            builder.Register<FeedbackManager>(Lifetime.Singleton);
            builder.Register<GameStateManager>(Lifetime.Singleton);
            builder.Register<BoardManager>(Lifetime.Singleton);
            builder.Register<MergeResolver>(Lifetime.Singleton);
            builder.Register<LineClearResolver>(Lifetime.Singleton);
            builder.Register<PlacementHandler>(Lifetime.Singleton);
            builder.Register<TutorialManager>(Lifetime.Singleton);

            // Multiplayer
            builder.RegisterInstance(_multiplayerConfig);
            builder.RegisterInstance(_multiplayerHUD);
            builder.RegisterInstance(_opponentVisualPlayer);

            builder.Register<OpponentAI>(Lifetime.Singleton);
            builder.Register<TurnManager>(Lifetime.Singleton);
            builder.Register<MultiplayerManager>(Lifetime.Singleton);

            builder.RegisterEntryPoint<MultiplayerTickRunner>();

            builder.RegisterEntryPoint<GameplayInitializer>();
        }
    }
}
