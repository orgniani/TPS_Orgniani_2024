using Cinemachine;
using MilkShake;
using StarterAssets;
using System.Collections;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    [Header("Shake")]
    [SerializeField] private ShakePreset explotionShakePreset;
    [SerializeField] private ShakePreset punchShakePreset;

    [SerializeField] private CinemachineVirtualCamera shakeVirtualCamera;

    [SerializeField] private float shakeDuration = 0.5f;

    [Header("Player")]
    [SerializeField] private ThirdPersonController thirdPersonController;
    [SerializeField] private HealthController playerHP;

    private Shaker shaker;

    private void Awake()
    {
        shaker = GetComponent<Shaker>();
    }

    private void OnEnable()
    {
        BombEnemy.onExplode += HandleExplotionShake;
        playerHP.onHurt += HandlePunchShake;
    }

    private void OnDisable()
    {
        BombEnemy.onExplode -= HandleExplotionShake;
        playerHP.onHurt -= HandlePunchShake;
    }

    private void HandleExplotionShake()
    {
        shakeVirtualCamera.gameObject.SetActive(true);
        thirdPersonController.SetStopLookInputOnShake(true);

        shaker.Shake(explotionShakePreset);

        StartCoroutine(EndCameraShake());
    }

    private void HandlePunchShake()
    {
        shakeVirtualCamera.gameObject.SetActive(true);
        thirdPersonController.SetStopLookInputOnShake(true);

        shaker.Shake(punchShakePreset);

        StartCoroutine(EndCameraShake());
    }

    private IEnumerator EndCameraShake()
    {
        yield return new WaitForSeconds(shakeDuration);

        shakeVirtualCamera.gameObject.SetActive(false);
        thirdPersonController.SetStopLookInputOnShake(false);
    }
}
