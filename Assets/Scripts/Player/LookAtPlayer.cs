using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    [Tooltip("摄像机的位置")] private Transform cameraTransform;
    [Tooltip("是否在X轴增加")] public bool isAddX;
    [Tooltip("是否在Y轴增加")] public bool isAddY;
    [Tooltip("是否在Z轴增加")] public bool isAddZ;
    [Tooltip("默认位置X")] public float defaultPosition;
    [Tooltip("自身的RectTransform组件")] private RectTransform rectTransform;
    [Tooltip("切换阈值")] private float threshold = 0.1f;

    private void Start() {
        // 只在Start时查找，减少性能开销
        cameraTransform = GameObject.Find("Player/Main Camera/").transform;
    }

    private void Update()
    {
        // 始终朝向摄像机
        transform.LookAt(cameraTransform, transform.position);
        float zAngle = transform.parent.eulerAngles.z;
        // 根据朝向判断UI显示位置
        if(zAngle > 0 && zAngle < 180) {
            // 如果物体朝上，使用正高度
            if(isAddX) transform.localPosition = new Vector3(defaultPosition, 0, 0);
            else if(isAddY) transform.localPosition = new Vector3(0, defaultPosition, 0);
            else if(isAddZ) transform.localPosition = new Vector3(0, 0, -defaultPosition);
        } else {
            // 如果物体朝下，使用负高度\
            if(isAddX) transform.localPosition = new Vector3(-defaultPosition, 0, 0);
            else if(isAddY) transform.localPosition = new Vector3(0, -defaultPosition, 0);
            else if(isAddZ) transform.localPosition = new Vector3(0, 0, defaultPosition);
        }
    }
}