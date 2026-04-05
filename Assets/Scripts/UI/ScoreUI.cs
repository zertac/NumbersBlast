using UnityEngine;
using TMPro;
using DG.Tweening;
using NumbersBlast.Core;

namespace NumbersBlast.UI
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;

        private const float PunchStrength = 0.3f;
        private const float PunchDuration = 0.3f;
        private const int PunchVibrato = 2;
        private const float PunchElasticity = 0.5f;

        private int _currentScore;
        private RectTransform _rectTransform;

        public void Initialize()
        {
            _currentScore = 0;
            if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
            UpdateDisplay();
            GameEvents.OnScoreChanged += HandleScoreChanged;
        }

        private void OnDestroy()
        {
            GameEvents.OnScoreChanged -= HandleScoreChanged;
        }

        private void HandleScoreChanged(int points)
        {
            _currentScore += points;
            UpdateDisplay();
            _rectTransform.DOKill();
            _rectTransform.DOPunchScale(Vector3.one * PunchStrength, PunchDuration, PunchVibrato, PunchElasticity)
                .SetLink(gameObject);
        }

        private void UpdateDisplay()
        {
            _scoreText.text = _currentScore.ToString();
        }

        public int GetScore()
        {
            return _currentScore;
        }
    }
}
