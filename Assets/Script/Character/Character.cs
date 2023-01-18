/// <author>Thoams Krahl</author>

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectGTA2_Unity.Characters
{
    [RequireComponent(typeof(Rigidbody))]
    public class Character : MonoBehaviour, IDamagable
    {
        public static Action<string> CharacterisDead;

        #region SerializedFields

        [Header("Base")]
        [SerializeField] protected CharacterData charData;
        [SerializeField] protected Rigidbody rb;
        [SerializeField] protected BoxCollider boxCollider;
        [SerializeField] protected SpriteRenderer spRend;
        [SerializeField] protected AudioEventList audioEvents; 
        [SerializeField] protected float CarEnterDistance = 2f;

        [Header("Health")]
        [SerializeField] protected bool godMode = false;    
        [SerializeField] protected float maxHealth;       
        [SerializeField] protected bool canRegenHealth = false;

        [Header("Death")]
        [SerializeField] protected Sprite deathSprite;
        [SerializeField] protected GameObject deathVfx;
        [SerializeField] protected float deletionTime = 5f;

        #endregion

        #region private Fields

        protected float currentHealth;
        protected bool healthRegenActive;
        [SerializeField] protected bool onGround;
        protected bool isDead;
        protected DamageType lastDamageType;
        protected string lastDamageTag;

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
            StartSetup();
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
            currentHealth = maxHealth;
        }

        protected virtual void StartSetup()
        {
        }

        protected virtual void DeathSetup()
        {
        }

        #endregion

        #region Damage

        public virtual void TakeDamage(float damageAmount, DamageType damageType, string character)
        {
            if(godMode || isDead) return;

            lastDamageType = damageType;
            DecreaseHealth(damageAmount);
            Debug.Log($"<color=orange>{gameObject.name} takes {damageAmount.ToString("0")} damage by {character}</color>");
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
                Death();
            }

            //if (healthBar != null) healthBar.UpdateBar(currentHealth, healthmax);
        }

        protected virtual void IncreaseHealth(float amount)
        {
            if (isDead) return;
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

        protected virtual void Death()
        {
            isDead = true;
            boxCollider.enabled = false;
            rb.isKinematic = true;
            spRend.sortingOrder = 1;          
            CharacterisDead?.Invoke(gameObject.tag);

            switch (lastDamageType)
            {
                case DamageType.Invalid:
                    break;

                case DamageType.Normal:
                    spRend.sprite = deathSprite;
                    break;

                case DamageType.Fire:
                    spRend.sprite = deathSprite;
                    break;

                case DamageType.Water:
                    spRend.enabled = false;
                    audioEvents.PlayAudioEventOneShot("Splash");
                    break;

                case DamageType.Electro:
                    spRend.sprite = deathSprite;
                    break;

                case DamageType.Car:
                    audioEvents.PlayAudioEventOneShot("CarHit");
                    spRend.sprite = deathSprite;
                    break;

                case DamageType.CopNormal:
                    break;

                case DamageType.CopGun:
                    break;

                default:
                    break;
            }

            Destroy(gameObject, deletionTime);
        }

        #endregion

        protected void CheckNearbyCarsToEnter()
        {
            Collider[] carColliders = Physics.OverlapSphere(transform.position, CarEnterDistance, 1 << 8);

            if (carColliders.Length < 1)
            {
                Debug.Log("NO Cars in Range");
                return;
            }

            int index = 0;
            float distance = 999f;

            for (int i = 0; i < carColliders.Length; i++)
            {
                float lastDistance = distance;
                distance = Vector3.Distance(transform.position, carColliders[i].transform.position);

                if (distance < lastDistance)
                {
                    index = i;
                }
            }

            //Debug.Log(carColliders[index].gameObject.name);

            List<Component> results = new List<Component>();

            foreach (var item in results)
            {
                Debug.Log(item.name);
            }

            var car = carColliders[index].gameObject.GetComponent<Car>();

            if (car != null)
            {
                car.CharacterEnter(this);
            }
            else
            {
                Debug.Log("NO CAR");
            }
        }
    }
}

