using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance { get; private set; }

    [Header("メインデッキ")]
    [SerializeField] private DeckData _deckData;

    public List<int> DeckMain => _deckData.Cards;

    private GameManager _gameManager;
    private UIManager_Battle _UIManager_Battle;
    private int _randomIndex;
    private int _temp;
    private List<int> _deck;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _gameManager = GameManager.Instance;
    }
    /// <summary>
    /// 報酬などでメインのデッキに入れるために使用予定
    /// </summary>
    /// <param name="id"></param>
    public void AddDeck(int id)
    {
        _deckData.Cards.Add(id);
    }

    /// <summary>
    /// デッキのシャッフルメソット
    /// </summary>
    public void ShuffleDeck()
    {
        if (_UIManager_Battle == null) _UIManager_Battle = FindAnyObjectByType<UIManager_Battle>();
        ReconstructionDeck(_UIManager_Battle.RemoveCard);
        for(int i = 0; i < _deck.Count; i++)
        {
            _randomIndex = Random.Range(i, _deck.Count);

            _temp = _deck[i];
            _deck[i] = _deck[_randomIndex];
            _deck[_randomIndex] = _temp;
        }
        Debug.Log("デッキをシャッフルしました");
    }

    /// <summary>
    /// デッキドローメッソト
    /// </summary>
    /// <returns></returns>
    public int DrawCard()
    {
        if (_deck.Count == 0)
        {
            if(_gameManager == null)_gameManager = GameManager.Instance;
            (_gameManager.CurrentUIManager as IBattleUI)?.ResetDeck();
            ShuffleDeck();
        }

        int _topCard = _deck[0];
        _deck.RemoveAt(0);
        return _topCard;
    }

    private void ReconstructionDeck(List<int> deleteID)
    {
        _deck = new List<int>(_deckData.Cards);
        if (deleteID.Count == 0) return;

        foreach (int id in deleteID)
        {
            _deck.Remove(id);
        }
    }
}
