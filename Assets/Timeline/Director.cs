using UnityEngine;
using UnityEngine.Playables;

public class LoopTimeline : MonoBehaviour
{
    // 时间线
    private PlayableDirector director;
    // 玩家代码
    private Player player;

    private void Awake(){
        // 获取组件和玩家代码
        director = GetComponent<PlayableDirector>();
        player = GameObject.Find("Player").GetComponent<Player>();
        // 启动玩家菜单模式
        player.isMenuMode = true;
    }

    private void Update() {
        // 禁用玩家的移动
        player.canMove = false;
    }

    /// <summary>
    /// 重新开始播放时间线
    /// </summary>
    public void ResetTimeline()
    {
        // 重置时间
        director.time = 0;
        // 播放时间线
        director.Play();
    }
}