using System.Collections.Generic;
using UnityEngine;

public class MapGenerator
{
    public static GenerateMapData GenerateMap(MapData mapData)
    {
        GenerateMapData map = new();
        map.CurrentFloorIndex = 0;
        map.CurrentRoomIndex = 0;

        int floorCount = mapData.FloorDatas.Length;
        map.Floors = new GenerateFloorData[floorCount];

        for(int f = 0; f < floorCount; f++)
        {
            FloorData floorData = mapData.FloorDatas[f];

            int roomCount = Random.Range(floorData.MinNodes, floorData.MaxNodes + 1);

            GenerateFloorData floor = new();
            floor.Rooms = new GenerateRoomData[roomCount];

            for (int r = 0; r < roomCount; r++)
            {
                RoomData baseRoom = floorData.RoomDatas[Random.Range(0, floorData.RoomDatas.Length)];

                floor.Rooms[r] = new GenerateRoomData { FloorIndex = f, RoomIndex = r, StageID = baseRoom.StageID,RoomType = baseRoom.RoomType,IsCleared = false };
            }

            map.Floors[f] = floor;
        }

        for (int f = 0; f < floorCount - 1; f++)
        {
            GenerateFloorData currentFloor = map.Floors[f];
            GenerateFloorData nextFloor = map.Floors[f + 1];

            int currentCount = currentFloor.Rooms.Length;
            int nextCount = nextFloor.Rooms.Length;

            // ---------- フェーズ①：基本接続 ----------
            for (int r = 0; r < currentCount; r++)
            {
                List<int> nextIndices = new();

                // index対応（交差しない）
                float t = (float)r / Mathf.Max(1, currentCount - 1);
                int baseIndex = Mathf.RoundToInt(t * (nextCount - 1));

                nextIndices.Add(baseIndex);

                // 分岐（隣のみ）
                if (baseIndex + 1 < nextCount && Random.value < 0.4f)
                    nextIndices.Add(baseIndex + 1);

                if (baseIndex - 1 >= 0 && Random.value < 0.3f)
                    nextIndices.Add(baseIndex - 1);

                currentFloor.Rooms[r].NextRoomIndices =
                    nextIndices.ToArray();
            }

            // ---------- フェーズ②：孤立部屋の救済 ----------
            bool[] hasIncoming = new bool[nextCount];

            // どの部屋に線が来ているかチェック
            for (int r = 0; r < currentCount; r++)
            {
                foreach (int idx in currentFloor.Rooms[r].NextRoomIndices)
                {
                    hasIncoming[idx] = true;
                }
            }

            // 来ていない部屋を救済
            for (int nextIdx = 0; nextIdx < nextCount; nextIdx++)
            {
                if (hasIncoming[nextIdx])
                    continue;

                // 一番近い currentRoom を探す
                int nearest = Mathf.RoundToInt(
                    (float)nextIdx / (nextCount - 1) * (currentCount - 1)
                );

                List<int> list =
                    new List<int>(currentFloor.Rooms[nearest].NextRoomIndices);

                list.Add(nextIdx);
                currentFloor.Rooms[nearest].NextRoomIndices = list.ToArray();
            }
        }
        //for (int f = 0;f < floorCount - 1; f++)
        //{
        //    GenerateFloorData currentFloor = map.Floors[f];
        //    GenerateFloorData nextFloor = map.Floors[f + 1];
        //    int count = 0;
        //    for (int r = 0;r < currentFloor.Rooms.Length; r++)
        //    {
        //        List<int> nextIndices = new();
        //        if (currentFloor.Rooms.Length == 1)
        //        {
        //            for (int i = 0; i < nextFloor.Rooms.Length; i++)
        //            {
        //                nextIndices.Add(i);
        //            }
        //        }
        //        else
        //        {
        //            if(nextFloor.Rooms.Length == 1)
        //            {
        //                nextIndices.Add(count);
        //            }
        //            else
        //            {

        //            }
        //        }
        //        currentFloor.Rooms[r].NextRoomIndices = nextIndices.ToArray();
        //    }
        //}
        return map;
    }
}
