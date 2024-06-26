using UnityEngine;

public class PatrolEnemy : Enemy
{
    [Header("References")]
    [SerializeField] private MeleeAttack attack;

    [Header("Parameters")]
    [SerializeField] private float visionRadius = 5f;
    [SerializeField] private float offset = 0.5f;

    [SerializeField] private float fieldOfViewAngle = 90f;

    private HealthController targetHP;


    private enum MovementState { PATROL = 0, FOLLOWTARGET }
    private MovementState movementState;

    protected override void Awake()
    {
        base.Awake();
        targetHP = target.gameObject.GetComponent<HealthController>();

        movementState = MovementState.PATROL;
    }

    private void Update()
    {
        if (!isAwake) return;

        CheckIfPlayerSpotted();

        switch (movementState)
        {
            case MovementState.PATROL:
                agent.isStopped = false;
                Patrol();
                break;

            case MovementState.FOLLOWTARGET:
                attack.AttackNow(target, targetHP);
                agent.SetDestination(target.position);
                break;

            default:
                break;
        }
    }

    private void CheckIfPlayerSpotted()
    {
        agent.isStopped = false;

        Vector3 spherePosition = transform.position + transform.forward * offset;
        bool playerIsInVisionRange = Physics.CheckSphere(spherePosition, visionRadius, targetLayer);

        Vector3 directionToPlayer = target.position - transform.position;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (PlayerIsTooClose())
        {
            movementState = MovementState.FOLLOWTARGET;
            return;
        }

        if (playerIsInVisionRange && angleToPlayer < fieldOfViewAngle * 0.5f)
        {
            RaycastHit hit;

            if (movementState == MovementState.PATROL)
            {
                if (Physics.Raycast(transform.position, directionToPlayer, out hit, visionRadius * 2f, ~targetLayer))
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

    private void OnDrawGizmos()
    {
        Gizmos.color = (movementState == MovementState.FOLLOWTARGET) ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position + transform.forward * offset, visionRadius);

        Gizmos.DrawWireSphere(transform.position, proximityRadius);
    }
}
