/// <author>Thoams Krahl</author>

using System;
using UnityEngine;

namespace ProjectGTA2_Unity
{
    public enum DamageType
    {
        Invalid = -1,
        Normal,
        Fire,
        Water,
        Electro,
    }

    public enum AttackType
    {
        Invalid = -1,
        Melee,
        Throw,
        ShootRaycast,
        ShootProjectile,
        Beam,
    }

    public class Weapon : MonoBehaviour
    {
        public static Action NoAmmoLeft;
        public static Action ShootWeapon;

        #region SerializedFields

        [SerializeField] private new string name;
        [SerializeField] private DamageType damageType;
        [SerializeField] private AttackType attackType;
        [SerializeField] private bool active = true;

        [SerializeField] private int damage;
        [SerializeField] private int currentAmmo;
        [SerializeField] private int maxAmmo;
        [SerializeField] private bool unlimtitedAmmo = false;
        [SerializeField] private float attackRate;

        [SerializeField] private GameObject projectile;
        [SerializeField] private Sprite icon;
        [SerializeField] private AudioEventList audioEventList;
        [SerializeField] private Transform projectileSpawn;

        #endregion

        #region PrivateFields

        private float shootTimer;
        private bool canShoot = true;

        #endregion

        #region PublicFields

        public string Name => name;
        public bool Active => active;
        public AttackType AttackType => attackType;
        public int CurrentAmmo => currentAmmo;
        public Sprite Icon => icon;

        #endregion

        #region UnityFunctions

        private void Awake()
        {
            shootTimer = 0f;          
        }

        private void Update()
        {
            //Debug.DrawRay(raycastStart.position, raycastStart.TransformDirection(Vector3.forward) * 300, Color.red);
            if(!active) return;
            shootTimer += Time.deltaTime;
        }

        #endregion

        #region Ammo

        public void DecreaseAmmo(int amount)
        {
            if (unlimtitedAmmo) return;
            currentAmmo -= amount;

            if (currentAmmo <= 0)
            {
                currentAmmo = 0;
                active = false;
            }

        }

        public void IncreaseAmmo(int amount) 
        {
            currentAmmo += amount;
            active = true;

            if (currentAmmo > maxAmmo)
            {
                currentAmmo = maxAmmo;
            }
        }

        #endregion

        public void SetActive(bool active)
        {
            this.active = active;
        }

        public void Attack()
        {
            var rof = 1 / (attackRate / 60);

            if (currentAmmo < 1) return;

            if (canShoot && shootTimer >= rof)
            {
                shootTimer = 0.0f;
                if(audioEventList != null) audioEventList.PlayAudioEventOneShot(name + "Attack");

                switch (attackType)
                {
                    case AttackType.Invalid:
                    default:
                        Debug.Log("ERROR: Invalid shoot type !!");
                        return;
                    case AttackType.Melee:
                        break;

                    case AttackType.ShootRaycast:
                        //RaycastShoot();
                        DecreaseAmmo(1);
                        break;
                    case AttackType.ShootProjectile:
                        ProjectileShoot();
                        DecreaseAmmo(1);
                        break;
                    case AttackType.Beam:
                        break;
                }

                //Debug.Log("weapon " + name + " shoot");
            }
        }

        private void MeleeAttack()
        {

        }

        /*private void RaycastShoot()
        {
            Vector3 direction = raycastStart.forward;
            Ray ray = new Ray(raycastStart.position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, raycastRange))
            {
                Debug.Log($"<color=lime>RaycastShot hit = {hit.collider.name}</color>");
                NewImpactVfx(hit);
                DamageTargetOnHit(hit);

                if (hit.collider.CompareTag("ShootTarget"))
                {
                    var target = hit.collider.GetComponent<Target>();
                    target.TargetGetHit();
                }
            }
        }*/

        /*private void DamageTargetOnHit(RaycastHit hit)
        {
            var damageableTarget = hit.collider.GetComponent<IDamageable>();
            if (damageableTarget == null) return;          
            damageableTarget.TakeDamage(damage);
        }*/

        private void ProjectileShoot()
        {
            var newProjectile = Instantiate(projectile, projectileSpawn.position, Game.Instance.player.transform.rotation);
            newProjectile.GetComponent<Projectile>().SetValues(damage, damageType);         
        }

    }
}

