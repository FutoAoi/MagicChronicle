using UnityEngine;

public class EventDamage : IEventEffect
{
    [SerializeField, Range(1, 100)] private int _healAmount;
    private PlayerStatus _status;
    public EventResult OnExcute()
    {
        _status = GameManager.Instance.PlayerStatus;

        int amount = -(int)(_status.PlayerMaxHp * ((float)_healAmount / 100));
        _status.HealHp(amount);

        return new EventResult { Type = EventResultType.Damage, Amount = amount, IsPositive = false };
    }
}
