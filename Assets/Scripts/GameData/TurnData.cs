using System;
using System.Collections.Generic;
using GameData.GameEvents;
using GameData.MapElement;

namespace GameData
{
    [Serializable]
    public class TurnData
    {
        public int Index;//回合序号
        public int Ai;//行动玩家编号0或1
        public MapPlace[][] Map;//本回合末的完整地图
        public List<GameEventBase> Events = new List<GameEventBase>();//本回合中发生的所有事件的数组
        public int[] Scores;//本回合末的分数
        public int[] Moneys;//本回合末的金钱
    }
}