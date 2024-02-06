using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// 用于控制摄像机跟随鼠标转动的类
/// </summary>
public class CameraController : MonoBehaviour
{
    // 鼠标灵敏度
    public float mouseSensitivity = 40f;
    // 玩家的身体部分，用于旋转
    public Transform playerBody;
    public GameObject player;
    // 摄像机在X轴的旋转角度
    float xRotation = 0f;
    // 输入系统
    private PlayerInput playerInput;

    // 游戏开始时执行
    void Start()
    {
        // 锁定光标
        if(!player.GetComponent<Player>().isMenuMode) Cursor.lockState = CursorLockMode.Locked;
        // 初始化输入系统
        playerInput = GetComponent<PlayerInput>();
    }

    // 每帧更新一次
    void Update()
    {
        // 获取鼠标输入，并根据鼠标灵敏度和时间差来计算旋转量
        float mouseX = playerInput.actions["Mouse X"].ReadValue<float>() * mouseSensitivity * Time.deltaTime;
        float mouseY = playerInput.actions["Mouse Y"].ReadValue<float>() * mouseSensitivity * Time.deltaTime;
        // 获取手柄输入，针对手柄使用不同的灵敏度
        mouseX += playerInput.actions["Right Stick X"].ReadValue<float>() * mouseSensitivity * 1.5f * Time.deltaTime;
        mouseY += playerInput.actions["Right Stick Y"].ReadValue<float>() * mouseSensitivity * 1.5f * Time.deltaTime;

        // 计算摄像机在X轴上的旋转角度
        xRotation -= mouseY;
        // 限制摄像机的上下旋转角度，防止过度旋转
        if(!player.GetComponent<Player>().isRowing) xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // 确保移动视角是被允许的
        if(player.GetComponent<Player>().canMove && !player.GetComponent<Player>().isRowing){
            // 应用摄像机在X轴的旋转
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            // 旋转玩家的身体，使其跟随鼠标的左右移动
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}
