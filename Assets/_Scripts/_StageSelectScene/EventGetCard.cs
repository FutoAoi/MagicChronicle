using UnityEngine;

public class EventGetCard : IEventEffect
{
    [SerializeField] private CardType _type;
    [SerializeField] private CardRarity _rarity;
    public void OnExcute()
    {
        DeckManager.Instance.AddDeck(GameManager.Instance.CardDataBase.GetRandomCardIDByRarity(_rarity,_type));
    }
}
