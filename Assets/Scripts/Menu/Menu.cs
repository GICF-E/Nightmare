using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

public class Menu : MonoBehaviour
{
    [Header("后处理系统")]
    [Tooltip("后处理体积")] public PostProcessVolume volume;
    [Tooltip("暗角")] private Vignette vignette;

    [Header("UI")]
    [Tooltip("菜单")] public GameObject menu;
    [Tooltip("设置")] public GameObject setting;
    [Tooltip("遮挡面板")] public GameObject panel;

    private void Awake() {
        // 初始化
        menu.SetActive(true);
        setting.SetActive(false);
        volume.profile.TryGetSettings(out vignette);
        vignette.intensity.value = 0.25f;
        panel.GetComponent<Graphic>().color = Color.clear;
        panel.SetActive(false);
    }

    // 显示设置界面
    public void ShowSetting() {
        // 显示设置界面并禁用菜单
        menu.SetActive(false);
        setting.SetActive(true);
    }

    // 关闭设置界面
    public void CloseSetting() {
        // 关闭设置界面并显示菜单
        menu.SetActive(true);
        setting.SetActive(false);
    }

    // 开始游戏
    public void StartGame() {
        // 渐变更改Panel颜色
        StartCoroutine(StartGame(panel.GetComponent<Graphic>(), Color.black, 2));
    }

    // 退出游戏
    public void QuitGame() {
        // 退出游戏程序
        Application.Quit();
    }

    /// <summary>
    /// 渐变面板并开始游戏
    /// </summary>
    /// <param name="graphic">要更改颜色的Graphic组件</param>
    /// <param name="color">目标颜色</param>
    /// <param name="time">过渡时间</param>
    private IEnumerator StartGame(Graphic graphic, Color targetColor, float time)
    {
        panel.SetActive(true);
        float timer = 0;
        Color startColor = graphic.color;
        while (timer < time)
        {
            timer += Time.deltaTime;
            graphic.color = Color.Lerp(startColor, targetColor, timer / time);
            yield return null;
        }
        graphic.color = targetColor;
        // 更改后处理效果
        vignette.intensity.value = 0.125f;
        // 切换到加载场景
        SceneManager.LoadScene("Loading");
    }
}
