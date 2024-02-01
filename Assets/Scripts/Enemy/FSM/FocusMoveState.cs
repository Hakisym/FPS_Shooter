using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusMoveState : EnemyBaseState
{
    public override void OnStateEnter(StateController enemy)
    {
        enemy.animIndex = 1;
    }

    public override void OnStateUpdate(StateController enemy)
    {
        enemy.MoveToTarget();
        float distance = Vector3.Distance(enemy.transform.position, enemy.GetPlayerTransform().position);
        if (distance <= 2f)
        {
            enemy.animIndex = 0;
        }
    }
}
