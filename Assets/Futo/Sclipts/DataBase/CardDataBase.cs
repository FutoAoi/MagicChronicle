using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "DataBase/CardDataBase")]
public class CardDataBase : ScriptableObject
{
    [Header("カードデータベース")]
    [SerializeField,Tooltip("カードリスト")] private List<CardData> _cards = new();

    public List<CardData> Cards { get { return _cards; } }

    private Dictionary<int, CardData> _cardDictionary;
    
    void Initialize()
    {
        if(_cardDictionary == null)
        {
            _cardDictionary = new Dictionary<int, CardData>();
            foreach(var card in _cards)
            {
                if(!_cardDictionary.ContainsKey(card.CardID))
                {
                    _cardDictionary.Add(card.CardID, card);
                }
                else
                {
                    Debug.LogWarning($"重複したキーがあります:{card.CardID}");
                }
            }
        }
    }

    public CardData GetCardData(int ID)
    {
        Initialize();
        if(_cardDictionary.TryGetValue(ID, out var cardData))
        {
            return cardData;
        }
        Debug.LogWarning($"ID{ID}のカードが見つかりません");
        return null;
    }

    public int GetRandomCardIDByRarity(CardRarity rarity)
    {
        Initialize();

        var filtered = _cards.Where(c => c.Rarity == rarity).ToList();

        if (filtered.Count == 0)
        {
            Debug.LogWarning($"{rarity} のカードが見つかりませんでした");
            return -1;
        }

        int index = Random.Range(0, filtered.Count);
        return filtered[index].CardID;
    }
}