using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Player;

/// <summary>
/// 武器音效的内部类
/// </summary>
// 序列化变量
[System.Serializable]
public class WeaponSoundClips
{
    // 开火音效
    public AudioClip shootSound;
    // 带消音器的开火音效
    public AudioClip silencerShootSound;
    // 非空弹夹更换子弹的音效
    public AudioClip reloadSoundAmmotLeft;
    // 空弹夹下更换弹夹的音效
    public AudioClip reloadSoundOutOfAmmo;
    // 瞄准音效
    public AudioClip aimSound;
    // 近战音效
    public AudioClip knifeAttackSound;
}

/// <summary>
/// 控制武器的类
/// </summary>
public class Weapon_AutomaticGun : Weapon
{
    // 玩家对象代码
    private Player player;
    // 玩家移动状态
    public Player.MovementState state;

    [Header("武器部件位置")]
    [Tooltip("射击的位置")] public Transform ShootPoint;
    [Tooltip("子弹特效打出的位置")] public Transform BulletShootPoint;
    [Tooltip("子弹壳跑出的位置")] public Transform CasingBulletSpawnPoint;
    [Tooltip("手电筒")] public GameObject flashLight;
    [Tooltip("主摄像机")] private Camera mainCamera;
    [Tooltip("枪械摄像机")] public Camera gunCamera;

    [Header("子弹预制体和特效")]
    [Tooltip("子弹")] public Transform bulletPrefab;
    [Tooltip("子弹抛壳")] public Transform casingPrefab;

    [Header("枪械属性")]
    [Tooltip("武器射程")] private float range = 500f;
    [Tooltip("武器射速")] public float fireRate;
    [Tooltip("武器的弹夹子弹数")] public int bulletMag;
    [Tooltip("当前弹夹剩余子弹数")] public int currentBullet;
    [Tooltip("当前备弹数")] public int bulletLeft;
    [Tooltip("原始射速")]  private float originRate;
    [Tooltip("射击偏移量")]  private float SpreadFactor;
    [Tooltip("武器射速计时器")]  private float fireTimer;
    [Tooltip("枪械最小伤害")] public float minDamgae;
    [Tooltip("枪械最大伤害")] public float maxDamage;
    [Tooltip("子弹发射的力")] private float bulletForce = 150f;
    [Tooltip("射击模式枚举")] public enum ShootMode {AutoRifle,SemiGun};
    [Tooltip("射击模式枚举值")] public ShootMode shootingMode;
    [Tooltip("射击输入方法的变更")] private bool GunShootInput;
    [Tooltip("用来区分中间参数（1.Auto 2.Semi）")] private int modeNum;
    [Tooltip("是否是自动武器")] public bool IS_AUTORIFLE;
    [Tooltip("是否带有消音器")] public bool IS_SILENCER;
    [Tooltip("是否是霰弹枪")] public bool IS_SHOTGUN;
    [Tooltip("是否是狙击枪")] public bool IS_SNIPER;
    [Tooltip("一次打出的子弹数")] private int gunFragment;
    [Tooltip("判断是否在装弹")] private bool isReloading;
    [Tooltip("判读是否进入了瞄准状态")] private bool isAiming;
    [Tooltip("玩家按下鼠标右键的次数")] private int mouseBottonNumber = 0;


    [Header("特效")]
    [Tooltip("开火灯光")] public Light muzzleflashLight;
    [Tooltip("灯光持续时间")] private float lightDuration = 0.02f;
    [Tooltip("枪口火焰粒子特效")] public ParticleSystem muzzlePlatic;
    [Tooltip("火花粒子特效")] public ParticleSystem sparkPlatic;
    [Tooltip("火星的最小随机分布值")] public int minSparkEmission;
    [Tooltip("火星的最大随机分布值")] public int maxSparkEmission;


    [Header("音效")]
    [Tooltip("主音源")] public AudioSource mainAudioSource;
    [Tooltip("声音对象")] public WeaponSoundClips soundClips;


    [Header("动画")]
    [Tooltip("动画状态机")] public Animator animator;


    [Header("UI")]
    [Tooltip("是否显示子弹剩余")] public bool isDisplayAmmo;
    [Tooltip("是否显示枪械状态")] public bool isDisplayShootMode;
    [Tooltip("准心")] public Image[] crossQuarterImages;
    [Tooltip("当前准心的开合度")] private float currentExpanedDegree;
    [Tooltip("最大开合度")] private float maxCrossDegree = 100f;
    [Tooltip("武器的射击模式的名称")]private string shootModeName;
    [Tooltip("枪械的剩余子弹量文本")] public TextMeshProUGUI ammoTextUI;
    [Tooltip("枪械的射击模式文本")] public TextMeshProUGUI shootModeTextUI;


    [Header("狙击镜")]
    [Tooltip("狙击镜材质")] public Material scopeRenderMaterial;
    [Tooltip("没有进行瞄准时狙击镜的颜色")] public Color fadeColor;
    [Tooltip("瞄准时狙击镜的颜色")] public Color defaultColor;


    [Header("键位设置")]
    [SerializeField][Tooltip("填充子弹的按键")] private KeyCode reloadInputName = KeyCode.R;
    [SerializeField][Tooltip("查看武器按键")] private KeyCode inspectInputName = KeyCode.I;
    [SerializeField][Tooltip("半自动切换按键")] private KeyCode GunShootModeInputName = KeyCode.X;
    [SerializeField][Tooltip("丢弃武器的按键")] private KeyCode throwWeaponInputName = KeyCode.T;
    [SerializeField][Tooltip("开关手电筒的按键")] private KeyCode flashLightInputName = KeyCode.C;
    [SerializeField][Tooltip("玩家用到攻击键")] private KeyCode knifeAttackInputName = KeyCode.Q;

    private void Awake()
    {
        // 对音源进行赋值
        mainAudioSource = GetComponent<AudioSource>();
        // 获取玩家代码
        player = GetComponentInParent<Player>();
        // 定义动画状态机
        animator = GetComponent<Animator>();
        // 赋值摄像机
        mainCamera = Camera.main;
    }

    private void Start()
    {
        // 隐藏开火灯光并调整摄像机视野
        muzzleflashLight.enabled = false;
        flashLight.SetActive(false);
        mainCamera.fieldOfView = 60f;

        // 定义枪械属性和部件位置
        currentBullet = bulletMag;
        bulletLeft = bulletMag * 4;
        bulletForce = 150f;
        range = 500f;
        originRate = fireRate;
        mouseBottonNumber = 0;
        if (IS_SHOTGUN) gunFragment = 8;
        else gunFragment = 1;
        if (IS_SNIPER) scopeRenderMaterial.color = fadeColor;

        // 定义UI属性
        maxCrossDegree = 15f;
        // 显示或隐藏UI
        if(isDisplayAmmo) ammoTextUI.gameObject.SetActive(true);
        else ammoTextUI.gameObject.SetActive(false);
        if(isDisplayShootMode) shootModeTextUI.gameObject.SetActive(true);
        else shootModeTextUI.gameObject.SetActive(false);

        // 如果是全自动武器
        if (IS_AUTORIFLE)
        {
            // 更改武器属性的枚举值
            modeNum = 1;
            shootModeName = "Auto";
            shootingMode = ShootMode.AutoRifle;
            // 初始化UI
            UpdateAmmoUI();
        }
        else
        {
            // 更改武器属性的枚举值
            modeNum = 2;
            shootModeName = "Semi";
            shootingMode = ShootMode.SemiGun;
            // 初始化UI
            UpdateAmmoUI();
        }

        // 如果是狙击枪，隐藏准心
        if (IS_SNIPER)
        {
            for (int i = 0; i < crossQuarterImages.Length; i++)
            {
                crossQuarterImages[i].gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        // 如果是狙击枪，隐藏准心
        if (IS_SNIPER)
        {
            for (int i = 0; i < crossQuarterImages.Length; i++)
            {
                crossQuarterImages[i].gameObject.SetActive(false);
            }
        }

        // 特殊情况隐藏准心
        if (!player.canMove)
        {
            for (int i = 0; i < crossQuarterImages.Length; i++)
            {
                crossQuarterImages[i].gameObject.SetActive(false);
            }
        }else if(!isAiming){
            for (int i = 0; i < crossQuarterImages.Length; i++)
            {
                crossQuarterImages[i].gameObject.SetActive(true);
            }
        }

        // 判断是否可以被移动
        if(player.canMove){
        // 如果是全自动枪械
        if (IS_AUTORIFLE)
        {
                // 切换射击模式
                if (Input.GetKeyDown(GunShootModeInputName) && modeNum != 1)
                {
                    // 切换射击模式的枚举值
                    modeNum = 1;
                    shootingMode = ShootMode.AutoRifle;
                    // 更新UI
                    shootModeName = "Auto";
                    UpdateAmmoUI();
                }else if(Input.GetKeyDown(GunShootModeInputName) && modeNum != 0)
                {
                    // 切换射击模式的枚举值
                    modeNum = 2;
                    shootingMode = ShootMode.SemiGun;
                    // 更新UI
                    shootModeName = "Semi";
                    UpdateAmmoUI();
                }
                // 根据射击模式判断射击
                switch (shootingMode)
                {
                    case ShootMode.AutoRifle:
                        GunShootInput = Input.GetMouseButton(0);
                        fireRate = originRate;
                        break;
                    case ShootMode.SemiGun:
                        GunShootInput = Input.GetMouseButtonDown(0);
                        fireRate = 0.2f;
                        break;
                }
            }
            else
            {
                // 切换射击模式的枚举值
                modeNum = 2;
                shootingMode = ShootMode.SemiGun;
                // 更新射速和触发方法
                GunShootInput = Input.GetMouseButtonDown(0);
                // 非全自动枪械使用默认射速
                fireRate = originRate;
                // 更新UI
                shootModeName = "Semi";
                UpdateAmmoUI();
            }

            // 判断玩家是否按下攻击键
            if(Input.GetKeyDown(knifeAttackInputName))
            {
                // 播放近战动画
                animator.SetTrigger("KnifeAttack");
                // 播放近战声音
                mainAudioSource.clip = soundClips.knifeAttackSound;
                // 播放声音
                mainAudioSource.Play();
            }

            // 如果按下了切换手电筒开关的按键
            if(Input.GetKeyDown(flashLightInputName) && !flashLight.activeSelf)
            {
                flashLight.SetActive(true);
            }
            else if(Input.GetKeyDown(flashLightInputName) && flashLight.activeSelf)
            {
                flashLight.SetActive(false);
            }

            // 判断换弹动画的状态
            if ((animator.GetCurrentAnimatorStateInfo(0).IsName("reload_ammo_left") ||
                animator.GetCurrentAnimatorStateInfo(0).IsName("reload_out_of_ammo") ||
                animator.GetCurrentAnimatorStateInfo(0).IsName("reload_open") ||
                animator.GetCurrentAnimatorStateInfo(0).IsName("reload_close") ||
                animator.GetCurrentAnimatorStateInfo(0).IsName("reload_insert 1") ||
                animator.GetCurrentAnimatorStateInfo(0).IsName("reload_insert 2") ||
                animator.GetCurrentAnimatorStateInfo(0).IsName("reload_insert 3") ||
                animator.GetCurrentAnimatorStateInfo(0).IsName("reload_insert 4") ||
                animator.GetCurrentAnimatorStateInfo(0).IsName("reload_insert 5") ||
                animator.GetCurrentAnimatorStateInfo(0).IsName("reload_insert 6")))
            {
                isReloading = true;
            }
            else {
                isReloading = false;
            }
        // 对于霰弹枪或狙击枪的结束换弹判定
            if((animator.GetCurrentAnimatorStateInfo(0).IsName("reload_insert 1") ||
                animator.GetCurrentAnimatorStateInfo(0).IsName("reload_insert 2") ||
                animator.GetCurrentAnimatorStateInfo(0).IsName("reload_insert 3") ||
                animator.GetCurrentAnimatorStateInfo(0).IsName("reload_insert 4") ||
                animator.GetCurrentAnimatorStateInfo(0).IsName("reload_insert 5") ||
                animator.GetCurrentAnimatorStateInfo(0).IsName("reload_insert 6")) && (currentBullet == bulletMag || bulletLeft <= 0))
            {
                isReloading = false;
                animator.Play("reload_close");
            }
            // 播放行走、跑步动画
            animator.SetBool("Walk", state == MovementState.walking);
            if(state == MovementState.running)
            {
                animator.SetBool("Run", true);
                // 退出瞄准状态
                isAiming = false;
                // 设置动画机状态
                animator.SetBool("Aim", isAiming);
            }
            else
            {
                animator.SetBool("Run", false);
            }
            // 播放检视动画
            if (Input.GetKeyDown(inspectInputName))
            {
                animator.SetTrigger("Inspect");
            }

            // 计时器
            if (fireTimer < fireRate) fireTimer += Time.deltaTime;

            // 判断玩家鼠标左键射击
            if (GunShootInput)
            {
                // 根据条件判定射线检测的次数
                if (IS_SHOTGUN) gunFragment = 8;
                else gunFragment = 1;
                // 开枪射击
                GunFire();
            }

            // 腰射和瞄准状态的射击精度
            SpreadFactor = isAiming ? 0f : 0.05f;

            // 判断玩家鼠标右键进入瞄准
            if(Input.GetMouseButtonDown(1) && mouseBottonNumber == 0 && !isReloading && !animator.GetCurrentAnimatorStateInfo(0).IsName("run"))
            {
                // 更改开镜状态
                isAiming = true;
                // 设置动画机状态
                animator.SetBool("Aim", isAiming);
                // 记录鼠标按下次数
                mouseBottonNumber = 1;
            }
            else if(Input.GetMouseButtonDown(1) && mouseBottonNumber == 1)
            {
                // 更改开镜状态
                isAiming = false;
                // 设置动画机状态
                animator.SetBool("Aim", isAiming);
                // 记录鼠标按下次数
                mouseBottonNumber = 0;
            }

            // 判断是否按下换弹按键
            if (Input.GetKeyDown(reloadInputName) && !isReloading)
            {
                // 执行换弹
                DoReloadAnimation();
                // 退出瞄准状态
                isAiming = false;
                // 显示准心
                for (int i = 0; i < crossQuarterImages.Length; i++)
                {
                    crossQuarterImages[i].gameObject.SetActive(true);
                }

                // 狙击枪瞄准时，改回GunCamera视野和瞄准镜的颜色
                if (IS_SNIPER)
                {
                    // 改变瞄准镜颜色
                    scopeRenderMaterial.color = fadeColor;
                    // 改变GunCamera视野
                    StartCoroutine(CameraView(gunCamera, 50));
                }
                // 摄像机视野变远
                StartCoroutine(CameraView(mainCamera, 60));
                // 设置动画机状态
                animator.SetBool("Aim", isAiming);
            }

            // 判断玩家是否按下丢弃武器的按键
            if (Input.GetKeyDown(throwWeaponInputName))
            {
                // 丢弃武器
                GameObject.Find("Inventory").GetComponent<Inventory>().ThrowWeapon(int.Parse(gameObject.name), gameObject);
            }

            // 实时获取玩家的移动状态
            state = player.state;

            // 根据玩家的状态调整准心的开合度
            if (state == MovementState.walking) ExpandingCrossUpdate(3);
            else if (state == MovementState.running) ExpandingCrossUpdate(8);
            else ExpandingCrossUpdate(0);
        }
    }

    // 实现枪械射击
    public override void GunFire()
    {
        // 判断是否符合开火条件
        if (fireTimer < fireRate || currentBullet <= 0 || animator.GetCurrentAnimatorStateInfo(0).IsName("take_out") || animator.GetCurrentAnimatorStateInfo(0).IsName("run") || animator.GetCurrentAnimatorStateInfo(0).IsName("inspect") || isReloading ) return;

        // 开火灯光
        StartCoroutine(MuzzleFlashLight());
        // 发射一个枪口火焰粒子
        muzzlePlatic.Emit(1);
        // 随机发射火星粒子
        sparkPlatic.Emit(Random.Range(minSparkEmission, maxSparkEmission));

        // 判断是否在瞄准
        if (!isAiming)
        {
            // 播放普通开火动画（淡入淡出）
            animator.CrossFadeInFixedTime("fire", 0.1f);
        }
        else
        {
            // 播放瞄准开火动画
            animator.Play("aim_fire",0,0);
        }

        // 循环子弹的发射次数
        for(int i = 0; i < gunFragment; i++)
        {
            // 定义射线击中的物体
            RaycastHit hit;
            // 向前方发射
            Vector3 shootDirection = ShootPoint.forward;
            // 堆射击方向添加偏移值
            shootDirection = shootDirection + ShootPoint.TransformDirection(new Vector3(Random.Range(-SpreadFactor, SpreadFactor), Random.Range(-SpreadFactor, SpreadFactor)));
            // 声名子弹的属性
            Transform bullet;
            // 生成子弹
            if (!IS_SHOTGUN)
            {
                // 在BulletShootPoint上生成子弹
                bullet = Instantiate(bulletPrefab, BulletShootPoint.transform.position, BulletShootPoint.transform.rotation);
                // 为子弹添加一个带有偏移值的向前的力
                bullet.GetComponent<Rigidbody>().velocity = (bullet.transform.forward + shootDirection) * bulletForce;
            }
            // 进行射线判定
            if (Physics.Raycast(ShootPoint.position, shootDirection, out hit, range))
            {
                Debug.Log(hit.transform.gameObject.name);
                // 霰弹枪生成子弹
                if(IS_SHOTGUN)
                {
                    // 霰弹枪特殊处理在hit.point上生成子弹
                    bullet = Instantiate(bulletPrefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                    // 为子弹添加一个带有偏移值的向前的力
                    bullet.GetComponent<Rigidbody>().velocity = (bullet.transform.forward + shootDirection) * bulletForce;
                }
                // 击中敌人判断
                if(hit.transform.tag == "Enemy")
                {
                    // 对击中的敌人进行随机扣血
                    if(currentBullet > 2) hit.transform.GetComponent<Enemy>().Health(Random.Range(minDamgae, maxDamage));
                    else hit.transform.GetComponent<Enemy>().Health(minDamgae * 2);
                }
                if(hit.transform.tag == "EnemyCollider")
                {
                    // 对击中碰撞题的父级进行扣血
                    if(currentBullet > 2) hit.transform.GetComponentInParent<Enemy>().Health(Random.Range(minDamgae, maxDamage));
                    else hit.transform.GetComponentInParent<Enemy>().Health(minDamgae * 2);
                }
            }
        }
        // 抛壳
        Instantiate(casingPrefab,CasingBulletSpawnPoint.transform.position,CasingBulletSpawnPoint.transform.rotation);

        // 根据是否装有消音器，播放不同的子弹发射音效
        mainAudioSource.clip = IS_SILENCER ? soundClips.silencerShootSound : soundClips.shootSound;
        mainAudioSource.Play();

        // 重置计时器
        fireTimer = 0f;
        // 累减子弹数
        currentBullet--;
        // 更新UI
        UpdateAmmoUI();
        // 准心在射击时扩大
        StartCoroutine(ShootCross());
    }

    // 控制开火灯光
    public IEnumerator MuzzleFlashLight()
    {
        // 激活灯光
        muzzleflashLight.enabled = true;
        // 等待设定秒数
        yield return new WaitForSeconds(lightDuration);
        // 隐藏灯光
        muzzleflashLight.enabled = false;

    }

    // 更新子弹的UI
    public void UpdateAmmoUI()
    {
        // 更新子弹剩余量的UI
        ammoTextUI.text = currentBullet + "/" + bulletLeft;
        // 更新射击模式UI
        shootModeTextUI.text = shootModeName;
    }

    // 播放不同的换弹动画
    public override void DoReloadAnimation()
    {
        // 当没有备弹时或弹药充足时退出执行
        if (bulletLeft <= 0 || currentBullet >= bulletMag) return;

        // 判断是否是霰弹枪
        if (IS_SHOTGUN || IS_SNIPER)
        {
            // 触发霰弹枪换弹动画
            animator.SetTrigger("shotgun_reload");
        }
        // 弹夹中有子弹的情况
        else if (currentBullet > 0 && bulletLeft > 0)
        {
            // 播放动画
            animator.Play("reload_ammo_left", 0, 0);
            // 换弹
            Reload();
            // 播放换弹声音
            mainAudioSource.clip = soundClips.reloadSoundAmmotLeft;
            mainAudioSource.Play();
        }
        // 弹夹中没有子弹的情况
        else if(currentBullet == 0 && bulletLeft > 0)
        {
            // 播放动画
            animator.Play("reload_out_of_ammo", 0, 0);
            // 换弹
            Reload();
            // 播放换弹声音
            mainAudioSource.clip = soundClips.reloadSoundOutOfAmmo;
            mainAudioSource.Play();
        }
    }
    // 装填弹药的逻辑，在DoReloadAnimation()调用
    public override void Reload()
    {
        // 计算需要填充的子弹数
        int bulletToLoad = bulletMag - currentBullet;
        // 计算备弹扣除的子弹数
        int bulletToReduce =  bulletLeft >= bulletToLoad ? bulletToLoad : bulletLeft;
        // 扣除备弹数
        bulletLeft -= bulletToReduce;
        // 增加弹夹中的子弹数
        currentBullet += bulletToReduce;
        // 更新UI
        UpdateAmmoUI();
    }

    // 霰弹枪装填弹药的逻辑,在Animator.ReloadAmmoState()中调用
    public void ShootGunReload()
    {
        // 当没有备弹时退出执行
        if (bulletLeft <= 0) return;
        // 如果子弹没有被填满
        if (currentBullet < bulletMag)
        {
            // 添加子弹数
            currentBullet++;
            bulletLeft--;
            // 更新UI
            UpdateAmmoUI();
        }
    }

    // 进入瞄准，隐藏准心，摄像机视野变近
    public override void AimIn()
    {
        // 隐藏准心
        for(int i = 0; i < crossQuarterImages.Length; i++)
        {
            crossQuarterImages[i].gameObject.SetActive(false);
        }

        // 狙击枪瞄准时，改变GunCamera视野和瞄准镜的颜色
        if (IS_SNIPER)
        {
            // 改变瞄准镜颜色
            scopeRenderMaterial.color = defaultColor;
            // 改变GunCamera视野
            StartCoroutine(CameraView(gunCamera, 25));
        }

        // 摄像机视野变近
        StartCoroutine(CameraView(mainCamera,30));
        // 播放瞄准声音
        mainAudioSource.clip = soundClips.aimSound;
        mainAudioSource.Play();
    }

    // 退出瞄准，显示准心，摄像机视野恢复
    public override void AimOut()
    {
        // 狙击枪瞄准时，改回GunCamera视野和瞄准镜的颜色
        if (IS_SNIPER)
        {
            // 改变瞄准镜颜色
            scopeRenderMaterial.color = fadeColor;
            // 改变GunCamera视野
            StartCoroutine(CameraView(gunCamera, 50));
        }
        else {
            // 非狙击枪显示准心
            for (int i = 0; i < crossQuarterImages.Length; i++)
            {
                crossQuarterImages[i].gameObject.SetActive(true);
            }
        }

        // 摄像机视野变远
        StartCoroutine(CameraView(mainCamera, 60));
        // 播放瞄准声音
        mainAudioSource.clip = soundClips.aimSound;
        mainAudioSource.Play();
    }

    // 调整摄像机视野
    public IEnumerator CameraView(Camera camera,int targetView)
    {
        // 容差值
        const float tolerance = 5f;
        // 每帧增加值
        float viewAdditive = 200f;

        // 判断摄像机视野变近还是变远
        while (Mathf.Abs(camera.fieldOfView - targetView) > tolerance)
        {
            if (camera.fieldOfView > targetView)
            {
                // 调整摄像机视野
                camera.fieldOfView -= viewAdditive * Time.deltaTime;
            }
            else
            {
                // 调整摄像机视野
                camera.fieldOfView += viewAdditive * Time.deltaTime;
            }
            // 等待一帧
            yield return null;
        }
        // 精确调整
        camera.fieldOfView = targetView;
    }


    // 根据准心的大小，增减准心的开合度
    public override void ExpandingCrossUpdate(float expanDegree)
    {
        if(currentExpanedDegree < expanDegree - 0.75f)
        {
            ExpandCross(Time.deltaTime * 50);
        }
        else if(currentExpanedDegree > expanDegree + 0.75f)
        {
            ExpandCross(Time.deltaTime * -200);
        } 
    }

    // 改变并记录当前准心的开合度
    public void ExpandCross(float add)
    {
        if (currentExpanedDegree < maxCrossDegree || add <= 0)
        {
            // 左准心
            crossQuarterImages[0].transform.localPosition -= new Vector3(add, 0, 0);
            // 右准心
            crossQuarterImages[1].transform.localPosition += new Vector3(add, 0, 0);
            // 上准心
            crossQuarterImages[2].transform.localPosition += new Vector3(0, add, 0);
            // 左准心
            crossQuarterImages[3].transform.localPosition -= new Vector3(0, add, 0);

            // 记录当前开合度
            currentExpanedDegree += add;
        }
    }

    // 调用ExpendCross()射击瞬间增大准心，5次每帧
    public IEnumerator ShootCross()
    {
        yield return null;
        for(int i = 0; i < 5; i++)
        {
            ExpandCross(Time.deltaTime * 200);
        }
    }
}