using System.Collections;
using System.Collections.Generic;
using GameData;
using UnityEngine;

public class Drone : BaseEquip{
    public static Drone Create(Vector2 position, DeviceRangeTypes rangeType){
        var ret = Instantiate(FindObjectOfType<GlobalConstants>().Drone,
            new Vector3(position.x, 0, position.y),
            new Quaternion());
        ret.Settle(rangeType);
        return ret;
    }
}