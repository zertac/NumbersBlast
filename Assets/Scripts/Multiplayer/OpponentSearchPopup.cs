using System;
using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class OpponentSearchPopup : BasePopup
{
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private TextMeshProUGUI _opponentNameText;
    [SerializeField] private UnityEngine.UI.Button _cancelButton;
    [SerializeField] private RectTransform _searchIcon;

    private MultiplayerConfig _config;
    private Action _onFound;
    private Action _onCancel;
    private Coroutine _searchCoroutine;
    private string _foundName;
    private bool _cancelled;
    private Tween _iconTween;

    public string FoundOpponentName => _foundName;

    protected override void Awake()
    {
        base.Awake();
        _cancelButton.onClick.AddListener(OnCancelClick);
    }

    public void StartSearch(MultiplayerConfig config, Action onFound, Action onCancel)
    {
        _config = config;
        _onFound = onFound;
        _onCancel = onCancel;
        _cancelled = false;

        _opponentNameText.text = "";
        _statusText.text = "Searching for opponent...";
        Show();
        StartSearchIconAnimation();

        _searchCoroutine = StartCoroutine(SearchSequence());
    }

    private IEnumerator SearchSequence()
    {
        float searchDuration = UnityEngine.Random.Range(_config.MinSearchDuration, _config.MaxSearchDuration);
        float elapsed = 0f;
        int dotCount = 0;

        // Cycle through random names
        while (elapsed < searchDuration)
        {
            if (_cancelled) yield break;

            dotCount = (dotCount + 1) % 4;
            string dots = new string('.', dotCount);
            _statusText.text = $"Searching{dots}";

            if (_config.FakeNames != null && _config.FakeNames.Length > 0)
            {
                _opponentNameText.text = _config.FakeNames[UnityEngine.Random.Range(0, _config.FakeNames.Length)];
                _opponentNameText.DOFade(0.3f, 0.1f).OnComplete(() => _opponentNameText.DOFade(1f, 0.1f));
            }

            yield return new WaitForSeconds(0.4f);
            elapsed += 0.4f;
        }

        // Found
        _foundName = _config.FakeNames != null && _config.FakeNames.Length > 0
            ? _config.FakeNames[UnityEngine.Random.Range(0, _config.FakeNames.Length)]
            : $"Player_{UnityEngine.Random.Range(1000, 9999)}";

        _opponentNameText.text = _foundName;
        _statusText.text = "Opponent found!";
        StopSearchIconAnimation();

        _opponentNameText.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 2);
        if (_searchIcon != null)
            _searchIcon.DOPunchScale(Vector3.one * 0.3f, 0.4f, 2);

        yield return new WaitForSeconds(1.5f);

        Hide();
        _onFound?.Invoke();
    }

    private void OnCancelClick()
    {
        _cancelled = true;
        StopSearchIconAnimation();
        if (_searchCoroutine != null)
            StopCoroutine(_searchCoroutine);
        Hide();
        _onCancel?.Invoke();
    }

    private void StartSearchIconAnimation()
    {
        if (_searchIcon == null) return;

        var seq = DOTween.Sequence();
        seq.Append(_searchIcon.DOAnchorPosX(_searchIcon.anchoredPosition.x + 30f, 0.6f).SetEase(Ease.InOutSine));
        seq.Append(_searchIcon.DOAnchorPosX(_searchIcon.anchoredPosition.x - 30f, 0.6f).SetEase(Ease.InOutSine));
        seq.Append(_searchIcon.DOAnchorPosX(_searchIcon.anchoredPosition.x, 0.4f).SetEase(Ease.InOutSine));
        seq.Join(_searchIcon.DOScale(Vector3.one * 1.1f, 0.3f).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo));
        seq.SetLoops(-1, LoopType.Restart);
        _iconTween = seq;
    }

    private void StopSearchIconAnimation()
    {
        _iconTween?.Kill();
        if (_searchIcon != null)
        {
            _searchIcon.localScale = Vector3.one;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _iconTween?.Kill();
        _cancelButton.onClick.RemoveListener(OnCancelClick);
    }
}
