using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人进入巡逻状态
/// </summary>
public class PatrolState : EnemyBaseState
{

    public override void EnemyState(Enemy enemy)
    {
        enemy.animState = 0;
        // 随机加载路线
        if(enemy.isPatrol) enemy.LoadPath(enemy.WayPointObj);
    }

    public override void OnUpdate(Enemy enemy)
    {
        // 获取当前动画状态信息
        AnimatorStateInfo stateInfo = enemy.animator.GetCurrentAnimatorStateInfo(0);

        // 如果敌人发现玩家
        if (enemy.attackList.Count > 0)
        {
            // 播放完咆哮动画前停止移动
            if (stateInfo.IsName("Roaring") && stateInfo.normalizedTime < 1)
            {
                // 停止移动
                enemy.agent.SetDestination(enemy.transform.position);
            }
            // 如果敌人已经到达攻击对象附近了
            if (Vector3.Distance(enemy.transform.position, enemy.attackList[0].transform.position) <= enemy.attackRange)
            {
                enemy.animState = 0;
                // 停止移动
                enemy.agent.SetDestination(enemy.transform.position);
                // 切换到攻击模式
                enemy.TransitionToState(enemy.attackState);
                return;
            }
        }

        // 播完Idle动画才能执行移动
        if ((stateInfo.normalizedTime >= 1 || enemy.attackList.Count != 0) && !enemy.isDead && enemy.attackList.Count == 0)
        {
            // 如果巡逻
            if (enemy.isPatrol)
            {
                // 播放行走动画
                enemy.animState = 1;
                // 调整移动速度
                enemy.agent.speed = enemy.currentSpeed;
                // 切换自动刹车状态
                enemy.agent.autoBraking = false;
                // 将敌人导航至巡航点
                enemy.MoveToTarget();
            }
            else
            {
                // 播放行走动画
                enemy.animState = 1;
                // 调整移动速度
                enemy.agent.speed = enemy.currentSpeed;
                // 切换自动刹车状态
                enemy.agent.autoBraking = true;
                enemy.agent.SetDestination(enemy.originalPosition);
            }
        } else if ((stateInfo.normalizedTime >= 1 || enemy.attackList.Count != 0) && !enemy.isDead && enemy.attackList.Count > 0)
        {
            // 先播放完咆哮动画再播放跑步
            if (stateInfo.normalizedTime >= 0.99f)
            {
                // 播放跑步动画
                enemy.animState = 2;
                // 切换自动刹车状态
                enemy.agent.autoBraking = true;
                // 调整移动速度
                enemy.agent.speed = enemy.currentSpeed;
                // 将敌人导航至巡航点
                enemy.MoveToTarget();
            }
        }
        if (enemy.isPatrol)
        {
            // 计算敌人到导航点的距离
            float patrolDistance = Vector3.Distance(enemy.transform.position, enemy.wayPoints[enemy.patrolIndex]);
            // 当距离很小时标识已经走到了导航点
            if (patrolDistance <= 2f && enemy.attackList.Count == 0)
            {
                // 设置下一个导航点
                if (enemy.patrolIndex < enemy.wayPoints.Count - 1) enemy.patrolIndex++;
                // 如果已经到达最后一个导航点，重置index为0
                else enemy.patrolIndex = 0;
            }
        }
        else
        {
            // 计算敌人到原始位置的距离
            float patrolDistance = Vector3.Distance(enemy.transform.position, enemy.originalPosition);
            // 当距离很小时表明已经回到了原始位置
            if (patrolDistance <= 2f && enemy.attackList.Count == 0)
            {
                // 停止移动并进入待机状态
                enemy.animState = 0;
                enemy.agent.SetDestination(enemy.transform.position);
            }
        }
    }
}