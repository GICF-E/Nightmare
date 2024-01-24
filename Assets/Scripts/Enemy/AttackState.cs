using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人进入攻击状态
/// </summary>
public class AttackState : EnemyBaseState
{
    // 获取玩家对象代码
    private Player player;

    public override void EnemyState(Enemy enemy)
    {
        // 初始化
        player = GameObject.Find("Player").gameObject.GetComponent<Player>();
    }

    public override void OnUpdate(Enemy enemy)
    {
        // 如果敌人已经到达攻击对象附近了
        if (enemy.attackList.Count > 0)
        {
            if (Vector3.Distance(enemy.transform.position, enemy.attackList[0].transform.position) <= enemy.attackRange)
            {
                // 进行攻击时间累加
                enemy.nextAttack += Time.deltaTime;
                // 如果达到要求攻击时间间隔
                if(enemy.nextAttack >= enemy.attackRate)
                {
                    // 运行攻击动画
                    enemy.animator.SetInteger("state", 3);
                    enemy.nextAttack = 0;
                }
                else
                {
                    // 没有满足攻击间隔的情况下播放站立动画
                    enemy.animator.SetInteger("state", 0);
                }
            }
            else
            {
                // 如果不达到攻击位置，交还给巡逻逻辑
                enemy.TransitionToState(enemy.patrolState);
            }
        }
    }
}
