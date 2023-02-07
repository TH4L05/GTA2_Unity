/// <author>Thoams Krahl</author>

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectGTA2_Unity.Characters.Data;
using ProjectGTA2_Unity.Cars;
using ProjectGTA2_Unity.Audio;
using ProjectGTA2_Unity.Weapons;

namespace ProjectGTA2_Unity.Characters
{
    public enum CharacterType
    {
        Invalid = -1,
        Player,
        NormalNPC,
        GangMember,
        CarThief,
        Mugger,
    }

    [RequireComponent(typeof(Rigidbody))]
    public class Character : MonoBehaviour, IDamagable
    {
        public static Action<string,string,string> CharacterisDead;

        #region SerializedFields

        [Header("Base")]
        [SerializeField] protected CharacterType characterType = CharacterType.Invalid;
        [SerializeField] protected CharacterData charData;
        [SerializeField] protected Rigidbody rb;
        [SerializeField] protected Animator animator;
        [SerializeField] protected Collider[] colliders;
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected AudioEventList audioEvents; 
        [SerializeField] protected float CarEnterDistance = 2f;

        [Header("Health")]
        [SerializeField] protected bool godMode = false;    
        [SerializeField] protected float maxHealth;       
        [SerializeField] protected bool canRegenHealth = false;

        [Header("Death")]
        [SerializeField] protected Sprite deathSpriteNormal;
        [SerializeField] protected float deletionTime = 5f;

        [Header("VFX")]
        [SerializeField] protected GameObject damgeVfxNormal;
        [SerializeField] protected GameObject damgeVfxFire;
        [SerializeField] protected GameObject deathVfxNormal;
        [SerializeField] protected GameObject deathVfxFire;
        [SerializeField] protected GameObject deathVfxElectro;

        #endregion

        

        #region private Fields

        protected float currentHealth;
        protected bool healthRegenActive;
        [SerializeField] protected bool onGround;
        protected bool isDead;
        protected DamageType lastDamageType;
        protected string lastDamageTag;
        protected string dotLastDamageTag;
        protected string killer;
        protected bool damageOverTime;

        #endregion

        #region PublicFields

        public CharacterType CharacterType
        {
            get { return characterType; }
        }

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

        void Update()
        {

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
            rb = GetComponent<Rigidbody>();
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
            killer = character;

            switch (damageType)
            {
                case DamageType.Invalid:
                    break;

                case DamageType.Normal:
                    audioEvents.PlayAudioEventOneShotAttached("DamageNormal", gameObject);
                    break;

                case DamageType.Fire:
                    if (!damageOverTime) DamageOverTime(1.15f, character);
                    return;

                case DamageType.Water:
                    if (damageOverTime) CancelInvoke("TakeDamageOverTime");                 
                    break;

                case DamageType.Electro:
                    break;

                case DamageType.Car:
                    break;

                default:
                    break;
            }

            DecreaseHealth(damageAmount);         
            Debug.Log($"<color=orange>{gameObject.name} takes {damageAmount} {damageType} damage by {character}</color>");
        }

        private IEnumerator TakeDamageOverTime(float repeatTime)
        {
            while (currentHealth > 0)
            {
                yield return new WaitForSeconds(repeatTime);
                DecreaseHealth(10f);
                audioEvents.PlayAudioEventOneShotAttached("CharacterOnFireScream", gameObject);
            }        
        }

        private void DamageOverTime(float repeatTime, string c)
        {
            dotLastDamageTag = c;
            damageOverTime = true;
            audioEvents.Create3DEvent("CharacterOnFire", transform);
            StartCoroutine(TakeDamageOverTime(repeatTime));
            //InvokeRepeating("TakeDamageOverTime", 0f, repeatTime);
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
            healthRegenActive = false;
            foreach (var coll in colliders)
            {
                coll.enabled = false;
            }

            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.drag = 99f;
            spriteRenderer.sortingOrder = 1;

            if(damageOverTime)
            {
                damageOverTime = false;
                StopCoroutine(TakeDamageOverTime(0f));
            }
            audioEvents.RemoveAllEvents();
            audioEvents.PlayAudioEventOneShotAttached("CharacterScreamDeath", gameObject);
            
            switch (lastDamageType)
            {
                case DamageType.Invalid:
                    break;

                case DamageType.Normal:
                    spriteRenderer.sprite = deathSpriteNormal;
                    if (animator != null) animator.SetTrigger("Dead");
                    if (deathVfxNormal != null) Instantiate(deathVfxNormal, transform.position, Quaternion.Euler(90f,0f,0f), transform);
                    break;

                case DamageType.Fire:
                    spriteRenderer.sprite = deathSpriteNormal;
                    if (animator != null) animator.SetTrigger("Dead");
                    break;

                case DamageType.Water:
                    audioEvents.PlayAudioEventOneShotAttached("Splash", gameObject);
                    spriteRenderer.enabled = false;
                    if (animator != null) animator.SetTrigger("Dead");                  
                    break;

                case DamageType.Electro:
                    spriteRenderer.sprite = deathSpriteNormal;
                    if (animator != null) animator.SetTrigger("Dead");
                    break;

                case DamageType.Car:
                    audioEvents.PlayAudioEventOneShotAttached("CarHit", gameObject);
                    spriteRenderer.sprite = deathSpriteNormal;
                    if (animator != null) animator.SetTrigger("Dead");
                    break;

                default:
                    break;
            }

            
            CharacterisDead?.Invoke(gameObject.name, lastDamageTag, killer);
        }

        #endregion

        protected void CheckNearbyCarsToEnter()
        {
            Collider[] carColliders = Physics.OverlapSphere(transform.position, CarEnterDistance, 1 << 8);
            List<Car> cars = new List<Car>();

            if (carColliders.Length < 1)
            {
                Debug.Log("NO Cars in Range");
                return;
            }

            foreach (var item in carColliders)
            {
                var car = item.gameObject.GetComponent<Car>();

                if (car != null) cars.Add(car);
            }

            if (cars.Count == 0) return;

            int index = 0;
            float distance = 999f;

            for (int i = 0; i < cars.Count; i++)
            {
                float lastDistance = distance;
                distance = Vector3.Distance(transform.position, cars[i].transform.position);

                if (distance < lastDistance)
                {
                    index = i;
                }
            }

            cars[index].CharacterEnter(this);           
        }
    }
}

