using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRange : MonoBehaviour
{
    // 声名敌人主程序
    private Enemy enemy;

    private void Awake()
    {
        // 初始化敌人主程序
        enemy = gameObject.GetComponentInParent<Enemy>();
    }

    // 敌人攻击范围判定
    public void OnTriggerStay(Collider other)
    {
        // 攻击列表里剔除子弹
        if (!enemy.attackList.Contains(other.transform) && !enemy.isDead && other.tag == "Player")
        {
            // 如果玩家不在潜行且不在开枪，在很短的距离内才能发现
            Player player = other.GetComponent<Player>();
            Weapon_AutomaticGun weapon = other.GetComponentInChildren<Weapon_AutomaticGun>() == null ? null : other.GetComponentInChildren<Weapon_AutomaticGun>();

            // 判断玩家是否持枪
            if (weapon == null)
            {
                if (!player.isCrouching && (player.moveDirection.x != 0 || player.moveDirection.z != 0) || Vector3.Distance(transform.position, other.transform.position) <= enemy.discoverCrouchPlayerDistance)
                {
                    // 添加至攻击列表
                    enemy.attackList.Add(other.transform);
                    // 播放咆哮动画
                    enemy.animState = 4;
                }
            }
            else
            {
                if (!player.isCrouching && (player.moveDirection.x != 0 || player.moveDirection.z != 0) || weapon.muzzleflashLight.enabled && !weapon.IS_SILENCER || Vector3.Distance(transform.position, other.transform.position) <= enemy.discoverCrouchPlayerDistance)
                {
                    // 添加至攻击列表
                    enemy.attackList.Add(other.transform);
                    // 播放咆哮动画
                    enemy.animState = 4;
                }
            }
        }
    }

    // 敌人攻击范围判定
    public void OnTriggerExit(Collider other)
    {
        // 移除物体
        enemy.attackList.Remove(other.transform);
    }
}
