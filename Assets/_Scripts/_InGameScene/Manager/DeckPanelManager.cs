using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DeckPanelManager : MonoBehaviour
{
    [Header("各エリア")]
    [SerializeField, Tooltip("背景")] private GameObject[] _background;
    [SerializeField, Tooltip("山札")] private RectTransform _deckArea;
    [SerializeField, Tooltip("捨て札")] private RectTransform _discardArea;

    [Header("参照")]
    [SerializeField, Tooltip("生成するカード")] private GameObject _cardPrefab;
    [SerializeField] private DeckTabButton[] _tabButton;

    private DeckManager _deckManager;
    private GameManager _gameManager;
    private UIManagerBase _uiManager;
    private CardType _cardType;
    private RectTransform[] _deckTabs;

    private Dictionary<int, List<GameObject>> _deckDict = new();
    private Dictionary<int, List<GameObject>> _discardDict = new();
    private void Start()
    {
        if (_deckManager == null)
        {
            Init();
        }
    }

    public void Init()
    {
        _gameManager = GameManager.Instance;

        if (_gameManager.CurrentUIManager.TryGetComponent(out UIManagerBase ui))
            _uiManager = ui;

        _deckManager = DeckManager.Instance;

        _deckTabs = new RectTransform[2];
        _deckTabs[0] = _deckArea;
        _deckTabs[1] = _discardArea;

        for (int i = 0; i < _deckManager.DeckMain.Count; i++)
        {
            int id = _deckManager.DeckMain[i];
            InstantiateCard(InGameDeckType.Deck, id);
            InstantiateCard(InGameDeckType.Discard, id);
        }

        ChangeDeckTab(InGameDeckType.Deck);
    }
    private void OnEnable()
    {
        if (_deckManager == null)
        {
            Init();
        }
        UpdateDeckPanel();
    }

    public void UpdateDeckPanel()
    {
        DisableAll(_deckDict);
        DisableAll(_discardDict);

        EnableCards(_uiManager.DeckCard, _deckDict);
        EnableCards(_uiManager.DiscardCard, _discardDict);
    }
    void DisableAll(Dictionary<int, List<GameObject>> dict)
    {
        foreach (List<GameObject> list in dict.Values)
        {
            foreach (GameObject obj in list)
            {
                obj.SetActive(false);
            }
        }
    }
    /// <summary>
    /// 指定のカードデータを表示
    /// </summary>
    /// <param name="ids">表示するカードID</param>
    /// <param name="dict">表示先オブジェクトのDictionary</param>
    void EnableCards(List<int> ids, Dictionary<int, List<GameObject>> dict)
    {
        //指定の同名カードが何枚あるかのDict
        Dictionary<int, int> counter = new();

        foreach (int id in ids)
        {
            if (!counter.ContainsKey(id))
                counter[id] = 0;

            int index = counter[id];

            if (dict.TryGetValue(id, out var list))
            {
                if (index < list.Count)
                {
                    list[index].SetActive(true);
                    counter[id]++;
                }
            }
        }
    }
    /// <summary>
    /// 指定のカードを生成し、
    /// 対応した場所の子オブジェクトに設定し、
    /// 非表示にする
    /// </summary>
    /// <param name="type">種類分け</param>
    /// <param name="id">どのIDのカードか</param>
    public void InstantiateCard(InGameDeckType type,int id)
    {
        Dictionary<int, List<GameObject>> dict = null;
        RectTransform rect = null;
        switch (type)
        {
            case InGameDeckType.Deck:
                dict = _deckDict;
                rect = _deckArea;
                break;

            case InGameDeckType.Discard:
                dict = _discardDict;
                rect = _discardArea;
                break;
            default:
                break;
        }

        if (!dict.ContainsKey(id))
        {
            dict[id] = new List<GameObject>();
        }

        GameObject card = Instantiate(_cardPrefab);
        card.transform.SetParent(rect,false);
        if (card.TryGetComponent(out Card cardData))
        {
            cardData.CardID = id;
            cardData.SetCard(id, false);
        }

        dict[id].Add(card);
        card.SetActive(false);
    }
    /// <summary>
    /// 指定のタブに切り替える
    /// </summary>
    /// <param name="type">切り替え先のタブ</param>
    public void ChangeDeckTab(InGameDeckType type)
    {
        for(byte i = 0; i < _deckTabs.Length; i++)
        {
            bool isChose = (byte)type == i;
            _deckTabs[i].gameObject.SetActive(isChose);
            _background[i].SetActive(isChose);
            _tabButton[i].ChangeColor(isChose);
        }
    }
}
