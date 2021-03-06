﻿using System;
using GameData;
using GameData.MapElement;
using UnityEngine;
using UnityEngine.Assertions;

namespace Tests {
    public class TestTestGameDataSource : UnityEngine.MonoBehaviour {
        public TestGameDataSource ds;
        private void Start()
        {
            Generate();
            Data();
            CopyFeature();
            Other();
            Debug.Log("Test TestGameDataSource passed");
            ExampleData();
            Debug.Log("Test GameDataSource passed");
        }

        public void ExampleData()
        {
            var dt = new GameDataSource();
            dt.ReadFile("播放文件示例.json");
        }
        
        public void Generate()
        {
            ds = new TestGameDataSource();
            ds.ReadFile("");
            Assert.IsTrue(true);
        }

        public void Data()
        {
            Assert.IsTrue(ds.GetStartData().Scores[0] == 0);
            Assert.IsNotNull(ds.GetTurnData(0));
            try
            {
                ds.GetTurnData(1);
                Assert.IsTrue(false);
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
            }
            Assert.IsTrue(ds.GetTurnData(0).Events.Count == 2);
        }

        public void CopyFeature()
        {
            Assert.IsTrue(ds.GetTurnData(0).Map != ds.GetStartData().Map);
            Assert.IsTrue(ds.GetTurnData(0).Moneys != ds.GetStartData().Moneys);
            Assert.IsTrue(ds.GetStartData().Map[2][2].Elements.Count == 0);
            Assert.IsTrue(ds.GetTurnData(0).Map[2][2].Elements.Count == 1);
            Assert.IsTrue(ds.GetStartData().Map[0][2].GetElement<PollutionSource>().Curbed == -1);
            Assert.IsTrue(ds.GetTurnData(0).Map[0][2].GetElement<PollutionSource>().Curbed == 0);
            Assert.IsNull(ds.GetStartData().Map[1][0].Bid);
            Assert.IsNotNull(ds.GetTurnData(0).Map[1][0].Bid);
        }

        public void Other()
        {
            object[][] qwq = new object[][]{new object[]{1,2}, new object[]{3,4}};
            Assert.IsTrue(((int)qwq[0][0]) == 1);
        }
    }
}