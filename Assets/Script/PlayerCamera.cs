using UnityEngine;

namespace ProjectGTA2_Unity
{
    public class PlayerCamera : MonoBehaviour
    {
        public Transform target;
        [SerializeField] private Vector3 targetOffset;
        [SerializeField] private float speed;

        void Update()
        {
            MoveCameraWithTarget();
        }

        public void SetCameraTarget(Transform target)
        {
            this.target = target; 
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
            if (target == null)
            {
                Debug.LogError("Camera Target is ´Missing !!");
                return;
            }

            transform.position = Vector3.Lerp(transform.position, target.position + targetOffset, speed * Time.deltaTime);
 
        }
    }
}

