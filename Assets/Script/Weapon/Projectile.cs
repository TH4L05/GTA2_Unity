using Project11;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectGTA2_Unity.Characters;

namespace ProjectGTA2_Unity
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] protected float lifeTime = 2.5f;
        [SerializeField] protected DamageType damageType;
        [SerializeField] protected float speed = 20f;
        [SerializeField] protected float damage = 20f;
        public float Speed { private get; set; }

        private void OnEnable()
        {
            Destroy(gameObject, lifeTime);
        }

        public void SetValues(float damage, DamageType damageType)
        {
            this.damage = damage;
            this.damageType = damageType;
        }

        void Update()
        {
            MoveProjectile();
        }

        public virtual void MoveProjectile()
        {

            if (Speed != 0)
            {
                speed = Speed;
            }

            //rbody.AddForce(transform.forward * speed, ForceMode.VelocityChange);
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            /*if (rbody != null)
            {
                transform.forward = Vector3.Lerp(transform.forward, rbody.velocity, Time.deltaTime);
            }*/
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("ProjectileCollide");
            Destroy(gameObject);

            var character = other.GetComponent<Character>();

            if (character)
            {
                character.TakeDamage(damage, damageType);
            }

        }
    }
}

