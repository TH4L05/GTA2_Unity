using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectGTA2_Unity.Characters.AI;
using UnityEngine.AI;

namespace ProjectGTA2_Unity
{
    public class WalkingState : BaseState
    {
        private NonPlayerStateMachine sm;


        public WalkingState(string name, NonPlayerStateMachine stateMachine) : base(name, stateMachine)
        {
            sm = stateMachine;          
        }

        public override void Enter()
        {
            sm.navAgent.isStopped = false;
            sm.navAgent.stoppingDistance = 1f;
            sm.navAgent.speed = sm.charData.Walkspeed;
        }

        public override void UpdateState()
        {
            sm.navAgent.SetDestination(sm.randomTargetTransform.position);
        }

        public override void Exit()
        {
            base.Exit();
        }
    }

}
