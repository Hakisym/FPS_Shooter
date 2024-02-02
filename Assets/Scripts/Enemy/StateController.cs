using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateController : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;
    [HideInInspector]public int animIndex;

    public Transform targetPlayer;
    private List<Transform> playerList = new List<Transform>();
    private Vector3 targetPosition;

    public EnemyBaseState currentState;
    private FocusMoveState focusMoveState = new FocusMoveState();
    private AttackState attackState = new AttackState();

    [Range(0,30)]
    public int viewRadius;

    //Initial data and state
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            playerList.Add(player.transform);
        }

        TransitionToState(new FocusMoveState());
    }

    void Update()
    {
        animator.SetInteger("state", animIndex);
        currentState.OnStateUpdate(this);
    }

    //Set navmesh destination
    public void MoveToTarget()
    {
        targetPosition = Vector3.MoveTowards(transform.position, targetPlayer.position, agent.speed * Time.deltaTime);
        agent.destination = targetPosition;
    }

    //Change state
    public void TransitionToState(EnemyBaseState newState)
    {
        currentState = newState;
        currentState.OnStateEnter(this);
    }

    //Get closest player from list
    public Transform GetClosestPlayer()
    {
        if (playerList.Count == 0)
        {   
            // Handle the case where playerList is empty (return null or another appropriate value)
            return null;
        }

        float minDistance = float.MaxValue;
        Transform target = null;
        foreach (var player in playerList)
        {
            float distance = Vector3.Distance(transform.position,player.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                target = player;
            }
        }
        return target;
    }
}
