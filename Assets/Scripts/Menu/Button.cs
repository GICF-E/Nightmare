using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // 文本组件
    private Text text;
    // 按钮的图片组件
    private Image buttonImage;
    // 音频源
    private AudioSource audioSource;
    [Tooltip("鼠标悬停在按钮上时发出的声音")] public AudioClip choosAudio;
    

    private void Awake()
    {
        // 获取组件
        text = GetComponentInChildren<Text>();
        buttonImage = GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
        // 初始化颜色
        buttonImage.color = Color.clear;
        text.color = Color.white;
    }

    // 当鼠标进入按钮区域时调用
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 渐变更改按钮颜色
        StartCoroutine(ChangeColor(buttonImage, Color.white, 0.2f));
        // 渐变更换文本颜色
        StartCoroutine(ChangeColor(text, Color.black, 0.2f));
        // 播放选择声音
        audioSource.clip = choosAudio;
        audioSource.Play();
    }

    // 当鼠标离开按钮区域时调用
    public void OnPointerExit(PointerEventData eventData)
    {
        // 渐变恢复按钮的默认颜色
        StartCoroutine(ChangeColor(buttonImage, Color.clear, 0.2f));
        // 渐变更换文本颜色
        StartCoroutine(ChangeColor(text, Color.white, 0.2f));
    }

    /// <summary>
    /// 渐变更改颜色
    /// </summary>
    /// <param name="graphic">要更改颜色的Graphic组件</param>
    /// <param name="color">目标颜色</param>
    /// <param name="time">过渡时间</param>
    private IEnumerator ChangeColor(Graphic graphic, Color targetColor, float time)
    {
        float timer = 0;
        Color startColor = graphic.color;
        while (timer < time)
        {
            timer += Time.deltaTime;
            graphic.color = Color.Lerp(startColor, targetColor, timer / time);
            yield return null;
        }
        graphic.color = targetColor;
    }
}