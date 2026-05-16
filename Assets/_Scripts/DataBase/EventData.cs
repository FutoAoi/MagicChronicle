using UnityEngine;

[CreateAssetMenu(menuName = "Datas/Event")]
public class EventData : ScriptableObject
{
    [SerializeField, Tooltip("イベントID")] private int _eventID;
    [SerializeField, Tooltip("名前")] private string _name;
    [SerializeField, Tooltip("イベントテキスト")] private string _description;
    [SerializeField, Tooltip("背景")] private Sprite _backGround;
    [SerializeField, Tooltip("選択肢")] private EventChoice[] _choices;

    public int EventID => _eventID;
    public string Name => _name;
    public string Description => _description;
    public Sprite BackGround => _backGround;
    public EventChoice[] Choices => _choices;
}
