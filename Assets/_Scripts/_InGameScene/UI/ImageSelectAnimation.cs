using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 画像にカーソル合わせた時に拡大、外して縮小のアニメーション
/// </summary>
[RequireComponent(typeof(Image))]
public class ImageSelectAnimation : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    [Header("アニメーション設定")]
    [SerializeField, Tooltip("演出時間")] private float _duration = 0.3f;
    [SerializeField, Tooltip("拡大比率")] private float _scaleMag = 1.2f;

    private RectTransform _rt;
    private Vector3 _startScale;
    private Tween _tween;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rt = GetComponent<RectTransform>();
        _startScale = _rt.localScale;
    }
    private void OnDisable()
    {
        _tween?.Kill();
        _rt.localScale = _startScale;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        ChangeScaleAnimation(_scaleMag);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ChangeScaleAnimation(1f);
    }

    private void ChangeScaleAnimation(float goal)
    {
        _tween?.Kill();
        _tween = _rt.DOScale(_startScale * goal, _duration)
                    .SetEase(Ease.OutQuad);
    }
}
