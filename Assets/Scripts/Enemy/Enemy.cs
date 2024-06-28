using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDraggable
{
    [Header("References")]
    [SerializeField] protected Transform target;
    [SerializeField] protected List<Transform> patrolPoints;

    [SerializeField] protected LayerMask targetLayer;

    [Header("Parameters")]
    [SerializeField] private float passedOutDuration = 10f;
    [SerializeField] protected float proximityRadius = 5f;

    [Header("Audio")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip wakeUpSound;

    protected AudioSource audioSource;

    protected HealthController HP;
    protected NavMeshAgent agent;

    protected bool isAwake = true;
    protected int currentPatrolPointIndex = 0;

    public static event Action<Enemy> onSpawn;
    public static event Action<Enemy> onTrapped;

    public event Action onWakeUpAnimation;

    private Coroutine wakeUpCoroutine;
    private Coroutine enableCoroutine;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        HP = GetComponent<HealthController>();

        if (patrolPoints == null)
            patrolPoints = new List<Transform>();

    }

    protected virtual void Start()
    {
        onSpawn?.Invoke(this);
    }

    protected virtual void OnEnable()
    {
        HP.onHPChange += HandleKnockedOut;
    }

    protected virtual void OnDisable()
    {
        HP.onHPChange -= HandleKnockedOut;
    }

    private void HandleKnockedOut()
    {
        if (wakeUpCoroutine != null)
            StopCoroutine(wakeUpCoroutine);

        if (enableCoroutine != null)
            StopCoroutine(enableCoroutine);

        isAwake = false;

        audioSource.PlayOneShot(deathSound);

        agent.isStopped = true;

        wakeUpCoroutine = StartCoroutine(WaitToWakeBackUp());
    }

    private IEnumerator WaitToWakeBackUp()
    {
        yield return new WaitForSeconds(passedOutDuration);

        onWakeUpAnimation?.Invoke();

        HP.SetToMaxHealth();

        audioSource.PlayOneShot(wakeUpSound);
        isAwake = true;

        enableCoroutine = StartCoroutine(WaitToEnable());
    }

    private IEnumerator WaitToEnable()
    {
        while (!isAwake)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1);

        agent.isStopped = false;
    }

    public void HandleGetTrapped()
    {
        onTrapped?.Invoke(this);

        gameObject.SetActive(false);
    }

    protected virtual void Patrol()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
            SetNextPatrolPoint();
    }

    protected virtual void SetNextPatrolPoint()
    {
        currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Count;
        agent.SetDestination(patrolPoints[currentPatrolPointIndex].position);
    }

    protected virtual void ChaseTarget()
    {
        agent.SetDestination(target.position);
    }

    protected bool PlayerIsTooClose()
    {
        return Physics.CheckSphere(transform.position, proximityRadius, targetLayer);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, proximityRadius);
    }

    public bool CanBeDragged()
    {
        return !isAwake;
    }

}