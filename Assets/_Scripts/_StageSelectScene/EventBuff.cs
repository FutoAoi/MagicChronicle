using UnityEngine;

public class EventBuff : IEventEffect
{
    [SerializeField] private BuffType _buffType;
    public void OnExcute()
    {
        GameManager.Instance.PlayerStatus.AddDefaultBuff(_buffType);
    }
}
