using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// ショップの機能をまとめているクラス
/// </summary>
public class ShopManager : MonoBehaviour
{
    [SerializeField] private MapView _mapView;
    [Header("ショップカードリスト")]
    [SerializeField] private List<ShopCardData> _ShopCards = new();

    [Header("-----レアリティ別の金額設定-----")]
    [SerializeField] private int _minComonPrice;
    [SerializeField] private int _maxComonPrice;
    [SerializeField] private int _minRarePrice;
    [SerializeField] private int _maxRarePrice;
    [SerializeField] private int _minEpicPrice;
    [SerializeField] private int _maxEpicPrice;
    [SerializeField] private int _minLegendaryPrice;
    [SerializeField] private int _maxLegendaryPrice;

    [Header("休息設定")]
    [SerializeField] private int _healAmount;
    [SerializeField] private TextMeshProUGUI _restPriceText;
    [SerializeField] private int _restPrice;

    [Header("カード削除設定")]
    [SerializeField] private TextMeshProUGUI _deletCardText;
    [SerializeField] private int _deletPrice;


    private CardDataBase _cardDatabase;
    private WalletManager _walletManager;
    private GameManager _gameManager;

    /// <summary>
    /// ショップの初期化
    /// </summary>
    public void InitShop()
    {
        _gameManager = GameManager.Instance;
        _cardDatabase = _gameManager.CardDataBase;
        _walletManager = WalletManager.Instance;
        gameObject.SetActive(true);
        foreach (ShopCardData card in _ShopCards)
        {
            CardData cardData = _cardDatabase.GetCardData(_cardDatabase.GetRandomCardIDByRarity(card.CardRarity, _gameManager.GetCardTypeByPlayerType(_gameManager.PlayerType)));
            card.ShopCard.SetCardData(cardData, RandomPriceByCardRarity(cardData.Rarity), this);
            card.ShopCard.gameObject.SetActive(true);
        }
    }

    private int RandomPriceByCardRarity(CardRarity cardRarity)
    {
        switch (cardRarity)
        { 
            case CardRarity.Common:    return UnityEngine.Random.Range(_minComonPrice, _maxComonPrice + 1);
            case CardRarity.Rare:      return UnityEngine.Random.Range(_minRarePrice, _maxRarePrice + 1);
            case CardRarity.Epic:      return UnityEngine.Random.Range(_minEpicPrice, _maxEpicPrice + 1);
            case CardRarity.Legendary: return UnityEngine.Random.Range(_minLegendaryPrice, _maxLegendaryPrice + 1);
            default: return 0;
        }
    }

    public void Buy(int price, int cardID, GameObject shopCardGameObject)
    {
        _walletManager = WalletManager.Instance;
        if (_walletManager.TrySpendMoney(price))
        {
            DeckManager.Instance.AddDeck(cardID);
            shopCardGameObject.SetActive(false);
            CriAudioManager.Instance.PlaySe("SE_Buy");
        }
        else
        {
            Debug.Log("お金が足りない");
        }
    }

    public void CloseShopPanel()
    {
        FadeManager.Instance.FadePanel(false, () =>
        {
            gameObject.SetActive(false);
            _mapView.UpdataPlayerPosition();
            FadeManager.Instance.FadePanel(true);
        });
    }
}

[Serializable]
public class ShopCardData
{
    public ShopCard ShopCard;
    public CardRarity CardRarity;
}

