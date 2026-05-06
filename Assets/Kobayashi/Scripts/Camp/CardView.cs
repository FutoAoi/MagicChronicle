using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    public int ID { get; private set; }

    [Header("-----参照-----")]
    [SerializeField, Tooltip("名前")] private TextMeshProUGUI _name;
    [SerializeField, Tooltip("コスト")] private TextMeshProUGUI _cost;
    [SerializeField, Tooltip("耐久値")] private TextMeshProUGUI _durability;
    [SerializeField, Tooltip("挿絵")] private Image _img;

    public void SetCardData(CardData data)
    {
        ID = data.CardID;
        _name.text = data.Name;
        _cost.text = data.Cost.ToString();
        _durability.text = data.MaxTimes.ToString();
        _img.sprite = data.Sprite;
    }
}
