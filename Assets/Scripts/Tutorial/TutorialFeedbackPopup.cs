using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialFeedbackPopup : BasePopup
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Button _continueButton;

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
