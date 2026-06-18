using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardMovement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public int ID;
    public bool IsDragging { get; private set; } = false;

    [Header("-----参照-----")]
    [SerializeField, Header("移動中のカード")] private GameObject _moveCardView;
    [SerializeField, Header("名前")] private TextMeshProUGUI _nameT;
    [SerializeField, Header("コスト")] private TextMeshProUGUI _costT;
    [SerializeField, Header("耐久値")] private TextMeshProUGUI _timeT;
    [SerializeField,Header("イラスト")] private Image _illustImg;
    [SerializeField, Header("置かれる魔法陣")] private GameObject _magicCircleView;
    [SerializeField, Header("魔法陣の画像")] private Image _magicCircleImage;
    [SerializeField, Header("魔法陣のコスト")] private TextMeshProUGUI _magicTimeText;
    [SerializeField, Header("上印")] private Image _upArrowImage;
    [SerializeField, Header("右印")] private Image _rightArrowImage;
    [SerializeField,Header("左印")] private Image _leftArrowImage;
    [SerializeField, Header("下印")] private Image _downArrowImage;

    [Header("-----数値調整-----")]
    [SerializeField, Header("ホールド時のローカル座標")] private Vector2 _offset = new Vector2(0,250);

    private GameManager _gameManager;
    private StagePlayer _player;
    private GameObject _dropTarget,_cardPrefab,_newCard;
    private Transform _trOriginalParent,_trHandArea;
    private Canvas _canvas;
    private CanvasGroup _canvasGroup;
    private RectTransform _rt;
    private TileSlot _tileSlot;
    private Card _card;
    private UIManager_Battle _uiManager;
    private bool _isBoardCard = false,_refundedCostOnDrag = false;
    private int _cost;

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _player = _gameManager.Player;
        _uiManager = FindAnyObjectByType<UIManager_Battle>();
        _rt = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
        if(_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        _trHandArea = _uiManager.HandArea;
        _cardPrefab = _uiManager.CardPrefab;
        UpdateCardObject(false);
    }
    /// <summary>
    /// タイルが置かれた
    /// </summary>
    public void SetAsBoardCard()
    {
        _isBoardCard = true;
        _trOriginalParent = transform.parent;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_gameManager.CurrentPhase != BattlePhase.Set) return;

        _trOriginalParent = transform.parent;

        if (eventData.button != PointerEventData.InputButton.Left) return;

        //前回置かれた魔法陣か確認
        if (IsBeforeOccupied()) return;

        IsDragging = true;
        _uiManager.RegisterCardMovement(true, this);
        transform.SetParent(_uiManager.DragLayer.transform);
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.alpha = 0.6f;

        //手札カードだったらIDを保持
        _card = GetComponent<Card>();
        if(_card != null) ID = _card.CardID;

        _cost = _gameManager.CardDataBase.GetCardData(ID).Cost;

        //盤面カードだったら処理
        if(_isBoardCard && _trOriginalParent.GetComponent<TileSlot>() != null && 
            _player.CurrentCost < _player.MaxCost)
        {
            //コスト変化
            _player.ChangeCost(_cost, false);
            _uiManager.UpdateCostText(_player.CurrentCost);
            _refundedCostOnDrag = true;

            //見た目変化
            UpdateCardObject(true);

        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (_gameManager.CurrentPhase != BattlePhase.Set) return;
        if (eventData.button != PointerEventData.InputButton.Left) return;

        //前回置かれた魔法陣か確認
        if (IsBeforeOccupied()) return;

        //_rt.anchoredPosition += eventData.delta / _canvas.scaleFactor;

        RectTransform canvasRect = _canvas.GetComponent<RectTransform>();

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint))
        {
            _rt.anchoredPosition = localPoint + _offset;
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (_gameManager.CurrentPhase != BattlePhase.Set) return;
        if (eventData.button != PointerEventData.InputButton.Left) return;

        TileSlot tileSlot = _trOriginalParent.GetComponent<TileSlot>();
        if (tileSlot != null && tileSlot.IsLastTimeCard) return;
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
        _dropTarget = eventData.pointerCurrentRaycast.gameObject;
        IsDragging = false;
        _uiManager.RegisterCardMovement(false, this);
        if (_dropTarget != null && _dropTarget.GetComponent<TileSlot>() != null)
        {
            _tileSlot = _dropTarget.GetComponent<TileSlot>();
            //カードが存在するとき元に戻す
            if (_tileSlot.IsOccupied || !_player.ConsumeCost(_cost))
            {
                ReturnToOriginalSlot();
                UpdateCardObject(false);
                return;
            }
            //盤面上から動かされてたらスロットを空に
            if(_isBoardCard && _trOriginalParent.GetComponent<TileSlot>() != null)
            {
                _trOriginalParent.GetComponent<TileSlot>().ClearSlot();
            }
            _tileSlot.PlaceCard(ID);
            _player.ChangeCost(_cost, true);
            _uiManager.UpdateCostText(_player.CurrentCost);
            Card card = GetComponent<Card>();
            if (card != null)
            {
                card.IgnorePointerFor(0.2f);
            }
            _uiManager.HandCard.Remove(gameObject);
            Destroy(gameObject,0.05f);
        }
        else if(_isBoardCard)
        {
            if (tileSlot.IsLastTimeCard) return;
            tileSlot.ClearSlot();
            InstanceHandCard(ID);
            Destroy(gameObject, 0.05f);
        }
        else
        {
            ReturnToOriginalSlot();
            UpdateCardObject(false);
        }
    }
    private void ReturnToOriginalSlot()
    {
        transform.SetParent(_trOriginalParent);
        _rt.anchoredPosition = Vector2.zero;
        if (_refundedCostOnDrag)
        {
            _player.ChangeCost(_cost, true);
            _uiManager.UpdateCostText(_player.CurrentCost);
            _refundedCostOnDrag = false;
        }
    }
    /// <summary>
    /// 右クリックで削除
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_gameManager.CurrentPhase != BattlePhase.Set) return;
        if (eventData.button == PointerEventData.InputButton.Right && _isBoardCard)
        {
            TileSlot tileSlot = _trOriginalParent.GetComponent<TileSlot>();
            //親がスロットなら中身を空に
            if (_trOriginalParent != null && tileSlot != null)
            {
                if(tileSlot.IsLastTimeCard)return;
                tileSlot.ClearSlot();
            }
            _cost = _gameManager.CardDataBase.GetCardData(ID).Cost;
            _player.ChangeCost(_cost, false);
            _uiManager.UpdateCostText(_player.CurrentCost);
            InstanceHandCard(ID);
            Destroy(gameObject,0.05f);
        }
    }
    /// <summary>
    /// 手札にカード生成
    /// </summary>
    /// <param name="id">ID</param>
    private void InstanceHandCard(int id)
    {
        _newCard = Instantiate(_cardPrefab, _trHandArea);
        _newCard.GetComponent<Card>().SetCard(id, true);
        CanvasGroup cg = _newCard.GetComponent<CanvasGroup>();
        if (cg == null) cg = _newCard.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = true;
        cg.alpha = 1f;
        RectTransform rt = _newCard.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;
        _uiManager.HandCard.Add(_newCard);
    }

    private bool IsBeforeOccupied()
    {
        TileSlot tileSlot = _trOriginalParent.GetComponent<TileSlot>();
        return tileSlot != null && tileSlot.IsLastTimeCard;
    }

    private void UpdateCardObject(bool isViewCard)
    {
        if (_magicCircleView != null) _magicCircleView.SetActive(!isViewCard);
        if (_moveCardView != null)
        {
            _moveCardView.SetActive(isViewCard);

            if (!_moveCardView.activeSelf) return;

            CardData data = _gameManager.CardDataBase.GetCardData(ID);

            _nameT.text = data.Name;
            _costT.text = data.Cost.ToString();
            _timeT.text = data.MaxTimes.ToString();
            _illustImg.sprite = data.CardSprite;
        }
    }

    public void SetSlotMagicImage(CardData data)
    {
        _magicCircleImage.sprite = data.MagicSprite;
        _magicTimeText.text = data.MaxTimes.ToString();

        foreach(MagicVector vector in data.DisplayArrowVector)
        {
            GetArrowImage(vector).gameObject.SetActive(true);
        }
    }

    public void MagicDestroyAnimation()
    {
        Destroy(gameObject);
    }

    private Image GetArrowImage(MagicVector vector)
    {
        return vector switch
        {
            MagicVector.UP => _upArrowImage,
            MagicVector.Right => _rightArrowImage,
            MagicVector.Left => _leftArrowImage,
            MagicVector.Down => _downArrowImage,
            _ => null
        };
    }
}
