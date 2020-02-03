﻿using GameData.MapElement;
using UnityEngine;

public class PollutionControl : MonoBehaviour, IGameObjectControl<PollutionSource>
{
    public enum PollutionModelStatus
    {
        NOTDETECTED,//未探测 
        DETECTED,//已探测
        PROCESSED//已处理
    }

    public PollutionModelStatus ModelStatus { get; private set; } = PollutionModelStatus.NOTDETECTED;

    public void SetModelStatus(PollutionModelStatus value, bool noAnimation = true)
    {
        
    }
    
    public void SyncMapElementStatus(PollutionSource element)
    {
        
    }
}