using UnityEngine;

/// <summary>
/// 用于拾取武器的类
/// </summary>
public class PickUpItem : MonoBehaviour
{
    [Header("引用")]
    [Tooltip("拾取物体的声音")] public AudioClip pickUpSound;
    [Tooltip("玩家脚本")] private Player player;

    [Header("状态")]
    [Tooltip("玩家是否在注视")] private bool isLookingAt = false;

    [Header("属性")]
    [Tooltip("武器编号")] public int itemID;
    [Tooltip("提示UI")] private GameObject canvas;
    [Tooltip("武器编号")] private GameObject weaponModel;

    private void Awake()
    {
        // 初始化
        isLookingAt = false;
        // 遍历所有子物体
        foreach (Transform child in transform)
        {
            if (child.name == "Canvas")
            {
                // 找到名为 "Canvas" 的子物体并赋值给 UI 变量
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
        // 判断是否是玩家接触并且按下交互按键
        if (isLookingAt)
        {
            if (player.playerInput.actions["Interaction"].triggered)
            {
                // 播放拾取声音
                player.auxiliaryAudioSource.clip = pickUpSound;
                player.auxiliaryAudioSource.loop = false;
                player.auxiliaryAudioSource.Play();
                // 获取武器库下的对应武器对象
                weaponModel = GameObject.Find("Player/Main Camera/Inventory/").gameObject.transform.GetChild(itemID).gameObject;
                // 拾取武器
                weaponModel.GetComponentInParent<Inventory>().AddWeapon(gameObject, weaponModel);
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
