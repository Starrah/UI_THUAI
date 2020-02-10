using System;
using UnityEngine;

namespace GameData.MapElement
{
    [Serializable]
    public class Building: MapElementBase
    {
        public Building() {}
        public Building(MapPlace place) : base(place) {}
        
        public Building(Point position): base(position){}
    }
}