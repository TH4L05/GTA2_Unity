/// <author>Thoams Krahl</author>

using System.Collections.Generic;
using UnityEngine;
using ProjectGTA2_Unity.Characters;
using System;

namespace ProjectGTA2_Unity
{
    public enum CarType
    {
        Invalid = -1,
        Compact,
        SportsCar,
        SuperCar,
        Truck,
        Bus,
        Taxi,
        Tank,
        GangCar,
    }

    [RequireComponent(typeof(Rigidbody))]
    public class Car : MonoBehaviour, IDamagable
    {
        #region Events

        public static Action<string> PlayerEntersCar;

        #endregion

        #region SerializedFields

        [SerializeField] protected new string name;
        [SerializeField] protected CarType carType = CarType.Invalid;
        [SerializeField] protected SpriteRenderer sr;
        [SerializeField] protected bool fixedCarColor = false;
        [SerializeField] protected Color carColor;
        [SerializeField] protected Sprite destroyedSprite;
        [SerializeField] protected GameObject carDeltasRoot;
        [SerializeField] protected GameObject carDeltasLightsRoot;

        [SerializeField] protected float maxHealth;
        [SerializeField] protected CarMovement carMovementComponent;
        [SerializeField] protected Transform[] carEntryPoints;
        [SerializeField] protected GameObject[] destroyVFX;
        [SerializeField] protected SpriteRenderer[] damageSpriteRenderers;
        [SerializeField] protected LayerMask groundLayer;
        [SerializeField] protected AudioEventList audioEvents;
        [SerializeField] protected bool isParked = false;

        #endregion

        #region PrivateFields

        protected List<Character> passangers = new List<Character>();
        protected Rigidbody rb;
        protected bool isActive = false;       
        protected float currentHealth;
        protected bool isDestroyed;
        protected DamageType lastDamageType;

        #endregion

        #region UnityFunctions

        private void Awake()
        {
            Initialize();
        }

        private void Start()
        {
            StartSetup();
        }

        private void Update()
        {
            if(!isActive) return;

            if (Input.GetKeyDown(KeyCode.E))
            {
                CharacterExit();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            var damagable = collision.collider.GetComponent<IDamagable>();
          
            if (damagable != null)
            {
                (float, float, float, CarMovement.MovementDirection) carValues = carMovementComponent.MovementValues;

                if (carValues.Item1 == 0) return;
                bool hit = false;

                switch (carValues.Item4)
                {
                    case CarMovement.MovementDirection.Forward:
                        if (carValues.Item1 > carValues.Item2 * 0.33f) hit = true;
                        break;

                    case CarMovement.MovementDirection.Backward:
                        if (carValues.Item1 > carValues.Item3 * 0.33f) hit = true;
                        break;


                    default:
                        break;
                }

                if(hit)
                {
                    Debug.Log("Damagable Hit");

                    if (collision.collider.gameObject.tag == "Player" || collision.collider.gameObject.tag == "NPC")
                    {
                        damagable.TakeDamage(999f, DamageType.Car);
                    }
                    else
                    {
                        damagable.TakeDamage(5f, DamageType.Car);
                    }                                     
                }                
            }
            else
            {

            }
        }

        #endregion

        #region Setup

        private void Initialize()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void StartSetup()
        {
            rb.isKinematic = true;
            currentHealth = maxHealth;

            if (isParked) carDeltasLightsRoot.SetActive(false);

            if (fixedCarColor) return;
            GetRandomColor();
            SetCarColors(carColor);

        }

        #endregion

        #region Passangers

        public void CharacterEnter(Character character)
        {         
            if(isDestroyed) return;           
            rb.isKinematic = false;
            character.gameObject.SetActive(false);
            passangers.Add(character);

            if (character.CompareTag("Player"))
            {
                PlayerCamera.SetCameraTarget(transform);
                gameObject.tag = "Player";
                PlayerEntersCar?.Invoke(name);
            }
            else
            {
                gameObject.tag = "NPC";
            }
                              
            if (passangers.Count > 1) return;

            if (isParked)
            {
                StartEngine();
            }
            else
            {
                carMovementComponent.SetActive(isActive);
                isActive = true;
            }
        }

        public void CharacterExit()
        {
            if (passangers.Count < 2)
            {
                isActive = false;
                carMovementComponent.SetActive(isActive);
                rb.isKinematic = true;
            }

            foreach (Character character in passangers)
            {
                CheckCarExit(character);
                character.gameObject.SetActive(true);

                //var player = character as Player;
                //Debug.Log(player.gameObject.name);

                if (character.CompareTag("Player"))
                {
                    PlayerCamera.SetCameraTarget(character.transform);
                }              
            }

            passangers.Clear();
            gameObject.tag = "Car";
        }

        private void CheckCarExit(Character character)
        {
            // TODO: Check if Exit is blocked use other Point when Point 0 is not avialable
            character.gameObject.transform.position = carEntryPoints[0].position;
        }
        
        private void StartEngine()
        {
            audioEvents.PlayAudioEventOneShot("StartEngine1");
            carDeltasLightsRoot.SetActive(true);
            isActive = true;
            isParked = false;
            carMovementComponent.SetActive(isActive);
        }

        #endregion

        #region Color

        private void GetRandomColor()
        {
            carColor = Game.Instance.GetRandomCarColor();
        }

        private void SetCarColors(Color color)
        {
            if (sr != null) sr.color = carColor;

            foreach (var sr in damageSpriteRenderers)
            {
                sr.color = carColor;
            }
        }

        #endregion

        #region DamageAndDestroy

        public void TakeDamage(float damageAmount, DamageType damageType)
        {
            if (isDestroyed) return;
            lastDamageType = damageType;

            DecreaseHealth(damageAmount);
            Debug.Log($"<color=orange>{gameObject.name} takes {damageAmount.ToString("0")} {damageType} damage</color>");
        }

        public void DecreaseHealth(float amount)
        {
            if (isDestroyed) return;

            currentHealth -= amount;

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                isDestroyed = true;
                OnCarDestroy();
            }
        }

        private void OnCarDestroy()
        {
            rb.isKinematic = true;
            carDeltasRoot.SetActive(false);

            foreach (Character charInside in passangers)
            {
                charInside.TakeDamage(999f, lastDamageType);
            }
                  
            switch (lastDamageType)
            {
                case DamageType.Invalid:
                    break;

                case DamageType.Normal:
                    Destroy(gameObject, 60f);
                    sr.color = Color.white;
                    sr.sprite = destroyedSprite;
                    SpawnDestroyVFX();
                    audioEvents.PlayAudioEventOneShot("ExplosionLarge");
                    break;

                case DamageType.Fire:
                    Destroy(gameObject, 60f);
                    sr.color = Color.white;
                    sr.sprite = destroyedSprite;
                    SpawnDestroyVFX();
                    audioEvents.PlayAudioEventOneShot("ExplosionLarge");
                    break;

                case DamageType.Water:
                    Destroy(gameObject, 5f);
                    sr.enabled = false;
                    var collider = GetComponent<Collider>() as BoxCollider;
                    collider.enabled = false;
                    audioEvents.PlayAudioEventOneShot("Splash");
                    break;

                case DamageType.Electro:
                    Destroy(gameObject, 60f);
                    sr.color = Color.white;
                    sr.sprite = destroyedSprite;
                    SpawnDestroyVFX();
                    audioEvents.PlayAudioEventOneShot("ExplosionLarge");
                    break;

                case DamageType.Car:
                    Destroy(gameObject, 60f);
                    sr.color = Color.white;
                    sr.sprite = destroyedSprite;
                    SpawnDestroyVFX();
                    audioEvents.PlayAudioEventOneShot("ExplosionLarge");
                    break;

                case DamageType.CopNormal:
                    break;

                case DamageType.CopGun:
                    break;

                default:
                    break;
            }
        }

        private void SpawnDestroyVFX()
        {
            if (destroyVFX.Length == 0) return;

            foreach (var vfx in destroyVFX)
            {
                var newDestroyVFX = Instantiate(vfx, transform.position, Quaternion.Euler(90f, 0f, 0f));
            }
        }

        #endregion

    }
}