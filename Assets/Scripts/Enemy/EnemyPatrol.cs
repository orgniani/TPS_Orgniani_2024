using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : Enemy
{
    [Header("References")]
    [SerializeField] private MeleeAttack attack;
    [SerializeField] private Transform target;

    [Header("Parameters")]
    [SerializeField] private float visionRadius = 5f;
    [SerializeField] private float offset = 0.5f;

    [SerializeField] private float fieldOfViewAngle = 90f;

    private enum MovementState { PATROL = 0, FOLLOWTARGET }
    private MovementState movementState;

    private void Start()
    {
        movementState = MovementState.PATROL;
    }

    private void Update()
    {
        CheckIfPlayerSpotted();

        switch (movementState)
        {
            case MovementState.PATROL:
                agent.isStopped = false;
                Patrol();
                break;

            case MovementState.FOLLOWTARGET:
                if (targetHP.Health <= 0) return;

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

        if (PlayerIsTooClose())
        {
            movementState = MovementState.FOLLOWTARGET;
            return;
        }

        Vector3 spherePosition = transform.position + transform.forward * offset;
        bool playerIsInVisionRange = Physics.CheckSphere(spherePosition, visionRadius, targetLayer);

        Vector3 directionToPlayer = target.position - transform.position;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

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
    }
}
