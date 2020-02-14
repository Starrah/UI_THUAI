using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossScan : Scan {
    // 设置被阻挡的地块，按位设置，被阻挡的地块位设置为1，中心块不可能被阻挡。
    // __7__
    // __3__
    // 62_04
    // __1__
    // __5__
    // 例如侦测器右边的两个地块被阻挡，则Block值设为(1<<0) | (1<<4)
}