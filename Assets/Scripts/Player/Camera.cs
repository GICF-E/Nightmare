using UnityEngine;

/// <summary>
/// 用于控制摄像机跟随鼠标转动的类
/// </summary>
public class CameraController : MonoBehaviour
{
    // 鼠标灵敏度
    public float mouseSensitivity = 50f;
    // 手柄状态下的鼠标灵敏度
    private float gamepadMouseSensitivity;
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
        gamepadMouseSensitivity = mouseSensitivity * 1.5f;
    }

    // 每帧更新一次
    void Update()
    {
        // 根据是否使用手柄更新鼠标灵敏度
        if(player.GetComponent<Player>().isGamepad) mouseSensitivity = gamepadMouseSensitivity;
        else mouseSensitivity = gamepadMouseSensitivity / 1.5f;

        // 获取鼠标输入，并根据鼠标灵敏度和时间差来计算旋转量
        float mouseX = player.GetComponent<Player>().playerInput.actions["Mouse X"].ReadValue<float>() * mouseSensitivity * Time.deltaTime;
        float mouseY = player.GetComponent<Player>().playerInput.actions["Mouse Y"].ReadValue<float>() * mouseSensitivity * Time.deltaTime;

        // 计算摄像机在X轴上的旋转角度
        xRotation -= mouseY;
        // 限制摄像机的上下旋转角度，防止过度旋转
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // 确保移动视角是被允许的
        if(player.GetComponent<Player>().canMove){
            // 应用摄像机在X轴的旋转
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            // 旋转玩家的身体，使其跟随鼠标的左右移动
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}
