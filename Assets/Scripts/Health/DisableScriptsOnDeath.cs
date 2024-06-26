using StarterAssets;
using UnityEngine;

public class DisableScriptsOnDeath : MonoBehaviour
{
    [Header("References")]
    [Header("Enemies")]
    [SerializeField] private ArsonistEnemy arsonist;
    [SerializeField] private PatrolEnemy patrol;

    private StarterAssetsInputs starterAssetsInputs;
    private HealthController playerHP;
    private ThirdPersonController TPS;

    private ShooterController gun;
    private HandController hands;
    private FireExtinguisherController fireExtinguisher;

    private CharacterController CC;

    private void Awake()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        playerHP = GetComponent<HealthController>();
        TPS = GetComponent<ThirdPersonController>();

        gun = GetComponent<ShooterController>();
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
        TPS.enabled = false;

        starterAssetsInputs.aim = false;
        starterAssetsInputs.hands = true;

        gun.enabled = false;
        hands.enabled = false;

        fireExtinguisher.StopShoot();
        fireExtinguisher.enabled = false;

        CC.enabled = false;

        HandleStopEnemies();
    }

    private void HandleStopEnemies()
    {
        patrol.enabled = false;
        arsonist.enabled = false;
    }

}
