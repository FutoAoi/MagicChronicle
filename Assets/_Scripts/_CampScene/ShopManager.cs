using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public int RestPrice => _restPrice;
    public int DeletePrice => _deletPrice;

    [SerializeField] private MapView _mapView;
    [Header("ショップアクション")]
    [SerializeField] private ShopActionButton _restButton;
    [SerializeField] private GameObject _rest;
    [SerializeField] private ShopActionButton _deleteCardButton;
    [SerializeField] private GameObject _delete;
    [SerializeField] private HpBarController _hpBarController;
    [SerializeField] private TextDisplayAnimation _chat;

    [Header("所持金")]
    [SerializeField] private TextMeshProUGUI _currentMoney;
    [SerializeField] private GameObject _arrow;
    [SerializeField] private TextMeshProUGUI _afterMoney;
    [SerializeField] private Color _defaultColor = Color.yellow;
    [SerializeField] private Color _dengerColor = Color.red;

    [Header("ショップカードリスト")]
    [SerializeField] private List<ShopCardData> _ShopCards = new();

    [Header("レアリティ別の金額設定")]
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
    [SerializeField] private DeckDeletePanel _deckDeletePanel;

    private CardDataBase _cardDatabase;
    private WalletManager _walletManager;
    private GameManager _gameManager;

    private IShopSelectable _selectedItem;
    private PlayerStatus _status;
    private Tween _moneyTween;
    private int _money;

    private void Start()
    {
        _money = -1;
    }

    public void InitShop()
    {
        _gameManager = GameManager.Instance;
        _cardDatabase = _gameManager.CardDataBase;
        _walletManager = WalletManager.Instance;

        gameObject.SetActive(true);

        _restPriceText.text = _restPrice.ToString();
        _deletCardText.text = _deletPrice.ToString();

        MoneyReset();

        foreach (ShopCardData card in _ShopCards)
        {
            int randomCardID = _cardDatabase.GetRandomCardIDByRarity(
                card.CardRarity,
                _gameManager.GetCardTypeByPlayerType(_gameManager.PlayerType));

            CardData cardData = _cardDatabase.GetCardData(randomCardID);

            card.ShopCard.SetCardData(
                cardData,
                RandomPriceByCardRarity(cardData.Rarity),
                this);

            card.ShopCard.gameObject.SetActive(true);
        }

        _restButton.Initialize(this);
        _deleteCardButton.Initialize(this);
    }

    private int RandomPriceByCardRarity(CardRarity cardRarity)
    {
        return cardRarity switch
        {
            CardRarity.Common =>
                UnityEngine.Random.Range(_minComonPrice, _maxComonPrice + 1),

            CardRarity.Rare =>
                UnityEngine.Random.Range(_minRarePrice, _maxRarePrice + 1),

            CardRarity.Epic =>
                UnityEngine.Random.Range(_minEpicPrice, _maxEpicPrice + 1),

            CardRarity.Legendary =>
                UnityEngine.Random.Range(_minLegendaryPrice, _maxLegendaryPrice + 1),

            _ => 0
        };
    }

    public void Buy(int price, int cardID, GameObject shopCardGameObject)
    {
        if (_walletManager.TrySpendMoney(price))
        {
            DeckManager.Instance.AddDeck(cardID);
            shopCardGameObject.SetActive(false);

            CriAudioManager.Instance.PlaySe("SE_Buy");

            MoneyReset();
        }
        else
        {
            Debug.Log("お金が足りない");
        }
    }

    public void Rest()
    {
        if (_walletManager.TrySpendMoney(_restPrice))
        {
            _restButton.gameObject.SetActive(false);
            _rest.SetActive(false);
            _status = GameManager.Instance.PlayerStatus;

            int amount = (int)(_status.PlayerMaxHp * ((float)_healAmount / 100));
            _status.HealHp(amount);
            _hpBarController.HpBarUpdate(GameManager.Instance.PlayerStatus.PlayerCurrentHp, GameManager.Instance.PlayerStatus.PlayerMaxHp);

            CriAudioManager.Instance.PlaySe("SE_Heal");
            CriAudioManager.Instance.PlaySe("SE_Buy");

            MoneyReset();
        }
        else
        {
            Debug.Log("お金が足りない");
        }
    }

    /// <summary>
    /// ショップの「カード削除」を確定した時に呼ぶ
    /// </summary>
    public void OpenDeleteCardPanel()
    {
        _deckDeletePanel.Open(TryDeleteDeckCard);
    }

    /// <summary>
    /// 料金を払い、選択されたカード1枚だけを削除する
    /// </summary>
    private bool TryDeleteDeckCard(int deckIndex)
    {
        // 無効なインデックスで料金だけ払うことを防ぐ
        if (deckIndex < 0 ||
            deckIndex >= DeckManager.Instance.DeckMain.Count)
        {
            return false;
        }

        if (!_walletManager.TrySpendMoney(_deletPrice))
        {
            Debug.Log("お金が足りない");
            return false;
        }

        bool isDeleted = DeckManager.Instance.RemoveDeckAt(deckIndex);

        if (isDeleted)
        {
            CriAudioManager.Instance.PlaySe("SE_Buy");
            _deleteCardButton.gameObject.SetActive(false);
            _delete.SetActive(false);

            MoneyReset();
        }

        return isDeleted;
    }

    public void CloseShopPanel()
    {
        FadeManager.Instance.FadePanel(false, () =>
        {
            _chat.gameObject.SetActive(false);
            gameObject.SetActive(false);
            _mapView.UpdataPlayerPosition();
            FadeManager.Instance.FadePanel(true);
        });
    }

    /// <summary>
    /// 別項目なら選択、同じ項目なら確定としてtrueを返す
    /// </summary>
    public bool SelectOrConfirm(IShopSelectable item)
    {
        if (_selectedItem == item)
            return true;

        _selectedItem?.Deselect();
        _selectedItem = item;

        return false;
    }

    public void ClearSelectedItem(IShopSelectable item)
    {
        if (_selectedItem == item)
        {
            _selectedItem = null;
        }
    }

    public void Greeding()
    {
        _chat.gameObject.SetActive(true);
    }

    public void MoneyReset()
    {
        if(_moneyTween != null) _moneyTween.Kill();

        if (_money < 0) _money = _walletManager.CurrentMoney;
        int money = _money;
        _moneyTween = DOTween.To(() => money,
            x =>
            {
                money = x;
                _currentMoney.text = $"{money}G";
            },
            _walletManager.CurrentMoney,
            0.2f);

        _currentMoney.color = _defaultColor;
        _afterMoney.color = _defaultColor;
        _money = _walletManager.CurrentMoney;
        _arrow.SetActive(false);
        _afterMoney.gameObject.SetActive(false);
    }

    public void PredictionMoney(int delta)
    {
        _arrow.SetActive(true);
        _afterMoney.gameObject.SetActive(true);
        _afterMoney.text = $"{_walletManager.CurrentMoney - delta}G";
        if (_walletManager.CurrentMoney - delta < 0)
        {
            _afterMoney.color = _dengerColor;
        }
    }
}

[Serializable]
public class ShopCardData
{
    public ShopCard ShopCard;
    public CardRarity CardRarity;
}