

using System;
using UnityEngine;

namespace ProjectGTA2_Unity.Characters
{
    public class CharacterNonPlayable : Character
    {
        protected override void Death()
        {
            base.Death();
            Destroy(gameObject, deletionTime);
        }
    }
}

