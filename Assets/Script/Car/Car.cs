/// <author>Thoams Krahl</author>

using UnityEngine;
using ProjectGTA2_Unity.Characters;

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
        [SerializeField] protected new string name;
        [SerializeField] protected CarType carType = CarType.Invalid;
        [SerializeField] protected SpriteRenderer sr;
        [SerializeField] protected bool fixedCarColor = false;
        [SerializeField] protected Color carColor;

        [SerializeField] protected Sprite destroyedCar;
        [SerializeField] protected float maxHealth;

        [SerializeField] protected CarMovement carMovementComponent;
        [SerializeField] protected Transform[] carEntryPoints;
        [SerializeField] protected GameObject[] destroyVFX;

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
            carColor = Game.Instance.GetRandomCarColor();
            if (sr != null) sr.color = carColor;           
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
            this.player = player;
            rb.isKinematic = false;
            player.gameObject.SetActive(false);
            isActive = true;
            carMovementComponent.SetActive(isActive);
            PlayerCamera.SetCameraTarget(transform);
        }

        public void PlayerExit()
        {
            isActive = false;
            carMovementComponent.SetActive(isActive);
            rb.isKinematic = true;            
            CheckCarExit();
            player.gameObject.SetActive(true);
            PlayerCamera.SetCameraTarget(player.transform);
        }

        private void CheckCarExit()
        {
            player.gameObject.transform.position = carEntryPoints[0].position;
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
            sr.sprite = destroyedCar;
            sr.color = Color.white;
            rb.isKinematic = true;
            Destroy(gameObject, 60f);

            if (destroyVFX.Length == 0) return;

            foreach (var vfx in destroyVFX)
            {
                var newDestroyVFX = Instantiate(vfx, transform.position, Quaternion.Euler(90f, 0f, 0f));
            }
        }


        

        

    }
}

