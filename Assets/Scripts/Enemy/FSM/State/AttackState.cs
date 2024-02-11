using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                enemy.TransitionToState(enemy.focusMoveState);
            }
        }

        //Attack
        if (enemy.AttackDecision() && Time.time - enemy.attackEndTime >= enemy.attackInterval && enemy.attackFinished)
        {
            ActAttack(enemy);
        }
    }

    private void ActAttack(StateController enemy)
    {
        var attack = enemy.attacks[Random.Range(0, enemy.attacks.Length)];

        enemy.animator.SetInteger("attack", attack.animationIndex);
        enemy.weaponHandlers[0].damageAmount = attack.damage;

        enemy.attackFinished = false;
    }
}
