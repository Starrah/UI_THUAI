namespace GameData.GameEvents
{
    public class BidResultEvent: GameEventBase
    {
        public bool Success;//true表示拍卖成功，false表示失败（因竞拍方余额不足而流拍）
    }
}