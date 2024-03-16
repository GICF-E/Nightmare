using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    [Tooltip("角色移动向量")][HideInInspector] public Vector3 moveDirection = Vector3.zero;
    [Tooltip("正确PIN码哈希字段")] private string[] PIN = { "111001011011110010100000", "111001101000001010100000", "111010001011111010110000", "001100010011", "010000110011" };


    [Header("玩家组件引用")]
    [Tooltip("角色控制器组件")][HideInInspector] public CharacterController controller;
    [Tooltip("移动音源")] public AudioSource movementAudioSource;
    [Tooltip("辅助音源")] public AudioSource auxiliaryAudioSource;
    [Tooltip("对回血协程的引用")][HideInInspector] public Coroutine addHealthCoroutine;


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
    [Tooltip("是否允许移动")][HideInInspector] public bool canMove;
    [Tooltip("是否在奔跑")][HideInInspector] public bool isRunning;
    [Tooltip("是否在跳跃")][HideInInspector] public bool isJumping;
    [Tooltip("是否正在蹲下")][HideInInspector] public bool isCrouching;
    [Tooltip("玩家是否可以下蹲")][HideInInspector] public bool canCrouching;
    [Tooltip("玩家是否在翻滚")][HideInInspector] public bool isRowing;
    [Tooltip("人物状态的枚举")][HideInInspector] public enum MovementState { walking, running, idle };
    [Tooltip("角色移动状态")][HideInInspector] public MovementState state;
    [Tooltip("实际移动速度")][HideInInspector] public float currentSpeed;
    [Tooltip("是否被全图标记")][HideInInspector] public bool isMarked;
    [Tooltip("场上是否还有敌人")][HideInInspector] public bool hasEnemy = true;


    [Header("玩家生命")]
    [Tooltip("玩家的生命值")] public float playerHealth = 100f;
    [Tooltip("判断玩家是否死亡")][HideInInspector] public bool isDead;
    [Tooltip("判断玩家是否受到伤害")][HideInInspector] public bool isDamage;


    [Header("跌落伤害的判定")]
    [Tooltip("安全跌落距离")] public float safeFallDistance = 3.0F;
    [Tooltip("开始下落时的高度")] private float fallStartLevel;
    [Tooltip("是否正在下落")] private bool isFalling;



    [Header("特殊模式")]
    [Tooltip("索尼克模式")] public bool isSonicMode;
    [Tooltip("菜单模式")] public bool isMenuMode;


    [Header("UI")]
    [Tooltip("是否显示FPS")] public bool isDisplayFPS;
    [Tooltip("是否显示血量百分比")] public bool isDisplayHealthFigure;
    [Tooltip("是否在查看纸条")] public bool isViewNotes;
    [Tooltip("玩家血量UI")] public Text playerHealthUI;
    [Tooltip("玩家血量提示灯")] public Image healthImage;
    [Tooltip("玩家血量提示灯的颜色")] public Color[] healthImageColor;
    [Tooltip("玩家Esc面板")] public GameObject EscPanel;
    [Tooltip("设置面板")] public GameObject settingPanel;
    [Tooltip("玩家血雾效果")] public Image hurtImage;
    [Tooltip("血雾收到伤害颜色")] private Color flashColor;
    [Tooltip("血雾没有受到伤害的颜色")] private Color clearColor;
    [Tooltip("结局面板")] public GameObject endingPanel;
    [Tooltip("结局文本")] public Text endingText;
    [Tooltip("结局内容")] public Text[] endingContent;
    [Tooltip("输入面板")] public Image inputPanel;
    [Tooltip("输入面板的内容（用于渐变）")] public Graphic[] inputPanelContent;
    [Tooltip("输入的内容（用于输入检测）")] public Text[] inputText;
    [Tooltip("帧间隔时间")] private float deltaTime = 0.0f;


    [Header("输入")]
    [Tooltip("输入系统组件")][HideInInspector] public PlayerInput playerInput;
    [Tooltip("使用单击鼠标右键来控制开镜")] public bool isClickAiming;

    [Header("场景")]
    [Tooltip("敌人列表")] public GameObject[] enemies;
    [Tooltip("武器列表")] public Weapon_AutomaticGun[] weapons;
    [Tooltip("可拾取的武器的列表")] public GameObject[] pickUpItems;
    [Tooltip("可拾取的食物列表")] public GameObject[] foods;
    [Tooltip("可引爆的油桶列表")] public GameObject[] explosiveBarrels;
    [Tooltip("可引爆的气瓶列表")] public GameObject[] gasTanks;

    private void Awake()
    {
        // 初始化
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        standHeight = controller.height;
        EscPanel.SetActive(false);
        settingPanel.SetActive(false);
        endingPanel.SetActive(false);
        endingText.text = "";
        inputPanel.gameObject.SetActive(false);
        foreach (Text text in inputText) text.text = "";
        // 初始化场景列表
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        pickUpItems = GameObject.FindGameObjectsWithTag("PickUpItem");
        foods = GameObject.FindGameObjectsWithTag("Food");
        explosiveBarrels = GameObject.FindGameObjectsWithTag("ExplosiveBarrel");
        gasTanks = GameObject.FindGameObjectsWithTag("GasTank");
        // 重置玩家生命值
        playerHealth = 100f;
        // 重置玩家状态
        isCrouching = false;
        canCrouching = true;
        isRowing = false;
        canMove = isMenuMode ? false : true;
        isMarked = false;
        hasEnemy = true;
        // 根据玩家设置更改变量
        isDisplayFPS = PlayerPrefs.GetInt("isDisplayFPS") == 1;
        isDisplayHealthFigure = PlayerPrefs.GetInt("isDisplayHealthFigure") == 1;
        isSonicMode = PlayerPrefs.GetInt("isSonicMode") == 1;
        isClickAiming = PlayerPrefs.GetInt("isClickAiming") == 1;
    }

    private void Start()
    {
        // 锁定光标
        if(!isMenuMode) Cursor.lockState = CursorLockMode.Locked;
        
        // 根据索尼克模式设置角色速度
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

        // 更新UI
        if (isMenuMode)
        {
            playerHealthUI.gameObject.SetActive(false);
            healthImage.gameObject.SetActive(false);
        }
        else if (isDisplayHealthFigure)
        {
            playerHealthUI.gameObject.SetActive(true);
            healthImage.gameObject.SetActive(false);
        }
        else
        {
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

        // 分割字符串来获取宽度和高度
        string[] dimensions = PlayerPrefs.GetString("resolutionRatio").Split('x');
        // 判断是否分割成功
        if (dimensions.Length == 2)
        {
            // 尝试解析宽度和高度
            int width, height;
            bool widthParsed = int.TryParse(dimensions[0], out width);
            bool heightParsed = int.TryParse(dimensions[1], out height);
            // 如果宽度和高度成功解析，则设置分辨率
            if (widthParsed && heightParsed)
            {
                // 设置分辨率并强制全屏
                Screen.SetResolution(width, height, true);
            }
            else
            {
                // 解析失败，使用当前屏幕分辨率作为默认值
                UseDefaultResolution();
            }
        }
        else
        {
            // 格式不正确，使用默认分辨率
            UseDefaultResolution();
        }

        // 读取存档
        LoadGame();
    }

    void Update()
    {
        // 更新帧间隔时间
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

        // 判断是否可以移动
        if (!canMove) movementAudioSource.Pause();

        // 判断场上是否还有敌人
        if (!hasEnemy && canMove)
        {
            // 停止移动
            canMove = false;
            // 解锁鼠标
            Cursor.lockState = CursorLockMode.None;
            // 更新结局字样
            endingText.text = isMarked ? "结局D: 迭代" : "结局C: 摇篮";
            // 打开结局面板
            StartCoroutine(OpenEndingPanel(endingPanel.GetComponent<Image>(), endingPanel.GetComponent<Image>().color, 1f, 1f));
        }

        // 当角色站在地面上且可以移动时执行以下逻辑
        if (controller.isGrounded && canMove)
        {
            // 根据玩家输入计算移动方向
            moveDirection = new Vector3(playerInput.actions["Move"].ReadValue<Vector2>().x, 0, playerInput.actions["Move"].ReadValue<Vector2>().y);
            moveDirection = transform.TransformDirection(moveDirection);

            // 检查玩家是否正在奔跑或跳跃
            isRunning = playerInput.actions["Run"].IsPressed();
            isJumping = playerInput.actions["Jump"].IsPressed();

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
            if (!isMenuMode) moveDirection.y -= gravity * Time.deltaTime;
        }

        // 玩家受到伤害后屏幕产生红色渐变
        if (isDamage && !isDead)
        {
            // 受到伤害时瞬间切换到红色
            hurtImage.color = flashColor;
        }
        else if (!isDead)
        {
            // 没有受到伤害时缓慢渐变到白色
            hurtImage.color = Color.Lerp(hurtImage.color, clearColor, Time.deltaTime * 3);
        }
        // 重置受到伤害的变量
        isDamage = false;

        // 判断是否死亡
        if (isDead)
        {
            // 面板切换为红色
            hurtImage.color = flashColor;
            // 停止移动
            canMove = false;
        }

        // 判断玩家按下翻滚按键
        if (playerInput.actions["Roll"].triggered && canMove)
        {
            StartCoroutine(Roll());
        }

        // 如果处在斜坡上移动
        if (OnSlope())
        {
            //给玩家施加一个向下的力
            controller.Move(Vector3.down * controller.height / 2 * slopeForce * Time.deltaTime);
        }

        // 判断玩家是否按下Esc
        if (playerInput.actions["Esc"].triggered && !isMenuMode && !isViewNotes && !isDead)
        {
            // 切换面板状态和玩家移动的状态
            if (settingPanel.activeSelf) CloseSetting();
            else
            {
                EscPanel.SetActive(EscPanel.activeSelf ? false : true);
                // 切换鼠标锁定状态
                Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
                canMove = !canMove;
                // 存档
                SaveGame();
            }
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

        // 可以移动的情况下移动角色控制器
        if (canMove) controller.Move(moveDirection * Time.deltaTime);
        // 播放声音
        if (canMove) PlayerSound();
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
        if (Physics.OverlapSphere(transform.position - new Vector3(0, 0.5f, 0) * standHeight, 0.1f, noCrouchLayerMask).Length != 0)
        {
            // 强制下蹲
            controller.height = crouchHeight;
            controller.center = controller.height / 2f * Vector3.up;
            isCrouching = true;
            // 退出程序
            return;
        }
        // 判断是否可以下蹲或有没有按下对应按键
        if (playerInput.actions["Crouch"].IsPressed() && canCrouching && !isCrouching)
        {
            // 根据是否可以下蹲改变高度和中心
            StartCoroutine(ChangeHeightTo(crouchHeight));
            StartCoroutine(ChangeCenterTo(0.5f));
            isCrouching = true;
        }
        else if (!playerInput.actions["Crouch"].IsPressed() && canCrouching && isCrouching)
        {
            // 根据是否可以下蹲改变高度和中心
            StartCoroutine(ChangeHeightTo(standHeight));
            StartCoroutine(ChangeCenterTo(0));
            isCrouching = false;
        }
    }

    // 玩家翻滚的逻辑
    private IEnumerator Roll()
    {
        // 翻滚计时器
        float rollTime = 0.9f;
        float rollTimer = 0f;
        // 翻滚方向
        Vector3 rollDirection = transform.forward;
        // 翻滚时是否在斜坡上
        bool isOnSlope = OnSlope();
        // 判断是否可以翻滚
        if (canCrouching && !isCrouching && !isRowing)
        {
            // 翻滚下蹲
            StartCoroutine(ChangeHeightTo(crouchHeight));
            StartCoroutine(ChangeCenterTo(0.5f));
            isRowing = true;
            while (rollTimer < rollTime)
            {
                rollTimer += Time.deltaTime;
                // 移动并应用旋转
                controller.Move(rollDirection * 20 * Time.deltaTime);
                transform.Rotate(Vector3.right * 400 * Time.deltaTime);
                // 翻滚时施加下压力
                if (isOnSlope) controller.Move(Vector3.down * 25 * Time.deltaTime);
                else controller.Move(Vector3.down * 5 * Time.deltaTime);
                yield return null;
            }
            // 锁定旋转
            transform.rotation = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y, 0));
            isRowing = false;
            yield return new WaitForSeconds(0.1f);
            // 站立
            StartCoroutine(ChangeHeightTo(standHeight));
            StartCoroutine(ChangeCenterTo(0));
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
        if (playerHealth <= 0 && !isDead)
        {
            // 更改死亡状态
            isDead = true;
            // 面板切换为红色
            hurtImage.color = flashColor;
            // 停止移动
            canMove = false;
            // 解锁鼠标
            Cursor.lockState = CursorLockMode.None;
            // 根据不同的状态显示不同的结局字样
            endingText.text = isMarked ? "结局B: 梦魇" : "结局A: 陨落";
            // 打开结局面板
            StartCoroutine(OpenEndingPanel(endingPanel.GetComponent<Image>(), endingPanel.GetComponent<Image>().color, 1f, 1f));
        }
    }

    /// <summary>
    /// 恢复玩家生命值，调用Player.AddHealth协程
    /// <summary>
    public void RestoringHealth(float restoringTimes, float restoringTimeBetween, float restoringHealthforUnitTime, AudioClip chewSound)
    {
        // 如果协程已经在运行，则停止它
        if (addHealthCoroutine != null) StopCoroutine(addHealthCoroutine);
        // 执行协程
        addHealthCoroutine = StartCoroutine(AddHealth(restoringTimes, restoringTimeBetween, restoringHealthforUnitTime, chewSound));
    }


    // 恢复玩家生命值
    private IEnumerator AddHealth(float restroingTimes, float restoringTimeBetween, float restoringHealthforUnitTime, AudioClip chewSound)
    {
        // 等待一段时间以播放拾取音效
        yield return new WaitForSeconds(1);
        // 更换咀嚼音效
        auxiliaryAudioSource.clip = chewSound;
        // 播放声音
        auxiliaryAudioSource.loop = true;
        auxiliaryAudioSource.Play();
        // 循环恢复次数
        for (int i = 0; i < restroingTimes; i++)
        {
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
            // 保持咀嚼音效
            auxiliaryAudioSource.clip = chewSound;
            if (!auxiliaryAudioSource.isPlaying) auxiliaryAudioSource.Play();
            // 等待下一次恢复时间
            yield return new WaitForSeconds(restoringTimeBetween);
        }
        // 暂停咀嚼声音播放
        auxiliaryAudioSource.Pause();
        // 在协程结束时重置引用
        addHealthCoroutine = null;
    }

    // 继续游戏
    public void ContinueGame()
    {
        // 锁定鼠标
        Cursor.lockState = CursorLockMode.Locked;
        // 关闭Esc面板并解锁玩家
        EscPanel.SetActive(false);
        canMove = !canMove;
    }

    // 显示设置界面
    public void ShowSetting()
    {
        settingPanel.SetActive(true);
    }

    // 关闭设置界面
    public void CloseSetting()
    {
        // 关闭设置面板
        settingPanel.SetActive(false);

        // 根据玩家设置更改变量
        isDisplayFPS = PlayerPrefs.GetInt("isDisplayFPS") == 1;
        isDisplayHealthFigure = PlayerPrefs.GetInt("isDisplayHealthFigure") == 1;
        isSonicMode = PlayerPrefs.GetInt("isSonicMode") == 1;
        isClickAiming = PlayerPrefs.GetInt("isClickAiming") == 1;

        // 根据超音速模式设置角色速度
        walkSpeed = isSonicMode ? 8f : 6f;
        runSpeed = isSonicMode ? 40f : 10f;
        jumpSpeed = isSonicMode ? 20f : 4f;
        crouchSpeed = isSonicMode ? 20f : 3f;

        // 根据玩家设置显示或隐藏健康状态
        playerHealthUI.gameObject.SetActive(isDisplayHealthFigure);
        healthImage.gameObject.SetActive(!isDisplayHealthFigure);

        // 遍历场景中所有Weapon和Enemy，更改设置
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Weapon"))
        {
            if (obj.GetComponent<Weapon_AutomaticGun>() != null)
            {
                obj.GetComponent<Weapon_AutomaticGun>().UpdateSettings();
            }
        }

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (obj.GetComponent<Enemy>() != null)
            {
                obj.GetComponent<Enemy>().UpdateSettings();
            }
        }

        // 分割字符串来获取宽度和高度
        string[] dimensions = PlayerPrefs.GetString("resolutionRatio").Split('x');
        // 判断是否分割成功
        if (dimensions.Length == 2)
        {
            // 尝试解析宽度和高度
            int width, height;
            bool widthParsed = int.TryParse(dimensions[0], out width);
            bool heightParsed = int.TryParse(dimensions[1], out height);
            // 如果宽度和高度成功解析，则设置分辨率
            if (widthParsed && heightParsed)
            {
                // 设置分辨率并强制全屏
                Screen.SetResolution(width, height, true);
            }
            else
            {
                // 解析失败，使用当前屏幕分辨率作为默认值
                UseDefaultResolution();
            }
        }
        else
        {
            // 格式不正确，使用默认分辨率
            UseDefaultResolution();
        }
    }

    // 默认分辨率
    private void UseDefaultResolution()
    {
        //获取设置当前屏幕分辩率
        Resolution[] resolutions = Screen.resolutions;

        //设置当前分辨率
        Screen.SetResolution(resolutions[resolutions.Length - 1].width, resolutions[resolutions.Length - 1].height, true);
    }

    // 返回主页
    public void BackHome()
    {
        SceneManager.LoadScene("Loading Menu");
    }

    // 清除存档并返回主页
    public void ClearGame()
    {
        // 清除数据
        SaveSystem.ClearData();
        // 加载主页
        SceneManager.LoadScene("Loading Menu");
    }

    // 退出游戏
    public void QuitGame()
    {
        Application.Quit();
    }

    // 隐藏结局，由输入面板的按钮调用
    public void HideEnding()
    {
        // 是否输入正确
        bool isInputCorrect = true;
        for (int i = 0; i < inputText.Length; i++)
        {
            if (inputText[i].text != PIN[i]) isInputCorrect = false;
        }
        // 停止移动
        canMove = false;
        // 根据不同的状态显示不同的结局字样
        endingText.text = isInputCorrect ? "结局F: 解梦" : "结局E: 迷失";
        // 打开结局面板
        StartCoroutine(OpenEndingPanel(endingPanel.GetComponent<Image>(), endingPanel.GetComponent<Image>().color, 1f, 1f));
    }

    /// <summary>
    /// 存储游戏数据
    /// </summary>
    public void SaveGame()
    {
        // 定义新的PlayerData存储实现
        PlayerData playerData = new PlayerData();
        // 将玩家数据写入实现
        playerData.position = transform.position;
        playerData.rotation = transform.rotation;
        playerData.isMarked = isMarked;
        playerData.playerHealth = playerHealth;
        // 遍历所有枪械
        foreach (Weapon_AutomaticGun weapon in weapons)
        {
            // 定义新的WeaponData存储实现
            WeaponData weaponData = new WeaponData();
            // 将枪械数据写入实现
            weaponData.isActive = weapon.gameObject == null ? false : weapon.gameObject.activeSelf;
            weaponData.currentBullet = weapon == null ? 0 : weapon.currentBullet;
            weaponData.bulletLeft = weapon == null ? 0 : weapon.bulletLeft;
            // 将WeaponData添加到PlayerData
            playerData.weaponsData.Add(weaponData);
        }
        // 遍历所有敌人
        foreach (GameObject enemy in enemies)
        {
            // 定义新的EnemyData存储实现
            EnemyData enemyData = new EnemyData();
            // 将敌人数据写入实现
            enemyData.position = enemy.transform.position;
            enemyData.patrolIndex = enemy.GetComponent<Enemy>() == null ? 0 : enemy.GetComponent<Enemy>().patrolIndex;
            enemyData.isDead = enemy.GetComponent<Enemy>() == null ? false : enemy.GetComponent<Enemy>().isDead;
            // 将EnemyData添加到PlayerData
            playerData.enemiesData.Add(enemyData);
        }
        // 遍历所有可拾取的枪械
        foreach (GameObject pickUpItem in pickUpItems){
            // 将枪械是否被拾取的信息写入PlayerData
            playerData.isItemPick.Add((pickUpItem == null || !pickUpItem.activeSelf) ? true : false);
        }
        // 遍历所有食物
        foreach (GameObject food in foods){
            // 将食物是否被拾取的信息写入PlayerData
            playerData.isFoodEaten.Add((food == null || !food.activeSelf) ? true : false);
        }
        // 遍历所有油桶
        foreach (GameObject explosiveBarrel in explosiveBarrels){
            // 将油桶是否被引爆的信息写入PlayerData
            playerData.isExplosiveBarrelsExplode.Add((explosiveBarrel == null || !explosiveBarrel.activeSelf) ? true : false);
        }
        // 遍历所有气瓶
        foreach (GameObject gasTank in gasTanks){
            // 将气瓶是否被引爆的信息写入PlayerData
            playerData.isGasTanksHit.Add((gasTank == null || !gasTank.activeSelf) ? true : false);
        }
        // 通过JSON保存数据
        SaveSystem.SaveData(playerData);
    }

    /// <summary>
    /// 读取游戏数据
    /// </summary>
    public void LoadGame()
    {
        PlayerData playerData = SaveSystem.LoadData();
        if (playerData == null) return;
        // 读取玩家数据
        transform.position = playerData.position;
        transform.rotation = playerData.rotation;
        isMarked = playerData.isMarked;
        playerHealth = playerData.playerHealth;
        // 读取枪械数据
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].gameObject.SetActive(playerData.weaponsData[i].isActive);
            weapons[i].currentBullet = playerData.weaponsData[i].currentBullet;
            weapons[i].bulletLeft = playerData.weaponsData[i].bulletLeft;
        }
        // 读取敌人数据
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].transform.position = playerData.enemiesData[i].position;
            enemies[i].GetComponent<Enemy>().patrolIndex = playerData.enemiesData[i].patrolIndex;
            enemies[i].GetComponent<Enemy>().isDead = playerData.enemiesData[i].isDead;
        }
        // 读取可拾取的枪械数据
        for (int i = 0; i < pickUpItems.Length; i++)
        {
            // 如果枪械被拾取则直接禁用
            if(playerData.isItemPick[i]) pickUpItems[i].SetActive(false);
        }
        // 读取食物数据
        for (int i = 0; i < foods.Length; i++)
        {
            // 如果食物被拾取则直接禁用
            if(playerData.isFoodEaten[i]) foods[i].SetActive(false);
        }
        // 读取油桶数据
        for (int i = 0; i < explosiveBarrels.Length; i++)
        {
            // 如果油桶被引爆则直接禁用
            if(playerData.isExplosiveBarrelsExplode[i]) explosiveBarrels[i].SetActive(false);
        }
        // 读取气瓶数据
        for (int i = 0; i < gasTanks.Length; i++)
        {
            // 如果气瓶被引爆则直接禁用
            if(playerData.isGasTanksHit[i]) gasTanks[i].SetActive(false);
        }
    }

    /// <summary>
    /// 渐变更改结局面板颜色
    /// </summary>
    public IEnumerator OpenEndingPanel(Graphic graphic, Color targetColor, float time, float waitTime)
    {
        // 清除存档数据
        SaveSystem.ClearData();
        // 初始化颜色
        graphic.color = Color.clear;
        foreach (Text text in inputText)
        {
            text.color = Color.clear;
        }
        // 等待时间
        yield return new WaitForSeconds(waitTime);
        // 启用面板
        graphic.gameObject.SetActive(true);
        // 计时器和开始颜色
        float timer = 0;
        Color startColor = graphic.color;
        // 循环渐变
        while (timer < time)
        {
            // 更新计时器
            timer += Time.deltaTime;
            // 更新颜色
            graphic.color = Color.Lerp(startColor, targetColor, timer / time);
            endingText.color = Color.Lerp(startColor, Color.white, timer / time);
            foreach (Text text in endingContent)
            {
                text.color = Color.Lerp(startColor, new Color(0.75f, 0.75f, 0.75f, 255), timer / time);
            }
            yield return null;
        }
        // 最终确定颜色
        graphic.color = targetColor;
        endingText.color = Color.white;
        foreach (Text text in endingContent)
        {
            text.color = new Color(0.75f, 0.75f, 0.75f, 255);
        }
    }

    /// <summary>
    /// 渐变更改结局面板颜色
    /// </summary>
    public IEnumerator OpenInputPanel(float time)
    {
        // 使用数组来存储渐变开始之前的颜色
        Color[] colors = new Color[inputPanelContent.Length];
        for (int i = 0; i < inputPanelContent.Length; i++)
        {
            colors[i] = inputPanelContent[i].color;
        }
        // 初始化颜色
        inputPanel.color = Color.clear;
        foreach (Graphic graphic in inputPanelContent) graphic.color = Color.clear;
        // 启用面板
        inputPanel.gameObject.SetActive(true);
        // 计时器和开始颜色
        float timer = 0;
        // 循环渐变
        while (timer < time)
        {
            // 更新计时器
            timer += Time.deltaTime;
            // 更新颜色
            for (int i = 0; i < inputPanelContent.Length; i++)
            {
                inputPanelContent[i].color = Color.Lerp(Color.clear, colors[i], timer / time);
            }
            yield return null;
        }
        // 最终确定颜色
        for (int i = 0; i < inputPanelContent.Length; i++)
        {
            inputPanelContent[i].color = colors[i];
        }
    }

    // 判断用户是否聚焦在了游戏窗口上
    void OnApplicationFocus(bool hasFocus)
    {
        // 用户离开了当前窗口
        if (!hasFocus && !isMenuMode && !isViewNotes && !isDead)
        {
            // 切换面板状态和玩家移动的状态
            CloseSetting();
            EscPanel.SetActive(true);
            // 切换鼠标锁定状态
            Cursor.lockState = CursorLockMode.None;
            canMove = false;
            // 存档
            SaveGame();
        }
    }


    /// <summary>
    /// 通过绘制GUI显示FPS
    /// <summary>
    private void OnGUI()
    {
        if (isDisplayFPS && !isMenuMode)
        {
            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(0, 0, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = new Color(1, 1, 1, 1.0f);
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            string text = string.Format(" {1:0.} fps", msec, fps);
            GUI.Label(rect, text, style);
        }
    }
}
