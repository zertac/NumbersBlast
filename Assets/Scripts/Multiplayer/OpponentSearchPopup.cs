using System;
using System.Threading;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using NumbersBlast.Data;
using NumbersBlast.UI;

namespace NumbersBlast.Multiplayer
{
    /// <summary>
    /// Simulates a fake matchmaking search with animated status text and icon, then returns a random opponent name.
    /// </summary>
    public class OpponentSearchPopup : BasePopup
    {
        [SerializeField] private TextMeshProUGUI _statusText;
        [SerializeField] private TextMeshProUGUI _opponentNameText;
        [SerializeField] private UnityEngine.UI.Button _cancelButton;
        [SerializeField] private RectTransform _searchIcon;

        private MultiplayerConfig _config;
        private Action _onFound;
        private Action _onCancel;
        private string _foundName;
        private CancellationTokenSource _cts;
        private Tween _iconTween;

        private const int SearchTickDelayMs = 400;
        private const int FallbackNameMin = 1000;
        private const int FallbackNameMax = 9999;
        private const int FoundDelayMs = 1500;
        private const float SearchIconSwingDistance = 30f;
        private const float SearchIconSwingDuration = 0.6f;
        private const float SearchIconReturnDuration = 0.4f;
        private const float SearchIconPulseScale = 1.1f;
        private const float SearchIconPulseDuration = 0.3f;

        public string FoundOpponentName => _foundName;

        protected override void Awake()
        {
            base.Awake();
            _cancelButton.onClick.AddListener(OnCancelClick);
        }

        /// <summary>
        /// Begins the fake opponent search sequence, invoking onFound or onCancel when complete.
        /// </summary>
        public void StartSearch(MultiplayerConfig config, Action onFound, Action onCancel)
        {
            _config = config;
            _onFound = onFound;
            _onCancel = onCancel;

            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            _opponentNameText.text = "";
            _statusText.text = "Searching for opponent...";
            Show();
            StartSearchIconAnimation();

            SearchSequenceAsync(_cts.Token).Forget();
        }

        private async UniTaskVoid SearchSequenceAsync(CancellationToken token)
        {
            float searchDuration = UnityEngine.Random.Range(_config.MinSearchDuration, _config.MaxSearchDuration);
            float elapsed = 0f;
            int dotCount = 0;

            while (elapsed < searchDuration)
            {
                if (token.IsCancellationRequested) return;

                dotCount = (dotCount + 1) % 4;
                _statusText.text = $"Searching{new string('.', dotCount)}";

                if (_config.FakeNames != null && _config.FakeNames.Length > 0)
                {
                    _opponentNameText.text = _config.FakeNames[UnityEngine.Random.Range(0, _config.FakeNames.Length)];
                    _opponentNameText.DOKill();
                    _opponentNameText.DOFade(0.3f, 0.1f)
                        .OnComplete(() => _opponentNameText.DOFade(1f, 0.1f).SetLink(_opponentNameText.gameObject))
                        .SetLink(_opponentNameText.gameObject);
                }

                await UniTask.Delay(SearchTickDelayMs, cancellationToken: token);
                elapsed += SearchTickDelayMs / 1000f;
            }

            _foundName = _config.FakeNames != null && _config.FakeNames.Length > 0
                ? _config.FakeNames[UnityEngine.Random.Range(0, _config.FakeNames.Length)]
                : $"Player_{UnityEngine.Random.Range(FallbackNameMin, FallbackNameMax)}";

            _opponentNameText.text = _foundName;
            _statusText.text = "Opponent found!";
            StopSearchIconAnimation();

            _opponentNameText.transform.DOKill();
            _opponentNameText.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 2)
                .SetLink(_opponentNameText.gameObject);

            if (_searchIcon != null)
            {
                _searchIcon.DOKill();
                _searchIcon.DOPunchScale(Vector3.one * 0.3f, 0.4f, 2)
                    .SetLink(_searchIcon.gameObject);
            }

            await UniTask.Delay(FoundDelayMs, cancellationToken: token);

            Hide();
            _onFound?.Invoke();
        }

        private void OnCancelClick()
        {
            _cts?.Cancel();
            StopSearchIconAnimation();
            Hide();
            _onCancel?.Invoke();
        }

        private void StartSearchIconAnimation()
        {
            if (_searchIcon == null) return;

            var seq = DOTween.Sequence();
            seq.Append(_searchIcon.DOAnchorPosX(_searchIcon.anchoredPosition.x + SearchIconSwingDistance, SearchIconSwingDuration).SetEase(Ease.InOutSine));
            seq.Append(_searchIcon.DOAnchorPosX(_searchIcon.anchoredPosition.x - SearchIconSwingDistance, SearchIconSwingDuration).SetEase(Ease.InOutSine));
            seq.Append(_searchIcon.DOAnchorPosX(_searchIcon.anchoredPosition.x, SearchIconReturnDuration).SetEase(Ease.InOutSine));
            seq.Join(_searchIcon.DOScale(Vector3.one * SearchIconPulseScale, SearchIconPulseDuration).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo));
            seq.SetLoops(-1, LoopType.Restart);
            seq.SetLink(_searchIcon.gameObject);
            _iconTween = seq;
        }

        private void StopSearchIconAnimation()
        {
            _iconTween?.Kill();
            if (_searchIcon != null)
                _searchIcon.localScale = Vector3.one;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _cts?.Cancel();
            _cts?.Dispose();
            _iconTween?.Kill();
            _cancelButton.onClick.RemoveListener(OnCancelClick);
        }
    }
}
