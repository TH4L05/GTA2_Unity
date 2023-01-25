/// <author>Thoams Krahl</author>

using UnityEngine;
using ProjectGTA2_Unity.Cars;

namespace ProjectGTA2_Unity
{
    public enum WorkshopType
    {
        Invalid = -1,
        ColorChange,
        Bomb,
        Macgun,
        Oil,
        Mines,
    }

    public class CarWorkshop : MonoBehaviour
    {
        [SerializeField] protected bool isEnabled;
        [SerializeField] protected WorkshopType workshopType = WorkshopType.Invalid;
        [SerializeField] protected LayerMask playerLayer;
        [SerializeField] protected float pauseTime = 2f;

        private bool onPause;
        private float currentTime;
        private GameObject lastEnteredObj;


        private void OnTriggerEnter(Collider collider)
        {
            if (!isEnabled) return;
            if (!collider.CompareTag("Player")) return;
            lastEnteredObj = collider.gameObject;
            OnEnterWorkshop(collider);
        }

        private void OnTriggerExit(Collider collider)
        {
            if (!isEnabled) return;
            if (lastEnteredObj == null) return;
            if (collider.gameObject.name != lastEnteredObj.name) return;
            onPause = false;
            lastEnteredObj = null;
        }

        protected virtual void OnEnterWorkshop(Collider collider)
        {
            if (!isEnabled) return;
            if (onPause) return;
            onPause = true;

            Car car = collider.GetComponent<Car>();
            if(car == null) return;
            car.EnteredWorkshop(workshopType);         
        }
    }
}

