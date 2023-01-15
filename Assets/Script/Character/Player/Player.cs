/// <author>Thoams Krahl</author>

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectGTA2_Unity.Characters
{
    public class Player : Character
    {
        public static Action<DamageType> PlayerDied;

        [SerializeField] private WeaponBelt weaponBelt;
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private float CarEnterDistance = 2f;

        #region UnityFunctions

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                weaponBelt.WeaponAttack();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                CheckNearbyCarsToEnter();
            }

        }

        #endregion

        protected override void AdditionalSetup()
        {
            base.AdditionalSetup();
            playerMovement.SetCharacterData(charData);
            Pickup.PickupCollected += PickupColleted;
            PlayerCamera.SetCameraTarget(transform);         
        }

        

        private void CheckNearbyCarsToEnter()
        {
            Collider[] carColliders = Physics.OverlapSphere(transform.position, CarEnterDistance, 1 << 8 );
           
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

        


        private void PickupColleted(PickupType pickupType, int amount)
        {
            Debug.Log("PickUpCollected");
            weaponBelt.AddAmmo(pickupType.ToString(), amount);                
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, CarEnterDistance);
        }

        protected override void Death()
        {
            base.Death();
            PlayerDied?.Invoke(lastDamageType);
            Destroy(gameObject);
        }
    }
}

