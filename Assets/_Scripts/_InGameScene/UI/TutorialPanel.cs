using UnityEngine;
using DG.Tweening;
using System;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private float _duration = 0.25f;
    [SerializeField] private float _slideOffsetX = 40f;

    private Vector2 _basePos;
    private Sequence _sequence;

    private void Awake()
    {
        _basePos = _rectTransform.anchoredPosition;
    }

    public void Show(bool fromNext = true)
    {
        gameObject.SetActive(true);
        _sequence?.Kill();

        float startOffsetX = fromNext ? _slideOffsetX : -_slideOffsetX;
        _canvasGroup.alpha = 0f;
        _rectTransform.anchoredPosition = _basePos + new Vector2(startOffsetX, 0f);
        _rectTransform.localScale = Vector3.one * 0.95f;

        _sequence = DOTween.Sequence()
            .Join(_canvasGroup.DOFade(1f, _duration))
            .Join(_rectTransform.DOAnchorPos(_basePos, _duration).SetEase(Ease.OutCubic))
            .Join(_rectTransform.DOScale(1f, _duration).SetEase(Ease.OutBack));
    }

    public void Hide(bool toNext = true,System.Action onFinish = null)
    {
        _sequence?.Kill();

        float endOffsetX = toNext ? -_slideOffsetX : _slideOffsetX;

        _sequence = DOTween.Sequence()
            .Join(_canvasGroup.DOFade(0f, _duration))
            .Join(_rectTransform.DOAnchorPos(_basePos + new Vector2(endOffsetX, 0f), _duration).SetEase(Ease.InCubic))
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                onFinish?.Invoke();
            });
    }
}