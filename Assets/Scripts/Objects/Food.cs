using System.Collections;
using System.Collections.Generic;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public class Food : MonoBehaviour
{
    [Header("属性")]
    [Tooltip("单位时间恢复血量")] public float restoringHealthforUnitTime;
    [Tooltip("单位恢复时间")] public float restoringTimeBetween;
    [Tooltip("恢复次数")] public float restroingTimes;
    [Tooltip("拾取物体的声音")] public AudioClip pickUpSound;
    [Tooltip("咀嚼的声音")] public AudioClip chewSound;
    [Tooltip("玩家代码")] private Player player;
    [Tooltip("提示UI")] private GameObject canvas;

    private void Awake() {
        // 遍历所有子物体
        foreach (Transform child in transform) {
            if (child.name == "Canvas") {
                // 找到名为 "UI" 的子物体并赋值给 UI 变量
                canvas = child.gameObject;
                break; // 找到后停止遍历
            }
        }
    }

    private void Start() {
        player = GameObject.Find("Player").GetComponent<Player>();
        canvas.SetActive(false);
    }

    private void OnTriggerEnter(Collider other) {
        // 判断是否是玩家接触
        if (other.tag == "ObjectCollider"){
            // 启动UI
            canvas.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other) {
        // 关闭UI
        canvas.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        // 判断是否是玩家接触并且按下F按键
        if(Input.GetKeyDown(KeyCode.F) && other.tag == "ObjectCollider"){
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
