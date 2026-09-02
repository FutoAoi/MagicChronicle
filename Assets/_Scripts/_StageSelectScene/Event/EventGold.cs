using UnityEngine;

public class EventGold : IEventEffect
{
    [SerializeField] private int _amount;
    public EventResult OnExcute()
    {
        WalletManager.Instance.ChangePlayerMoney(_amount);
        CriAudioManager.Instance.PlaySe("SE_MoneyDrop");
        return new EventResult { Type = EventResultType.Gold, Amount = _amount, IsPositive = _amount >= 0 };
    }
}
