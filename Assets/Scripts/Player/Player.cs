using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 用于控制玩家行动的类
/// </summary>
public class Player : MonoBehaviour
{
    [Header("玩家属性")]
    [Tooltip("正常行走速度")] public float walkSpeed = 6f;
    [Tooltip("奔跑速度")] public float runSpeed = 10f;
    [Tooltip("跳跃速度")] public float jumpSpeed = 4.0f;
    [Tooltip("重力加速度")] public float gravity = 9.8f;
    [Tooltip("推动物体力的大小")] public float pushPower = 4f;
    [Tooltip("角色移动向量")] public Vector3 moveDirection = Vector3.zero;


    [Header("玩家组件引用")]
    [Tooltip("角色控制器组件")] public CharacterController controller;
    [Tooltip("移动音源")] [SerializeField] public AudioSource movementAudioSource;
    [Tooltip("辅助音源")] [SerializeField] public AudioSource auxiliaryAudioSource;
    [Tooltip("摄像机的位置")] public Transform cemara;
    [Tooltip("对回血协程的引用")] private Coroutine addHealthCoroutine;


    [Header("玩家的下蹲属性")]
    [Tooltip("角色控制器的原始高度")] public float standHeight = 2f;
    [Tooltip("蹲下时的高度")] public float crouchHeight = 1f;
    [Tooltip("蹲下时的移动速度")] public float crouchSpeed = 3.0f;
    [Tooltip("玩家可以下蹲的图层")] public LayerMask crouchLayerMask;
    [Tooltip("玩家不可以下蹲的图层")] public LayerMask noCrouchLayerMask;


    [Header("修复在斜坡上移动的顿挫感")]
    [Tooltip("玩家走斜坡时施加的力度")] private float slopeForce = 8.0f;
    [Tooltip("斜坡射线长度")] private float slopeRayLength = 2.5f;

    
    [Header("玩家发出的音效")]
    [Tooltip("行走音效")] public AudioClip walkSound;
    [Tooltip("奔跑音效")] public AudioClip runSound;
    [Tooltip("在水中的移动音效")] public AudioClip waterSound;
    


    [Header("玩家状态")]
    [Tooltip("是否在奔跑")] public bool isRunning;
    [Tooltip("是否在跳跃")] public bool isJumping;
    [Tooltip("是否正在蹲下")] public bool isCrouching = false;
    [Tooltip("玩家是否可以下蹲")] public bool canCrouching = false;
    [Tooltip("人物状态的枚举")] public enum MovementState { walking, running, idle};
    [Tooltip("角色移动状态")] public MovementState state;
    [Tooltip("实际移动速度")] public float currentSpeed;

    


    [Header("玩家生命")]
    [Tooltip("玩家的生命值")] public float playerHealth = 100f;
    [Tooltip("判断玩家是否死亡")] public bool isDead;
    [Tooltip("判断玩家是否受到伤害")] public bool isDamage;


    [Header("跌落伤害的判定")]
    [Tooltip("安全跌落距离")] public float safeFallDistance = 3.0F;
    [Tooltip("开始下落时的高度")] private float fallStartLevel;
    [Tooltip("是否正在下落")] private bool isFalling;
    


    [Header("特殊模式")]
    [Tooltip("索尼克模式")] public bool isSonicMode;
    [Tooltip("观察者模式")] public bool isObserverMode;
    [Tooltip("观察者")] public GameObject Observer;


    [Header("UI")]
    [Tooltip("是否显示FPS")] public bool isDisplayFPS;
    [Tooltip("是否显示血量百分比")] public bool isDisplayHealthFigure;
    [Tooltip("玩家血量UI")] public TextMeshProUGUI playerHealthUI;
    [Tooltip("玩家血量提示灯")] public Image healthImage;
    [Tooltip("玩家血量提示灯的颜色")] public Color[] healthImageColor; 
    [Tooltip("玩家血雾效果")] public Image hurtImage;
    [Tooltip("血雾收到伤害颜色")] private Color flashColor;
    [Tooltip("血雾没有受到伤害的颜色")] private Color clearColor;

    void Start()
    {
        // 获取角色控制器组件并存储原始高度
        controller = GetComponent<CharacterController>();
        standHeight = controller.height;
        // 重置玩家生命值
        playerHealth = 100f;
        // 重置玩家速度
        if (!isSonicMode)
        {
            walkSpeed = 6;
            runSpeed = 10f;
            jumpSpeed = 4f;
            crouchSpeed = 3f;
        }
        else
        {
            walkSpeed = 8f;
            runSpeed = 40f;
            jumpSpeed = 20f;
            crouchSpeed = 20f;
        }
        // 判断观察者模式
        if (isObserverMode)
        {
            // 生成观察者
            Instantiate(Observer, transform.position, Quaternion.identity);
            // 关闭玩家
            gameObject.SetActive(false);
        }
        // 更新UI
        if(isDisplayHealthFigure){
            playerHealthUI.gameObject.SetActive(true);
            healthImage.gameObject.SetActive(false);
        }
        else{
            playerHealthUI.gameObject.SetActive(false);
            healthImage.gameObject.SetActive(true);
        }
        // 计算颜色索引
        int colorIndex = Mathf.Clamp((int)(playerHealth * (healthImageColor.Length - 1) / 100f), 0, healthImageColor.Length - 1);
        // 根据血量百分比更新血量提示灯的颜色
        healthImage.color = healthImageColor[colorIndex];
        // 更新血量百分比
        playerHealthUI.text = playerHealth + "%";
        // 更新血雾颜色
        flashColor = Color.red;
        clearColor = Color.clear;
        // 开关帧率显示
        if (isDisplayFPS) GetComponent<FPSDisplay>().enabled = true;
        else GetComponent<FPSDisplay>().enabled = false;
    }

    void Update()
    {
        // 当角色站在地面上时执行以下逻辑
        if (controller.isGrounded)
        {
            // 根据玩家输入计算移动方向
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);

            // 检查玩家是否正在奔跑或跳跃
            isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            isJumping = Input.GetButton("Jump");

            // 判断玩家是否可以下蹲
            CanCrouch();
            // 执行下蹲逻辑
            Crouch();

            // 根据是否蹲下、奔跑或正常行走来设置移动速度
            currentSpeed = isCrouching ? crouchSpeed : (isRunning ? runSpeed : walkSpeed);
            moveDirection *= currentSpeed;
            
            // 如果玩家按下跳跃键，则设置跳跃速度
            if (isJumping && !isCrouching)
            {
                moveDirection.y = jumpSpeed;
            }

            // 根据当前的玩家的速度更改枚举值
            if (moveDirection.x == 0 && moveDirection.z == 0) state = MovementState.idle;
            else if (currentSpeed == walkSpeed) state = MovementState.walking;
            else if (currentSpeed == runSpeed) state = MovementState.running;
        }
        else
        {
            // 应用重力效果
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // 玩家受到伤害后屏幕产生红色渐变
        if (isDamage)
        {
            // 受到伤害时瞬间切换到红色
            hurtImage.color = flashColor;
        }
        else {
            // 没有受到伤害时缓慢渐变到白色
            hurtImage.color = Color.Lerp(hurtImage.color, clearColor, Time.deltaTime * 3);
        }
        // 重置受到伤害的变量
        isDamage = false;


        // 如果处在斜坡上移动
        if (OnSlope())
        {
            //给玩家施加一个向下的力
            controller.Move(Vector3.down * controller.height / 2 * slopeForce * Time.deltaTime);
        }

        if (!isSonicMode)
        {
            // 使用射线检测来处理跌落伤害
            if (controller.isGrounded && isFalling)
            {
                // 当玩家再次着陆时，计算跌落距离并应用伤害
                isFalling = false;
                float fallDistance = fallStartLevel - transform.position.y;
                if (fallDistance > safeFallDistance)
                {
                    float fallDamage = (fallDistance - safeFallDistance) * 1;
                    PlayerDamageHealth(fallDamage);
                }
            }
            else if (!controller.isGrounded && !isFalling)
            {
                // 当玩家离开地面时，开始跟踪下落
                isFalling = true;
                fallStartLevel = transform.position.y;
            }
        }

        // 移动角色控制器
        controller.Move(moveDirection * Time.deltaTime);
        // 播放声音
        PlayerSound();
    }

    // 判断玩家是否可以下蹲
    public void CanCrouch()
    {
        // 获取玩家头顶的坐标
        Vector3 headSphereLocation = transform.position + new Vector3(0, 0.5f, 0) * standHeight;
        // 获取玩家头顶的坐标
        Vector3 footSphereLocation = transform.position - new Vector3(0, 0.5f, 0) * standHeight;
        // 判断sphereLocation半境内是否有物体
        canCrouching = Physics.OverlapSphere(headSphereLocation, 0.2f, crouchLayerMask).Length == 0 && Physics.OverlapSphere(footSphereLocation, 0.1f, noCrouchLayerMask).Length == 0;
    }

    // 玩家下蹲的逻辑
    public void Crouch()
    {
        // 判断是否站在水里
        if(Physics.OverlapSphere(transform.position - new Vector3(0, 0.5f, 0) * standHeight, 0.1f, noCrouchLayerMask).Length != 0)
        {
            // 强制下蹲
            controller.height = crouchHeight;
            controller.center = controller.height / 2f * Vector3.up;
            isCrouching = true;
            // 退出程序
            return;
        }
        // 判断是否可以下蹲或有没有按下对应按键
        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && canCrouching && !isCrouching)
        {
            // 根据是否可以下蹲改变高度和中心
            StartCoroutine(ChangeHeightTo(crouchHeight));
            StartCoroutine(ChangeCenterTo(0.5f));
            isCrouching = true;
        }else if(!(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && canCrouching && isCrouching)
        {
            // 根据是否可以下蹲改变高度和中心
            StartCoroutine(ChangeHeightTo(standHeight));
            StartCoroutine(ChangeCenterTo(0));
            isCrouching = false;
        }
    }

    // 判断玩家是否在斜坡上
    public bool OnSlope()
    {
        if (isJumping) return false;
        // 被射线击中物体的信息
        RaycastHit hit;
        // 向下打出射线
        if (Physics.Raycast(transform.position, Vector3.down, out hit, controller.height / 2 * slopeRayLength))
        {
            // 判断被射物体法线是否垂直
            if (hit.normal != Vector3.up) return true;
        }
        return false;
    }

    // 当触碰到物体
    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // 获取物体对象的刚体
        Rigidbody body = hit.collider.attachedRigidbody;

        // 确保我们碰到的是一个可以移动的刚体而不是静态的
        if (body == null || body.isKinematic)
        {
            return;
        }

        // 我们不想推动地面或者墙壁等
        if (hit.moveDirection.y < -0.3f)
        {
            return;
        }

        // 计算推力方向和大小
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        body.velocity = pushDir * pushPower;
    }

    // 玩家移动的声音
    public void PlayerSound()
    {
        // 判断是否在地面上移动
        if (controller.isGrounded && moveDirection.x != 0 && moveDirection.z != 0 && !isCrouching)
        {
            // 判断是否在奔跑
            movementAudioSource.clip = isRunning ? runSound : walkSound;
            // 判断是否没有音频播放
            if (!movementAudioSource.isPlaying)
            {
                // 播放音频
                movementAudioSource.Play();
            }
        }
        // 判断是否站在水里且在水中移动
        else if (Physics.OverlapSphere(transform.position - new Vector3(0, 0.5f, 0) * standHeight, 0.1f, noCrouchLayerMask).Length != 0 && (moveDirection.x != 0 || moveDirection.z != 0))
        {
            // 播放在水中移动的音效
            movementAudioSource.clip = waterSound;
            // 判断是否没有音频播放
            if (!movementAudioSource.isPlaying)
            {
                // 播放音频
                movementAudioSource.Play();
            }
        }
        // 如果不再移动
        else
        {
            // 判断是否有音频播放
            if (movementAudioSource.isPlaying)
            {
                // 暂停播放
                movementAudioSource.Pause();
            }
        }
    }

    public IEnumerator ChangeHeightTo(float heightTarget)
    {
        // 过渡持续的时间
        float duration = 0.2f;
        // 已经过去的时间
        float elapsed = 0;

        float startHeight = controller.height;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // 计算插值比例
            float t = elapsed / duration;

            controller.height = Mathf.SmoothStep(startHeight, heightTarget, t);

            yield return null;
        }
        // 确保最终值准确设置
        controller.height = heightTarget;
    }


    public IEnumerator ChangeCenterTo(float centerHeight)
    {
        // 过渡持续的时间
        float duration = 0.2f;
        // 已经过去的时间
        float elapsed = 0;

        float startCenterY = controller.center.y;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // 计算插值比例
            float t = elapsed / duration;

            controller.center = new Vector3(0, Mathf.SmoothStep(startCenterY, centerHeight, t), 0);

            yield return null;
        }
        // 确保最终值准确设置
        controller.center = new Vector3(0, centerHeight, 0);
    }

    /// <summary>
    /// 扣除玩家生命值
    /// </summary>
    public void PlayerDamageHealth(float damage)
    {
        // 扣除玩家生命值
        playerHealth -= (int)damage;
        // 受到伤害
        isDamage = true;
        // 控制玩家血量
        playerHealth = Mathf.Clamp(playerHealth, 0, 100f);
        // 更新UI
        playerHealthUI.text = playerHealth + "%";
        // 计算颜色索引
        int colorIndex = Mathf.Clamp((int)(playerHealth * (healthImageColor.Length - 1) / 100f), 0, healthImageColor.Length - 1);
        // 根据血量百分比更新血量提示灯的颜色
        healthImage.color = healthImageColor[colorIndex];
        // 判断玩家是否死亡
        if(playerHealth <= 0)
        {
            isDead = true;
            // 游戏暂停
            Time.timeScale = 0;
        }
    }

    /// <summary>
    /// 恢复玩家生命值，调用Player.AddHealth协程
    /// <summary>
    public void RestoringHealth(float restoringTimes, float restoringTimeBetween, float restoringHealthforUnitTime, AudioClip chewSound){
        // 如果协程已经在运行，则停止它
        if (addHealthCoroutine != null) StopCoroutine(addHealthCoroutine);
        // 执行协程
        addHealthCoroutine = StartCoroutine(AddHealth(restoringTimes, restoringTimeBetween, restoringHealthforUnitTime, chewSound));
    }


    // 恢复玩家生命值
    private IEnumerator AddHealth(float  restroingTimes, float restoringTimeBetween, float restoringHealthforUnitTime, AudioClip chewSound){
        // 等待一段时间以播放拾取音效
        yield return new WaitForSeconds(1);
        // 更换咀嚼音效
        auxiliaryAudioSource.clip = chewSound;
        // 播放声音
        auxiliaryAudioSource.loop = true;
        auxiliaryAudioSource.Play();
        // 循环恢复次数
        for(int i = 0; i < restroingTimes; i++){
            // 增加玩家血量
            playerHealth += (int)restoringHealthforUnitTime;
            // 控制玩家血量
            playerHealth = Mathf.Clamp(playerHealth, 0, 100f);
            // 更新UI
            playerHealthUI.text = playerHealth + "%";
            // 计算颜色索引
            int colorIndex = Mathf.Clamp((int)(playerHealth * (healthImageColor.Length - 1) / 100f), 0, healthImageColor.Length - 1);
            // 根据血量百分比更新血量提示灯的颜色
            healthImage.color = healthImageColor[colorIndex];
            // 等待下一次恢复时间
            yield return new WaitForSeconds(restoringTimeBetween);
        }
        // 暂停咀嚼声音播放
        auxiliaryAudioSource.Pause();
        // 在协程结束时重置引用
        addHealthCoroutine = null;
    }
}
