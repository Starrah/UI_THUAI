using System;
using GameData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalConstants : MonoBehaviour{
    public Scan[] Scans;

    public Scan GetScan(DeviceRangeTypes rangeType){
        return Scans[(int) rangeType];
    }

    public Drone Drone;
    public Equip Equip;
    public Forbid Forbid;
    public Polluted Polluted;

    private void Start(){
        StartCoroutine(this.nextTick(() => { Equip.Create(new Vector2(), DeviceRangeTypes.DIAGON); }));
    }
}