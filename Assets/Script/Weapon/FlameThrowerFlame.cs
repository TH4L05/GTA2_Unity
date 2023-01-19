using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectGTA2_Unity
{
    public class FlameThrowerFlame : Projectile
    {
        protected override void Setup()
        {
            base.Setup();
            transform.Rotate(new Vector3(90f, 0, 0), Space.Self);
        }

        protected override void MoveProjectile()
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }
    }
}

