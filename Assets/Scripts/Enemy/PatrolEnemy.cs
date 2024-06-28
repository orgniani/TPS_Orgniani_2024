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
                ChaseTarget();
                break;

            default:
                break;
        }
    }

    private void CheckIfPlayerSpotted()
    {
        agent.isStopped = false;

        bool targetIsTooClose = Physics.CheckSphere(transform.position, proximityRadius, targetLayer);

        Vector3 spherePosition = transform.position + transform.forward * offset;
        bool targetIsInVisionRange = Physics.CheckSphere(spherePosition, visionRadius, targetLayer);

        Vector3 directionToTarget = target.position - transform.position;
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

        if (targetIsTooClose)
        {
            movementState = MovementState.FOLLOWTARGET;
            return;
        }

        if (targetIsInVisionRange)
        {
            RaycastHit hit;

            bool targetIsInVisionAngle = Physics.Raycast(transform.position, directionToTarget, out hit, visionRadius * 2f);

            if (angleToTarget < fieldOfViewAngle * 0.5f && targetIsInVisionAngle)
            {
                if (((1 << hit.collider.gameObject.layer) & targetLayer) != 0)
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

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = (movementState == MovementState.FOLLOWTARGET) ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position + transform.forward * offset, visionRadius);

        //VISION ANGLE
        Gizmos.color = Color.yellow;
        Vector3 forward = transform.forward * visionRadius * 2;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-fieldOfViewAngle * 0.5f, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(fieldOfViewAngle * 0.5f, Vector3.up);

        Vector3 leftRayDirection = leftRayRotation * forward;
        Vector3 rightRayDirection = rightRayRotation * forward;

        Gizmos.DrawLine(transform.position, transform.position + leftRayDirection);
        Gizmos.DrawLine(transform.position, transform.position + rightRayDirection);
    }
}
