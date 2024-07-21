using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour, IHittable
{
    [Header("References")]
    [SerializeField] private ParticleSystem bloodSplashPrefab;

    [Header("Parameters")]
    [SerializeField] private float health = 100;
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private bool shouldDisappearAfterDeath = false;

    private List<ParticleSystem> activeBloodSplash = new List<ParticleSystem>();
    private List<ParticleSystem> bloodSplashPool = new List<ParticleSystem>();

    public event Action onHPChange = delegate { };
    public event Action onRevive = delegate { };
    public event Action onDead = delegate { };
    public event Action onHurt = delegate { };

    public float Health => health;

    public float MaxHealth => maxHealth;

    public void SetToMaxHealth()
    {
        health = maxHealth;
        onRevive?.Invoke();
    }

    public void ReceiveDamage(float damage, Vector3 hitPoint)
	{
		health -= damage;
        onHPChange?.Invoke();

        onHurt?.Invoke();

        CreateBloodSplash(hitPoint);

        if (health <= 0)
			Die();
	}

    public void RestoreHP(float restoredHealth)
    {
        health += restoredHealth;

        if (health >= maxHealth)
        {
            health = maxHealth;
        }

        onHPChange?.Invoke();
    }

    private void CreateBloodSplash(Vector3 hitPoint)
    {
        if (!bloodSplashPrefab) return;

        GetBloodSplashFromPool(hitPoint);
    }

    private void GetBloodSplashFromPool(Vector3 hitPoint)
    {
        ParticleSystem bloodSplash;

        if (bloodSplashPool.Count == 0)
        {
            bloodSplash = Instantiate(bloodSplashPrefab, hitPoint, Quaternion.identity);
            activeBloodSplash.Add(bloodSplash);

            StartCoroutine(WaitToDeactivateBloodSplash(bloodSplash));
        }

        else
        {
            ParticleSystem reusedBloodSplash = bloodSplashPool[0];

            reusedBloodSplash.transform.position = hitPoint;
            reusedBloodSplash.transform.rotation = Quaternion.identity;

            reusedBloodSplash.gameObject.SetActive(true);

            activeBloodSplash.Add(reusedBloodSplash);
            bloodSplashPool.Remove(reusedBloodSplash);

            StartCoroutine(WaitToDeactivateBloodSplash(reusedBloodSplash));
        }
    }

    private IEnumerator WaitToDeactivateBloodSplash(ParticleSystem bloodSplash)
    {
        yield return new WaitForSeconds(0.5f);

        activeBloodSplash.Remove(bloodSplash);
        bloodSplashPool.Add(bloodSplash);

        bloodSplash.gameObject.SetActive(false);
    }

    private void Die()
    {
        onDead?.Invoke();

        if(shouldDisappearAfterDeath) gameObject.SetActive(false);
    }
}
