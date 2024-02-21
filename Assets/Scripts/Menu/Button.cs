using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // 文本组件
    private Text text;
    // 音频源
    private AudioSource audioSource;
    [Tooltip("鼠标悬停在按钮上时发出的声音")] public AudioClip choosAudio;
    

    private void Awake()
    {
        // 获取组件
        text = GetComponentInChildren<Text>();
        audioSource = GetComponent<AudioSource>();
        // 初始化颜色
        text.color = new Color(0.75f,0.75f,0.75f,255);
    }

    // 当鼠标进入按钮区域时调用
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 渐变更换文本颜色
        StartCoroutine(ChangeColor(text, Color.white, 0.2f));
        // 播放选择声音
        audioSource.clip = choosAudio;
        audioSource.Play();
    }

    // 当鼠标离开按钮区域时调用
    public void OnPointerExit(PointerEventData eventData)
    {
        // 渐变更换文本颜色
        StartCoroutine(ChangeColor(text, new Color(0.75f,0.75f,0.75f,255), 0.2f));
    }

    /// <summary>
    /// 渐变更改颜色
    /// </summary>
    /// <param name="graphic">要更改颜色的Graphic组件</param>
    /// <param name="color">目标颜色</param>
    /// <param name="time">过渡时间</param>
    private IEnumerator ChangeColor(Graphic graphic, Color targetColor, float time)
    {
        // 初始化颜色
        float timer = 0;
        Color startColor = graphic.color;
        // 循环渐变
        while (timer < time)
        {
            // 更新计时器
            timer += Time.deltaTime;
            // 更新颜色
            graphic.color = Color.Lerp(startColor, targetColor, timer / time);
            yield return null;
        }
        // 确定最终颜色
        graphic.color = targetColor;
    }
}