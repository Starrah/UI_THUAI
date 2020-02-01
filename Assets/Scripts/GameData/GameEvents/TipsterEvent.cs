using UnityEngine;

namespace GameData.GameEvents
{
    public class TipsterEvent: GameEventBase
    {
        public bool Success;//是否成功找到了
        public Vector2? Result = null;//找到的点的坐标。若Success为false则必定为null。
    }
}