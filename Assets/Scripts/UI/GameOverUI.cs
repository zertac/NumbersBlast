using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NumbersBlast.Core;

namespace NumbersBlast.UI
{
    /// <summary>
    /// Game over popup that displays the final score and provides a restart option.
    /// </summary>
    public class GameOverUI : BasePopup
    {
        [SerializeField] private TextMeshProUGUI _finalScoreText;
        [SerializeField] private Button _restartButton;

        protected override void Awake()
        {
            base.Awake();
            _restartButton.onClick.AddListener(HandleRestart);
        }

        /// <summary>
        /// Sets the final score value displayed on the game over screen.
        /// </summary>
        public void SetScore(int score)
        {
            _finalScoreText.text = StringCache.IntToString(score);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _restartButton.onClick.RemoveListener(HandleRestart);
        }

        private void HandleRestart()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(GameConstants.GameScene);
        }
    }
}
