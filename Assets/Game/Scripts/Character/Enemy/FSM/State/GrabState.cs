using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GrabState", menuName = "Enemy/State/GrabState")]
public class GrabState : EnemyBaseState
{
    public override void OnStateEnter(StateController enemy)
    {
        enemy.animator.SetTrigger("grab");
        enemy.agent.enabled = false;
    }

    public override void OnStateUpdate(StateController enemy)
    {
        // Calculate the direction towards the target player
        Vector3 directionToPlayer = enemy.targetPlayer.position - enemy.transform.position;

        // Check if the enemy is already close enough to the target player
        if (directionToPlayer.magnitude > enemy.moveDistance)
        {
            // Normalize the direction vector to ensure consistent movement speed
            directionToPlayer.Normalize();

            // Calculate the new position by lerping towards the target player
            Vector3 newPosition = Vector3.Lerp(enemy.transform.position, enemy.targetPlayer.position, Time.deltaTime * 2f);

            // Move the enemy to the new position
            enemy.transform.position = newPosition;
        }
    }
}
