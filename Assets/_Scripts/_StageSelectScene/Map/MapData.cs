using UnityEngine;

public enum RoomType
{
    None,
    Buttle,
    Elite,
    Rest,
    Event,
    Shop,
}

[CreateAssetMenu(menuName = "Map/MapData")]
public class MapData : ScriptableObject
{
    [SerializeField] private FloorData[] _floorDatas;
    public FloorData[] FloorDatas => _floorDatas;
}