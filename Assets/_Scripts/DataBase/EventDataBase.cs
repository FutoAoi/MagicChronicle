using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("DataBase/EventDataBase"))]
public class EventDataBase : ScriptableObject
{
    [Header("イベントデータベース")]
    [SerializeField, Tooltip("イベントリスト")] private EventData[] _eventDatas;

    private Dictionary<int, EventData> _eventDictionary;

    /// <summary>
    /// ディクショナリーに登録
    /// </summary>
    private void Initialize()
    {
        if(_eventDictionary == null)
        {
            _eventDictionary = new();
            foreach (EventData eventDate in _eventDatas)
            {
                if(!_eventDictionary.ContainsKey(eventDate.EventID))
                {
                    _eventDictionary.Add(eventDate.EventID, eventDate);
                }
                else
                {
                    Debug.LogWarning($"重複したキーがあります:{eventDate.EventID}");
                }
            }
        }
    }

    public EventData GetEventData(int eventID)
    {
        Initialize();
        if(_eventDictionary.TryGetValue(eventID, out EventData eventData))
        {
            return eventData;
        }
        Debug.LogWarning($"ID{eventID}のイベントが見つかりません");
        return null;
    }
}
