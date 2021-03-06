﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using GameData.GameEvents;
using GameData.MapElement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GameData
{
    public class GameDataSource
    {
        public float ProcessedRate { get; private set; } = 0.0f;
        
        public virtual IEnumerator ReadFileAsync(string fileName)
        {
            long startTime = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
            
            // 读取JSON文件并解析
            string playFileContent = File.ReadAllText(fileName);
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(playFileContent);
            JSONData updateInfo = serializer.Deserialize<JSONData>(new JsonTextReader(sr));
            long midTime = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
            yield return null;

            _startData = new StartData
            {
                MapHeight = updateInfo.settings.mapHeight,
                MapWidth = updateInfo.settings.mapWidth, 
                Map = new MapPlace[updateInfo.settings.mapWidth][],
                PollutionComponentProcessPrices = updateInfo.settings.processorTypeCost,
                PollutionComponentProcessProfits = updateInfo.settings.pollutionProfit,
                ActualRoundNum = updateInfo.events.GetLength(0), 
                MaxRoundNum = updateInfo.settings.maxRoundNum,
                DetectorRangePrices = updateInfo.settings.detectorRangeCost,
                Moneys = updateInfo.settings.moneys,
                Scores = updateInfo.settings.scores,
                LandPrice = updateInfo.settings.landPrice,
                TipsterPrice = updateInfo.settings.tipsterCost,
                ProcessorRangePrices = updateInfo.settings.processorRangeCost
            };

            for (int x = 0; x < _startData.MapWidth; x++)
            {
                _startData.Map[x] = new MapPlace[_startData.MapHeight];
                for (int y = 0; y < _startData.MapHeight; y++)
                {
                    var place = new MapPlace(new Point(x, y));
                    _startData.Map[x][y] = place;
                }
            } // 新建地图

            int buildLen = updateInfo.settings.buildings.GetLength(0);
            for (int i = 0; i < buildLen; i++)
            {
                int x = updateInfo.settings.buildings[i, 0];
                int y = updateInfo.settings.buildings[i, 1];
                _startData.Map[x][y].Elements.Add(new Building(new Point(x, y)));
                _startData.Map[x][y].Type = MapPlaceTypes.CANNOT_BUY;
            } // 加入建筑物数据

            for (int x = 0; x < _startData.MapWidth; x++)
            {
                for (int y = 0; y < _startData.MapHeight; y++)
                {
                    int polluVal = updateInfo.settings.pollutionMap[x, y];
                    if (polluVal != 0) // 当前污染点的值, 非0表示有污染源
                    {
                        bool[] components = new bool[updateInfo.settings.pollutionComponentNum];
                        for (int i = 0; i < components.Length; i++, polluVal >>= 1){
                            components[i] = (polluVal & 1) == 1;
                        }
                        // 处理成分

                        bool[] visible = new bool[2];
                        visible[0] = Convert.ToBoolean(updateInfo.settings.pollutionMap0[x, y]);
                        visible[1] = Convert.ToBoolean(updateInfo.settings.pollutionMap1[x, y]);
                        // 处理可见性

                        _startData.Map[x][y].Elements.Add(new PollutionSource(new Point(x, y))
                        {
                            Components = components, Visible = visible
                        });
                    }
                }
            } // 加入污染源数据
            yield return null;

            long cloneTotalTime = 0;
            for(int i = 0; i < _startData.ActualRoundNum; i++)
            {
                //处理每回合事件的方式，可以写成一个复杂的if判断和循环：

                //step 1:深拷贝整张地图（从上一回合的地图拷贝）
                //step 2:生成TurnData对象：注意Moneys、Scores数组要重新new，不能复用
                var turnData = new TurnData() { };
                turnData.Index = i;
                long clone1 = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
                if (i != 0)
                {
                    turnData.Map = Utils.Clone(_turnData[i - 1].Map);
                    turnData.Moneys = new int[] { _turnData[i - 1].Moneys[0], _turnData[i - 1].Moneys[1] };
                    turnData.Scores = new int[] { _turnData[i - 1].Scores[0], _turnData[i - 1].Scores[1] };
                }
                else
                {
                    turnData.Map = Utils.Clone(_startData.Map);
                    turnData.Moneys = new int[] { _startData.Moneys[0], _startData.Moneys[1] };
                    turnData.Scores = new int[] { _startData.Scores[0], _startData.Scores[1] };
                }
                long clone2 = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
                cloneTotalTime += (clone2 - clone1);
                
                // 对当前回合中的事件按编号排序
                var curList = updateInfo.events[i].AsQueryable().ToList();
                curList.Sort((o1, o2) => { return Convert.ToInt32(o1[0]) - Convert.ToInt32(o2[0]); });

                //step 3:遍历当前回合中的事件类型

                PutProcessorEvent processorEvent = null;
                TipsterEvent tipsterEvent = null;
                PutDetectorEvent detectorEvent = null;

                foreach (var currTurnEvent in curList)
                { 
                    int x, y;
                    switch(Convert.ToInt32(currTurnEvent[0]))
                    {
                        case 1: // 建造治理设备
                            turnData.Ai = Convert.ToInt32(currTurnEvent[1]);
                            
                            // 准备治理设备
                            x = Convert.ToInt32((currTurnEvent[2] as JArray)[0]);
                            y = Convert.ToInt32((currTurnEvent[2] as JArray)[1]);
                            var processor = new Processor(new Point(x, y))
                            {
                                Owner = turnData.Ai,
                                RangeType = IntToDeviceType(Convert.ToInt32(currTurnEvent[3])),
                                PollutionComponentIndex = Convert.ToInt32(currTurnEvent[4])
                            };

                            // 准备event
                            processorEvent = new PutProcessorEvent()
                            {
                                Position = new Point(x, y),
                                Processor = processor,
                                Result = new List<Tuple<PollutionSource, int>>() { }
                            };

                            // 修改地图
                            turnData.Map[x][y].Elements.Add(processor);
                            turnData.Moneys[turnData.Ai] -=
                                (_startData.ProcessorRangePrices[Convert.ToInt32(currTurnEvent[3])] +
                                _startData.PollutionComponentProcessPrices[Convert.ToInt32(currTurnEvent[4])]);
                            break;
                       
                        case 10 : // 成功治理污染源
                            turnData.Ai = Convert.ToInt32(currTurnEvent[1]);

                            // 准备事件对象和增加结果
                            x = Convert.ToInt32((currTurnEvent[2] as JArray)[0]);
                            y = Convert.ToInt32((currTurnEvent[2] as JArray)[1]);
                            int profit = Convert.ToInt32(currTurnEvent[3]);
                            processorEvent.Result.Add(new Tuple<PollutionSource, int>(turnData.Map[x][y].GetElement<PollutionSource>(), profit)); 

                            // 修改地图、金钱、分数
                            turnData.Map[x][y].GetElement<PollutionSource>().Curbed = turnData.Ai;
                            turnData.Moneys[turnData.Ai] += profit;
                            turnData.Scores[turnData.Ai] += profit;
                            break;

                        case 2: // 出新的竞价
                            turnData.Ai = Convert.ToInt32(currTurnEvent[1]);

                            // 准备出价信息
                            var bidInfo = new BidInfo()
                            {
                                Ai = turnData.Ai,
                                money = Convert.ToInt32(currTurnEvent[3]),
                                turn = i
                            };

                            // 准备事件对象和放Events中
                            x = Convert.ToInt32((currTurnEvent[2] as JArray)[0]);
                            y = Convert.ToInt32((currTurnEvent[2] as JArray)[1]);
                            var bidEvent = new NewBidEvent()
                            {
                                Position = new Point(x, y),
                                Bid = bidInfo
                            };
                            turnData.Events.Add(bidEvent);

                            // 修改地图
                            turnData.Map[x][y].Bid = bidInfo;
                            break;

                        case 3: // 使用情报贩子
                            turnData.Ai = Convert.ToInt32(currTurnEvent[1]);

                            // 准备事件对象和放Events中
                            x = Convert.ToInt32((currTurnEvent[2] as JArray)[0]);
                            y = Convert.ToInt32((currTurnEvent[2] as JArray)[1]);
                            tipsterEvent = new TipsterEvent()
                            {
                                Position = new Point(x, y),
                                Success = false,
                            };

                            turnData.Moneys[turnData.Ai] -= _startData.TipsterPrice;
                            break;

                        case 4: // 运用情报找到的污染源
                            turnData.Ai = Convert.ToInt32(currTurnEvent[1]);

                            // 修改event
                            x = Convert.ToInt32((currTurnEvent[2] as JArray)[0]);
                            y = Convert.ToInt32((currTurnEvent[2] as JArray)[1]);
                            tipsterEvent.Success = true;
                            tipsterEvent.Result = new Point(x, y);

                            // 修改地图
                            turnData.Map[x][y].GetElement<PollutionSource>().Visible[turnData.Ai] = true;
                            break;

                        case 5: // 放置检测设备
                            turnData.Ai = Convert.ToInt32(currTurnEvent[1]);

                            // 准备检测设备
                            x = Convert.ToInt32((currTurnEvent[2] as JArray)[0]);
                            y = Convert.ToInt32((currTurnEvent[2] as JArray)[1]);
                            var detector = new Detector(new Point(x, y))
                            {
                                Owner = turnData.Ai,
                                RangeType = IntToDeviceType(Convert.ToInt32(currTurnEvent[3])),
                            };

                            // 准备event
                            detectorEvent = new PutDetectorEvent()
                            {
                                Position = new Point(x, y),
                                Detector = detector,
                                Result = new List<PollutionSource>() { }
                            };

                            // 修改地图
                            turnData.Map[x][y].Elements.Add(detector);
                            turnData.Moneys[turnData.Ai] -=
                                _startData.DetectorRangePrices[Convert.ToInt32(currTurnEvent[3])];
                            break;

                        case 6: // 检测到污染源
                            turnData.Ai = Convert.ToInt32(currTurnEvent[1]);

                            // 修改event
                            x = Convert.ToInt32((currTurnEvent[2] as JArray)[0]);
                            y = Convert.ToInt32((currTurnEvent[2] as JArray)[1]);
                            detectorEvent.Result.Add(turnData.Map[x][y].GetElement<PollutionSource>());

                            // 修改地图
                            turnData.Map[x][y].GetElement<PollutionSource>().Visible[turnData.Ai] = true;
                            break;

                        case 7: // 竞价轮数更新，无需处理
                            break;

                        case 8: // 竞价成功
                            turnData.Ai = Convert.ToInt32(currTurnEvent[2]);

                            // 准备event
                            x = Convert.ToInt32((currTurnEvent[1] as JArray)[0]);
                            y = Convert.ToInt32((currTurnEvent[1] as JArray)[1]);
                            var bidSuccessEvent = new BidResultEvent()
                            {
                                Position = new Point(x, y),
                                Success = true
                            };
                            turnData.Events.Add(bidSuccessEvent);

                            // 修改金钱
                            turnData.Map[x][y].Type = MapPlaceTypes.BOUGHT;
                            turnData.Map[x][y].Owner = turnData.Ai;
                            turnData.Moneys[turnData.Ai] -= Convert.ToInt32(currTurnEvent[3]);
                            break;

                        case 9: // 流拍
                            turnData.Ai = Convert.ToInt32(currTurnEvent[2]);

                            // 准备event
                            x = Convert.ToInt32((currTurnEvent[1] as JArray)[0]);
                            y = Convert.ToInt32((currTurnEvent[1] as JArray)[1]);
                            var bidFailureEvent = new BidResultEvent()
                            {
                                Position = new Point(x, y),
                                Success = false
                            };
                            turnData.Events.Add(bidFailureEvent);
                            
                            turnData.Map[x][y].Type = MapPlaceTypes.BOUGHT_FAILED;
                            break;
                        
                        default:
                            throw new Exception("事件编号不是1-10，无法处理");
                    }
                }

                if(processorEvent != null)
                {
                    turnData.Events.Add(processorEvent);
                }
                if (tipsterEvent != null)
                {
                    turnData.Events.Add(tipsterEvent);
                }
                if(detectorEvent != null)
                {
                    turnData.Events.Add(detectorEvent);
                }

                _turnData.Add(turnData);
                ProcessedRate = (float) (i + 1) / _startData.ActualRoundNum;
                yield return null;
            }
            
            long endTime = (long)(DateTime.Now - new System.DateTime(1970, 1, 1)).TotalMilliseconds;
            Debug.Log("GameDataSource: Reading File Cost " + (endTime-startTime) + "ms (in which JSON parsing "+ (midTime-startTime) +"ms, cloning map " + cloneTotalTime +"ms)");
        
        }

        /**
        * 读取播放协议文件（一个符合播放协议的JSON文件），解析为每个回合的状态信息和动作信息，并缓存起来。
        * 该函数允许有比较长的耗时。
        */
        public virtual void ReadFile(string fileName)
        {
            var enumerator = ReadFileAsync(fileName);
            var res = true;
            while (res)
            {
                res = enumerator.MoveNext();
            }
        }

        public DeviceRangeTypes IntToDeviceType(int x)
        {
            switch(x)
            {
                case 0:
                    return DeviceRangeTypes.STRAIGHT;
                case 1:
                    return DeviceRangeTypes.SQUARE;
                case 2:
                    return DeviceRangeTypes.DIAGON;
                default:
                    throw new Exception("该变量值不在枚举范围内！");
            }
        }

        public virtual StartData GetStartData()
        {
            return _startData;
        }

        public virtual TurnData GetTurnData(int turnIndex)
        {
            return _turnData[turnIndex];
        }

        //以下是推荐实现，即在ReadFile中就完整解析播放文件，计算StartData和所有回合的TurnData并混存起来，然后两个Get函数直接返回就可以。
        private StartData _startData;

        private List<TurnData> _turnData = new List<TurnData>();
    }
    
    public class JSONSettings
    {
        public int mapWidth;
        
        public int mapHeight;
        
        public int landPrice;
        
        public int[,] buildings;
        
        public int pollutionComponentNum;
        
        public int maxRoundNum;

        public int maxRangeNum;
        
        public int[] processorRangeCost = new int[3];

        public int[] processorTypeCost;

        public int[] detectorRangeCost = new int[3];

        public int tipsterCost;

        public int[] pollutionProfit;

        public int[,] pollutionMap;

        public int[,] pollutionMap0;

        public int[,] pollutionMap1;

        public int[] scores = new int[2];

        public int[] moneys = new int[2];
    }
    
    public class JSONData
    {
        public JSONSettings settings;

        public object[][][] events;
    }
    public class JSONData2
    {
        //public JSONSettings settings;
        public int[] events;
        
        public JSONData2(){}
    }
}