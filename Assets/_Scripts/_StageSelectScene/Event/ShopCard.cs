using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopCard : MonoBehaviour, IPointerEnterHandler
{
    [Header("-----参照-----")]
    [SerializeField, Tooltip("名前")] private TextMeshProUGUI _name;
    [SerializeField, Tooltip("コスト")] private TextMeshProUGUI _cost;
    [SerializeField, Tooltip("耐久値")] private TextMeshProUGUI _durability;
    [SerializeField, Tooltip("挿絵")] private Image _img;

    private int _cardID;

    public void SetCardData(int cardID)
    {
        _cardID = cardID;
        CardData cardData = GameManager.Instance.CardDataBase.GetCardData(cardID);
        _name.text = cardData.Name;
        _cost.text = $"{cardData.Cost}";
        _durability.text = $"{cardData}";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

}
