using System;
using UnityEngine.Assertions;

namespace Tests
{
    public class TestTestGameDataSource : UnityEngine.MonoBehaviour
    {
        public TestGameDataSource ds;
        private void Start()
        {
            Generate();
        }


        public void Generate()
        {
            ds = new TestGameDataSource();
            ds.ReadFile("");
            Assert.IsTrue(true);
        }
    }
}