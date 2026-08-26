using System.Xml.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeckDeleteCard : MonoBehaviour, IPointerClickHandler
{
    [Header("参照")]
    [SerializeField, Tooltip("名前")] private TextMeshProUGUI _name;
    [SerializeField, Tooltip("コスト")] private TextMeshProUGUI _cost;
    [SerializeField, Tooltip("耐久値")] private TextMeshProUGUI _maxTimes;
    [SerializeField, Tooltip("挿絵")] private Image _cardImage;

    [SerializeField, Tooltip("矢印色")] private Color _arrowColor = Color.yellowGreen;
    [SerializeField, Tooltip("矢印デフォルト色")] private Color _defaultColor = Color.darkGreen;

    [Header("アニメーション")]
    [SerializeField] private float _selectedScale = 1.1f;
    [SerializeField] private float _duration = 0.2f;

    [Header("矢印")]
    [SerializeField] private Image _up;
    [SerializeField] private Image _right;
    [SerializeField] private Image _left;
    [SerializeField] private Image _down;

    public int DeckIndex { get; private set; }

    private DeckDeletePanel _panel;
    private Vector3 _defaultScale;

    public void SetCard(int cardID, int deckIndex, DeckDeletePanel panel)
    {
        _panel = panel;
        CardData cardData =
            GameManager.Instance.CardDataBase.GetCardData(cardID);

        DeckIndex = deckIndex;
        _defaultScale = transform.localScale;

        _name.text = cardData.Name;
        _cost.text = $"{cardData.Cost}";
        _maxTimes.text = $"{cardData.MaxTimes}";
        _cardImage.sprite = cardData.CardSprite;
        _up.color = _defaultColor;
        _down.color = _defaultColor;
        _right.color = _defaultColor;
        _left.color = _defaultColor;
        foreach (MagicVector vector in cardData.DisplayArrowVector)
        {
            GetArrowImage(vector).color = _arrowColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _panel.SelectOrDelete(this);
    }

    public void Select()
    {
        transform.DOKill();
        transform.DOScale(_defaultScale * _selectedScale, _duration)
            .SetEase(Ease.OutBack);
    }

    public void Deselect()
    {
        transform.DOKill();
        transform.DOScale(_defaultScale, _duration)
            .SetEase(Ease.OutQuad);
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
}