using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectGTA2_Unity.Characters.AI;
using UnityEngine.AI;

namespace ProjectGTA2_Unity
{
    public class NonPlayerStateMachine : StateMachine
    {
        [HideInInspector] public IdleState idleState;
        [HideInInspector] public WalkingState walkingState;
        [HideInInspector] public RunningState runningState;

        public Transform randomTargetTransform;
        public NavMeshAgent navAgent;
        public CharacterData charData;

        private void Awake()
        {
            idleState = new IdleState("idle",this);
            walkingState = new WalkingState("walk", this);
            runningState = new RunningState("run", this);
        }

        protected override BaseState GetInitialState()
        {
            return idleState;
        }
    }
}
