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
    }
}
