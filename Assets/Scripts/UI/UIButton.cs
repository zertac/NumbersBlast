using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(Image))]
public class UIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [SerializeField] private Button _button;

    private Vector3 _originalScale;
    private Tween _scaleTween;

    private const float PressScale = 0.92f;
    private const float PressDuration = 0.08f;
    private const float ReleaseDuration = 0.15f;

    private void Awake()
    {
        _originalScale = transform.localScale;
        if (_button == null)
            _button = GetComponent<Button>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_button.interactable) return;

        _scaleTween?.Kill();
        _scaleTween = transform.DOScale(_originalScale * PressScale, PressDuration)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true)
            .SetLink(gameObject);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _scaleTween?.Kill();
        _scaleTween = transform.DOScale(_originalScale, ReleaseDuration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true)
            .SetLink(gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_button.interactable) return;

        AudioManager.Instance?.PlayButtonClick();
        HapticManager.Light();
    }

    private void OnDestroy()
    {
        _scaleTween?.Kill();
    }
}
