using GameData.MapElement;
using UnityEngine;

namespace GameData.GameEvents
{
    public class GameEventBase
    {
        public Vector2Int Position;//事件发生的位置对应的地图元素的对象
        
        public MapPlace GetPlace(MapPlace[][] map)
        {
            return map[Position.x][Position.y];
        }
    }
}