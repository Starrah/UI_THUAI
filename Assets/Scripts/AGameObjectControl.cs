using System;
using GameData.MapElement;
using UnityEngine;

/**
 * 游戏对象控制器的基类，每个该类的派生类，代表一种地图元素（如污染源、检测器）的控制器。
 * 派生类通过实现抽象属性ModelStatus完成状态定义，
 * 实现SetModelStatus方法用于带动画或不带动画的状态转换，
 * 实现SyncMapElementStatus方法，其中调用SetModelStatus来实现根据MapElement直接不带动画的改变状态，
 * 以减少控制器与GameControl中回合切换控制代码的耦合程度。
 *
 * 注：第一阶段开发中，ModelStatus、SyncMapElementStatus由 @Starrah 实现，
 * SetModelStatus由 @Rika 实现。
 */
public abstract class AGameObjectControl<T, T2>: MonoBehaviour
    where T: MapElementBase//地图元素类型
    where T2: Enum//状态对象类型，通常为一个枚举
{
    public abstract T2 ModelStatus { get; protected set; }

    public abstract void SetModelStatus(T2 value, T element, bool noAnimation = true);

    public abstract void SyncMapElementStatus(T element);
}