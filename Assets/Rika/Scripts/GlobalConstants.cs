using System;
using GameData;
using System.Collections;
using System.Collections.Generic;
using GameData.MapElement;
using UnityEngine;

public class GlobalConstants : MonoBehaviour{
    public Scan[] Scans;

    public Scan GetScan(DeviceRangeTypes rangeType){
        return Scans[(int) rangeType];
    }

    public DetectorControl Detector;
    public ProcessorControl Processor;
    public BuildingControl Building;
    public PollutionControl Pollution;
}