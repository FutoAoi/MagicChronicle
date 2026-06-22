using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionPanel : MonoBehaviour
{
    [Header("-----参照-----")]
    [SerializeField] private GameObject _parent;
    [Header("カード")]
    [SerializeField] private GameObject _cardWindow;
    [SerializeField] private TextMeshProUGUI _cardName;
    [SerializeField] private TextMeshProUGUI _cardCost;
    [SerializeField] private TextMeshProUGUI _cardDurability;
    [SerializeField] private TextMeshProUGUI _cardDescription;

    [Header("バフ")]
    [SerializeField] private GameObject _buffPrefab;

    [Header("キーワード")]
    [SerializeField] private GameObject _keywordPrefab;

    private GameManager _gameManager;
    private Dictionary<DescriptionKeyWord, DescriptionWindow> _keywordWindowDic = new();
    private void Start()
    {
        _gameManager = GameManager.Instance;

        foreach(KeywordData data in _gameManager.KeywordDataBase.Keywords)
        {
            if(Enum.TryParse(data.Type.ToString(),out BuffType buff))
            {
                GameObject window = Instantiate(_buffPrefab);
                window.transform.SetParent(_parent.transform, false);
                DescriptionWindow description = window.GetComponent<DescriptionWindow>();
                _keywordWindowDic.Add(data.Type, description);
                description.SetBuffWindow(_gameManager.BuffDataBase.GetBuffData(buff),data);
                description.gameObject.SetActive(false);
            }
            else
            {
                GameObject window = Instantiate(_keywordPrefab);
                window.transform.SetParent(_parent.transform, false);
                DescriptionWindow description = window.GetComponent<DescriptionWindow>();
                _keywordWindowDic.Add(data.Type, description);
                description.SetKeyWordWindow(data);
                description.gameObject.SetActive(false);
            }
        }
    }

    
    /// <summary>
    /// 説明パネルの魔法陣情報を更新
    /// </summary>
    /// <param name="data">更新したいデータ</param>
    /// <param name="isClear">情報を消す</param>
    public void UpdateCardWindow(CardData data)
    {
        _cardWindow.SetActive(true);
        _cardName.text = data.Name;
        _cardCost.text = data.Cost.ToString();
        _cardDurability.text = data.MaxTimes.ToString();
        _cardDescription.text = data.ColorDescription;
    }


    public void DisplayKeyWordWindow(DescriptionKeyWord type)
    {
        if(_keywordWindowDic.TryGetValue(type, out var window))
        {
            window.gameObject.SetActive(true);
        }
    }

    public void DisplayWindow(bool isDisplay)
    {
        ClearWindow();
        _parent.SetActive(isDisplay);
    }

    public void ClearWindow()
    {
        _cardWindow.SetActive(false);
        foreach (var window in _keywordWindowDic.Values)
        {
            window.gameObject.SetActive(false);
        }
    }
}
