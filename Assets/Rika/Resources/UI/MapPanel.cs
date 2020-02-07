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

public class MapPanel : MonoBehaviour{
    private GameObject childCanvas;
    private Camera mainCamera;

    private void OnMouseOver(){
        if (!childCanvas.activeInHierarchy)
            childCanvas.SetActive(true);
        var childPanel = childCanvas.transform.Find("Panel").gameObject;
        var rectTransform = childPanel.GetComponent<RectTransform>();
        var size = rectTransform.sizeDelta;
        rectTransform.offsetMin = Input.mousePosition;
        rectTransform.offsetMax = (Vector2) Input.mousePosition + size;
    }

    private void OnMouseExit(){
        if (childCanvas.activeInHierarchy)
            childCanvas.SetActive(false);
    }

    public GameObject canvasPrefab;

    private void Start(){
        Instantiate(canvasPrefab, transform);
        childCanvas = GetComponentInChildren<Canvas>().gameObject;
        childCanvas.SetActive(false);
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        lines = childCanvas.GetComponentsInChildren<HorizontalLayoutGroup>();
        clearPanel();
    }

    private void clearPanel(){
        foreach (var line in lines)
            for (var i = 0; i < line.transform.childCount; i++)
                Destroy(line.transform.GetChild(i));
    }

    private HorizontalLayoutGroup[] lines;

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
                    Instantiate(ImagePrefab, lines[1].transform).color = b ? Color.red : Color.white;
                break;
        }
    }
}