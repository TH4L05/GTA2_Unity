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
        public LayerMask groundLayer;
        public Vector3 destination;
        public Vector3 direction;
        public Quaternion desiredRotation;
        public List<Tile> groundTiles = new List<Tile>();
        public LineRenderer lineRenderer;
        public Color gizmoColor;

        public float _stoppingDistance = 1.5f;

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

        public override void ParseMono(BaseState state)
        {
            base.ParseMono(state);
        }

        public override void ShowMessage(string message)
        {
            base.ShowMessage(message);
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
