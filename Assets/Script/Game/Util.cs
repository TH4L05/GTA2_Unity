using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectGTA2_Unity
{
    public class Util : MonoBehaviour
    {
        public static int RandomIntNumber(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        public static float RandomFloatNumber(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }
    }
}

