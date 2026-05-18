using UnityEngine;

[CreateAssetMenu(menuName = "Map/RoomIconData")]
public class RoomIconData : ScriptableObject
{
    [System.Serializable]
    private class RoomIconEntry
    {
        public RoomType RoomType => _roomType;
        public Sprite RoomIcon => _roomIcon;

        [SerializeField] private RoomType _roomType;
        [SerializeField] private Sprite _roomIcon;
    }

    [SerializeField] private RoomIconEntry[] _roomIconEntries;

    public Sprite GetSprite(RoomType roomType)
    {
        foreach (RoomIconEntry entry in _roomIconEntries)
        {
            if(entry.RoomType == roomType)
            {
                return entry.RoomIcon;
            }
        }
        Debug.LogWarning($"{roomType}‚ÌƒAƒCƒRƒ“‚ªİ’è‚³‚ê‚Ä‚¢‚Ü‚¹‚ñ");
        return null;
    }
}
