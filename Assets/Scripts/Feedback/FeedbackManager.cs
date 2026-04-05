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

    private const float PlaceShakeMultiplier = 0.5f;
    private const float MergeShakeMultiplier = 0.6f;
    private const float ChainShakeMultiplier = 0.8f;
    private const float ChainShakeDurationMultiplier = 0.6f;
    private const float GameOverMultiplier = 2f;
    private const float ClearShrinkRatio = 0.7f;
    private const float ShakeRandomnessDegrees = 90f;
    private const int PickupVibrato = 1;
    private const float PickupElasticity = 0.5f;
    private const int PunchScaleVibrato = 2;

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
        if (_shakeTarget == null) return;
        _isHoveringMerge = true;

        _hoverBoardTween?.Kill();
        _hoverPulseTween?.Kill();
        _hoverBoardTween = _shakeTarget.DOScale(Vector3.one * _config.HoverBoardScale, _config.HoverGrowDuration)
            .SetEase(Ease.InOutSine)
            .SetLink(_shakeTarget.gameObject)
            .OnComplete(() =>
            {
                _hoverPulseTween = _shakeTarget.DOScale(Vector3.one * (_config.HoverBoardScale + _config.HoverPulseIncrement), _config.HoverPulseDuration)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetLink(_shakeTarget.gameObject);
            });

        for (int i = 0; i < mergeCells.Length; i++)
        {
            var cell = mergeCells[i];
            cell.transform.DOKill();
            cell.transform.DOScale(Vector3.one * _config.HoverCellScale, _config.HoverCellPulseDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetLink(cell.gameObject);
        }
    }

    public void StopMergeHover(CellView[] mergeCells)
    {
        if (!_isHoveringMerge) return;
        _isHoveringMerge = false;

        _hoverBoardTween?.Kill();
        _hoverPulseTween?.Kill();
        _shakeTarget.DOKill();
        _shakeTarget.DOScale(Vector3.one, _config.HoverResetDuration).SetEase(Ease.OutQuad).SetLink(_shakeTarget.gameObject);

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
                .SetDelay(i * _config.PlaceDelayPerCell)
                .SetLink(cell.gameObject);
        }

        ShakeScreen(_config.ShakeStrength * PlaceShakeMultiplier, _config.ShakeDuration * PlaceShakeMultiplier);
        HapticManager.Light();
    }

    // === MERGE ===

    public void PlayMergeSmash(CellView targetCell, CellView[] absorbedCells)
    {
        if (targetCell == null) return;
        _isHoveringMerge = false;

        _hoverBoardTween?.Kill();
        _hoverPulseTween?.Kill();
        _shakeTarget.DOScale(Vector3.one, _config.MergeResetDuration).SetEase(Ease.OutQuad).SetLink(_shakeTarget.gameObject);
        ShakeScreen(_config.ShakeStrength * MergeShakeMultiplier, _config.ShakeDuration * PlaceShakeMultiplier);
        HapticManager.Medium();

        for (int i = 0; i < absorbedCells.Length; i++)
        {
            var absorbed = absorbedCells[i];
            absorbed.transform.DOKill();
            absorbed.transform.localScale = Vector3.one;

            var seq = DOTween.Sequence();
            seq.Append(absorbed.transform.DOScale(Vector3.one * _config.AbsorbedFlashScale, _config.AbsorbedFlashDuration).SetEase(Ease.OutQuad));
            seq.Append(absorbed.transform.DOScale(Vector3.zero, _config.AbsorbedShrinkDuration).SetEase(Ease.InBack));
            seq.OnComplete(() => absorbed.transform.localScale = Vector3.one);
            seq.SetLink(absorbed.gameObject);
        }

        DOVirtual.DelayedCall(_config.ImpactDelay, () =>
        {
            if (targetCell == null) return;
            targetCell.Refresh();

            var impactSeq = DOTween.Sequence();
            impactSeq.Append(targetCell.transform.DOScale(new Vector3(_config.ImpactScale, _config.ImpactScale, 1f), _config.ImpactDuration).SetEase(Ease.OutQuad));
            impactSeq.Append(targetCell.transform.DOScale(new Vector3(_config.SquashScaleX, _config.SquashScaleY, 1f), _config.SquashDuration).SetEase(Ease.InQuad));
            impactSeq.Append(targetCell.transform.DOScale(new Vector3(_config.StretchScaleX, _config.StretchScaleY, 1f), _config.StretchDuration).SetEase(Ease.InOutQuad));
            impactSeq.Append(targetCell.transform.DOScale(Vector3.one, _config.SettleDuration).SetEase(Ease.OutBack));
            impactSeq.SetLink(targetCell.gameObject);
        }).SetLink(targetCell.gameObject);
    }

    public void PlayChainMergeSmash(CellView targetCell)
    {
        if (targetCell == null) return;
        ShakeScreen(_config.ShakeStrength * ChainShakeMultiplier, _config.ShakeDuration * ChainShakeDurationMultiplier);
        HapticManager.Medium();

        targetCell.Refresh();

        var impactSeq = DOTween.Sequence();
        impactSeq.Append(targetCell.transform.DOScale(new Vector3(_config.ChainImpactScale, _config.ChainImpactScale, 1f), _config.ChainImpactDuration).SetEase(Ease.OutQuad));
        impactSeq.Append(targetCell.transform.DOScale(new Vector3(_config.ChainSquashScaleX, _config.ChainSquashScaleY, 1f), _config.ChainSquashDuration).SetEase(Ease.InQuad));
        impactSeq.Append(targetCell.transform.DOScale(new Vector3(_config.ChainStretchScaleX, _config.ChainStretchScaleY, 1f), _config.ChainStretchDuration).SetEase(Ease.InOutQuad));
        impactSeq.Append(targetCell.transform.DOScale(Vector3.one, _config.ChainSettleDuration).SetEase(Ease.OutBack));
        impactSeq.SetLink(targetCell.gameObject);
    }

    // === LINE CLEAR ===

    public void PlayLineClearEffect(CellView[] cells, System.Action onComplete)
    {
        if (cells == null || cells.Length == 0) { onComplete?.Invoke(); return; }
        int completed = 0;
        int total = cells.Length;

        for (int i = 0; i < cells.Length; i++)
        {
            var cell = cells[i];
            float delay = i * _config.ClearDelay;

            var sequence = DOTween.Sequence();
            sequence.SetDelay(delay);
            sequence.Append(cell.transform.DOScale(Vector3.one * _config.ClearGrowScale, _config.ClearDuration * _config.ClearGrowRatio).SetEase(Ease.OutQuad));
            sequence.Append(cell.transform.DOScale(Vector3.zero, _config.ClearDuration * ClearShrinkRatio).SetEase(Ease.InBack));
            sequence.OnComplete(() =>
            {
                cell.transform.localScale = Vector3.one;
                completed++;
                if (completed >= total)
                    onComplete?.Invoke();
            });
            sequence.SetLink(cell.gameObject);
        }

        ShakeScreen(_config.ShakeStrength, _config.ShakeDuration);
        HapticManager.Heavy();
    }

    // === GAME OVER ===

    public void PlayGameOverEffect(System.Action onComplete)
    {
        ShakeScreen(_config.ShakeStrength * GameOverMultiplier, _config.ShakeDuration * GameOverMultiplier);
        HapticManager.Heavy();
        DOVirtual.DelayedCall(_config.ShakeDuration * GameOverMultiplier, () => onComplete?.Invoke());
    }

    // === PIECE PICKUP ===

    public void PlayPiecePickupEffect(Transform pieceTransform)
    {
        if (pieceTransform == null) return;
        pieceTransform.DOPunchScale(Vector3.one * _config.PickupPunchScale, _config.PickupPunchDuration, PickupVibrato, PickupElasticity)
            .SetLink(pieceTransform.gameObject);
    }

    // === SCREEN SHAKE ===

    private void ShakeScreen(float strength, float duration)
    {
        if (_shakeTarget == null) return;

        _shakeTarget.DOKill(true);
        _shakeTarget.anchoredPosition = _shakeOriginalPos;
        _shakeTarget.DOShakeAnchorPos(duration, strength, _config.ShakeVibrato, ShakeRandomnessDegrees, false, true, ShakeRandomnessMode.Harmonic)
            .OnComplete(() => _shakeTarget.anchoredPosition = _shakeOriginalPos)
            .SetLink(_shakeTarget.gameObject);
    }
}
