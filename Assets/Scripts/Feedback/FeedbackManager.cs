using UnityEngine;
using DG.Tweening;

public class FeedbackManager
{
    private readonly FeedbackConfig _config;

    private RectTransform _shakeTarget;
    private Vector2 _shakeOriginalPos;
    private bool _isHoveringMerge;
    private Tween _hoverBoardTween;
    private Tween _hoverPulseTween;

    public FeedbackManager(FeedbackConfig config)
    {
        _config = config;
    }

    public void Initialize(RectTransform shakeTarget)
    {
        _shakeTarget = shakeTarget;
        _shakeOriginalPos = shakeTarget.anchoredPosition;
    }

    // === HOVER STATE ===

    public void StartMergeHover(CellView[] mergeCells)
    {
        _isHoveringMerge = true;

        _hoverBoardTween?.Kill();
        _hoverPulseTween?.Kill();
        _hoverBoardTween = _shakeTarget.DOScale(Vector3.one * _config.HoverBoardScale, _config.HoverGrowDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                _hoverPulseTween = _shakeTarget.DOScale(Vector3.one * (_config.HoverBoardScale + 0.005f), 0.4f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            });

        for (int i = 0; i < mergeCells.Length; i++)
        {
            var cell = mergeCells[i];
            cell.transform.DOKill();
            cell.transform.DOScale(Vector3.one * 1.08f, 0.3f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }

    public void StopMergeHover(CellView[] mergeCells)
    {
        if (!_isHoveringMerge) return;
        _isHoveringMerge = false;

        _hoverBoardTween?.Kill();
        _hoverPulseTween?.Kill();
        _shakeTarget.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutQuad);

        if (mergeCells != null)
        {
            for (int i = 0; i < mergeCells.Length; i++)
            {
                mergeCells[i].transform.DOKill();
                mergeCells[i].transform.localScale = Vector3.one;
            }
        }
    }

    // === PLACEMENT ===

    public void PlayPlaceEffect(CellView[] cells)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            var cell = cells[i];
            cell.transform.localScale = Vector3.zero;
            cell.transform.DOScale(Vector3.one, _config.PlaceDuration)
                .SetEase(Ease.OutBack)
                .SetDelay(i * 0.03f);
        }

        ShakeScreen(_config.ShakeStrength * 0.5f, _config.ShakeDuration * 0.5f);
        HapticManager.Light();
    }

    // === MERGE ===

    public void PlayMergeSmash(CellView targetCell, CellView[] absorbedCells)
    {
        _isHoveringMerge = false;

        _hoverBoardTween?.Kill();
        _hoverPulseTween?.Kill();
        _shakeTarget.DOScale(Vector3.one, 0.05f).SetEase(Ease.OutQuad);
        ShakeScreen(_config.ShakeStrength * 0.6f, _config.ShakeDuration * 0.5f);
        HapticManager.Medium();

        for (int i = 0; i < absorbedCells.Length; i++)
        {
            var absorbed = absorbedCells[i];
            absorbed.transform.DOKill();
            absorbed.transform.localScale = Vector3.one;

            var seq = DOTween.Sequence();
            seq.Append(absorbed.transform.DOScale(Vector3.one * 1.3f, 0.06f).SetEase(Ease.OutQuad));
            seq.Append(absorbed.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack));
            seq.OnComplete(() => absorbed.transform.localScale = Vector3.one);
        }

        DOVirtual.DelayedCall(0.18f, () =>
        {
            targetCell.Refresh();

            var impactSeq = DOTween.Sequence();
            impactSeq.Append(targetCell.transform.DOScale(new Vector3(1.5f, 1.5f, 1f), 0.08f).SetEase(Ease.OutQuad));
            impactSeq.Append(targetCell.transform.DOScale(new Vector3(0.85f, 1.15f, 1f), 0.06f).SetEase(Ease.InQuad));
            impactSeq.Append(targetCell.transform.DOScale(new Vector3(1.1f, 0.9f, 1f), 0.05f).SetEase(Ease.InOutQuad));
            impactSeq.Append(targetCell.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack));
        });
    }

    public void PlayChainMergeSmash(CellView targetCell)
    {
        ShakeScreen(_config.ShakeStrength * 0.8f, _config.ShakeDuration * 0.6f);
        HapticManager.Medium();

        targetCell.Refresh();

        var impactSeq = DOTween.Sequence();
        impactSeq.Append(targetCell.transform.DOScale(new Vector3(1.8f, 1.8f, 1f), 0.1f).SetEase(Ease.OutQuad));
        impactSeq.Append(targetCell.transform.DOScale(new Vector3(0.7f, 1.3f, 1f), 0.07f).SetEase(Ease.InQuad));
        impactSeq.Append(targetCell.transform.DOScale(new Vector3(1.15f, 0.85f, 1f), 0.05f).SetEase(Ease.InOutQuad));
        impactSeq.Append(targetCell.transform.DOScale(Vector3.one, 0.12f).SetEase(Ease.OutBack));
    }

    // === LINE CLEAR ===

    public void PlayLineClearEffect(CellView[] cells, System.Action onComplete)
    {
        int completed = 0;
        int total = cells.Length;

        for (int i = 0; i < cells.Length; i++)
        {
            var cell = cells[i];
            float delay = i * _config.ClearDelay;

            var sequence = DOTween.Sequence();
            sequence.SetDelay(delay);
            sequence.Append(cell.transform.DOScale(Vector3.one * 1.2f, _config.ClearDuration * 0.3f).SetEase(Ease.OutQuad));
            sequence.Append(cell.transform.DOScale(Vector3.zero, _config.ClearDuration * 0.7f).SetEase(Ease.InBack));
            sequence.OnComplete(() =>
            {
                cell.transform.localScale = Vector3.one;
                completed++;
                if (completed >= total)
                    onComplete?.Invoke();
            });
        }

        ShakeScreen(_config.ShakeStrength, _config.ShakeDuration);
        HapticManager.Heavy();
    }

    // === GAME OVER ===

    public void PlayGameOverEffect(System.Action onComplete)
    {
        ShakeScreen(_config.ShakeStrength * 2f, _config.ShakeDuration * 2f);
        HapticManager.Heavy();
        DOVirtual.DelayedCall(_config.ShakeDuration * 2f, () => onComplete?.Invoke());
    }

    // === PIECE PICKUP ===

    public void PlayPiecePickupEffect(Transform pieceTransform)
    {
        pieceTransform.DOPunchScale(Vector3.one * 0.1f, 0.15f, 1, 0.5f);
    }

    // === SCREEN SHAKE ===

    private void ShakeScreen(float strength, float duration)
    {
        if (_shakeTarget == null) return;

        _shakeTarget.DOKill(true);
        _shakeTarget.anchoredPosition = _shakeOriginalPos;
        _shakeTarget.DOShakeAnchorPos(duration, strength, _config.ShakeVibrato, 90f, false, true, ShakeRandomnessMode.Harmonic)
            .OnComplete(() => _shakeTarget.anchoredPosition = _shakeOriginalPos);
    }
}
