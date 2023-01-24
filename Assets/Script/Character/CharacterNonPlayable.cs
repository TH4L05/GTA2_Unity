
using UnityEngine;
/// <author>Thoams Krahl</author>

namespace ProjectGTA2_Unity.Characters
{
    public class CharacterNonPlayable : Character
    {
        [SerializeField] protected NonPlayableBehaviour npcBehaviour;

        protected override void Death()
        {
            base.Death();
            npcBehaviour.IsDead = true;
            Destroy(gameObject, deletionTime);
        }
    }
}

