using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField] private Image _panelImg;
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
        _panelImg.color = new Color(1f, 1f, 1f, 0f);
        _rectTransform.anchoredPosition = _basePos + new Vector2(startOffsetX, 0f);
        _rectTransform.localScale = Vector3.one * 0.95f;

        _sequence = DOTween.Sequence()
            .Join(_panelImg.DOFade(1f, _duration))
            .Join(_rectTransform.DOAnchorPos(_basePos, _duration).SetEase(Ease.OutCubic))
            .Join(_rectTransform.DOScale(1f, _duration).SetEase(Ease.OutBack));
    }

    public void Hide(bool toNext = true,System.Action onFinish = null)
    {
        _sequence?.Kill();

        float endOffsetX = toNext ? -_slideOffsetX : _slideOffsetX;

        _sequence = DOTween.Sequence()
            .Join(_panelImg.DOFade(0f, _duration * 0.5f))
            .Join(_rectTransform.DOAnchorPos(_basePos + new Vector2(endOffsetX, 0f), _duration).SetEase(Ease.OutCubic))
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                onFinish?.Invoke();
            });
    }
}