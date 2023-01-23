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

        public static Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
        {
            //origin from bottom left to top left
            screenPosition1.y = Screen.height - screenPosition1.y;
            screenPosition2.y = Screen.height - screenPosition2.y;

            //corners
            var topLeft = Vector3.Min(screenPosition1, screenPosition2);
            var bottomRight = Vector3.Max(screenPosition1, screenPosition2);

            Rect rect = Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);

            return rect;
        }

        public static float GetDistance(Vector3 vec1, Vector3 vec2)
        {
            float distance = Vector3.Distance(vec1, vec2);
            return distance;
        }
    }
}

