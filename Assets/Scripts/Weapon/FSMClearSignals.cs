using UnityEngine;

public class FSMClearSignals : StateMachineBehaviour
{
    // 清理进入动画的信号量
    public string[] clearAtEnter;
    // 清理退出动画的信号量
    public string[] clearAtExit;

    // 进入该动画时
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 清处多余的信号量
        foreach(string signal in clearAtEnter)
        {
            animator.ResetTrigger(signal);
        }
    }

    // 退出该动画时
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 清处多余的信号量
        foreach (string signal in clearAtExit)
        {
            animator.ResetTrigger(signal);
        }
    }
}
