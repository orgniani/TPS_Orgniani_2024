using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float damage = 10f;

    [SerializeField] private float attackProximity = 5f;

    [SerializeField] private float offset = 0.5f;

    [SerializeField] private LayerMask playerLayer;

    [Header("Audio")]
    [SerializeField] private AudioClip punchSound;

    private AudioSource audioSource;
    private NavMeshAgent agent;

    private HealthController playerHP;

    private Vector3 hitPoint;

    public bool IsAttacking { get; private set; }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void CheckIfPlayerInAttackRange()
    {
        Vector3 spherePosition = transform.position + transform.forward * offset;
        bool playerIsInAttackRange = Physics.CheckSphere(spherePosition, attackProximity, playerLayer);

        IsAttacking = playerIsInAttackRange;
    }

    public void CheckIfShouldAttack(HealthController targetHP)
    {
        playerHP = targetHP;

        if (playerHP.Health <= 0)
        {
            IsAttacking = false;
            enabled = false;
            return;
        }

        CheckIfPlayerInAttackRange();
    }

    //zombie-attack event
    public void OnSwipe()
    {
        RaycastHit hit;

        Vector3 sourcePos = transform.position;

        Physics.Raycast(sourcePos, transform.forward, out hit, attackProximity, playerLayer);
        hitPoint = hit.point;

        agent.isStopped = true;
        if (punchSound) audioSource.PlayOneShot(punchSound);
        if (IsAttacking) playerHP.ReceiveDamage(damage, hitPoint);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + transform.forward * offset, attackProximity);
    }
}
