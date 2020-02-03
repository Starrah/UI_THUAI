﻿using System;
 using GameData.MapElement;
using UnityEngine;

public class PollutionControl : AGameObjectControl<PollutionSource, PollutionControl.PollutionModelStatus>
{
    public enum PollutionModelStatus
    {
        NOTDETECTED,//未探测 
        DETECTED,//已探测
        PROCESSED//已处理
    }

    public override PollutionModelStatus ModelStatus { get; protected set; } = PollutionModelStatus.NOTDETECTED;
        
    public override void SetModelStatus(PollutionModelStatus value, bool noAnimation = true)
    {
        ModelStatus = value;
        //TODO 填充模型显示改变的控制代码
        throw new NotImplementedException();
    }
    
    public override void SyncMapElementStatus(PollutionSource element)
    {
        PollutionModelStatus status;
        //if(element.Curbed !== -1)
    }
}