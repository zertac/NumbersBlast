using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NumbersBlast.Core;
using NumbersBlast.StateMachine;

namespace NumbersBlast.UI
{
    /// <summary>
    /// Pause menu popup with resume, restart, and main menu options. Supports both single-player and multiplayer modes.
    /// </summary>
    public class PausePopup : BasePopup
    {
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _mainMenuButton;

        private GameStateManager _gameStateManager;
        private bool _isMultiplayer;

        protected override void Awake()
        {
            base.Awake();
            _resumeButton.onClick.AddListener(OnResume);
            _restartButton.onClick.AddListener(OnRestart);
            _mainMenuButton.onClick.AddListener(OnMainMenu);
        }

        /// <summary>
        /// Injects the game state manager used to pause and resume gameplay.
        /// </summary>
        public void SetGameStateManager(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
        }

        /// <summary>
        /// Sets whether the game is in multiplayer mode, which affects pause/resume behavior.
        /// </summary>
        public void SetMultiplayerMode(bool isMultiplayer)
        {
            _isMultiplayer = isMultiplayer;
        }

        private void OnResume()
        {
            Hide();
            if (!_isMultiplayer)
                _gameStateManager?.Resume();
        }

        private void OnRestart()
        {
            if (!_isMultiplayer)
                _gameStateManager?.Resume();
            SceneManager.LoadScene(GameConstants.GameScene);
        }

        private void OnMainMenu()
        {
            if (!_isMultiplayer)
                _gameStateManager?.Resume();
            SceneManager.LoadScene(GameConstants.MainMenuScene);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _resumeButton.onClick.RemoveListener(OnResume);
            _restartButton.onClick.RemoveListener(OnRestart);
            _mainMenuButton.onClick.RemoveListener(OnMainMenu);
        }
    }
}
