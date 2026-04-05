using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using NumbersBlast.Board;

namespace NumbersBlast.Tutorial
{
    public class TutorialOverlay : MonoBehaviour
    {
        [SerializeField] private RawImage _overlayImage;
        [SerializeField] private RectTransform _handIcon;
        [SerializeField] private TextMeshProUGUI _instructionText;
        [SerializeField] private CanvasGroup _canvasGroup;

        private Material _overlayMaterial;
        private Tween _handTween;
        private Tween _cutoutTween;
        private Vector2 _handOffset;

        private static readonly int CutoutCenterProp = Shader.PropertyToID("_CutoutCenter");
        private static readonly int CutoutSizeProp = Shader.PropertyToID("_CutoutSize");

        public void Initialize(Vector2 handOffset = default)
        {
            _handOffset = handOffset == default ? new Vector2(30f, -30f) : handOffset;
            _overlayMaterial = new Material(_overlayImage.material);
            _overlayImage.material = _overlayMaterial;
            SetCutout(new Vector2(0.5f, 0.5f), Vector2.zero);
            Hide();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = false;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            KillHandTween();
        }

        public void SetInstruction(string text)
        {
            _instructionText.text = text;
        }

        public void HighlightRect(RectTransform target, float padding = 0.02f)
        {
            var uv = WorldToUV(target);
            uv.size += new Vector2(padding, padding);
            AnimateCutout(uv.center, uv.size);
        }

        public void HighlightTwoCells(CellView topLeft, CellView bottomRight, float padding = 0.02f)
        {
            Vector3[] tlCorners = new Vector3[4];
            Vector3[] brCorners = new Vector3[4];
            topLeft.RectTransform.GetWorldCorners(tlCorners);
            bottomRight.RectTransform.GetWorldCorners(brCorners);

            // Find actual min/max from all corners
            Vector3 worldBL = new Vector3(
                Mathf.Min(tlCorners[0].x, brCorners[0].x),
                Mathf.Min(tlCorners[0].y, brCorners[0].y),
                0
            );
            Vector3 worldTR = new Vector3(
                Mathf.Max(tlCorners[2].x, brCorners[2].x),
                Mathf.Max(tlCorners[2].y, brCorners[2].y),
                0
            );

            var uv = CornersToUV(worldBL, worldTR);
            uv.size += new Vector2(padding, padding);
            AnimateCutout(uv.center, uv.size);
        }

        private (Vector2 center, Vector2 size) WorldToUV(RectTransform target)
        {
            Vector3[] corners = new Vector3[4];
            target.GetWorldCorners(corners);
            return CornersToUV(corners[0], corners[2]);
        }

        private (Vector2 center, Vector2 size) CornersToUV(Vector3 worldBL, Vector3 worldTR)
        {
            // Get overlay bounds in world space
            Vector3[] overlayCorners = new Vector3[4];
            _overlayImage.rectTransform.GetWorldCorners(overlayCorners);

            float oMinX = overlayCorners[0].x;
            float oMinY = overlayCorners[0].y;
            float oW = overlayCorners[2].x - oMinX;
            float oH = overlayCorners[2].y - oMinY;

            // Convert to 0-1 UV relative to overlay
            float u0 = (worldBL.x - oMinX) / oW;
            float v0 = (worldBL.y - oMinY) / oH;
            float u1 = (worldTR.x - oMinX) / oW;
            float v1 = (worldTR.y - oMinY) / oH;

            Vector2 center = new Vector2((u0 + u1) * 0.5f, (v0 + v1) * 0.5f);
            Vector2 size = new Vector2(Mathf.Abs(u1 - u0), Mathf.Abs(v1 - v0));

            return (center, size);
        }

        public void ShowHandAtPosition(RectTransform target)
        {
            if (_handIcon == null || target == null) return;

            KillHandTween();
            _handIcon.gameObject.SetActive(true);
            _handIcon.localScale = Vector3.one;
            _handIcon.position = target.position;
            _handIcon.anchoredPosition += _handOffset;

            // Gentle breathing animation
            var seq = DOTween.Sequence();
            seq.Append(_handIcon.DOScale(Vector3.one * 1.1f, 0.5f).SetEase(Ease.InOutSine));
            seq.Append(_handIcon.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutSine));
            seq.SetLoops(-1, LoopType.Restart);
            seq.SetLink(_handIcon.gameObject);
            _handTween = seq;
        }

        public void MoveHandToPosition(RectTransform target, float duration = 0.4f)
        {
            if (_handIcon == null || target == null) return;

            KillHandTween();
            _handIcon.gameObject.SetActive(true);

            var targetPos = (Vector2)target.position + _handOffset;
            _handIcon.DOMove(targetPos, duration)
                .SetEase(Ease.InOutCubic)
                .SetLink(_handIcon.gameObject)
                .OnComplete(() =>
                {
                    // Breathing at destination
                    var seq = DOTween.Sequence();
                    seq.Append(_handIcon.DOScale(Vector3.one * 1.1f, 0.5f).SetEase(Ease.InOutSine));
                    seq.Append(_handIcon.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutSine));
                    seq.SetLoops(-1, LoopType.Restart);
                    seq.SetLink(_handIcon.gameObject);
                    _handTween = seq;
                });
        }

        public void HideHand()
        {
            KillHandTween();
            if (_handIcon != null)
            {
                _handIcon.DOScale(Vector3.zero, 0.2f)
                    .SetEase(Ease.InBack)
                    .SetLink(_handIcon.gameObject)
                    .OnComplete(() => _handIcon.gameObject.SetActive(false));
            }
        }

        private void SetCutout(Vector2 center, Vector2 size)
        {
            _overlayMaterial.SetVector(CutoutCenterProp, center);
            _overlayMaterial.SetVector(CutoutSizeProp, size);
        }

        private void AnimateCutout(Vector2 targetCenter, Vector2 targetSize)
        {
            _cutoutTween?.Kill();

            Vector2 currentCenter = (Vector4)_overlayMaterial.GetVector(CutoutCenterProp);
            Vector2 currentSize = (Vector4)_overlayMaterial.GetVector(CutoutSizeProp);

            float t = 0;
            _cutoutTween = DOTween.To(() => t, x =>
            {
                t = x;
                SetCutout(
                    Vector2.Lerp(currentCenter, targetCenter, t),
                    Vector2.Lerp(currentSize, targetSize, t)
                );
            }, 1f, 0.3f).SetEase(Ease.OutCubic);
        }

        private void KillHandTween()
        {
            _handTween?.Kill();
            _handTween = null;
        }

        private void OnDestroy()
        {
            _cutoutTween?.Kill();
            if (_overlayMaterial != null)
                Destroy(_overlayMaterial);
        }
    }
}
