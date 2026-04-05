using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MultiplayerHUD : MonoBehaviour
{
    [SerializeField] private GameObject _multiplayerPanel;
    [SerializeField] private TextMeshProUGUI _playerScoreText;
    [SerializeField] private TextMeshProUGUI _opponentScoreText;
    [SerializeField] private TextMeshProUGUI _playerNameText;
    [SerializeField] private TextMeshProUGUI _opponentNameText;
    [SerializeField] private TextMeshProUGUI _turnIndicatorText;
    [SerializeField] private Image _timerBar;

    private int _playerScore;
    private int _opponentScore;

    public void Initialize(string opponentName)
    {
        _multiplayerPanel.SetActive(true);
        _playerNameText.text = "YOU";
        _opponentNameText.text = opponentName;
        _playerScore = 0;
        _opponentScore = 0;
        UpdateScores();
    }

    public void Hide()
    {
        _multiplayerPanel.SetActive(false);
    }

    public void SetPlayerTurn()
    {
        _turnIndicatorText.text = "YOUR TURN";
        _turnIndicatorText.color = new Color(0.3f, 0.85f, 0.4f);
        _turnIndicatorText.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 2);
    }

    public void SetOpponentTurn()
    {
        _turnIndicatorText.text = "OPPONENT'S TURN";
        _turnIndicatorText.color = new Color(0.9f, 0.5f, 0.3f);
        _turnIndicatorText.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 2);
    }

    public void UpdateTimer(float normalized)
    {
        float clamped = Mathf.Clamp01(normalized);
        _timerBar.fillAmount = clamped;

        if (clamped < 0.25f)
            _timerBar.color = Color.red;
        else if (clamped < 0.5f)
            _timerBar.color = Color.yellow;
        else
            _timerBar.color = Color.green;
    }

    public void AddPlayerScore(int points)
    {
        _playerScore += points;
        UpdateScores();
        _playerScoreText.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 2);
    }

    public void AddOpponentScore(int points)
    {
        _opponentScore += points;
        UpdateScores();
        _opponentScoreText.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 2);
    }

    public void ApplyPenalty(bool isPlayer, int penalty)
    {
        if (isPlayer)
        {
            _playerScore = Mathf.Max(0, _playerScore - penalty);
            _playerScoreText.color = Color.red;
            DOVirtual.DelayedCall(0.5f, () => _playerScoreText.color = Color.white);
        }
        else
        {
            _opponentScore = Mathf.Max(0, _opponentScore - penalty);
            _opponentScoreText.color = Color.red;
            DOVirtual.DelayedCall(0.5f, () => _opponentScoreText.color = Color.white);
        }
        UpdateScores();
    }

    public int PlayerScore => _playerScore;
    public int OpponentScore => _opponentScore;

    private void UpdateScores()
    {
        _playerScoreText.text = _playerScore.ToString();
        _opponentScoreText.text = _opponentScore.ToString();
    }
}
