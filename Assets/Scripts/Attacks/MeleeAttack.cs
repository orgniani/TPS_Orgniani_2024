using System;
using System.Collections;
using UnityEngine;

public class MeleeAttack : MonoBehaviour, IAttack
{
    [Header("Parameters")]
    [SerializeField] private float damage;

    [SerializeField] private float attackCooldown;
    [SerializeField] private float attackProximity = 5f;

    [SerializeField] private float fieldOfAttackAngle = 90f;
    [SerializeField] private float offset = 0.5f;

    [SerializeField] private LayerMask targetLayer;

    [Header("Audio")]
    [SerializeField] private AudioClip punchSound;

    private AudioSource audioSource;

    private Transform targetTransform;
    private HealthController targetHP;

    private bool shouldAttack = true;
    private Vector3 hitPoint;
    private float occupiedTimeAfterAttack = 1;

    public event Action onPunch = delegate { };

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void HandleAttack()
    {
        if (!shouldAttack) return;
        StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        shouldAttack = false;

        Vector3 spherePosition = transform.position + transform.forward * offset;
        bool targetIsInAttackRange = Physics.CheckSphere(spherePosition, attackProximity, targetLayer);

        Vector3 directionToTarget = targetTransform.position - transform.position;
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

        if (angleToTarget < fieldOfAttackAngle && targetIsInAttackRange)
            Punch();

        yield return new WaitForSeconds(attackCooldown);

        shouldAttack = true;
    }

    private void Punch()
    {
        RaycastHit hit;
        Vector3 sourcePos = transform.position;

        Physics.Raycast(sourcePos, transform.forward, out hit, attackProximity, targetLayer);
        hitPoint = hit.point;

        onPunch?.Invoke();
        if (punchSound) audioSource.PlayOneShot(punchSound);

        targetHP.ReceiveDamage(damage, hitPoint);
    }

    public float AttackNow(Transform target, HealthController targetHP)
    {
        targetTransform = target;
        this.targetHP = targetHP;

        HandleAttack();
        return occupiedTimeAfterAttack;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + transform.forward * offset, attackProximity);
    }
}
