using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private TextMeshProUGUI _costBefore;
    [SerializeField] private TextMeshProUGUI _costAfter;
    [SerializeField] private TextMeshProUGUI _durabilityBefore;
    [SerializeField] private TextMeshProUGUI _durabilityAfter;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Color _updateColor = Color.orange;
    [SerializeField] private float _updateScale = 1.2f;

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
        _enhanceButton.onClick.AddListener(() =>
        {
            OnEnhanceButtonClicked();

            //強化時の演出
            if(_enhanceButton.TryGetComponent<RectTransform>(out var rt))
            {
                Vector3 scale = rt.localScale;
                rt.DOScale(scale * 1.05f, 0.1f)
                .OnComplete(() =>
                {
                    rt.DOScale(scale, 0.15f);
                });
            }
        });
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

        if (canEnhance)
        {
            CardData before = _currentCardView.CardData;
            CardData after = _enhancedCardView.CardData;
            _costBefore.text = before.Cost.ToString();
            _costAfter.text = after.Cost.ToString();
            _durabilityBefore.text = before.MaxTimes.ToString();
            _durabilityAfter.text = after.MaxTimes.ToString();
            _descriptionText.text = after.Description.ToString();
            _costAfter.rectTransform.localScale = Vector3.one;
            _durabilityAfter.rectTransform.localScale = Vector3.one;

            if (before.Cost < after.Cost)
            {
                _costAfter.color = _updateColor;
                _costAfter.rectTransform.localScale *= _updateScale;
            }

            if(before.MaxTimes < after.MaxTimes)
            {
                _durabilityAfter.color = _updateColor;
                _durabilityAfter.rectTransform.localScale *= _updateScale;
            }
        }
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
        //ShowEnhancePanel(deckIndex); // 強化後の内容でパネルを更新

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