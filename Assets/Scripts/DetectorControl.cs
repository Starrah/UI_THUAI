using GameData.MapElement;
using UnityEngine;

public class DetectorControl : AGameObjectControl<Detector, DetectorControl.StatusEnum> {
    public enum StatusEnum {
        DISABLED, //对象刚实例化时是此状态
        NORMAL //随后会立即变为此状态。in动画应当在DISABLED变为NORMAL状态的SetModelStatus函数调用时开始播放。
    }

    public override StatusEnum ModelStatus { get; protected set; } = StatusEnum.DISABLED;
    private Scan scan;

    public override void SetModelStatus(StatusEnum value, Detector element, bool noAnimation = true){
        var Animation = GetComponent<Animation>();
        if (!noAnimation)
            Animation.Play("in");
        Animation.PlayQueued("idle", QueueMode.CompleteOthers);
        scan = Instantiate(Rika.Utils.GetScan(element.RangeType), transform);
        // var transform1 = scan.transform;
        // var position = transform.position;
        // position = new Vector3(position.x, .01f, position.z);
        // transform1.position = position;
        scan.transform.localPosition = new Vector3(0, -.99f, 0);
        scan.cloneMaterial();
    }

    public override void SyncMapElementStatus(Detector element){
        if (ModelStatus != StatusEnum.NORMAL) SetModelStatus(StatusEnum.NORMAL, element, true);
    }
}