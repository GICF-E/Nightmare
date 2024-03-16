using UnityEngine;

public class ReloadAmmoState : StateMachineBehaviour
{
    // 动画播放到百分之几
    public float reloadTime = 0.3f;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime >= reloadTime)
        {
            if(animator.GetComponent<Weapon_AutomaticGun>() != null)
            {
                animator.GetComponent<Weapon_AutomaticGun>().ShootGunReload();
            }
        }
        
    }
}
