using UnityEngine;

public class Food : MonoBehaviour
{
    [Header("状态")]
    [Tooltip("玩家是否在注视")] private bool isLookingAt = false;

    [Header("引用")]
    [Tooltip("玩家脚本")] private Player player;
    [Tooltip("提示UI")] private GameObject canvas;

    [Header("属性")]
    [Tooltip("单位时间恢复血量")] public float restoringHealthforUnitTime;
    [Tooltip("单位恢复时间")] public float restoringTimeBetween;
    [Tooltip("恢复次数")] public float restroingTimes;
    [Tooltip("拾取物体的声音")] public AudioClip pickUpSound;
    [Tooltip("咀嚼的声音")] public AudioClip chewSound;

    private void Awake()
    {
        // 初始化
        isLookingAt = false;
        // 遍历所有子物体
        foreach (Transform child in transform)
        {
            if (child.name == "Canvas")
            {
                // 找到名为 "UI" 的子物体并赋值给 UI 变量
                canvas = child.gameObject;
                // 找到后停止遍历
                break;
            }
        }
    }

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        canvas.SetActive(false);
    }

    private void Update()
    {
        // 判断玩家是否注视并且按下交互按键
        if (isLookingAt)
        {
            if (player.playerInput.actions["Interaction"].triggered && player.addHealthCoroutine == null)
            {
                // 播放拾取声音
                player.auxiliaryAudioSource.clip = pickUpSound;
                player.auxiliaryAudioSource.loop = false;
                player.auxiliaryAudioSource.Play();
                // 恢复玩家生命值
                player.RestoringHealth(restroingTimes, restoringTimeBetween, restoringHealthforUnitTime, chewSound);
                // 自我销毁
                Destroy(gameObject, 0f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 判断是否是玩家接触
        if (other.tag == "ObjectCollider")
        {
            // 启动UI
            canvas.SetActive(true);
            // 更改查看状态
            isLookingAt = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 判断是否是玩家手臂离开
        if (other.tag == "ObjectCollider")
        {
            // 关闭UI
            canvas.SetActive(false);
            // 更改查看状态
            isLookingAt = false;
        }
    }
}
