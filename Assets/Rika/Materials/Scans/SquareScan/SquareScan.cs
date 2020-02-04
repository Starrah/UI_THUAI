using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareScan : MonoBehaviour {
    private Color scanColor = Color.white;
    Material material;
    /// <summary>
    /// 设置地块扫描颜色，包含透明度项
    /// </summary>
    public Color ScanColor {
        get => scanColor;
        set {
            scanColor = value;
            material.SetColor("_Color", value);
        }
    }
    /// <summary>
    /// 设置被阻挡的地块，按位设置，被阻挡的地块位设置为1，中心块不可能被阻挡。
    /// 7,6,5
    /// 4,x,0
    /// 3,2,1
    /// 例如侦测器右边和下面的地块被阻挡，则Block值设为(1<<0) | (1<<2)=5
    /// </summary>
    public int Blocked {
        get => blocked;
        set {
            value &= (1 << 8) - 1;
            blocked = value;
            material.SetInt("_Blocked", value);
        }
    }

    private int blocked;

    // Start is called before the first frame update
    void Start() {
        material = new Material(GetComponent<MeshRenderer>().material);
        GetComponent<MeshRenderer>().material = material;
    }

    // Update is called once per frame
    void Update() {

    }
}
