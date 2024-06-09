using StarterAssets;
using System;
using UnityEngine;

public class PlayerEnvironmentInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LayerMask triggerLayer;

    [SerializeField] private string buildingTag = "Building";
    [SerializeField] private string wellTag = "Well";

    [Header("Text")]
    [SerializeField] private GameObject instructionsCanvas;

    private HandController handController;

    private bool isPlayerCloseToWell = false;
    private bool isAtDropSpot = false;

    public event Action onSplash = delegate { };

    private void Awake()
    {
        handController = GetComponent<HandController>();   
    }

    private void Update()
    {
        if (handController.IsDraggingEnemy && isAtDropSpot)
        {
            instructionsCanvas.SetActive(true);
        }

        else
        {
            instructionsCanvas.SetActive(false);
        }
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

    private void OnTriggerExit(Collider other)
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
