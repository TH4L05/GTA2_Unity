using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ProjectGTA2_Unity.Characters.AI;

namespace ProjectGTA2_Unity
{
    public class IdleState : BaseState
    {
        private NonPlayerStateMachine sm;

        public IdleState(string name, NonPlayerStateMachine stateMachine) : base(name, stateMachine)
        {
            sm = stateMachine;
        }

        public override void Enter()
        {
            sm.navAgent.isStopped = true;
            sm.randomTargetTransform.position = RandomTargetPosition();
        }

        public override void UpdateState()
        {
            sm.ChangeState(sm.walkingState);
        }

        public override void Exit()
        {
            sm.navAgent.isStopped = false;
        }

        protected Vector3 RandomTargetPosition()
        {
            Vector3 position = UnityEngine.Random.insideUnitSphere * 5;
            position += sm.gameObject.transform.position;

            NavMesh.SamplePosition(position, out NavMeshHit hit, 20, 1);

            return hit.position;
        }
    }
}

