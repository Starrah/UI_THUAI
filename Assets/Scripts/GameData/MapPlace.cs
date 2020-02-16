using System;
using System.Collections.Generic;
using System.Numerics;
using GameData.MapElement;
using UnityEngine;

namespace GameData
{
    [Serializable]
    public class MapPlace
    {
        public Point Position;
        public MapPlaceTypes Type = MapPlaceTypes.EMPTY;//类型
        public int Owner = -1;//所有者，若无人所有就是-1，否则是0或1
        public BidInfo Bid = null;//当前的出价信息。流拍后或卖出后，需要一直保留着最后一次的出价信息。
        public List<MapElementBase> Elements = new List<MapElementBase>();

        public MapPlace(): this(new Point()){}
        
        public MapPlace(Point position)
        {
            Position = position;
        }

        public T GetElement<T>()
            where T : MapElementBase
        {
            foreach (MapElementBase element in Elements)
            {
                if (element is T t)
                {
                    return t;
                }
            }

            return null;
        }
    }
    
    public enum MapPlaceTypes
    {
        CANNOT_BUY,//不能购买（已有高大建筑物）
        EMPTY,//尚未卖出（尚无人出价或出价在等待期间）
        BOUGHT_FAILED,//有人出了价但是流拍了
        BOUGHT//已有人购买
    }
}