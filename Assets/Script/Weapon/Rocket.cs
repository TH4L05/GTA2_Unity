/// <author>Thoams Krahl</author>

using UnityEngine;

namespace ProjectGTA2_Unity.Weapons.Projectiles
{
    public class Rocket : Projectile
    {
        protected override void PlayImpactSound(Collider collider)
        {
            if (audioEventListSO != null) audioEventListSO.PlayAudioEventOneShotAttached("Explosion", collider.gameObject);
        }
    }
}


