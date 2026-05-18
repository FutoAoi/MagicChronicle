using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _tileBoardPrefab;
    [SerializeField] private Sprite[] _tileSprites;
    [NonSerialized] public int ID;
    public bool IsLastTimeCard = false;//前のフェーズで置かれたものかどうか
    public bool IsOccupied { get; private set; } = false;//すでに置かれているか
    private GameManager _gameManager;
    private GameObject _newCard;
    private CardMovement _tileMovement;
    private UIManager_Battle _uiManager;
    private DOTween _tween;
    private Image _img;
    private int _index,_currentnumber;
    private bool _isDestroy = false,_isColorChange = false;

    private void Start()
    {
        _img = GetComponent<Image>();
        _index = UnityEngine.Random.Range(0, _tileSprites.Length - 1);
        _img.sprite = _tileSprites[_index];
        IsLastTimeCard = false;
        _gameManager = GameManager.Instance;
        if(_gameManager.CurrentUIManager.TryGetComponent<UIManager_Battle>(out var manager))
        {
            _uiManager = manager;
        }
    }
    /// <summary>
    /// カードを置く
    /// </summary>
    /// <param name="cardSprite"></param>
    public void PlaceCard(int id)
    {
        if(IsOccupied)return;
        ID = id;
        _newCard = Instantiate(_tileBoardPrefab,transform);
        _newCard.GetComponent<Image>().sprite = _gameManager.CardDataBase.GetCardData(id).Sprite;
        _newCard.GetComponentInChildren<TextMeshProUGUI>().text = _gameManager.CardDataBase.GetCardData(id).MaxTimes.ToString();
        _tileMovement = _newCard.GetComponent<CardMovement>();
        _tileMovement.ID = id;
        _tileMovement.SetAsBoardCard();
        _currentnumber = _gameManager.CardDataBase.GetCardData(id).MaxTimes;
        IsOccupied = true;
    }
    /// <summary>
    /// スロットの中を無くす
    /// </summary>
    public void ClearSlot()
    {
        if(_newCard != null)
        {
            Destroy(_newCard);
            _newCard = null;
        }
        IsOccupied = false;
    }
    /// <summary>
    /// スロットの使用回数減少
    /// </summary>
    /// <param name="times">使用回数</param>
    public void DecreaseTimes(int times)
    {
        if (!IsOccupied || _isDestroy) return;

        _currentnumber -= times;
        _newCard.GetComponentInChildren<TextMeshProUGUI>().text = _currentnumber.ToString();
        if (_currentnumber <= 0)
        {
            _isDestroy = true;
            CardData data = _gameManager.CardDataBase.GetCardData(ID);
            if (data.IsGhost)
            {
                foreach(IEffect effect in data.Effect)
                {
                    effect.OnExcute(null);
                }
            }
            if (data.IsDestruction)
            {
                (_gameManager.CurrentUIManager as IBattleUI)?.RegisterRemoveCard(ID);
            }
            else
            {
                (_gameManager.CurrentUIManager as IBattleUI)?.ResisterDiscardCard(ID);
            }
            _isDestroy = false;
            ClearSlot();
        }
    }
    /// <summary>
    /// タイルを発光させる
    /// </summary>
    /// <param name="duration">演出時間</param>
    /// <param name="toGrow">明るくさせるかどうか</param>
    public void TileColorChangeAnimation(float duration,bool toGrow)
    {
        if (_isColorChange) return;

        Color startColor = _img.color;
        Color endColor = toGrow ? _uiManager.GrowColor : Color.white;

        if(startColor == endColor) return;

        Sequence seq = DOTween.Sequence();

        seq.Append(_img.DOColor(endColor, duration).SetEase(Ease.Linear))
            .OnStart(() => _isColorChange = true)
            .OnComplete(() => _isColorChange = false)
            .OnKill(() => _isColorChange = false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _img.DOColor(_uiManager.SelectColor, 0.1f);
        if (!IsOccupied) return;
        _uiManager.DisplayDescriptionPanel(true);
        _uiManager.UpdateDescriptionPanel(ID,false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _uiManager.DisplayDescriptionPanel(false);
        _img.DOColor(Color.white, 0.1f);
    }
}
