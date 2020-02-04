using UnityEngine;

namespace GameData.MapElement
{ 
    public abstract class MapElementBase
    {
        public MapPlace Place;//所在的位置的MapPlace对象

        public MapElementBase(MapPlace place)
        {
            Place = place;
        }
    }
}