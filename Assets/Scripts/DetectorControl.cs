using System.Collections.Generic;
using System.Linq;
using GameData.MapElement;
using UnityEngine;

public class DetectorControl : AGameObjectControl<Detector, DetectorControl.StatusEnum> {
    public enum StatusEnum {
        DISABLED, //对象刚实例化时是此状态
        NORMAL //随后会立即变为此状态。in动画应当在DISABLED变为NORMAL状态的SetModelStatus函数调用时开始播放。
    }

    public override StatusEnum ModelStatus { get; protected set; } = StatusEnum.DISABLED;
    private Scan scan;

    /// <summary>
    /// <see cref="Scan.Blocked"/>
    /// </summary>
    public int Blocked {
        get => scan.Blocked;
        set => scan.Blocked = value;
    }

    public override void SetModelStatus(StatusEnum value, Detector element, bool noAnimation = true){
        ModelStatus = value;
        var Animation = GetComponent<Animation>();
        if (!noAnimation)
            Animation.Play("in");
        Animation.PlayQueued("idle", QueueMode.CompleteOthers);
        scan = Instantiate(Rika.Utils.GetScan(element.RangeType), transform);
        // var transform1 = scan.transform;
        // var position = transform.position;
        // position = new Vector3(position.x, .01f, position.z);
        // transform1.position = position;
        var position = transform.position;
        var transform1 = scan.transform;
        transform1.position = new Vector3(position.x, .01f, position.z);
        transform1.localScale *= 1 / .8f;
        scan.cloneMaterial();
        var color = element.Owner == 0 ? Color.red : Color.blue;
        color -= new Color(0, 0, 0, .6f);
        scan.ScanColor = color;
        foreach (var material in transform.GetComponentsInChildren<MeshRenderer>().AsQueryable()
            .Select(r => r.material)
            .Where(m => m.name.Contains("材质")))
            material.color = color;
        GetComponent<MapPanel>().setStatus(element);
        
    }

    public override void SyncMapElementStatus(Detector element){
        if (ModelStatus != StatusEnum.NORMAL) SetModelStatus(StatusEnum.NORMAL, element, true);
    }
}