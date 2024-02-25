using System.Collections;
using UnityEngine;

public class Eyes : MonoBehaviour
{
    [Header("状态")]
    [Tooltip("是否发现玩家")] public bool isFoundPlayer = false;
    [Tooltip("玩家是否在注视")] private bool isLookingAt = false;

    [Header("引用")]
    [Tooltip("材质颜色")] private Material material;
    [Tooltip("灯光")] private Light pointLight;

    [Header("属性")]
    [Tooltip("移动速度")] public float speed;
    [Tooltip("一般颜色")] public Color normalColor;
    [Tooltip("变色后的颜色")] public Color changeColor;
    [Tooltip("玩家脚本")] private Player player;

    private void Awake()
    {
        // 初始化
        isFoundPlayer = false;
        isLookingAt = false;
        // 获取玩家属性
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        // 获取材质
        material = GetComponent<Renderer>().material;
        // 获取灯光
        pointLight = GetComponent<Light>();
    }

    private void Start()
    {
        // 更改材质颜色
        material.color = normalColor;
        // 更改材质自发光颜色
        material.SetColor("_EmissionColor", normalColor);
        // 更改灯光颜色
        pointLight.color = normalColor;
    }

    private void Update()
    {
        // 判断是否发现玩家
        if (isFoundPlayer)
        {
            // 向玩家移动
            StartCoroutine(MoveToPlayer());
            isFoundPlayer = false;
        }
        // 判断玩家是否在注视
        if(isLookingAt){
            // 判断玩家是否按下交互按键
            if(player.playerInput.actions["Interaction"].triggered){
                // 禁止移动
                player.canMove = false;
                // 更新状态
                player.isViewNotes = true;
                // 显示鼠标指针
                Cursor.lockState = CursorLockMode.None;
                // 打开输入面板
                StartCoroutine(player.OpenInputPanel(1f));
            }
        }
    }

    /// <summary>
    /// 持续向玩家移动
    /// </summary>
    private IEnumerator MoveToPlayer()
    {
        // 判断是否需要下降
        if (transform.position.y - player.transform.position.y > 1f)
        {
            // 判断与玩家的高度差
            while (transform.position.y - player.transform.position.y > 1f)
            {
                // 持续下降
                transform.position = new Vector3(transform.position.x, transform.position.y - speed * Time.deltaTime, transform.position.z);
                // 等待下一帧
                yield return null;
            }
            // 等待2秒
            yield return new WaitForSeconds(2f);
        }
        // 向玩家平面移动
        // 计算向玩家移动的方向向量
        Vector3 direction = (player.transform.position - transform.position).normalized;
        // 忽略Y轴的差异
        direction.y = 0;
        // 检测是否已经非常接近玩家
        while (Vector3.Distance(transform.position, player.transform.position) > 2.5f)
        {
            // 每帧更新位置，向玩家移动
            Vector3 targetPosition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed / 2 * Time.deltaTime);

            // 等待下一帧
            yield return null;
        }
    }

    /// <summary>
    /// 渐变更改材质灯光颜色
    /// </summary>
    private IEnumerator ChangeColor(Material material, Light light, Color startColor, Color targetColor, float time)
    {
        // 初始化颜色
        float timer = 0;
        material.color = startColor;
        light.color = startColor;
        // 循环渐变
        while (timer < time)
        {
            // 更新计时器
            timer += Time.deltaTime;
            // 更新材质颜色
            material.color = Color.Lerp(startColor, targetColor, timer / time);
            // 更改材质自发光颜色
            material.SetColor("_EmissionColor", Color.Lerp(startColor, targetColor, timer / time));
            // 更改灯光颜色
            light.color = Color.Lerp(startColor, targetColor, timer / time);
            yield return null;
        }
        // 确定最终颜色
        material.color = targetColor;
    }


    private void OnTriggerEnter(Collider other)
    {
        // 判断是否是玩家接触
        if (other.tag == "Player")
        {
            // 切换发现状态
            isFoundPlayer = true;
        }
        // 判断是否与玩家手臂接触
        if (other.tag == "ObjectCollider")
        {
            // 启用变色协程
            StartCoroutine(ChangeColor(material, pointLight, normalColor, changeColor, 1));
            // 更改查看状态
            isLookingAt = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 判断是否是玩家手臂离开
        if (other.tag == "ObjectCollider")
        {
            // 启用变色协程
            StartCoroutine(ChangeColor(material, pointLight, changeColor, normalColor, 1));
            // 更改查看状态
            isLookingAt = false;
        }
    }
}