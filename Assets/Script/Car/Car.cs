/// <author>Thoams Krahl</author>

using System;
using System.Collections.Generic;
using UnityEngine;
using ProjectGTA2_Unity.Tiles;
using ProjectGTA2_Unity.Characters;
using ProjectGTA2_Unity.Audio;
using ProjectGTA2_Unity.Weapons;

namespace ProjectGTA2_Unity.Cars
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

        [Header("Ref")]
        [SerializeField] protected LayerMask groundLayer;
        [SerializeField] protected CarMovement carMovement;
        [SerializeField] protected AudioEventList audioEvents;
        [SerializeField] protected CarCollider[] carColliders;
 
        [Header("Settings")]
        [SerializeField] protected new string name;
        [SerializeField] protected CarType carType = CarType.Invalid;
        [SerializeField] protected float maxHealth;
        [SerializeField] protected bool fixedCarColor = false;
        [SerializeField] protected Transform[] carEntryPoints;
        [SerializeField] protected bool isParked = false;
        [SerializeField] protected bool playerControlled;

        [Header("Visuals")]
        [SerializeField] protected GameObject carDeltasRoot;
        [SerializeField] protected SpriteRenderer spriteRendererMain;
        [SerializeField] protected Sprite destroyedSprite;
        [SerializeField] protected SpriteRenderer[] damageSpriteRenderers;
        [SerializeField] protected SpriteRenderer[] lightSpriteRenderers;

        [Header("VFX")]
        [SerializeField] protected GameObject[] destroyVFX;

        #endregion

        #region PrivateFields

        protected List<Character> passangers = new List<Character>();
        protected Rigidbody rb;
        protected bool isActive = false;       
        protected float currentHealth;
        protected bool isDestroyed;
        protected DamageType lastDamageType;
        protected Transform charOrgParentTransform;

        #endregion

        #region PublicFields

        public bool IsParked => isParked;
        public bool IsPlayerControlled => playerControlled;
        public bool IsDestroyed => isDestroyed;
        public CarMovement CarMovement => carMovement;

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

            /*if (playerControlled && Input.GetKeyDown(KeyCode.E))
            {
                CharacterExit();
            }*/
        }

        private void OnDestroy()
        {
            foreach (var carCollider in carColliders)
            {
                carCollider.onHit -= OnHit;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            var damagable = collision.collider.GetComponent<IDamagable>();
          
            if (damagable != null)
            {
                (float, float, float, CarMovement.MovementDirection) carValues = (0f,0f,0f, CarMovement.MovementDirection.Forward);

                carValues = carMovement.MovementValues;


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
                        damagable.TakeDamage(999f, DamageType.Car, gameObject.tag);
                    }
                    else
                    {
                        damagable.TakeDamage(5f, DamageType.Car,gameObject.tag);
                    }                                     
                }                
            }
            else
            {
                var tile = collision.collider.GetComponent<Tile>();

                if (tile && tile.GetTileTypeA() == TileTypeMain.Wall)
                {
                }
            }


        }

        #endregion

        #region Setup

        private void Initialize()
        {
            rb = GetComponent<Rigidbody>();

            foreach (var carCollider in carColliders)
            {
                carCollider.onHit += OnHit;
            }
        }

        private void StartSetup()
        {
            currentHealth = maxHealth;
            //rb.isKinematic = true;

            if (isParked) EnableDisableCarLights(false);   
            EnableDisableCarDamage(false);
            Color color = Game.Instance.GetRandomCarColor();
            SetCarColor(color);
        }

        #endregion

        #region Passangers

        public void CharacterEnter(Character character)
        {         
            if(isDestroyed) return;           
            rb.isKinematic = false;
            character.gameObject.SetActive(false);
            passangers.Add(character);
            gameObject.name = character.gameObject.name;

            if (character.CompareTag("Player"))
            {
                PlayerCamera.SetCameraTarget(transform);
                gameObject.tag = "Player";
                gameObject.layer = character.gameObject.layer;
                PlayerEntersCar?.Invoke(name);
                playerControlled = true;
            }
            else
            {
                gameObject.tag = "NPC";
                playerControlled = false;
            }
                              
            if (passangers.Count > 1) return;

            StartEngine();
        }

        public void CharacterExit()
        {
            if (passangers.Count < 2)
            {
                isActive = false;
                carMovement.SetActive(false, isActive);
                rb.isKinematic = true;
            }

            foreach (Character character in passangers)
            {
                CheckCarExit(character);
                character.gameObject.SetActive(true);
                //character.transform.parent = charOrgParentTransform;

                //var player = character as Player;
                //Debug.Log(player.gameObject.name);

                if (character.CompareTag("Player"))
                {
                    PlayerCamera.SetCameraTarget(character.transform);
                    playerControlled = false;
                }              
            }

            ChangeActivationAndMovementStatus(false, playerControlled);
            passangers.Clear();
            gameObject.name = name;
            gameObject.tag = "Car";
            gameObject.layer = LayerMask.NameToLayer("Car");
        }

        private void CheckCarExit(Character character)
        {
            // TODO: Check if Exit is blocked use other Point when Point 0 is not avialable
            character.gameObject.transform.position = carEntryPoints[0].position;
        }
        
        #endregion

        #region Color

        private void SetCarColor(Color color)
        {
            if (fixedCarColor) return;
            var material = spriteRendererMain.material;
            material.SetColor("_CarColor", color);

            foreach (var spriteRenderer in damageSpriteRenderers)
            {
                material = spriteRenderer.material;
                material.SetColor("_CarColor", color);
            }

        }

        #endregion

        #region DamageAndDestroy

        public void TakeDamage(float damageAmount, DamageType damageType, string character)
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
                charInside.TakeDamage(999f, lastDamageType, gameObject.tag);
            }
            passangers.Clear();

            string audioEventName = string.Empty;
            float deleteTime = 60f;

            switch (lastDamageType)
            {
                case DamageType.Invalid:
                    break;

                case DamageType.Normal:
                    audioEventName = "ExplosionLarge";
                    break;

                case DamageType.Fire:
                    audioEventName = "ExplosionLarge";
                    break;

                case DamageType.Water:
                    spriteRendererMain.enabled = false;
                    var collider = GetComponent<Collider>() as BoxCollider;
                    collider.enabled = false;
                    deleteTime = 5f;
                    audioEventName = "Splash";
                    break;

                case DamageType.Electro:
                    audioEventName = "ExplosionLarge";
                    break;

                case DamageType.Car:               
                    audioEventName = "ExplosionLarge";
                    break;

                default:
                    break;
            }

            SetMainSprite(destroyedSprite, Color.white);
            SpawnDestroyVFX();
            if (string.IsNullOrEmpty(audioEventName)) return;
            audioEvents.PlayAudioEventOneShotAttached("ExplosionLarge", gameObject);
            Destroy(gameObject, deleteTime);
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

        #region LightsAndDamageSprites

        private void OnHit(CarCollider.HitDirection hitDirection)
        {
            var movementValues = carMovement.MovementValues;

            switch (movementValues.Item4)
            {
                case CarMovement.MovementDirection.Forward:
                    if (movementValues.Item1 < movementValues.Item2 * 0.45f) return;
                    break;
                case CarMovement.MovementDirection.Backward:
                    if (movementValues.Item1 < movementValues.Item3 * 0.45f) return;
                    break;

                default:
                    return;
            }

            //Debug.Log(hitDirection.ToString());
            switch (hitDirection)
            {
                case CarCollider.HitDirection.FontLeft:
                   
                    EnableDisableCarLights(false, 0);
                    EnableDisableCarDamage(true, 0);
                    break;
                case CarCollider.HitDirection.FrontRight:
                    EnableDisableCarLights(false, 1);
                    EnableDisableCarDamage(true, 1);
                    break;
                case CarCollider.HitDirection.BackLeft:
                    EnableDisableCarLights(false, 2);
                    EnableDisableCarDamage(true, 2);
                    break;
                case CarCollider.HitDirection.BackRight:
                    EnableDisableCarLights(false, 3);
                    EnableDisableCarDamage(true, 3);
                    break;
                default:
                    break;
            }
        }

        private void EnableDisableCarLights(bool enabled)
        {
            foreach (var sp in lightSpriteRenderers)
            {
                sp.enabled = enabled;
            }
        }

        private void EnableDisableCarLights(bool enabled, int index)
        {
            if (index < 0 || index > lightSpriteRenderers.Length) return;
            lightSpriteRenderers[index].enabled = enabled;
        }

        private void EnableDisableCarDamage(bool enabled)
        {
            foreach (var sp in damageSpriteRenderers)
            {
                sp.enabled = enabled;
            }
        }

        private void EnableDisableCarDamage(bool enabled, int index)
        {
            if (index < 0 || index > damageSpriteRenderers.Length) return;
            damageSpriteRenderers[index].enabled = enabled;
        }

        #endregion


        private void StartEngine()
        {
            if (!isParked)
            {
                ChangeActivationAndMovementStatus(true, playerControlled);
                return;
            }


            audioEvents.PlayAudioEventOneShotAttached("StartEngine1", gameObject);
            isParked = false;
            EnableDisableCarLights(true);
            ChangeActivationAndMovementStatus(true, playerControlled);

        }

        public void ChangeActivationAndMovementStatus(bool active, bool playerControlled)
        {
            isActive = active;
            carMovement.SetActive(playerControlled, isActive);         
        }

        private void SetMainSprite(Sprite sprite, Color color)
        {
            spriteRendererMain.color = color;
            spriteRendererMain.sprite = sprite;
        }

        public void EnteredWorkshop(WorkshopType type)
        {
            switch (type)
            {
                case WorkshopType.Invalid:
                    break;
                case WorkshopType.ColorChange:
                    int rndIndex = Util.RandomIntNumber(0, Game.Instance.carColors.Length);
                    Color newColor = Game.Instance.carColors[rndIndex];
                    SetCarColor(newColor);
                    EnableDisableCarDamage(false);
                    EnableDisableCarLights(true);
                    break;
                case WorkshopType.Bomb:
                    break;
                case WorkshopType.Macgun:
                    break;
                case WorkshopType.Oil:
                    break;
                case WorkshopType.Mines:
                    break;
                default:
                    break;
            }
        }

    }
}