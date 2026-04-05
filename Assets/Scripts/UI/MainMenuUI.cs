using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using VContainer;
using NumbersBlast.Audio;
using NumbersBlast.Core;
using NumbersBlast.Multiplayer;

namespace NumbersBlast.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _multiplayerButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _exitButton;

        [Inject] private AudioManager _audioManager;
        [Inject] private UIManager _uiManager;

        private void Awake()
        {
            _playButton.onClick.AddListener(OnPlay);

            if (_multiplayerButton != null)
                _multiplayerButton.onClick.AddListener(OnMultiplayer);

            if (_settingsButton != null)
                _settingsButton.onClick.AddListener(OnSettings);

            if (_exitButton != null)
                _exitButton.onClick.AddListener(OnExit);
        }

        private void Start()
        {
            Application.runInBackground = false;
            _audioManager?.StopMusic();
            _audioManager?.PlayMenuMusic();
        }

        private void OnDestroy()
        {
            _playButton.onClick.RemoveListener(OnPlay);

            if (_multiplayerButton != null)
                _multiplayerButton.onClick.RemoveListener(OnMultiplayer);

            if (_settingsButton != null)
                _settingsButton.onClick.RemoveListener(OnSettings);

            if (_exitButton != null)
                _exitButton.onClick.RemoveListener(OnExit);
        }

        private void OnPlay()
        {
            _audioManager?.PlayButtonClick();
            GameModeHolder.Instance.CurrentMode = GameMode.SinglePlayer;
            SceneManager.LoadScene(GameConstants.GameScene);
        }

        private void OnMultiplayer()
        {
            _audioManager?.PlayButtonClick();
            GameModeHolder.Instance.CurrentMode = GameMode.Multiplayer;
            SceneManager.LoadScene(GameConstants.GameScene);
        }

        private void OnSettings()
        {
            _audioManager?.PlayButtonClick();
            var popup = _uiManager.ShowPopup<SettingsPopup>();
            popup?.SetAudioManager(_audioManager);
        }

        private void OnExit()
        {
            _audioManager?.PlayButtonClick();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
