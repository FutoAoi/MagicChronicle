using UnityEngine;

public class EventHeal : IEventEffect
{
    [SerializeField,Range(1,100)] private int _healAmount;
    private PlayerStatus _status;
    public EventResult OnExcute()
    {
        _status = GameManager.Instance.PlayerStatus;

        int amount = (int)(_status.PlayerMaxHp * ((float)_healAmount / 100));
        _status.HealHp(amount);

        CriAudioManager.Instance.PlaySe("SE_Heal");
        return new EventResult { Type = EventResultType.Heal, Amount = amount, IsPositive = true };
    }
}
