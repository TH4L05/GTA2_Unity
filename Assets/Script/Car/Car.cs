/// <author>Thoams Krahl</author>

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
        public static Action<string> PlayerEntersCar;
        public UnityEvent OnCarGetDestroyed;

        [SerializeField] protected new string name;
        [SerializeField] protected CarType carType = CarType.Invalid;
        [SerializeField] protected SpriteRenderer sr;
        [SerializeField] protected bool fixedCarColor = false;
        [SerializeField] protected Color carColor;

        [SerializeField] protected float maxHealth;
        [SerializeField] protected CarMovement carMovementComponent;
        [SerializeField] protected Transform[] carEntryPoints;
        [SerializeField] protected GameObject[] destroyVFX;
        [SerializeField] protected SpriteRenderer[] damageSpriteRenderers;
        [SerializeField] protected LayerMask groundLayer;

        protected Player player;
        protected Rigidbody rb;
        protected bool isActive = false;     
        protected float currentHealth;
        protected bool isDestroyed;


        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            rb.isKinematic = true;
            currentHealth = maxHealth;
            if (fixedCarColor) return;
            GetRandomColor();
            SetCarColors(carColor);
        }

        private void Update()
        {
            if(!isActive) return;

            if (Input.GetKeyDown(KeyCode.E))
            {
                PlayerExit();
            }
        }

        public void PlayerEnters(Player player)
        {         
            if(isDestroyed) return;

            this.player = player;
            rb.isKinematic = false;
            player.gameObject.SetActive(false);
            isActive = true;
            carMovementComponent.SetActive(isActive);
            PlayerCamera.SetCameraTarget(transform);
            gameObject.tag = "Player";
            PlayerEntersCar?.Invoke(name);
        }

        public void PlayerExit()
        {
            isActive = false;
            carMovementComponent.SetActive(isActive);
            rb.isKinematic = true;            
            CheckCarExit();
            player.gameObject.SetActive(true);
            PlayerCamera.SetCameraTarget(player.transform);
            gameObject.tag = "Car";
        }

        private void CheckCarExit()
        {
            // TODO: Check if Exit is blocked use other Point when Point 0 is not avialable
            player.gameObject.transform.position = carEntryPoints[0].position;
        }

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

        public void TakeDamage(float damageAmount, DamageType damageType)
        {
            if (isDestroyed) return;

            DecreaseHealth(damageAmount);
            Debug.Log($"<color=orange>{gameObject.name} takes damage - {damageAmount.ToString("0")}</color>");
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
            sr.color = Color.white;

            OnCarGetDestroyed?.Invoke();
            Destroy(gameObject, 60f);

            if (destroyVFX.Length == 0) return;

            foreach (var vfx in destroyVFX)
            {
                var newDestroyVFX = Instantiate(vfx, transform.position, Quaternion.Euler(90f, 0f, 0f));
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
    }
}

