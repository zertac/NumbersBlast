using UnityEngine;
using DG.Tweening;

public class FeedbackManager : MonoBehaviour
{
    [Header("Screen Shake")]
    [SerializeField] private float _shakeStrength = 10f;
    [SerializeField] private float _shakeDuration = 0.3f;
    [SerializeField] private int _shakeVibrato = 15;

    [Header("Cell Place")]
    [SerializeField] private float _placePunchScale = 0.2f;
    [SerializeField] private float _placeDuration = 0.25f;

    [Header("Merge")]
    [SerializeField] private float _mergePunchScale = 0.3f;

    [Header("Line Clear")]
    [SerializeField] private float _clearDuration = 0.3f;
    [SerializeField] private float _clearDelay = 0.03f;

    [Header("Hover Anticipation")]
    [SerializeField] private float _hoverBoardScale = 1.015f;
    [SerializeField] private float _hoverGrowDuration = 0.3f;
    [SerializeField] private float _hoverCellShake = 2f;

    private RectTransform _shakeTarget;
    private Vector2 _shakeOriginalPos;
    private bool _isHoveringMerge;
    private Tween _hoverBoardTween;
    private Tween _hoverPulseTween;

    public void Initialize(RectTransform shakeTarget)
    {
        _shakeTarget = shakeTarget;
        _shakeOriginalPos = shakeTarget.anchoredPosition;
    }

    // === HOVER STATE ===

    public void StartMergeHover(CellView[] mergeCells)
    {
        _isHoveringMerge = true;

        // Board pulse (breathe)
        _hoverBoardTween?.Kill();
        _hoverPulseTween?.Kill();
        _hoverBoardTween = _shakeTarget.DOScale(Vector3.one * _hoverBoardScale, _hoverGrowDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                _hoverPulseTween = _shakeTarget.DOScale(Vector3.one * (_hoverBoardScale + 0.005f), 0.4f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            });

        // Merge cells gentle pulse
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
            cell.transform.DOScale(Vector3.one, _placeDuration)
                .SetEase(Ease.OutBack)
                .SetDelay(i * 0.03f);
        }

        ShakeScreen(_shakeStrength * 0.5f, _shakeDuration * 0.5f);
    }

    // === MERGE (called on drop, after hover anticipation) ===

    public void PlayMergeSmash(CellView targetCell, CellView[] absorbedCells)
    {
        _isHoveringMerge = false;

        // Board snaps back (smash!)
        _hoverBoardTween?.Kill();
        _hoverPulseTween?.Kill();
        _shakeTarget.DOScale(Vector3.one, 0.05f).SetEase(Ease.OutQuad);
        ShakeScreen(_shakeStrength * 0.6f, _shakeDuration * 0.5f);

        // Absorbed cells flash + vanish
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

        // Target cell impact after absorb
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
        ShakeScreen(_shakeStrength * 0.8f, _shakeDuration * 0.6f);

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
            float delay = i * _clearDelay;

            var sequence = DOTween.Sequence();
            sequence.SetDelay(delay);
            sequence.Append(cell.transform.DOScale(Vector3.one * 1.2f, _clearDuration * 0.3f).SetEase(Ease.OutQuad));
            sequence.Append(cell.transform.DOScale(Vector3.zero, _clearDuration * 0.7f).SetEase(Ease.InBack));
            sequence.OnComplete(() =>
            {
                cell.transform.localScale = Vector3.one;
                completed++;
                if (completed >= total)
                    onComplete?.Invoke();
            });
        }

        ShakeScreen(_shakeStrength, _shakeDuration);
    }

    // === GAME OVER ===

    public void PlayGameOverEffect(System.Action onComplete)
    {
        ShakeScreen(_shakeStrength * 2f, _shakeDuration * 2f);
        DOVirtual.DelayedCall(_shakeDuration * 2f, () => onComplete?.Invoke());
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
        _shakeTarget.DOShakeAnchorPos(duration, strength, _shakeVibrato, 90f, false, true, ShakeRandomnessMode.Harmonic)
            .OnComplete(() => _shakeTarget.anchoredPosition = _shakeOriginalPos);
    }
}
