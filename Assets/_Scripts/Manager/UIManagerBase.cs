using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基本UIManagerクラス
/// </summary>
public abstract class UIManagerBase : MonoBehaviour
{
    [Header("-----カード-----")]
    [Tooltip("山札")] public List<int> DeckCard = new List<int>();
    [Tooltip("手札")] public List<GameObject> HandCard = new List<GameObject>();
    [Tooltip("捨て札")] public List<int> DiscardCard = new List<int>();
    [Tooltip("除外札")] public List<int> RemoveCard = new List<int>();

    [Header("-----説明パネル-----")]
    [SerializeField, Header("親オブジェクト")] private GameObject _descriptionPanel;
    private DescriptionPanel _description;
    protected GameManager _gameManager;
    private RectTransform _rt;

    protected virtual void Awake()
    {
        _gameManager = GameManager.Instance;
        _gameManager.RegisterUIManager(this);
        _rt = _descriptionPanel.GetComponent<RectTransform>();
    }
    /// <summary>
    /// 初期化
    /// </summary>
    public abstract void InitUI();
    public abstract void UpdateCostUI();


    public void UpdateDescriptionPanel(bool isCard,RectTransform rect,int id = 0,BuffType buff = BuffType.Combo)
    {
        if (!_descriptionPanel.activeSelf) return;

        if (_description == null)
            _description = _descriptionPanel.GetComponent<DescriptionPanel>();

        _rt.position = rect.position;

        if (isCard)
        {
            CardData cardData = _gameManager.CardDataBase.GetCardData(id);
            _description.UpdateCardWindow(cardData);
            foreach (DescriptionKeyWord key in cardData.KeyWords)
            {
                _description.DisplayKeyWordWindow(key);
            }
        }
        else
        {
            BuffData buffData = _gameManager.BuffDataBase.GetBuffData(buff);
            foreach(DescriptionKeyWord key in buffData.KeyWords)
            {
                _description.DisplayKeyWordWindow(key);
            }
        }
    }

    public void DisplayDescriptionPanel(bool isDisplay)
    {
        if (_description == null)
            _description = _descriptionPanel.GetComponent<DescriptionPanel>();

        _description.DisplayWindow(isDisplay);
    }
}
