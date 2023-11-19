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
   

    [RequireComponent(typeof(Rigidbody))]
    public class Character : MonoBehaviour, IDamagable
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

        public static Action<string,string> CharacterisDead;

        #region SerializedFields

        [Header("Base")]
        [SerializeField] protected CharacterType charType = CharacterType.Invalid;
        [SerializeField] protected CharacterData charData;
        [SerializeField] protected Rigidbody rb;
        [SerializeField] protected Animator animator;
        [SerializeField] protected Collider[] colliders;
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected AudioEventList audioEvents; 
        [SerializeField] protected float CarEnterDistance = 2f;

        [Header("Health")]
        [SerializeField] protected bool godMode = false;
        [SerializeField] protected Health health;

        [Header("Death")]
        [SerializeField] protected Sprite deathSpriteNormal;
        [SerializeField] protected float deletionTime = 2f;

        [Header("VFX")]
        [SerializeField] protected GameObject damgeVfxNormal;
        [SerializeField] protected GameObject damgeVfxFire;
        [SerializeField] protected GameObject deathVfxNormal;
        [SerializeField] protected GameObject deathVfxFire;
        [SerializeField] protected GameObject deathVfxElectro;

        #endregion

        #region private Fields
     
        protected bool onGround;
        protected bool isDead;      
        protected string killer;
        protected DamageType lastDamageType;
        #endregion

        #region PublicFields

        public CharacterType CharType
        {
            get { return charType; }
        }

        #endregion

        #region UnityFunctions

        void Awake()
        {
            Initialize();
        }

        void OnEnable()
        {
            OnEnbaleSetup();
        }

        void Start()
        {
            StartSetup();
        }
    
        void OnDestroy()
        {
            //DeathSetup();
        }

        #endregion

        #region Initialize and Destroy

        protected virtual void Initialize()
        {
            rb = GetComponent<Rigidbody>();
        }

        protected virtual void StartSetup()
        {
        }

        protected virtual void OnEnbaleSetup()
        {
        }

        protected virtual void DeathSetup()
        {
        }

        #endregion

        #region Damage

        public virtual void TakeDamage(float damageAmount, DamageType damageType, string character)
        {
            if (godMode) return;
            if (isDead) return;
            
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
                    health.DamageOverTime(1.15f);
                    return;

                case DamageType.Water:
                    health.CancelDamageOverTime();             
                    break;

                case DamageType.Electro:
                    break;

                case DamageType.Car:
                    break;

                default:
                    break;
            }

           
            health.DecreaseHealth(damageAmount);
            Debug.Log($"<color=orange>{gameObject.name} takes {damageAmount} {damageType} damage by {character}</color>");
            if (health.IsDead)
            {
                Death();
            }
        }
      
        #endregion

        #region Health

        protected virtual void Death()
        {
            isDead = true;
            
            foreach (var coll in colliders)
            {
                coll.enabled = false;
            }

            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            rb.drag = 99f;
            spriteRenderer.sortingOrder = 1;

            
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

            
            CharacterisDead?.Invoke(gameObject.name, killer);
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

