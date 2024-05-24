using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    //[SerializeField] private MeleeAttack attack;

    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private Transform target;

    [SerializeField] private LayerMask playerLayer;

    [Header("Parameters")]
    [SerializeField] private float visionRadius = 5f;
    [SerializeField] private float offset = 0.5f;

    [SerializeField] private float proximityRadius = 5f;
    [SerializeField] private float fieldOfViewAngle = 90f;

    [SerializeField] private float SpeedChangeRate = 10.0f;

    private NavMeshAgent agent;
    private HealthController playerHP;

    private int currentPatrolPointIndex = 0;

    private enum MovementState { PATROL = 0, FOLLOWTARGET }
    private MovementState movementState;

    // animation IDs
    private int _animIDSpeed;

    //animator blends
    private float _animationSpeedBlend;

    private Animator _animator;
    private bool _hasAnimator;

    private void Start()
    {
        _hasAnimator = TryGetComponent(out _animator);
        AssignAnimationIDs();

        agent = GetComponent<NavMeshAgent>();
        playerHP = target.gameObject.GetComponent<HealthController>();

        movementState = MovementState.PATROL;
        StartCoroutine(PatrolRoutine());
    }

    private void Update()
    {
        _hasAnimator = TryGetComponent(out _animator);

        CheckIfPlayerSpotted();

        var speed = agent.velocity.magnitude;

        _animationSpeedBlend = Mathf.Lerp(_animationSpeedBlend, speed, Time.deltaTime * SpeedChangeRate);
        if (_animationSpeedBlend < 0.01f) _animationSpeedBlend = 0f;

        switch (movementState)
        {
            case MovementState.PATROL:
                agent.speed = 1;
                agent.isStopped = false;
                Patrol();
                break;

            case MovementState.FOLLOWTARGET:
                agent.speed = 2;
                if (playerHP.Health <= 0) return;

                //attack.AttackNow(target, playerHP);
                agent.SetDestination(target.position);
                break;

            default:
                break;
        }

        if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, _animationSpeedBlend);
        }
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
    }

    private void CheckIfPlayerSpotted()
    {
        agent.isStopped = false;

        bool playerIsTooClose = Physics.CheckSphere(transform.position, proximityRadius, playerLayer);

        Vector3 spherePosition = transform.position + transform.forward * offset;
        bool playerIsInVisionRange = Physics.CheckSphere(spherePosition, visionRadius, playerLayer);

        Vector3 directionToPlayer = target.position - transform.position;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (playerIsTooClose)
        {
            movementState = MovementState.FOLLOWTARGET;
            return;
        }

        if (playerIsInVisionRange && angleToPlayer < fieldOfViewAngle * 0.5f)
        {
            RaycastHit hit;

            if (movementState == MovementState.PATROL)
            {
                if (Physics.Raycast(transform.position, directionToPlayer, out hit, visionRadius * 2f, ~playerLayer))
                {
                    movementState = MovementState.PATROL;
                }

                else
                {
                    movementState = MovementState.FOLLOWTARGET;

                }
            }
        }

        else
        {
            movementState = MovementState.PATROL;
        }
    }

    private void Patrol()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
            SetNextPatrolPoint();
    }

    private void SetNextPatrolPoint()
    {
        agent.SetDestination(patrolPoints[currentPatrolPointIndex].position);
        currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Length;
    }

    private IEnumerator PatrolRoutine()
    {
        while (true)
        {
            if (movementState == MovementState.PATROL)
            {
                agent.isStopped = false;

                yield return new WaitForSeconds(1f);

                agent.isStopped = true;

                yield return new WaitForSeconds(1f);
            }
            else
            {
                yield return null;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = (movementState == MovementState.FOLLOWTARGET) ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position + transform.forward * offset, visionRadius);

        Gizmos.DrawWireSphere(transform.position, proximityRadius);
    }
}
