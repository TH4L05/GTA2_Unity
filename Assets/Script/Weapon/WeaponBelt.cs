/// <author>Thoams Krahl</author>

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectGTA2_Unity
{
    public class WeaponBelt : MonoBehaviour
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
            
            if (weapons[currentweaponIndex].AttackType == AttackType.ShootProjectile)
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

