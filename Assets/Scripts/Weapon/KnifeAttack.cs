using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeAttack : StateMachineBehaviour
{
    [Header("属性")]
    [Tooltip("攻击范围")]public float attackRange = 2.5f;
    [Tooltip("攻击伤害")]public int attackDamage = 150;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 从角色前方发射射线
        RaycastHit hit;
        if (Physics.Raycast(GameObject.Find("Player").transform.position, GameObject.Find("Player").transform.forward, out hit, attackRange))
        {
            // 如果射线检测到标记为Enemy的对象
            if(hit.transform.tag == "Collider" || hit.transform.tag == "Enemy"){
                // 敌人扣血的代码
                hit.transform.GetComponentInParent<Enemy>().Health(attackDamage);
            }
            // 如果射线检测到油桶
            if(hit.transform.tag == "ExplosiveBarrel"){
                // 使油桶爆炸
                hit.transform.GetComponent<ExplosiveBarrelScript>().explode = true;
            }
            // 如果射线检测到气瓶
            if(hit.transform.tag == "GasTank"){
                // 使气瓶泄气
                hit.transform.GetComponent<GasTankScript>().isHit = true;
            }
        }
    }
}
