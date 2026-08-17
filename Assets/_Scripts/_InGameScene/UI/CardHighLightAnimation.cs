using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CardHighLightAnimation : MonoBehaviour
{
    [SerializeField] private Image _img;
    [SerializeField] private float _duration = 0.6f;
    [SerializeField] private Color _start;
    [SerializeField] private Color _end;
    private Tween _tween;
    private void OnEnable()
    {
        _img.color = _start;
        _tween?.Kill();
        _tween = _img.DOColor(_end, _duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(gameObject);
    }

    private void OnDisable()
    {
        _tween.Kill() ;
    }
}
