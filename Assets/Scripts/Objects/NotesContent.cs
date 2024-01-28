using System.Collections;
using System.Collections.Generic;
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
            // 更新玩家文本UI
            playerNotesText.text = texts[ID];
        }
    }

    private void OnTriggerExit(Collider other) {
        // 关闭UI
        notesCanvas.SetActive(false);
        // 判断是否是玩家接触
        if (other.tag == "ObjectCollider"){
            // 更新玩家文本UI
            playerNotesText.text = "";
        }
    }

    // 当Inspector中的值发生变化时调用
    #if UNITY_EDITOR
    void OnValidate()
    {
        // 确保textIndex不会超出texts数组的范围
        if (texts != null && ID >= 0 && ID < texts.Length)
        {
            previewText = texts[ID];
        }
        else
        {
            previewText = "";
        }

        // 更新Inspector显示
        EditorUtility.SetDirty(this);
    }
    #endif
}
