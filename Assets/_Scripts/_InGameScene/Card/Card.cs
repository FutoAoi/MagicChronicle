using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    [Header("カードの見た目")]
    [SerializeField, Tooltip("見た目の親")] private RectTransform _view;
    [SerializeField, Tooltip("挿絵")] private Image _cardImage;
    [SerializeField, Tooltip("カード名")] private TextMeshProUGUI _nameText;
    [SerializeField, Tooltip("コスト表示")]private TextMeshProUGUI _costText;

    [Header("数値設定")]
    [SerializeField,Tooltip("表示アニメーション時間")] private float _duration = 0.2f;
    [SerializeField, Tooltip("非表示アニメーション時間")] private float _hideSpeed = 0.1f;

    public int CardID;

    private IBattleUI _battleUI;
    private CardDataBase _cardDataBase;
    private int _cardCost;
    private bool _ignorePointer = false,_isGhostCircle;

    private void Start()
    {
        
    }
    public void SetCard(int id,bool isDraw)
    {
        _cardDataBase = GameManager.Instance.CardDataBase;
        CardID = id;
        CardData data = _cardDataBase.GetCardData(CardID);
        _cardImage.sprite = data.Sprite;
        _nameText.text = data.Name;
        _cardCost = data.Cost;
        _costText.text = _cardCost.ToString();
        _isGhostCircle = data.IsGhost;

        if(GameManager.Instance.CurrentUIManager.TryGetComponent<IBattleUI>(out var battleUI))
        {
            _battleUI = battleUI;
        }

        if (isDraw)
        {
            #region ドローアニメーション
            _view.localScale = Vector3.zero;
            _view.anchoredPosition = new Vector2(0, -200);
            _view.DOScale(1f, 0.25f).SetEase(Ease.OutBack);
            _view.DOAnchorPos(Vector2.zero, 0.25f).SetEase(Ease.OutCubic);
            #endregion
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _battleUI.DisplayDescriptionPanel(true);
        _battleUI.UpdateDescriptionPanel(CardID,false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _battleUI.DisplayDescriptionPanel(false);
    }

    /// <summary>
    /// カーソルが一定時間重なってるかフラグ
    /// </summary>
    /// <param name="time"></param>
    public void IgnorePointerFor(float time)
    {
        _ignorePointer = true;
        Invoke(nameof(EnablePointer), time);
    }

    private void EnablePointer()
    {
        _ignorePointer = false;
    }
}
