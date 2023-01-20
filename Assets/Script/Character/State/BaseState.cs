using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectGTA2_Unity.Characters.AI
{
    public class BaseState
    {
        [SerializeField] protected string name;
        protected StateMachine stateMachine;

        public BaseState(string name, StateMachine stateMachine)
        {
            this.name = name;
            this.stateMachine = stateMachine;
        }

        public string GetStateName()
        {
            return name;
        }

        public virtual void Enter() { }

        public virtual void Exit() { }

        public virtual void UpdateState() { }
    }
}

