using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

/// <summary>
/// 用于状态切换即加载敌人的巡逻路线的类
/// </summary>
public class Enemy : MonoBehaviour
{
    // 导航系统
    public NavMeshAgent agent;
    // 动画状态机
    public Animator animator;

    [Header("导航路线")]
    [Tooltip("存放敌人不同路线")] public GameObject WayPointObj;
    [Tooltip("存放巡逻路线的每个巡逻点")] public List<Vector3> wayPoints = new List<Vector3>();
    [Tooltip("索引下标")] public int patrolIndex;
    [Tooltip("动画状态标识")] public int animState;// 0:Idle, 1:Walk , 2:Run, 3:Attack

    [Header("UI")]
    [Tooltip("敌人血条")] public Slider slider;
    [Tooltip("是否显示血条")] public bool isEnableSlider;

    [Header("敌人属性")]
    [Tooltip("敌人原有血量")] public float enemyHealth;
    [Tooltip("敌人实际血量")] public float currentHealth;
    [Tooltip("敌人的行走速度")] public float walkSpeed;
    [Tooltip("敌人的跑步速度")] public float runSpeed;
    [Tooltip("敌人实际移动速度")] public float currentSpeed;
    [Tooltip("敌人最小攻击的伤害值")] public float minAttackDamage;
    [Tooltip("敌人最大攻击的伤害值")] public float maxAttackDamage;
    [Tooltip("敌人是否死亡")] public bool isDead = false;
    [Tooltip("敌人是否巡逻")] public bool isPatrol = true;
    [Tooltip("敌人是否受到伤害")] public bool isDamage;
    [Tooltip("玩家潜行状态下可以发现玩家的距离")] public float discoverCrouchPlayerDistance;
    [Tooltip("敌人原始位置")] public Vector3 originalPosition;
    [Tooltip("敌人的攻击目标（玩家）")] public List<Transform> attackList = new List<Transform>();
    [Tooltip("攻击间隔时间")] public float attackRate;
    [Tooltip("下次攻击时间")] public float nextAttack = 0;
    [Tooltip("普通攻击距离")] public float attackRange;
    [Tooltip("攻击是否有特效")] public bool isAttackEffect;
    [Tooltip("敌人攻击的粒子效果")] public GameObject attackEffect;
    [Tooltip("敌人攻击的点")] public Transform hitPoint;

    // 皮肤渲染
    public SkinnedMeshRenderer meshRenderer;
    // 网格碰撞
    public MeshCollider meshCollider;

    // 采样间隔
    public int FrameInterval = 10;
    // 释放资源间隔
    private float _unloadResouceTime = 15f;
    private float _unloadTimer;
    private float _interval;

    // 存放敌人当前的状态
    public EnemyBaseState currentState;
    // 定义敌人巡逻状态声名对象
    public PatrolState patrolState;
    // 定义敌人攻击状态声名对象
    public AttackState attackState;
    // 获取玩家对象代码
    private Player player;

    private void Awake()
    {
        // 初始化变量
        agent = gameObject.GetComponent<NavMeshAgent>();
        animator = gameObject.GetComponent<Animator>();
        player = GameObject.Find("Player").gameObject.GetComponent<Player>();
        originalPosition = transform.position;
        nextAttack = attackRate;
        _interval = FrameInterval;
        patrolIndex = 0;
        // 初始化血量条
        isDead = false;
        slider.minValue = 0;
        slider.maxValue = enemyHealth;
        slider.value = enemyHealth;
        currentHealth = enemyHealth;

        // 根据玩家设置更改变量
        UpdateSettings();
    }

    private void Start()
    {
        if (isEnableSlider && !player.isMenuMode) slider.gameObject.SetActive(true);
        else slider.gameObject.SetActive(false);

        // 初始化状态对象
        patrolState = new PatrolState();
        attackState = new AttackState();

        // 游戏一开始就进入巡逻状态
        TransitionToState(patrolState);
    }


    private void Update()
    {
        // 根据敌人受伤状态增减移动速度
        if (isDamage)
        {
            // 确保减少后的速度大于等于0
            if (currentSpeed - 0.5f >= runSpeed * 0.2f) currentSpeed -= 0.5f;
            else currentSpeed = runSpeed * 0.2f;
            // 重置变量
            isDamage = false;
        }
        else
        {
            // 在没有收到增加的情况下增加移动速度
            if ((animState == 1 && currentSpeed + Time.deltaTime * 2 <= walkSpeed) || (animState == 2 && currentSpeed + Time.deltaTime * 2 <= runSpeed))
            {
                currentSpeed += Time.deltaTime * 2.5f;
            }
            else
            {
                currentSpeed = animState == 2 ? runSpeed : walkSpeed;
            }
        }

        // 只有非死亡状态执行操作
        if (!isDead)
        {
            // 更新动画状态机
            animator.SetInteger("state", animState);
            // 表示当前状态持续执行
            // 敌人移动方法要一直执行
            currentState.OnUpdate(this);
        }
        else
        {
            // 隐藏血量条
            slider.gameObject.SetActive(false);
            // 如果死亡，停止移动
            agent.isStopped = true;
        }

        // 判断玩家到敌人的距离
        if (Vector3.Distance(transform.position, player.transform.position) < 200f)
        {
            // 更新Mesh Collider
            if (_interval >= FrameInterval)
            {
                _interval = 0;
                Mesh colliderMesh = new Mesh();
                // 更新mesh
                meshRenderer.BakeMesh(colliderMesh);
                meshCollider.sharedMesh = null;
                // 将新的mesh赋给meshcollider
                meshCollider.sharedMesh = colliderMesh;
                colliderMesh = null;
            }
            else
            {
                _interval += Time.deltaTime;
            }

            // 定时释放资源，防止内存泄露
            if (_unloadTimer < _unloadResouceTime)
            {
                _unloadTimer += Time.deltaTime;
            }
            else
            {
                Resources.UnloadUnusedAssets();
                _unloadTimer = 0;

            }
        }
    }

    // 敌人向导航点移动
    public void MoveToTarget()
    {
        // 判断敌人有没有扫描到玩家
        if (attackList.Count == 0)
        {
            // 敌人没有扫描到玩家，执行巡逻
            // 向巡逻点移动
            agent.SetDestination(wayPoints[patrolIndex]);
        }
        else
        {
            // 敌人扫描到玩家，向玩家移动
            agent.SetDestination(attackList[0].transform.position);
        }
    }

    // 加载导航路线
    public void LoadPath(GameObject go)
    {
        // 加载路线前清空List
        wayPoints.Clear();
        // 遍历路线的所有当航点位置信息并加载到wayPoints
        foreach (Transform T in go.transform)
        {
            wayPoints.Add(T.position);
        }
    }

    // 切换敌人的状态
    public void TransitionToState(EnemyBaseState state)
    {
        currentState = state;
        currentState.EnemyState(this);
    }

    // 敌人受到伤害并扣除血量
    public void Health(float damage)
    {
        // 如果敌人已经死亡，不执行方法
        if (isDead) return;
        // 更新UI即血量
        currentHealth -= damage;
        slider.value = currentHealth;
        // 更新变量
        isDamage = true;
        // 判断是否死亡
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
            animator.SetTrigger("dying");
        }
    }

    // 敌人的攻击逻辑
    public void HurtPlayer()
    {
        // 生成粒子效果
        if (isAttackEffect) Instantiate(attackEffect, hitPoint.position, Quaternion.identity);
        if (attackList.Count > 0)
        {
            if (Vector3.Distance(hitPoint.position, attackList[0].transform.position) <= attackRange)
            {
                // 执行扣血
                player.PlayerDamageHealth(Random.Range(minAttackDamage, maxAttackDamage));
            }
        }
    }

    // 更新设置
    public void UpdateSettings()
    {
        isEnableSlider = PlayerPrefs.GetInt("isEnableSlider") == 1 ? true : false;
        if (isEnableSlider && !player.isMenuMode) slider.gameObject.SetActive(true);
        else slider.gameObject.SetActive(false);
    }
}
