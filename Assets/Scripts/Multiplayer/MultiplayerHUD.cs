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
    [SerializeField] private RectTransform _timerParent;

    private int _playerScore;
    private int _opponentScore;
    private bool _isPulsing;

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

    private static readonly Color TimerFullColor = new(0.4f, 0.85f, 0.4f);
    private static readonly Color TimerMidColor = new(0.95f, 0.85f, 0.3f);
    private static readonly Color TimerLowColor = new(0.9f, 0.3f, 0.3f);

    public void UpdateTimer(float normalized)
    {
        float clamped = Mathf.Clamp01(normalized);
        _timerBar.fillAmount = clamped;

        if (clamped > 0.5f)
            _timerBar.color = Color.Lerp(TimerMidColor, TimerFullColor, (clamped - 0.5f) * 2f);
        else
            _timerBar.color = Color.Lerp(TimerLowColor, TimerMidColor, clamped * 2f);

        // Pulse when low
        if (_timerParent != null && clamped < TimerWarningThreshold && clamped > 0f)
        {
            if (!_isPulsing)
            {
                _isPulsing = true;
                _timerParent.DOKill();
                _timerParent.DOScale(Vector3.one * 1.05f, 0.3f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetLink(_timerParent.gameObject);
            }
        }
        else if (_isPulsing)
        {
            _isPulsing = false;
            _timerParent.DOKill();
            _timerParent.DOScale(Vector3.one, 0.15f)
                .SetLink(_timerParent.gameObject);
        }
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
        var scoreText = isPlayer ? _playerScoreText : _opponentScoreText;

        if (isPlayer)
            _playerScore = Mathf.Max(0, _playerScore - penalty);
        else
            _opponentScore = Mathf.Max(0, _opponentScore - penalty);

        UpdateScores();

        // Red flash
        scoreText.color = Color.red;
        DOVirtual.DelayedCall(PenaltyFlashDelay, () => scoreText.color = Color.white)
            .SetLink(scoreText.gameObject);

        // Shake
        scoreText.transform.DOKill();
        scoreText.transform.DOShakePosition(0.4f, 8f, 15, 90f, false, true, ShakeRandomnessMode.Harmonic)
            .SetLink(scoreText.gameObject);

        // Show penalty amount
        ShowPenaltyText(scoreText.transform, penalty);
    }

    private void ShowPenaltyText(Transform anchor, int amount)
    {
        var penaltyGo = new GameObject("PenaltyText", typeof(RectTransform), typeof(TextMeshProUGUI));
        penaltyGo.transform.SetParent(anchor.parent, false);
        penaltyGo.transform.position = anchor.position;

        var text = penaltyGo.GetComponent<TextMeshProUGUI>();
        text.text = $"-{amount}";
        text.fontSize = 28;
        text.fontStyle = FontStyles.Bold;
        text.color = Color.red;
        text.alignment = TextAlignmentOptions.Center;
        text.raycastTarget = false;

        var rect = penaltyGo.GetComponent<RectTransform>();

        // Float up and fade
        var seq = DOTween.Sequence();
        seq.Append(rect.DOAnchorPosY(rect.anchoredPosition.y + 80f, 1f).SetEase(Ease.OutCubic));
        seq.Join(text.DOFade(0f, 1f).SetEase(Ease.InCubic));
        seq.OnComplete(() => Destroy(penaltyGo));
        seq.SetLink(penaltyGo);
    }

    public int PlayerScore => _playerScore;
    public int OpponentScore => _opponentScore;

    private void UpdateScores()
    {
        _playerScoreText.text = _playerScore.ToString();
        _opponentScoreText.text = _opponentScore.ToString();
    }
}
