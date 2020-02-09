namespace GameData.MapElement
{
    public class Detector: MapElementBase
    {
        public Detector(MapPlace place) : base(place)
        {
        }

        public int Owner = -1;
        
        public DeviceRangeTypes RangeType;//覆盖区域范围类型

    }
}