using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("-----参照-----")]
    [SerializeField, Tooltip("名前")] private TextMeshProUGUI _name;
    [SerializeField, Tooltip("コスト")] private TextMeshProUGUI _cost;
    [SerializeField, Tooltip("耐久値")] private TextMeshProUGUI _maxTimes;
    [SerializeField, Tooltip("挿絵")] private Image _cardImage;
    [SerializeField, Tooltip("値段")] private TextMeshProUGUI _cardPriceText;

    [Header("-----アニメーション-----")]
    [SerializeField] private float _hoverScale = 1.1f;
    [SerializeField] private float _duration = 0.2f;

    private int _cardID;
    private Vector3 _defaultScale;
    private int _cardPrice;
    private ShopManager _shopManager;

    /// <summary>
    /// カードデータセット
    /// </summary>
    /// <param name="cardID"></param>
    public void SetCardData(CardData cardData, int cardPrice, ShopManager shopManager)
    {
        _defaultScale = transform.localScale;
        _shopManager = shopManager;
        _cardID = cardData.CardID;
        _cardPrice = cardPrice;
        _name.text = cardData.Name;
        _cost.text = $"{cardData.Cost}";
        _maxTimes.text = $"{cardData.MaxTimes}";
        _cardImage.sprite = cardData.CardSprite;
        _cardPriceText.text = $"{cardPrice}";
    }

    /// <summary>
    /// マウスが入った時の処理
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(_defaultScale * _hoverScale, _duration).SetEase(Ease.OutBack);
    }

    /// <summary>
    /// マウスが出たときの処理
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(_defaultScale, _duration).SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// クリックされたときの処理
    /// </summary>
    /// <param name="eventData"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void OnPointerClick(PointerEventData eventData)
    {
        _shopManager.Buy(_cardPrice, _cardID, this.gameObject);
    }
}
