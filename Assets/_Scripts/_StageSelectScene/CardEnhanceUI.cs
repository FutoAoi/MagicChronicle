using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardEnhanceUI : MonoBehaviour
{
    [SerializeField] private MapView _mapView;
    [Header("カード一覧")]
    [SerializeField] private Transform _listParent;
    [SerializeField] private EnhanceCard _listItemPrefab;

    [Header("詳細パネル")]
    [SerializeField] private GameObject _detailPanel;
    [SerializeField] private NomalCardView _currentCardView;
    [SerializeField] private NomalCardView _enhancedCardView;
    [SerializeField] private Button _enhanceButton;
    [SerializeField] private GameObject _obj;

    /// <summary>
    /// 強化が実行されたときに発火。引数は(デッキ内インデックス, 強化後のCardData)
    /// </summary>
    public event Action<int, CardData> OnCardEnhanced;

    private CardDataBase _cardDataBase;
    private DeckManager _deckManager;
    private readonly List<EnhanceCard> _spawnedItems = new();
    private int _selectedDeckIndex = -1;

    private void OnDisable()
    {
        _obj.SetActive(false);
    }
    private void Start()
    {
        _enhanceButton.onClick.AddListener(OnEnhanceButtonClicked);
        _cardDataBase = GameManager.Instance.CardDataBase;
        _deckManager = DeckManager.Instance;
        RefreshList();
    }

    public void RefreshList()
    {
        foreach (var item in _spawnedItems)
        {
            Destroy(item.gameObject);
        }
        _spawnedItems.Clear();

        List<int> deck = _deckManager.DeckMain;
        for (int i = 0; i < deck.Count; i++)
        {
            CardData card = _cardDataBase.GetCardData(deck[i]);
            if (card == null) continue;
            if (!card.CanEvolution) continue;

            EnhanceCard item = Instantiate(_listItemPrefab, _listParent);
            item.SetCardData(card, i, ShowEnhancePanel);
            _spawnedItems.Add(item);
        }
    }

    /// <summary>
    /// 指定したデッキ内インデックスのカードの強化パネルを開く。
    /// 一覧のクリックからも、外部(報酬画面など)からも呼び出せる。
    /// </summary>
    public void ShowEnhancePanel(int deckIndex)
    {
        if (deckIndex < 0 || deckIndex >= _deckManager.DeckMain.Count) return;

        int currentId = _deckManager.DeckMain[deckIndex];
        CardData currentCard = _cardDataBase.GetCardData(currentId);
        if (currentCard == null) return;

        _selectedDeckIndex = deckIndex;
        _detailPanel.SetActive(true);
        _currentCardView.Setup(currentCard);

        bool canEnhance = currentCard.CanEvolution;
        _enhancedCardView.Setup(canEnhance ? _cardDataBase.GetCardData(currentCard.EvolutionID) : null);

        _enhanceButton.interactable = canEnhance;
    }

    public void HideEnhancePanel()
    {
        _detailPanel.SetActive(false);
        _selectedDeckIndex = -1;
    }

    private void OnEnhanceButtonClicked()
    {
        if (_selectedDeckIndex < 0) return;

        int deckIndex = _selectedDeckIndex;
        if (!_deckManager.EnhanceCard(deckIndex)) return;

        RefreshList();
        ShowEnhancePanel(deckIndex); // 強化後の内容でパネルを更新

        CardData enhancedResult = _cardDataBase.GetCardData(_deckManager.DeckMain[deckIndex]);
        OnCardEnhanced?.Invoke(deckIndex, enhancedResult);

        FadeManager.Instance.FadePanel(false, () =>
        {
            gameObject.SetActive(false);
            _mapView.UpdataPlayerPosition();
            FadeManager.Instance.FadePanel(true);
        });
    }

    public void TextAnimation()
    {
        _obj.SetActive(true);
    }
}