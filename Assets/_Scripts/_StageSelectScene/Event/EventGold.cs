using UnityEngine;

public class EventGold : IEventEffect
{
    [SerializeField] private int _amount;
    public void OnExcute()
    {
        WalletManager.Instance.ChangePlayerMoney(_amount);
    }
}
