using System;
using UnityEngine;

namespace GameData.MapElement
{
    [Serializable]
    public class Detector: MapElementBase
    {
        public Detector(){}
        
        public Detector(MapPlace place) : base(place) {}
        
        public Detector(Point position): base(position){}

        public int Owner = -1;
        
        public DeviceRangeTypes RangeType;//覆盖区域范围类型

    }
}