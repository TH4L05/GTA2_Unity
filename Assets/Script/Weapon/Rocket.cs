/// <author>Thoams Krahl</author>

using UnityEngine;
using ProjectGTA2_Unity.Characters;

namespace ProjectGTA2_Unity
{
    public class Rocket : Projectile
    {
        protected override void PlayImpactSound(Collider collider)
        {
            audioEventList.PlayAudioEventOneShot("Explosion");
        }
    }
}


