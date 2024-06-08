using StarterAssets;
using UnityEngine;

public class FireExtinguisherController : AttackController
{
    [Header("References")]
    [SerializeField] private ParticleSystem fireFoam;
    [SerializeField] private AudioSource extinguishSound;


    private void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;

        if (!enabled) return;

        if (hasAnimator)
        {
            animator.SetTrigger(animIDExtinguisher);
        }

        HandlePlayerAiming();

        if (starterAssetInputs.shoot)
        {
            Shoot();
        }

        else
        {
            StopShoot();
        }
    }


    public override void Shoot()
    {
        if(!extinguishSound.isPlaying && !fireFoam.isPlaying)
        {
            fireFoam.Play();
            extinguishSound.Play();
        }
    }

    public void StopShoot()
    {
        fireFoam.Stop();
        extinguishSound.Stop();
    }

}
