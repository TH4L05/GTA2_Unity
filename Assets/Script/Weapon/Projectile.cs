/// <author>Thoams Krahl</author>

using UnityEngine;
using ProjectGTA2_Unity.Characters;
using ProjectGTA2_Unity.Cars;

namespace ProjectGTA2_Unity
{
    public class Projectile : MonoBehaviour
    {
        #region SerializedFields

        [SerializeField] protected float lifeTime = 2.5f;
        [SerializeField] protected DamageType damageType;
        [SerializeField] protected float speed = 20f;
        [SerializeField] protected float damage = 1f;
        [SerializeField] protected AudioEventList audioEventList;
        [SerializeField] protected GameObject impactVFX;

        #endregion

        #region PrivateFields

        public float Speed { get; set; }
        private string owner;

        #endregion

        #region UnityFunctions

        protected void OnEnable()
        {
            Setup();
        }

        protected void Update()
        {
            MoveProjectile();
        }

        protected void OnTriggerEnter(Collider collider)
        {
            OnImpact(collider);
        }

        #endregion

        #region Setup

        protected virtual void Setup()
        {
            Destroy(gameObject, lifeTime);
        }

        public void SetValues(float damage, DamageType damageType, string owner )
        {
            this.damage = damage;
            this.damageType = damageType;
            this.owner = owner;
        }

        #endregion

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


        #region Impact

        protected virtual void OnImpact(Collider collider)
        {
            if (collider.gameObject.name == owner) return;
            //Debug.Log("ProjectileCollide");

            var damageableTarget = collider.GetComponent<IDamagable>();
            if (damageableTarget != null) damageableTarget.TakeDamage(damage, damageType, owner);

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
                PlayAudio("BulletCarImpact");
            }
            else
            {
                var impactSurface = tile.GetSurfaceType();
                switch (impactSurface)
                {
                    case SurfaceType.Invalid:
                        break;

                    case SurfaceType.Normal:
                        PlayAudio("BulletWallImpact");
                        break;

                    case SurfaceType.Grass:
                        PlayAudio("BulletWallImpact");
                        break;

                    case SurfaceType.Metal:
                        PlayAudio("BulletWallImpact");
                        break;

                    default:
                        break;
                }
            }
            
        }

        private void PlayAudio(string eventName)
        {
            if (audioEventList != null) audioEventList.PlayAudioEventOneShot(eventName);
        }

        protected virtual void CreateImpactVFX()
        {
            if (impactVFX == null) return;
            var newImpactVfx = Instantiate(impactVFX, transform.position, Quaternion.Euler(90f,0f,0f));
        }

        #endregion
    }
}

