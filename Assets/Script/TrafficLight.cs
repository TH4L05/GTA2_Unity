/// <author>Thoams Krahl</author>

using System;
using UnityEngine;
using ProjectGTA2_Unity.Cars;
using System.Collections.Generic;

namespace ProjectGTA2_Unity
{
    public class TrafficLight : MonoBehaviour
    {
        public enum TrafficLightState
        {
            Disabled,
            Green,
            Red,
        }

        [SerializeField] private TrafficLightState state = TrafficLightState.Green;
        [SerializeField] private SpriteRenderer lightSpriteRenderer;
        [SerializeField] private Sprite redLight;
        [SerializeField] private Sprite greenLight;
        public List<Car> carsInZone = new List<Car>();
     
        public void ChangeState(TrafficLightState trafficLightState)
        {
            state = trafficLightState;

            switch (state)
            {
                case TrafficLightState.Disabled:
                    if (lightSpriteRenderer != null) lightSpriteRenderer.sprite = null;
                    break;

                case TrafficLightState.Red:
                    if (lightSpriteRenderer != null) lightSpriteRenderer.sprite = redLight;
                    break;

                case TrafficLightState.Green:
                    if (lightSpriteRenderer != null) lightSpriteRenderer.sprite = greenLight;
                    break;
            }

            foreach (var car in carsInZone)
            {
                car.CarMovement.OnTrafficLightJunction(state);
            }
        }

        public void ChangeState(int stateIndex)
        {
            if (stateIndex < 0 || stateIndex > Enum.GetValues(typeof(TrafficLightState)).Length - 1) return;
            state = (TrafficLightState)stateIndex;

            switch (state)
            {
                case TrafficLightState.Disabled:
                    if (lightSpriteRenderer != null) lightSpriteRenderer.sprite = null;
                    break;

                case TrafficLightState.Red:
                    if (lightSpriteRenderer != null) lightSpriteRenderer.sprite = redLight;
                    break;

                case TrafficLightState.Green:
                    if (lightSpriteRenderer != null) lightSpriteRenderer.sprite = greenLight;
                    break;
            }

            foreach (var car in carsInZone)
            {
                car.CarMovement.OnTrafficLightJunction(state);
            }

        }

        public TrafficLightState GetCurrentState()
        {
            return state;
        }


        private void OnTriggerEnter(Collider collider)
        {
            
            if (!collider.CompareTag("Car")) return;
            Debug.Log("car in Zone");

            var car = collider.GetComponent<Car>();

            if(car == null) return;
            if (car.IsDestroyed || car.IsParked || car.IsPlayerControlled) return;

            foreach (var carInZone in carsInZone)
            {
                if (carInZone.gameObject.name == car.gameObject.name) return;
            }

            carsInZone.Add(car);
            car.CarMovement.OnTrafficLightJunction(state);
        }

        private void OnTriggerStay(Collider collider)
        {

            if (!collider.CompareTag("Car")) return;
            //Debug.Log("car in Zone");

            var car = collider.GetComponent<Car>();

            if (car == null) return;
            if (car.IsDestroyed || car.IsParked || car.IsPlayerControlled) return;

            foreach (var carInZone in carsInZone)
            {
                if (carInZone.gameObject.name == car.gameObject.name) return;
            }

            carsInZone.Add(car);
            car.CarMovement.OnTrafficLightJunction(state);
        }

        private void OnTriggerExit(Collider collider)
        {
            if (!collider.CompareTag("Car")) return;
            var car = collider.GetComponent<Car>();
            if (car == null) return;

            foreach (var carInZone in carsInZone)
            {
                if (carInZone.gameObject.name == car.gameObject.name)
                {
                    carsInZone.Remove(carInZone);
                    return;
                }
            }
        }

    }
}

