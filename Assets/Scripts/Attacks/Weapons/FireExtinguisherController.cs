using StarterAssets;
using UnityEngine;

public class FireExtinguisherController : WeaponController
{
    [Header("References")]
    [SerializeField] private ParticleSystem fireFoam;
    [SerializeField] private AudioSource extinguishSound;

    private void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;
        if (!enabled) return;

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
        fireFoam.Play();

        if(fireFoam.isPlaying )
        {
            Debug.Log("spraying");
        }
        extinguishSound.Play();
    }

    private void StopShoot()
    {
        fireFoam.Stop();
        extinguishSound.Stop();
    }
}
