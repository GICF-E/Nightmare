using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRange : MonoBehaviour
{
    // 声名敌人主程序
    private Enemy enemy;
    // 玩家对象脚本
    private Player player;

    private void Awake()
    {
        // 初始化敌人主程序
        enemy = gameObject.GetComponentInParent<Enemy>();
    }

    private void Start()
    {
        // 获取玩家对象脚本
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    private void Update()
    {
        // 每帧判断玩家是否被全图标记
        if (player.isMarked && enemy.attackList.Count == 0)
        {
            // 添加至攻击列表
            enemy.attackList.Add(player.transform);
            // 播放咆哮动画
            enemy.animState = 4;
        }
    }

    // 敌人攻击范围判定
    public void OnTriggerStay(Collider other)
    {
        // 攻击列表里剔除其他物体
        if (!enemy.attackList.Contains(other.transform) && !enemy.isDead && other.tag == "Player" && !player.isMarked)
        {
            // 如果玩家不在潜行且不在开枪，在很短的距离内才能发现
            Player player = other.GetComponent<Player>();
            Weapon_AutomaticGun weapon = other.GetComponentInChildren<Weapon_AutomaticGun>() == null ? null : other.GetComponentInChildren<Weapon_AutomaticGun>();

            // 判断玩家是否持枪
            if (weapon == null)
            {
                if (!player.isCrouching && (player.moveDirection.x != 0 || player.moveDirection.z != 0) || Vector3.Distance(transform.position, other.transform.position) <= enemy.discoverCrouchPlayerDistance || (enemy.currentHealth <= enemy.enemyHealth * 0.7f && enemy.isDamage))
                {
                    // 添加至攻击列表
                    enemy.attackList.Add(other.transform);
                    // 播放咆哮动画
                    enemy.animState = 4;
                }
            }
            else
            {
                if (!player.isCrouching && (player.moveDirection.x != 0 || player.moveDirection.z != 0) || weapon.muzzleflashLight.enabled && !weapon.IS_SILENCER || Vector3.Distance(transform.position, other.transform.position) <= enemy.discoverCrouchPlayerDistance || (enemy.currentHealth <= enemy.enemyHealth * 0.7f && enemy.isDamage))
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
        if(!player.isMarked) enemy.attackList.Remove(other.transform);
    }
}
