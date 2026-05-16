using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardView : MonoBehaviour, IPointerEnterHandler
{
    public int ID => _id;

    [Header("-----参照-----")]
    [SerializeField, Tooltip("名前")] private TextMeshProUGUI _name;
    [SerializeField, Tooltip("コスト")] private TextMeshProUGUI _cost;
    [SerializeField, Tooltip("耐久値")] private TextMeshProUGUI _durability;
    [SerializeField, Tooltip("挿絵")] private Image _img;

    [Header("-----設定-----")]
    [SerializeField, Tooltip("リワードカード？")] private bool _isRewardCard = false;

    [SerializeField] private int _id;
    private CardEncyclopedia _encyclopedia;
    public void SetCardData(CardData data)
    {
        _id = data.CardID;
        _name.text = data.Name;
        _cost.text = data.Cost.ToString();
        _durability.text = data.MaxTimes.ToString();
        _img.sprite = data.Sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_isRewardCard)
        {

            return;
        }

        if (_encyclopedia == null)
            _encyclopedia = GetComponentInParent<CardEncyclopedia>();

        _encyclopedia.UpdateBigCard(ID);
    }
}
