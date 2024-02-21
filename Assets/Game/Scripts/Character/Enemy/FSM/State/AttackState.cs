using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackState", menuName = "Enemy/State/AttackState")]
public class AttackState : EnemyBaseState
{
    public override void OnStateEnter(StateController enemy)
    {
        enemy.animLocomotion = 0;
        ActAttack(enemy);
    }

    public override void OnStateUpdate(StateController enemy)
    {
        enemy.transform.LookAt(enemy.targetPlayer);

        if (enemy.attackFinished)
        {
            //Exit attack
            if (!enemy.AttackDecision())
            {
                enemy.TransitionToState<FocusMoveState>();
            }
        }

        //Attack
        if (enemy.AttackDecision() && Time.time - enemy.attackEndTime >= enemy.attackInterval && enemy.attackFinished)
        {
            ActAttack(enemy);
        }

        if (enemy.grabbedTarget)
        {
            enemy.TransitionToState<GrabState>();
        }
    }

    private void ActAttack(StateController enemy)
    {
        var attack = enemy.attacks[Random.Range(0, enemy.attacks.Length)];

        enemy.animator.SetInteger("attack", attack.animationIndex);

        foreach (var hitPoint in enemy.hitHandlers)
        {
            hitPoint.attack = attack;
        }
        
        enemy.attackFinished = false;
    }
}
