using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using NumbersBlast.UI;

namespace NumbersBlast.Tutorial
{
    /// <summary>
    /// Popup displayed after a tutorial step is completed, showing a success message with a check icon animation.
    /// </summary>
    public class TutorialFeedbackPopup : BasePopup
    {
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Button _continueButton;
        [SerializeField] private RectTransform _checkIcon;

        private const float CheckScaleDuration = 0.4f;
        private const float CheckScaleDelay = 0.2f;
        private const float CheckPunchStrength = 0.15f;
        private const float CheckPunchDuration = 0.25f;
        private const int CheckPunchVibrato = 2;

        private System.Action _onContinue;

        protected override void Awake()
        {
            base.Awake();
            _continueButton.onClick.AddListener(HandleContinue);
        }

        /// <summary>
        /// Displays the popup with the given title and description, invoking onContinue when the player taps continue.
        /// </summary>
        public void ShowWithMessage(string title, string description, System.Action onContinue)
        {
            _onContinue = onContinue;
            _titleText.text = title;
            _descriptionText.text = description;
            Show();
            PlayCheckAnimation();
        }

        private void PlayCheckAnimation()
        {
            if (_checkIcon == null) return;

            _checkIcon.DOKill();
            _checkIcon.localScale = Vector3.zero;
            _checkIcon.DOScale(Vector3.one, CheckScaleDuration)
                .SetEase(DG.Tweening.Ease.OutBack)
                .SetDelay(CheckScaleDelay)
                .SetLink(_checkIcon.gameObject)
                .OnComplete(() =>
                {
                    _checkIcon.DOPunchScale(Vector3.one * CheckPunchStrength, CheckPunchDuration, CheckPunchVibrato)
                        .SetLink(_checkIcon.gameObject);
                });
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _continueButton.onClick.RemoveListener(HandleContinue);
        }

        private void HandleContinue()
        {
            Hide();
            _onContinue?.Invoke();
        }
    }
}
