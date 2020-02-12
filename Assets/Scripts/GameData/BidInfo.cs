using System;

namespace GameData
{
    [Serializable]
    public class BidInfo
    {
        public int Ai;//出价玩家0或1
        public int money;//出价金额
        public int turn;//出价时刻是第几回合
    }
}