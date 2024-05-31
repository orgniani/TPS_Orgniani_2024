using StarterAssets;
using System;
using System.Collections;
using UnityEngine;

public class AttackSwapController : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;

    [SerializeField] private ShooterController gunController;
    [SerializeField] private FireExtinguisherController fireExtinguisherController;
    [SerializeField] private HandController handController;

    [Header("Weapon objects")]
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject fireExtinguisher;

    [Header("Audio")]
    [SerializeField] private AudioClip swapSound;
    private AudioSource audioSource;

    private bool canSwitch = true;

    public event Action onSwap = delegate { };

    public bool AquiredExtinguisher { get; set; }
    public bool AquiredGun { get; set; }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(starterAssetsInputs.gun)
        {
            StopAiming();

            SwapToGun();
            StartCoroutine(WaitToDisableInput());
        }

        if(starterAssetsInputs.fireExtinguisher)
        {
            StopAiming();

            SwapToFireExtinguisher();
            StartCoroutine(WaitToDisableInput());
        }

        if (starterAssetsInputs.hands)
        {
            StopAiming();

            SwapToHands();

            StartCoroutine(WaitToDisableInput());
        }
    }

    public void StopAiming()
    {
        starterAssetsInputs.aim = false;

        fireExtinguisherController.HandlePlayerAiming();
        handController.HandlePlayerAiming();
        gunController.HandlePlayerAiming();
    }

    /// <summary>
    /// Waiting time so the masks weight in the Animator can reset to their 
    /// default state properly, masks weight are handled by the WeaponController.
    /// </summary>
    public IEnumerator WaitToDisableInput()
    {
        yield return new WaitForSeconds(0.5f);

        starterAssetsInputs.hands = false;
        starterAssetsInputs.fireExtinguisher = false;
        starterAssetsInputs.gun = false;

        canSwitch = true;
    }

    public void SwapToFireExtinguisher()
    {
        if (!AquiredExtinguisher) return;

        if (!canSwitch) return;

        if (fireExtinguisher.activeSelf) return;

        canSwitch = false;

        if (gun.activeSelf)
        {
            gunController.enabled = false;
            StartCoroutine(AnimateExitSwap(gun));
        }

        else
        {
            handController.CanDrag = false;
            audioSource.PlayOneShot(swapSound);
        }

        StartCoroutine(AnimateSelectSwap(fireExtinguisher));
    }

    public void SwapToGun()
    {
        if (!AquiredGun) return;

        if (!canSwitch) return;

        if (gun.activeSelf) return;

        canSwitch = false;

        if (fireExtinguisher.activeSelf)
        {
            fireExtinguisherController.enabled = false;
            StartCoroutine(AnimateExitSwap(fireExtinguisher));
        }

        else
        {
            handController.CanDrag = false;
            audioSource.PlayOneShot(swapSound);
        }

        StartCoroutine(AnimateSelectSwap(gun));
    }

    public void SwapToHands()
    {
        if (!canSwitch) return;

        if (!gun.activeSelf && !fireExtinguisher.activeSelf) return;

        canSwitch = false;

        if (gun.activeSelf)
        {
            gunController.enabled = false;
            StartCoroutine(AnimateExitSwap(gun));
        }

        else
        {
            fireExtinguisherController.enabled = false;
            StartCoroutine(AnimateExitSwap(fireExtinguisher));
        }

        handController.CanDrag = true;
    }

    private IEnumerator AnimateExitSwap(GameObject weapon)
    {
        audioSource.PlayOneShot(swapSound);
        //onSwap?.Invoke();

        yield return new WaitForSeconds(0f);

        weapon.SetActive(false);
    }

    private IEnumerator AnimateSelectSwap(GameObject weapon)
    {
        weapon.SetActive(true);
        //onSwap?.Invoke();

        yield return new WaitForSeconds(0f);

        if (weapon == gun)
        {
            gunController.enabled = true;
        }

        else
        {
            fireExtinguisherController.enabled = true;
        }

        canSwitch = true;
    }

}