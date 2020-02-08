using System.Collections.Generic;

namespace GameData.MapElement
{
    public class PollutionSource: MapElementBase
    {
        public PollutionSource(MapPlace place) : base(place)
        {
        }
        
        public int Curbed = -1;//-1表示未被治理，0或1表示已被某个玩家治理
        
        /**
         * 污染物成分列表
         * 长度等于污染物总的种类数；对应每种污染物，true为存在，false为不存在
         */
        public bool[] Components;

        /**
         * 长为2的数组，表示污染源对玩家的可见性
         * 下标0和1分别对应两个玩家AI0和AI1，例如{true false}表示AI0可见、AI1不可见
         */
        public bool[] Visible = new bool[2];
        
    }
}