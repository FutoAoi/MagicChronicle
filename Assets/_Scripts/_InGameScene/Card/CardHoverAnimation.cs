using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
/// <summary>
/// カードにカーソル重なったときの拡大縮小アニメーション
/// </summary>
public class CardHoverAnimation : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{
    [Header("-----参照-----")]
    [SerializeField] private Image _img;
    [SerializeField] private Image _highLightImg;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private RectTransform _rect;
    [SerializeField] private GameObject _arrowParent;

    [Header("-----数値設定-----")]
    [SerializeField, Tooltip("効果時間")] private float _duration = 0.2f;
    [SerializeField, Tooltip("拡大比率")] private float _magnificationRatio = 1.2f;
    [SerializeField, Tooltip("上昇量")] private float _upper = 50f;
    [SerializeField, Tooltip("明るさ")] private float _bloom = 0.3f;

    private UIManagerBase _uiManager;
    private IBattleUI _battleUI;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value) return;

            if (_uiManager == null)
            {
                _uiManager = GameManager.Instance.CurrentUIManager;
                if(_uiManager.TryGetComponent<IBattleUI>(out var battle))
                {
                    _battleUI = battle;
                }
            }

            if (value && _uiManager.CardMovement != null) return;

            _isSelected = value;

            if (_isSelected)
            {
                //他のカードを未選択状態にする
                if (_battleUI != null)
                {
                    _battleUI.ChangeSelectHandCard(this);
                }

                //表示順を手前に変更
                _canvas.overrideSorting = true;
                _canvas.sortingOrder = 100;

                //自分を大きくする
                _scaleTween?.Kill();
                _scaleTween = _rect.DOScale(_defaultScale * _magnificationRatio, _duration)
                    .SetEase(Ease.OutBack);
                _rectTween?.Kill();
                _rectTween = _rect.DOAnchorPos(new Vector2(0, _upper), _duration)
                    .SetEase(Ease.OutQuad);
                _highLightImg.gameObject.SetActive(true);
            }
            else
            {
                _canvas.overrideSorting =false;

                //自分を小さくする
                _scaleTween?.Kill();
                _scaleTween = _rect.DOScale(_defaultScale, _duration)
                    .SetEase(Ease.OutBack);
                _rectTween?.Kill();
                _rectTween = _rect.DOAnchorPos(Vector2.zero, _duration)
                    .SetEase(Ease.OutQuad);
                _highLightImg.gameObject.SetActive(false);
            }

            //矢印表示
            _arrowParent.SetActive(IsSelected);
        }
    }
    private Tweener _scaleTween,_rectTween,_colorTween;
    private Vector3 _defaultScale;
    private bool _isSelected = false;
    private void Awake()
    {
        _defaultScale = _rect.localScale;
        _img.color = new Color(0f, 0f, 0f, 0f);
        _canvas.overrideSorting = false;
        _arrowParent.SetActive(false);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        IsSelected = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        IsSelected= false;
    }

    public void ColorAnimation(bool canSelect)
    {
        float bloom = canSelect ? 0f : _bloom;
        _colorTween?.Kill();
        _colorTween = _img.DOFade(bloom, _duration)
            .SetEase(Ease.OutBack);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            IsSelected = true;
        }
    }
}
