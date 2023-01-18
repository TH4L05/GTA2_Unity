/// <author>Thoams Krahl</author>

using UnityEngine;
using FMODUnity;

namespace ProjectGTA2_Unity
{
    public class PlayerCamera : MonoBehaviour
    {
        public static Transform targetObj;
        [SerializeField] private Vector3 targetOffset;
        [SerializeField] private Vector3 targetOffsetMax;
        [SerializeField] private float speed;
        [SerializeField] private static StudioListener sl;

        private void Awake()
        {
            sl = GetComponent<StudioListener>();
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

        public void SetTargetOffset(Vector3 targetCamOffset)
        {
            targetOffset = targetCamOffset;
        }

        public void SetTargetOffset(float x, float y, float z)
        {         
            targetOffset = new Vector3(x, y, z);
        }

        public void SetTargetXOffset(float x)
        {
            targetOffset = new Vector3(x, targetOffset.y, targetOffset.z);
        }

        public void SetTargetYOffset(float y)
        {
            targetOffset = new Vector3(targetOffset.x, y, targetOffset.z);
        }

        public void SetTargetZOffset(float z)
        {
            targetOffset = new Vector3(targetOffset.x, targetOffset.y,z);
        }

        private void MoveCameraWithTarget()
        {
            if (targetObj == null)
            {
                Debug.LogError("Camera Target is ´Missing !!");
                return;
            }

            transform.position = Vector3.Lerp(transform.position, targetObj.position + targetOffset, speed * Time.deltaTime);
 
        }
    }
}

