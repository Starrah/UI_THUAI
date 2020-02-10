﻿using System;
 using System.Collections.Generic;
 using System.IO;
 using System.Reflection;
 using System.Runtime.Serialization.Formatters.Binary;
 using System.Runtime.Serialization.Json;
 using System.Text;
 using GameData;
 using GameData.GameEvents;
 using GameData.MapElement;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

public class TestGameDataSource: GameDataSource
{
    public static string Serialize<T>(T obj)
        where T: class
    {
        DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(T));
        MemoryStream msObj = new MemoryStream();
        //将序列化之后的Json格式数据写入流中
        js.WriteObject(msObj, obj);
        msObj.Position = 0;
        //从0这个位置开始读取流中的数据
        StreamReader sr = new StreamReader(msObj, Encoding.UTF8);
        string json = sr.ReadToEnd();
        sr.Close();
        msObj.Close();
        return json;
    }
    
    public static T Deserialize<T>(string json)
        where T: class
    {
        using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
        {
            DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(T));
            return deseralizer.ReadObject(ms) as T;// //反序列化ReadObject
        }
    }
    
    public static T DeepCopyObject<T>(T obj)
        where T: class
    {
        if (obj is MapPlace[][])
        {
            return DeepCopy2DArray(obj as MapPlace[][]) as T;
        }
        var qwq = Serialize(obj); 
        return Deserialize<T>(qwq);
    }

    public static T[][] DeepCopy2DArray<T>(T[][] obj)
        where T: class
    {
        T[][] result = new T[obj.Length][];
        for (int i = 0; i < obj.Length; i++)
        {
            T[] subArray = obj[i];
            result[i] = new T[subArray.Length];
            for (int j = 0; j < subArray.Length; j++)
            {
                result[i][j] = DeepCopyObject(subArray[j]);
            }
        }

        return result;
    }
    
    public new void ReadFile(string fileName)
    {
        _startData = new StartData {MapHeight = 3, MapWidth = 3, Map = new MapPlace[3][], 
            PollutionComponentProcessPrices = new int[]{30,30}, PollutionComponentProcessProfits = new int[]{300,300},
            ActualRoundNum = 1, MaxRoundNum = 50, DetectorRangePrices = new int[]{10,15,18}, 
            Moneys = new int[]{1000,1000}, Scores = new int[]{0,0}, LandPrice = 10, TipsterPrice = 30, 
            ProcessorRangePrices = new int[]{10,15,18}
        };
        for (int x = 0; x < _startData.MapWidth; x++)
        {
            _startData.Map[x] = new MapPlace[3];
            for (int y = 0; y < _startData.MapHeight; y++)
            {
                var place = new MapPlace(new Vector2Int(x, y));
                _startData.Map[x][y] = place;
            }
        }
        _startData.Map[0][0].Elements.Add(new Building(new Vector2Int(0,0)));
        _startData.Map[0][2].Elements.Add(new PollutionSource(new Vector2Int(0,2))
        {
            Components = new bool[]{true,false},Visible = new bool[]{true, false}
        });
        _startData.Map[1][1].Elements.Add(new Detector(new Vector2Int(1,1))
        {
            RangeType = DeviceRangeTypes.SQUARE, Owner = 0
        });

        //处理每回合事件的方式，可以写成一个复杂的if判断和循环：
        //step 1：深拷贝整张地图（从上一回合的地图拷贝）
        var map1 = DeepCopyObject(_startData.Map);
        //step 2:生成TurnData对象：注意Moneys、Scores数组要重新new，不能复用
        var turnData = new TurnData(){Map = map1, Ai = 0, Index = 0, 
            Moneys = new int[]{_startData.Moneys[0], _startData.Moneys[1]},
            Scores = new int[]{_startData.Scores[0], _startData.Scores[1]},
        };
        
        //step 3：产生事件和修改地图
        //3.1 出价事件
        //3.1.1 准备用到的相关对象
        var bidInfo10 = new BidInfo() {Ai = 0, money = 80, turn = 0};
        //3.1.2 准备事件对象和放Events中
        var bidEvent = new NewBidEvent() {Position = new Vector2Int(1, 0), Bid = bidInfo10};
        turnData.Events.Add(bidEvent);
        //3.1.3 根据事件的作用效果修改地图
        map1[1][0].Bid = bidInfo10;
        
        //3.2 放置治理设备事件
        //3.2.1 准备用到的相关对象
        var processor22 = new Processor(new Vector2Int(2,2)){Owner = 0};
        //3.1.2 准备事件对象和放Events中
        var processorEvent = new PutProcessorEvent()
        {
            Position = new Vector2Int(2, 2), Processor = processor22,
            Result = new List<Tuple<PollutionSource, int>>()
                {new Tuple<PollutionSource, int>(map1[0][2].GetElement<PollutionSource>(), 300)}
        };
        turnData.Events.Add(processorEvent);
        //3.1.3 根据事件的作用效果修改地图
        map1[2][2].Elements.Add(processor22);
        map1[0][2].GetElement<PollutionSource>().Curbed = 0;
        turnData.Moneys[0] += 300;
        
        //step 4：缓存该turnData进入数组里面
        _turnData[0] = turnData;
    }

    public new StartData GetStartData()
    {
        return _startData;
    }

    public new TurnData GetTurnData(int turnIndex)
    {
        return _turnData[turnIndex];
    }
    
    public StartData _startData;

    public List<TurnData> _turnData;
}