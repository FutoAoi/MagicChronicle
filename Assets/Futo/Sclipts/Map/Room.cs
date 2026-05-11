using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Room : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private RoomType _roomType;
    [SerializeField] private int _stageID;
    [SerializeField] private Room[] _nextroom;
    [SerializeField] private Image _roomImage;

    private MapManager _mapManager;
    private int _floorIndex;
    private int _roomIndex;

    /// <summary>
    /// ルーム情報セット
    /// </summary>
    /// <param name="roomData"></param>
    /// <param name="mapManager"></param>
    public void SetRoomData(GenerateRoomData roomData,MapManager mapManager)
    {
        _roomType = roomData.RoomType;
        _stageID = roomData.StageID;
        _floorIndex = roomData.FloorIndex;
        _roomIndex = roomData.RoomIndex;
        _mapManager = mapManager;
        _roomImage.sprite = _mapManager.RoomIconData.GetSprite(_roomType);
    }

    public void SetNextRoom(Room[] nextRoom)
    {
        _nextroom = nextRoom;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_floorIndex == _mapManager.MapData.CurrentFloorIndex + 1)
        {
            if(_roomType == RoomType.Event)
            {
                _mapManager.OpenEventPanel(_roomIndex, _stageID);
            }
            else
            {
                _mapManager.MoveTo(_roomIndex);
                GameManager.Instance.StageID = _stageID;
            }
        }
        else
        {
            Debug.Log("今は選択できない部屋");
        }
    }
}
