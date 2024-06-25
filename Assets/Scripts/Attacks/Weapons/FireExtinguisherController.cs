using UnityEngine;
using static AttackSwapController;

public class FireExtinguisherController : AttackController
{
    [Header("References")]
    [SerializeField] private ParticleSystem fireFoam;
    [SerializeField] private AudioSource extinguishSound;


    private void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;

        if (activeWeapon != ActiveAttackSetter.ActiveWeapon.EXTINGUISHER) return;

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
