using System;
using System.Collections.Generic;
using GameData.MapElement;

namespace GameData.GameEvents
{
    [Serializable]
    public class PutProcessorEvent: GameEventBase
    {
        public Processor Processor;
        /**
         * 成功治理的所有污染源。若没有成功治理的污染源，就是空List。
         * 否则，每个元素是一个pair：第一个是污染源对象，第二个是彻底治理该污染源得到的收益的数值。
         */
        public List<Tuple<PollutionSource, int>> Result = new List<Tuple<PollutionSource, int>>();
    }
}