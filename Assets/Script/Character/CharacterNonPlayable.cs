

using UnityEngine;

namespace ProjectGTA2_Unity.Characters
{
    public class CharacterNonPlayable : Character
    {
        [SerializeField] protected SpriteRenderer sr;
        [SerializeField] private BoxCollider boxCollider;
        public Sprite deathSprite;

        protected override void Death()
        {
            base.Death();
            boxCollider.enabled = false;    
            rb.isKinematic = true;
            sr.sortingOrder = 1;
            sr.sprite = deathSprite;
            Destroy(gameObject, 5f);
        }
    }
}

