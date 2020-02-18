using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GameData;
using GameData.MapElement;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class MapPanel : MonoBehaviour {
    private Canvas childCanvas;
    private Camera mainCamera;
    private static readonly Color[] _colors = new[]{new Color(1, .2f, .2f), new Color(.2f, .2f, 1)};

    private void OnMouseOver(){
        if (!childCanvas.enabled)
            childCanvas.enabled = true;
        var childPanel = childCanvas.transform.Find("Panel").gameObject;
        var rectTransform = childPanel.GetComponent<RectTransform>();
        var size = rectTransform.sizeDelta;
        rectTransform.offsetMin = Input.mousePosition;
        rectTransform.offsetMax = (Vector2) Input.mousePosition + size;
    }

    private void OnMouseExit(){
        if (childCanvas.enabled)
            childCanvas.enabled = false;
    }

    private void Start(){
        /*if (canvasPrefab is null)
            canvasPrefab = Resources.Load<GameObject>("UI/MapPanelCanvas");
        Instantiate(canvasPrefab, transform);*/
        childCanvas = GetComponentInChildren<Canvas>();
        childCanvas.enabled = false;
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        // lines = childCanvas.GetComponentsInChildren<HorizontalLayoutGroup>();
        // clearPanel();
    }

    private void clearPanel(){
        foreach (var line in lines)
            for (var i = 0; i < line.transform.childCount; i++)
                Destroy(line.transform.GetChild(i).gameObject);
    }

    public HorizontalLayoutGroup[] lines;

    public void setStatus<T>(T value) where T : MapElementBase{
        clearPanel();
        var ImagePrefab = Resources.Load<Image>("UI/Image");
        var RawImagePrefab = Resources.Load<RawImage>("UI/RawImage");
        switch (value){
            case PollutionSource v:
                Instantiate(RawImagePrefab, lines[0].transform).texture = Resources.Load<Texture2D>(v.Visible[0]
                    ? "Icons/Visible/Visible_Red"
                    : "Icons/Visible/Invisible_Red");
                Instantiate(RawImagePrefab, lines[0].transform).texture = Resources.Load<Texture2D>(v.Visible[1]
                    ? "Icons/Visible/Visible_Blue"
                    : "Icons/Visible/Invisible_Blue");
                foreach (var b in v.Components)
                    Instantiate(ImagePrefab, lines[1].transform).color = b ? Color.yellow : Color.white;
                break;
            case Detector v:
                Texture2D tex;
                switch (v.RangeType){
                    case DeviceRangeTypes.STRAIGHT:
                        tex = Resources.Load<Texture2D>("Icons/RangeType/plus");
                        break;
                    case DeviceRangeTypes.SQUARE:
                        tex = Resources.Load<Texture2D>("Icons/RangeType/square");
                        break;
                    case DeviceRangeTypes.DIAGON:
                        tex = Resources.Load<Texture2D>("Icons/RangeType/cross");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var im = Instantiate(RawImagePrefab, lines[0].transform).GetComponent<RawImage>();
                im.texture = tex;
                im.color = _colors[v.Owner];
                break;
            case Processor v:
                switch (v.RangeType){
                    case DeviceRangeTypes.STRAIGHT:
                        tex = Resources.Load<Texture2D>("Icons/RangeType/plus");
                        break;
                    case DeviceRangeTypes.SQUARE:
                        tex = Resources.Load<Texture2D>("Icons/RangeType/square");
                        break;
                    case DeviceRangeTypes.DIAGON:
                        tex = Resources.Load<Texture2D>("Icons/RangeType/cross");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                for (int i = 0; i < 3; i++){
                    Instantiate(ImagePrefab, lines[1].transform).color =
                        i==v.PollutionComponentIndex ? Color.green : Color.white;
                }

                im = Instantiate(RawImagePrefab, lines[0].transform).GetComponent<RawImage>();
                im.texture = tex;
                im.color = _colors[v.Owner];
                break;
        }
    }
}