using System;
using System.Collections;
using GameData.MapElement;
using UnityEngine;
using System.Linq;

public class PollutionControl : AGameObjectControl<PollutionSource, PollutionControl.StatusEnum> {
    public enum StatusEnum {
        NOTDETECTED, //未探测 
        DETECTED, //已探测
        PROCESSED //已处理
    }

    public override StatusEnum ModelStatus { get; protected set; } = StatusEnum.NOTDETECTED;

    #region 模型相关参数
    public MeshRenderer[] Glows;
    public MeshRenderer Bottom;

    private static readonly Color[] colors = {
        new Color(0, 0, 0, .2f),
        new Color(0, 0, 0, .5f),
        new Color(0, 1, 0, .3f),
    };

    private static readonly int _Color = Shader.PropertyToID("_Color");

    private void Start(){
        #region 初始化模型有关参数
        // var renders = GetComponentsInChildren<MeshRenderer>().AsQueryable();
        // Glows = renders
        // .Where(meshRenderer => !meshRenderer.gameObject.name.Contains("Bottom"))
        // .ToArray();
        // Bottom = renders.Single(meshRenderer => meshRenderer.gameObject.name.Contains("Bottom"));
        #endregion
    }

    /// <summary>
    /// 改变材质的颜色，用于表现污染源的治理情况
    /// </summary>
    /// <param name="tarC">变更的目标颜色</param>
    /// <param name="fadeDelay">渐变时间。0为瞬间完成</param>
    /// <returns></returns>
    IEnumerator changeMaterial(Color tarC, float fadeDelay = 0){
        var delay = 0f;
        Color initC = Glows[0].material.GetColor(_Color);

        var g = new Material(Glows[0].material);
        var b = new Material(Bottom.material);

        foreach (var glow in Glows)
            glow.material = g;
        Bottom.material = b;
        if (fadeDelay > 0)
            while (delay < fadeDelay){
                delay += Time.fixedDeltaTime;
                var nowC = Color.Lerp(initC, tarC, delay / fadeDelay);
                g.SetColor(_Color, nowC);
                b.SetColor(_Color, new Color(nowC.r, nowC.g, nowC.b, nowC.a / 3));
                yield return new WaitForEndOfFrame();
            }
        else{
            g.SetColor(_Color, tarC);
            b.SetColor(_Color, new Color(tarC.r, tarC.g, tarC.b, tarC.a / 3));
        }
    }
    #endregion

    #region 状态改变事件处理
    private PollutionSource _pollutionSource;

    public override void SetModelStatus(StatusEnum value, PollutionSource element, bool noAnimation = true){
        ModelStatus = value;
        StartCoroutine(changeMaterial(colors[(int) value], noAnimation ? 0 : .5f));
        GetComponent<MapPanel>().setStatus(element);
    }
    #endregion

    /**
     * 根据地图信息，无动画的直接改变状态
     */
    public override void SyncMapElementStatus(PollutionSource element){
        StatusEnum status;
        if (element.Curbed != -1) status = StatusEnum.PROCESSED;
        else{
            if (element.Visible[GameControl.Instance.MyAi]) status = StatusEnum.DETECTED;
            else status = StatusEnum.NOTDETECTED;
        }

        SetModelStatus(status, element, true);
    }
}