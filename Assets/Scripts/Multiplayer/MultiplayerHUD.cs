using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace NumbersBlast.Multiplayer
{
    /// <summary>
    /// Displays the multiplayer UI including player/opponent scores, turn indicator, and countdown timer with animations.
    /// </summary>
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
        private const float TimerPulseScale = 1.05f;
        private const float TimerPulseResetDuration = 0.15f;
        private const float ShakeDuration = 0.4f;
        private const float ShakeStrength = 8f;
        private const int ShakeVibrato = 15;
        private const float ShakeRandomness = 90f;
        private const int PenaltyFontSize = 28;
        private const float PenaltyFloatDistance = 80f;
        private const float PenaltyAnimDuration = 1f;

        private static readonly Color PlayerTurnColor = new Color(0.3f, 0.85f, 0.4f);
        private static readonly Color OpponentTurnColor = new Color(0.9f, 0.5f, 0.3f);

        /// <summary>
        /// Shows the multiplayer panel and resets scores with the given opponent name.
        /// </summary>
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

        /// <summary>
        /// Switches the turn indicator to show the player's turn with a punch animation.
        /// </summary>
        public void SetPlayerTurn()
        {
            _turnIndicatorText.text = "YOUR TURN";
            _turnIndicatorText.color = PlayerTurnColor;
            _turnIndicatorText.transform.DOKill();
            _turnIndicatorText.transform.DOPunchScale(Vector3.one * TurnPunchScale, PunchDuration, PunchVibrato)
                .SetLink(_turnIndicatorText.gameObject);
        }

        /// <summary>
        /// Switches the turn indicator to show the opponent's turn with a punch animation.
        /// </summary>
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

        /// <summary>
        /// Updates the timer bar fill and color based on normalized remaining time, with a pulse effect when low.
        /// </summary>
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
                    _timerParent.DOScale(Vector3.one * TimerPulseScale, PunchDuration)
                        .SetEase(Ease.InOutSine)
                        .SetLoops(-1, LoopType.Yoyo)
                        .SetLink(_timerParent.gameObject);
                }
            }
            else if (_isPulsing)
            {
                _isPulsing = false;
                _timerParent.DOKill();
                _timerParent.DOScale(Vector3.one, TimerPulseResetDuration)
                    .SetLink(_timerParent.gameObject);
            }
        }

        /// <summary>
        /// Adds points to the player's score and plays a punch animation on the score text.
        /// </summary>
        public void AddPlayerScore(int points)
        {
            _playerScore += points;
            UpdateScores();
            _playerScoreText.transform.DOKill();
            _playerScoreText.transform.DOPunchScale(Vector3.one * ScorePunchScale, PunchDuration, PunchVibrato)
                .SetLink(_playerScoreText.gameObject);
        }

        /// <summary>
        /// Adds points to the opponent's score and plays a punch animation on the score text.
        /// </summary>
        public void AddOpponentScore(int points)
        {
            _opponentScore += points;
            UpdateScores();
            _opponentScoreText.transform.DOKill();
            _opponentScoreText.transform.DOPunchScale(Vector3.one * ScorePunchScale, PunchDuration, PunchVibrato)
                .SetLink(_opponentScoreText.gameObject);
        }

        /// <summary>
        /// Applies a score penalty with red flash, shake, and floating penalty text animation.
        /// </summary>
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
            scoreText.transform.DOShakePosition(ShakeDuration, ShakeStrength, ShakeVibrato, ShakeRandomness, false, true, ShakeRandomnessMode.Harmonic)
                .SetLink(scoreText.gameObject);

            // Show penalty amount
            ShowPenaltyText(scoreText.transform, penalty);
        }

        // Runtime GameObject creation is acceptable here: only called on turn timeout (rare event).
        private void ShowPenaltyText(Transform anchor, int amount)
        {
            var penaltyGo = new GameObject("PenaltyText", typeof(RectTransform), typeof(TextMeshProUGUI));
            penaltyGo.transform.SetParent(anchor.parent, false);
            penaltyGo.transform.position = anchor.position;

            var text = penaltyGo.GetComponent<TextMeshProUGUI>();
            text.text = $"-{amount}";
            text.fontSize = PenaltyFontSize;
            text.fontStyle = FontStyles.Bold;
            text.color = Color.red;
            text.alignment = TextAlignmentOptions.Center;
            text.raycastTarget = false;

            var rect = penaltyGo.GetComponent<RectTransform>();

            // Float up and fade
            var seq = DOTween.Sequence();
            seq.Append(rect.DOAnchorPosY(rect.anchoredPosition.y + PenaltyFloatDistance, PenaltyAnimDuration).SetEase(Ease.OutCubic));
            seq.Join(text.DOFade(0f, PenaltyAnimDuration).SetEase(Ease.InCubic));
            seq.OnComplete(() => Destroy(penaltyGo));
            seq.SetLink(penaltyGo);
        }

        public int PlayerScore => _playerScore;
        public int OpponentScore => _opponentScore;

        private int _lastDisplayedPlayerScore = -1;
        private int _lastDisplayedOpponentScore = -1;

        private void UpdateScores()
        {
            if (_playerScore != _lastDisplayedPlayerScore)
            {
                _playerScoreText.text = _playerScore.ToString();
                _lastDisplayedPlayerScore = _playerScore;
            }
            if (_opponentScore != _lastDisplayedOpponentScore)
            {
                _opponentScoreText.text = _opponentScore.ToString();
                _lastDisplayedOpponentScore = _opponentScore;
            }
        }
    }
}
