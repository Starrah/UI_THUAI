using UnityEngine;

namespace GameData.MapElement
{ 
    public abstract class MapElementBase
    {
        public Vector2Int Position;

        public MapPlace GetPlace(MapPlace[][] map)
        {
            return map[Position.x][Position.y];
        }

        public MapElementBase(Vector2Int position)
        {
            Position = position;
        }
        public MapElementBase(MapPlace place)
        {
            Position = place.Position;
        }
    }
}