using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IShopSelectable 
{
    public bool IsSelect
    {
        get => _isSelect;
        set
        {
            if (_isSelect == value) return;

            _isSelect = value;

            if(_uiManager == null) _uiManager = GameManager.Instance.CurrentUIManager;
            _uiManager.DisplayDescriptionPanel(_isSelect);
            _uiManager.UpdateDescriptionPanel(true,_rt,_cardID);

            _highLight.SetActive(_isSelect);
        }
    }

    [Header("-----参照-----")]
    [SerializeField, Tooltip("名前")] private TextMeshProUGUI _name;
    [SerializeField, Tooltip("コスト")] private TextMeshProUGUI _cost;
    [SerializeField, Tooltip("耐久値")] private TextMeshProUGUI _maxTimes;
    [SerializeField, Tooltip("挿絵")] private Image _cardImage;
    [SerializeField, Tooltip("値段")] private TextMeshProUGUI _cardPriceText;
    [SerializeField] private Transform _parent;
    [SerializeField] private RectTransform _rt;
    [SerializeField] private GameObject _highLight;

    [Header("矢印")]
    [SerializeField] private Image _up;
    [SerializeField] private Image _right;
    [SerializeField] private Image _left;
    [SerializeField] private Image _down;

    [Header("-----アニメーション-----")]
    [SerializeField] private float _hoverScale = 1.1f;
    [SerializeField] private float _duration = 0.2f;

    [Header("-----カードの見た目-----")]
    [SerializeField, Tooltip("矢印色")] private Color _arrowColor = Color.yellowGreen;
    [SerializeField, Tooltip("矢印デフォルト色")] private Color _defaultColor = Color.darkGreen;

    private int _cardID;
    private Vector3 _defaultScale;
    private int _cardPrice;
    private ShopManager _shopManager;
    private UIManagerBase _uiManager;
    private static ShopCard _selectedCard;
    private bool _isSelect = false;


    /// <summary>
    /// カードデータセット
    /// </summary>
    /// <param name="cardID"></param>
    public void SetCardData(CardData cardData, int cardPrice, ShopManager shopManager)
    {
        _defaultScale = _parent.localScale;
        _shopManager = shopManager;
        _cardID = cardData.CardID;
        _cardPrice = cardPrice;
        _name.text = cardData.Name;
        _cost.text = $"{cardData.Cost}";
        _maxTimes.text = $"{cardData.MaxTimes}";
        _cardImage.sprite = cardData.CardSprite;
        _cardPriceText.text = $"{cardPrice}";
        _highLight.SetActive(false);

        _up.color = _defaultColor;
        _down.color = _defaultColor;
        _right.color = _defaultColor;
        _left.color = _defaultColor;
        foreach(MagicVector vector in cardData.DisplayArrowVector)
        {
            GetArrowImage(vector).color = _arrowColor;
        }
    }

    /// <summary>
    /// マウスが入った時の処理
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        _parent.DOKill();
        _parent.DOScale(_defaultScale * _hoverScale, _duration).SetEase(Ease.OutBack);

        if (_uiManager == null) _uiManager = GameManager.Instance.CurrentUIManager;
        _uiManager.DisplayDescriptionPanel(true);
        _uiManager.UpdateDescriptionPanel(true, _rt, _cardID);
    }

    /// <summary>
    /// マウスが出たときの処理
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isSelect) return;

        _parent.DOKill();
        _parent.DOScale(_defaultScale, _duration).SetEase(Ease.OutQuad);

        _uiManager.DisplayDescriptionPanel(false);
    }

    /// <summary>
    /// クリックされたときの処理
    /// </summary>
    /// <param name="eventData"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void OnPointerClick(PointerEventData eventData)
    {
        bool isConfirmed = _shopManager.SelectOrConfirm(this);

        if (!isConfirmed)
        {
            IsSelect = true;

            _parent.DOKill();
            _parent.DOScale(_defaultScale * _hoverScale, _duration)
                .SetEase(Ease.OutBack);
            return;
        }

        _shopManager.Buy(_cardPrice, _cardID, gameObject);
        _uiManager.DisplayDescriptionPanel(false);
    }

    private Image GetArrowImage(MagicVector vector)
    {
        return vector switch
        {
            MagicVector.UP => _up,
            MagicVector.Right => _right,
            MagicVector.Left => _left,
            MagicVector.Down => _down,
            _ => null
        };
    }

    public void Deselect()
    {
        IsSelect = false;

        _parent.DOKill();
        _parent.DOScale(_defaultScale, _duration).SetEase(Ease.OutQuad);
    }

    private void OnDisable()
    {
        if (_selectedCard == this)
            _selectedCard = null;
    }
}
