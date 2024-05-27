using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider enemyCollider;
    [SerializeField] private GameObject healthBar;

    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private Transform target;

    [SerializeField] private LayerMask playerLayer;

    [Header("Parameters")]
    [SerializeField] private float visionRadius = 5f;
    [SerializeField] private float offset = 0.5f;

    [SerializeField] private float proximityRadius = 5f;
    [SerializeField] private float fieldOfViewAngle = 90f;

    [SerializeField] private float SpeedChangeRate = 10.0f;

    [SerializeField] private float patrolSpeed = 0.5f;
    [SerializeField] private float chaseSpeed = 2f;

    private NavMeshAgent agent;
    private EnemyAttack attack;
    private HealthController HP;
    private HealthController playerHP;

    private int currentPatrolPointIndex = 0;

    private enum MovementState { PATROL = 0, FOLLOWTARGET }
    private MovementState movementState;

    // animation
    private int _animIDSpeed;
    private int _animIDDeath;
    private int _animIDHurt;
    private int _animIDAttack;
    private float _animationSpeedBlend;

    private Animator _animator;
    private bool _hasAnimator;

    private void Awake()
    {
        attack = GetComponent<EnemyAttack>();
        HP = GetComponent<HealthController>();
    }

    private void OnEnable()
    {
        HP.onHurt += HandleHurt;
        HP.onDead += HandleDeath;
    }

    private void OnDisable()
    {
        HP.onHurt -= HandleHurt;
        HP.onDead -= HandleDeath;
    }

    private void Start()
    {
        _hasAnimator = TryGetComponent(out _animator);
        AssignAnimationIDs();

        agent = GetComponent<NavMeshAgent>();

        playerHP = target.gameObject.GetComponent<HealthController>();

        movementState = MovementState.PATROL;
    }

    private void Update()
    {
        _hasAnimator = TryGetComponent(out _animator);

        CheckIfPlayerSpotted();

        attack.CheckIfShouldAttack(playerHP);
        HandleAttackAnimation();

        var speed = agent.velocity.magnitude;

        _animationSpeedBlend = Mathf.Lerp(_animationSpeedBlend, speed, Time.deltaTime * SpeedChangeRate);
        if (_animationSpeedBlend < 0.01f) _animationSpeedBlend = 0f;

        switch (movementState)
        {
            case MovementState.PATROL:
                agent.speed = patrolSpeed;

                Patrol();
                break;

            case MovementState.FOLLOWTARGET:
                agent.speed = chaseSpeed;

                if (playerHP.Health <= 0) return;

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
        _animIDDeath = Animator.StringToHash("Death");
        _animIDHurt = Animator.StringToHash("Hurt");
        _animIDAttack = Animator.StringToHash("Attack");
    }

    private void CheckIfPlayerSpotted()
    {
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

    private void HandleHurt()
    {
        if (_hasAnimator)
        {
            _animator.SetTrigger(_animIDHurt);
        }

        agent.isStopped = true;
    }

    private void HandleDeath()
    {
        if (_hasAnimator)
        {
            _animator.SetTrigger(_animIDDeath);
        }

        enabled = false;
        enemyCollider.enabled = false;
        healthBar.SetActive(false);
    }


    private void HandleAttackAnimation()
    {
        if (_hasAnimator)
        {
            _animator.SetBool(_animIDAttack, attack.IsAttacking);
        }
    }

    //zombie-walk animation event
    public void OnStopMoving(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            agent.isStopped = true;
        }
    }

    //zombie-walk animation event
    public void OnStartMoving(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            agent.isStopped = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = (movementState == MovementState.FOLLOWTARGET) ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position + transform.forward * offset, visionRadius);

        Gizmos.DrawWireSphere(transform.position, proximityRadius);
    }
}
