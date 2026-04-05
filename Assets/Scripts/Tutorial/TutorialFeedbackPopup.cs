using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using NumbersBlast.UI;

namespace NumbersBlast.Tutorial
{
    public class TutorialFeedbackPopup : BasePopup
    {
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Button _continueButton;
        [SerializeField] private RectTransform _checkIcon;

        private System.Action _onContinue;

        protected override void Awake()
        {
            base.Awake();
            _continueButton.onClick.AddListener(HandleContinue);
        }

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
            _checkIcon.DOScale(Vector3.one, 0.4f)
                .SetEase(DG.Tweening.Ease.OutBack)
                .SetDelay(0.2f)
                .SetLink(_checkIcon.gameObject)
                .OnComplete(() =>
                {
                    _checkIcon.DOPunchScale(Vector3.one * 0.15f, 0.25f, 2)
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
