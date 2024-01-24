using UnityEngine;

public class KnifeAttack : StateMachineBehaviour
{
    [Header("属性")]
    [Tooltip("攻击范围")] public float attackRange = 2.5f;
    [Tooltip("攻击伤害")] public int attackDamage = 150;
    [Tooltip("喷血特效")] public Transform [] bloodImpactPrefabs;
    [Tooltip("获取角色位置")] private Transform playerTransform;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 获取角色位置
        playerTransform = animator.transform;
        // 获取球体内所有碰撞体
        Collider[] hitColliders = Physics.OverlapSphere(playerTransform.position + playerTransform.forward * 0.5f - Vector3.down * 0.5f, attackRange);
        foreach (var hitCollider in hitColliders)
        {
            // 如果碰撞体标记为Enemy
            if (hitCollider.CompareTag("EnemyCollider"))
            {
                // 敌人扣血的代码
                hitCollider.GetComponentInParent<Enemy>().Health(attackDamage);
                // 生成喷血特效
			    Instantiate (bloodImpactPrefabs [Random.Range (0, bloodImpactPrefabs.Length)], playerTransform.position + playerTransform.forward * 0.5f, Quaternion.LookRotation (playerTransform.position + Vector3.right * 5));
            }
            // 如果碰撞体标记为ExplosiveBarrel
            else if (hitCollider.CompareTag("ExplosiveBarrel"))
            {
                // 使油桶爆炸
                hitCollider.GetComponent<ExplosiveBarrelScript>().explode = true;
            }
            // 如果碰撞体标记为GasTank
            else if (hitCollider.CompareTag("GasTank"))
            {
                // 使气瓶泄气
                hitCollider.GetComponent<GasTankScript>().isHit = true;
            }
        }
    }
}
