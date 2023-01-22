using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectGTA2_Unity.Characters.AI
{
    public class StateMachine : MonoBehaviour
    {
        protected BaseState currentState;
        public string currentStateName;

        private void Start()
        {
            currentState = GetInitialState();
            if (currentState != null)
            {
                currentState.Enter();
                currentStateName = currentState.GetStateName();
            }

        }

        private void Update()
        {
            if (currentState != null)
            {
                currentState.UpdateState();
                currentStateName = currentState.GetStateName();
            }

        }

        public void ChangeState(BaseState state)
        {
            currentState.Exit();
            currentState = state;
            currentState.Enter();
            currentStateName = currentState.GetStateName();
        }

        protected virtual BaseState GetInitialState()
        {
            return null;
        }

        protected virtual BaseState GetState(string stateName)
        {
            return null;
        }

        public virtual void ParseMono(BaseState state)
        {
            state.ParseMono(this);
        }

        public virtual void ShowMessage(string message)
        {
            Debug.Log(message);
        }
    }
}

