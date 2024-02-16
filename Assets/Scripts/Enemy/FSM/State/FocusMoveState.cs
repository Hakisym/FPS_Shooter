using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FocusMoveState", menuName = "Enemy/State/FocusMoveState")]
public class FocusMoveState : EnemyBaseState
{
    public override void OnStateEnter(StateController enemy)
    {
        //set target player and walk pose
        enemy.animLocomotion = 1;
        enemy.targetPlayer = enemy.GetClosestPlayer();
    }

    public override void OnStateUpdate(StateController enemy)
    {
        //Walk to target
        enemy.MoveToTarget();

        //Sight radius
        enemy.LookDecision();

        //Engage radius
        if (enemy.AttackDecision())
        {
            enemy.TransitionToState<AttackState>();
        }
    }
}
