using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class StateController : MonoBehaviour
{
    public EnemyBaseState currentState;
    public EnemyBaseState[] enemyBaseStates;

    public Transform targetPlayer;
    public Transform lineOfSight;
    private List<Transform> playerList = new List<Transform>();
    private Vector3 targetPosition;

    [Header("Enemy Stats")]
    [Range(0,30)]
    public float engageRadius;
    [Range(0,100)]
    public int viewRadius;
    [Range(0,120)]
    public int viewAngle;

    [Header("Enemy Stats SO")]
    public float attackInterval;
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [Header("Enemy Class SO")]
    public Attack[] attacks;
    public MeleeHandler[] hitHandlers;
    public int grabDamage;
    public float moveDistance;

    [HideInInspector] public Animator animator;
    [HideInInspector] public int animLocomotion;
    [HideInInspector] public float attackEndTime;
    [HideInInspector] public bool attackFinished;
    [HideInInspector] public bool grabbedTarget;
    [HideInInspector] public NavMeshAgent agent;
    private AudioSource audioSource;

    //Initial data and state
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        GetComponent<EnemyHealth>().OnDie += DeathHandler;
        attackEndTime = float.MaxValue;
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            playerList.Add(player.transform);
        }

        TransitionToState<FocusMoveState>();
    }

    void Update()
    {
        animator.SetInteger("locomotion", animLocomotion);
        currentState.OnStateUpdate(this);
    }

    //Set navmesh destination
    public void MoveToTarget()
    {
        targetPosition = Vector3.MoveTowards(transform.position, targetPlayer.position, agent.speed * Time.deltaTime);
        agent.destination = targetPosition;
    }

    //Change state
    public void TransitionToState<T>() where T : EnemyBaseState
    {
        foreach (var state in enemyBaseStates)
        {
            if (state is T)
            {
                currentState = state;
                currentState.OnStateEnter(this);
                return;
            }
        }
        throw new Exception("Transition state doesn't exist");
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
    public Collider[] CheckTargetsInRadius(float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, targetMask);
        if (colliders.Length == 0)
            return null;

        var orderedByProximity = colliders.OrderBy(c => (transform.position - c.transform.position).sqrMagnitude).ToArray();
        return orderedByProximity;
    }

    public bool CheckTargetInRadius(Transform target,float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, targetMask);
        foreach (var collider in colliders)
        {
            if (collider.transform == target)
                return true;
        }
        return false;
    }

    //The common cast function
    public bool BlockedSight(Vector3 target)
    {
        //Get cast to target parameters
        Vector3 castOrigin = lineOfSight.position;
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
        var colliders = CheckTargetsInRadius(viewRadius);
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

    public bool AttackDecision()
    {
        return CheckTargetInRadius(targetPlayer, engageRadius);
    }

    public void DeathHandler()
    {
        if (grabbedTarget)
            targetPlayer.GetComponent<PlayerMovement>().isGrabbed = false;
    }
}
