using UnityEngine;
using System.Collections;

public class PlaySoundScript : StateMachineBehaviour
{

    [Header("Audio Clips")]
    public AudioClip soundClip;
    [Tooltip("是否循环")] public bool isLoop;
    [Tooltip("等待时间")] public float waitTime;
    [Tooltip("播放音量")] public float volume = 0.1f;
    // 是否已经播放过
    private bool isPlayed;
    // AudioSource 组件
    public AudioSource audioSource;
    // 用于累计时间的变量
    private float timer = 0f;

    // 当状态开始时重置计时器
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 获取 AudioSource 组件
        audioSource = animator.gameObject.GetComponent<AudioSource>();
        audioSource.Pause();
        isPlayed = false;
        timer = 0f;
    }

    // 在状态更新时检查计时器
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 累加时间
        timer += Time.deltaTime;

        // 检查是否达到等待时间
        if (timer > waitTime && !isPlayed)
        {
            // 如果尚未播放音频，则开始播放
            if (!audioSource.isPlaying)
            {
                audioSource.clip = soundClip;
                audioSource.loop = isLoop;
                audioSource.volume = volume;
                audioSource.Play();
                isPlayed = true;
            }
        }
    }
}
