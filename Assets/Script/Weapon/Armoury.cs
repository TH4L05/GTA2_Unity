/// <author>Thoams Krahl</author>

using System;
using System.Collections.Generic;
using UnityEngine;
using ProjectGTA2_Unity.Weapons;

namespace ProjectGTA2_Unity
{
    public class Armoury : MonoBehaviour
    {            
        [SerializeField] protected List<Weapon> weapons = new List<Weapon>();

        protected int currentweaponIndex = 0;
        public bool GunEquipped { get; private set ;}
    
        public  virtual void AttackWithCurrentEquippedWeapon()
        {
            weapons[currentweaponIndex].Attack();         
        }

        protected void GunEquippedCheck()
        {
            
            if (weapons[currentweaponIndex].AttackType == Weapon.AttackTypes.ShootProjectile)
            {
                GunEquipped = true;
            }
            else
            {
                GunEquipped = false;
            }
        }

        public virtual void AddAmmo(string weaponName, int amount)
        {           
        }
    }
}

