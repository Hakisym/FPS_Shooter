using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : EnemyBaseState
{
    public override void OnStateEnter(StateController enemy)
    {
        enemy.animIndex = 0;
        enemy.animator.SetTrigger("attack");
    }

    public override void OnStateUpdate(StateController enemy)
    {

        //Swap to move state when target is far
        if (!enemy.AttackDecision() && enemy.animator.GetCurrentAnimatorStateInfo(1).IsName("Empty"))
        {
            enemy.animator.ResetTrigger("attack");
            enemy.TransitionToState(enemy.focusMoveState);
        }

        //Attack timer
        if (enemy.AttackDecision() && Time.time > enemy.nextAttackTime)
        {
            enemy.animator.SetTrigger("attack");
            enemy.nextAttackTime = Time.time + enemy.attackRate;
        }

        enemy.transform.LookAt(enemy.targetPlayer);
    }
}
