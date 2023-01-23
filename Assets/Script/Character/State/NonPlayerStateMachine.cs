using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectGTA2_Unity.Characters.AI;
using ProjectGTA2_Unity.Characters.Data;
using UnityEngine.AI;

namespace ProjectGTA2_Unity
{
    public class NonPlayerStateMachine : StateMachine
    {
        [HideInInspector] public IdleState idleState;
        [HideInInspector] public WalkingState walkingState;
        [HideInInspector] public RunningState runningState;

        public List<BaseState> states= new List<BaseState>();
        public Transform randomTargetTransform;
        public NavMeshAgent navAgent;
        public CharacterData charData;
        public LayerMask groundLayer;
        public Vector3 destination;
        public Vector3 lastDestination;


        public Quaternion desiredRotation;
        public List<Tile> groundTiles = new List<Tile>();
        public Color gizmoColor;

        public float _stoppingDistance = 1.5f;
        public bool hasDestination;

        private void Awake()
        {
            idleState = new IdleState("idle",this);
            walkingState = new WalkingState("walk", this);
            runningState = new RunningState("run", this);

            states.Add(idleState);
            states.Add(walkingState);
            states.Add(runningState);
        }

        protected override BaseState GetInitialState()
        {
            return idleState;
        }

        public override void ParseMono(BaseState state)
        {
            base.ParseMono(state);
        }

        public override void ShowMessage(string message)
        {
            base.ShowMessage(message);
        }

        protected override BaseState GetState(string stateName)
        {
            foreach (var state in states)
            {
                if (state.GetStateName() == stateName)
                {
                    return state;
                }
            }
            return base.GetState(stateName);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawCube(destination, new Vector3(0.3f, 0.3f, 0.3f));

            /*Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = new Color(1f, 0f, 0f, 0.45f);
            Gizmos.DrawCube(new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.5f), new Vector3(0.5f, 1f, 1.5f));*/
        }
    }
}
