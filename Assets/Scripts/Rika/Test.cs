using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using GameData;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = System.Numerics.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Test : MonoBehaviour {
    // Start is called before the first frame update
    void Start(){
        var prefab = Resources.Load<DirtControl>("Materials/Dirt/Dirt");
        for (int i = 0; i < 10; i++){
            for (int j = 0; j < 10; j++){
                var d = Instantiate(prefab, new Vector3(i, 0, j), new Quaternion());
                d.Place = new MapPlace(new Vector2(i, j));
            }
        }

        this.nextTick(() => {
            FindObjectsOfType<DirtControl>().Single(control => control.transform.position.magnitude < .1f).Place =
                new MapPlace(Vector2.Zero){
                    Type = MapPlaceTypes.BOUGHT,
                    Bid = new BidInfo(){
                        turn = 2, Ai = 1
                    },
                    Owner = 1,
                };
        });
    }
}