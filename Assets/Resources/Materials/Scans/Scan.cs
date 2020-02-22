using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scan : MonoBehaviour {
    [SerializeField] Color scanColor = Color.white;

    /// <summary>
    /// 设置地块扫描颜色，包含透明度项
    /// </summary>
    public Color ScanColor {
        get => scanColor;
        set {
            scanColor = value;
            GetComponent<MeshRenderer>().material.SetColor(_Color, value);
        }
    }

    /// <summary>
    /// 设置被阻挡的地块，按位设置，被阻挡的地块位设置为1，中心块不可能被阻挡。
    /// 具体设置方式参考派生类的注释。
    /// <see cref="SquareScan"/>
    /// <see cref="DiagScan"/>
    /// <see cref="CrossScan"/>
    /// </summary>
    public int Blocked {
        get => blocked;
        set {
            value &= (1 << 8) - 1;
            blocked = value;
            GetComponent<MeshRenderer>().material.SetInt(_Blocked, value);
        }
    }

    private int blocked;
    private static readonly int _Blocked = Shader.PropertyToID("_Blocked");
    private static readonly int _Color = Shader.PropertyToID("_Color");

    public void cloneMaterial(){
        GetComponent<MeshRenderer>().material = new Material(GetComponent<MeshRenderer>().material);
    }
}