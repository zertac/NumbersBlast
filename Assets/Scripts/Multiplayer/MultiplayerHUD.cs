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

    private const float TimerCriticalThreshold = 0.25f;
    private const float TimerWarningThreshold = 0.5f;
    private const float TurnPunchScale = 0.2f;
    private const float ScorePunchScale = 0.3f;
    private const float PunchDuration = 0.3f;
    private const int PunchVibrato = 2;
    private const float PenaltyFlashDelay = 0.5f;

    private static readonly Color PlayerTurnColor = new Color(0.3f, 0.85f, 0.4f);
    private static readonly Color OpponentTurnColor = new Color(0.9f, 0.5f, 0.3f);

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
        _turnIndicatorText.color = PlayerTurnColor;
        _turnIndicatorText.transform.DOKill();
        _turnIndicatorText.transform.DOPunchScale(Vector3.one * TurnPunchScale, PunchDuration, PunchVibrato)
            .SetLink(_turnIndicatorText.gameObject);
    }

    public void SetOpponentTurn()
    {
        _turnIndicatorText.text = "OPPONENT'S TURN";
        _turnIndicatorText.color = OpponentTurnColor;
        _turnIndicatorText.transform.DOKill();
        _turnIndicatorText.transform.DOPunchScale(Vector3.one * TurnPunchScale, PunchDuration, PunchVibrato)
            .SetLink(_turnIndicatorText.gameObject);
    }

    public void UpdateTimer(float normalized)
    {
        float clamped = Mathf.Clamp01(normalized);
        _timerBar.fillAmount = clamped;

        if (clamped < TimerCriticalThreshold)
            _timerBar.color = Color.red;
        else if (clamped < TimerWarningThreshold)
            _timerBar.color = Color.yellow;
        else
            _timerBar.color = Color.green;
    }

    public void AddPlayerScore(int points)
    {
        _playerScore += points;
        UpdateScores();
        _playerScoreText.transform.DOKill();
        _playerScoreText.transform.DOPunchScale(Vector3.one * ScorePunchScale, PunchDuration, PunchVibrato)
            .SetLink(_playerScoreText.gameObject);
    }

    public void AddOpponentScore(int points)
    {
        _opponentScore += points;
        UpdateScores();
        _opponentScoreText.transform.DOKill();
        _opponentScoreText.transform.DOPunchScale(Vector3.one * ScorePunchScale, PunchDuration, PunchVibrato)
            .SetLink(_opponentScoreText.gameObject);
    }

    public void ApplyPenalty(bool isPlayer, int penalty)
    {
        if (isPlayer)
        {
            _playerScore = Mathf.Max(0, _playerScore - penalty);
            _playerScoreText.color = Color.red;
            DOVirtual.DelayedCall(PenaltyFlashDelay, () => _playerScoreText.color = Color.white)
                .SetLink(_playerScoreText.gameObject);
        }
        else
        {
            _opponentScore = Mathf.Max(0, _opponentScore - penalty);
            _opponentScoreText.color = Color.red;
            DOVirtual.DelayedCall(PenaltyFlashDelay, () => _opponentScoreText.color = Color.white)
                .SetLink(_opponentScoreText.gameObject);
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
