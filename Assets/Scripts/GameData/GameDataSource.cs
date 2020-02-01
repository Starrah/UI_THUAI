using System.Collections.Generic;

namespace GameData
{
    public class GameDataSource
    {
        /**
        * 读取播放协议文件（一个符合播放协议的JSON文件），解析为每个回合的状态信息和动作信息，并缓存起来。
        * 该函数允许有比较长的耗时。
        */
        public void ReadFile(string fileName)
        {
            //TODO
        }

        public StartData GetStartData()
        {
            //TODO
            return null;
        }

        public StartData GetTurnData(int turnIndex)
        {
            //TODO
            return null;
        }

        //以下是推荐实现，即在ReadFile中就完整解析播放文件，计算StartData和所有回合的TurnData并混存起来，然后两个Get函数直接返回就可以。
        private StartData _startData;

        private List<TurnData> _turnData;
    }
}