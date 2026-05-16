using System;

[Serializable]
public class GenerateMapData
{
    public GenerateFloorData[] Floors;
    public int CurrentFloorIndex;
    public int CurrentRoomIndex;
}
[Serializable]
public class GenerateFloorData
{
    public GenerateRoomData[] Rooms;
}

[Serializable]
public class GenerateRoomData
{
    public int StageID;
    public RoomType RoomType;
    public int FloorIndex;
    public int RoomIndex;
    public int[] NextRoomIndices;
    public bool IsCleared;
}