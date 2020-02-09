using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareScan : Scan{
    // 设置被阻挡的地块，按位设置，被阻挡的地块位设置为1，中心块不可能被阻挡。
    // 7,6,5
    // 4,x,0
    // 3,2,1
    // 例如侦测器右边和下面的地块被阻挡，则Block值设为(1<<0) | (1<<2)=5
}