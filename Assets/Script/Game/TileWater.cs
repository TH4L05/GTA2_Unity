/// <author>Thoams Krahl</author>

using UnityEngine;
using ProjectGTA2_Unity.Weapons;


namespace ProjectGTA2_Unity.Tiles
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

