using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 用于扩展实现敌人基础状态的抽象类
/// </summary>
public abstract class EnemyBaseState : MonoBehaviour
{
    // 首次进入状态
    public abstract void EnemyState(Enemy enemy);

    // 持续执行
    public abstract void OnUpdate(Enemy enemy);
}
