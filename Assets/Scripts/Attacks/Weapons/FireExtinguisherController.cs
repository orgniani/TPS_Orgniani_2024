using UnityEngine;
using UnityEngine.Animations.Rigging;

public class FireExtinguisherController : AttackController
{
    [Header("References")]
    [SerializeField] private Rig casualHoldRig;
    [SerializeField] private ParticleSystem fireFoam;
    [SerializeField] private AudioSource extinguishSound;

    private float casualHoldRigWeight;

    private void Update()
    {
        casualHoldRig.weight = Mathf.Lerp(casualHoldRig.weight, casualHoldRigWeight, Time.deltaTime * 20f);

        if (activeWeapon != ActiveAttackSetter.ActiveWeapon.EXTINGUISHER)
        {
            StopShoot();
            casualHoldRigWeight = 0;
            return;
        }

        if (hasAnimator)
        {
            animator.SetTrigger(animIDExtinguisher);
        }

        HandlePlayerAiming();

        if(starterAssetInputs.aim)
        {
            casualHoldRigWeight = 0;
        }

        else
        {
            casualHoldRigWeight = 1;
        }

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
