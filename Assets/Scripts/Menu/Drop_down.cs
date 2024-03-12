using UnityEngine;
using UnityEngine.UI;

public class Drop_down : MonoBehaviour
{
    // Dropdown组件
    private Dropdown dropdown;
    // 选择音频
    public AudioClip selectAudioClip;
    // 音频源
    private AudioSource audioSource;

    private void Awake()
    {
        // 初始化
        dropdown = GetComponent<Dropdown>();
        audioSource = GetComponent<AudioSource>();
        // 初始化Dropdown的选中项
        InitDropdownSelection();
    }

    private void Start()
    {
        // 为Dropdown的值变化添加监听器
        dropdown.onValueChanged.AddListener(delegate
        {
            DropdownValueChanged(dropdown);
        });
    }

    // 初始化Dropdown的选中项
    private void InitDropdownSelection()
    {
        // 读取保存的选中项
        string selected = PlayerPrefs.GetString(transform.name, dropdown.options[0].text);
        // 根据选中项设置Dropdown的选中项
        int index = dropdown.options.FindIndex(option => option.text == selected);
        // 如果找到选项，设置Dropdown的选中项
        if (index != -1) dropdown.value = index;
    }

    // Dropdown值变化时调用
    private void DropdownValueChanged(Dropdown change)
    {
        // 播放音频
        audioSource.clip = selectAudioClip;
        audioSource.Play();
        // 渐变恢复按钮的默认颜色
        //StartCoroutine(ChangeColor(image, Color.white, 0.2f));
        // 保存选中的选项
        PlayerPrefs.SetString(dropdown.gameObject.name, dropdown.options[dropdown.value].text);
    }
}
