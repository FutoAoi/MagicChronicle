using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnhanceCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("-----参照-----")]
    [SerializeField, Tooltip("名前")] private TextMeshProUGUI _name;
    [SerializeField, Tooltip("コスト")] private TextMeshProUGUI _cost;
    [SerializeField, Tooltip("耐久値")] private TextMeshProUGUI _maxTimes;
    [SerializeField, Tooltip("挿絵")] private Image _cardImage;
    [SerializeField, Tooltip("矢印色")] private Color _arrowColor = Color.yellowGreen;
    [SerializeField, Tooltip("矢印デフォルト色")] private Color _defaultColor = Color.darkGreen;

    [Header("矢印")]
    [SerializeField] private Image _up;
    [SerializeField] private Image _right;
    [SerializeField] private Image _left;
    [SerializeField] private Image _down;

    [Header("-----アニメーション-----")]
    [SerializeField] private float _hoverScale = 1.1f;
    [SerializeField] private float _duration = 0.2f;

    private int _deckIndex;
    private Vector3 _defaultScale;
    private Action<int> _onClick;
    private Transform _tf;

    /// <summary>
    /// カードデータセット
    /// </summary>
    /// <param name="cardData">表示するカードデータ</param>
    /// <param name="deckIndex">デッキ内でのインデックス(同一ID対策)</param>
    /// <param name="onClick">クリック時に呼ばれるコールバック</param>
    public void SetCardData(CardData cardData, int deckIndex, Action<int> onClick)
    {
        _tf = transform;
        _defaultScale = _tf.localScale;
        _deckIndex = deckIndex;
        _onClick = onClick;

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

    /// <summary>
    /// マウスが入った時の処理
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        _tf.DOKill();
        _tf.DOScale(_defaultScale * _hoverScale, _duration).SetEase(Ease.OutBack);
    }

    /// <summary>
    /// マウスが出たときの処理
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        _tf.DOKill();
        _tf.DOScale(_defaultScale, _duration).SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// クリックされたときの処理
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"clicked: {_deckIndex}, onClick is null: {_onClick == null}");
        _onClick?.Invoke(_deckIndex);
    }

    private void OnDisable()
    {
        _tf.DOKill();
        _tf.localScale = _defaultScale;
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
