

using UnityEngine;

namespace ProjectGTA2_Unity.Characters
{
    public class CharacterNonPlayable : Character
    {
        [SerializeField] protected SpriteRenderer sr;
        [SerializeField] private BoxCollider boxCollider;
        [SerializeField] private Rigidbody rb;
        public Sprite deathSprite;

        protected override void Death()
        {
            base.Death();
            boxCollider.enabled = false;    
            rb.isKinematic = true;
            sr.sortingOrder = 1;
            Destroy(gameObject, 5f);

            switch (lastDamageType)
            {
                case DamageType.Invalid:
                    break;

                case DamageType.Normal:
                    sr.sprite = deathSprite;
                    break;

                case DamageType.Fire:
                    sr.sprite = deathSprite;
                    break;

                case DamageType.Water:
                    sr.enabled = false;
                    audioEvents.PlayAudioEventOneShot("Splash");
                    break;

                case DamageType.Electro:
                    sr.sprite = deathSprite;
                    break;

                case DamageType.Car:
                    audioEvents.PlayAudioEventOneShot("CarHit");
                    sr.sprite = deathSprite;
                    break;

                case DamageType.CopNormal:
                    break;

                case DamageType.CopGun:
                    break;

                default:
                    break;
            }
        }
    }
}

