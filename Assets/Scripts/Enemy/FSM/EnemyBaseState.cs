using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBaseState : MonoBehaviour
{
    public abstract void OnStateEnter(StateController enemy);

    public abstract void OnStateUpdate(StateController enemy);
}
