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
        //walk to target
        enemy.MoveToTarget();

        //Sight radius
        float distance = Vector3.Distance(enemy.transform.position, enemy.GetClosestPlayer().position);
        if (distance <= enemy.viewRadius)
        {
            enemy.animIndex = 0;
        }
    }
}
