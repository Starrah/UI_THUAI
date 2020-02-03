﻿using System;
 using GameData.MapElement;
using UnityEngine;

public class PollutionControl : AGameObjectControl<PollutionSource, PollutionControl.StatusEnum>
{
    public enum StatusEnum
    {
        NOTDETECTED,//未探测 
        DETECTED,//已探测
        PROCESSED//已处理
    }

    public override StatusEnum ModelStatus { get; protected set; } = StatusEnum.NOTDETECTED;
        
    public override void SetModelStatus(StatusEnum value, PollutionSource element, bool noAnimation = true)
    {
        ModelStatus = value;
        //TODO 填充模型显示改变的控制代码
        throw new NotImplementedException();
    }
    
    /**
     * 根据地图信息，无动画的直接改变状态
     */
    public override void SyncMapElementStatus(PollutionSource element)
    {
        StatusEnum status;
        if (element.Curbed != -1) status = StatusEnum.PROCESSED;
        else
        {
            if (element.Visible[GameControl.Instance.myAi]) status = StatusEnum.DETECTED;
            else status = StatusEnum.NOTDETECTED;
        }
        if(status != ModelStatus) SetModelStatus(status, element, true);
    }
}