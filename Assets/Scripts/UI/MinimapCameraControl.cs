using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameData;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MinimapCameraControl : MonoBehaviour {
    private CameraControl mainCameraControl;
    private GameObject mainCamera => mainCameraControl.gameObject;
    private Camera _mainCamera;
    public RenderTexture RenderTarget;
    private Rect StageRect => mainCameraControl.StageRect;
    private Camera _camera;
    public RawImage MiniMap;
    public RawImage Frame;
    private RectTransform _rectTransform;
    private const float minimapZoomRatio = 1000f / 256f;

    private
        void Start(){
        _rectTransform = MiniMap.gameObject.GetComponent<RectTransform>();
        mainCameraControl = FindObjectOfType<CameraControl>();
        _mainCamera = mainCameraControl.GetComponent<Camera>();
        _camera = GetComponent<Camera>();
        this.nextTick(initialize);
    }

    public void initialize(){
        transform.position = new Vector3(StageRect.center.x - .5f, 2, StageRect.center.y - .5f);
        _camera.orthographicSize = Mathf.Max(StageRect.width, StageRect.height) / 2;
    }

    private void drawLine(Texture2D tex, Vector2 from, Vector2 to, Color color){
        var rect = new Rect(0, 0, tex.width, tex.height);
        from = new Vector2(tex.width * from.x, tex.height * from.y);
        to = new Vector2(tex.width * to.x, tex.height * to.y);
        var at = from;
        var step = (to - from).normalized;
        while (Vector2.Dot(at - from, at - to) <= 0){
            if (rect.Contains(at))
                tex.SetPixel((int) at.x, (int) at.y, color);
            at += step;
        }
    }

    void Update(){
        //获取相机四个角的射线
        var transform1 = mainCamera.transform;
        var alpha = Mathf.Deg2Rad * _mainCamera.fieldOfView / 2;
        var height = transform1.position.y;
        //targets是相机视图四个角的坐标，地图区域映射到0~1之间
        var gamma = Mathf.Atan(Mathf.Sin(alpha));
        var d = Quaternion.AngleAxis(gamma * Mathf.Rad2Deg, Vector3.forward) * Vector3.down;
        var targets =
            (from i in new[]{0, 1, 2, 3}
                select Quaternion.AngleAxis(i * 90, transform1.forward) * d
                into direction
                let ray = new Ray(transform1.position, direction)
                let distance = height / Mathf.Cos(Mathf.Deg2Rad * Vector3.Angle(Vector3.down, direction))
                select ray.GetPoint(distance)
                into target3
                select (new Vector2(target3.x + .5f, target3.z + .5f)) / Mathf.Max(StageRect.width, StageRect.height)
            ).ToArray();
        var targetTexture = _camera.targetTexture;

        var texture2D = new Texture2D(targetTexture.width, targetTexture.height, targetTexture.graphicsFormat,
            1, TextureCreationFlags.None);

        Frame.texture = texture2D;
        for (int i = 0; i < texture2D.width; i++){
            for (int j = 0; j < texture2D.height; j++){
                texture2D.SetPixel(i, j, new Color(0, 0, 0, 0));
            }
        }

        drawLine(texture2D, targets[0], targets[1], Color.green);
        drawLine(texture2D, targets[1], targets[2], Color.green);
        drawLine(texture2D, targets[2], targets[3], Color.green);
        drawLine(texture2D, targets[3], targets[0], Color.green);
        texture2D.Apply();

        //小地图放大
        if (Input.GetKeyDown(KeyCode.M)){
            MiniMap.gameObject.transform.localScale = new Vector3(minimapZoomRatio, minimapZoomRatio);
            _rectTransform.offsetMin += new Vector2(256, 0);
            _rectTransform.offsetMax += new Vector2(256, 0);
            MiniMap.color = new Color(1, 1, 1, 1);
        }
        else if (Input.GetKeyUp(KeyCode.M)){
            MiniMap.gameObject.transform.localScale = new Vector3(1, 1);
            _rectTransform.offsetMin -= new Vector2(256, 0);
            _rectTransform.offsetMax -= new Vector2(256, 0);
            MiniMap.color = new Color(1, 1, 1, .5f);
        }
    }
}