using GameData.MapElement;

public class ProcessorControl : AGameObjectControl<Processor, ProcessorControl.StatusEnum>
{
    public enum StatusEnum
    {
        DISABLED, //对象刚实例化时是此状态
        NORMAL //随后会立即变为此状态。in动画应当在DISABLED变为NORMAL状态的SetModelStatus函数调用时开始播放。
    }

    public override StatusEnum ModelStatus { get; protected set; } = StatusEnum.DISABLED;

    public override void SetModelStatus(StatusEnum value, Processor element, bool noAnimation = true)
    {
        //TODO noAnimation=false时播放in动画，quque idle动画，否则直接循环播放idle动画
        throw new System.NotImplementedException();
    }

    public override void SyncMapElementStatus(Processor element)
    {
        if (ModelStatus != StatusEnum.NORMAL) SetModelStatus(StatusEnum.NORMAL, element, true);
    }
}