using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardMovement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public int ID;
    private GameManager _gameManager;
    private StagePlayer _player;
    private GameObject _dropTarget,_cardPrefab,_newCard;
    private Transform _trOriginalParent,_trHandArea;
    private Canvas _canvas;
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
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
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
        if(_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        _trHandArea = _uiManager.HandArea;
        _cardPrefab = _uiManager.CardPrefab;
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
        TileSlot tileSlot = _trOriginalParent.GetComponent<TileSlot>();
        if(tileSlot != null && tileSlot.IsLastTimeCard)return;
        transform.SetParent(_uiManager.DragLayer.transform);
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.alpha = 0.6f;
        _card = GetComponent<Card>();
        if(_card != null) ID = _card.CardID;
        _cost = _gameManager.CardDataBase.GetCardData(ID).Cost;
        if(_isBoardCard && _trOriginalParent.GetComponent<TileSlot>() != null && 
            _player.CurrentCost < _player.MaxCost)
        {
            _player.ChangeCost(_cost, false);
            _uiManager.UpdateCostText(_player.CurrentCost);
            _refundedCostOnDrag = true;
}
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (_gameManager.CurrentPhase != BattlePhase.Set) return;
        TileSlot tileSlot = _trOriginalParent.GetComponent<TileSlot>();
        if (tileSlot != null && tileSlot.IsLastTimeCard) return;
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (_gameManager.CurrentPhase != BattlePhase.Set) return;
        TileSlot tileSlot = _trOriginalParent.GetComponent<TileSlot>();
        if (tileSlot != null && tileSlot.IsLastTimeCard) return;
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
        _dropTarget = eventData.pointerCurrentRaycast.gameObject;
        if (_dropTarget != null && _dropTarget.GetComponent<TileSlot>() != null)
        {
            _tileSlot = _dropTarget.GetComponent<TileSlot>();
            //カードが存在するとき元に戻す
            if (_tileSlot.IsOccupied || !_player.ConsumeCost(_cost))
            {
                ReturnToOriginalSlot();
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
        }
    }
    private void ReturnToOriginalSlot()
    {
        transform.SetParent(_trOriginalParent);
        _rectTransform.anchoredPosition = Vector2.zero;
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
}
