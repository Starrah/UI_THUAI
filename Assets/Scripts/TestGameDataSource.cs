﻿using System;
 using System.Collections.Generic;
 using System.Reflection;
 using GameData;
 using GameData.GameEvents;
 using GameData.MapElement;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

public class TestGameDataSource: GameDataSource
{
    public static T DeepCopyByReflect<T>(T obj)
    {
        //如果是字符串或值类型则直接返回
        if (obj is string || obj.GetType().IsValueType) return obj;

        object retval = Activator.CreateInstance(obj.GetType());
        FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        foreach (FieldInfo field in fields)
        {
            try { field.SetValue(retval, DeepCopyByReflect(field.GetValue(obj))); }
            catch { }
        }
        return (T)retval;
    }
    
    public new void ReadFile(string fileName)
    {
        _startData = new StartData {MapHeight = 3, MapWidth = 3, Map = new MapPlace[][3], 
            PollutionComponentProcessPrices = new int[]{30,30}, PollutionComponentProcessProfits = new int[]{300,300},
            ActualRoundNum = 2, MaxRoundNum = 50, DetectorRangePrices = new int[]{10,15,18}, 
            Moneys = new int[]{1000,1000}, Scores = new int[]{0,0}, LandPrice = 10, TipsterPrice = 30, 
            ProcessorRangePrices = new int[]{10,15,18}
        };
        for (int x = 0; x < _startData.MapWidth; x++)
        {
            _startData.Map[x] = new MapPlace[3];
            for (int y = 0; y < _startData.MapHeight; y++)
            {
                var place = new MapPlace(new Vector2(x, y));
                _startData.Map[x][y] = place;
            }
        }
        _startData.Map[0][0].Elements.Add(new Building(_startData.Map[0][0]));
        _startData.Map[0][1].Elements.Add(new PollutionSource(_startData.Map[0][1])
        {
            Components = new bool[]{true,false},Visible = new bool[]{true, false}
        });
        _startData.Map[1][1].Elements.Add(new Detector(_startData.Map[1][1])
        {
            RangeType = DeviceRangeTypes.SQUARE,
            Owner = 0
        });

        var map1 = DeepCopyByReflect<MapPlace[][]>(_startData.Map);
        var Processor0 = new Processor(){Owner = 0, Place = _startData[]};
        _turnData[0] = new TurnData(){Ai = 0, Index = 0, Moneys = new int[]{1000, 1000}, Scores = new int[]{0, 0},
            Events = new List<GameEventBase>()
            {
                new NewBidEvent(){Bid = new BidInfo(){Ai = 0, money = 80, turn = 0}},
                new PutProcessorEvent(){}
            }};
        
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