using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private UIConfig _uiConfig;
    [SerializeField] private AudioConfig _audioConfig;
    [SerializeField] private Transform _popupContainer;

    private UIManager _uiManager;
    private AudioManager _audioManager;

    private void Awake()
    {
        if (_audioConfig != null)
            _audioManager = new AudioManager(_audioConfig);

        _uiManager = new UIManager(_uiConfig, _popupContainer);

        _playButton.onClick.AddListener(OnPlay);

        if (_settingsButton != null)
            _settingsButton.onClick.AddListener(OnSettings);

        if (_exitButton != null)
            _exitButton.onClick.AddListener(OnExit);
    }

    private void Start()
    {
        _audioManager?.StopMusic();
        _audioManager?.PlayMenuMusic();
    }

    private void OnDestroy()
    {
        _playButton.onClick.RemoveListener(OnPlay);

        if (_settingsButton != null)
            _settingsButton.onClick.RemoveListener(OnSettings);

        if (_exitButton != null)
            _exitButton.onClick.RemoveListener(OnExit);
    }

    private void OnPlay()
    {
        _audioManager?.PlayButtonClick();
        SceneManager.LoadScene("GameScene");
    }

    private void OnSettings()
    {
        _audioManager?.PlayButtonClick();
        _uiManager.ShowPopup(PopupType.Settings);

        var settingsPopup = _uiManager.GetPopup<SettingsPopup>(PopupType.Settings);
        if (settingsPopup != null)
            settingsPopup.SetAudioManager(_audioManager);
    }

    private void OnExit()
    {
        _audioManager?.PlayButtonClick();
        Application.Quit();
    }
}
