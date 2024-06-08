using StarterAssets;
using System;
using UnityEngine;

public class PlayerEnvironmentInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LayerMask triggerLayer;

    [SerializeField] private string buildingTag = "Building";
    [SerializeField] private string wellTag = "Well";

    private HandController handController;

    private bool isPlayerCloseToWell = false;
    private bool isAtDropSpot = false;

    public event Action onSplash = delegate { };

    public bool IsAtDropSpot => isAtDropSpot;

    private void Awake()
    {
        handController = GetComponent<HandController>();   
    }

    private void OnEnable()
    {
        handController.onClick += HandleSplash;
        handController.onClick += HandleTrapGoblin;
    }

    private void OnDisable()
    {
        handController.onClick -= HandleSplash;
        handController.onClick -= HandleTrapGoblin;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerLayer == (triggerLayer | (1 << other.gameObject.layer)))
        {
            if (other.gameObject.CompareTag(buildingTag))
            {
                if (handController.IsDraggingEnemy)
                {
                    isAtDropSpot = true;
                }
            }

            else if (other.gameObject.CompareTag(wellTag))
            {
                isPlayerCloseToWell = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (triggerLayer == (triggerLayer | (1 << other.gameObject.layer)))
        {
            if (other.gameObject.CompareTag(buildingTag))
            {
                isAtDropSpot = false;
            }

            else if (other.gameObject.CompareTag(wellTag))
            {
                isPlayerCloseToWell = false;
            }
        }
    }

    private void HandleSplash()
    {
        if (!isPlayerCloseToWell) return;
        onSplash?.Invoke();
    }

    private void HandleTrapGoblin()
    {
        if (!isAtDropSpot) return;
        handController.TrapGoblin();
    }

    public bool IsDraggingEnemy()
    {
        if (handController.IsDraggingEnemy) return true;
        else return false;
    }
}
