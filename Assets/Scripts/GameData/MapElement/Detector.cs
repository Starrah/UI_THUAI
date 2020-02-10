using UnityEngine;

namespace GameData.MapElement
{
    public class Detector: MapElementBase
    {
        public Detector(): base(Vector2Int.zero){}
        
        public Detector(MapPlace place) : base(place) {}
        
        public Detector(Vector2Int position): base(position){}

        public int Owner = -1;
        
        public DeviceRangeTypes RangeType;//覆盖区域范围类型

    }
}