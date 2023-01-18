using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectGTA2_Unity
{
    public class TileWater : Tile
    {
        private void OnTriggerEnter(Collider collider)
        {
            Debug.Log(collider.name);

            var damageable = collider.GetComponent<IDamagable>();
            if (damageable != null) damageable.TakeDamage(999f, DamageType.Water, gameObject.tag);
        }
    }
}

