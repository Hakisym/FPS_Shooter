using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateController : MonoBehaviour
{
    private Transform player;
    private Animator animator;
    private NavMeshAgent agent;
    public int animIndex;

    private Vector3 targetPosition;
    
    public EnemyBaseState currentState;
    private FocusMoveState focusMoveState = new FocusMoveState();
    private AttackState attackState = new AttackState();

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        TransitionToState(new FocusMoveState());
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetInteger("state", animIndex);
        currentState.OnStateUpdate(this);
    }

    public void MoveToTarget()
    {
        targetPosition = Vector3.MoveTowards(transform.position, player.position, agent.speed * Time.deltaTime);
        agent.destination = targetPosition;
    }

    public void TransitionToState(EnemyBaseState newState)
    {
        currentState = newState;
        currentState.OnStateEnter(this);
    }

    public Transform GetPlayerTransform()
    {
        return player;
    }
}
