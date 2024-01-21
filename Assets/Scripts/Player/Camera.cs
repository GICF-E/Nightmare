using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 用于控制摄像机跟随鼠标转动的类
/// </summary>
public class CameraController : MonoBehaviour
{
    // 鼠标灵敏度
    public float mouseSensitivity = 200f;
    // 玩家的身体部分，用于旋转
    public Transform playerBody;
    public GameObject player;
    // 摄像机在X轴的旋转角度
    float xRotation = 0f;

    // 游戏开始时执行
    void Start()
    {
        // 锁定光标
        Cursor.lockState = CursorLockMode.Locked;
    }

    // 每帧更新一次
    void Update()
    {
        // 获取鼠标输入，并根据鼠标灵敏度和时间差来计算旋转量
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 计算摄像机在X轴上的旋转角度
        xRotation -= mouseY;
        // 限制摄像机的上下旋转角度，防止过度旋转
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // 应用摄像机在X轴的旋转
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        // 旋转玩家的身体，使其跟随鼠标的左右移动
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
