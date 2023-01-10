using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ProjectGTA2_Unity.Characters
{
    public class Character : MonoBehaviour
    {
        #region SerializedFields
      
        [SerializeField] protected Animator animator;
        [SerializeField] protected Rigidbody rb;

        [SerializeField] protected float maxHealth;
        [SerializeField] protected float walkSpeed;
        [SerializeField] protected float runSpeed;
        [SerializeField] protected float jumpForce;
        [SerializeField] protected float gravityFactor = 5f;
        [SerializeField] protected float rotationSensitivity = 5f;
        [SerializeField] protected bool canRegenHealth = false;

        [SerializeField] protected Transform groundCheck;
        [SerializeField] protected LayerMask groundLayer;

       
        #endregion

        #region private Fields

        protected float currentHealth;
        protected bool healthRegenActive;
        protected bool onGround;
        protected bool isDead;

        #endregion

        #region PublicFields
        #endregion

        #region UnityFunctions

        void Awake()
        {
            Initialize();
        }

        void Start()
        {
            AdditionalSetup();
        }

        void LateUpdate()
        {
            if(!canRegenHealth) return;
            RegenerateHealth();
        }

        void OnDestroy()
        {
            //DeathSetup();
        }

        #endregion

        #region Initialize and Destroy

        protected virtual void Initialize()
        {
            
        }

        protected virtual void AdditionalSetup()
        {
        }

        protected virtual void DeathSetup()
        {
        }

        #endregion

        #region Damage

        public virtual void TakeDamage(float damageAmount, DamageType damageType)
        {
            if (isDead) return;

            DecreaseHealth(damageAmount);
            Debug.Log($"<color=orange>{gameObject.name} takes damage - {damageAmount.ToString("0")}</color>");
        }

        #endregion

        #region Health

        protected virtual void DecreaseHealth(float amount)
        {
            if (isDead) return;

            currentHealth -= amount;

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                isDead = true;
            }

            //if (healthBar != null) healthBar.UpdateBar(currentHealth, healthmax);
        }

        protected virtual void IncreaseHealth(float amount)
        {
            if (isDead) return;

            currentHealth += amount;

            //var healthmax = characterStats.GetStatValue(StatType.HealthMax);
            var healthmax = maxHealth;
            if (healthmax < 0) return;

            if (currentHealth >= healthmax)
            {
                currentHealth = healthmax;
                if (healthRegenActive)
                {
                    CancelInvoke("HealthRegen");
                    healthRegenActive = false;
                }
            }

            //if (healthBar != null) healthBar.UpdateBar(currentHealth, healthmax);
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

        #endregion
    }
}

