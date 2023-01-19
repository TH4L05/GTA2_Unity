/// <author>Thoams Krahl</author>

using UnityEngine;
using FMODUnity;

namespace ProjectGTA2_Unity
{
    [RequireComponent(typeof(Camera))]
    public class PlayerCamera : MonoBehaviour
    {      
        [SerializeField] private Vector3 targetOffsetStart;
        [SerializeField] private Vector3 targetOffsetMax;
        [SerializeField] private float speed;
        [SerializeField] private static StudioListener sl;

        public static Transform targetObj;
        private static Vector3 targetOffset;

        private void Awake()
        {
            sl = GetComponent<StudioListener>();
            targetOffset = targetOffsetStart;
        }

        void LateUpdate()
        {
            MoveCameraWithTarget();
        }

        public static void SetCameraTarget(Transform target)
        {
            targetObj = target;
            sl.SetAttenuationObject(targetObj.gameObject);
        }

        public static void SetTargetOffset(Vector3 targetCamOffset)
        {
            targetOffset = targetCamOffset;
        }

        public static void SetTargetOffset(float x, float y, float z)
        {         
            targetOffset = new Vector3(x, y, z);
        }

        public static void SetTargetXOffset(float x)
        {
            targetOffset = new Vector3(x, targetOffset.y, targetOffset.z);
        }

        public static void SetTargetYOffset(float y)
        {
            targetOffset = new Vector3(targetOffset.x, y, targetOffset.z);
        }

        public static void SetTargetZOffset(float z)
        {
            targetOffset = new Vector3(targetOffset.x, targetOffset.y,z);
        }

        private void MoveCameraWithTarget()
        {
            if (targetObj == null)
            {
                //Debug.LogError("Camera Target is ´Missing !!");
                return;
            }

            transform.position = Vector3.Lerp(transform.position, targetObj.position + targetOffset, speed * Time.deltaTime);
 
        }
    }
}

