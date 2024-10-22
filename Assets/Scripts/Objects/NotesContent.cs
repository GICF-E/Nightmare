using UnityEditor;
using UnityEngine;

public class NotesContent : Notes
{
    [Header("属性")]
    [Tooltip("文本内容编号")] public int ID;
    [TextArea] [SerializeField] [Tooltip("文本内容预览")] private string previewText;

    private void OnTriggerEnter(Collider other) {
        // 判断是否是玩家接触
        if (other.tag == "ObjectCollider"){
            // 显示提示UI
            notesCanvas.SetActive(true);
            // 更改查看状态
            isLookingAt = true;
            // 更新玩家文本UI
            playerNotesText.text = texts[ID - 1];
        }
    }

    private void OnTriggerExit(Collider other) {
        // 判断是否是玩家接触
        if (other.tag == "ObjectCollider"){
            // 关闭UI
            notesCanvas.SetActive(false);
            // 更改查看状态
            isLookingAt = false;
            // 更新玩家文本UI
            playerNotesText.text = "";
        }
    }

    // 当Inspector中的值发生变化时调用
    #if UNITY_EDITOR
    void OnValidate()
    {
        // 从文件中提取文本
        texts = textFile.text.Split(new string[] { "----------" }, System.StringSplitOptions.RemoveEmptyEntries);
        // 确保textIndex不会超出texts数组的范围
        if (texts != null && ID > 0 && ID <= texts.Length)
        {
            previewText = texts[ID - 1];
        }
        else
        {
            previewText = "输入超限";
        }

        // 更新Inspector显示
        EditorUtility.SetDirty(this);
    }
    #endif
}
