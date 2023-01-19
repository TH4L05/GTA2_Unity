/// <author>Thoams Krahl</author>

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectGTA2_Unity
{
    public class WeaponBelt : MonoBehaviour
    {
        public static Action<Sprite, int> WeaponChanged;
        public static Action<int> WeaponUpdate;


        [SerializeField] private List<Weapon> weapons = new List<Weapon>();

        private int currentweaponIndex = 0;
        public bool GunEquipped { get; private set ;}


        private void Awake()
        {
            Weapon.NoAmmoLeft += SetWeaponIndexZero;
            
        }

        private void Start()
        {
            SetWeaponIndexZero();
        }

        private void OnDestroy()
        {
            Weapon.NoAmmoLeft -= SetWeaponIndexZero;
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Y))
            {
                PreviousWeapon();
            }
            else if(Input.GetKeyUp(KeyCode.X))
            {
                NextWeapon();
            }
        }

        public void WeaponAttack()
        {
            weapons[currentweaponIndex].Attack();
            if(currentweaponIndex != 0) WeaponUpdate?.Invoke(weapons[currentweaponIndex].CurrentAmmo);
        }


        private void SetWeaponIndexZero()
        {
            currentweaponIndex= 0;
            WeaponChanged?.Invoke(weapons[currentweaponIndex].Icon, weapons[currentweaponIndex].CurrentAmmo);
        }

        public void NextWeapon()
        {
            currentweaponIndex++;

            if (currentweaponIndex > weapons.Count -1)
            {
                currentweaponIndex = 0;
            }

            if (!weapons[currentweaponIndex].Active)
            {
                NextWeapon();
                return;
            }

            GunEquippedCheck();

            WeaponChanged?.Invoke(weapons[currentweaponIndex].Icon, weapons[currentweaponIndex].CurrentAmmo);
            Debug.Log($"<color=orange>Weapon {weapons[currentweaponIndex].name} selected</color>");

        }

        public void PreviousWeapon()
        {
            currentweaponIndex--;

            if (currentweaponIndex < 0)
            {
                currentweaponIndex = weapons.Count - 1; 
            }

            if (!weapons[currentweaponIndex].Active)
            {
                PreviousWeapon();
                return;
            }

            GunEquippedCheck();
            WeaponChanged?.Invoke(weapons[currentweaponIndex].Icon, weapons[currentweaponIndex].CurrentAmmo);
            Debug.Log($"<color=orange>Weapon {weapons[currentweaponIndex].name} selected</color>");
        }

        private void GunEquippedCheck()
        {
            
            if (weapons[currentweaponIndex].AttackType == AttackType.ShootProjectile)
            {
                GunEquipped = true;
            }
            else
            {
                GunEquipped = false;
            }
        }

        public void AddAmmo(string weaponName, int amount)
        {
            if (string.IsNullOrEmpty(weaponName))
            {
                Debug.Log("Error");
                return;
            }

            int index = 0;
            foreach (var equippedWeapon in weapons)
            {
                if (equippedWeapon.Name == weaponName)
                {
                    if (!equippedWeapon.Active)
                    {
                        if (currentweaponIndex == 0) currentweaponIndex = index;                       
                        equippedWeapon.SetActive(true);
                        WeaponChanged?.Invoke(weapons[currentweaponIndex].Icon, weapons[currentweaponIndex].CurrentAmmo);
                    }
               
                    equippedWeapon.IncreaseAmmoMagazine(amount);
                    WeaponUpdate?.Invoke(equippedWeapon.CurrentAmmo);
                    return;
                }
                index++;
            }          
        }
    }
}

