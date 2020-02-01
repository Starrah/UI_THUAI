using System.Collections.Generic;
using GameData.MapElement;

namespace GameData.GameEvents
{
    public class PutDetectorEvent: GameEventBase
    {
        public Detector Detector;
        public List<PollutionSource> Result = new List<PollutionSource>();//找到的所有污染源。若啥也没找到。就是空List。
    }
}