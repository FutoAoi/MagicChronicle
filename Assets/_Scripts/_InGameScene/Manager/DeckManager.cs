using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance { get; private set; }

    [Header("メインデッキ")]
    [SerializeField] private DeckData _deckData;

    public List<int> DeckMain => _deckData.Cards;

    private GameManager _gameManager;

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
}
