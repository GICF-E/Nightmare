using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// 人物武器切换、添加、去除
/// </summary>
public class Inventory : MonoBehaviour
{
    // 存储枪械的列表
    public List<GameObject> weapons = new List<GameObject>();
    // 存储枪械模型的列表
    public List<GameObject> gunsModel = new List<GameObject>();
    // 当前启用武器的编号
    public int currentWeaponID = -1;
    // 是否使用数字键盘切换
    private bool isNumberSwitch;
    // 切换间隔时间
    private float switchTime = 1f;
    // 切换计时器
    private float timer = 1f;

    private void Start()
    {
        // 初始化数据
        currentWeaponID = -1;
        switchTime = 1f;
        timer = 1f;
    }

    private void Update()
    {
        // 进行计时操作
        timer += Time.deltaTime;
        // 执行武器编号更新
        ChangeCurrentWeaponID();
    }

    // 更新武器编号
    public void ChangeCurrentWeaponID()
    {
        // 判断是否达到时间标准
        if (timer < switchTime) return;

        // 鼠标的滚动实现武器切换（-0.1～0.1）
        if(Input.GetAxis("Mouse ScrollWheel") < -0.05)
        {
            // 下一把武器
            ChangeWeapon(currentWeaponID + 1);
            isNumberSwitch = false;
        }
        else if(Input.GetAxis("Mouse ScrollWheel") > 0.05)
        {
            // 上一把武器
            ChangeWeapon(currentWeaponID - 1);
            isNumberSwitch = false;
        }

        // 数字键盘实现武器切换
        for(int i = 0; i < 10; i++)
        {
            // 判断按下了第几个数字键
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                // 判断数字是否大于总武器数量
                if (i > weapons.Count) break;
                // 切换武器
                ChangeWeapon(i - 1);
                isNumberSwitch = true;
            }
        }
    }

    // 武器切换
    public void ChangeWeapon(int WeaponID)
    {
        // 如果武器库里没有枪，退出函数
        if (weapons.Count == 0) return;

        // 重置计时器
        timer = 0f;

        // 判断是否是第一个或最后一个武器，头尾相接
        if (!isNumberSwitch) {
            if (WeaponID > weapons.Count - 1) WeaponID = 0;
            else if (WeaponID <= -1) WeaponID = weapons.Count - 1;
        }
        else
        {
            if (WeaponID > weapons.Count - 1) WeaponID = weapons.Count - 1;
        }
        // 更新武器索引
        currentWeaponID = WeaponID;
        // 遍历武器库
        for(int i = 0; i < weapons.Count; i++)
        {
            // 先隐藏武器，以便显示takeout动画
            weapons[i].gameObject.SetActive(false);
            // 根据武器编号显示对应的武器
            if(WeaponID == i)
            {
                weapons[i].gameObject.SetActive(true);
            }
        }
    }

    // 添加武器
    public void AddWeapon(GameObject destoryObject, GameObject addWeapon)
    {
        // 判断是否已有这把武器
        if (weapons.Contains(addWeapon))
        {
            // 填充满备弹
            addWeapon.GetComponent<Weapon_AutomaticGun>().bulletLeft = addWeapon.GetComponent<Weapon_AutomaticGun>().bulletMag * 4;
            // 切换至当前武器
            ChangeWeapon(weapons.IndexOf(addWeapon));
            // 销毁武器模型
            Destroy(destoryObject, 0f);
            // 更新UI
            addWeapon.GetComponent<Weapon_AutomaticGun>().UpdateAmmoUI();
        }
        else
        {
            // 设定武器上限
            if (weapons.Count >= 3) return;
            // 添加武器
            weapons.Add(addWeapon);
            // 切换至新武器
            ChangeWeapon(weapons.IndexOf(addWeapon));
            // 销毁武器模型
            Destroy(destoryObject, 0f);
        }
    }

    // 丢弃武器
    public void ThrowWeapon(int itemID, GameObject throwWeapon)
    {
        // 判断武器库里是否有丢弃武器
        if (!weapons.Contains(throwWeapon) || weapons.Count == 0) return;
        else
        {
            // 生成这把枪械的实体
            // 根据是否是狙击枪使用不同的生成模式
            Instantiate(gunsModel[itemID], GameObject.Find("Player").transform.position + GameObject.Find("Player").transform.forward * 2, Quaternion.Euler(GameObject.Find("Player").transform.eulerAngles.x, GameObject.Find("Player").transform.eulerAngles.y + 90, GameObject.Find("Player").transform.eulerAngles.z));
            // 遍历武器库
            for (int i = 0; i < weapons.Count; i++)
            {
                // 隐藏所有武器
                weapons[i].gameObject.SetActive(false);
            }
            // 在集合中删除武器
            weapons.Remove(throwWeapon);
        }
    }
}
