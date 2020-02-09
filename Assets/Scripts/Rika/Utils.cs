using System;
using GameData;
using System.Collections;
using System.Collections.Generic;
using GameData.MapElement;
using UnityEngine;

namespace Rika{
    public static class Utils{
        public static Scan GetScan(DeviceRangeTypes rangeType){
            switch (rangeType){
                case DeviceRangeTypes.STRAIGHT:
                    return Resources.Load<CrossScan>("Materials/Scans/CrossScan/CrossScan");
                case DeviceRangeTypes.SQUARE:
                    return Resources.Load<SquareScan>("Materials/Scans/SquareScan/SquareScan");
                case DeviceRangeTypes.DIAGON:
                    return Resources.Load<DiagScan>("Materials/Scans/DiagScan/DiagScan");
                default:
                    throw new ArgumentOutOfRangeException(nameof(rangeType), rangeType, null);
            }
        }

        /// <summary>
        /// 调用该方法获取地图元素的prefab
        /// </summary>
        /// <typeparam name="TPrefab"></typeparam>
        /// <typeparam name="TMapElementBase"></typeparam>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static TPrefab GetMapElementPrefab<TPrefab, TMapElementBase, TEnum>()
            where TPrefab : AGameObjectControl<TMapElementBase, TEnum>
            where TMapElementBase : MapElementBase
            where TEnum : Enum{
            switch (typeof(TPrefab)){
                case var __ when __ == typeof(BuildingControl):
                    return Resources.Load<TPrefab>("Prefabs/Building");
                case var __ when __ == typeof(DetectorControl):
                    return Resources.Load<TPrefab>("Prefabs/Detector");
                case var __ when __ == typeof(ProcessorControl):
                    return Resources.Load<TPrefab>("Prefabs/Processor");
                case var __ when __ == typeof(PollutionControl):
                    return Resources.Load<TPrefab>("Prefabs/Pollution");
            }

            return null;
        }
    }
}