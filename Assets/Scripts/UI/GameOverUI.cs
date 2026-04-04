using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _finalScoreText;
    [SerializeField] private Button _restartButton;

    private CanvasGroup _canvasGroup;

    public void Initialize()
    {
        _canvasGroup = _panel.GetComponent<CanvasGroup>();
        _panel.SetActive(false);
        _restartButton.onClick.AddListener(HandleRestart);
        GameEvents.OnGameOver += Show;
    }

    private void OnDestroy()
    {
        _restartButton.onClick.RemoveListener(HandleRestart);
        GameEvents.OnGameOver -= Show;
    }

    private void Show()
    {
        var scoreUI = FindAnyObjectByType<ScoreUI>();
        int score = scoreUI != null ? scoreUI.GetScore() : 0;
        _finalScoreText.text = score.ToString();

        _panel.SetActive(true);
        _canvasGroup.alpha = 0f;
        _canvasGroup.DOFade(1f, 0.3f).SetEase(Ease.OutCubic);
        _panel.transform.localScale = Vector3.one * 0.8f;
        _panel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
    }

    private void HandleRestart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
