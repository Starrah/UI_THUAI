using System;
using System.Collections;
using System.Collections.Generic;
using GameData;
using UnityEngine;

public class CameraControl : MonoBehaviour {
    /// <summary>
    /// 舞台区域
    /// </summary>
    public Rect StageRect;

    private const float TOLERANCE = .001f;
    private (float, float) distanceRange => (5, Mathf.Max(StageRect.width, StageRect.height));

    private const float moveVelocity = 15f;
    private const float zoomVelocity = .4f;
    private Camera _camera;

    private Vector2 checkEdge(Vector2 direction){
        var transform1 = transform;
        var position = transform1.position;
        var forward = transform1.forward;
        var ray = new Ray(position, forward);
        var height = position.y;
        var angle = Vector3.Angle(Vector3.down, forward) / 180 * Mathf.PI;
        var target3 = ray.GetPoint(height / Mathf.Cos(angle));
        var target = new Vector2(target3.x, target3.z);
        var ret = direction;
        if (target.x < StageRect.xMin && direction.x < 0 ||
            target.x > StageRect.xMax && direction.x > 0) ret.x = 0;
        if (target.y < StageRect.yMin && direction.y < 0 ||
            target.y > StageRect.yMax && direction.y > 0) ret.y = 0;
        return ret;
    }

    private void Zoom(float direction){
        var transform1 = transform;
        var forward = transform1.forward;
        var height = transform1.position.y;
        var angle = Vector3.Angle(Vector3.down, forward) / 180 * Mathf.PI;
        var distance = height / Mathf.Cos(angle);
        if (distance < distanceRange.Item1 && direction > 0) return;
        if (distance > distanceRange.Item2 && direction < 0) return;
        transform.position += transform1.forward * (direction * zoomVelocity);
    }

    private void Move(Vector2 direction){
        direction = checkEdge(direction);
        transform.position += new Vector3(direction.x, 0, direction.y) * (moveVelocity * Time.deltaTime);
    }

    private void Update(){
        //处理相机的移动
        var direction = Vector2.zero;
        var zoom = 0f;
        if (Input.GetKey(KeyCode.W))
            direction += Vector2.up;
        if (Input.GetKey(KeyCode.A))
            direction += Vector2.left;
        if (Input.GetKey(KeyCode.S))
            direction += Vector2.down;
        if (Input.GetKey(KeyCode.D))
            direction += Vector2.right;
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) ||
            Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand) || Input.GetKey(KeyCode.Z))
            zoom += 1;
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.X))
            zoom -= 1;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
            direction /= 3;
            zoom /= 3;
        }

        if (direction.magnitude > 0)
            Move(direction);
        if (Math.Abs(zoom) > TOLERANCE)
            Zoom(zoom);
    }

    private void Start(){
        _camera = GetComponent<Camera>();
        var startData = GameControl.Instance.DataSource.GetStartData();
        StageRect = new Rect(0, 0, startData.MapWidth, startData.MapHeight);
    }
}