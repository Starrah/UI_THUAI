using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using GameData;
using GameData.MapElement;
using UnityEngine;

public static class Utils
{
    public static T Clone<T>(T obj)
        where T: class
    {
        MemoryStream stream = new MemoryStream();
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, obj);
        stream.Position = 0;
        var yyy = formatter.Deserialize(stream) as T;
        return yyy;
    }

    public static bool HaveBuilding(MapPlace[][] map, Point centerPosition, Point offset)
    {
        var width = map.Length;
        var height = map[0].Length;
        var res = new Point(centerPosition.x + offset.x, centerPosition.y + offset.y);
        if (res.x < 0 || res.x >= width || res.y < 0 || res.y >= height) return true;
        return map[res.x][res.y].GetElement<Building>() != null;
    }

    private static readonly Dictionary<DeviceRangeTypes, List<(Point, List<int>)>> _blockReference = new
        Dictionary<DeviceRangeTypes, List<(Point, List<int>)>>()
        {
            {
                DeviceRangeTypes.STRAIGHT, new List<(Point, List<int>)>()
                {
                    (new Point(-1, 0), new List<int>() {2, 6}),
                    (new Point(1, 0), new List<int>() {0, 4}),
                    (new Point(0, -1), new List<int>() {1, 5}),
                    (new Point(0, 1), new List<int>() {3, 7}),
                    (new Point(-2, 0), new List<int>() {6}),
                    (new Point(2, 0), new List<int>() {4}),
                    (new Point(0, -2), new List<int>() {5}),
                    (new Point(0, 2), new List<int>() {7}),
                }
            },
            {
                DeviceRangeTypes.SQUARE, new List<(Point, List<int>)>()
                {
                    (new Point(-1, 0), new List<int>() {4}),
                    (new Point(1, 0), new List<int>() {0}),
                    (new Point(0, -1), new List<int>() {2}),
                    (new Point(0, 1), new List<int>() {6}),
                    (new Point(-1, -1), new List<int>() {3}),
                    (new Point(1, -1), new List<int>() {1}),
                    (new Point(-1, 1), new List<int>() {7}),
                    (new Point(1, 1), new List<int>() {5}),
                }
            },
            {
                DeviceRangeTypes.DIAGON, new List<(Point, List<int>)>()
                {
                    (new Point(-1, -1), new List<int>() {2,6}),
                    (new Point(1, -1), new List<int>() {1,5}),
                    (new Point(-1, 1), new List<int>() {3,7}),
                    (new Point(1, 1), new List<int>() {4,8}),
                    (new Point(-2, -2), new List<int>() {6}),
                    (new Point(2, -2), new List<int>() {5}),
                    (new Point(-2, 2), new List<int>() {7}),
                    (new Point(2, 2), new List<int>() {8}),
                }
            },
        };

    public static int CalculateBlocked(MapPlace[][] map, Detector obj)
    {
        var res = 0;
        foreach ((Point, List<int>) tuple in _blockReference[obj.RangeType])
        {
            if (HaveBuilding(map, obj.Position, tuple.Item1))
            {
                foreach (int bitIndex in tuple.Item2)
                {
                    res = res | (1 << bitIndex);
                }
            }    
        }

        return res;
    }
    
    public static int CalculateBlocked(MapPlace[][] map, Processor obj)
    {
        var res = 0;
        foreach ((Point, List<int>) tuple in _blockReference[obj.RangeType])
        {
            if (HaveBuilding(map, obj.Position, tuple.Item1))
            {
                foreach (int bitIndex in tuple.Item2)
                {
                    res = res | (1 << bitIndex);
                }
            }    
        }
        
        return res;
    }
}