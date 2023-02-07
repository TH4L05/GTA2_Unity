/// <author>Thoams Krahl</author>

using System;
using System.Collections.Generic;
using UnityEngine;
using ProjectGTA2_Unity.Collectables;
using ProjectGTA2_Unity.Weapons;

namespace ProjectGTA2_Unity.Characters
{
    public class Player : Character
    {
        public static Action<DamageType,string> OnDeath;
        public static Action<float,float> OnHealthChanged;
        public static Action<int> OnUpdateMoney;

        [SerializeField] private ArmouryPlayer armoury;
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private int money;

        #region UnityFunctions

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                armoury.AttackWithCurrentEquippedWeapon();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                CheckNearbyCarsToEnter();
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                audioEvents.PlayAudioEventOneShot("BurpFart");
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, CarEnterDistance);
        }
        
        #endregion

        #region Money

        public void IncreaseMoney(int amount)
        {
            money += amount;

            if(money > int.MaxValue)
            {
                money = int.MaxValue;
            }
            OnUpdateMoney?.Invoke(money);

        }

        public void DecreaseMoney(int amount)
        {
            money -= amount;

            if (money < 0)
            {
                money = 0;
            }
            OnUpdateMoney?.Invoke(money);
        }

        #endregion

        #region Setup

        protected override void StartSetup()
        {
            playerMovement.SetCharacterData(charData);
            Collectable.CollectableGathered += CollectableGathered;
            PlayerCamera.SetCameraTarget(transform);
            OnUpdateMoney?.Invoke(money);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        #endregion

        private void CollectableGathered(CollectableTypes collectableType, int amount, float time)
        {
            armoury.AddAmmo(collectableType.ToString(), amount);                
        }

        protected override void DecreaseHealth(float amount)
        {
            base.DecreaseHealth(amount);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        protected override void IncreaseHealth(float amount)
        {
            base.IncreaseHealth(amount);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        protected override void Death()
        {
            base.Death();
            OnDeath?.Invoke(lastDamageType, gameObject.name);           
        }
    }
}

