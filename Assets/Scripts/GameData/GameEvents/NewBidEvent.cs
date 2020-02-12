using System;

namespace GameData.GameEvents
{
    [Serializable]
    public class NewBidEvent: GameEventBase
    {
        public BidInfo Bid;
    }
}