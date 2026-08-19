using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NomalCardView : MonoBehaviour
{
    [Header("参照UI")]
    [SerializeField] private Image _cardImage;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _costText;
    [SerializeField] private TMP_Text _maxTimes;

    [Header("矢印")]
    [SerializeField] private Image _up;
    [SerializeField] private Image _right;
    [SerializeField] private Image _left;
    [SerializeField] private Image _down;

    [Header("-----カードの見た目-----")]
    [SerializeField, Tooltip("矢印色")] private Color _arrowColor = Color.yellowGreen;
    [SerializeField, Tooltip("矢印デフォルト色")] private Color _defaultColor = Color.darkGreen;

    public CardData CardData { get; private set; }

    public void Setup(CardData cardData)
    {
        CardData = cardData;

        if (cardData == null)
        {
            // 強化先が無い場合など、空表示にしたいケース用
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        if (_cardImage != null) _cardImage.sprite = cardData.CardSprite;
        if (_nameText != null) _nameText.text = cardData.Name;
        if (_maxTimes != null) _maxTimes.text = cardData.MaxTimes.ToString();
        if (_costText != null) _costText.text = cardData.Cost.ToString();

        _up.color = _defaultColor;
        _down.color = _defaultColor;
        _right.color = _defaultColor;
        _left.color = _defaultColor;
        foreach (MagicVector vector in cardData.DisplayArrowVector)
        {
            GetArrowImage(vector).color = _arrowColor;
        }
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
