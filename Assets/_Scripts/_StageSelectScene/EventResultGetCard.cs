using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventResultGetCard : EventResultPanelBase
{
    [Header("-----カード-----")]
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _cost;
    [SerializeField] private TextMeshProUGUI _durability;
    [SerializeField, Tooltip("挿絵")] private Image _cardImage;
    [SerializeField, Tooltip("矢印色")] private Color _arrowColor = Color.yellowGreen;
    [SerializeField, Tooltip("矢印デフォルト色")] private Color _defaultColor = Color.darkGreen;
    [Header("矢印")]
    [SerializeField] private Image _up;
    [SerializeField] private Image _right;
    [SerializeField] private Image _left;
    [SerializeField] private Image _down;

    [SerializeField] private GameObject _text;
    public override void ResultAnimation(EventResult result)
    {
        //カードへデータ挿入
        CardData card = _gameManager.CardDataBase.GetCardData(result.ID);
        _name.text = card.Name;
        _cost.text = card.Cost.ToString();
        _durability.text = card.MaxTimes.ToString();
        _cardImage.sprite = card.CardSprite;
        _up.color = _defaultColor;
        _down.color = _defaultColor;
        _right.color = _defaultColor;
        _left.color = _defaultColor;
        foreach (MagicVector vector in card.DisplayArrowVector)
        {
            GetArrowImage(vector).color = _arrowColor;
        }

        _text.SetActive(true);
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
