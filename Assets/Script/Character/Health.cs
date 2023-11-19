using ProjectGTA2_Unity.Audio;
using ProjectGTA2_Unity.Weapons;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Health : MonoBehaviour
{
    public static Action<float, float> OnHealthChanged;

    [SerializeField, Range(1.0f, 1000.0f)] protected float maxHealth = 100;
    [SerializeField] protected bool canRegenHealth = false;

    private bool isPlayer;
    private float currentHealth;
    private bool healthRegenActive;
    private bool damageOverTime;
    private Coroutine damageOverTimeRoutine;

    public bool IsDead
    {
        get
        {
            if (currentHealth <= 0)
            {
                return true;
            }
            return false;
        }
    }



    void LateUpdate()
    {
        if (!canRegenHealth) return;
        RegenerateHealth();
    }

    public void Initialize(bool isPlayer)
    {
        currentHealth = maxHealth;
        this.isPlayer = isPlayer;

        if (!this.isPlayer) return;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }


    public virtual void DecreaseHealth(float amount)
    {
        if (IsDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            NoHealthLeft();
        }

        if (!isPlayer) return;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public virtual void IncreaseHealth(float amount)
    {
        if (maxHealth < 0) return;

        currentHealth += amount;

        if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
            if (healthRegenActive)
            {
                CancelInvoke("HealthRegen");
                healthRegenActive = false;
            }
        }

        if (!isPlayer) return;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void RegenerateHealth()
    {
        var healthMax = maxHealth;
        if (healthMax < 0) return;

        if (currentHealth < healthMax && !healthRegenActive)
        {
            InvokeRepeating("HealthRegen", 1f, 1f);
            healthRegenActive = true;
        }
    }

    private void HealthRegen()
    {
        var healthRegen = maxHealth;
        if (healthRegen < 0)
        {
            CancelInvoke("HealthRegen");
            healthRegenActive = false;
        }

        IncreaseHealth(healthRegen);
    }

    private IEnumerator TakeDamageOverTime(float repeatTime)
    {
        while (!IsDead)
        {
            yield return new WaitForSeconds(repeatTime);
            DecreaseHealth(10f);
            //audioEvents.PlayAudioEventOneShotAttached("CharacterOnFireScream", gameObject);
        }
    }
    
    public void CancelDamageOverTime()
    {
        if(!damageOverTime) return;
        StopCoroutine(damageOverTimeRoutine);
    }

    public void DamageOverTime(float repeatTime)
    {   
        damageOverTime = true;
        //audioEvents.Create3DEvent("CharacterOnFire", transform);
        damageOverTimeRoutine = StartCoroutine(TakeDamageOverTime(repeatTime));
        //InvokeRepeating("TakeDamageOverTime", 0f, repeatTime);
    }


    public void NoHealthLeft()
    {
        healthRegenActive = false;

        if (damageOverTime)
        {
            damageOverTime = false;
            StopCoroutine(damageOverTimeRoutine);
        }
    }
}
