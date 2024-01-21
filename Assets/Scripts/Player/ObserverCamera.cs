using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ObserverCamera : MonoBehaviour
{
    [Header("属性")]
    [Tooltip("摄像机移动的速度")] public float moveSpeed = 8f;
    [Tooltip("摄像机跑步的速度")] public float runSpeed = 15f;
    [Tooltip("摄像机的Z轴移动速度")] public float flySpeed = 5f;
    [Tooltip("鼠标灵敏度")] public float mouseSensitivity = 200f;
    [Tooltip("摄像机在X轴的旋转角度")] private float xRotation;

    private void Start()
    {
        // 锁定光标
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // 获取用户输入
        float moveHorizontal = Input.GetAxis("Horizontal"); // AD键
        float moveVertical = Input.GetAxis("Vertical"); // WS键
        float moveUpDown = 0;

        // 获取鼠标输入，并根据鼠标灵敏度和时间差来计算旋转量
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 检测是否按下Q或E键来上升或下降
        if (Input.GetKey(KeyCode.Q)) moveUpDown = 1;
        else if (Input.GetKey(KeyCode.E)) moveUpDown = -1;

        // 根据用户输入计算移动量
        Vector3 movement;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            movement = new Vector3(moveHorizontal, moveUpDown, moveVertical) * runSpeed * Time.deltaTime;
        else
            movement = new Vector3(moveHorizontal, moveUpDown, moveVertical) * moveSpeed * Time.deltaTime;

        // 移动摄像机
        transform.parent.Translate(movement);

        // 判断用户QE输入
        if (Input.GetKey(KeyCode.Space))
        {
            // 移动摄像机
            transform.parent.Translate(Vector3.up * flySpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            // 移动摄像机
            transform.parent.Translate(Vector3.down * flySpeed * Time.deltaTime);
        }

        // 计算摄像机在X轴上的旋转角度
        xRotation -= mouseY;
        // 限制摄像机的上下旋转角度，防止过度旋转
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        // 应用摄像机在X轴的旋转（上下看）
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        // 旋转玩家的身体，使其跟随鼠标的左右移动
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
