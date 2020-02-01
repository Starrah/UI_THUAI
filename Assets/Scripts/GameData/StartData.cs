using System.Collections.Generic;
using GameData.MapElement;
using UnityEditor.PackageManager.Requests;

namespace GameData
{
    public class StartData
    {
        public int MapWidth;
        public int MapHeight;
        public int LandPrice;
        public int MaxRoundNum;
        public List<int> PollutionComponentProcessPrices;//治理污染成分的治理设备价格
        public List<int> DetectorRangePrices;
        public List<int> ProcessorRangePrices;
        public List<int> PollutionComponentProcessProfits;//某个污染源被彻底治理时，该污染成分带来的收益
        public int TipsterPrice;
        public List<List<MapElementBase>> Map;//初始时的地图
        public List<int> Scores;//初始时的分数
        public List<int> Moneys;//初始时的金钱
    }
}