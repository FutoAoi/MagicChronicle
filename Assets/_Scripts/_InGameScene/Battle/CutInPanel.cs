using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CutInPanel : MonoBehaviour
{
    [SerializeField, Tooltip("移行先のフェーズ")] private BattlePhase _battlePhase;
    [SerializeField] private TextMeshProUGUI _text;
    private GameManager _gamemanager;
    private Image _panelimg;
    private RectTransform _panelRectTr;
    private Color _defaultColor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _panelimg = GetComponent<Image>();
        _panelRectTr = GetComponent<RectTransform>();
        _defaultColor = _panelimg.color;
        _gamemanager = GameManager.Instance;
    }
    /// <summary>
    /// カットインアニメーション
    /// </summary>
    public void CutInAnimation(float duration)
    {
        _panelRectTr.anchoredPosition = Vector2.right * 1500f;
        _defaultColor = _panelimg.color;
        _panelimg.color = new Color(_defaultColor.r, _defaultColor.g, _defaultColor.b, 0f);
        _panelRectTr.transform.localScale = Vector3.one;
        _text.alpha = 0f;
        _text.transform.localScale = Vector3.one * 0.8f;

        Sequence seq = DOTween.Sequence();
        seq.Append(_panelimg.DOFade(1f, duration * 0.12f)).SetEase(Ease.OutQuad);
        seq.Join(_panelRectTr.DOAnchorPos(Vector2.zero, duration * 0.38f).SetEase(Ease.OutExpo));
        seq.Join(_text.DOFade(1f, duration * 0.25f).SetEase(Ease.OutQuad));
        seq.Join(_text.transform.DOScale(1.25f, duration * 0.25f).SetEase(Ease.OutBack));
        seq.AppendInterval(duration * 0.05f);
        seq.Append(_text.transform.DOScale(1f, duration * 0.1f).SetEase(Ease.OutQuad));
        seq.Append(_panelRectTr.DOAnchorPos(Vector2.left * 2500f, duration * 0.2f).SetEase(Ease.InQuad));
        seq.Join(_panelimg.DOFade(0f, duration * 0.15f));
        seq.Join(_text.DOFade(0f, duration * 0.15f));
        seq.OnComplete(() =>
        {
            _gamemanager.CurrentPhase = _battlePhase;
            gameObject.SetActive(false);
        });
    }
}
