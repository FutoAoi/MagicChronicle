using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GenerateMapData MapData => _mapData;
    public RoomIconData RoomIconData => _roomIconData;

    [SerializeField] GenerateMapData _mapData;
    [SerializeField] RoomIconData _roomIconData;
    [SerializeField] EventPanelController _eventPanelController;
    [SerializeField] ShopManager _shopManager;
    [SerializeField] GameObject _gameObject;

    private void Start()
    {
        _mapData = GameManager.Instance.GenerateMapData;
    }

    public GenerateRoomData CurrentRoom
    {
        get
        {
            return MapData.Floors[MapData.CurrentFloorIndex].Rooms[MapData.CurrentRoomIndex];
        }
    }
    
    /// <summary>
    /// 生成したマップデータのセット
    /// </summary>
    /// <param name="mapData"></param>
    public void SetMap(GenerateMapData mapData)
    {
        _mapData = mapData;
    }

    /// <summary>
    /// 次の部屋に移動できるかの判定
    /// </summary>
    /// <param name="nextRoomIndex"></param>
    /// <returns></returns>
    public bool CanMoveTo(int nextRoomIndex)
    {
        foreach (int index in CurrentRoom.NextRoomIndices)
        {
            if(index == nextRoomIndex) return true;
        }
        return false;
    }
    
    public void MoveTo(int nextRoomIndex)
    {
        if(!CanMoveTo(nextRoomIndex))
        {
            Debug.LogWarning("移動できない部屋です");
            return;
        }

        CurrentRoom.IsCleared = true;

        MapData.CurrentFloorIndex++;
        MapData.CurrentRoomIndex = nextRoomIndex;

        Debug.Log($"移動先 Floor:{MapData.CurrentFloorIndex} Room:{nextRoomIndex}");

        GameManager.Instance.SceneChange(SceneType.InGameScene);
    }

    public void OpenEventPanel(int nextRoomIndex, int eventID)
    {
        if (!CanMoveTo(nextRoomIndex))
        {
            Debug.LogWarning("移動できない部屋です");
            return;
        }

        CurrentRoom.IsCleared = true;

        _eventPanelController.SetupEvent(eventID);

        MapData.CurrentFloorIndex++;
        MapData.CurrentRoomIndex = nextRoomIndex;
    }

    public void OpenShopPanel(int nextRoomIndex)
    {
        if (!CanMoveTo(nextRoomIndex))
        {
            Debug.LogWarning("移動できない部屋です");
            return;
        }

        CurrentRoom.IsCleared = true;

        _shopManager.InitShop();

        MapData.CurrentFloorIndex++;
        MapData.CurrentRoomIndex = nextRoomIndex;
    }

    public void OpenEndPanel()
    {
        _gameObject.SetActive(true);
    }
}
