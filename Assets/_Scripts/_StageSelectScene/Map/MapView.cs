using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapView : MonoBehaviour
{
    [SerializeField] private Room _roomPrefab;
    [SerializeField] private RectTransform _linePrefab;
    [SerializeField] private MapManager _mapManager;
    [SerializeField] private Image _circle;

    private Room[][] _roomViews;
    float _xSpacing;
    float _ySpacing;
    float _offsetX;
    float _offsetY;
    float _x;
    float _y;

    public void Start()
    {
        CreateMap(GameManager.Instance.GenerateMapData);
    }

    /// <summary>
    /// マップツリーの表示
    /// </summary>
    /// <param name="mapData"></param>
    public void CreateMap(GenerateMapData mapData)
    {
        int floorCount = mapData.Floors.Length;
        _roomViews = new Room[floorCount][];

        for (int f  = 0; f < floorCount; f++)
        {
            GenerateFloorData floorData = mapData.Floors[f];
            int roomCount = floorData.Rooms.Length;
            _roomViews[f] = new Room[roomCount];
            for (int r  = 0; r < roomCount; r++)
            {
                Room room = Instantiate(_roomPrefab,transform);
                room.SetRoomData(floorData.Rooms[r],_mapManager);

                RectTransform rt = room.GetComponent<RectTransform>();

                rt.anchoredPosition = GetRoomPosition(f, r, roomCount);

                _roomViews[f][r] = room;
            }
        }

        for (int f = 0;f < floorCount - 1; f++)
        {
            for (int r = 0; r < _roomViews[f].Length; r++)
            {
                GenerateRoomData roomData = mapData.Floors[f].Rooms[r];
                List<Room> nextRooms = new();
                
                foreach (int nextIndex in roomData.NextRoomIndices)
                {
                    nextRooms.Add(_roomViews[f+1][nextIndex]);
                }

                _roomViews[f][r].SetNextRoom(nextRooms.ToArray());
            }
        }

        for (int f = 0; f < floorCount - 1; f++)
        {
            for (int r = 0; r < _roomViews[f].Length; r++)
            {
                GenerateRoomData data = mapData.Floors[f].Rooms[r];

                foreach (int next in data.NextRoomIndices)
                {
                    DrawUILine(
                        _roomViews[f][r].GetComponent<RectTransform>(),
                        _roomViews[f + 1][next].GetComponent<RectTransform>()
                    );
                }
            }
        }

        UpdataPlayerPosition();
    }

    /// <summary>
    /// 部屋と部屋を線でつなぐ
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    private void DrawUILine(RectTransform from, RectTransform to)
    {
        RectTransform line = Instantiate(_linePrefab, transform);
        line.SetSiblingIndex(0);

        Vector2 start = from.anchoredPosition;
        Vector2 end = to.anchoredPosition;

        Vector2 dir = end - start;
        float distance = dir.magnitude;

        line.anchoredPosition = start + dir * 0.5f;

        line.sizeDelta = new Vector2(
            line.sizeDelta.x,
            distance
        );

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        line.localRotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    /// <summary>
    /// 部屋の位置を決める
    /// </summary>
    /// <param name="floorIndex"></param>
    /// <param name="roomIndex"></param>
    /// <param name="roomCount"></param>
    /// <returns></returns>
    private Vector3 GetRoomPosition(int floorIndex, int roomIndex, int roomCount)
    {
        _xSpacing = 200f;
        _ySpacing = 200f;

        _offsetX = -(roomCount - 1) * _xSpacing * 0.5f;
        _offsetY = -700f;

        _x = _offsetX + roomIndex * _xSpacing;
        _y = _offsetY + floorIndex * _ySpacing;

        return new Vector3(_x, _y, 0f);
    }

    public void UpdataPlayerPosition()
    {
        _circle.transform.localPosition = _roomViews[GameManager.Instance.GenerateMapData.CurrentFloorIndex][GameManager.Instance.GenerateMapData.CurrentRoomIndex].transform.localPosition;
    }
}
