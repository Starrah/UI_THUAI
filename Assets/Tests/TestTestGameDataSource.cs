using System;
using GameData;
using UnityEngine;
using UnityEngine.Assertions;

namespace Tests {
    public class TestTestGameDataSource : UnityEngine.MonoBehaviour {
        public TestGameDataSource ds;

        private void Start(){
            // Generate();
            generateDirts();
        }

        private void generateDirts(){
            var dirtPrefab = Resources.Load<DirtControl>("Prefabs/Dirt");
            for (int i = 0; i < 25; i++)
                for (int j = 0; j < 25; j++){
                    var dirt = Instantiate(dirtPrefab);
                    dirt.Place = new MapPlace(new Vector2Int(i, j));
                }
        }

        public void Generate(){
            ds = new TestGameDataSource();
            ds.ReadFile("");
            Assert.IsTrue(true);
        }
    }
}