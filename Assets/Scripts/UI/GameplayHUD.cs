using UnityEngine;
using UnityEngine.UI;

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
        _gameStateManager.Pause();
        var pausePopup = _uiManager.ShowPopup<PausePopup>();
        pausePopup?.SetGameStateManager(_gameStateManager);
    }

    private void OnSettings()
    {
        _uiManager.ShowPopup<SettingsPopup>();
    }
}
