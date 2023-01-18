/// <author>Thoams Krahl</author>

using UnityEngine;
using ProjectGTA2_Unity.Characters;
using Unity.Burst.CompilerServices;

namespace ProjectGTA2_Unity
{
    public class Projectile : MonoBehaviour
    {    
        [SerializeField] protected float lifeTime = 2.5f;
        [SerializeField] protected DamageType damageType;
        [SerializeField] protected float speed = 20f;
        [SerializeField] protected float damage = 1f;
        [SerializeField] protected AudioEventList audioEventList;
        [SerializeField] protected GameObject impactVFX;

        public float Speed { get; set; }



        protected void OnEnable()
        {
            Destroy(gameObject, lifeTime);
        }

        public void SetValues(float damage, DamageType damageType)
        {
            this.damage = damage;
            this.damageType = damageType;
        }

        protected void Update()
        {
            MoveProjectile();
        }

        protected virtual void MoveProjectile()
        {
            if (speed == 0f) Destroy(gameObject);
            
            //rbody.AddForce(transform.forward * speed, ForceMode.VelocityChange);
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            /*if (rbody != null)
            {
                transform.forward = Vector3.Lerp(transform.forward, rbody.velocity, Time.deltaTime);
            }*/
        }

        protected void OnTriggerEnter(Collider collider)
        {
            Debug.Log("ProjectileCollide");

            var damageableTarget = collider.GetComponent<IDamagable>();
            if (damageableTarget != null) damageableTarget.TakeDamage(damage, damageType, gameObject.tag);

            PlayImpactSound(collider);
            CreateImpactVFX();
            Destroy(gameObject);
        }

        protected virtual void PlayImpactSound(Collider collider)
        {
            var character = collider.GetComponent<Character>();
            var car = collider.GetComponent<Car>();
            var tile = collider.GetComponent<Tile>();


            if (character)
            {

            }
            else if (car)
            {
                audioEventList.PlayAudioEventOneShot("BulletCarImpact");
            }
            else
            {
                var impactSurface = tile.GetSurfaceType();
                switch (impactSurface)
                {
                    case SurfaceType.Invalid:
                        break;

                    case SurfaceType.Normal:
                        audioEventList.PlayAudioEventOneShot("BulletWallImpact");
                        break;

                    case SurfaceType.Grass:
                        audioEventList.PlayAudioEventOneShot("BulletWallImpact");
                        break;

                    case SurfaceType.Metal:
                        audioEventList.PlayAudioEventOneShot("BulletWallImpact");
                        break;

                    default:
                        break;
                }
            }
            
        }

        protected virtual void CreateImpactVFX()
        {
            if (impactVFX == null) return;
            var newImpactVfx = Instantiate(impactVFX, transform.position, Quaternion.Euler(90f,0f,0f));
        }

    }
}

