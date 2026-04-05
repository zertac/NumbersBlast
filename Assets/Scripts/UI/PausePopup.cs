using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PausePopup : BasePopup
{
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _mainMenuButton;

    private GameStateManager _gameStateManager;

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

    private void OnResume()
    {
        Hide();
        _gameStateManager?.Resume();
    }

    private void OnRestart()
    {
        _gameStateManager?.Resume();
        SceneManager.LoadScene("GameScene");
    }

    private void OnMainMenu()
    {
        _gameStateManager?.Resume();
        SceneManager.LoadScene("MainMenuScene");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _resumeButton.onClick.RemoveListener(OnResume);
        _restartButton.onClick.RemoveListener(OnRestart);
        _mainMenuButton.onClick.RemoveListener(OnMainMenu);
    }
}
