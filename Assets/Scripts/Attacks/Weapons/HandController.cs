using StarterAssets;
using System;
using UnityEngine;
using UnityEngine.AI;

public class HandController : AttackController
{
    [Header("References")]
    [SerializeField] private ThirdPersonController TPSController;
    [SerializeField] private AudioSource trapGoblinSound;

    [Header("Parameters")]
    [SerializeField] private float proximityRadius = 5f;
    [SerializeField] private float dragSpeed = 2f;

    private NavMeshAgent enemyAgent;

    private float moveSpeed = 2f;
    private Transform currentlyDraggedObject;
    private bool isDraggingEnemy = false;

    public event Action onClick = delegate { };

    public bool IsDraggingEnemy => isDraggingEnemy;

    public bool CanDrag { set; get; }


    private void OnEnable()
    {
        moveSpeed = TPSController.MoveSpeed;
    }

    private void OnDisable()
    {
        StopDragging();
    }

    private void Update()
    {
        if (!CanDrag)
        {
            StopDragging();
            return;
        }

        HandlePlayerAiming();

        if (starterAssetInputs.aim)
        {
            if (hasAnimator)
            {
                animator.SetTrigger(animIDDrag);
            }

            DragObject();
        }

        else
        {
            StopDragging();
        }

        if (starterAssetInputs.shoot) onClick?.Invoke();
    }

    private void DragObject()
    {
        Transform nearestObject = FindNearestObject();

        if (nearestObject != null)
        {
            float distanceToNearestObject = Vector3.Distance(transform.position, nearestObject.position);

            if (distanceToNearestObject <= proximityRadius)
            {
                enemyAgent = nearestObject.GetComponent<NavMeshAgent>();

                if (enemyAgent != null)
                {
                    enemyAgent.isStopped = false;
                    enemyAgent.stoppingDistance = 1;
                    enemyAgent.SetDestination(transform.position);

                    isDraggingEnemy = true;
                }

                thirdPersonController.SetRotateOnMove(false);
                thirdPersonController.SetStrafeOnAim(true);

                TPSController.MoveSpeed = dragSpeed;

                currentlyDraggedObject = nearestObject;

                RotateTowardsTarget();
            }
        }

        else
        {
            StopDragging();
        }
    }

    private void RotateTowardsTarget()
    {
        if (currentlyDraggedObject != null)
        {
            Vector3 aimDirection = (currentlyDraggedObject.position - transform.position).normalized;
            aimDirection.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(aimDirection);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 20f);
        }
    }

    public void StopDragging()
    {
        isDraggingEnemy = false;
        TPSController.MoveSpeed = moveSpeed;

        if(CanDrag)
        {
            thirdPersonController.SetRotateOnMove(true);
            thirdPersonController.SetStrafeOnAim(false);
        }

        if (currentlyDraggedObject != null) currentlyDraggedObject = null;

        if (enemyAgent == null) return;
        enemyAgent.isStopped = true;
        enemyAgent = null;

    }

    private Transform FindNearestObject()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, proximityRadius);
        Transform nearestObject = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider hitCollider in hitColliders)
        {
            Transform parentTransform = hitCollider.transform.parent;

            if (parentTransform != null)
            {
                IDraggable draggableInParent = parentTransform.GetComponent<IDraggable>();

                if (draggableInParent != null && draggableInParent.CanBeDragged())
                {
                    float distanceToObject = Vector3.Distance(transform.position, parentTransform.position);

                    if (distanceToObject < minDistance)
                    {
                        nearestObject = parentTransform;
                        minDistance = distanceToObject;
                    }
                }
            }
        }

        return nearestObject;
    }

    public void TrapGoblin()
    {
        if (currentlyDraggedObject == null) return;

        Enemy enemyScript = currentlyDraggedObject.GetComponent<Enemy>();

        if (enemyScript != null)
        {
            trapGoblinSound.Play();
            enemyScript.HandleGetTrapped();
        }

        enemyAgent = null;
        isDraggingEnemy = false;

        starterAssetInputs.shoot = false;
    }
}
