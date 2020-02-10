using System;
using UnityEngine;

namespace GameData.MapElement
{ 
    [Serializable]
    public abstract class MapElementBase
    {
        public Point Position;

        public MapElementBase(){}
        public MapPlace GetPlace(MapPlace[][] map)
        {
            return map[Position.x][Position.y];
        }

        public MapElementBase(Point position)
        {
            Position = position;
        }
        public MapElementBase(MapPlace place)
        {
            Position = place.Position;
        }
    }
}