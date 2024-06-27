using StarterAssets;
using UnityEngine;

public class DisableScriptsOnDeath : MonoBehaviour
{
    [Header("References")]
    [Header("Player Collider")]
    [SerializeField] private Collider playerCollider;

    [Header("Enemies")]
    [SerializeField] private ArsonistEnemy arsonist;
    [SerializeField] private PatrolEnemy patrol;

    private StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdPersonController;
    private HealthController playerHP;

    private HandController hands;
    private FireExtinguisherController fireExtinguisher;

    private CharacterController CC;

    private void Awake()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        playerHP = GetComponent<HealthController>();

        hands = GetComponent<HandController>();
        fireExtinguisher = GetComponent<FireExtinguisherController>();

        CC = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        playerHP.onDead += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        playerHP.onDead -= HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        playerHP.enabled = false;
        thirdPersonController.enabled = false;

        starterAssetsInputs.aim = false;
        starterAssetsInputs.hands = true;

        hands.enabled = false;

        fireExtinguisher.StopShoot();
        fireExtinguisher.enabled = false;

        CC.enabled = false;

        playerCollider.enabled = false;

        HandleStopEnemies();
    }

    private void HandleStopEnemies()
    {
        patrol.enabled = false;
        arsonist.enabled = false;
    }

}
