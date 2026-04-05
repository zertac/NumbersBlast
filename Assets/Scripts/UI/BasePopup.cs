using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public abstract class BasePopup : MonoBehaviour
{
    [SerializeField] private Image _dimBackground;
    [SerializeField] private RectTransform _content;

    private CanvasGroup _canvasGroup;
    private Tween _showTween;
    private Tween _hideTween;

    private const float ShowDuration = 0.3f;
    private const float HideDuration = 0.2f;
    private float _dimTargetAlpha;

    protected virtual void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_dimBackground != null)
            _dimTargetAlpha = _dimBackground.color.a;
        gameObject.SetActive(false);
    }

    public virtual void Show()
    {
        _hideTween?.Kill();
        gameObject.SetActive(true);

        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;

        if (_content != null)
            _content.localScale = Vector3.one * 0.7f;

        if (_dimBackground != null)
        {
            var dimColor = _dimBackground.color;
            dimColor.a = 0f;
            _dimBackground.color = dimColor;
            _dimBackground.DOFade(_dimTargetAlpha, ShowDuration);
        }

        var sequence = DOTween.Sequence();
        sequence.Append(_canvasGroup.DOFade(1f, ShowDuration).SetEase(Ease.OutCubic));

        if (_content != null)
            sequence.Join(_content.DOScale(Vector3.one, ShowDuration).SetEase(Ease.OutBack));

        sequence.OnComplete(() => _canvasGroup.interactable = true);
        _showTween = sequence;

        OnShow();
    }

    public virtual void Hide()
    {
        _showTween?.Kill();
        _canvasGroup.interactable = false;

        var sequence = DOTween.Sequence();
        sequence.Append(_canvasGroup.DOFade(0f, HideDuration).SetEase(Ease.InCubic));

        if (_content != null)
            sequence.Join(_content.DOScale(Vector3.one * 0.7f, HideDuration).SetEase(Ease.InBack));

        sequence.OnComplete(() =>
        {
            gameObject.SetActive(false);
            OnHide();
        });
        _hideTween = sequence;
    }

    protected virtual void OnShow() { GameEvents.PopupOpened(); }
    protected virtual void OnHide() { GameEvents.PopupClosed(); }

    protected virtual void OnDestroy()
    {
        _showTween?.Kill();
        _hideTween?.Kill();
    }
}
