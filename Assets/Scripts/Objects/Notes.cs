using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public abstract class Notes : MonoBehaviour
{
    [Header("属性")]
    [Tooltip("文本文件")] public TextAsset textFile;
    [Tooltip("文本内容")] protected string[] texts;
    [Tooltip("是否有纸条被查看")] private bool isViewNotes;

    [Header("音效")]
    [Tooltip("拾取音效")] public AudioClip viewSound;

    [Header("引用")]
    [Tooltip("玩家代码")] protected Player player;
    [Tooltip("玩家查看视图")] protected GameObject playerNotesUI;
    [Tooltip("玩家文本UI")] protected Text playerNotesText;
    [Tooltip("拾取提示UI")] protected GameObject notesCanvas;

    private void Awake() {
        // 从文件中提取文本
        texts = textFile.text.Split(new string[] { "----------" }, System.StringSplitOptions.RemoveEmptyEntries);
        // 寻找玩家代码和UI
        player = GameObject.Find("Player").GetComponent<Player>();
        playerNotesUI = GameObject.Find("Player/Canvas/Notes");
        playerNotesText = GameObject.Find("Player/Canvas/Notes/Scroll View/Viewport/Content/Notes_Text").GetComponent<Text>();
        // 遍历所有子物体
        foreach (Transform child in transform) {
            if (child.name == "Canvas") {
                // 找到名为 "Canvas" 的子物体并赋值给 UI 变量
                notesCanvas = child.gameObject;
                // 找到后停止遍历
                break;
            }
        }
        // 设置 Rigidbody 使其不参与物理计算
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    private void Start() {
        isViewNotes = false;
        notesCanvas.SetActive(false);
        playerNotesUI.SetActive(false);
    }

    private void OnTriggerStay(Collider other){
        // 如果玩家朝向对象并按下拾取按键
        if(player.playerInput.actions["Interaction"].triggered && other.tag == "ObjectCollider" && !isViewNotes){
            // 播放拾取声音
            player.auxiliaryAudioSource.clip = viewSound;
            player.auxiliaryAudioSource.loop = false;
            player.auxiliaryAudioSource.Play();
            // 启用文本UI
            playerNotesUI.SetActive(true);
            // 暂停玩家声音
            player.movementAudioSource.Pause();
            // 解除光标锁定
            Cursor.lockState = CursorLockMode.Confined;
            // 更改变量状态
            isViewNotes = true;
            // 停止玩家移动
            player.canMove = false;
        }
        // 如果玩家按下退出按键或窗口已经被关闭
        if((player.playerInput.actions["Submit"].triggered || !playerNotesUI.activeSelf) && isViewNotes){
            // 播放拾取声音
            player.auxiliaryAudioSource.clip = viewSound;
            player.auxiliaryAudioSource.loop = false;
            player.auxiliaryAudioSource.Play();
            // 禁用文本UI
            playerNotesUI.SetActive(false);
            // 恢复变量状态
            isViewNotes = false;
            // 锁定光标
            Cursor.lockState = CursorLockMode.Locked;
            // 恢复玩家移动
            player.canMove = true;
        }
    }
}
