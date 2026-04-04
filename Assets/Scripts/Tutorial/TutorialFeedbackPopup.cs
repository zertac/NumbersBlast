using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TutorialFeedbackPopup : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Button _continueButton;

    private CanvasGroup _canvasGroup;
    private System.Action _onContinue;

    public void Initialize()
    {
        _canvasGroup = _panel.GetComponent<CanvasGroup>();
        _panel.SetActive(false);
        _continueButton.onClick.AddListener(HandleContinue);
    }

    private void OnDestroy()
    {
        _continueButton.onClick.RemoveListener(HandleContinue);
    }

    public void Show(string title, string description, System.Action onContinue)
    {
        _onContinue = onContinue;
        _titleText.text = title;
        _descriptionText.text = description;

        _panel.SetActive(true);
        _canvasGroup.alpha = 0f;
        _canvasGroup.DOFade(1f, 0.25f).SetEase(Ease.OutCubic);
        _panel.transform.localScale = Vector3.one * 0.8f;
        _panel.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack);
    }

    public void Hide()
    {
        _panel.SetActive(false);
    }

    private void HandleContinue()
    {
        _panel.SetActive(false);
        _onContinue?.Invoke();
    }
}
