using UnityEngine;

namespace GameData.MapElement
{
    public class Building: MapElementBase
    {
        public Building() {}
        public Building(MapPlace place) : base(place) {}
        
        public Building(Vector2Int position): base(position){}
    }
}