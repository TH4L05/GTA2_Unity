/// <author>Thoams Krahl</author>

using System;
using System.Collections.Generic;
using UnityEngine;
using ProjectGTA2_Unity.Characters.Data;

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
        public static Action<string,string> CharacterisDead;

        #region SerializedFields

        [Header("Base")]
        [SerializeField] protected CharacterType characterType = CharacterType.Invalid;
        [SerializeField] protected CharacterData charData;
        [SerializeField] protected Rigidbody rb;
        [SerializeField] protected Animator animator;
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

            switch (damageType)
            {
                case DamageType.Invalid:
                    break;

                case DamageType.Normal:
                    charData.AudioEvents.PlayAudioEventOneShot("DamageNormal");
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
            Debug.Log($"<color=orange>{gameObject.name} takes {damageAmount.ToString("0")} damage by {character}</color>");
        }

        private void TakeDamageOverTime()
        {
            DecreaseHealth(10f);
            audioEvents.PlayAudioEventOneShot("CharacterOnFireScream");
        }

        private void DamageOverTime(float repeatTime, string c)
        {
            dotLastDamageTag = c;
            damageOverTime = true;
            audioEvents.Create3DEvent("CharacterOnFire", transform);
            InvokeRepeating("TakeDamageOverTime", 0f, repeatTime);
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
            damageOverTime = false;
            healthRegenActive = false;
            boxCollider.enabled = false;
            rb.isKinematic = true;
            spRend.sortingOrder = 1;

            CancelInvoke();
            
            switch (lastDamageType)
            {
                case DamageType.Invalid:
                    break;

                case DamageType.Normal:
                    spRend.sprite = deathSprite;
                    if (animator != null) animator.SetTrigger("Dead");
                    break;

                case DamageType.Fire:
                    spRend.sprite = deathSprite;
                    if (animator != null) animator.SetTrigger("Dead");
                    break;

                case DamageType.Water:
                    audioEvents.PlayAudioEventOneShot("Splash");
                    spRend.enabled = false;
                    if (animator != null) animator.SetTrigger("Dead");                  
                    break;

                case DamageType.Electro:
                    spRend.sprite = deathSprite;
                    if (animator != null) animator.SetTrigger("Dead");
                    break;

                case DamageType.Car:
                    audioEvents.PlayAudioEventOneShot("CarHit");
                    spRend.sprite = deathSprite;
                    if (animator != null) animator.SetTrigger("Dead");
                    break;

                default:
                    break;
            }

            CharacterisDead?.Invoke(gameObject.tag, lastDamageTag);
            audioEvents.RemoveAllEvents();
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

