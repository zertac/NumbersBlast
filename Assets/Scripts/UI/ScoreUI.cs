using UnityEngine;
using TMPro;
using DG.Tweening;
using NumbersBlast.Core;

namespace NumbersBlast.UI
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;

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
            _rectTransform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 2, 0.5f)
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
