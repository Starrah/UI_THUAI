using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

    /*public static MapPanel AttachTo(GameObject gameObject){
        // var panel = gameObject.AddComponent<MapPanel>();
        Instantiate(canvasPrefab, gameObject.transform);
        
        return  panel
    }*/
    public GameObject canvasPrefab;

    private void Start(){
        Instantiate(canvasPrefab, transform);
        childCanvas = GetComponentInChildren<Canvas>().gameObject;
        childCanvas.SetActive(false);
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }
}