using System;
using System.Linq;
using GameData.MapElement;
using UnityEngine;

public class ProcessorControl : AGameObjectControl<Processor, ProcessorControl.StatusEnum> {
    public enum StatusEnum {
        DISABLED, //对象刚实例化时是此状态
        NORMAL //随后会立即变为此状态。in动画应当在DISABLED变为NORMAL状态的SetModelStatus函数调用时开始播放。
    }

    public override StatusEnum ModelStatus { get; protected set; } = StatusEnum.DISABLED;
    private Scan scan;

    public int Blocked {
        get => scan.Blocked;
        set => scan.Blocked = value;
    }

    public override void SetModelStatus(StatusEnum value, Processor element, bool noAnimation = true){
        var Animation = GetComponentInChildren<Animation>();
        if (!noAnimation)
            Animation.Play("in");
        Animation.PlayQueued("idle", QueueMode.CompleteOthers);
        scan = Instantiate(Rika.Utils.GetScan(element.RangeType), transform);
        // var transform1 = scan.transform;
        var position = transform.position;
        // position = new Vector3(position.x, .01f, position.z);
        // transform1.position = position;
        scan.transform.position = new Vector3(position.x, .01f, position.z);
        scan.cloneMaterial();
        var color = element.Owner == 0 ? Color.red : Color.blue;
        scan.ScanColor = color;
        foreach (var material in transform.GetComponentsInChildren<MeshRenderer>().AsQueryable()
            .Select(r => r.material)
            .Where(m => m.name.Contains("mat")))
            material.color = color;
    }

    public override void SyncMapElementStatus(Processor element){
        if (ModelStatus != StatusEnum.NORMAL) SetModelStatus(StatusEnum.NORMAL, element, true);
    }
}