using UnityEngine;

namespace GameData.MapElement
{ 
    public abstract class MapElementBase
    {
        public Vector2 Position;
        public MapElementTypes Type = MapElementTypes.CANNOT_BUY;//类型
        public int Owner = -1;//所有者，若无人所有就是-1，否则是0或1
        public BidInfo Bid = null;//当前的出价信息。流拍后或卖出后，需要一直保留着最后一次的出价信息。
    }

    public enum MapElementTypes
    {
        CANNOT_BUY,//不能购买（已有高大建筑物）
        EMPTY,//尚未卖出（尚无人出价或出价在等待期间）
        BOUGHT_FAILED,//有人出了价但是流拍了
        BOUGHT//已有人购买
    }
}