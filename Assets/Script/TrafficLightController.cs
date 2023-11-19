/// <author>Thoams Krahl</author>

using System.Collections;
using UnityEngine;

namespace ProjectGTA2_Unity
{
    public class TrafficLightController : MonoBehaviour
    {
        [SerializeField] private bool isEnabled = true;
        [SerializeField] private bool upDownRedOnStart = true;
        [SerializeField] private TrafficLight[] upDownLights;
        [SerializeField] private TrafficLight[] leftRightLights;
        [SerializeField] private float phaseDuration = 5f;

        private float currentTime = 0f;
        private TrafficLight.TrafficLightState currentUpDownState;
        private TrafficLight.TrafficLightState currentLeftRightState;

        private void Start()
        {
            if(!isEnabled) return;

            if (upDownRedOnStart)
            {
                currentUpDownState = TrafficLight.TrafficLightState.Red;
                currentLeftRightState = TrafficLight.TrafficLightState.Green;
            }
            else
            {
                currentUpDownState = TrafficLight.TrafficLightState.Green;
                currentLeftRightState = TrafficLight.TrafficLightState.Red;
            }
            SetStates();
            StartCoroutine(ChangePhase());
        }

        private IEnumerator ChangePhase()
        {
            while( currentTime < phaseDuration )
            {
                currentTime += Time.deltaTime;
                yield return null;
            }
            ChangeState();
        }

        private void ChangeState()
        {
            if (!isEnabled) return;
            currentTime = 0f;

            TrafficLight.TrafficLightState lastUpDownState = currentUpDownState;
            TrafficLight.TrafficLightState lastLeftRightState = currentLeftRightState;

            currentUpDownState = lastLeftRightState; 
            currentLeftRightState = lastUpDownState;

            SetStates();       
            StartCoroutine(ChangePhase());
        }

        private void SetStates()
        {
            foreach (var light in upDownLights)
            {
                light.ChangeState(currentUpDownState);
            }

            foreach (var light in leftRightLights)
            {
                light.ChangeState(currentLeftRightState);
            }
        }
    }
}

