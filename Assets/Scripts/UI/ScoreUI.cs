using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;

    private int _currentScore;

    public void Initialize()
    {
        _currentScore = 0;
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
