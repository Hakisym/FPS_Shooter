using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : EnemyBaseState
{
    public override void OnStateEnter(StateController enemy)
    {
        enemy.animIndex = 0;
    }

    public override void OnStateUpdate(StateController enemy)
    {
        var transitionInfo = enemy.animator.GetAnimatorTransitionInfo(1);
        var stateInfo = enemy.animator.GetCurrentAnimatorStateInfo(1);

        enemy.transform.LookAt(enemy.targetPlayer);

        //Attack timer
        if (enemy.AttackDecision() && Time.time > enemy.nextAttackTime)
        {
            enemy.animator.SetTrigger("attack");
            enemy.nextAttackTime = Time.time + enemy.attackRate;
        }

        //Swap to move state when target is far
        if (!enemy.AttackDecision() && !stateInfo.IsName("Attack") && transitionInfo.nameHash != Animator.StringToHash("Empty -> Attack"))
        {
            enemy.animator.ResetTrigger("attack");
            enemy.TransitionToState(enemy.focusMoveState);
        }
    }
}
