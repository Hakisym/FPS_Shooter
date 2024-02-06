using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class StateController : MonoBehaviour
{
    [HideInInspector] public Animator animator;
    [HideInInspector] public int animIndex;
    private NavMeshAgent agent;
    private AudioSource audioSource;

    public Transform targetPlayer;
    private List<Transform> playerList = new List<Transform>();
    private Vector3 targetPosition;

    public EnemyBaseState currentState;
    public FocusMoveState focusMoveState = new FocusMoveState();
    public AttackState attackState = new AttackState();

    [Header("Enemy Stats")]
    [Range(0,30)]
    public int attackRadius;
    [Range(0,100)]
    public int viewRadius;
    [Range(0,120)]
    public int viewAngle;
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public float headHeight;

    public AudioClip attackSound;
    public float attackInterval;
    public float attackRange;
    public float attackTimer;

    //Initial data and state
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            playerList.Add(player.transform);
        }

        TransitionToState(focusMoveState);
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

    //Get the closest player
    public Transform GetClosestPlayer()
    {
        return FindClosestTransform(playerList);
    }

    //Find the closest transform
    public Transform FindClosestTransform(IEnumerable<Transform> transforms)
    {
        if (playerList.Count == 0)
        {
            return null;
        }

        float minDistance = float.MaxValue;
        Transform closestTransform = null;
        foreach (var _transform in transforms)
        {
            float distance = Vector3.Distance(transform.position, _transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestTransform = _transform;
            }
        }
        return closestTransform;
    }

    //The common overlap function for senses decisions
    public Collider[] CheckTargetInRadius(float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, targetMask);
        var orderedByProximity = colliders.OrderBy(c => (transform.position - c.transform.position).sqrMagnitude).ToArray();
        return orderedByProximity;
    }

    //The common cast function
    public bool BlockedSight(Vector3 target)
    {
        //Get cast to target parameters
        Vector3 castOrigin = transform.position + Vector3.up * headHeight;
        Vector3 dirToTarget = target - castOrigin;
        //Hit anything other than targets?
        bool blockedSight = Physics.Raycast(castOrigin, dirToTarget, dirToTarget.magnitude, obstacleMask);

        return blockedSight;
    }

    public bool HandleViewAngle(Collider collider)
    {
        Vector3 target = collider.transform.position;
        //Check if target is in field of view
        Vector3 dirToTarget = target - transform.position;
        bool inFOVCondition = (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2);
        //Is target in FOV and NPC has a clear sight?
        if (inFOVCondition && !BlockedSight(target))
        {
            return true;
        }
        //No target in sight
        return false;
    }

    public void LookDecision()
    {
        var colliders = CheckTargetInRadius(viewRadius);
        //Is the collider is in FOV?
        foreach (var collider in colliders)
        {
            if (HandleViewAngle(collider))
            {
                //Change to closer target in FOV
                if (targetPlayer != collider.transform)
                {
                    targetPlayer = collider.transform;
                }
                return;
            }
        }
    }
}
