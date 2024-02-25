using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Check_box : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // 文本组件
    private Text text;
    // 按钮的图片组件
    private Image image;
    // 音频源
    private AudioSource audioSource;
    // 按钮是否被按下
    private bool isPressed;
    [Tooltip("按钮默认颜色")] public Color defaultColor;
    [Tooltip("鼠标悬停在按钮上时的颜色")] public Color hoverColor;
    [Tooltip("鼠标悬停在按钮上时发出的声音")] public AudioClip choosAudio;
    

    private void Awake()
    {
        // 获取组件
        text = GetComponentInChildren<Text>();
        image = GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
        // 初始化颜色
        image.color = defaultColor;
        text.color = Color.white;
        // 加载设置
        isPressed = PlayerPrefs.GetInt(transform.name) == 1;
    }

    private void Start() {
        if(isPressed) StartCoroutine(ChangeColor(image, Color.white, 0.2f));
    }

    // 当鼠标进入按钮区域时调用
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 渐变更改按钮颜色
        StartCoroutine(ChangeColor(image, hoverColor, 0.2f));
    }

    // 当鼠标离开按钮区域时调用
    public void OnPointerExit(PointerEventData eventData)
    {
        // 渐变恢复按钮的默认颜色
        if(!isPressed) StartCoroutine(ChangeColor(image, defaultColor, 0.2f));
        else StartCoroutine(ChangeColor(image, Color.white, 0.2f));
    }

    /// <summary>
    /// 选中复选框执行的操作
    /// <summary>
    public void OnCheck(){
        // 根据按钮状态切换颜色
        if(isPressed) StartCoroutine(ChangeColor(image, defaultColor, 0.2f));
        else StartCoroutine(ChangeColor(image, Color.white, 0.2f));
        // 播放选择声音
        audioSource.clip = choosAudio;
        audioSource.Play();
        // 切换按钮状态
        isPressed = !isPressed;
        // 记录切换内容
        PlayerPrefs.SetInt(transform.name, isPressed ? 1 : 0);
    }

    /// <summary>
    /// 渐变更改颜色
    /// </summary>
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