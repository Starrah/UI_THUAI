# Models.md
该文件说明现有的一些模型的状态，用于编制相关游戏对象(Prefab)的接口标准。

**注意：该文件的描述内容不作为任何标准，仅供编制标准参考。**

## Drone 检测器
目前位于`Asstes/Rika/Prefabs/Drone.prefab`
* 动画
    * `in` 进入动画，对象被部署时播放。
    * `idle` 闲置动画，对象部署后循环播放。

## Forbid 高大建筑物
目前位于`Asstes/Rika/Prefabs/Forbid.prefab`

仅有默认状态。

## Polluted 污染源
目前位于`Asstes/Rika/Prefabs/Polluted.prefab`

* 状态
    * 不可见
    * 未治理
    * 已治理

关于加入**部分治理**状态的讨论正在进行中。
