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
        if (Vector3.Distance(enemy.transform.position, enemy.targetPlayer.position) > enemy.attackRadius 
            && !enemy.animator.GetCurrentAnimatorStateInfo(1).IsName("Attack"))
        {
            enemy.TransitionToState(enemy.focusMoveState);
            enemy.attackTimer = 0f;
        }

        //Attack timer
        if(!enemy.animator.GetCurrentAnimatorStateInfo(1).IsName("Attack"))
            enemy.attackTimer += Time.deltaTime;

        if (enemy.attackTimer > enemy.attackInterval)
        {
            enemy.animator.SetTrigger("attack");
            enemy.attackTimer = 0f;
        }
    }
}
