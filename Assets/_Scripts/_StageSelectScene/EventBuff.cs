using UnityEngine;

public class EventBuff : IEventEffect
{
    [SerializeField] private BuffType _buffType;
    public EventResult OnExcute()
    {
        GameManager.Instance.PlayerStatus.AddDefaultBuff(_buffType);
        return new EventResult { Type = EventResultType.Buff, ID = (int)_buffType, IsPositive = true };
    }
}
