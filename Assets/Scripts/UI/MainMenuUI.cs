using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _exitButton;

    private void Awake()
    {
        _playButton.onClick.AddListener(OnPlay);

        if (_settingsButton != null)
            _settingsButton.onClick.AddListener(OnSettings);

        if (_exitButton != null)
            _exitButton.onClick.AddListener(OnExit);
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
        SceneManager.LoadScene("GameScene");
    }

    private void OnSettings()
    {
        // TODO: Settings popup
    }

    private void OnExit()
    {
        Application.Quit();
    }
}
