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
    [SerializeField, Tooltip("耐久値表示")] private TextMeshProUGUI _timeText;
    [SerializeField, Tooltip("説明パネル表示")] private RectTransform _rt;
    [SerializeField, Tooltip("矢印色")] private Color _arrowColor = Color.yellowGreen;
    [SerializeField, Tooltip("矢印デフォルト色")] private Color _defaultColor = Color.darkGreen;

    [Header("矢印(上部)")]
    [SerializeField] private Image _up;
    [SerializeField] private Image _right;
    [SerializeField] private Image _left;
    [SerializeField] private Image _down;

    [Header("矢印(左下)")]
    [SerializeField] private Image _upArrow;
    [SerializeField] private Image _rightArrow;
    [SerializeField] private Image _leftArrow;
    [SerializeField] private Image _downArrow;

    [Header("数値設定")]
    //[SerializeField,Tooltip("表示アニメーション時間")] private float _duration = 0.2f;
    //[SerializeField, Tooltip("非表示アニメーション時間")] private float _hideSpeed = 0.1f;

    public int CardID;

    private UIManagerBase _uiManager;
    private CardDataBase _cardDataBase;
    //private bool _ignorePointer = false,_isGhostCircle;

    private void Start()
    {
        
    }
    public void SetCard(int id,bool isDraw)
    {
        _cardDataBase = GameManager.Instance.CardDataBase;
        CardID = id;
        CardData data = _cardDataBase.GetCardData(CardID);
        _cardImage.sprite = data.CardSprite;
        _nameText.text = data.Name;
        _costText.text = data.Cost.ToString();
        _timeText.text = data.MaxTimes.ToString();
        
        if(_up != null)
        {
            _up.color = _defaultColor;
            _upArrow.color = _defaultColor;
            _down.color = _defaultColor;
            _downArrow.color = _defaultColor;
            _left.color = _defaultColor;
            _leftArrow.color = _defaultColor;
            _rightArrow.color = _defaultColor;
            _right.color = _defaultColor;
            foreach(MagicVector vector in data.DisplayArrowVector)
            {
                GetArrowImage(vector).color = _arrowColor;
                GetDownArrowImage(vector).color = _arrowColor;
            }
        }

        _uiManager = GameManager.Instance.CurrentUIManager;

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
        _uiManager.DisplayDescriptionPanel(true);
        _uiManager.UpdateDescriptionPanel(true,_rt,CardID);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData is UnityEngine.InputSystem.UI.ExtendedPointerEventData extended
        && extended.pointerType == UnityEngine.InputSystem.UI.UIPointerType.Touch)
        {
            return;
        }
        _uiManager.DisplayDescriptionPanel(false);
    }

    /// <summary>
    /// カーソルが一定時間重なってるかフラグ
    /// </summary>
    /// <param name="time"></param>
    public void IgnorePointerFor(float time)
    {
        //_ignorePointer = true;
        Invoke(nameof(EnablePointer), time);
    }

    private void EnablePointer()
    {
        //_ignorePointer = false;
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
    private Image GetDownArrowImage(MagicVector vector)
    {
        return vector switch
        {
            MagicVector.UP => _upArrow,
            MagicVector.Right => _rightArrow,
            MagicVector.Left => _leftArrow,
            MagicVector.Down => _downArrow,
            _ => null
        };
    }
}
