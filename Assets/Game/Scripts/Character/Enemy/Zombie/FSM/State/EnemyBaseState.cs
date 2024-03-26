using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBaseState : ScriptableObject
{
    public abstract void OnStateEnter(ZombieStateController enemy);

    public abstract void OnStateUpdate(ZombieStateController enemy);
}
