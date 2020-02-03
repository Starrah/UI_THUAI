using System;
using GameData.MapElement;
using UnityEngine;

public class BuildingControl : AGameObjectControl<Building, BuildingControl.StatusEnum>
{
    public enum StatusEnum
    {
        NORMAL
    }

    public override StatusEnum ModelStatus { get; protected set; } = StatusEnum.NORMAL;
    //暂不存在状态转换，空方法填充
    public override void SetModelStatus(StatusEnum value, Building element, bool noAnimation = true) {}
    public override void SyncMapElementStatus(Building element) {}

}