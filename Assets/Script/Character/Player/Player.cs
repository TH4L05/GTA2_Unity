/// <author>Thoams Krahl</author>

using System;
using System.Collections.Generic;
using UnityEngine;
using ProjectGTA2_Unity.Collectables;
using ProjectGTA2_Unity.Weapons;
using ProjectGTA2_Unity.Input;
using UnityEngine.InputSystem;

namespace ProjectGTA2_Unity.Characters
{
    public class Player : Character
    {
        public static Action<DamageType,string> OnDeath;
        public static Action<int> OnUpdateMoney;

        [Header("Player")]
        [SerializeField] private ArmouryPlayer armoury;
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private int money;

        #region UnityFunctions

        private void Update()
        {
            if (InputHandler.Instance.ShootInputPressed)
            {
                armoury.AttackWithCurrentEquippedWeapon();
            }

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                CheckNearbyCarsToEnter();
            }

            if (Keyboard.current.tabKey.wasPressedThisFrame)
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
            playerMovement.SetActive(true);
           
        }

        #endregion

        private void CollectableGathered(CollectableTypes collectableType, int amount, float time)
        {
            armoury.AddAmmo(collectableType.ToString(), amount);                
        }

        protected override void Death()
        {
            base.Death();
            playerMovement.SetActive(false);
            OnDeath?.Invoke(lastDamageType, gameObject.name);           
        }
    }
}

