using System;
using GameData.MapElement;
using UnityEngine;

namespace GameData.GameEvents
{
    [Serializable]
    public class GameEventBase
    {
        public Point Position;//事件发生的位置对应的地图元素的对象
        
        public MapPlace GetPlace(MapPlace[][] map)
        {
            return map[Position.x][Position.y];
        }
    }
}