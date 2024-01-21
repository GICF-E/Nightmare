using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于拾取武器的类
/// </summary>
public class PickUpItem : MonoBehaviour
{
    [Tooltip("武器编号")] public int itemID;
    [Tooltip("武器编号")] private GameObject weaponModel;

    // 当物体被接触时
    private void OnTriggerEnter(Collider other)
    {
        // 判断是否是玩家接触
        if (other.tag == "Player")
        {
            // 获取武器库下的对应武器对象
            weaponModel = GameObject.Find("Player/Main Camera/Inventory/").gameObject.transform.GetChild(itemID).gameObject;
            // 拾取武器
            weaponModel.GetComponentInParent<Inventory>().AddWeapon(gameObject, weaponModel);
        }
    }
}
