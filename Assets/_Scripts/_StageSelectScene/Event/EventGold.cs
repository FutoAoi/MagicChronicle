using UnityEngine;

public class EventGold : IEventEffect
{
    [SerializeField] private int _amount;
    public EventResult OnExcute()
    {
        WalletManager.Instance.ChangePlayerMoney(_amount);
        return new EventResult { Type = EventResultType.Gold, Amount = _amount, IsPositive = _amount >= 0 };
    }

    public void PlaySE()
    {
        CriAudioManager.Instance.PlaySe("SE_MoneyDrop");
    }
}
