using UnityEngine;

public class EventDebug : IEventEffect
{
    [SerializeField] private string text;
    public void OnExcute()
    {
        Debug.Log(text);
    }
}
