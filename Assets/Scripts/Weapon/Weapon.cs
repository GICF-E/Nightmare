using UnityEngine;

// 武器抽象类
public abstract class Weapon : MonoBehaviour
{
    public abstract void GunFire();

    public abstract void Reload();

    public abstract void ExpandingCrossUpdate(float expanDegree);
    public abstract void DoReloadAnimation();

    public abstract void AimIn();
    public abstract void AimOut();
}
