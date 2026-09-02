using UnityEngine;

public class EventDebug : IEventEffect
{
    [SerializeField] private string text;
    public EventResult OnExcute()
    {
        Debug.Log(text);
        return new EventResult { Type = EventResultType.None };
    }
}
