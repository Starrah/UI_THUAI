using UnityEngine;

namespace GameData.MapElement
{
    public class Processor: MapElementBase
    {
        public Processor(){}
        public Processor(MapPlace place) : base(place) {}
        
        public Processor(Vector2Int position): base(position){}
        
        public int Owner = -1;
        public DeviceRangeTypes RangeType;//覆盖区域范围类型
        public int PollutionComponentIndex;//能治理的污染成分的编号
        
    }
}