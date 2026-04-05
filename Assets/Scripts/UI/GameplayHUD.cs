using UnityEngine;
using UnityEngine.UI;
using NumbersBlast.Multiplayer;
using NumbersBlast.StateMachine;

namespace NumbersBlast.UI
{
    public class GameplayHUD : MonoBehaviour
    {
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _settingsButton;

        private UIManager _uiManager;
        private GameStateManager _gameStateManager;

        public void Initialize(UIManager uiManager, GameStateManager gameStateManager)
        {
            _uiManager = uiManager;
            _gameStateManager = gameStateManager;

            _pauseButton.onClick.AddListener(OnPause);
            _settingsButton.onClick.AddListener(OnSettings);
        }

        private void OnDestroy()
        {
            _pauseButton.onClick.RemoveListener(OnPause);
            _settingsButton.onClick.RemoveListener(OnSettings);
        }

        private void OnPause()
        {
            bool isMultiplayer = GameModeHolder.Instance.CurrentMode == GameMode.Multiplayer;

            // Multiplayer: just show popup, game continues in background
            // Single player: pause game state
            if (!isMultiplayer)
                _gameStateManager.Pause();

            var pausePopup = _uiManager.ShowPopup<PausePopup>();
            pausePopup?.SetGameStateManager(_gameStateManager);
            pausePopup?.SetMultiplayerMode(isMultiplayer);
        }

        private void OnSettings()
        {
            _uiManager.ShowPopup<SettingsPopup>();
        }
    }
}
