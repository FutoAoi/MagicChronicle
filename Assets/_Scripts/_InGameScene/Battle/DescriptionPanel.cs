using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionPanel : MonoBehaviour
{
    [Header("-----参照-----")]
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _cost;
    [SerializeField] private TextMeshProUGUI _durability;
    [SerializeField] private TextMeshProUGUI _description;

    [Header("-----アニメーション設定-----")]
    [SerializeField] private float _duration = 0.1f;

    private bool _isTween;
    private Image _img;

    private void Start()
    {
        _img = GetComponent<Image>();
    }
    /// <summary>
    /// 説明パネルの魔法陣情報を更新
    /// </summary>
    /// <param name="data">更新したいデータ</param>
    /// <param name="isClear">情報を消す</param>
    public void UpdateText(CardData data,bool isClear)
    {
        if (isClear)
        {
            _name.text = null;
            _cost.text = null;
            _durability.text = null;
            _description.text = null;
        }
        else
        {
            _name.text = data.Name;
            _cost.text = data.Cost.ToString();
            _durability.text = data.MaxTimes.ToString();
            _description.text = data.Description;
        }   
    }
}
