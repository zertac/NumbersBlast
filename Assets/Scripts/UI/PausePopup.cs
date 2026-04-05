using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NumbersBlast.Core;
using NumbersBlast.StateMachine;

namespace NumbersBlast.UI
{
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

        public void SetGameStateManager(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
        }

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
