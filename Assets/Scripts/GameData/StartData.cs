using System;

namespace GameData
{
    [Serializable]
    public class StartData
    {
        public int MapWidth;
        public int MapHeight;
        public int LandPrice;
        public int MaxRoundNum;
        public int ActualRoundNum;
        public int[] PollutionComponentProcessPrices;//治理污染成分的治理设备价格
        public int[] DetectorRangePrices;//不同种类检测器的价格
        public int[] ProcessorRangePrices;//不同种类治理器的价格
        public int[] PollutionComponentProcessProfits;//某个污染源被彻底治理时，该污染成分带来的收益
        public int TipsterPrice;
        public MapPlace[][] Map;//初始时的地图
        public int[] Scores;//初始时的分数
        public int[] Moneys;//初始时的金钱
    }
}