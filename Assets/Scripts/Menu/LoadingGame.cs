using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingGame : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("加载进度条")] public Slider slider;
    [Tooltip("遮挡面板")] public Image panel;
    [Tooltip("提示文本")] public Text loadText;

    private void Awake() {
        // 初始化
        slider.value = 0;
        panel.color = Color.black;
        loadText.text = "";
    }

    private void Start() {
        // 渐变更改遮挡面板的颜色
        StartCoroutine(LoadGame());
    }

    /// <summary>
    /// 加载主场景
    /// <summary>
    private IEnumerator LoadGame(){
        // 渐变关闭面板
        StartCoroutine(ChangeColor(panel, Color.clear, 2));
        // 开始异步加载
        AsyncOperation operation = SceneManager.LoadSceneAsync("Main Scene");
        // 不允许自动跳转场景
        operation.allowSceneActivation = false;
        // 判断是否加载完成
        while(!operation.isDone){
            // 更新进度条
            slider.value = operation.progress;
            // 进度是否大于90%
            if(operation.progress >= 0.9f){
                // 将进度条拉满
                slider.value = 1;
                // 更新文本
                loadText.text = "Press any key to continue";
                // 任意键按下跳转
                if(Input.anyKey) operation.allowSceneActivation = true;
            }
            yield return null;
        }
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
