using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NumbersBlast.Core;

namespace NumbersBlast.UI
{
    public class GameOverUI : BasePopup
    {
        [SerializeField] private TextMeshProUGUI _finalScoreText;
        [SerializeField] private Button _restartButton;

        protected override void Awake()
        {
            base.Awake();
            _restartButton.onClick.AddListener(HandleRestart);
        }

        public void SetScore(int score)
        {
            _finalScoreText.text = score.ToString();
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
