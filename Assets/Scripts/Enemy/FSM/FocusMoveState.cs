using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusMoveState : EnemyBaseState
{
    public override void OnStateEnter(StateController enemy)
    {
        //set target player and walk pose
        enemy.animIndex = 1;
        enemy.targetPlayer = enemy.GetClosestPlayer();
    }

    public override void OnStateUpdate(StateController enemy)
    {
        //Walk to target
        enemy.MoveToTarget();

        //Sight radius
        enemy.LookDecision();

        //Engage radius
        if (Vector3.Distance(enemy.transform.position, enemy.targetPlayer.position) < enemy.attackRadius)
        {
            enemy.TransitionToState(enemy.attackState);
        }
    }
}
