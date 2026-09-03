using UnityEngine;

public class EventGetCard : IEventEffect
{
    [SerializeField] private CardType _type;
    [SerializeField] private CardRarity _rarity;
    public EventResult OnExcute()
    {
        int cardID = GameManager.Instance.CardDataBase.GetRandomCardIDByRarity(_rarity, _type);
        DeckManager.Instance.AddDeck(cardID);
        return new EventResult { Type = EventResultType.Card, ID = cardID, IsPositive = true };
    }
}
