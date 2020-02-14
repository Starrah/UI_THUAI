﻿using System;
 using System.Collections.Generic;
 using GameData;
 using GameData.GameEvents;
 using GameData.MapElement;

 public class TestGameDataSource: GameDataSource
{
    
    public override void ReadFile(string fileName)
    {
        _startData = new StartData {MapHeight = 20, MapWidth = 20, 
            PollutionComponentProcessPrices = new int[]{30,30}, PollutionComponentProcessProfits = new int[]{300,300},
            ActualRoundNum = 1, MaxRoundNum = 50, DetectorRangePrices = new int[]{10,15,18}, 
            Moneys = new int[]{1000,1000}, Scores = new int[]{0,0}, LandPrice = 10, TipsterPrice = 30, 
            ProcessorRangePrices = new int[]{10,15,18}
        };
        
        _startData.Map = new MapPlace[_startData.MapWidth][]; 
        for (int x = 0; x < _startData.MapWidth; x++)
        {
            _startData.Map[x] = new MapPlace[_startData.MapHeight];
            for (int y = 0; y < _startData.MapHeight; y++)
            {
                var place = new MapPlace(new Point(x, y));
                _startData.Map[x][y] = place;
            }
        }
        _startData.Map[0][0].Elements.Add(new Building(new Point(0,0)));
        _startData.Map[0][2].Elements.Add(new PollutionSource(new Point(0,2))
        {
            Components = new bool[]{true,false},Visible = new bool[]{true, false}
        });
        _startData.Map[1][1].Elements.Add(new Detector(new Point(1,1))
        {
            RangeType = DeviceRangeTypes.SQUARE, Owner = 0
        });

        //处理每回合事件的方式，可以写成一个复杂的if判断和循环：
        //step 1：深拷贝整张地图（从上一回合的地图拷贝）
        var map1 = Utils.Clone(_startData.Map);
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
        var bidEvent = new NewBidEvent() {Position = new Point(1, 0), Bid = bidInfo10};
        turnData.Events.Add(bidEvent);
        //3.1.3 根据事件的作用效果修改地图
        map1[1][0].Bid = bidInfo10;
        
        //3.2 放置治理设备事件
        //3.2.1 准备用到的相关对象
        var processor22 = new Processor(new Point(2,2)){Owner = 0};
        //3.1.2 准备事件对象和放Events中
        var processorEvent = new PutProcessorEvent()
        {
            Position = new Point(2, 2), Processor = processor22,
            Result = new List<Tuple<PollutionSource, int>>()
                {new Tuple<PollutionSource, int>(map1[0][2].GetElement<PollutionSource>(), 300)}
        };
        turnData.Events.Add(processorEvent);
        //3.1.3 根据事件的作用效果修改地图
        map1[2][2].Elements.Add(processor22);
        map1[0][2].GetElement<PollutionSource>().Curbed = 0;
        turnData.Moneys[0] += 300;
        
        //step 4：缓存该turnData进入数组里面
        _turnData.Add(turnData);
    }

    public override StartData GetStartData()
    {
        return _startData;
    }

    public override TurnData GetTurnData(int turnIndex)
    {
        return _turnData[turnIndex];
    }
    
    public StartData _startData;

    public List<TurnData> _turnData = new List<TurnData>();
}