using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackState", menuName = "Enemy/State/AttackState")]
public class AttackState : EnemyBaseState
{
    public override void OnStateEnter(ZombieStateController enemy)
    {
        ActAttack(enemy);
    }

    public override void OnStateUpdate(ZombieStateController enemy)
    {
        enemy.agent.enabled = enemy.isAttacking ? false : true;

        enemy.transform.LookAt(enemy.targetPlayer);

        if (!enemy.isAttacking)
        {
            //Exit attack
            if (!enemy.AttackDecision())
            {
                enemy.TransitionToState<FocusMoveState>();
            }
        }

        //Attack
        if (enemy.AttackDecision() && Time.time - enemy.attackEndTime >= enemy.attackInterval && !enemy.isAttacking)
        {
            ActAttack(enemy);
        }

        if (enemy.isGrabbing)
        {
            enemy.TransitionToState<GrabState>();
        }
    }

    private void ActAttack(ZombieStateController enemy)
    {
        enemy.isAttacking = true;

        var attack = enemy.attacks[Random.Range(0, enemy.attacks.Length)];

        enemy.animator.SetInteger("attack", attack.animationIndex);

        foreach (var hitPoint in enemy.hitHandlers)
        {
            hitPoint.attack = attack;
        }
    }
}
